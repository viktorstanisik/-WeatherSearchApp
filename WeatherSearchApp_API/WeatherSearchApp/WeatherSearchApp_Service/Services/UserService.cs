using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Security.Claims;
using System.Text.RegularExpressions;
using WeatherSearchApp_DataAccess.Interfaces;
using WeatherSearchApp_Domain.DTOModels;
using WeatherSearchApp_Domain.EntityModels;
using WeatherSearchApp_Service.Interfaces;
using WeatherSearchApp_Service.Models;
using WeatherSearchApp_Shared.AppConstants;
using WeatherSearchApp_Shared.Enums;
using WeatherSearchApp_Shared.HelperMethods;

namespace WeatherSearchApp_Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILoggedUsersInfoRepository _loggedUsersInfoRepository;
        private readonly IEmailService _emailService;
        private readonly IDistributedCache _distributedCache;


        public UserService(IUserRepository userRepository, ITokenService tokenService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper, ILoggedUsersInfoRepository loggedUsersInfoRepository, IEmailService emailService, IDistributedCache distributedCache)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _loggedUsersInfoRepository = loggedUsersInfoRepository;
            _emailService = emailService;
            _distributedCache = distributedCache;
        }

        public async Task<ServiceResponse<int>> RegisterAccount(UserDto userDtoModel)
        {
            var response = new ServiceResponse<int>();

            ServiceResponse<bool> hasError = await ValidateFieldsForRegisterAccount(userDtoModel);
            if (!hasError.Success)
            {
                response.Success = hasError.Success;
                response.ErrorMessage = hasError.ErrorMessage;
                return response;
            }

            userDtoModel.Password = Methods.GenerateSha512Hash(userDtoModel.Password);

            var user = await _userRepository.CreateUser(_mapper.Map<User>(userDtoModel));

            if (user == null)
            {
                response.ErrorMessage = ErrorMessages.GenericError;
                return response;
            }

            await _emailService.RegisterAccountConfirmationLink(user);

            response.Data = user.Id;
            response.Success = true;
            return response;

        }

        public int GetLoggedUserIdFromHttpContext()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Convert.ToInt32(userIdClaim);
        }

        public async Task<ServiceResponse<bool>> LogoutUser()
        {
            var response = new ServiceResponse<bool>() { Success = false};

            // Get logged user ID from HttpContext
            var userId = GetLoggedUserIdFromHttpContext();

            // Get user information from logged user info
            var validateUserToken = await GetUserByUserIdFromLoggedUserInfo(userId);

            if (validateUserToken == null)
            {
                response.ErrorMessage = ErrorMessages.InvalidUser;
                return response;
            }

            // Check if the user is logged in
            if (validateUserToken.LastLogin != null && validateUserToken.LoginStatusId != (int)LoginStatus.LoggedOut)
            {
                // Logout user
                var loggedUserInfo = await _loggedUsersInfoRepository.LogoutUser(userId);
                _httpContextAccessor.HttpContext.Response.Cookies.Delete(_configuration["Jwt:Name"]);

                response.Data = loggedUserInfo != null;
                response.Success = true;

                return response;
            }

            response.Data = false; // User is already logged out
            response.Success = true;

            return response;

        }


        public async Task<ServiceResponse<LoginResponseModel>> Login(LoginModel model, bool isCalledFromSecurityCodeVerification = false)
        {
            var response = new ServiceResponse<LoginResponseModel>() { ErrorMessage = "", Success = false };
            //when we are calling the Login from the Verify Security Code the user password is already hashed thats why we dont need to hash the pass
            if (!isCalledFromSecurityCodeVerification)
            {
                ServiceResponse<bool> hasError = await ValidateLoginFields(model);

                if (!hasError.Success)
                {
                    response.ErrorMessage = hasError.ErrorMessage;
                    response.Success = hasError.Success;
                    return response;
                }
            }

            LoginResponseModel loginResponse = new();

            //when we are calling the Login from the Verify Security Code the user password is already hashed thats why we dont need to hash the pass
            model.Password = isCalledFromSecurityCodeVerification ? model.Password : Methods.GenerateSha512Hash(model.Password);

            //Get the user from the DB
            User user = await _userRepository.GetUser(model.Email, model.Password);

            if (user is null)
            {
                response.ErrorMessage = ErrorMessages.InvalidEmailOrPassword;
                return response;
            }

            //set the properties to the login response
            loginResponse.IsAccountConfirmed = user.IsAccountConfirmed;
            loginResponse.IsTwoFactorEnabled = user.IsTwoFactorEnabled;


            //check if the account is confirmed 
            if (!loginResponse.IsAccountConfirmed)
            {
                response.ErrorMessage = ErrorMessages.ValidateAccount;
                return response;
            }

            //check how many times user has entered bad security code
            loginResponse.IsAccountLocked = await CheckUserAttempts(model.Email);

            if (loginResponse.IsAccountLocked)
            {
                response.ErrorMessage = ErrorMessages.AccountLocked;
                return response;
            }

            //if the two factor is enabled than we are generating a code
            if (loginResponse.IsTwoFactorEnabled && !isCalledFromSecurityCodeVerification)
            {
                await _emailService.GenerateAndSendSecurityCodeForLogin(model.Email);

                response.Data = loginResponse;
                response.Success = true;
                return response;
            }

            // We are generating a token and storing in HTTP Only Cookie
            loginResponse.isTokenGenerated = _tokenService.GenerateJwtToken(model, user.Id);

            await _loggedUsersInfoRepository.CreateLoggedUserRecord(user);

            response.Success = true;
            response.Data = loginResponse;
            return response;
        }

        public async Task<ServiceResponse<LoginResponseModel>> VerifySecurityCode(VerifyTwoFactorAuthModel model)
        {
            var response = new ServiceResponse<LoginResponseModel>() { Data = null, ErrorMessage = "", Success = false };

            model.TrimStringProperties();

            //first check if there is a user in the db
            User user = await _userRepository.GetUserByEmail(model.Email);

            if (user is null || !user.IsTwoFactorEnabled)
            {
                response.ErrorMessage = ErrorMessages.InvalidUser;
                return response;
            }


            LoginResponseModel loginResponseModel = new LoginResponseModel();

            //check if there is a record in the cache
            var userEmailFromCache = await _distributedCache.GetStringAsync(model.SecurityCode);

            if (String.IsNullOrWhiteSpace(userEmailFromCache))
            {
                //check if there is more than one attempt in the cache that was failed by that user and if the limit is reached than lock the account
                //this is done with redis storage but can be improved
                loginResponseModel.IsAccountLocked = await CheckUserAttempts(model.Email, true);

                if (loginResponseModel.IsAccountLocked)
                {
                    response.Data = loginResponseModel;
                    response.ErrorMessage = ErrorMessages.AccountLocked;
                    return response;
                }

                response.ErrorMessage = ErrorMessages.SecurityCodeExpired;
                return response;

            }

            await _distributedCache.RemoveAsync(model.SecurityCode);


            //if the verification is correct than we are login the user and generating JWT token
            return await Login(new LoginModel { Email = user.Email, Password = user.Password }, user != null);

        }

        private async Task<bool> CheckUserAttempts(string userEmail, bool isFromVerifySecurityCode = false)
        {
            //we are setting different record in redis for failed attempts key is user.Email
            //first by email (this can be improved by ip in the future) to get the user and from the user to get the number Of failed attempts
            var numberOfAttempts = await _distributedCache.GetStringAsync(userEmail);

            //first time if he enters wrong security code we are setting the attempt to 1
            var firstFailedAttemptValue = "1";

            //if there is a record for that user we are entering
            if (!String.IsNullOrEmpty(numberOfAttempts))
            {
                var convertednumberOfAttempts = Convert.ToInt32(numberOfAttempts);

                //if the limit of the attempts is reached than we are returning true that means that account will be locked
                if (convertednumberOfAttempts > Convert.ToInt64(_configuration["Caching:FailedAttempsCounter"])) return true;

                //if the counter is smaller than two than increase it
                convertednumberOfAttempts++;

                //if the limit is not reached remove the record and than add new record with updated number
                await _distributedCache.RemoveAsync(userEmail);

                await _distributedCache.SetStringAsync(userEmail,
                                                       convertednumberOfAttempts.ToString(),
                                                       new DistributedCacheEntryOptions
                                                       {
                                                           AbsoluteExpiration = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Caching:AccountLockingTime"]))
                                                       });

                return false;
            }
            //if this is the first attempt we are setting the attemp to 1
            if (isFromVerifySecurityCode)
            {
                await _distributedCache.SetStringAsync(userEmail,
                                                       firstFailedAttemptValue,
                                                       new DistributedCacheEntryOptions
                                                       {
                                                           AbsoluteExpiration = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Caching:AccountLockingTime"]))
                                                       });
            }

            return false;
        }


        public async Task<LoggedUserInfo> GetUserByUserIdFromLoggedUserInfo(int userId)
        {
            return await _loggedUsersInfoRepository.GetLoggedUserInfo(userId);
        }


        public async Task<ServiceResponse<bool>> ConfirmAccountRegistration(string hashedToken)
        {
            var response = new ServiceResponse<bool>() { Success = false };

            var userEmail = await _distributedCache.GetStringAsync(hashedToken);

            if (String.IsNullOrWhiteSpace(userEmail))
            {
                response.ErrorMessage = ErrorMessages.InvalidUser;
                return response;
            }

            var user = await _userRepository.GetUserByEmail(userEmail);

            if (user == null)
            {
                response.ErrorMessage = ErrorMessages.InvalidUser;
                return response;
            }

            var isUpdated = await _userRepository.ConfirmUserAccount(user);

            await _distributedCache.RemoveAsync(hashedToken);

            response.ErrorMessage = !isUpdated ? ErrorMessages.ValidateAccount : string.Empty;
            response.Data = !isUpdated ? false : true;
            response.Success = true;
            return response;

        }

       

        public async Task<bool> IsUserAuthenticate()
        {
            var hasCookie = _httpContextAccessor.HttpContext.Request.Cookies.ContainsKey("token");

            return await Task.FromResult(hasCookie);
        }

        public async Task<ServiceResponse<AccountInfoModel>> GetLoggedUserInfo()
        {
            var response = new ServiceResponse<AccountInfoModel>() { Success = false };

            var userEmail = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Upn)?.Value;

            if (String.IsNullOrEmpty(userEmail))
            {
                response.ErrorMessage = ErrorMessages.InvalidUser;
                response.Success = false;
                return response;
            }

            var loggedUser = await _userRepository.GetUserByEmail(userEmail);

            if (loggedUser == null)
            {
                response.ErrorMessage = ErrorMessages.InvalidUser;
                response.Success = false;
                return response;
            }

            AccountInfoModel accountInfoModel = new()
            {
                FirstName = loggedUser.FirstName,
                LastName = loggedUser.LastName,
                Email = loggedUser.Email,
                IsTwoFactorEnabled = loggedUser.IsTwoFactorEnabled
            };


            response.Data = accountInfoModel;
            response.Success = true;
            return response;

        }

        public async Task<ServiceResponse<bool>> ChangeTwoFactorAuthenticationStatus(ChangeTwoFactorAuthStatusModel model)
        {
            await ValidateLoginFields(model);

            var response = new ServiceResponse<bool>() { Success = false };

            response.Data = await _userRepository.UpdateUserTwoFactorAuthStatus(model.Email, Methods.GenerateSha512Hash(model.Password));

            if (!response.Data)
            {
                response.ErrorMessage = ErrorMessages.FailedUpdateOfTwoFactorAuth;
                response.Success = false;
                return response;
            }

            response.Success = true;
            return response;

        }



        #region FormValidations
        public async Task<ServiceResponse<bool>> ValidateLoginFields(LoginModel loginModel)
        {
            // Initialize the service response
            var response = new ServiceResponse<bool>() { Success = false };

            loginModel.TrimStringProperties();

            Regex emailRegex = new Regex(_configuration["RegexValidation:EmailRegex"]);
            Regex passwordRegex = new Regex(_configuration["RegexValidation:PasswordRegex"]);

            if (!emailRegex.IsMatch(loginModel.Email) || !passwordRegex.IsMatch(loginModel.Password))
            {
                response.ErrorMessage = ErrorMessages.InvalidEmailOrPassword;
                return response;
            }

            response.Data = true; // Validation successful
            response.Success = true;
            return await Task.FromResult(response);
        }

        public async Task<ServiceResponse<bool>> ValidateFieldsForRegisterAccount(UserDto model)
        {
            // Initialize the service response
            var response = new ServiceResponse<bool>() { Success = false };

            model.TrimStringProperties();

            Regex emailRegex = new Regex(_configuration["RegexValidation:EmailRegex"]);
            Regex passwordRegex = new Regex(_configuration["RegexValidation:PasswordRegex"]);

            if (model.Password != model.ConfirmPassword)
            {
                response.ErrorMessage = ErrorMessages.InvalidEmailOrPassword;
                return response;
            }

            User currentUser = await _userRepository.GetUserByEmail(model.Email);

            if (currentUser != null)
            {
                response.ErrorMessage = ErrorMessages.UserAlreadyExist;
                return response;
            }

            if (!emailRegex.IsMatch(model.Email) || !passwordRegex.IsMatch(model.Password))
            {
                response.ErrorMessage = ErrorMessages.InvalidEmailOrPassword;
                return response;
            }

            response.Data = true; // Validation successful
            response.Success = true;
            return await Task.FromResult(response);

        }     
        #endregion
    }

}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using WeatherSearchApp_Domain.DTOModels;
using WeatherSearchApp_Service.Interfaces;
using WeatherSearchApp_Service.Models;
using WeatherSearchApp_Service.Services;
using WeatherSearchApp_Shared.AppConstants;

namespace WeatherSearchApp_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public UserController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        [HttpPost("register-account")]
        public async Task<ServiceResponse<int>> RegisterUser([FromBody] UserDto userEntity)
        {
            try
            {
                return await _userService.RegisterAccount(userEntity);

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new ServiceResponse<int>() { Success = false, ErrorMessage = ErrorMessages.GenericErrorControllerMessage };
            }
        }

        [HttpPost("login-user")]
        public async Task<ServiceResponse<LoginResponseModel>> LoginUser([FromBody] LoginModel loginModel)
        {
            try
            {
                var res = await _userService.Login(loginModel);

                return res;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new ServiceResponse<LoginResponseModel>() { Success = false, ErrorMessage = ErrorMessages.GenericErrorControllerMessage };
            }
        }

        [HttpPost("verify-two-factor-auth")]
        public async Task<ServiceResponse<LoginResponseModel>> VerifyTwoFactorAuth([FromBody] VerifyTwoFactorAuthModel model)
        {
            try
            {
                return await _userService.VerifySecurityCode(model);

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);

                return new ServiceResponse<LoginResponseModel>() { Success = false, ErrorMessage = ErrorMessages.GenericErrorControllerMessage };
            }
        }

        [HttpPost("confirm-account")]
        public async Task<ServiceResponse<bool>> ConfirmAccountRegistration([FromBody] VerifyAccount model)
        {
            try
            {
                return await _userService.ConfirmAccountRegistration(model.HashedToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new ServiceResponse<bool>() { Success = false, ErrorMessage = ErrorMessages.GenericErrorControllerMessage };
            }
        }

        [HttpPost("resend-email")]
        public async Task<ServiceResponse<bool>> ResendEmailForTwoFactor([FromBody] ResendSecuirtyCode model)
        {
            try
            {
                return await _emailService.GenerateAndSendSecurityCodeForLogin(model.Email);
               
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new ServiceResponse<bool>() { Success = false, ErrorMessage = ErrorMessages.GenericErrorControllerMessage };
            }
        }
        [Authorize]
        [HttpPost("get-logged-user-info")]
        public async Task<ServiceResponse<AccountInfoModel>> GetLoggedUserInfo()
        {
            try
             {
                return await _userService.GetLoggedUserInfo();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new ServiceResponse<AccountInfoModel>() { Success = false, ErrorMessage = ErrorMessages.GenericErrorControllerMessage };
            }
        }

        [Authorize]
        [HttpPost("change-two-factor-auth-status")]
        public async Task<ServiceResponse<bool>> ChangeTwoFactorAuthenticationStatus(ChangeTwoFactorAuthStatusModel changeTwoFactorAuthStatusModel)
        {
            try
            {
                return await _userService.ChangeTwoFactorAuthenticationStatus(changeTwoFactorAuthStatusModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new ServiceResponse<bool>() { Success = false, ErrorMessage = ErrorMessages.GenericErrorControllerMessage };
            }
        }

        [TypeFilter(typeof(JWTInterceptorAuthFilter))]
        [HttpPost("logout")]
        public async Task<ServiceResponse<bool>> Logout()
        {
            try
            {
                return await _userService.LogoutUser();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new ServiceResponse<bool>() { Success = false, ErrorMessage = ErrorMessages.GenericErrorControllerMessage };
            }
        }

        [HttpPost("isAuthenticated")]
        public async Task<bool> IsUserAuthenticated()
        {
            try
            {
                return await _userService.IsUserAuthenticate();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }
    }
}

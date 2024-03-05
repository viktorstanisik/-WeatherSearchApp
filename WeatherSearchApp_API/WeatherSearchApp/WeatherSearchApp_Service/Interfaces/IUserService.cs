using WeatherSearchApp_Domain.DTOModels;
using WeatherSearchApp_Domain.EntityModels;
using WeatherSearchApp_Service.Models;

namespace WeatherSearchApp_Service.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse<int>> RegisterAccount(UserDto userDtoModel);
        Task<ServiceResponse<LoginResponseModel>> Login(LoginModel model, bool isCalledFromSecurityCodeVerification = false);
        Task<ServiceResponse<LoginResponseModel>> VerifySecurityCode(VerifyTwoFactorAuthModel model);
        int GetLoggedUserIdFromHttpContext();
        Task<ServiceResponse<bool>> LogoutUser();
        Task<LoggedUserInfo> GetUserByUserIdFromLoggedUserInfo(int userId);
        Task<ServiceResponse<bool>> ConfirmAccountRegistration(string hashedToken);
        Task<bool> IsUserAuthenticate();
        Task<ServiceResponse<AccountInfoModel>> GetLoggedUserInfo();
        Task<ServiceResponse<bool>> ChangeTwoFactorAuthenticationStatus(ChangeTwoFactorAuthStatusModel changeTwoFactorAuthStatusModel);

    }
}

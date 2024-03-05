using WeatherSearchApp_Domain.DTOModels;
using WeatherSearchApp_Domain.EntityModels;
using WeatherSearchApp_Service.Models;

namespace WeatherSearchApp_Service.Interfaces
{
    public interface IEmailService
    {  
        Task RegisterAccountConfirmationLink(User user);

        Task<ServiceResponse<bool>> GenerateAndSendSecurityCodeForLogin(string email);

    }
}

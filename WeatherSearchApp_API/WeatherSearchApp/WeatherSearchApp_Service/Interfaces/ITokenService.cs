using WeatherSearchApp_Service.Models;

namespace WeatherSearchApp_Service.Interfaces
{
    public interface ITokenService
    {
        bool GenerateJwtToken(LoginModel model, int id);
    }
}

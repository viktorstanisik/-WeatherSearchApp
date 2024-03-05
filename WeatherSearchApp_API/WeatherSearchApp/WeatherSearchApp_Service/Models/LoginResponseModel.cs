using WeatherSearchApp_Domain.DTOModels;

namespace WeatherSearchApp_Service.Models
{
    public class LoginResponseModel
    {
        public string? Email { get; set; }
        public bool IsAccountConfirmed { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public bool IsAccountLocked { get; set; }
        public bool isTokenGenerated { get; set; }

    }
}

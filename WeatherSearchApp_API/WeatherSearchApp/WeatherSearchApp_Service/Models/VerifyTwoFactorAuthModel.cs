using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherSearchApp_Service.Models
{
    public class VerifyTwoFactorAuthModel
    {
        public required string Email { get; set; }
        public required string SecurityCode { get; set; }

    }
}

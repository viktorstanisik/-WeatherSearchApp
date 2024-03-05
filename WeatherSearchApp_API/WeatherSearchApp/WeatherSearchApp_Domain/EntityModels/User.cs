using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherSearchApp_Domain.EntityModels
{
    public class User : BaseEntity
    {
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
        public bool IsAccountConfirmed { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
    }
}

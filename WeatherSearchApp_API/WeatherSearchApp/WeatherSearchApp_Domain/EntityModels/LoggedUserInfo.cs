using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherSearchApp_Domain.EntityModels
{
    public class LoggedUserInfo : BaseEntity
    {
        public int UserId { get; set; }
        public DateTime? LastLogin { get; set; }
        public int LoginStatusId { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
    }
}

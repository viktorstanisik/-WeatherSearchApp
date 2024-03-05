using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherSearchApp_Domain.EntityModels;

namespace WeatherSearchApp_DataAccess.Interfaces
{
    public interface ILoggedUsersInfoRepository
    {
        Task<LoggedUserInfo> CreateLoggedUserRecord(User user);
        Task<LoggedUserInfo> GetLoggedUserInfo(int userId, bool isFromLogout = false);
        Task<LoggedUserInfo> LogoutUser(int userId);

    }
}

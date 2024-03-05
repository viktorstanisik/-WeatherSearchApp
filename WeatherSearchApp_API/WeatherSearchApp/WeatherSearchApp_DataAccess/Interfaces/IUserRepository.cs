using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherSearchApp_Domain.EntityModels;

namespace WeatherSearchApp_DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        Task<User> GetUser(string email, string password);
        Task<User> GetUserByEmail(string email);
        Task<bool> ConfirmUserAccount(User user);
        Task<bool> UpdateUserTwoFactorAuthStatus(string userEmail, string password);
    }
}

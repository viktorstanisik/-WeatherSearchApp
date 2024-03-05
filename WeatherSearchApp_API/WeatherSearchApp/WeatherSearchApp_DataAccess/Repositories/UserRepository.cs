using Microsoft.EntityFrameworkCore;
using Serilog;
using WeatherSearchApp_DataAccess.Interfaces;
using WeatherSearchApp_Domain;
using WeatherSearchApp_Domain.EntityModels;
using WeatherSearchApp_Shared.AppConstants;

namespace WeatherSearchApp_DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly WeatherSearchAppDbContext _dbContext;

        public UserRepository(WeatherSearchAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> CreateUser(User userModel)
        {
            try
            {
                await _dbContext.AddAsync(userModel);
                await _dbContext.SaveChangesAsync();

                return userModel;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<User> GetUser(string email, string password)
        {
            try
            {
                return await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            try
            {
                return await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<bool> ConfirmUserAccount(User user)
        {
            try
            {
                var dbUser = await GetUserByEmail(user.Email);

                if (user == null) throw new Exception(ErrorMessages.InvalidUser);

                dbUser.IsAccountConfirmed = true;

                await _dbContext.SaveChangesAsync();

                return true;


            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }

        }

        public async Task<bool> UpdateUserTwoFactorAuthStatus(string userEmail, string password)
        {
            try
            {
                var dbUser = await GetUser(userEmail, password);

                if (dbUser == null) throw new Exception(ErrorMessages.InvalidUser);

                dbUser.IsTwoFactorEnabled = !dbUser.IsTwoFactorEnabled;

                await _dbContext.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }
    }
}

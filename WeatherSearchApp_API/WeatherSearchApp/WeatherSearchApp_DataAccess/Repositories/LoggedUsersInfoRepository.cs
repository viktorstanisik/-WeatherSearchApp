using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherSearchApp_DataAccess.Interfaces;
using WeatherSearchApp_Domain;
using WeatherSearchApp_Domain.EntityModels;
using WeatherSearchApp_Shared.AppConstants;
using WeatherSearchApp_Shared.Enums;

namespace WeatherSearchApp_DataAccess.Repositories
{
    public class LoggedUsersInfoRepository : ILoggedUsersInfoRepository
    {
        private readonly WeatherSearchAppDbContext _dbContext;
        private readonly ILogger<LoggedUsersInfoRepository> _logger;

        public LoggedUsersInfoRepository(WeatherSearchAppDbContext dbContext, ILogger<LoggedUsersInfoRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<LoggedUserInfo> CreateLoggedUserRecord(User user)
        {
            try
            {
                var userisLogged = await _dbContext.LoggedUsersInfo.Where(x => x.UserId == user.Id && x.LoginStatusId == (int)LoginStatus.LoggedIn)
                                                                   .OrderByDescending(x => x.LastLogin)
                                                                   .FirstOrDefaultAsync();
                if (userisLogged != null)
                {
                    userisLogged.LastLogin = DateTime.Now;
                    userisLogged.IsTwoFactorEnabled = user.IsTwoFactorEnabled;
                    await _dbContext.SaveChangesAsync();
                    return userisLogged;
                }

                var loggedUser = new LoggedUserInfo()
                {
                    UserId = user.Id,
                    LastLogin = DateTime.Now,
                    LoginStatusId = (int)LoginStatus.LoggedIn,
                    IsTwoFactorEnabled = user.IsTwoFactorEnabled
                };

                await _dbContext.AddAsync(loggedUser);
                await _dbContext.SaveChangesAsync();

                return loggedUser;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }

        public async Task<LoggedUserInfo> LogoutUser(int userId)
        {
            try
            {
                var loggedUserInfo = await GetLoggedUserInfo(userId, true);

                if (loggedUserInfo is null) throw new Exception(ErrorMessages.GenericError);

                loggedUserInfo.LoginStatusId = (int)LoginStatus.LoggedOut;
                await _dbContext.SaveChangesAsync();

                return loggedUserInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }

        public async Task<LoggedUserInfo> GetLoggedUserInfo(int userId, bool isFromLogout = false)
        {
            try
            {
                return await _dbContext.LoggedUsersInfo
                                        .Where(x => x.UserId == userId && (!isFromLogout || x.LoginStatusId == (int)LoginStatus.LoggedIn))
                                        .OrderByDescending(x => x.LastLogin)
                                        .FirstOrDefaultAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }
    }
}

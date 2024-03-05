using Microsoft.Extensions.DependencyInjection;
using WeatherSearchApp_DataAccess.Interfaces;
using WeatherSearchApp_DataAccess.Repositories;
using WeatherSearchApp_Service.Interfaces;
using WeatherSearchApp_Service.Services;
using WeatherSearchApp_Shared.Mapper;

namespace WeatherSearchApp_Service
{
    public static class DIServiceFactory
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton(AutoMapperConfig.Configure());

            services.AddTransient(typeof(JWTInterceptorAuthFilter));

           services.AddTransient(typeof(IEmailService), typeof(EmailService));

            services.AddTransient(typeof(IUserRepository), typeof(UserRepository));

            services.AddTransient(typeof(IWeatherService), typeof(WeatherService));

            services.AddTransient(typeof(IUserService), typeof(UserService));
            services.AddTransient(typeof(ILoggedUsersInfoRepository), typeof(LoggedUsersInfoRepository));

            services.AddTransient(typeof(ITokenService), typeof(TokenService));
        }
    }
}

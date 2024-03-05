using AutoMapper;
using WeatherSearchApp_Domain.DTOModels;
using WeatherSearchApp_Domain.EntityModels;

namespace WeatherSearchApp_Shared.Mapper
{
    public class AutoMapperConfig
    {
        public static IMapper Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                #region User and UserInfo

                cfg.CreateMap<User, UserDto>()
                .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore());

                cfg.CreateMap<UserDto, User>();


                #endregion
            });

            return config.CreateMapper();
        }
    }
}

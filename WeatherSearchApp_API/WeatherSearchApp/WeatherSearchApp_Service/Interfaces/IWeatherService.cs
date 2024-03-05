using WeatherSearchApp_Service.Models;

namespace WeatherSearchApp_Service.Interfaces
{
    public interface IWeatherService
    {
        Task<ServiceResponse<List<WeatherResponseModel>>> GetDataFromWeatherApi(WeatherSearchModel weatherSearchModel);

    }
}

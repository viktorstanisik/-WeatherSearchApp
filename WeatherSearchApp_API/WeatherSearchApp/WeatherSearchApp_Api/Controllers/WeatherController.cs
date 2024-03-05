using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using WeatherSearchApp_Service.Interfaces;
using WeatherSearchApp_Service.Models;
using WeatherSearchApp_Shared.AppConstants;

namespace WeatherSearchApp_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpPost("get-weather-data")]
        public async Task<ServiceResponse<List<WeatherResponseModel>>> GetWeatherData([FromBody] WeatherSearchModel weatherSearchModel)
        {
            try
            {
               return await _weatherService.GetDataFromWeatherApi(weatherSearchModel);
            
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new ServiceResponse<List<WeatherResponseModel>> () { Success = false, ErrorMessage = ErrorMessages.GenericErrorControllerMessage };
            }
        }
    }
}

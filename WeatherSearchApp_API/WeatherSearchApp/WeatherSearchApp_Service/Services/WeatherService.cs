using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeatherSearchApp_Domain.EntityModels;
using WeatherSearchApp_Service.Interfaces;
using WeatherSearchApp_Service.Models;
using WeatherSearchApp_Shared.AppConstants;
using WeatherSearchApp_Shared.HelperMethods;

namespace WeatherSearchApp_Service.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IConfiguration _configuration;

        public WeatherService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ServiceResponse<List<WeatherResponseModel>>> GetDataFromWeatherApi(WeatherSearchModel weatherSearchModel)
        {
            var response = new ServiceResponse<List<WeatherResponseModel>>() { ErrorMessage = "", Success = false };

            var hasError = await ValidateInputModel(weatherSearchModel);
            if(!hasError.Success)
            {
                response.Success = hasError.Success;
                response.ErrorMessage = hasError.ErrorMessage;
                return response;
            }


            string json;

            using (var client = new HttpClient())
            {
                string openWeatherApiPath = _configuration["OpenWeatherApiConfig:ApiPath"];
                string openWeatherAppId = _configuration["OpenWeatherApiConfig:AppId"];

                var url = $"{openWeatherApiPath}{weatherSearchModel.CityName}&units=metric&appid={openWeatherAppId}";

                var request = new RestRequest("/resource/", Method.Get);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("appid", openWeatherAppId);
                client.DefaultRequestHeaders.Add("q", weatherSearchModel.CityName);

                var endpoint = new Uri(url);

                var result = await client.GetAsync(endpoint);
                json = await result.Content.ReadAsStringAsync();
            }
            var responseFromOpenWeatherApi = JsonConvert.DeserializeObject<OpenWeatherApiResponseModel>(json);

            if(responseFromOpenWeatherApi == null)
            {
                response.ErrorMessage = ErrorMessages.GenericError;
                return response;
            }

            var convertedResponse = ParseResponse(responseFromOpenWeatherApi);

            if(convertedResponse == null || !convertedResponse.Any())
            {
                response.ErrorMessage = ErrorMessages.InvalidCityName;
                return response;
            }

            response.Success = true;
            response.Data = convertedResponse;
            return response;
        }

        private List<WeatherResponseModel> ParseResponse(OpenWeatherApiResponseModel response)
        {
            List<WeatherResponseModel> data = new();

            int num = 0;
            List<string> daysOfWeek = new();
            foreach (var item in response.list)
            {
                foreach (var value in item.weather)
                {
                    if (daysOfWeek.Contains(item.Dt_txt.Split(" ")[0])) continue;

                    WeatherResponseModel responseToAngular = new()
                    {
                        Date = item.Dt,
                        Temperature = item.Main.Temp,
                        MaximumTemperature = item.Main.Temp_max,
                        MinimumTemperature = item.Main.Temp_min,
                        Humidity = item.Main.Humidity,
                        FeelsLike = item.Main.Feels_like,
                        Pressure = item.Main.Pressure,
                        WeatherCondition = value.Main,
                        WeatherConditionDescription = value.Description
                    };
                    num++;
                    if (num < 5) data.Add(responseToAngular);

                    daysOfWeek.Add(item.Dt_txt.Split(" ")[0]);
                }
            }

            return data;

        }

        private async Task<ServiceResponse<bool>> ValidateInputModel(WeatherSearchModel model)
        {
            // Initialize the service response
            var response = new ServiceResponse<bool>() { Success = false };

            model.TrimStringProperties();

            Regex onlyLetterRegex = new(_configuration["RegexValidation:GenericValidationOnlyLetterRegex"]);

            if (!onlyLetterRegex.IsMatch(model.CityName))
            {
                response.ErrorMessage = ErrorMessages.InvalidCityName;
                return response;
            }

            response.Success = true;
            return await Task.FromResult(response);
        }
    }
}

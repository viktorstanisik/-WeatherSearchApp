using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherSearchApp_Service.Models
{
    public class WeatherResponseModel
    {
        public int Date { get; set; }
        public string? Temperature { get; set; }
        public string? FeelsLike { get; set; }
        public string? MinimumTemperature { get; set; }
        public string? MaximumTemperature { get; set; }
        public string? Humidity { get; set; }
        public string? Pressure { get; set; }
        public string? WeatherCondition { get; set; }
        public string? WeatherConditionDescription { get; set; }
    }
}

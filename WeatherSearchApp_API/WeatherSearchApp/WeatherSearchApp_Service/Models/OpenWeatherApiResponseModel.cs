namespace WeatherSearchApp_Service.Models
{
    public class OpenWeatherApiResponseModel
    {
        public bool ValidRequest { get; set; }
        public string Cnt { get; set; }

        public List<ListOfItems> list = new List<ListOfItems>();
    }

    public class ListOfItems
    {

        public int Dt { get; set; }
        public string? Dt_txt { get; set; }
        public TempInfo? Main { get; set; }
        public List<WeatherConditions> weather = new List<WeatherConditions>();


    }

    public class TempInfo
    {
        public string? Temp { get; set; }
        public string? Feels_like { get; set; }
        public string? Temp_min { get; set; }
        public string? Temp_max { get; set; }
        public string? Pressure { get; set; }
        public string? Sea_level { get; set; }
        public string? Grnd_level { get; set; }
        public string? Humidity { get; set; }
        public string? Temp_kf { get; set; }
    }

    public class WeatherConditions
    {
        public string Main { get; set; }
        public string Description { get; set; }

    }
}

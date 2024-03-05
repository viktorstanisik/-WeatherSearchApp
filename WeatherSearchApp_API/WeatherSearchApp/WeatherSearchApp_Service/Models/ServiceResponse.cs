using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherSearchApp_Service.Models
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public ServiceResponse(T data)
        {
            Data = data;
            Success = true;
        }

        public ServiceResponse() { }

        public ServiceResponse(string errorMessage)
        {
            ErrorMessage = errorMessage;
            Success = false;
        }
    }
}

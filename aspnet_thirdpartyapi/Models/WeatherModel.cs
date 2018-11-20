using System.ComponentModel.DataAnnotations;

namespace aspnet_thirdpartyapi.Models
{
    public class WeatherModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Currently Currently { get; set; }
    }
}
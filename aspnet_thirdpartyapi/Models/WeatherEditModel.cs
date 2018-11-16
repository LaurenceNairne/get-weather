using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace aspnet_thirdpartyapi.Models
{
    public class WeatherEditModel
    {
        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public long Time { get; set; }
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace aspnet_thirdpartyapi.Models
{
    public class WeatherModel
    {
        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public string Timezone { get; set; }

        public Currently Currently { get; set; }
    }
}
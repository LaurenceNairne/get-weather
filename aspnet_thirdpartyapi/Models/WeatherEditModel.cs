using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace aspnet_thirdpartyapi.Models
{
    public class WeatherEditModel
    {
        [RegularExpression(@"^[-+]?([1-8]?\d(\.\d+)?|90(\.0+)?)$")]
        [Required]
        public double Latitude { get; set; }

        [RegularExpression(@"^[-+]?(180(\.0+)?|((1[0-7]\d)|([1-9]?\d))(\.\d+)?)$")]
        [Required]
        public double Longitude { get; set; }

        [Range(1000000000, 9999999999)]
        [Required]
        public long Time { get; set; }
    }
}
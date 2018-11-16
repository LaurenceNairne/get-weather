using aspnet_thirdpartyapi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace aspnet_thirdpartyapi.Services
{
    public interface IWeatherService
    {
        Task<WeatherModel> GetWeatherBasedOnCriteria(double lat, double lon, long time);
    }
}

using aspnet_thirdpartyapi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace aspnet_thirdpartyapi.Services
{
    public class WeatherService : IWeatherService
    {
        private IGetWeather _getWeather;

        public WeatherService(IGetWeather getWeather)
        {
            _getWeather = getWeather;
        }

        public async Task<WeatherModel> GetWeatherBasedOnCriteria(double lat, double lon, long time)
        {
            return await _getWeather.ReturnWeatherBasedOnQueries(lat,lon,time);
        }
    }
}

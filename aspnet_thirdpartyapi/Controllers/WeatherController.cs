using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspnet_thirdpartyapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace aspnet_thirdpartyapi.Controllers
{
    public class WeatherController : Controller
    {
        private IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        [Route("/{lat}/{lon}/{time}")]
        public async Task<IActionResult> GetTheWeather(double lat, double lon, long time)
        {
            var result = await _weatherService.GetWeatherBasedOnCriteria(lat,lon,time);
            return View(result);
        }
    }
}
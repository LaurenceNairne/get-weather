using System.Threading.Tasks;
using aspnet_thirdpartyapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace aspnet_thirdpartyapi.Controllers
{
    public class WeatherController : Controller
    {
        private IGetWeather _getWeather;

        public WeatherController(IGetWeather getWeather)
        {
            _getWeather = getWeather;
        }

        [HttpGet]
        [Route("/{lat}/{lon}/{time}")]
        public async Task<IActionResult> GetTheWeather(double lat, double lon, long time)
        {
            var result = await _getWeather.ReturnWeatherBasedOnQueries(lat, lon, time);
            return View(result);
        }
    }
}
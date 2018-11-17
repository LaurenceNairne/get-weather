using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using aspnet_thirdpartyapi.Models;

namespace aspnet_thirdpartyapi.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(WeatherEditModel weatherEditModel)
        {
            if (ModelState.IsValid)
            {
                var weather = new WeatherEditModel
                {
                    Latitude = weatherEditModel.Latitude,
                    Longitude = weatherEditModel.Longitude,
                    Time = weatherEditModel.Time
                };

                return Redirect($"/{weather.Latitude}/{weather.Longitude}/{weather.Time}");
            }
            return View();
        }
    }
}
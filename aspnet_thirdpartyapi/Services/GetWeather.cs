using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using aspnet_thirdpartyapi.Models;
using Newtonsoft.Json;

namespace aspnet_thirdpartyapi.Services
{
    public class GetWeather : IGetWeather
    {
        public async Task<WeatherModel> ReturnWeatherBasedOnQueries(double lat, double lon, long time)
        {
            const string darkSkyKey = "dfefe770beeae342a4a392ee4f4f7760";
            using (var client = new HttpClient())
            {
                var url = new Uri
                    ($"https://api.darksky.net/forecast/{darkSkyKey}/{lat},{lon},{time}?" +
                    $"exclude=daily,hourly,minutely,alerts,flags?units=uk2");
                var response = await client.GetAsync(url);

                string json;
                using (var content = response.Content)
                {
                    json = await content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<WeatherModel>(json);
                }
            }
        }
    }
}

using System.Threading.Tasks;
using aspnet_thirdpartyapi.Models;

namespace aspnet_thirdpartyapi.Services
{
    public interface IGetWeather
    {
        Task<WeatherModel> ReturnWeatherBasedOnQueries(double lat, double lon, long time);
    }
}

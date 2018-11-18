# get-weather
This repository provides an example of connecting to a third-party API and using MVC framework to provide a form to fill out a request and a results page with the expected data returned.

## Startup.cs
Our startup class is fairly straight forward in this one. We only register two further services:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.AddScoped<IGetWeather, GetWeather>();
}
```
Our custom service will provide the logic to retrieve the correct weather from the third party API DarkSky. We provide an interface and an implementation so that we could switch out later for another implementation if we needed to

We also provide a custom method that will set up our default route configuration:

```csharp
private void ConfigureRoutes(IRouteBuilder routeBuilder)
{
    routeBuilder.MapRoute(
        "Default",
        "{controller=Home}/{action=Index}"
        );
}
```
We inject the IRouteBuilder service which requires using the `Microsoft.AspNetCore.Routing` namespace. This allows us to create the contract of the routing, or the format of the URL that will correctly trigger our actions

We provide this method as a parameter to the MVC service when it is called in the pipeline:

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseMvc(ConfigureRoutes);
}
```

## Services
We have a public interface `IGetWeather` which simply has a single method `Task<WeatherModel> ReturnWeatherBasedOnQueries(double lat, double lon, long time)`. Our implementation of this is held in `GetWeather`.

```csharp
public class GetWeather : IGetWeather
{
    public async Task<WeatherModel> ReturnWeatherBasedOnQueries(double lat, double lon, long time)
    {
        const string darkSkyKey = "dfefe770beeae342a4a392ee4f4f7760";
        using (var client = new HttpClient())
        {
            var url = new Uri
                ($"https://api.darksky.net/forecast/{darkSkyKey}/{lat},{lon},{time}?" +
                $"exclude=daily,hourly,minutely,alerts,flags&units=auto");
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
```
******************* Pick up from here **********************

## Controllers
## Models
### Input Model
### Output Model
### Data class
## Views
### Index
### GetTheWeather
### _Layout
### _ViewStart
### _ViewImports


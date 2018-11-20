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
There's a few parts to the implementation of this method so we'll go through it step-by-step:

- We first make this method async, which tells the compiler we want this to be handled asynchronously (and it will handle all of the more complicated aspects of asynchronous programming for us)
- We have a `darkSkyKey` field - this is the secret key that we've been assigned by the DarkSky API so that we can access it, we're just creating a local variable to hold this key
- I've only just discovered `using` **statements** (as opposed to using **directives** at the top of files). I had to do some digging to get at what this is used for:

It is used with objects that implement the IDisposable interface. This interface handle the disposing of unmanaged resources in line with garbage collection, as the garbage collector does not know about unmanaged resources. In the docs it reads:

*"When the lifetime of an IDisposable object is limited to a single method, you should declare and instantiate it in the using statement. The using statement calls the Dispose method on the object in the correct way, and (when you use it as shown earlier) it also causes the object itself to go out of scope as soon as Dispose is called."*

- In our case, our `using` statement is ensuring that the scope of our `client` variable that references a `HttpClient` object (which by inheritance of HttpMessageInvoker implements IDisposable) remains within our method and is put out of scope for garbarge collection after the method has finished
- Inside the using block, we create a variable `url` and assign it a new `Uri` object with the URL of our third party API, configured to only contain the points of data we want, set to pull in the `darkSkyKey` (this makes things easier if the key changes as we just change the value of our variable) and allowing for the latitude, longitude and time properties in the url to be populated by the parameters in the method call
- We create a new variable `response` to reference the `HttpResponseMessage` returned from the GET request sent using the `client.GetAsync(url)` method, and create an (currently) empty string variable `json`
- We then have another `using` statement block in which we create a `content` variable to reference a `HttpContent` object returned with the `HttpResponseMessage`
- This `HttpContent` contains the Http entity body and content headers
- We assign a string serialized version of our `HttpContent` object (referenced by `content`) to our `json` string using the `ReadAsStringAsync()` method
- We then Deserialize this string into a `WeatherModel` object so we can use it in our application
- To do this we're using a NewtonSoft.Json method `JsonConvert.DeserializeObject<T>(string valueToDeserialize)`
- We provide our `WeatherModel` as the .NET type in place of the generic `<T>` type, and our `json` string as the value to convert
- We return the resulting `WeatherModel` object

## Controllers
We have two controllers in our application: `HomeController` and `WeatherController`. The former provides a GET `Index` action that returns an view called "Index", and a POST `Index` action that is invoked when we submit data entered into our form on the Index view. The latter provides a GET `GetTheWeather` action that will return a view named "GetTheWeather" pulling in the the resulting data retrieved from the DarkSky API via the `ReturnWeatherBasedOnQueries(lat, lon, time)` method in our `IGetWeather` service.

```csharp
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
```
The `HomeController` is fairly straight forward. There are a few things of note:

We use the `[AutoValidateAntiforgeryToken]` attribute at the beginning of the class. When we create a form in MVC, it automatically creates a hidden input element called `__RequestVerificationToken` which has a unique value. This is to allow us to verify that a POST request was sent from our site, protecting against cross-site request forgery attacks. However, we must validate the token for it to be useful. There are a few ways that we can do this - we can do it on a per action basis or at a controller class level. The attribute I have chosen is essentially a shortcut whereby MVC knows to validate tokens from POST requests, but ignores safe request types (GET, HEAD, OPTIONS, and TRACE) which do not have a token generated for them. This removes the need to use `[IgnoreAntiForgeryToken]` on all safe actions, or individually assigning the `[ValidateAntiForgeryToken]` attribute on all unsafe actions.

We perform a check on ModelState.IsValid - if it returns false it means that there are issues with the values input in the form and we simply return the current view (and the view will handle rendering of the validation error message - more on that below in the [Views>Index](#index) section). If it returns true, all validation checks are good.

We create an input model object`WeatherEditModel`, which holds the properties we need to send a good API request - more on this below in the [Input Model](#input-model) section.

When we have the model created, we return a redirect to the route of our latitude, longitude and time properties (which then invokes the `GetTheWeather` action on our `WeatherController` (more on that below).

```csharp
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
```
Our `WeatherController` is even simpler. It has one action `GetTheWeather`. We've dictated that the route to invoke this action must contain a latitude value, a longitude value and a time value (which it reads from the parameters in the method call. Inside the action we create a `WeatherModel` object named `result` that takes the returned value from calling `ReturnWeatherBasedOnQueries(lat, lon, time)` and then we return the `GetTheWeather` view passing in `result`.

## Models
We only have two models. `WeatherModel` is our output model that provides the properties we wish to retrieve from the API, `WeatherEditModel` is our input model that provides the properties we need to give values to in order to retrieve the information we need from the API.

### Input Model

```csharp
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
```
`WeatherEditModel` has three properties: Latitude, Longitude and Time. These are the properties that are needed to make a TimeMachine request from the DarkSkyAPI. Latitude and Longitude are of type `double` because we decimal places for latitude and longitude values in the real world. Time is of type long because we are using UNIX timestamp, which is seconds since midnight (UTC) on the 1st January 1970

We are using attribute based validation rules on our properties to ensure users enter useful values. Latitude and Longitude require a particular regular expression to fit the format of their real world formats (latitude -90 through 90, longitude -180 through 180). I'm currently terrible at RegEx so I had to find these on StackOverflow. Annoyingly, using the `[RegularExpression]` attribute results in a pretty ugly and non user-friendly validation error and I'm currently not knowledgeable enough to fix it. For time we're just expecting integers, but the API is subject to a limitation on the data it holds for given timestamps. I've set a reasonable arbitrary value range between 1,000,000,000 and 9,999,999,999, but we'd want to be more explicit in real world terms because there definitely will not be a forecast available for the upper or lower limits of the range.

Finally, all fields are required, so if any are left blank, the request will not go through.

### Output Model



### Data class
## Views
### Index
### GetTheWeather
### _Layout
### _ViewStart
### _ViewImports

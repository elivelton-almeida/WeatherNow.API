using System.Text.Json;
using WeatherNow.API.Models;

namespace WeatherNow.API.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<WeatherResponse> GetWeatherAsync(string city)
        {
            var apiKey = _configuration["OpenWeather:ApiKey"];

            var response = await _httpClient.GetAsync(
                $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric&lang=pt_br");


            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonDocument.Parse(json);

            return new WeatherResponse
            {
                City = data.RootElement.GetProperty("name").GetString(),
                Temperature = data.RootElement.GetProperty("main").GetProperty("temp").GetDouble(),
                Humidity = data.RootElement.GetProperty("main").GetProperty("humidity").GetInt32(),
                Description = data.RootElement.GetProperty("weather")[0].GetProperty("description").GetString(),
                Icon = data.RootElement.GetProperty("weather")[0].GetProperty("icon").GetString(),

                // Temperatura detalhada
                FeelsLike = data.RootElement.GetProperty("main").TryGetProperty("feels_like", out var feelsLike)
        ? feelsLike.GetDouble()
        : (double?)null,
                TempMin = data.RootElement.GetProperty("main").TryGetProperty("temp_min", out var tempMin)
        ? tempMin.GetDouble()
        : (double?)null,
                TempMax = data.RootElement.GetProperty("main").TryGetProperty("temp_max", out var tempMax)
        ? tempMax.GetDouble()
        : (double?)null,
                Pressure = data.RootElement.GetProperty("main").TryGetProperty("pressure", out var pressure)
        ? pressure.GetInt32()
        : (int?)null,

                // Vento
                WindSpeed = data.RootElement.TryGetProperty("wind", out var windProp)
        ? windProp.TryGetProperty("speed", out var speed) ? speed.GetDouble() : (double?)null
        : (double?)null,
                WindDeg = data.RootElement.TryGetProperty("wind", out var windProp2)
        ? windProp2.TryGetProperty("deg", out var deg) ? deg.GetInt32() : (int?)null
        : (int?)null,

                // Nuvens
                Cloudiness = data.RootElement.TryGetProperty("clouds", out var cloudsProp)
        ? cloudsProp.TryGetProperty("all", out var all) ? all.GetInt32() : (int?)null
        : (int?)null,

                // Precipitação
                Rain1h = data.RootElement.TryGetProperty("rain", out var rainProp) && rainProp.TryGetProperty("1h", out var rain1h)
        ? rain1h.GetDouble()
        : (double?)null,
                Rain3h = data.RootElement.TryGetProperty("rain", out var rainProp3) && rainProp3.TryGetProperty("3h", out var rain3h)
        ? rain3h.GetDouble()
        : (double?)null,
                Snow1h = data.RootElement.TryGetProperty("snow", out var snowProp) && snowProp.TryGetProperty("1h", out var snow1h)
        ? snow1h.GetDouble()
        : (double?)null,
                Snow3h = data.RootElement.TryGetProperty("snow", out var snowProp3) && snowProp3.TryGetProperty("3h", out var snow3h)
        ? snow3h.GetDouble()
        : (double?)null,

                // Sol: nascer e pôr
                Sunrise = data.RootElement.TryGetProperty("sys", out var sysProp) && sysProp.TryGetProperty("sunrise", out var sunrise)
        ? sunrise.GetInt64()
        : (long?)null,
                Sunset = data.RootElement.TryGetProperty("sys", out var sysProp2) && sysProp2.TryGetProperty("sunset", out var sunset)
        ? sunset.GetInt64()
        : (long?)null
            };
        }

        public async Task<object> GetForecastAsync(string city)
        {
            var apiKey = _configuration["OpenWeather:ApiKey"];

            var response = await _httpClient.GetAsync(
                $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apiKey}&units=metric&lang=pt_br");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonDocument.Parse(json);

            var forecastList = data.RootElement.GetProperty("list").EnumerateArray()
                .Select(item => new
                {
                    dt = item.GetProperty("dt").GetInt64(),
                    temperature = item.GetProperty("main").GetProperty("temp").GetDouble(),
                    feelsLike = item.GetProperty("main").GetProperty("feels_like").GetDouble(),
                    humidity = item.GetProperty("main").GetProperty("humidity").GetInt32(),
                    windSpeed = item.GetProperty("wind").GetProperty("speed").GetDouble(),
                    windDeg = item.GetProperty("wind").GetProperty("deg").GetInt32(),
                    cloudiness = item.GetProperty("clouds").GetProperty("all").GetInt32(),
                    rain3h = item.TryGetProperty("rain", out var rainProp) && rainProp.TryGetProperty("3h", out var rain3h)
                        ? rain3h.GetDouble()
                        : (double?)null,
                    snow3h = item.TryGetProperty("snow", out var snowProp) && snowProp.TryGetProperty("3h", out var snow3h)
                        ? snow3h.GetDouble()
                        : (double?)null,
                    description = item.GetProperty("weather")[0].GetProperty("description").GetString(),
                    icon = item.GetProperty("weather")[0].GetProperty("icon").GetString()
                }).ToList();

            return new
            {
                city = data.RootElement.GetProperty("city").GetProperty("name").GetString(),
                list = forecastList
            };
        }
    }
}
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
                Description = data.RootElement.GetProperty("weather")[0].GetProperty("description").GetString()
            };
        }
    }
}
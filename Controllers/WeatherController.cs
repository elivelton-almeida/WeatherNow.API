using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherNow.API.Services;

namespace WeatherNow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("{city}")]
        public async Task<IActionResult> Get(string city)
        {
            try
            {
                var result = await _weatherService.GetWeatherAsync(city);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
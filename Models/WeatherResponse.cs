namespace WeatherNow.API.Models
{
    public class WeatherResponse
    {
        public string City { get; set; }
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public string Description { get; set; }
    }
}
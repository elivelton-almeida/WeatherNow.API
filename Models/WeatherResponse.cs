namespace WeatherNow.API.Models
{
    public class WeatherResponse
    {
        public string City { get; set; }
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public string Description { get; set; }

        public double? FeelsLike { get; set; }
        public double? TempMin { get; set; }
        public double? TempMax { get; set; }
        public int? Pressure { get; set; }

        public double? WindSpeed { get; set; }
        public int? WindDeg { get; set; }

        public int? Cloudiness { get; set; }

        public double? Rain1h { get; set; }
        public double? Rain3h { get; set; }
        public double? Snow1h { get; set; }
        public double? Snow3h { get; set; }

        public long? Sunrise { get; set; }
        public long? Sunset { get; set; }

        public string Icon { get; set; }
    }
}
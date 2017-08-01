using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Models
{
    public class WeatherModelJson
    {
        [JsonProperty("latitude")]
        public decimal Latitude { get; set; }

        [JsonProperty("longitude")]
        public decimal Longitude { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty("currently")]
        public Currently CurrentWeather { get; set; }

        [JsonProperty("hourly")]
        public Hourly HourlyWeather { get; set; }

        [JsonProperty("daily")]
        public Daily DailyWeather { get; set; }
    }

    public class Currently
    {
        [JsonProperty("time")]
        public int Time { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("nearestStormDistance")]
        public decimal NearestStormDistance { get; set; }

        [JsonProperty("precipProbability")]
        public decimal PrecipProbability { get; set; }

        [JsonProperty("temperature")]
        public decimal Temperature { get; set; }

        [JsonProperty("apparentTemperature")]
        public decimal FeelsLikeTemp { get; set; }

        [JsonProperty("humidity")]
        public decimal HumidityPercent { get; set; } // need to multiply by 100
    }

    public class Hourly
    {
        [JsonProperty("summary")]
        public string Summary { get; set; }

        List<HourlyData> HourlyData { get; set; }
    }

    public class HourlyData
    {
        [JsonProperty("time")]
        public int Time { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("precipProbability")]
        public decimal PrecipProbability { get; set; }

        [JsonProperty("temperature")]
        public decimal Temperature { get; set; }

        [JsonProperty("apperentTemperature")]
        public decimal FeelsLikeTemp { get; set; }

        [JsonProperty("humidity")]
        public decimal Humidity { get; set; }
    }

    public class Daily
    {
        [JsonProperty("summary")]
        public string Summary { get; set; }
        [JsonProperty("data")]
        public List<DailyData> DailyData { get; set; }
    }

    public class DailyData
    {
        [JsonProperty("time")]
        public int Time { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("sunriseTime")]
        public int SunriseTime { get; set; }

        [JsonProperty("sunsetTime")]
        public int SunsetTime { get; set; }

        [JsonProperty("precipProbability")]
        public decimal PrecipProbability { get; set; }

        [JsonProperty("temperatureMin")]
        public decimal TemperatureMin { get; set; }

        [JsonProperty("temperatureMax")]
        public decimal TemperatureMax { get; set; }

        [JsonProperty("apparentTemperatureMin")]
        public decimal ApparentTempMin { get; set; }

        [JsonProperty("apparentTemperatureMax")]
        public decimal ApparentTempMax { get; set; }

        [JsonProperty("humidity")]
        public decimal Humidity { get; set; }
    }
}

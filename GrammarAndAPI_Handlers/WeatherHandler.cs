using Jarvis.Helpers;
using Jarvis.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.GrammarAndAPI_Handlers
{
    public static class WeatherHandler
    {
        private static WeatherModelJson WeatherModel { get; set; }

        public static async Task GetWeatherAsync(decimal _latitude, decimal _longitude)
        {
            string key = "52f1f5c25626cd9f6dc0bda65c75a1aa";
            // set the uri with the passed in parameters
            string URI = "https://api.darksky.net/forecast/" + key + "/" + _latitude + "," + _longitude;

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(URI))
            using (HttpContent content = response.Content)
            {
                string jsonResponse = await content.ReadAsStringAsync();

                // bind the json properties to the weather model
                WeatherModel = JsonConvert.DeserializeObject<WeatherModelJson>(jsonResponse);
            }
        }

        public static string ParseWeatherDictation(IReadOnlyDictionary<string, IReadOnlyList<string>> _properties)
        {
            string response = "";

            if (_properties.ContainsKey("weatherDay") && !_properties.ContainsKey("weatherType"))
            {
                // this is for the full forecast of requested day
                string weatherDay = _properties["weatherDay"].First();

                response = GetWeatherResponseByDay(weatherDay);
            }
            else if (_properties.ContainsKey("weatherType"))
            {
                // this is the partial forecast dictated by what user asks for
                string weatherType = _properties["weatherType"].First();
                string weatherDay = _properties["weatherDays"].First();

                response = GetWeatherResponseByTypeOfWeather(weatherType, weatherDay);
            }

            return response;
        }

        private static string GetWeatherResponseByTypeOfWeather(string _type, string _day)
        {
            var dayIndex = DayToQuery(_day);
            var dayModel = WeatherModel.DailyWeather.DailyData[dayIndex];

            string day = "";
            string weatherType = "";

            if (_type == "rain")
            {
                weatherType = ((int)(dayModel.PrecipProbability * 100)).ToString();
            }

            if (_day == "today")
            {
                day = _day;
            }
            else
            {
                day = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(dayModel.Time)).DayOfWeek.ToString();
            }

            string response = "There is a " + weatherType + " percent chance of " + _type + " " + day + ".";

            return response;
        }

        private static string GetWeatherResponseByDay(string _day)
        {
            string day = "";
            string low = "";
            string high = "";
            string precip = "";
            string summary = "";
            string humidity = "";

            // get the dailyWeather data for the day requested
            var dayIndex = DayToQuery(_day);
            var dayModel = WeatherModel.DailyWeather.DailyData[dayIndex];

            if (_day == "today")
            {
                day = _day;
            }
            else
            {
                day = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt32(dayModel.Time)).DayOfWeek.ToString();
            }
            low = ((int)(dayModel.TemperatureMin)).ToString();
            high = ((int)(dayModel.TemperatureMax)).ToString();
            precip = ((int)(dayModel.PrecipProbability * 100)).ToString();
            summary = dayModel.Summary;
            humidity = ((int)(dayModel.Humidity * 100)).ToString();

            string response = "The weather for " + day + " is " + summary + " with a high of " + high + " degrees fahrenheit and a low of " + low + " degrees fahrenheit. There is a " + precip + " percent " +
                "chance of rain and " + humidity + " percent humidity.";

            System.Diagnostics.Debug.WriteLine(response);

            return response;
        }


        private static int DayToQuery(string _day)
        {
            var dayIndex = WeatherExtensions.FindDayIndex(DateTime.Today, WeatherExtensions.GetDayOfWeek(_day));

            return dayIndex;
        }
    }
}

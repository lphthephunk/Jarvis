using Jarvis.GrammarAndAPI_Handlers;
using Jarvis.Models;
using Jarvis.Models.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis
{
    public class HandleTags
    {
        private CoordinateHandler _coordinateHandler;

        public static UserModel UserData { get; set; }

        private decimal Latitude { get; set; }
        private decimal Longitude { get; set; }
        private string City { get; set; }
        private string State { get; set; }

        public async Task GetCoordinates()
        {
            _coordinateHandler = new CoordinateHandler();
            var coordsModel = await _coordinateHandler.GetCoordinatesModelAsync();

            // parse the model
            City = coordsModel.City;
            State = coordsModel.State;

            var latAndLng = coordsModel.Coordinates;
            string[] coordArray = new string[2];
            coordArray = latAndLng.Split(',');

            Latitude = Convert.ToDecimal(coordArray[0]);
            Longitude = Convert.ToDecimal(coordArray[1]);
        }

        /// <summary>
        /// Identifies the tag picked up by Jarvis. If tag cannot be recognized, returns false
        /// </summary>
        /// <param name="_tag">main context tag to start handling</param>
        /// <returns></returns>
        public async Task<string> IdentifyTag(IReadOnlyDictionary<string, IReadOnlyList<string>> _properties, string _tag)
        {
            string response = string.Empty;

            switch (_tag)
            {
                case "weather":
                    await WeatherHandler.GetWeatherAsync(Latitude, Longitude);
                    response = WeatherHandler.ParseWeatherDictation(_properties);
                    break;
                case "weatherType":
                    await WeatherHandler.GetWeatherAsync(Latitude, Longitude);
                    response = WeatherHandler.ParseWeatherDictation(_properties);
                    break;
                case "getBudget":
                    try
                    {
                        UserData = await UserHandler.GetUserData("1"); // TODO: handle logic to dynamically get different user data
                        response = await BudgetHandler.ParseBudgetDictation(_properties);
                    }
                    catch(Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                        response = "I have failed to retrieve your budget information. Please ensure that the main server is online.";
                    }
                    break;
                case "spotify":
                    // call spotify handler
                    response = "Still waiting on Spotify integration";
                    break;
                case "recalibrate":
                    await GetCoordinates();
                    System.Diagnostics.Debug.WriteLine("Latitude: {0}, Longitude: {1}", Latitude, Longitude);
                    response = "Successfully recalibrated";
                    break;
                case "sleep":
                    response = "Entering sleep mode";
                    break;
                case "stop":
                    response = string.Empty;
                    break;
                default:
                    response = "I didn't quite get that";
                    break;
            }

            return response;
        }
    }
}


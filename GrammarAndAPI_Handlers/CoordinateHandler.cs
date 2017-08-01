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
    public class CoordinateHandler
    {
        ///// <summary>
        ///// Get the coordinates of Jarvis' location based off of his public IP
        ///// </summary>
        ///// <returns></returns>
        //public async Task<List<CoordinatesJson>> GetCoordinatesAsync()
        //{
        //    var _coordinateModel = await GetCoordinatesModelAsync();

        //    string coordinates = _coordinateModel.Coordinates;

        //    // split the coordinates
        //    string[] splitCoords = coordinates.Split(',');

        //    List<CoordinatesJson> latAndLng = new List<CoordinatesJson>();      

        //    return latAndLng;
        //}

        /// <summary>
        /// Makes the web request for the coordinates of Jarvis
        /// </summary>
        /// <returns></returns>
        public async Task<CoordinatesJson> GetCoordinatesModelAsync()
        {
            // get the public ip address of this machine
            var ipAddress = await new HttpClient().GetStringAsync("https://api.ipify.org/");

            // setup the URI with the newly found ip address
            string URI = "http://ipinfo.io/" + ipAddress;
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(URI))
            using (HttpContent content = response.Content)
            {
                string jsonResponse = await content.ReadAsStringAsync();

                // bind the json properties to the coordinates-model
                return JsonConvert.DeserializeObject<CoordinatesJson>(jsonResponse);
            }
        }
    }
}

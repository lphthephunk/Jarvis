using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Models
{
    public class CoordinatesJson
    {
        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("region")]
        public string State { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("loc")]
        public string Coordinates { get; set; } // this will need to be split to retrieve both latitude and longitude

        [JsonProperty("postal")]
        public string ZipCode { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Models.Budget
{
    public class UserModel
    {
        [JsonProperty("idUsers")]
        public int UserId { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("Budget")]
        public decimal Budget { get; set; }
    }
}

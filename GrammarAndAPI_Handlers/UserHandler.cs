using Jarvis.Models.Budget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.GrammarAndAPI_Handlers
{
    public static class UserHandler
    {
        public static async Task<UserModel> GetUserData(string _userId)
        {
            using (HttpClient client = new HttpClient())
            {
                string uri = "http://192.168.1.72/ApiCalls/User.php/" + _userId;

                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                using (HttpResponseMessage response = await client.GetAsync(uri))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (HttpContent content = response.Content)
                        {
                            var jsonResponse = await content.ReadAsStringAsync();

                            return JsonConvert.DeserializeObject<UserModel>(jsonResponse);
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Couldn't get User Request");
                        return null;
                    }
                }
            }
        }
    }
}

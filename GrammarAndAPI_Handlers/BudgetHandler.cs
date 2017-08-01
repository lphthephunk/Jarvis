using Jarvis.Helpers;
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
    public static class BudgetHandler
    {
        public static List<Get_Budget> BudgetModel { get; set; }

        public static async Task<string> ParseBudgetDictation(IReadOnlyDictionary<string, IReadOnlyList<string>> _properties)
        {
            string response = string.Empty;

            try
            {
                // extract which month user is trying to get budget for
                if (_properties.ContainsKey("budgetOrSpent") && _properties.ContainsKey("timePeriod"))
                {
                    string type = _properties["budgetOrSpent"].First();
                    string timePeriod = _properties["timePeriod"].First();

                    // make call to GetBudget with specified month and set the BudgetModel static variable
                    await GetBudget(GetMonthNumber(timePeriod));

                    if (type == "budget" && timePeriod != "thisWeek")
                    {
                        return CreateBudgetResponseByMonth();
                    }
                    else if (type == "spent" && timePeriod == "thisWeek")
                    {
                        return CreateBudgetResponseThisWeek();
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Creates a budget response for the current week
        /// </summary>
        /// <returns></returns>
        private static string CreateBudgetResponseThisWeek()
        {
            return "Please access my web application for specific details like that";
        }

        /// <summary>
        /// This creates a response for the expenditures of the current user in the requested month
        /// </summary>
        /// <returns></returns>
        private static string CreateBudgetResponseByMonth()
        {
            var calculatedBudget = CalculateBudget.ReturnBudgetByMonth();

            if (calculatedBudget.Month == DateConversions.ConvertMonthIntToString(DateTime.Today.Month))
            {
                return "You have spent " + calculatedBudget.TotalExpensesThisMonth + " this month. You have " + calculatedBudget.AmountLeftForMonth +
                    " left for the month.";
            }

            return "For " + calculatedBudget.Month + ", you spent " + calculatedBudget.TotalExpensesThisMonth + ".";
        }

        private static async Task GetBudget(string _month)
        {
            string uri = "http://[Your server id]/ApiCalls/Budget.php/month=/"+_month+"/UserId=/"+HandleTags.UserData.UserId;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                using (HttpResponseMessage response = await client.GetAsync(uri))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (HttpContent content = response.Content)
                        {
                            string jsonResponse = await content.ReadAsStringAsync();

                            BudgetModel = JsonConvert.DeserializeObject<List<Get_Budget>>(jsonResponse);
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.Write("Failed to get Budget");
                    }
                }
            }
        }

        private static string GetMonthNumber(string _month)
        {
            switch (_month)
            {
                case "thisMonth":
                    return DateTime.Today.Month.ToString();
                case "lastMonth":
                    if (DateTime.Today.Month == 1)
                    {
                        return "12";
                    }
                    else
                    {
                        return (DateTime.Today.Month - 1).ToString();
                    }
                default:
                    return DateTime.Today.Month.ToString();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Helpers
{
    public static class WeatherExtensions
    {
        /// <summary>
        /// Find the index of the requested day based off of the current day
        /// </summary>
        /// <param name="today">index of current day</param>
        /// <param name="requestedDay">name of requested day of week</param>
        /// <returns></returns>
        public static int FindDayIndex(this DateTime today, DayOfWeek requestedDay)
        {
            int diff = 0;
            if (requestedDay == DayOfWeek.Sunday)
            {
                diff = (int)today.DayOfWeek - 7;
            }
            else
            {
                diff = today.DayOfWeek - requestedDay;

                if (diff > 0)
                {
                    // this would mean jarvis is searching for weather within the next week
                    DateTime currentDay = DateTime.Now;
                    int daysAway = 0;
                    while (currentDay.DayOfWeek != requestedDay)
                    {
                        currentDay = currentDay.AddDays(1);
                        daysAway++;
                    }
                    return daysAway;
                }
            }

            // multiply by one to make positive
            // this value will be the index of the requested date based off of the current day
            return (diff * -1);
        }

        public static DayOfWeek GetDayOfWeek(string _day)
        {
            DayOfWeek returnDay = DateTime.Today.DayOfWeek; // default to current day if _day cannot be understood

            switch (_day.ToLower())
            {
                case "monday":
                    returnDay = DayOfWeek.Monday;
                    break;
                case "tuesday":
                    returnDay = DayOfWeek.Tuesday;
                    break;
                case "wednesday":
                    returnDay = DayOfWeek.Wednesday;
                    break;
                case "thursday":
                    returnDay = DayOfWeek.Thursday;
                    break;
                case "friday":
                    returnDay = DayOfWeek.Friday;
                    break;
                case "saturday":
                    returnDay = DayOfWeek.Saturday;
                    break;
                case "sunday":
                    returnDay = DayOfWeek.Sunday;
                    break;
                case "today":
                    returnDay = DateTime.Today.DayOfWeek;
                    break;
                case "tomorrow":
                    returnDay = DateTime.Today.AddDays(1).DayOfWeek;
                    break;
            }

            return returnDay;
        }
    }
}

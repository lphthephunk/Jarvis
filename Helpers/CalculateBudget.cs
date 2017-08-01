using Jarvis.GrammarAndAPI_Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Helpers
{
    public static class CalculateBudget
    {
        /// <summary>
        /// This method will sum the expenditures on the server and compare them to my base budget limit
        /// </summary>
        /// <returns></returns>
        public static FullBudgetResponseModel ReturnBudgetByMonth()
        {
            var expenditures = BudgetHandler.BudgetModel;
            var budgetResponse = new FullBudgetResponseModel();

            foreach (var expense in expenditures)
            {
                budgetResponse.TotalExpensesThisMonth += expense.TotalCost;
            }

            budgetResponse.AmountLeftForMonth = HandleTags.UserData.Budget - budgetResponse.TotalExpensesThisMonth;
            budgetResponse.Month = DateConversions.ConvertMonthIntToString(expenditures.FirstOrDefault().PurchaseDate.Month);

            return budgetResponse;
        }
    }

    public class FullBudgetResponseModel
    {
        private decimal _totalExpensesThisMonth { get; set; }
        private decimal _amountLeftForMonth { get; set; }
        private string _month { get; set; }
        //private decimal _amountSpentForWeek { get; set; } // this value will vary depending on which week the user requests

        public decimal TotalExpensesThisMonth
        {
            get { return _totalExpensesThisMonth; }
            set
            {
                _totalExpensesThisMonth = value;
            }
        }

        public decimal AmountLeftForMonth
        {
            get { return _amountLeftForMonth; }
            set
            {
                _amountLeftForMonth = value;
            }
        }

        public string Month
        {
            get { return _month; }
            set
            {
                _month = value;
            }
        }

        //public decimal AmountSpentForWeek
        //{
        //    get { return _amountSpentForWeek; }
        //    set
        //    {
        //        _amountSpentForWeek = value;
        //    }
        //}
    }
}

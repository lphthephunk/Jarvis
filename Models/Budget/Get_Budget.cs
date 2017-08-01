using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Models.Budget
{
    public class Get_Budget
    {
        [JsonProperty("ReceiptType")]
        public string ReceiptType { get; set; }

        [JsonProperty("TotalCost")]
        public decimal TotalCost { get; set; }

        [JsonProperty("PurchaseDate")]
        public DateTime PurchaseDate { get; set; }

        [JsonProperty("BusinessName")]
        public string BusinessName { get; set; }
    }
}

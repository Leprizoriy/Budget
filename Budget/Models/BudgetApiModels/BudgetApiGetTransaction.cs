using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.Models.BudgetApiModels
{
    public class BudgetApiGetTransaction : BudgetApiBaseResponse
    {
        public int id { get; set; }
        public string type { get; set; }
        public DateTime date { get; set; }
        public string currency { get; set; }
        public double amount { get; set; }
        public int categoryId { get; set; }
        public BudgetApiGetProfileAction profileAction { get; set; }
        public int userId { get; set; }
    }
}

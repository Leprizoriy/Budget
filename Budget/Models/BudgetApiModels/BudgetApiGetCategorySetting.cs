using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.Models.BudgetApiModels
{
    public class BudgetApiGetCategorySetting : BudgetApiBaseResponse
    {
        public int id { get; set; }
        public double amount { get; set; }
        public int userId { get; set; }
        public int categoryId { get; set; }
    }
}

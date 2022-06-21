using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.Models.BudgetApiModels
{
    public class BudgetApiAddCategorySetting : BudgetApiBaseResponse
    {
        public int id { get; set; }
        public int categoryId { get; set; }
        public double amount { get; set; }
    }
}

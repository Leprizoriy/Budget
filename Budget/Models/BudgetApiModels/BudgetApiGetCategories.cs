using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.Models.BudgetApiModels
{
    public class BudgetApiGetCategories : BudgetApiBaseResponse
    {
        public List<BudgetApiGetCategory> categories { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.Models.BudgetApiModels
{
    public class BudgetApiGetCategorySettings : BudgetApiBaseResponse
    {
        public List<BudgetApiGetCategorySetting> categorySettings { get; set; }
    }
}

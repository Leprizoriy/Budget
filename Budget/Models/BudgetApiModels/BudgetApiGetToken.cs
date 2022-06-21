using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.Models.BudgetApiModels
{
    public class BudgetApiGetToken : BudgetApiBaseResponse
    {
        public string token { get; set; }
    }
}

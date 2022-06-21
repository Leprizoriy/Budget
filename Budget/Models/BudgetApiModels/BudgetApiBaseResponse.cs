using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.Models.BudgetApiModels
{
    public class BudgetApiBaseResponse
    {
        public bool hasErrors { get; set; }
        public string errorMessage { get; set; }
    }
}

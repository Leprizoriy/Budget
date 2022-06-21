using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.Models.BudgetApiModels
{
    public class BudgetApiGetProfileAction : BudgetApiBaseResponse
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}

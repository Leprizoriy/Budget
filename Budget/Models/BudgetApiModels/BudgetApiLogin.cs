using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.Models.BudgetApiModels
{
    public class BudgetApiLogin : BudgetApiBaseResponse
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

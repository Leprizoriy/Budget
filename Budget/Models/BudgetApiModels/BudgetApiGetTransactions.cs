using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.Models.BudgetApiModels
{
    public class BudgetApiGetTransactions : BudgetApiBaseResponse
    {
        public List<BudgetApiGetTransaction> transactions { get; set; }
    }
}

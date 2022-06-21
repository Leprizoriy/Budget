using Budget.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.Models.BudgetApiModels
{
    public class BudgetApiAddAction : BudgetApiBaseResponse
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public List<BudgetApiAddTransaction> Transactions { get; set; }
        public List<BudgetApiAddAttachment> Attachments { get; set; }
    }
}

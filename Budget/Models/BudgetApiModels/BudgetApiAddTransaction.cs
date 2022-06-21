using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Budget.Models.BudgetApiModels
{
    public class BudgetApiAddTransaction
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public int CategoryId { get; set; }
        public double Amount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.Models.BudgetApiModels
{
    public class BudgetApiAddCategory
    {
        public string Name { get; set; }
        public string Currency { get; set; }
        public int ParentId { get; set; }
    }
}
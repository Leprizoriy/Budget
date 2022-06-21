using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.Models.BudgetApiModels
{
    public class BudgetApiGetCategory : BudgetApiBaseResponse
    {
        public int id { get; set; }
        public string currency { get; set; }
        public string name { get; set; }
        public BudgetApiGetCategory parent { get; set; }
        public List<BudgetApiGetCategory> children { get; set; }
        public string userId { get; set; }
        public string user { get; set; }
        public string groupId { get; set; }
        public string group { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime? updatedDate { get; set; }
        public DateTime? deletedDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.ViewModels
{
    public class GetTransactionViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public GetProfileActionViewModel ProfileAction { get; set; }
        public int UserId { get; set; }
    }
}

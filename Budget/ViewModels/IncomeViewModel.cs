using Budget.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Budget.ViewModels
{
    public class IncomeViewModel
    {
        public List<GetTransactionViewModel> Transactions { get; set; }
        public ObservableCollection<GetCategoryViewModel> Categories { get; set; }
        public ObservableCollection<GetCategoryChildViewModel> Children { get; set; }
        public string Currency { get; set; }
    }
}

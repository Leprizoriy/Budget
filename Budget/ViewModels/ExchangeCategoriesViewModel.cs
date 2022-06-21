using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Budget.ViewModels
{
    public class ExchangeCategoriesViewModel : BindableBase
    {
        public ObservableCollection<GetCategoryViewModel> Categories { get; set; }
        public GetCategoryViewModel Category { get; set; }
        public GetCategoryViewModel Category2 { get; set; }
        public ObservableCollection<GetCategoryChildViewModel> SubCategories { get; set; }
        public GetCategoryChildViewModel SubCategory { get; set; }
        public ObservableCollection<GetCategoryChildViewModel> SubCategories2 { get; set; }
        public GetCategoryChildViewModel SubCategory2 { get; set; }
        public ObservableCollection<string> Currencies { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
    }
}

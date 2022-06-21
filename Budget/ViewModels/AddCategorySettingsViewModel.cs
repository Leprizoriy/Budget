using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Budget.ViewModels
{
    public class AddCategorySettingsViewModel : BindableBase
    {
        public ObservableCollection<string> Currencies { get; set; }
        public string Currency { get; set; }
        public ObservableCollection<GetCategoryViewModel> Categories { get; set; }
        public GetCategoryViewModel Category { get; set; }
        public double Amount { get; set; }
        public bool YesOrNo { get; set; }
    }
}

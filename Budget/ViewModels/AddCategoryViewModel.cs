using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Budget.ViewModels
{
    public class AddCategoryViewModel : BindableBase
    {
        public ObservableCollection<string> Currencies { get; set; }
        public string Currency { get; set; }
        public ObservableCollection<GetCategoryViewModel> Categories { get; set; }
        public List<AddCategoryViewModel> CategoryAmounts { get; set; }
        public GetCategoryViewModel Category { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
    }
}

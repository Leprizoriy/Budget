using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Budget.ViewModels
{
    public class SettingsViewModel
    {
        public ObservableCollection<GetCategorySettingViewModel> CategorySettings { get; set; }
        public ObservableCollection<string> Currencies { get; set; }
        public string Currency { get; set; }
    }
}

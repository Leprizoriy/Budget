using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Budget.ViewModels
{
    public class ExchangeCurrencyMoneyViewModel : BindableBase
    {
        public ObservableCollection<GetCurrencyCourseViewModel> CurrencyCourses { get; set; }
        public double SelectedCurrencyAmount { get; set; }
    }
}

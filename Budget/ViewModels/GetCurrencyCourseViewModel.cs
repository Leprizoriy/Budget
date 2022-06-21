using System;
using System.Collections.Generic;
using System.Text;

namespace Budget.ViewModels
{
    public class GetCurrencyCourseViewModel : BindableBase
    {
        private string currencyFrom;
        private string currencyTo;
        private double course;
        private double amountFrom;
        private double amountTo;

        public string CurrencyFrom { get => currencyFrom; set => SetProperty(ref currencyFrom, value); }
        public string CurrencyTo { get => currencyTo; set => SetProperty(ref currencyTo, value); }
        public double Course { get => course; set => SetProperty(ref course, value); }
        public double AmountFrom { get => amountFrom; set => SetProperty(ref amountFrom, value); }
        public double AmountTo { get => amountTo; set => SetProperty(ref amountTo, value); }
    }
}

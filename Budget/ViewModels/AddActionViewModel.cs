using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media;

namespace Budget.ViewModels
{
    public class AddActionViewModel : BindableBase
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public ObservableCollection<string> Currencies { get; set; }
        public string Currency { get; set; }
        public ObservableCollection<AddTransactionViewModel> Transactions { get; set; }
        //public List<string> TransactionTypes { get; set; }
        //public string TransactionType { get; set; }

        double amountLeft;
        private ImageSource calendarImage;

        public double AmountLeft
        {
            get => amountLeft;
            set => SetProperty(ref amountLeft, value);
        }

        public ImageSource CalendarImage { get => calendarImage; set => SetProperty(ref calendarImage, value); }
    }
}

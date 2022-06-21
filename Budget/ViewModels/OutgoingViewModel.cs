using Budget.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media;

namespace Budget.ViewModels
{
    public class OutgoingViewModel : BindableBase
    {
        private string currency;
        private string transactionType;
        private DateTime dateFrom;
        private DateTime dateTo;
        private GetCategoryViewModel category;
        private ImageSource calendarImage;
        private ImageSource clearFilterImage;
        private ImageSource searchImage;

        public ObservableCollection<GetTransactionViewModel> Transactions { get; set; }
        public ObservableCollection<GetCategoryViewModel> Categories { get; set; }
        public GetCategoryViewModel Category { get => category; set => SetProperty(ref category, value); }
        public ObservableCollection<string> Currencies { get; set; }
        public string Currency { get => currency; set => SetProperty(ref currency, value); }
        public List<string> TransactionTypes { get; set; }
        public string TransactionType { get => transactionType; set => SetProperty(ref transactionType, value); }
        public DateTime DateFrom { get => dateFrom; set => SetProperty(ref dateFrom, value); }
        public DateTime DateTo { get => dateTo; set => SetProperty(ref dateTo, value); }

        public ImageSource CalendarImage { get => calendarImage; set => SetProperty(ref calendarImage, value); }
        public ImageSource ClearFilterImage { get => clearFilterImage; set => SetProperty(ref clearFilterImage, value); }
        public ImageSource SearchImage { get => searchImage; set => SetProperty(ref searchImage, value); }
    }
}

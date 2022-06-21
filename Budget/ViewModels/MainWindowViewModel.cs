using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media;

namespace Budget.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private double uahAmount;
        private double usdAmount;
        private double eurAmount;
        private double plnAmount;
        private double summAmount;
        private ImageSource collapseImage;

        public double UahAmount { get => uahAmount; set => SetProperty(ref uahAmount, value); }
        public double UsdAmount { get => usdAmount; set => SetProperty(ref usdAmount, value); }
        public double EurAmount { get => eurAmount; set => SetProperty(ref eurAmount, value); }
        public double PlnAmount { get => plnAmount; set => SetProperty(ref plnAmount, value); }
        public double SummAmount { get => summAmount; set => SetProperty(ref summAmount, value); }
        public ObservableCollection<string> Currencies { get; set; }
        public string Currency { get; set; }

        public ImageSource CollapseImage { get => collapseImage; set => SetProperty(ref collapseImage, value); }
    }
}

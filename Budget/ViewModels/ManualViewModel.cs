using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Budget.ViewModels
{
    public class ManualViewModel : BindableBase
    {
        private ImageSource addActionWindow;
        private ImageSource addNewCategory;
        private ImageSource addSettingWindow;
        private ImageSource addTransactionWindow;
        private ImageSource currencyCourseWindow;
        private ImageSource deleteCategory;
        private ImageSource distributionWindow;
        private ImageSource exchangeCategory;
        private ImageSource incomeView;
        private ImageSource mainWindow;
        private ImageSource outgoingFilter;
        private ImageSource outgoingView;
        private ImageSource settingsView;

        public ImageSource AddActionWindowImage { get => addActionWindow; set => SetProperty(ref addActionWindow, value); }
        public ImageSource AddNewCategoryImage { get => addNewCategory; set => SetProperty(ref addNewCategory, value); }
        public ImageSource AddSettingWindowImage { get => addSettingWindow; set => SetProperty(ref addSettingWindow, value); }
        public ImageSource AddTransactionWindowImage { get => addTransactionWindow; set => SetProperty(ref addTransactionWindow, value); }
        public ImageSource CurrencyCourseWindowImage { get => currencyCourseWindow; set => SetProperty(ref currencyCourseWindow, value); }
        public ImageSource DeleteCategoryImage { get => deleteCategory; set => SetProperty(ref deleteCategory, value); }
        public ImageSource DistributionWindowImage { get => distributionWindow; set => SetProperty(ref distributionWindow, value); }
        public ImageSource ExchangeCategoryImage { get => exchangeCategory; set => SetProperty(ref exchangeCategory, value); }
        public ImageSource IncomeViewImage { get => incomeView; set => SetProperty(ref incomeView, value); }
        public ImageSource MainWindowImage { get => mainWindow; set => SetProperty(ref mainWindow, value); }
        public ImageSource OutgoingFilterImage { get => outgoingFilter; set => SetProperty(ref outgoingFilter, value); }
        public ImageSource OutgoingViewImage { get => outgoingView; set => SetProperty(ref outgoingView, value); }
        public ImageSource SettingsViewImage { get => settingsView; set => SetProperty(ref settingsView, value); }
    }
}

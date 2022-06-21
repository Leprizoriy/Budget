using Budget.Services;
using Budget.ViewModels;
using System;
using System.IO;
using System.Windows.Controls;

namespace Budget.Views
{
    /// <summary>
    /// Interaction logic for ManualView.xaml
    /// </summary>
    public partial class ManualView : UserControl
    {
        private readonly ManualViewModel _vm;
        private ImageService _imageService;

        public ManualView()
        {
            InitializeComponent();
            _vm = new ManualViewModel();
            _imageService = new ImageService();

            _vm.AddActionWindowImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "AddActionWindow.png"));
            _vm.AddNewCategoryImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "AddNewCategory.png"));
            _vm.AddSettingWindowImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "AddSettingWindow.png"));
            _vm.AddTransactionWindowImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "AddTransactionWindow.png"));
            _vm.CurrencyCourseWindowImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "CurrencyCourseWindow.png"));
            _vm.DeleteCategoryImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "DeleteCategory.png"));
            _vm.DistributionWindowImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "DistributionWindow.png"));
            _vm.ExchangeCategoryImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "ExchangeCategory.png"));
            _vm.IncomeViewImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "IncomeView.png"));
            _vm.MainWindowImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "MainWindow.png"));
            _vm.OutgoingFilterImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "OutgoingFilter.png"));
            _vm.OutgoingViewImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "OutgoingView.png"));
            _vm.SettingsViewImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "SettingsView.png"));

            this.DataContext = _vm;
        }
    }
}

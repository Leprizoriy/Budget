using Budget.Extensions;
using Budget.Services;
using Budget.ViewModels;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Budget.Views
{
    /// <summary>
    /// Interaction logic for WarningWindow.xaml
    /// </summary>
    public partial class WarningWindow : Window
    {
        private readonly MessageViewModel _vm;
        private readonly DeleteCategoryViewModel _deleteCategoryViewModel;
        private ImageService _imageService;

        public WarningWindow(string message, DeleteCategoryViewModel deleteCategoryViewModel)
        {
            InitializeComponent();
            _vm = new MessageViewModel();
            _imageService = new ImageService();

            if (!string.IsNullOrEmpty(message.TranslateErrorMessageToRU())) _vm.Message = message.TranslateErrorMessageToRU();
            else _vm.Message = message;

            _vm.LogoImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "budget.png"));
            _vm.CloseImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "close.png"));

            this.DataContext = _vm;
            _deleteCategoryViewModel = deleteCategoryViewModel;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            _deleteCategoryViewModel.YesOrNo = false;
            Close();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            _deleteCategoryViewModel.YesOrNo = true;
            Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            _deleteCategoryViewModel.YesOrNo = false;
            Close();
        }
    }
}

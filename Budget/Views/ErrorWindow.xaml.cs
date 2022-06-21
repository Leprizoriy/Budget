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
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        private readonly MessageViewModel _vm;
        private ImageService _imageService;

        public ErrorWindow(string error)
        {
            InitializeComponent();
            _vm = new MessageViewModel();
            _imageService = new ImageService();

            if (!string.IsNullOrEmpty(error.TranslateErrorMessageToRU())) _vm.Message = error.TranslateErrorMessageToRU();
            else _vm.Message = error;

            _vm.LogoImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "budget.png"));
            _vm.CloseImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "close.png"));

            this.DataContext = _vm;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

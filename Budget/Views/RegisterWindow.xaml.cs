using Budget.Models.BudgetApiModels;
using Budget.Services;
using Budget.ViewModels;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Budget.Views
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private readonly RegistrationViewModel _vm;
        private readonly BudgetApiClient _budgetApiClient;
        private ImageService _imageService;

        public RegisterWindow()
        {
            InitializeComponent();
            _vm = new RegistrationViewModel();
            _budgetApiClient = new BudgetApiClient();
            _imageService = new ImageService();

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

        private async void registerUserButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string errorMessage = CheckRequiredFields();
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    var error = new ErrorWindow(errorMessage);
                    error.ShowDialog();
                }
                else
                {
                    var login = await _budgetApiClient.RegisterUser(new BudgetApiRegistration
                    {
                        Email = _vm.Email,
                        FirstName = _vm.FirstName,
                        LastName = _vm.LastName,
                        Password = _vm.Password,
                        UserName = _vm.UserName
                    });

                    if (!login.hasErrors) GoToLogin();
                    else
                    {
                        var error = new ErrorWindow(login.errorMessage);
                        error.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }

        }

        private string CheckRequiredFields()
        {
            string errorMessage = string.Empty;

            if (string.IsNullOrEmpty(_vm.Email))
                errorMessage = "Empty email";
            else if (string.IsNullOrEmpty(_vm.FirstName))
                errorMessage = "Empty first name";
            else if (string.IsNullOrEmpty(_vm.LastName))
                errorMessage = "Empty last name";
            else if (string.IsNullOrEmpty(_vm.UserName))
                errorMessage = "Empty user name";
            else if (string.IsNullOrEmpty(_vm.Password))
                errorMessage = "Empty password";

            return errorMessage;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
                ((RegistrationViewModel)this.DataContext).Password = ((PasswordBox)sender).Password;
        }

        private void Login_Click(object sender, MouseButtonEventArgs e)
        {
            GoToLogin();
        }

        private void GoToLogin()
        {
            try
            {
                var login = new LoginWindow();
                login.Show();
                Close();
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }
    }
}

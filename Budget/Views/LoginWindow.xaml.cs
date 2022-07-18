using Budget.Models;
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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _vm;
        private readonly BudgetApiClient _budgetApiClient;
        private ImageService _imageService;

        public LoginWindow()
        {
            InitializeComponent();

            try
            {
                _vm = new LoginViewModel();
                _budgetApiClient = new BudgetApiClient();
                _imageService = new ImageService();
                var user = Config.Load();
                _vm.UserName = user?.AssosiatedCredemtials?.UserName;
                _vm.Password = user?.AssosiatedCredemtials?.Password;
                if (user?.AssosiatedCredemtials?.RememberMe != null) _vm.RememberMe = (bool)user?.AssosiatedCredemtials?.RememberMe;

                if (!string.IsNullOrEmpty(_vm.UserName) && !string.IsNullOrEmpty(_vm.Password))
                {
                    GoToMain();
                }

                _vm.LogoImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "budget.png"));
                _vm.CloseImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "close.png"));

                this.DataContext = _vm;
            }
            catch (Exception exc)
            {
                FileLogger.ReportToLog($"[LoginWindow]", $"Error:[{exc.Message}]", exc);
            }
        }

        private void Register_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var register = new RegisterWindow();
                register.Show();
                Close();
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void LoginUserButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginUser();
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
                ((LoginViewModel)this.DataContext).Password = ((PasswordBox)sender).Password;
        }

        private async void LoginUser()
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
                    var login = await _budgetApiClient.LoginUser(new BudgetApiLogin
                    {
                        UserName = _vm.UserName,
                        Password = _vm.Password
                    });

                    if (!login.hasErrors)
                    {
                        var user = new User
                        {
                            Token = login.token,
                            ExpirationTokenTime = DateTime.Now.AddMinutes(15),
                            AssosiatedCredemtials = new LoginViewModel
                            {
                                UserName = _vm.UserName,
                                Password = _vm.RememberMe ? _vm.Password : null,
                                RememberMe = _vm.RememberMe
                            }
                        };
                        Config.Save(user);
                        GoToMain();
                    }
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

            if (string.IsNullOrEmpty(_vm.UserName))
                errorMessage = "Empty user name";
            else if (string.IsNullOrEmpty(_vm.Password))
                errorMessage = "Empty password";

            return errorMessage;
        }

        private void GoToMain()
        {
            try
            {
                var main = new MainWindow();
                main.Show();
                Close();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The invocation of the constructor on type 'Budget.Views.IncomeView' that matches the specified binding constraints threw an exception."))
                {
                    string message = "Server connection error";
                    var error = new ErrorWindow(message);
                    error.ShowDialog();
                }
                else
                {
                    var error = new ErrorWindow(ex.Message);
                    error.ShowDialog();
                }
            }
        }

        private void RememberMe_Click(object sender, MouseButtonEventArgs e)
        {
            if (_vm.RememberMe == false)
                _vm.RememberMe = true;
            else _vm.RememberMe = false;
        }
    }
}

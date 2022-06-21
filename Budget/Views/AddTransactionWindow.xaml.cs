using Budget.Models;
using Budget.Services;
using Budget.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Budget.Views
{
    /// <summary>
    /// Interaction logic for AddTransactionWindow.xaml
    /// </summary>
    public partial class AddTransactionWindow : Window
    {
        private readonly AddActionViewModel _vm;
        private readonly IncomeViewModel _incomeViewModel;
        private readonly BudgetApiClient _budgetApiClient;
        private List<string> _currencies;
        private ImageService _imageService;

        public AddTransactionWindow(IncomeViewModel incomeViewModel)
        {
            InitializeComponent();
            var user = Config.Load();
            _vm = new AddActionViewModel();
            _imageService = new ImageService();
            _budgetApiClient = new BudgetApiClient(user.Token);
            _vm.Currencies = new ObservableCollection<string>();

            GetAvailableCurrencies().GetAwaiter().GetResult();

            foreach (var item in _currencies) _vm.Currencies.Add(item);

            _vm.Date = DateTime.Today;
            _vm.LogoImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "budget.png"));
            _vm.CloseImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "close.png"));
            _vm.CalendarImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "calendar.png"));

            this.DataContext = _vm;
            _incomeViewModel = incomeViewModel;
        }

        private Task GetAvailableCurrencies()
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_vm != null)
                    {
                        _currencies = new List<string>();
                        var res = await _budgetApiClient.GetAvailableCurrencies();
                        if (res != null) foreach (var item in res) _currencies.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    var error = new ErrorWindow(ex.Message);
                    error.ShowDialog();
                }
            });
        }

        private void AddTransaction_Click(object sender, RoutedEventArgs e)
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

                    var distribution = new DistributionWindow(_incomeViewModel, _vm.Name, _vm.Date, _vm.Amount, _vm.Currency);
                    distribution.ShowDialog();
                    Close();
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

            if (string.IsNullOrEmpty(_vm.Name))
                errorMessage = "Empty name";
            else if (_vm.Amount == 0)
                errorMessage = "Amount must be greater than 0";
            else if (string.IsNullOrEmpty(_vm.Currency))
                errorMessage = "Empty currency";

            return errorMessage;
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Source is TextBox ue)
            {
                if (e.Text == ",")
                {
                    var error = new InfoWindow("Please use a dot instead of a comma");
                    error.ShowDialog();
                }

                Regex regex;
                if (ue.Text.Contains(".")) regex = new Regex("[^0-9]+");
                else regex = new Regex("[^0-9.]+");

                if (ue.Text.Contains(".") && ue.Text.Split('.')[1].Length < 2)
                    e.Handled = regex.IsMatch(e.Text);
                else if (!ue.Text.Contains(".")) e.Handled = regex.IsMatch(e.Text);
                else e.Handled = true;
            }
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

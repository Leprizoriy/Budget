using Budget.Extensions;
using Budget.Models;
using Budget.Services;
using Budget.ViewModels;
using Budget.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Budget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window, RecalculateCallBack
    {
        private readonly MainWindowViewModel _vm;
        private readonly BudgetApiClient _budgetApiClient;
        private List<GetTransactionViewModel> _transactions;
        private List<string> _currencies;
        private ImageService _imageService;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                var user = Config.Load();
                _vm = new MainWindowViewModel();
                _budgetApiClient = new BudgetApiClient(user.Token);
                _imageService = new ImageService();
                _vm.Currencies = new ObservableCollection<string>();
                RecalculateTopBarService.recalculateCallBack = this;
                GetTransactions().GetAwaiter().GetResult();
                GetAvailableCurrencies().GetAwaiter().GetResult();
                //SumCurrencyAmount();

                foreach (var item in _currencies) _vm.Currencies.Add(item);

                _vm.LogoImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "budget.png"));
                _vm.CloseImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "close.png"));
                _vm.CollapseImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "collapse.png"));
                this.DataContext = _vm;
            }
            catch (Exception exc)
            {
                FileLogger.ReportToLog($"[MainWindow]", $"Error:[{exc.Message}]", exc);
            }         
        }

        private Task GetTransactions()
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_vm != null)
                    {
                        _transactions = new List<GetTransactionViewModel>();
                        var res = await _budgetApiClient.GetTransactions();
                        if (!res.hasErrors)
                        {
                            foreach (var item in res.transactions)
                            {
                                GetTransactionViewModel transaction = new GetTransactionViewModel();
                                transaction.Id = item.id;
                                transaction.Type = item.type;
                                transaction.Date = item.date;
                                transaction.Currency = item.currency;
                                transaction.Amount = item.amount.TwoSymbolsAfterDot();
                                transaction.CategoryId = item.categoryId;
                                transaction.UserId = item.userId;
                                transaction.ProfileAction = new GetProfileActionViewModel();
                                transaction.ProfileAction.Id = item.profileAction.id;
                                transaction.ProfileAction.Name = item.profileAction.name;

                                _transactions.Add(transaction);
                            }
                        }
                        else
                        {
                            var error = new ErrorWindow(res.errorMessage);
                            error.ShowDialog();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var error = new ErrorWindow(ex.Message);
                    error.ShowDialog();
                }
            });
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

        private void SumCurrencyAmount()
        {
            try
            {
                _vm.UahAmount = _transactions.FindAll(f => f.Currency == "UAH").Sum(s => s.Amount).TwoSymbolsAfterDot();
                _vm.UsdAmount = _transactions.FindAll(f => f.Currency == "USD").Sum(s => s.Amount).TwoSymbolsAfterDot();
                _vm.EurAmount = _transactions.FindAll(f => f.Currency == "EUR").Sum(s => s.Amount).TwoSymbolsAfterDot();
                _vm.PlnAmount = _transactions.FindAll(f => f.Currency == "PLN").Sum(s => s.Amount).TwoSymbolsAfterDot();
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Collapse_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowCurrencyCourse(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                var exchangeCurrencyMoney = new ExchangeCurrencyMoneyWindow(_vm);
                if (exchangeCurrencyMoney.ShowDialog() != true) return;
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }

        public void Recalculate(List<GetTransactionViewModel> transactions)
        {
            try
            {
                _vm.UahAmount = transactions.FindAll(f => f.Currency == "UAH").Sum(s => s.Amount).TwoSymbolsAfterDot();
                _vm.UsdAmount = transactions.FindAll(f => f.Currency == "USD").Sum(s => s.Amount).TwoSymbolsAfterDot();
                _vm.EurAmount = transactions.FindAll(f => f.Currency == "EUR").Sum(s => s.Amount).TwoSymbolsAfterDot();
                _vm.PlnAmount = transactions.FindAll(f => f.Currency == "PLN").Sum(s => s.Amount).TwoSymbolsAfterDot();
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }
    }
}

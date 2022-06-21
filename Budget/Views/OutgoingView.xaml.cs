using Budget.Extensions;
using Budget.Models;
using Budget.Services;
using Budget.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Budget.Views
{
    /// <summary>
    /// Interaction logic for OutputView.xaml
    /// </summary>
    public partial class OutgoingView : UserControl
    {
        private readonly OutgoingViewModel _vm;
        private readonly BudgetApiClient _budgetApiClient;
        private List<GetTransactionViewModel> transactions;
        private List<GetCategoryViewModel> _categories;
        private List<string> _currencies;
        private ImageService _imageService;

        public OutgoingView()
        {
            InitializeComponent();

            var user = Config.Load();
            _vm = new OutgoingViewModel();
            _budgetApiClient = new BudgetApiClient(user.Token);
            _imageService = new ImageService();
            _vm.Transactions = new ObservableCollection<GetTransactionViewModel>();
            _vm.Categories = new ObservableCollection<GetCategoryViewModel>();
            _vm.Currencies = new ObservableCollection<string>();
            transactions = new List<GetTransactionViewModel>();

            GetAvailableCategories().GetAwaiter().GetResult();
            GetTransactions().GetAwaiter().GetResult();
            GetAvailableTransactionTypes().GetAwaiter().GetResult();
            GetAvailableCurrencies().GetAwaiter().GetResult();
            foreach (var transaction in transactions) _vm.Transactions.Add(transaction);
            foreach (var item in _currencies) _vm.Currencies.Add(item);

            _vm.DateFrom = DateTime.Today;
            _vm.DateTo = DateTime.Today;
            _vm.CalendarImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "calendar.png"));
            _vm.ClearFilterImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "clear_filter.png"));
            _vm.SearchImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "search.png"));

            this.DataContext = _vm;
        }

        private void AddTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newAction = new AddActionWindow(_vm);
                if (newAction.ShowDialog() != true)
                {
                    this.DataContext = _vm;
                    return;
                }
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
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
                        transactions = new List<GetTransactionViewModel>();
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

                                GetCategoryViewModel category = _categories.ToList().Find(f => f.Id == item.categoryId);
                                if (category != null) transaction.CategoryName = category.Name;

                                transactions.Add(transaction);
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

        private Task GetAvailableCategories()
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_vm != null)
                    {
                        _categories = new List<GetCategoryViewModel>();
                        var res = await _budgetApiClient.GetAvailableCategories();
                        if (!res.hasErrors)
                        {
                            foreach (var item in res.categories)
                            {
                                GetCategoryViewModel category = new GetCategoryViewModel();
                                category.Id = item.id;
                                category.Name = item.name;
                                category.Currency = item.currency;
                                if (item.parent != null) category.ParentId = item.parent.id;

                                _categories.Add(category);
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

        private Task GetAvailableTransactionTypes()
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_vm != null)
                    {
                        _vm.TransactionTypes = new List<string>();
                        var res = await _budgetApiClient.GetAvailableTransactionTypes();
                        if (res != null) foreach (var item in res) _vm.TransactionTypes.Add(item);
                    }

                }
                catch (Exception ex)
                {
                    var error = new ErrorWindow(ex.Message);
                    error.ShowDialog();
                }
            });
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _vm.Transactions.Clear();
                List<GetTransactionViewModel> transactionsFilter = new List<GetTransactionViewModel>();

                if (_vm.TransactionType == null && _vm.Category == null && _vm.Currency == null)
                    transactionsFilter = FindTransactions();
                else if (_vm.TransactionType != null && _vm.Category == null && _vm.Currency == null)
                    transactionsFilter = FindTransactions(_vm.TransactionType);
                else if (_vm.TransactionType == null && _vm.Category != null && _vm.Currency == null)
                    transactionsFilter = FindTransactionsByCategory(_vm.Category.Name);
                else if (_vm.TransactionType == null && _vm.Category == null && _vm.Currency != null)
                    transactionsFilter = FindTransactionsByCurrency(_vm.Currency);
                else if (_vm.TransactionType != null && _vm.Category != null && _vm.Currency == null)
                    transactionsFilter = FindTransactionsByTransTypeAndCatName(_vm.TransactionType, _vm.Category.Name);
                else if (_vm.TransactionType == null && _vm.Category != null && _vm.Currency != null)
                    transactionsFilter = FindTransactionsByCurrencyByCatIdAndCur(_vm.Category.Name, _vm.Currency);
                else if (_vm.TransactionType != null && _vm.Category == null && _vm.Currency != null)
                    transactionsFilter = FindTransactions(_vm.TransactionType, _vm.Currency);
                else transactionsFilter = FindTransactions(_vm.TransactionType, _vm.Category.Name, _vm.Currency);

                if (transactionsFilter.Any()) foreach (var transactionFilter in transactionsFilter)
                    {
                        _vm.Transactions.Add(transactionFilter);
                    }
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }

        private List<GetTransactionViewModel> FindTransactions ()
        {
            return transactions.FindAll(f => f.Date >= _vm.DateFrom && f.Date <= _vm.DateTo);
        }

        private List<GetTransactionViewModel> FindTransactions(string transactionType)
        {
            return transactions.FindAll(f => f.Type == transactionType && f.Date >= _vm.DateFrom && f.Date <= _vm.DateTo);
        }

        private List<GetTransactionViewModel> FindTransactionsByCategory(string categoryName)
        {
            return transactions.FindAll(f => f.Date >= _vm.DateFrom && f.Date <= _vm.DateTo && f.CategoryName == categoryName);
        }

        private List<GetTransactionViewModel> FindTransactionsByCurrency(string currency)
        {
            return transactions.FindAll(f => f.Date >= _vm.DateFrom && f.Date <= _vm.DateTo && f.Currency == currency);
        }

        private List<GetTransactionViewModel> FindTransactionsByTransTypeAndCatName(string transactionType, string categoryName)
        {
            return transactions.FindAll(f => f.Type == transactionType && f.Date >= _vm.DateFrom && f.Date <= _vm.DateTo && f.CategoryName == categoryName);
        }

        private List<GetTransactionViewModel> FindTransactionsByCurrencyByCatIdAndCur(string categoryName, string currency)
        {
            return transactions.FindAll(f => f.Date >= _vm.DateFrom && f.Date <= _vm.DateTo && f.CategoryName == categoryName && f.Currency == currency);
        }

        private List<GetTransactionViewModel> FindTransactions(string transactionType, string currency)
        {
            return transactions.FindAll(f => f.Type == transactionType && f.Date >= _vm.DateFrom && f.Date <= _vm.DateTo && f.Currency == currency);
        }

        private List<GetTransactionViewModel> FindTransactions(string transactionType, string categoryName, string currency)
        {
            return transactions.FindAll(f => f.Type == transactionType && f.Date >= _vm.DateFrom && f.Date <= _vm.DateTo && f.CategoryName == categoryName && f.Currency == currency);
        }

        private async void ListUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _vm.Transactions.Clear();
                await GetAvailableCategories();
                await GetTransactions();
                await GetAvailableTransactionTypes();
                foreach (var transaction in transactions) _vm.Transactions.Add(transaction);
                this.DataContext = _vm;
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _vm.TransactionType = null;
                _vm.Category = null;
                _vm.DateFrom = DateTime.Today;
                _vm.DateTo = DateTime.Today;
                _vm.Currency = null;

                _vm.Transactions.Clear();
                foreach (var transaction in transactions) _vm.Transactions.Add(transaction);

                this.DataContext = _vm;
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }

        private async void DeleteTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var transaction = TransactionsListView.SelectedItem as GetTransactionViewModel;
                if (transaction == null)
                {
                    var error = new ErrorWindow("Transaction was not selected");
                    error.ShowDialog();
                }
                else
                {
                    var res = await _budgetApiClient.DeleteTransaction(transaction.Id);
                    if (!res.hasErrors)
                    {
                        ((ObservableCollection<GetTransactionViewModel>)TransactionsListView.ItemsSource).Remove(transaction);
                        RecalculateTopBarService.RecalculateTopBar(_vm.Transactions.ToList());
                    }
                    else
                    {
                        var error = new ErrorWindow(res.errorMessage);
                        error.ShowDialog();
                    }

                    this.DataContext = _vm;
                }
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }

        private void ShowCategories(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _vm.Categories.Clear();
                var categories = _categories.Where(w => w.Currency == _vm.Currency && w.ParentId == 0);
                foreach (var category in categories) _vm.Categories.Add(category);

                this.DataContext = _vm;
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }
    }
}

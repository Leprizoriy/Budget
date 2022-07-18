using Budget.Extensions;
using Budget.Models;
using Budget.Models.BudgetApiModels;
using Budget.Services;
using Budget.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Budget.Views
{
    public partial class AddActionWindow : Window
    {
        private readonly AddActionViewModel _vm;
        private readonly OutgoingViewModel _outgoingViewModel;
        private readonly BudgetApiClient _budgetApiClient;
        private List<GetCategoryViewModel> _categories;
        private List<string> _currencies;
        private ImageService _imageService;

        public AddActionWindow(OutgoingViewModel outgoingViewModel)
        {
            InitializeComponent();

            var user = Config.Load();
            _vm = new AddActionViewModel();
            _budgetApiClient = new BudgetApiClient(user.Token);
            _imageService = new ImageService();
            _vm.Transactions = new ObservableCollection<AddTransactionViewModel>();

            GetAvailableCurrencies().GetAwaiter().GetResult();
            GetAvailableCategories().GetAwaiter().GetResult();

            var transaction = new AddTransactionViewModel { Amount = 0 };
            transaction.Categories = new ObservableCollection<GetCategoryViewModel>();
            transaction.SubCategories = new ObservableCollection<GetCategoryChildViewModel>();
            transaction.Currencies = new ObservableCollection<string>();

            foreach (var item in _currencies) transaction.Currencies.Add(item);

            _vm.Date = DateTime.Today;
            _vm.Transactions.Add(transaction);

            _vm.LogoImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "budget.png"));
            _vm.CloseImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "close.png"));
            _vm.CalendarImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "calendar.png"));

            this.DataContext = _vm;
            _outgoingViewModel = outgoingViewModel;
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
                catch (Exception e)
                {
                    var error = new ErrorWindow(e.Message);
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
                                category.Children = new List<GetCategoryChildViewModel>();
                                if (item.children != null) foreach (var child in item?.children)
                                    {
                                        GetCategoryChildViewModel categoryChild = new GetCategoryChildViewModel();
                                        categoryChild.Id = child.id;
                                        categoryChild.Name = child.name;
                                        categoryChild.ParentCategoryName = item.name;

                                        switch (item.currency)
                                        {
                                            case "USD":
                                                categoryChild.UsdParentCategoryId = item.id;
                                                break;
                                            case "UAH":
                                                categoryChild.UahParentCategoryId = item.id;
                                                break;
                                            case "EUR":
                                                categoryChild.EurParentCategoryId = item.id;
                                                break;
                                            case "PLN":
                                                categoryChild.PlnParentCategoryId = item.id;
                                                break;
                                            default:
                                                break;
                                        }

                                        category.Children.Add(categoryChild);
                                    }

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
                catch (Exception e)
                {
                    var error = new ErrorWindow(e.Message);
                    error.ShowDialog();
                }
            });
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

        private void NewTransaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var transaction = new AddTransactionViewModel { Amount = 0 };
                transaction.Categories = new ObservableCollection<GetCategoryViewModel>();
                transaction.SubCategories = new ObservableCollection<GetCategoryChildViewModel>();
                transaction.Currencies = new ObservableCollection<string>();

                foreach (var item in _currencies) transaction.Currencies.Add(item);
                _vm.Transactions.Add(transaction);

                TransactionListBox.Items.MoveCurrentToLast();
                TransactionListBox.ScrollIntoView(TransactionListBox.Items.CurrentItem);
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }

        private async void AddTransaction_Click(object sender, RoutedEventArgs e)
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
                    BudgetApiAddAction addAction = new BudgetApiAddAction();
                    addAction.Name = _vm.Name;
                    addAction.Date = _vm.Date;

                    addAction.Transactions = new List<BudgetApiAddTransaction>();
                    foreach (var transaction in _vm.Transactions)
                    {
                        BudgetApiAddTransaction apiTransaction = new BudgetApiAddTransaction();
                        apiTransaction.Type = "Outgoing";
                        apiTransaction.Date = _vm.Date;
                        apiTransaction.Currency = transaction.Currency;

                        if (transaction.SubCategory != null) apiTransaction.CategoryId = transaction.SubCategory.Id;
                        else apiTransaction.CategoryId = transaction.Category.Id;

                        apiTransaction.Amount = -transaction.Amount.TwoSymbolsAfterDot();
                        addAction.Transactions.Add(apiTransaction);
                    }

                    var res = await _budgetApiClient.AddAction(addAction);
                    if (!res.hasErrors)
                    {
                        var result = res.Transactions.Select(t =>
                        new GetTransactionViewModel
                        {
                            Id = t.Id,
                            Amount = t.Amount.TwoSymbolsAfterDot(),
                            CategoryName = _categories.Find(c => c.Id == t.CategoryId).Name,
                            Currency = t.Currency,
                            Date = t.Date,
                            Type = t.Type,
                            ProfileAction = new GetProfileActionViewModel { Name = addAction.Name }
                        }).ToList();

                        foreach (var item in result)
                            _outgoingViewModel.Transactions.Add(item);

                        RecalculateTopBarService.RecalculateTopBar(_outgoingViewModel.Transactions.ToList());

                        Close();
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
        }

        private string CheckRequiredFields()
        {
            string errorMessage = string.Empty;

            if (string.IsNullOrEmpty(_vm.Name))
                errorMessage = "Empty name";
            else if (_vm.Transactions.ToList().Find(f => f.Amount == 0) != null)
                errorMessage = "Amount must be greater than 0";
            else if (_vm.Transactions.ToList().Find(f => string.IsNullOrEmpty(f.Currency)) != null)
                errorMessage = "Empty currency";
            else if (_vm.Transactions.ToList().Find(f => f.Category == null) != null)
                errorMessage = "Select a category";

            return errorMessage;
        }

        private void CheckAmountSumm(object sender, TextChangedEventArgs e)
        {
            try
            {
                var textBox = sender as TextBox;
                if (textBox == null) return;

                double.TryParse(textBox.Text, out double amount);

                double summ = _vm.Transactions.Sum(s => s.Amount) + amount;
                _vm.AmountSumm = summ;
                this.DataContext = _vm;
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
            Close();
        }

        private void DeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button == null) return;

                var transaction = button.DataContext as AddTransactionViewModel;
                ((ObservableCollection<AddTransactionViewModel>)TransactionListBox.ItemsSource).Remove(transaction);

                this.DataContext = _vm;
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
                var comboBox = sender as ComboBox;
                if (comboBox == null) return;

                var transaction = comboBox.DataContext as AddTransactionViewModel;
                if (transaction != null)
                {
                    transaction.Categories.Clear();
                    var categories = _categories.Where(w => w.Currency == transaction.Currency && w.ParentId == 0);
                    foreach (var category in categories) transaction.Categories.Add(category);
                }

                this.DataContext = _vm;
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }

        private void ShowSubCategory(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var comboBox = sender as ComboBox;
                if (comboBox == null) return;

                var transaction = comboBox.DataContext as AddTransactionViewModel;
                if (transaction.Category != null)
                {
                    if (transaction.Category.Children.Any())
                    {
                        transaction.SubCategories.Clear();
                        foreach (var child in transaction.Category.Children) transaction.SubCategories.Add(child);
                    }
                }
                else if (transaction.Category == null && transaction.SubCategories.Any())
                    transaction.SubCategories.Clear();

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

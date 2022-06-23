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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Budget.Views
{
    /// <summary>
    /// Interaction logic for DistributionWindow.xaml
    /// </summary>
    public partial class DistributionWindow : Window
    {
        private readonly AddActionViewModel _vm;
        private readonly IncomeViewModel _incomeViewModel;
        private readonly BudgetApiClient _budgetApiClient;
        private List<GetCategorySettingViewModel> _categoriesSettings;
        private List<GetCategoryViewModel> _categories;
        private List<AddTransactionViewModel> _transactions;
        private ImageService _imageService;

        public DistributionWindow(IncomeViewModel incomeViewModel, string name, DateTime date, double amount, string currency)
        {
            InitializeComponent();
            var user = Config.Load();
            _vm = new AddActionViewModel();
            _budgetApiClient = new BudgetApiClient(user.Token);
            _imageService = new ImageService();
            _vm.Transactions = new ObservableCollection<AddTransactionViewModel>();

            _vm.Name = name;
            _vm.Date = date;
            _vm.Amount = amount.TwoSymbolsAfterDot();
            _vm.AmountLeft = amount.TwoSymbolsAfterDot();
            _vm.Currency = currency;

            GetAvailableCategories().GetAwaiter().GetResult();
            GetCategoriesSettings().GetAwaiter().GetResult();
            CreateTransactions();

            foreach (var transaction in _transactions) _vm.Transactions.Add(transaction);

            _vm.LogoImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "budget.png"));
            _vm.CloseImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "close.png"));

            this.DataContext = _vm;
            _incomeViewModel = incomeViewModel;
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
                catch (Exception ex)
                {
                    var error = new ErrorWindow(ex.Message);
                    error.ShowDialog();
                }
            });
        }

        private Task GetCategoriesSettings()
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_vm != null)
                    {
                        _categoriesSettings = new List<GetCategorySettingViewModel>();
                        var res = await _budgetApiClient.GetCategorySetting();
                        if (!res.hasErrors)
                        {
                            foreach (var item in res.categorySettings) _categoriesSettings.Add(new GetCategorySettingViewModel { CategoryId = item.categoryId, Amount = item.amount });
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

        private void CreateTransactions()
        {
            try
            {
                _transactions = new List<AddTransactionViewModel>();
                foreach (var _categorySetting in _categoriesSettings)
                {
                    GetCategoryViewModel categorySett = _categories.Find(f => f.Id == _categorySetting.CategoryId && f.Currency == _vm.Currency);
                    if (categorySett == null)
                        continue;

                    _categorySetting.CategoryName = categorySett.Name;
                    AddTransactionViewModel transaction = new AddTransactionViewModel();
                    transaction.Categories = new ObservableCollection<GetCategoryViewModel>();
                    transaction.SubCategories = new ObservableCollection<GetCategoryChildViewModel>();
                    var categories = _categories.Where(w => w.Currency == _vm.Currency && w.ParentId == 0);
                    foreach (var category in categories) transaction.Categories.Add(category);
                    transaction.Category = transaction.Categories.ToList().Find(f => f.Id == _categorySetting.CategoryId);
                    transaction.Amount = (_vm.Amount * _categorySetting.Amount) / 100;
                    _vm.AmountLeft = _vm.AmountLeft - transaction.Amount.TwoSymbolsAfterDot();

                    _transactions.Add(transaction);
                }
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
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

        private void DeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button == null) return;

                var transaction = button.DataContext as AddTransactionViewModel;
                ((ObservableCollection<AddTransactionViewModel>)TransactionListBox.ItemsSource).Remove(transaction);

                double summ = _vm.Transactions.Sum(s => s.Amount);
                _vm.AmountLeft = (_vm.Amount - summ).TwoSymbolsAfterDot();
                this.DataContext = _vm;
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
                        apiTransaction.Type = "Income";
                        apiTransaction.Date = _vm.Date;
                        apiTransaction.Currency = _vm.Currency;

                        if (transaction.SubCategory != null) apiTransaction.CategoryId = transaction.SubCategory.Id;
                        else apiTransaction.CategoryId = transaction.Category.Id;

                        apiTransaction.Amount = transaction.Amount.TwoSymbolsAfterDot();
                        addAction.Transactions.Add(apiTransaction);
                    }

                    var res = await _budgetApiClient.AddAction(addAction);
                    if (!res.hasErrors)
                    {
                        var result = addAction.Transactions.Select(t =>
                        new GetTransactionViewModel
                        {
                            Amount = t.Amount.TwoSymbolsAfterDot(),
                            CategoryId = t.CategoryId,
                            Currency = t.Currency,
                            Date = t.Date,
                            Type = t.Type,
                            ProfileAction = new GetProfileActionViewModel { Name = addAction.Name }
                        }).ToList();

                        foreach (var child in _incomeViewModel.Children)
                        {
                            child.Uah += result.FindAll(f => f.CategoryId == child.Id && f.Currency == "UAH").Sum(s => s.Amount).TwoSymbolsAfterDot();
                            child.Usd += result.FindAll(f => f.CategoryId == child.Id && f.Currency == "USD").Sum(s => s.Amount).TwoSymbolsAfterDot();
                            child.Eur += result.FindAll(f => f.CategoryId == child.Id && f.Currency == "EUR").Sum(s => s.Amount).TwoSymbolsAfterDot();
                            child.Pln += result.FindAll(f => f.CategoryId == child.Id && f.Currency == "PLN").Sum(s => s.Amount).TwoSymbolsAfterDot();
                        }

                        SumCurrencyAmountForParentCategory(_incomeViewModel.Children, result);
                        foreach (var resu in result) _incomeViewModel.Transactions.Add(resu);
                        RecalculateTopBarService.RecalculateTopBar(_incomeViewModel.Transactions);

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

        private void SumCurrencyAmountForParentCategory(ObservableCollection<GetCategoryChildViewModel> children, List<GetTransactionViewModel> transactions)
        {
            try
            {
                var parentCategoryGroup = children.ToList().GroupBy(x => x.ParentCategoryName).Select(x => x);
                foreach (var parentCategory in parentCategoryGroup)
                {
                    List<int> categoryIds = new List<int>();
                    if (parentCategory.First().UsdParentCategoryId > 0) categoryIds.Add(parentCategory.First().UsdParentCategoryId);
                    if (parentCategory.First().UahParentCategoryId > 0) categoryIds.Add(parentCategory.First().UahParentCategoryId);
                    if (parentCategory.First().EurParentCategoryId > 0) categoryIds.Add(parentCategory.First().EurParentCategoryId);
                    if (parentCategory.First().PlnParentCategoryId > 0) categoryIds.Add(parentCategory.First().PlnParentCategoryId);

                    foreach (var id in categoryIds)
                    {
                        foreach (var child in parentCategory) child.UahSumParent += transactions.FindAll(f => f.CategoryId == id && f.Currency == "UAH").Sum(s => s.Amount).TwoSymbolsAfterDot();
                        foreach (var child in parentCategory) child.UsdSumParent += transactions.FindAll(f => f.CategoryId == id && f.Currency == "USD").Sum(s => s.Amount).TwoSymbolsAfterDot();
                        foreach (var child in parentCategory) child.EurSumParent += transactions.FindAll(f => f.CategoryId == id && f.Currency == "EUR").Sum(s => s.Amount).TwoSymbolsAfterDot();
                        foreach (var child in parentCategory) child.PlnSumParent += transactions.FindAll(f => f.CategoryId == id && f.Currency == "PLN").Sum(s => s.Amount).TwoSymbolsAfterDot();
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

            if (_vm.AmountLeft > 0)
                errorMessage = "Not all the money has been distributed";
            else if (_vm.Transactions.ToList().Find(f => f.Amount == 0) != null)
                errorMessage = "Amount must be greater than 0";
            else if (_vm.Transactions.ToList().Find(f => f.Category == null) != null)
                errorMessage = "Select a category";

            return errorMessage;
        }

        private void NewTransaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var transaction = new AddTransactionViewModel { Amount = 0 };
                transaction.Categories = new ObservableCollection<GetCategoryViewModel>();
                transaction.SubCategories = new ObservableCollection<GetCategoryChildViewModel>();
                var categories = _categories.Where(w => w.Currency == _vm.Currency && w.ParentId == 0);
                foreach (var category in categories) transaction.Categories.Add(category);

                if (!transaction.Categories.Any())
                {
                    var error = new InfoWindow("No categories for this currency");
                    error.ShowDialog();
                }

                _vm.Transactions.Add(transaction);
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }

        private void CheckAmountLeft(object sender, TextChangedEventArgs e)
        {
            try
            {
                var textBox = sender as TextBox;
                if (textBox == null) return;

                double.TryParse(textBox.Text, out double amount);

                double summ = _vm.Transactions.Sum(s => s.Amount) + amount;
                _vm.AmountLeft = (_vm.Amount - summ).TwoSymbolsAfterDot();
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

        private void ShowSubCategory(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var comboBox = sender as ComboBox;
                if (comboBox == null) return;

                var transaction = comboBox.DataContext as AddTransactionViewModel;
                if (transaction.Category.Children.Any())
                {
                    transaction.SubCategories.Clear();
                    foreach (var child in transaction.Category.Children) transaction.SubCategories.Add(child);
                }

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

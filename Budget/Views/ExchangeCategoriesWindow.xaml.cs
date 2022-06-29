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
    /// Interaction logic for ExchangeCategoriesWindow.xaml
    /// </summary>
    public partial class ExchangeCategoriesWindow : Window
    {
        private readonly ExchangeCategoriesViewModel _vm;
        private readonly IncomeViewModel _incomeViewModel;
        private readonly BudgetApiClient _budgetApiClient;
        private List<GetCategoryViewModel> _categories;
        private List<string> _currencies;
        private ImageService _imageService;

        public ExchangeCategoriesWindow(IncomeViewModel incomeViewModel)
        {
            InitializeComponent();
            var user = Config.Load();
            _vm = new ExchangeCategoriesViewModel();
            _budgetApiClient = new BudgetApiClient(user.Token);
            _imageService = new ImageService();
            _vm.Currencies = new ObservableCollection<string>();
            _vm.Categories = new ObservableCollection<GetCategoryViewModel>();
            _vm.SubCategories = new ObservableCollection<GetCategoryChildViewModel>();
            _vm.SubCategories2 = new ObservableCollection<GetCategoryChildViewModel>();

            GetAvailableCategories().GetAwaiter().GetResult();
            GetAvailableCurrencies().GetAwaiter().GetResult();

            foreach (var item in _currencies) _vm.Currencies.Add(item);

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

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void ExchangeCategory_Click(object sender, RoutedEventArgs e)
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
                    addAction.Name = "Обмен";
                    addAction.Date = DateTime.Today;

                    addAction.Transactions = new List<BudgetApiAddTransaction>();
                    List<BudgetApiAddTransaction> apiTransactions = CreateAction();
                    foreach (var apiTransaction in apiTransactions) addAction.Transactions.Add(apiTransaction);

                    var res = await _budgetApiClient.AddAction(addAction);
                    if (!res.hasErrors)
                    {
                        foreach (var child in _incomeViewModel.Children)
                        {
                            child.Uah += apiTransactions.FindAll(f => f.CategoryId == child.Id && f.Currency == "UAH").Sum(s => s.Amount).TwoSymbolsAfterDot();
                            child.Usd += apiTransactions.FindAll(f => f.CategoryId == child.Id && f.Currency == "USD").Sum(s => s.Amount).TwoSymbolsAfterDot();
                            child.Eur += apiTransactions.FindAll(f => f.CategoryId == child.Id && f.Currency == "EUR").Sum(s => s.Amount).TwoSymbolsAfterDot();
                            child.Pln += apiTransactions.FindAll(f => f.CategoryId == child.Id && f.Currency == "PLN").Sum(s => s.Amount).TwoSymbolsAfterDot();
                        }

                        SumCurrencyAmountForParentCategory(_incomeViewModel.Children, apiTransactions);
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

        private void SumCurrencyAmountForParentCategory(ObservableCollection<GetCategoryChildViewModel> children, List<BudgetApiAddTransaction> transactions)
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

            if (string.IsNullOrEmpty(_vm.Currency))
                errorMessage = "Empty currency";
            else if (_vm.Amount == 0)
                errorMessage = "Amount must be greater than 0";
            else if (_vm.Category == null)
                errorMessage = "Select a category";
            else if (_vm.Category2 == null)
                errorMessage = "Select a second category";
            else if (_vm.Category == _vm.Category2 && _vm.SubCategory == _vm.SubCategory2)
                errorMessage = "You cannot exchange savings for the same category";
            else if ((_vm.SubCategory != null || _vm.SubCategory2 != null) && _vm.SubCategory == _vm.SubCategory2)
                errorMessage = "You cannot exchange savings for the same subcategory";

            return errorMessage;
        }

        private List<BudgetApiAddTransaction> CreateAction()
        {
            List<BudgetApiAddTransaction> apiTransactions = new List<BudgetApiAddTransaction>();

            try
            {
                BudgetApiAddTransaction apiTransaction = new BudgetApiAddTransaction();
                apiTransaction.Type = "Transfer";
                apiTransaction.Date = DateTime.Today;
                apiTransaction.Currency = _vm.Currency;
                if (_vm.SubCategory != null) apiTransaction.CategoryId = _vm.SubCategory.Id;
                else apiTransaction.CategoryId = _vm.Category.Id;
                apiTransaction.Amount = -_vm.Amount.TwoSymbolsAfterDot();
                apiTransactions.Add(apiTransaction);

                BudgetApiAddTransaction apiTransaction2 = new BudgetApiAddTransaction();
                apiTransaction2.Type = "Transfer";
                apiTransaction2.Date = DateTime.Today;
                apiTransaction2.Currency = _vm.Currency;
                if (_vm.SubCategory2 != null) apiTransaction2.CategoryId = _vm.SubCategory2.Id;
                else apiTransaction2.CategoryId = _vm.Category2.Id;
                apiTransaction2.Amount = _vm.Amount.TwoSymbolsAfterDot();
                apiTransactions.Add(apiTransaction2);
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }

            return apiTransactions;
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

        private void ShowSubCategory(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _vm.SubCategories.Clear();
                if (_vm.Category != null && _vm.Category.Children.Any())
                {
                    foreach (var child in _vm.Category.Children) _vm.SubCategories.Add(child);
                }

                this.DataContext = _vm;
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }

        private void ShowSubCategory2(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _vm.SubCategories2.Clear();
                if (_vm.Category2 != null &&_vm.Category2.Children.Any())
                {
                    foreach (var child in _vm.Category2.Children) _vm.SubCategories2.Add(child);
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

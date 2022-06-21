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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Budget.Views
{
    /// <summary>
    /// Interaction logic for DeleteCategoryWindow.xaml
    /// </summary>
    public partial class DeleteCategoryWindow : Window
    {
        private readonly DeleteCategoryViewModel _vm;
        private readonly IncomeViewModel _incomeViewModel;
        private readonly BudgetApiClient _budgetApiClient;
        private List<GetCategoryViewModel> _categories;
        private List<GetTransactionViewModel> _transactions;
        private List<string> _currencies;
        private ImageService _imageService;

        public DeleteCategoryWindow(IncomeViewModel incomeViewModel)
        {
            InitializeComponent();
            var user = Config.Load();
            _vm = new DeleteCategoryViewModel();
            _budgetApiClient = new BudgetApiClient(user.Token);
            _imageService = new ImageService();
            _vm.Currencies = new ObservableCollection<string>();
            _vm.Categories = new ObservableCollection<GetCategoryViewModel>();
            _vm.SubCategories = new ObservableCollection<GetCategoryChildViewModel>();

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

        private Task GetTransactionsForCategory(int categoryId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_vm != null)
                    {
                        _transactions = new List<GetTransactionViewModel>();
                        var res = await _budgetApiClient.GetTransactionsForCategory(categoryId);
                        if (!res.hasErrors)
                        {
                            foreach (var item in res.transactions)
                            {
                                GetTransactionViewModel transaction = new GetTransactionViewModel();
                                transaction.CategoryId = item.categoryId;
                                transaction.Amount = item.amount;
                                transaction.Currency = item.currency;
                                _transactions.Add(transaction);
                            }
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

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void DeleteCategory_Click(object sender, RoutedEventArgs e)
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
                    GetCategoryChildViewModel catForDelete = null;
                    string warningsMessage = CheckWarnings();
                    if (!string.IsNullOrEmpty(warningsMessage))
                    {
                        var warning = new WarningWindow(warningsMessage, _vm);
                        warning.ShowDialog();

                        if (_vm.YesOrNo)
                            catForDelete = await DeleteCategory();
                    }
                    else
                        catForDelete = await DeleteCategory();

                    if(catForDelete != null && !string.IsNullOrEmpty(catForDelete.Name))
                    {
                        catForDelete.Name = string.Empty;
                        catForDelete.Uah = 0;
                        catForDelete.Usd = 0;
                        catForDelete.Eur = 0;
                        catForDelete.Pln = 0;
                    }
                    else if (catForDelete != null) _incomeViewModel.Children.Remove(catForDelete);

                    Close();
                }
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }

        private Task<GetCategoryChildViewModel> DeleteCategory()
        {
            Func<Task<GetCategoryChildViewModel>> value = async () =>
                        {
                            BudgetApiBaseResponse res = new BudgetApiBaseResponse();
                            if (_vm.SubCategory != null) res = await _budgetApiClient.DeleteCategory(_vm.SubCategory.Id);
                            else res = await _budgetApiClient.DeleteCategory(_vm.Category.Id);

                            if (!res.hasErrors)
                            {
                                GetCategoryChildViewModel incomeCategory = null;
                                if (_vm.SubCategory != null)
                                {
                                    if (incomeCategory == null && _vm.Currency == "UAH")
                                        incomeCategory = _incomeViewModel.Children.ToList().Find(f => f.UahParentCategoryId == _vm.SubCategory.UahParentCategoryId);
                                    if (incomeCategory == null && _vm.Currency == "EUR")
                                        incomeCategory = _incomeViewModel.Children.ToList().Find(f => f.EurParentCategoryId == _vm.SubCategory.EurParentCategoryId);
                                    if (incomeCategory == null && _vm.Currency == "USD")
                                        incomeCategory = _incomeViewModel.Children.ToList().Find(f => f.UsdParentCategoryId == _vm.SubCategory.UsdParentCategoryId);
                                    if (incomeCategory == null && _vm.Currency == "PLN")
                                        incomeCategory = _incomeViewModel.Children.ToList().Find(f => f.PlnParentCategoryId == _vm.SubCategory.PlnParentCategoryId);
                                }
                                else
                                {
                                    if (incomeCategory == null && _vm.Currency == "UAH")
                                        incomeCategory = _incomeViewModel.Children.ToList().Find(f => f.UahParentCategoryId == _vm.Category.Id);
                                    if (incomeCategory == null && _vm.Currency == "EUR")
                                        incomeCategory = _incomeViewModel.Children.ToList().Find(f => f.EurParentCategoryId == _vm.Category.Id);
                                    if (incomeCategory == null && _vm.Currency == "USD")
                                        incomeCategory = _incomeViewModel.Children.ToList().Find(f => f.UsdParentCategoryId == _vm.Category.Id);
                                    if (incomeCategory == null && _vm.Currency == "PLN")
                                        incomeCategory = _incomeViewModel.Children.ToList().Find(f => f.PlnParentCategoryId == _vm.Category.Id);
                                }

                                return incomeCategory;
                            }
                            else
                            {
                                var error = new ErrorWindow(res.errorMessage);
                                error.ShowDialog();
                                return null;
                            }
                        };
            return Task.Run(value);
        }

        private string CheckRequiredFields()
        {
            string errorMessage = string.Empty;

            if (_vm.Category == null)
                errorMessage = "Select a category";
            else if (string.IsNullOrEmpty(_vm.Currency))
                errorMessage = "Empty currency";

            if (string.IsNullOrEmpty(errorMessage))
            {
                if (_vm.Category.Children.Any() && _vm.SubCategory == null)
                    errorMessage = "Can't delete category. This category has a subcategory";
            }

            return errorMessage;
        }

        private string CheckWarnings()
        {
            string warningMessage = string.Empty;

            if (_vm.SubCategory != null)
            {
                GetTransactionsForCategory(_vm.SubCategory.Id).GetAwaiter().GetResult();
                if (_transactions.Any())
                {
                    double summ = _transactions.Sum(s => s.Amount).TwoSymbolsAfterDot();
                    if (summ != 0) warningMessage = "This category has incomes or outgoings. Are you sure you want to delete this category?";
                }
            }
            else if (_vm.SubCategory == null)
            {
                GetTransactionsForCategory(_vm.Category.Id).GetAwaiter().GetResult();
                if (_transactions.Any())
                {
                    double summ = _transactions.Sum(s => s.Amount).TwoSymbolsAfterDot();
                    if (summ != 0) warningMessage = "This category has incomes or outgoings. Are you sure you want to delete this category?";
                }
            }

            return warningMessage;
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

        private void ShowSubCategories(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _vm.SubCategories.Clear();
                if (_vm.Category != null && _vm.Category.Children.Any()) foreach (var child in _vm.Category.Children) _vm.SubCategories.Add(child);

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

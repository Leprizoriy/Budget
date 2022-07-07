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
    /// Interaction logic for AddCategoryWindow.xaml
    /// </summary>
    public partial class AddCategoryWindow : Window
    {
        private readonly AddCategoryViewModel _vm;
        private readonly IncomeViewModel _incomeViewModel;
        private readonly BudgetApiClient _budgetApiClient;
        private List<GetCategoryViewModel> _categories;
        private List<string> _currencies;
        private ImageService _imageService;

        public AddCategoryWindow(IncomeViewModel incomeViewModel)
        {
            InitializeComponent();

            var user = Config.Load();
            _vm = new AddCategoryViewModel();
            _budgetApiClient = new BudgetApiClient(user.Token);
            _imageService = new ImageService();
            _vm.Categories = new ObservableCollection<GetCategoryViewModel>();
            _vm.Currencies = new ObservableCollection<string>();

            GetAvailableCurrencies().GetAwaiter().GetResult();
            GetAvailableCategories().GetAwaiter().GetResult();

            foreach (var item in _currencies) _vm.Currencies.Add(item);

            _vm.LogoImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "budget.png"));
            _vm.CloseImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "close.png"));

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
                        if (!res.hasErrors && res.categories.Any())
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
                        else if (res.hasErrors)
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

        private async void AddCategory_Click(object sender, RoutedEventArgs e)
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
                    var response = await _budgetApiClient.AddCategory(new BudgetApiAddCategory
                    {
                        Name = _vm.Name,
                        Currency = _vm.Currency,
                        ParentId = _vm.Category != null ? _vm.Category.Id : 0
                    });

                    if (!response.hasErrors)
                    {
                        GetCategoryChildViewModel child = new GetCategoryChildViewModel();
                        child.ParentCategoryName = _vm.Name;

                        var parentId = _vm.Category != null ? _vm.Category.Id : 0;
                        if (parentId > 0)
                        {
                            GetCategoryChildViewModel parent = new GetCategoryChildViewModel();

                            switch (_vm.Currency)
                            {
                                case "USD":
                                    child.UsdParentCategoryId = parentId;
                                    parent = _incomeViewModel.Children.ToList().Find(f => f.UsdParentCategoryId == child.UsdParentCategoryId);
                                    break;
                                case "UAH":
                                    child.UahParentCategoryId = parentId;
                                    parent = _incomeViewModel.Children.ToList().Find(f => f.UahParentCategoryId == child.UahParentCategoryId);
                                    break;
                                case "EUR":
                                    child.EurParentCategoryId = parentId;
                                    parent = _incomeViewModel.Children.ToList().Find(f => f.EurParentCategoryId == child.EurParentCategoryId);
                                    break;
                                case "PLN":
                                    child.PlnParentCategoryId = parentId;
                                    parent = _incomeViewModel.Children.ToList().Find(f => f.PlnParentCategoryId == child.PlnParentCategoryId);
                                    break;
                                default:
                                    break;
                            }

                            if (parent.Id != 0 && parent.Name != null)
                            {
                                GetCategoryChildViewModel newParent = new GetCategoryChildViewModel();
                                newParent.ParentCategoryName = parent.ParentCategoryName;
                                newParent.UsdParentCategoryId = parent.UsdParentCategoryId;
                                newParent.UahParentCategoryId = parent.UahParentCategoryId;
                                newParent.EurParentCategoryId = parent.EurParentCategoryId;
                                newParent.PlnParentCategoryId = parent.PlnParentCategoryId;
                                newParent.Id = response.id;
                                newParent.Name = _vm.Name;

                                _incomeViewModel.Children.Add(newParent);
                            }
                            else
                            {
                                parent.Id = response.id;
                                parent.Name = _vm.Name;
                            }
                        }
                        else
                        {
                            switch (_vm.Currency)
                            {
                                case "USD":
                                    child.UsdParentCategoryId = response.id;
                                    break;
                                case "UAH":
                                    child.UahParentCategoryId = response.id;
                                    break;
                                case "EUR":
                                    child.EurParentCategoryId = response.id;
                                    break;
                                case "PLN":
                                    child.PlnParentCategoryId = response.id;
                                    break;
                                default:
                                    break;
                            }

                            var categoryExist = _incomeViewModel.Children.ToList().Find(f => f.ParentCategoryName == child.ParentCategoryName);
                            if (categoryExist != null)
                            {
                                if (child.UsdParentCategoryId != 0)
                                    categoryExist.UsdParentCategoryId = child.UsdParentCategoryId;
                                else if (child.UahParentCategoryId != 0)
                                    categoryExist.UahParentCategoryId = child.UahParentCategoryId;
                                else if (child.EurParentCategoryId != 0)
                                    categoryExist.EurParentCategoryId = child.EurParentCategoryId;
                                else if (child.PlnParentCategoryId != 0)
                                    categoryExist.PlnParentCategoryId = child.PlnParentCategoryId;
                            }
                            else _incomeViewModel.Children.Add(child);
                        }

                        Close();
                    }
                    else
                    {
                        var error = new ErrorWindow(response.errorMessage);
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
            else if (string.IsNullOrEmpty(_vm.Currency))
                errorMessage = "Empty currency";

            if (string.IsNullOrEmpty(errorMessage))
            {
                var category = _vm.Categories.ToList().Find(f => f.Name == _vm.Name && f.Currency == _vm.Currency);
                if (category != null)
                    errorMessage = "This category already exists";
            }

            return errorMessage;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
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

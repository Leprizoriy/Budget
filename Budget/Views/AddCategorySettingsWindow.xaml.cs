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
    /// Interaction logic for AddCategorySettingsWindow.xaml
    /// </summary>
    public partial class AddCategorySettingsWindow : Window
    {
        private readonly AddCategorySettingsViewModel _vm;
        private readonly SettingsViewModel _settingsViewModel;
        private readonly BudgetApiClient _budgetApiClient;
        private List<GetCategoryViewModel> _categories;
        private List<string> _currencies;
        private ImageService _imageService;

        public AddCategorySettingsWindow(SettingsViewModel settingsViewModel)
        {
            InitializeComponent();
            var user = Config.Load();
            _vm = new AddCategorySettingsViewModel();
            _budgetApiClient = new BudgetApiClient(user.Token);
            _imageService = new ImageService();
            _categories = new List<GetCategoryViewModel>();

            GetAvailableCategories().GetAwaiter().GetResult();
            GetAvailableCurrencies().GetAwaiter().GetResult();

            _vm.Categories = new ObservableCollection<GetCategoryViewModel>();
            _vm.Currencies = new ObservableCollection<string>();

            foreach (var item in _currencies) _vm.Currencies.Add(item);

            _vm.LogoImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "budget.png"));
            _vm.CloseImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "close.png"));

            this.DataContext = _vm;
            _settingsViewModel = settingsViewModel;
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

        private async void AddCategorySetting_Click(object sender, RoutedEventArgs e)
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
                    BudgetApiAddCategorySetting addCategorySetting = new BudgetApiAddCategorySetting();
                    addCategorySetting.categoryId = _vm.Category.Id;
                    addCategorySetting.amount = _vm.Amount.TwoSymbolsAfterDot();
                    var res = await _budgetApiClient.AddCategorySetting(addCategorySetting);

                    if (!res.hasErrors)
                    {
                        if (!string.IsNullOrEmpty(_settingsViewModel.Currency) && _settingsViewModel.Currency == _vm.Currency)
                        {
                            GetCategorySettingViewModel categorySettingViewModel = new GetCategorySettingViewModel();
                            categorySettingViewModel.Id = res.id;
                            categorySettingViewModel.CategoryId = addCategorySetting.categoryId;
                            categorySettingViewModel.CategoryName = _vm.Category.Name;
                            categorySettingViewModel.Amount = _vm.Amount.TwoSymbolsAfterDot();
                            _settingsViewModel.CategorySettings.Add(categorySettingViewModel);
                        }

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

            if (_vm.Amount == 0)
                errorMessage = "Percentage must be greater than 0";
            else if (_vm.Amount > 100)
                errorMessage = "Percentage must be less than 100";
            else if (string.IsNullOrEmpty(_vm.Currency))
                errorMessage = "Empty currency";
            else if (_vm.Category == null)
                errorMessage = "Select a category";
            else if ((_settingsViewModel.CategorySettings.Sum(s => s.Amount) + _vm.Amount) > 100)
                errorMessage = "The sum of percentages in categories cannot be more than 100";
            else if (_settingsViewModel.CategorySettings.ToList().Find(f => f.CategoryId == _vm.Category.Id) != null)
                errorMessage = "There is already a percentage for this category";

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

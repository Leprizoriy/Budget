using Budget.Extensions;
using Budget.Models;
using Budget.Services;
using Budget.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Budget.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private readonly SettingsViewModel _vm;
        private readonly BudgetApiClient _budgetApiClient;
        private List<GetCategoryViewModel> _categories;
        private List<GetCategorySettingViewModel> _categoriesSettings;
        private List<string> _currencies;

        public SettingsView()
        {
            InitializeComponent();

            var user = Config.Load();
            _vm = new SettingsViewModel();
            _vm.CategorySettings = new ObservableCollection<GetCategorySettingViewModel>();
            _vm.Currencies = new ObservableCollection<string>();
            _budgetApiClient = new BudgetApiClient(user.Token);

            GetAvailableCategories().GetAwaiter().GetResult();
            GetAvailableCurrencies().GetAwaiter().GetResult();
            foreach (var item in _currencies) _vm.Currencies.Add(item);

            this.DataContext = _vm;
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
                            foreach (var item in res.categorySettings) _categoriesSettings.Add(new GetCategorySettingViewModel { Id = item.id, CategoryId = item.categoryId, Amount = item.amount.TwoSymbolsAfterDot() });
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
                        if (res != null) foreach (var item in res.categories) _categories.Add(new GetCategoryViewModel { Id = item.id, Currency = item.currency, Name = item.name });
                    }
                }
                catch (Exception ex)
                {
                    var error = new ErrorWindow(ex.Message);
                    error.ShowDialog();
                }
            });
        }

        private void AddSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newCategorySettings = new AddCategorySettingsWindow(_vm);
                if (newCategorySettings.ShowDialog() != true) return;
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }

        private void CurrencySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _vm.CategorySettings.Clear();
                GetCategoriesSettings().GetAwaiter().GetResult();

                foreach (var _categorySetting in _categoriesSettings)
                {
                    GetCategoryViewModel category = _categories.Find(f => f.Id == _categorySetting.CategoryId && f.Currency == _vm.Currency);
                    if (category != null)
                    {
                        _categorySetting.CategoryName = category.Name;
                        _vm.CategorySettings.Add(_categorySetting);
                    }
                }

                this.DataContext = _vm;
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }

        private async void DeleteSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var categorySetting = CategorySettingsListView.SelectedItem as GetCategorySettingViewModel;
                if (categorySetting == null)
                {
                    var error = new ErrorWindow("Category setting was not selected");
                    error.ShowDialog();
                }
                else
                {
                    var res = await _budgetApiClient.DeleteCategorySetting(categorySetting.Id);
                    if (!res.hasErrors)
                        ((ObservableCollection<GetCategorySettingViewModel>)CategorySettingsListView.ItemsSource).Remove(categorySetting);
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
    }
}

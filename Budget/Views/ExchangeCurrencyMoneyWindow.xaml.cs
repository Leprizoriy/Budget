using Budget.Extensions;
using Budget.Models;
using Budget.Services;
using Budget.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Budget.Views
{
    /// <summary>
    /// Interaction logic for ExchangeCurrencyMoneyWindow.xaml
    /// </summary>
    public partial class ExchangeCurrencyMoneyWindow : Window
    {
        private readonly ExchangeCurrencyMoneyViewModel _vm;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly BudgetApiClient _budgetApiClient;
        private List<GetCurrencyCourseViewModel> courses;
        private ImageService _imageService;

        public ExchangeCurrencyMoneyWindow(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            var user = Config.Load();
            _vm = new ExchangeCurrencyMoneyViewModel();
            _budgetApiClient = new BudgetApiClient(user.Token);
            _imageService = new ImageService();
            _vm.CurrencyCourses = new ObservableCollection<GetCurrencyCourseViewModel>();
            _mainWindowViewModel = mainWindowViewModel;

            MatchCurrencyCourse();
            foreach (var course in courses) _vm.CurrencyCourses.Add(course);

            _vm.LogoImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "budget.png"));
            _vm.CloseImage = _imageService.InitImage(Path.Combine(FileService.GetRecourcesDirectory(), "close.png"));

            this.DataContext = _vm;
        }

        private void MatchCurrencyCourse()
        {
            courses = new List<GetCurrencyCourseViewModel>();

            foreach (var currency in _mainWindowViewModel.Currencies)
            {
                if (currency == _mainWindowViewModel.Currency)
                {
                    if (currency == "USD")
                        _vm.SelectedCurrencyAmount = _mainWindowViewModel.UsdAmount;
                    else if (currency == "UAH")
                        _vm.SelectedCurrencyAmount = _mainWindowViewModel.UahAmount;
                    else if (currency == "EUR")
                        _vm.SelectedCurrencyAmount = _mainWindowViewModel.EurAmount;
                    else if (currency == "PLN")
                        _vm.SelectedCurrencyAmount = _mainWindowViewModel.PlnAmount;

                    continue;
                }

                GetCurrencyCourseViewModel course = new GetCurrencyCourseViewModel();
                course.CurrencyFrom = currency;
                course.CurrencyTo = _mainWindowViewModel.Currency;

                if (course.CurrencyFrom == "USD")
                    course.AmountFrom = _mainWindowViewModel.UsdAmount;
                else if (course.CurrencyFrom == "UAH")
                    course.AmountFrom = _mainWindowViewModel.UahAmount;
                else if (course.CurrencyFrom == "EUR")
                    course.AmountFrom = _mainWindowViewModel.EurAmount;
                else if (course.CurrencyFrom == "PLN")
                    course.AmountFrom = _mainWindowViewModel.PlnAmount;

                courses.Add(course);
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

        private void WriteAmountTo(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;


            double.TryParse(textBox.Text, out double course);
            var currencyCourse = textBox.DataContext as GetCurrencyCourseViewModel;
            if (currencyCourse != null)
                currencyCourse.AmountTo = course * currencyCourse.AmountFrom;

            this.DataContext = _vm;
        }

        private void SummCurrencyAmount_Click(object sender, RoutedEventArgs e)
        {
            _mainWindowViewModel.SummAmount = (_vm.CurrencyCourses.Sum(s => s.AmountTo) + _vm.SelectedCurrencyAmount).TwoSymbolsAfterDot();
            Close();
        }
    }
}

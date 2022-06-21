using Budget.Extensions;
using Budget.Models;
using Budget.Models.BudgetApiModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Budget.Views
{
    /// <summary>
    /// Interaction logic for InputView.xaml
    /// </summary>
    public partial class IncomeView : UserControl
    {
        private readonly IncomeViewModel _vm;
        private readonly BudgetApiClient _budgetApiClient;
        private List<GetTransactionViewModel> transactions;
        private List<BudgetApiGetCategory> categories;


        public IncomeView()
        {
            InitializeComponent();
            var user = Config.Load();
            _vm = new IncomeViewModel();
            _budgetApiClient = new BudgetApiClient(user.Token);

            GetAvailableCategories().GetAwaiter().GetResult();
            GetTransactions().GetAwaiter().GetResult();
            MatchCategories(categories).GetAwaiter().GetResult();
            _vm.Transactions = transactions;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(_vm.Children);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("ParentCategoryName");
            view.GroupDescriptions.Add(groupDescription);

            this.DataContext = _vm;
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newCategory = new AddCategoryWindow(_vm);
                if (newCategory.ShowDialog() != true)
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

        private void ChangeCategory_Click(object sender, RoutedEventArgs e)
        {
            //var newCategory = new AddCategoryWindow();
            //if (newCategory.ShowDialog() != true) return;
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

                                BudgetApiGetCategory category = categories.Find(f => f.id == transaction.CategoryId);
                                if (category != null) transaction.CategoryName = category.name;

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
                        categories = new List<BudgetApiGetCategory>();
                        var res = await _budgetApiClient.GetAvailableCategories();
                        if (!res.hasErrors) foreach (var item in res.categories) categories.Add(item);
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

        private Task MatchCategories(List<BudgetApiGetCategory> categories)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_vm != null)
                    {
                        _vm.Children = new ObservableCollection<GetCategoryChildViewModel>();

                        if (categories != null)
                        {
                            var resGroup = categories.GroupBy(x => x.name).ToList();
                            if (resGroup != null)
                                foreach (var items in resGroup)
                                {
                                    bool noParent = false;
                                    List<GetCategoryChildViewModel> children = new List<GetCategoryChildViewModel>();
                                    GetCategoryChildViewModel child = new GetCategoryChildViewModel();
                                    child.ParentCategoryName = items.Key;
                                    foreach (var item in items)
                                    {
                                        if (item.children.Any())
                                        {
                                            switch (item.currency)
                                            {
                                                case "USD":
                                                    child.UsdParentCategoryId = item.id;
                                                    break;
                                                case "UAH":
                                                    child.UahParentCategoryId = item.id;
                                                    break;
                                                case "EUR":
                                                    child.EurParentCategoryId = item.id;
                                                    break;
                                                case "PLN":
                                                    child.PlnParentCategoryId = item.id;
                                                    break;
                                                default:
                                                    break;
                                            }

                                            foreach (var apiChild in item.children) children.Add(new GetCategoryChildViewModel { ParentCategoryName = items.Key, Name = apiChild.name, Id = apiChild.id });
                                        }
                                        else if (item.parent == null)
                                        {
                                            switch (item.currency)
                                            {
                                                case "USD":
                                                    child.UsdParentCategoryId = item.id;
                                                    break;
                                                case "UAH":
                                                    child.UahParentCategoryId = item.id;
                                                    break;
                                                case "EUR":
                                                    child.EurParentCategoryId = item.id;
                                                    break;
                                                case "PLN":
                                                    child.PlnParentCategoryId = item.id;
                                                    break;
                                                default:
                                                    break;
                                            }

                                            noParent = true;
                                        }
                                    }

                                    if (children.Any())
                                    {
                                        SumCurrencyAmountForChild(children);

                                        foreach (var item in children)
                                        {
                                            GetCategoryChildViewModel newChild = new GetCategoryChildViewModel();
                                            newChild.UsdParentCategoryId = child.UsdParentCategoryId;
                                            newChild.UahParentCategoryId = child.UahParentCategoryId;
                                            newChild.EurParentCategoryId = child.EurParentCategoryId;
                                            newChild.PlnParentCategoryId = child.PlnParentCategoryId;
                                            newChild.ParentCategoryName = child.ParentCategoryName;
                                            newChild.Name = item.Name;
                                            newChild.Id = item.Id;
                                            newChild.Uah = item.Uah;
                                            newChild.Usd = item.Usd;
                                            newChild.Eur = item.Eur;
                                            newChild.Pln = item.Pln;
                                            newChild.Id = item.Id;

                                            _vm.Children.Add(newChild);
                                        }
                                    }
                                    else if (noParent) _vm.Children.Add(child);
                                }

                            SumCurrencyAmountForParentCategory(_vm.Children);
                            RecalculateTopBarService.RecalculateTopBar(transactions);
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

        public void SumCurrencyAmountForParentCategory(ObservableCollection<GetCategoryChildViewModel> children)
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

        private void SumCurrencyAmountForChild(List<GetCategoryChildViewModel> children)
        {
            try
            {
                var categoryGroup = children.ToList().GroupBy(x => x.Name).Select(x => x);
                foreach (var category in categoryGroup)
                {
                    foreach (var child in category) child.Uah = transactions.FindAll(f => f.CategoryName == category.Key && f.Currency == "UAH").Sum(s => s.Amount).TwoSymbolsAfterDot();
                    foreach (var child in category) child.Usd = transactions.FindAll(f => f.CategoryName == category.Key && f.Currency == "USD").Sum(s => s.Amount).TwoSymbolsAfterDot();
                    foreach (var child in category) child.Eur = transactions.FindAll(f => f.CategoryName == category.Key && f.Currency == "EUR").Sum(s => s.Amount).TwoSymbolsAfterDot();
                    foreach (var child in category) child.Pln = transactions.FindAll(f => f.CategoryName == category.Key && f.Currency == "PLN").Sum(s => s.Amount).TwoSymbolsAfterDot();
                }
            }
            catch (Exception ex)
            {
                var error = new ErrorWindow(ex.Message);
                error.ShowDialog();
            }
        }

        private void AddTransaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var transaction = new AddTransactionWindow(_vm);
                if (transaction.ShowDialog() != true)
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

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var category = new DeleteCategoryWindow(_vm);
                if (category.ShowDialog() != true)
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

        private void ExchangeCategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var exchangeCategories = new ExchangeCategoriesWindow(_vm);
                if (exchangeCategories.ShowDialog() != true)
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
    }
}

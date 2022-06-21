using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;

namespace Budget.Services
{
    public class ResourcesService
    {
        public static string GetResourceString(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            string resource;
            using (Stream stream = assembly.GetManifestResourceStream($"Budget.Resources.{resourceName}"))
            using (StreamReader reader = new StreamReader(stream))
                resource = reader.ReadToEnd();

            return resource;
        }

        public static Image GetResourceImage(string resourceName, string resourceFolder = "")
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream($"Budget.Resources{resourceFolder}.{resourceName}"))
            {
                if (stream != null) return Image.FromStream(stream);
            }
            return null;
        }

        public static void InitializeResources()
        {
            var resourcesPath = FileService.GetRecourcesDirectory();
            Directory.CreateDirectory(resourcesPath);

            var logoImage = GetResourceImage("budget.png");
            logoImage.Save(Path.Combine(resourcesPath, "budget.png"), ImageFormat.Png);

            var closeImage = GetResourceImage("close.png");
            closeImage.Save(Path.Combine(resourcesPath, "close.png"), ImageFormat.Png);

            var calendarImage = GetResourceImage("calendar.png");
            calendarImage.Save(Path.Combine(resourcesPath, "calendar.png"), ImageFormat.Png);

            var clearFilterImage = GetResourceImage("clear_filter.png");
            clearFilterImage.Save(Path.Combine(resourcesPath, "clear_filter.png"), ImageFormat.Png);

            var colapseImage = GetResourceImage("collapse.png");
            colapseImage.Save(Path.Combine(resourcesPath, "collapse.png"), ImageFormat.Png);

            var exchangeImage = GetResourceImage("exchange.png");
            exchangeImage.Save(Path.Combine(resourcesPath, "exchange.png"), ImageFormat.Png);

            var searchImage = GetResourceImage("search.png");
            searchImage.Save(Path.Combine(resourcesPath, "search.png"), ImageFormat.Png);

            //Manual
            var addActionWindowImage = GetResourceImage("AddActionWindow.png", ".Manual");
            addActionWindowImage.Save(Path.Combine(resourcesPath, "AddActionWindow.png"), ImageFormat.Png);

            var addNewCategoryImage = GetResourceImage("AddNewCategory.png", ".Manual");
            addNewCategoryImage.Save(Path.Combine(resourcesPath, "AddNewCategory.png"), ImageFormat.Png);

            var addSettingWindowImage = GetResourceImage("AddSettingWindow.png", ".Manual");
            addSettingWindowImage.Save(Path.Combine(resourcesPath, "AddSettingWindow.png"), ImageFormat.Png);

            var addTransactionWindowImage = GetResourceImage("AddTransactionWindow.png", ".Manual");
            addTransactionWindowImage.Save(Path.Combine(resourcesPath, "AddTransactionWindow.png"), ImageFormat.Png);

            var currencyCourseWindowImage = GetResourceImage("CurrencyCourseWindow.png", ".Manual");
            currencyCourseWindowImage.Save(Path.Combine(resourcesPath, "CurrencyCourseWindow.png"), ImageFormat.Png);

            var deleteCategoryImage = GetResourceImage("DeleteCategory.png", ".Manual");
            deleteCategoryImage.Save(Path.Combine(resourcesPath, "DeleteCategory.png"), ImageFormat.Png);

            var distributionWindowImage = GetResourceImage("DistributionWindow.png", ".Manual");
            distributionWindowImage.Save(Path.Combine(resourcesPath, "DistributionWindow.png"), ImageFormat.Png);

            var exchangeCategoryImage = GetResourceImage("ExchangeCategory.png", ".Manual");
            exchangeCategoryImage.Save(Path.Combine(resourcesPath, "ExchangeCategory.png"), ImageFormat.Png);

            var incomeViewImage = GetResourceImage("IncomeView.png", ".Manual");
            incomeViewImage.Save(Path.Combine(resourcesPath, "IncomeView.png"), ImageFormat.Png);

            var mainWindowImage = GetResourceImage("MainWindow.png", ".Manual");
            mainWindowImage.Save(Path.Combine(resourcesPath, "MainWindow.png"), ImageFormat.Png);

            var outgoingFilterImage = GetResourceImage("OutgoingFilter.png", ".Manual");
            outgoingFilterImage.Save(Path.Combine(resourcesPath, "OutgoingFilter.png"), ImageFormat.Png);

            var outgoingViewImage = GetResourceImage("OutgoingView.png", ".Manual");
            outgoingViewImage.Save(Path.Combine(resourcesPath, "OutgoingView.png"), ImageFormat.Png);

            var settingsViewImage = GetResourceImage("SettingsView.png", ".Manual");
            settingsViewImage.Save(Path.Combine(resourcesPath, "SettingsView.png"), ImageFormat.Png);
        }
    }
}

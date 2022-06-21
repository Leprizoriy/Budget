using Budget.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Budget.Models
{
    public static class Config
    {
        private const string CONFIG_FILE_NAME = "budgetconfig.xml";

        public static void Save(User user)
        {
            SerializerService.WriteToXmlFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), GetConfigPath()), user);
        }

        public static User Load()
        {
            return SerializerService.ReadFromXmlFile<User>(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), GetConfigPath()));
        }

        private static string GetConfigPath()
        {
            string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Budget");
            Directory.CreateDirectory(directoryPath);
            return Path.Combine(directoryPath, CONFIG_FILE_NAME);
        }
    }
}

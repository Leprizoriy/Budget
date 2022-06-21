using System;
using System.IO;

namespace Budget.Services
{
    public class FileService
    {
        public static string GetApplicationDirectory()
        {
            return Environment.CurrentDirectory;
        }

        public static string GetRecourcesDirectory()
        {
            return Path.Combine(GetApplicationDirectory(), "Recources");
        }

        public static string GetLogDirectory()
        {
            return Path.Combine(GetApplicationDirectory(), "Logs");
        }
    }
}

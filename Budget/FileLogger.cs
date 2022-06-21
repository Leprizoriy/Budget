using Budget.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Budget
{
    public static class FileLogger
    {
        private static object _lockObject = new object();
        private const string LOG_FILE_NAME = @"BudgetError.txt";

        public static void ReportToLog(string id, string entry, Exception ex)
        {
            try
            {
                Directory.CreateDirectory(FileService.GetLogDirectory());
                string data = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{id}] - {entry}{Environment.NewLine}";
                if (ex != null)
                {
                    data += $"{ex}{Environment.NewLine}";
                    if (ex.InnerException != null)
                        data += $"INNER - {ex.InnerException.Message}{Environment.NewLine}";
                }

                lock (LOG_FILE_NAME)
                {
                    File.AppendAllText(Path.Combine(FileService.GetLogDirectory(), LOG_FILE_NAME), data + Environment.NewLine);
                }
            }
            catch
            {
                //ignore
            }
        }
    }
}

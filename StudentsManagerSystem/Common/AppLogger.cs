
using System.IO;

namespace StudentsManagerSystem.Common
{
    /// <summary>
    /// 简单日志记录器。
    /// </summary>
    public static class AppLogger
    {
        private static readonly object SyncRoot = new();
        private static readonly string LogDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
        private static readonly string LogFilePath = Path.Combine(LogDirectory, "StudentsManagerSystem.log");

        public static void Info(string message) => Write("INFO", message);

        public static void Warn(string message) => Write("WARN", message);

        public static void Error(string message, Exception? exception = null)
        {
            var detail = exception == null ? message : $"{message}{Environment.NewLine}{exception}";
            Write("ERROR", detail);
        }

        private static void Write(string level, string message)
        {
            lock (SyncRoot)
            {
                Directory.CreateDirectory(LogDirectory);
                var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";
                File.AppendAllText(LogFilePath, line);
            }
        }
    }
}

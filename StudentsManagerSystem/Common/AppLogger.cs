
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
        private const long MaxLogFileBytes = 1024 * 1024;
        private const int MaxArchiveFiles = 10;

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
                RotateIfNeeded();
                var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";
                File.AppendAllText(LogFilePath, line);
            }
        }

        public static IReadOnlyList<string> GetLogFiles()
        {
            Directory.CreateDirectory(LogDirectory);
            return Directory.GetFiles(LogDirectory, "StudentsManagerSystem*.log")
                .OrderByDescending(File.GetLastWriteTime)
                .ToList();
        }

        public static string ReadRecentText(int maxLines = 500)
        {
            lock (SyncRoot)
            {
                if (!File.Exists(LogFilePath))
                {
                    return "暂无日志。";
                }

                return string.Join(Environment.NewLine, File.ReadLines(LogFilePath).TakeLast(maxLines));
            }
        }

        private static void RotateIfNeeded()
        {
            if (!File.Exists(LogFilePath) || new FileInfo(LogFilePath).Length < MaxLogFileBytes)
            {
                return;
            }

            var archivePath = Path.Combine(LogDirectory, $"StudentsManagerSystem-{DateTime.Now:yyyyMMddHHmmss}.log");
            File.Move(LogFilePath, archivePath);

            var archives = Directory.GetFiles(LogDirectory, "StudentsManagerSystem-*.log")
                .OrderByDescending(File.GetLastWriteTime)
                .Skip(MaxArchiveFiles);

            foreach (var archive in archives)
            {
                File.Delete(archive);
            }
        }
    }
}

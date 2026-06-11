using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace StudentsManagerSystem.Common
{
    /// <summary>
    /// CSV 导出与导入辅助工具。
    /// </summary>
    public static class CsvExportHelper
    {
        public static void ExportToCsv<T>(IEnumerable<T> items, string filePath)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(property => property.CanRead)
                .ToArray();

            var builder = new StringBuilder();
            builder.AppendLine(string.Join(",", properties.Select(property => Escape(property.Name))));

            foreach (var item in items)
            {
                var values = properties.Select(property => Escape(FormatValue(property.GetValue(item))));
                builder.AppendLine(string.Join(",", values));
            }

            File.WriteAllText(filePath, builder.ToString(), Encoding.UTF8);
        }

        public static string FormatValue(object? value)
        {
            return value switch
            {
                null => string.Empty,
                DateTime dateTime => dateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                decimal decimalValue => decimalValue.ToString(CultureInfo.InvariantCulture),
                double doubleValue => doubleValue.ToString(CultureInfo.InvariantCulture),
                float floatValue => floatValue.ToString(CultureInfo.InvariantCulture),
                _ => value.ToString() ?? string.Empty
            };
        }

        public static string Escape(string value)
        {
            if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }
    }
}

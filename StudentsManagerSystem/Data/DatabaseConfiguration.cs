using System.Configuration;
using System.IO;
using Microsoft.Data.Sqlite;

namespace StudentsManagerSystem.Data
{
    /// <summary>
    /// 数据库配置与路径解析。
    /// </summary>
    public static class DatabaseConfiguration
    {
        private const string ConnectionStringName = "StudentsManagerSystemDb";
        private const string DatabaseFileKey = "DatabaseFile";

        public static string ConnectionString
        {
            get
            {
                var configured = ConfigurationManager.ConnectionStrings[ConnectionStringName]?.ConnectionString;
                if (!string.IsNullOrWhiteSpace(configured))
                {
                    var builder = new SqliteConnectionStringBuilder(configured);
                    if (string.IsNullOrWhiteSpace(builder.DataSource))
                    {
                        builder.DataSource = ResolveDatabaseFileName();
                    }
                    else if (!Path.IsPathRooted(builder.DataSource))
                    {
                        builder.DataSource = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, builder.DataSource));
                    }

                    builder.Mode = SqliteOpenMode.ReadWriteCreate;
                    return builder.ToString();
                }

                return new SqliteConnectionStringBuilder
                {
                    DataSource = ResolveDatabaseFileName(),
                    Mode = SqliteOpenMode.ReadWriteCreate
                }.ToString();
            }
        }

        public static string ResolveDatabaseFileName()
        {
            var fileName = ConfigurationManager.AppSettings[DatabaseFileKey];
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "StudentsManagerSystem.db";
            }

            if (Path.IsPathRooted(fileName))
            {
                return fileName;
            }

            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, fileName));
        }
    }
}

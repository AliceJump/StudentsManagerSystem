using System.Configuration;
using Microsoft.Data.SqlClient;

namespace StudentsManagerSystem.Data.SqlServer
{
    internal static class SqlServerConnectionFactory
    {
        private const string ConnectionStringName = "StudentsManagerSystemDb";
        private const string DefaultConnectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=StudentsManagerSystemDb;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

        public static string ConnectionString =>
            ConfigurationManager.ConnectionStrings[ConnectionStringName]?.ConnectionString ?? DefaultConnectionString;

        public static SqlConnection CreateConnection() => new SqlConnection(ConnectionString);

        public static SqlConnection CreateMasterConnection()
        {
            var builder = new SqlConnectionStringBuilder(ConnectionString)
            {
                InitialCatalog = "master"
            };

            return new SqlConnection(builder.ConnectionString);
        }
    }
}
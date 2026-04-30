using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Data.SqlServer
{
    internal sealed class UsersRepository
    {
        public bool ValidateCredentials(string username, string password, out string? displayName)
        {
            displayName = null;
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT PasswordHash, DisplayName FROM dbo.Users WHERE Username = @Username;";
            command.Parameters.AddWithValue("@Username", username);
            using var reader = command.ExecuteReader();
            if (!reader.Read()) return false;
            var stored = reader.GetString(0);
            displayName = reader.IsDBNull(1) ? null : reader.GetString(1);

            var hash = ComputeHash(password);
            return string.Equals(stored, hash, StringComparison.OrdinalIgnoreCase);
        }

        private static string ComputeHash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashed = sha.ComputeHash(bytes);
            return BitConverter.ToString(hashed).Replace("-", "").ToLowerInvariant();
        }
    }
}

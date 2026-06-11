using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StudentsManagerSystem.Data;

namespace StudentsManagerSystem.Data.SqlServer
{
    /// <summary>
    /// 用户仓储。
    /// </summary>
    internal sealed class UsersRepository
    {
        public bool ValidateCredentials(string username, string password, out string? displayName)
        {
            displayName = null;
            username = username.Trim();
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var user = context.Users.FirstOrDefault(item => item.Username == username && item.IsActive);
            if (user == null)
            {
                return false;
            }

            var valid = string.Equals(user.PasswordHash, ComputeHash(password), StringComparison.OrdinalIgnoreCase);
            if (!valid)
            {
                return false;
            }

            displayName = user.DisplayName;
            user.LastLoginAt = DateTime.Now;
            context.Users.Update(user);
            context.SaveChanges();
            return true;
        }

        private static string ComputeHash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashed = sha.ComputeHash(bytes);
            return Convert.ToHexString(hashed).ToLowerInvariant();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StudentsManagerSystem.Data
{
    /// <summary>
    /// 统一创建数据库上下文。
    /// </summary>
    public static class StudentsManagerDbContextFactory
    {
        public static StudentsManagerDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<StudentsManagerDbContext>()
                .UseSqlite(DatabaseConfiguration.ConnectionString)
                .EnableSensitiveDataLogging()
                .Options;

            return new StudentsManagerDbContext(options);
        }
    }

    public sealed class DesignTimeStudentsManagerDbContextFactory : IDesignTimeDbContextFactory<StudentsManagerDbContext>
    {
        public StudentsManagerDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<StudentsManagerDbContext>()
                .UseSqlite(DatabaseConfiguration.ConnectionString)
                .Options;

            return new StudentsManagerDbContext(options);
        }
    }
}

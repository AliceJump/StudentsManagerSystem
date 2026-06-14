using Microsoft.EntityFrameworkCore;
using StudentsManagerSystem.Data;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Data.Repositories
{
    /// <summary>
    /// 基础数据仓储。
    /// </summary>
    internal sealed class BasicDataRepository
    {
        public List<Department> GetDepartments()
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Departments.AsNoTracking().OrderBy(item => item.DepartmentNo).ToList();
        }

        public List<Major> GetMajors()
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Majors.AsNoTracking().OrderBy(item => item.MajorNo).ToList();
        }

        public List<Class> GetClasses()
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Classes.AsNoTracking().OrderBy(item => item.ClassNo).ToList();
        }

        public List<string> GetLookupValues(string category)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.LookupOptions.AsNoTracking()
                .Where(item => item.Category == category && item.IsActive)
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.Value)
                .Select(item => item.Value)
                .ToList();
        }

        public int AddDepartment(Department department)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.Departments.Add(department);
            context.SaveChanges();
            return department.Id;
        }

        public void UpdateDepartment(Department department)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.Departments.Update(department);
            context.SaveChanges();
        }

        public void DeleteDepartment(int id) => DeleteEntity<Department>(id);

        public int AddMajor(Major major)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.Majors.Add(major);
            context.SaveChanges();
            return major.Id;
        }

        public void UpdateMajor(Major major)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.Majors.Update(major);
            context.SaveChanges();
        }

        public void DeleteMajor(int id) => DeleteEntity<Major>(id);

        public int AddClass(Class classInfo)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.Classes.Add(classInfo);
            context.SaveChanges();
            return classInfo.Id;
        }

        public void UpdateClass(Class classInfo)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.Classes.Update(classInfo);
            context.SaveChanges();
        }

        public void DeleteClass(int id) => DeleteEntity<Class>(id);

        private static void DeleteEntity<TEntity>(int id) where TEntity : class
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var entity = context.Set<TEntity>().Find(id);
            if (entity == null)
            {
                return;
            }

            context.Set<TEntity>().Remove(entity);
            context.SaveChanges();
        }
    }
}

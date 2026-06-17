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

        public List<Class> GetClassesByDepartment(string departmentName)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Classes.AsNoTracking()
                .Where(item => item.DepartmentName == departmentName)
                .OrderBy(item => item.ClassNo)
                .ToList();
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

        public Department? GetDepartmentById(int id)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Departments.AsNoTracking().FirstOrDefault(item => item.Id == id);
        }

        public Major? GetMajorById(int id)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Majors.AsNoTracking().FirstOrDefault(item => item.Id == id);
        }

        public Class? GetClassById(int id)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Classes.AsNoTracking().FirstOrDefault(item => item.Id == id);
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

        public void RenameStudentsDepartment(string oldName, string newName)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var students = context.Students.Where(item => item.Department == oldName).ToList();
            foreach (var student in students)
            {
                student.Department = newName;
            }

            context.SaveChanges();
        }

        public void RenameStudentsMajor(string oldName, string newName)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var students = context.Students.Where(item => item.Major == oldName).ToList();
            foreach (var student in students)
            {
                student.Major = newName;
            }

            context.SaveChanges();
        }

        public void RenameStudentsClass(string oldName, string newName)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var students = context.Students.Where(item => item.Class == oldName).ToList();
            foreach (var student in students)
            {
                student.Class = newName;
            }

            context.SaveChanges();
        }

        public void RenameMajorsDepartment(string oldName, string newName)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var majors = context.Majors.Where(item => item.DepartmentName == oldName).ToList();
            foreach (var major in majors)
            {
                major.DepartmentName = newName;
            }

            context.SaveChanges();
        }

        public void RenameClassesDepartment(string oldName, string newName)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var classes = context.Classes.Where(item => item.DepartmentName == oldName).ToList();
            foreach (var classInfo in classes)
            {
                classInfo.DepartmentName = newName;
            }

            context.SaveChanges();
        }

        public void RenameClassesMajor(string oldName, string newName)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var classes = context.Classes.Where(item => item.MajorName == oldName).ToList();
            foreach (var classInfo in classes)
            {
                classInfo.MajorName = newName;
            }

            context.SaveChanges();
        }

        public int CountStudentsByClassName(string className)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Students.Count(item => item.Class == className);
        }

        public bool DepartmentHasReferences(string departmentName)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Students.Any(item => item.Department == departmentName) ||
                   context.Majors.Any(item => item.DepartmentName == departmentName) ||
                   context.Classes.Any(item => item.DepartmentName == departmentName);
        }

        public bool MajorHasReferences(string majorName)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Students.Any(item => item.Major == majorName) ||
                   context.Classes.Any(item => item.MajorName == majorName);
        }

        public bool ClassHasReferences(string className)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Students.Any(item => item.Class == className);
        }

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

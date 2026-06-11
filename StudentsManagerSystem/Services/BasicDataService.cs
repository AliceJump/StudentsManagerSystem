using StudentsManagerSystem.Common;
using StudentsManagerSystem.Data.SqlServer;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Services
{
    internal sealed class BasicDataService
    {
        private readonly BasicDataRepository repository = new();

        public List<Department> GetDepartments() => repository.GetDepartments();

        public List<Major> GetMajors() => repository.GetMajors();

        public List<Class> GetClasses() => repository.GetClasses();

        public ServiceResult AddDepartment(Department department)
        {
            var validation = ValidateDepartment(department);
            if (!validation.Succeeded) return validation;
            repository.AddDepartment(department);
            return ServiceResult.Success("院系新增成功");
        }

        public ServiceResult UpdateDepartment(Department department)
        {
            var validation = ValidateDepartment(department);
            if (!validation.Succeeded) return validation;
            repository.UpdateDepartment(department);
            return ServiceResult.Success("院系修改成功");
        }

        public ServiceResult AddMajor(Major major)
        {
            var validation = ValidateMajor(major);
            if (!validation.Succeeded) return validation;
            repository.AddMajor(major);
            return ServiceResult.Success("专业新增成功");
        }

        public ServiceResult UpdateMajor(Major major)
        {
            var validation = ValidateMajor(major);
            if (!validation.Succeeded) return validation;
            repository.UpdateMajor(major);
            return ServiceResult.Success("专业修改成功");
        }

        public ServiceResult AddClass(Class classInfo)
        {
            var validation = ValidateClass(classInfo);
            if (!validation.Succeeded) return validation;
            repository.AddClass(classInfo);
            return ServiceResult.Success("班级新增成功");
        }

        public ServiceResult UpdateClass(Class classInfo)
        {
            var validation = ValidateClass(classInfo);
            if (!validation.Succeeded) return validation;
            repository.UpdateClass(classInfo);
            return ServiceResult.Success("班级修改成功");
        }

        public void DeleteDepartment(int id) => repository.DeleteDepartment(id);

        public void DeleteMajor(int id) => repository.DeleteMajor(id);

        public void DeleteClass(int id) => repository.DeleteClass(id);

        private static ServiceResult ValidateDepartment(Department department)
        {
            if (string.IsNullOrWhiteSpace(department.DepartmentNo) || string.IsNullOrWhiteSpace(department.DepartmentName))
            {
                return ServiceResult.Failure("请填写院系编号和院系名称");
            }

            if (!string.IsNullOrWhiteSpace(department.PhoneNumber) && !InputValidator.ValidatePhone(department.PhoneNumber))
            {
                return ServiceResult.Failure("联系电话格式错误");
            }

            return ServiceResult.Success();
        }

        private static ServiceResult ValidateMajor(Major major)
        {
            if (string.IsNullOrWhiteSpace(major.MajorNo) || string.IsNullOrWhiteSpace(major.MajorName) || string.IsNullOrWhiteSpace(major.DepartmentName))
            {
                return ServiceResult.Failure("请填写专业编号、专业名称和所属院系");
            }

            if (major.Duration <= 0 || string.IsNullOrWhiteSpace(major.DegreeType))
            {
                return ServiceResult.Failure("请检查学制和学位类型");
            }

            return ServiceResult.Success();
        }

        private static ServiceResult ValidateClass(Class classInfo)
        {
            if (string.IsNullOrWhiteSpace(classInfo.ClassNo) || string.IsNullOrWhiteSpace(classInfo.ClassName) || string.IsNullOrWhiteSpace(classInfo.DepartmentName) || string.IsNullOrWhiteSpace(classInfo.MajorName))
            {
                return ServiceResult.Failure("请填写班级编号、班级名称、所属院系和所属专业");
            }

            if (string.IsNullOrWhiteSpace(classInfo.Grade) || classInfo.StudentCount < 0)
            {
                return ServiceResult.Failure("请检查年级和学生人数");
            }

            return ServiceResult.Success();
        }
    }
}

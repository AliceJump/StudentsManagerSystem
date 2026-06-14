using StudentsManagerSystem.Common;
using StudentsManagerSystem.Data.Repositories;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Services
{
    internal sealed class StudentService
    {
        private readonly StudentRepository repository = new();

        public List<Student> GetAll() => repository.GetAll();

        public List<Student> Search(string keyword) => repository.Search(keyword);

        public List<FamilyInfo> GetFamilyInfos() => repository.GetFamilyInfos();

        public List<RewardRecord> GetRewardRecords() => repository.GetRewardRecords();

        public List<PunishmentRecord> GetPunishmentRecords() => repository.GetPunishmentRecords();

        public List<HealthRecord> GetHealthRecords() => repository.GetHealthRecords();

        public ServiceResult Add(Student student)
        {
            var validation = Validate(student, student.Id);
            if (!validation.Succeeded)
            {
                return validation;
            }

            repository.Add(student);
            AppLogger.Info($"新增学生：{student.StudentNo} {student.Name}");
            return ServiceResult.Success("学生信息新增成功");
        }

        public ServiceResult Update(Student student)
        {
            if (student.Id <= 0)
            {
                return ServiceResult.Failure("请选择要修改的学生记录");
            }

            var validation = Validate(student, student.Id);
            if (!validation.Succeeded)
            {
                return validation;
            }

            repository.Update(student);
            AppLogger.Info($"修改学生：{student.StudentNo} {student.Name}");
            return ServiceResult.Success("学生信息修改成功");
        }

        public ServiceResult SaveImported(Student student)
        {
            var existing = repository.GetAll().FirstOrDefault(item => item.StudentNo == student.StudentNo);
            if (existing == null)
            {
                return Add(student);
            }

            student.Id = existing.Id;
            return Update(student);
        }

        public void Delete(int id)
        {
            repository.Delete(id);
            AppLogger.Info($"删除学生：Id={id}");
        }

        public ServiceResult Validate(Student student, int excludeId = 0)
        {
            if (!InputValidator.ValidateStudentNo(student.StudentNo))
            {
                return ServiceResult.Failure("学号格式错误：应为4-20位字母或数字");
            }

            if (!InputValidator.ValidateName(student.Name))
            {
                return ServiceResult.Failure("姓名格式错误：应为2-50位汉字或字母");
            }

            if (string.IsNullOrWhiteSpace(student.Gender) || student.BirthDate == null)
            {
                return ServiceResult.Failure("请填写性别和出生日期");
            }

            if (!InputValidator.ValidateIdCard(student.IdCard))
            {
                return ServiceResult.Failure("身份证号格式错误：应为18位（末位可为X）");
            }

            if (!InputValidator.ValidatePhone(student.PhoneNumber))
            {
                return ServiceResult.Failure("手机号格式错误：应为11位数字且以1开头");
            }

            if (!InputValidator.ValidateEmail(student.Email))
            {
                return ServiceResult.Failure("邮箱格式错误");
            }

            if (string.IsNullOrWhiteSpace(student.Department) || string.IsNullOrWhiteSpace(student.Major) || string.IsNullOrWhiteSpace(student.Class))
            {
                return ServiceResult.Failure("请选择院系、专业和班级");
            }

            if (repository.StudentNoExists(student.StudentNo, excludeId))
            {
                return ServiceResult.Failure("学号已存在，请使用其他学号");
            }

            if (repository.IdCardExists(student.IdCard, excludeId))
            {
                return ServiceResult.Failure("身份证号已存在，请检查输入");
            }

            return ServiceResult.Success();
        }
    }
}

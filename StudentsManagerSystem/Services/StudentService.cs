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

        public ServiceResult<int> SaveFamilyInfo(FamilyInfo familyInfo)
        {
            var validation = ValidateLinkedStudent(familyInfo.StudentNo);
            if (!validation.Succeeded)
            {
                return ServiceResult<int>.Failure(validation.Message);
            }

            if (!InputValidator.ValidateName(familyInfo.RelationName) || string.IsNullOrWhiteSpace(familyInfo.Relationship))
            {
                return ServiceResult<int>.Failure("请填写关系人姓名和关系");
            }

            if (!string.IsNullOrWhiteSpace(familyInfo.PhoneNumber) && !InputValidator.ValidatePhone(familyInfo.PhoneNumber))
            {
                return ServiceResult<int>.Failure("联系电话格式错误");
            }

            familyInfo.StudentId = validation.Data;
            var id = repository.SaveFamilyInfo(familyInfo);
            AppLogger.Info($"保存家庭信息：{familyInfo.StudentNo} {familyInfo.RelationName}");
            return ServiceResult<int>.Success(id, "家庭信息保存成功");
        }

        public ServiceResult<int> SaveRewardRecord(RewardRecord rewardRecord)
        {
            var validation = ValidateLinkedStudent(rewardRecord.StudentNo);
            if (!validation.Succeeded)
            {
                return ServiceResult<int>.Failure(validation.Message);
            }

            if (rewardRecord.RewardDate == null || string.IsNullOrWhiteSpace(rewardRecord.RewardType) || string.IsNullOrWhiteSpace(rewardRecord.RewardLevel))
            {
                return ServiceResult<int>.Failure("请填写奖励日期、类型和等级");
            }

            rewardRecord.StudentId = validation.Data;
            var id = repository.SaveRewardRecord(rewardRecord);
            AppLogger.Info($"保存奖励记录：{rewardRecord.StudentNo} {rewardRecord.RewardType}");
            return ServiceResult<int>.Success(id, "奖励记录保存成功");
        }

        public ServiceResult<int> SavePunishmentRecord(PunishmentRecord punishmentRecord)
        {
            var validation = ValidateLinkedStudent(punishmentRecord.StudentNo);
            if (!validation.Succeeded)
            {
                return ServiceResult<int>.Failure(validation.Message);
            }

            if (punishmentRecord.PunishmentDate == null || string.IsNullOrWhiteSpace(punishmentRecord.PunishmentType) || string.IsNullOrWhiteSpace(punishmentRecord.PunishmentLevel) || string.IsNullOrWhiteSpace(punishmentRecord.Status))
            {
                return ServiceResult<int>.Failure("请填写处分日期、类型、等级和状态");
            }

            punishmentRecord.StudentId = validation.Data;
            var id = repository.SavePunishmentRecord(punishmentRecord);
            AppLogger.Info($"保存处分记录：{punishmentRecord.StudentNo} {punishmentRecord.PunishmentType}");
            return ServiceResult<int>.Success(id, "处分记录保存成功");
        }

        public ServiceResult<int> SaveHealthRecord(HealthRecord healthRecord)
        {
            var validation = ValidateLinkedStudent(healthRecord.StudentNo);
            if (!validation.Succeeded)
            {
                return ServiceResult<int>.Failure(validation.Message);
            }

            if (healthRecord.CheckDate == null || healthRecord.Height <= 0 || healthRecord.Weight <= 0 || string.IsNullOrWhiteSpace(healthRecord.BloodType) || string.IsNullOrWhiteSpace(healthRecord.Vision) || string.IsNullOrWhiteSpace(healthRecord.HealthStatus))
            {
                return ServiceResult<int>.Failure("请填写体检日期、身高、体重、血型、视力和健康状况");
            }

            healthRecord.StudentId = validation.Data;
            var id = repository.SaveHealthRecord(healthRecord);
            AppLogger.Info($"保存体检信息：{healthRecord.StudentNo} {healthRecord.CheckDate:yyyy-MM-dd}");
            return ServiceResult<int>.Success(id, "体检信息保存成功");
        }

        public void DeleteFamilyInfo(int id)
        {
            repository.DeleteFamilyInfo(id);
            AppLogger.Info($"删除家庭信息：Id={id}");
        }

        public void DeleteRewardRecord(int id)
        {
            repository.DeleteRewardRecord(id);
            AppLogger.Info($"删除奖励记录：Id={id}");
        }

        public void DeletePunishmentRecord(int id)
        {
            repository.DeletePunishmentRecord(id);
            AppLogger.Info($"删除处分记录：Id={id}");
        }

        public void DeleteHealthRecord(int id)
        {
            repository.DeleteHealthRecord(id);
            AppLogger.Info($"删除体检信息：Id={id}");
        }

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

        private ServiceResult<int> ValidateLinkedStudent(string studentNo)
        {
            if (!InputValidator.ValidateStudentNo(studentNo))
            {
                return ServiceResult<int>.Failure("学号格式错误：应为4-20位字母或数字");
            }

            var student = repository.GetByStudentNo(studentNo);
            return student == null
                ? ServiceResult<int>.Failure("学号不存在，请先维护学生基本信息")
                : ServiceResult<int>.Success(student.Id);
        }
    }
}

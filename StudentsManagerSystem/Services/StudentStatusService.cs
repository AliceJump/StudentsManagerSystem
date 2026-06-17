using StudentsManagerSystem.Common;
using StudentsManagerSystem.Data;
using StudentsManagerSystem.Data.Repositories;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Services
{
    internal sealed class StudentStatusService
    {
        private readonly StudentStatusRepository repository = new();

        private static string ResolveStudentName(string studentNo)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Students.Where(s => s.StudentNo == studentNo).Select(s => s.Name).FirstOrDefault() ?? string.Empty;
        }

        public List<StudentRegistration> GetRegistrations(string? keyword = null)
        {
            return string.IsNullOrWhiteSpace(keyword) ? repository.GetRegistrations() : repository.SearchRegistrations(keyword);
        }

        public List<StatusChangeRecord> GetChanges(string? keyword = null)
        {
            return string.IsNullOrWhiteSpace(keyword) ? repository.GetChanges() : repository.SearchChanges(keyword);
        }

        public List<ScholarshipInfo> GetScholarships(string? keyword = null)
        {
            return string.IsNullOrWhiteSpace(keyword) ? repository.GetScholarships() : repository.SearchScholarships(keyword);
        }

        public List<GraduationInfo> GetGraduations(string? keyword = null)
        {
            return string.IsNullOrWhiteSpace(keyword) ? repository.GetGraduations() : repository.SearchGraduations(keyword);
        }

        public ServiceResult<int> SaveRegistration(StudentRegistration record)
        {
            record.StudentName = ResolveStudentName(record.StudentNo);
            if (string.IsNullOrEmpty(record.StudentName))
            {
                return ServiceResult<int>.Failure("学号不存在，请先维护学生基本信息");
            }

            if (record.Id != 0 && !repository.RegistrationExists(record.Id))
            {
                return ServiceResult<int>.Failure("当前记录已不存在，请刷新后重试");
            }

            if (repository.RegistrationDuplicateExists(record))
            {
                return ServiceResult<int>.Failure("该学号已经存在登记记录");
            }

            if (record.Id == 0)
            {
                record.Id = repository.AddRegistration(record);
                AppLogger.Info($"新增学籍登记：{record.StudentNo}");
            }
            else
            {
                repository.UpdateRegistration(record);
                AppLogger.Info($"更新学籍登记：{record.StudentNo}");
            }

            return ServiceResult<int>.Success(record.Id);
        }

        public ServiceResult<int> SaveChange(StatusChangeRecord record)
        {
            record.StudentName = ResolveStudentName(record.StudentNo);
            if (string.IsNullOrEmpty(record.StudentName))
            {
                return ServiceResult<int>.Failure("学号不存在，请先维护学生基本信息");
            }

            if (record.Id != 0 && !repository.ChangeExists(record.Id))
            {
                return ServiceResult<int>.Failure("当前记录已不存在，请刷新后重试");
            }

            if (repository.ChangeDuplicateExists(record))
            {
                return ServiceResult<int>.Failure("该学号已经存在变动记录");
            }

            if (record.Id == 0)
            {
                record.Id = repository.AddChange(record);
                AppLogger.Info($"新增学籍变动：{record.StudentNo}");
            }
            else
            {
                repository.UpdateChange(record);
                AppLogger.Info($"更新学籍变动：{record.StudentNo}");
            }

            return ServiceResult<int>.Success(record.Id);
        }

        public ServiceResult<int> SaveScholarship(ScholarshipInfo record)
        {
            record.StudentName = ResolveStudentName(record.StudentNo);
            if (string.IsNullOrEmpty(record.StudentName))
            {
                return ServiceResult<int>.Failure("学号不存在，请先维护学生基本信息");
            }

            if (record.Id != 0 && !repository.ScholarshipExists(record.Id))
            {
                return ServiceResult<int>.Failure("当前记录已不存在，请刷新后重试");
            }

            if (repository.ScholarshipDuplicateExists(record))
            {
                return ServiceResult<int>.Failure("该学号已经存在奖助记录");
            }

            if (record.Id == 0)
            {
                record.Id = repository.AddScholarship(record);
                AppLogger.Info($"新增奖助记录：{record.StudentNo}");
            }
            else
            {
                repository.UpdateScholarship(record);
                AppLogger.Info($"更新奖助记录：{record.StudentNo}");
            }

            return ServiceResult<int>.Success(record.Id);
        }

        public ServiceResult<int> SaveGraduation(GraduationInfo record)
        {
            record.StudentName = ResolveStudentName(record.StudentNo);
            if (string.IsNullOrEmpty(record.StudentName))
            {
                return ServiceResult<int>.Failure("学号不存在，请先维护学生基本信息");
            }

            if (record.Id != 0 && !repository.GraduationExists(record.Id))
            {
                return ServiceResult<int>.Failure("当前记录已不存在，请刷新后重试");
            }

            if (repository.GraduationDuplicateExists(record))
            {
                return ServiceResult<int>.Failure("该学号已经存在毕业记录");
            }

            if (record.Id == 0)
            {
                record.Id = repository.AddGraduation(record);
                AppLogger.Info($"新增毕业记录：{record.StudentNo}");
            }
            else
            {
                repository.UpdateGraduation(record);
                AppLogger.Info($"更新毕业记录：{record.StudentNo}");
            }

            return ServiceResult<int>.Success(record.Id);
        }

        public void DeleteRegistration(int id)
        {
            repository.DeleteRegistration(id);
            AppLogger.Info($"删除学籍登记：{id}");
        }

        public void DeleteChange(int id)
        {
            repository.DeleteChange(id);
            AppLogger.Info($"删除学籍变动：{id}");
        }

        public void DeleteScholarship(int id)
        {
            repository.DeleteScholarship(id);
            AppLogger.Info($"删除奖助记录：{id}");
        }

        public void DeleteGraduation(int id)
        {
            repository.DeleteGraduation(id);
            AppLogger.Info($"删除毕业记录：{id}");
        }
    }
}

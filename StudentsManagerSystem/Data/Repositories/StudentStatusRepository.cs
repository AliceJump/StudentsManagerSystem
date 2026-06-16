using Microsoft.EntityFrameworkCore;
using StudentsManagerSystem.Data;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Data.Repositories
{
    /// <summary>
    /// 学籍、奖助、毕业仓储。
    /// </summary>
    internal sealed class StudentStatusRepository
    {
        public List<StudentRegistration> GetRegistrations() => QueryRegistrations(null);

        public List<StudentRegistration> SearchRegistrations(string keyword)
        {
            keyword = keyword.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return GetRegistrations();
            }

            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.StudentRegistrations.AsNoTracking()
                .Where(item =>
                    EF.Functions.Like(item.StudentNo, $"%{keyword}%") ||
                    EF.Functions.Like(item.StudentName, $"%{keyword}%") ||
                    EF.Functions.Like(item.AcademicYear, $"%{keyword}%") ||
                    EF.Functions.Like(item.Semester, $"%{keyword}%") ||
                    EF.Functions.Like(item.Status, $"%{keyword}%"))
                .OrderBy(item => item.StudentNo)
                .ToList();
        }

        public List<StatusChangeRecord> GetChanges() => QueryChanges(null);

        public List<StatusChangeRecord> SearchChanges(string keyword)
        {
            keyword = keyword.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return GetChanges();
            }

            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.StatusChangeRecords.AsNoTracking()
                .Where(item =>
                    EF.Functions.Like(item.StudentNo, $"%{keyword}%") ||
                    EF.Functions.Like(item.StudentName, $"%{keyword}%") ||
                    EF.Functions.Like(item.ChangeType, $"%{keyword}%") ||
                    EF.Functions.Like(item.OriginalInfo, $"%{keyword}%") ||
                    EF.Functions.Like(item.NewInfo, $"%{keyword}%") ||
                    EF.Functions.Like(item.Reason, $"%{keyword}%") ||
                    EF.Functions.Like(item.ApprovalStatus, $"%{keyword}%"))
                .OrderByDescending(item => item.ChangeDate)
                .ThenBy(item => item.StudentNo)
                .ToList();
        }

        public List<ScholarshipInfo> GetScholarships() => QueryScholarships(null);

        public List<ScholarshipInfo> SearchScholarships(string keyword)
        {
            keyword = keyword.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return GetScholarships();
            }

            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.ScholarshipInfos.AsNoTracking()
                .Where(item =>
                    EF.Functions.Like(item.StudentNo, $"%{keyword}%") ||
                    EF.Functions.Like(item.StudentName, $"%{keyword}%") ||
                    EF.Functions.Like(item.AcademicYear, $"%{keyword}%") ||
                    EF.Functions.Like(item.Semester, $"%{keyword}%") ||
                    EF.Functions.Like(item.ScholarshipType, $"%{keyword}%") ||
                    EF.Functions.Like(item.ScholarshipLevel, $"%{keyword}%") ||
                    EF.Functions.Like(item.Status, $"%{keyword}%"))
                .OrderByDescending(item => item.AwardDate)
                .ThenBy(item => item.StudentNo)
                .ToList();
        }

        public List<GraduationInfo> GetGraduations() => QueryGraduations(null);

        public List<GraduationInfo> SearchGraduations(string keyword)
        {
            keyword = keyword.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return GetGraduations();
            }

            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.GraduationInfos.AsNoTracking()
                .Where(item =>
                    EF.Functions.Like(item.StudentNo, $"%{keyword}%") ||
                    EF.Functions.Like(item.StudentName, $"%{keyword}%") ||
                    EF.Functions.Like(item.GraduationType, $"%{keyword}%") ||
                    EF.Functions.Like(item.DegreeType, $"%{keyword}%") ||
                    EF.Functions.Like(item.CertificateNo, $"%{keyword}%") ||
                    EF.Functions.Like(item.DegreeNo, $"%{keyword}%"))
                .OrderByDescending(item => item.GraduationDate)
                .ThenBy(item => item.StudentNo)
                .ToList();
        }

        public void DeleteRegistration(int id) => DeleteEntity<StudentRegistration>(id);

        public void DeleteChange(int id) => DeleteEntity<StatusChangeRecord>(id);

        public void DeleteScholarship(int id) => DeleteEntity<ScholarshipInfo>(id);

        public void DeleteGraduation(int id) => DeleteEntity<GraduationInfo>(id);

        public int AddChange(StatusChangeRecord record)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.StatusChangeRecords.Add(record);
            context.SaveChanges();
            return record.Id;
        }

        public void UpdateChange(StatusChangeRecord record)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.StatusChangeRecords.Update(record);
            context.SaveChanges();
        }

        public int AddScholarship(ScholarshipInfo record)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.ScholarshipInfos.Add(record);
            context.SaveChanges();
            return record.Id;
        }

        public void UpdateScholarship(ScholarshipInfo record)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.ScholarshipInfos.Update(record);
            context.SaveChanges();
        }

        public int AddGraduation(GraduationInfo record)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.GraduationInfos.Add(record);
            context.SaveChanges();
            return record.Id;
        }

        public void UpdateGraduation(GraduationInfo record)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.GraduationInfos.Update(record);
            context.SaveChanges();
        }

        public bool RegistrationExists(int id) => ExistsById<StudentRegistration>(id);

        public bool ChangeExists(int id) => ExistsById<StatusChangeRecord>(id);

        public bool ScholarshipExists(int id) => ExistsById<ScholarshipInfo>(id);

        public bool GraduationExists(int id) => ExistsById<GraduationInfo>(id);

        public bool RegistrationDuplicateExists(StudentRegistration record)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.StudentRegistrations.Any(item => item.StudentNo == record.StudentNo && item.Id != record.Id);
        }

        public bool ChangeDuplicateExists(StatusChangeRecord record)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.StatusChangeRecords.Any(item => item.StudentNo == record.StudentNo && item.Id != record.Id);
        }

        public bool ScholarshipDuplicateExists(ScholarshipInfo record)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.ScholarshipInfos.Any(item => item.StudentNo == record.StudentNo && item.Id != record.Id);
        }

        public bool GraduationDuplicateExists(GraduationInfo record)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.GraduationInfos.Any(item => item.StudentNo == record.StudentNo && item.Id != record.Id);
        }

        public int AddRegistration(StudentRegistration reg)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.StudentRegistrations.Add(reg);
            context.SaveChanges();
            return reg.Id;
        }

        public void UpdateRegistration(StudentRegistration reg)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.StudentRegistrations.Update(reg);
            context.SaveChanges();
        }

        private static List<StudentRegistration> QueryRegistrations(string? keyword)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var query = context.StudentRegistrations.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(item =>
                    EF.Functions.Like(item.StudentNo, $"%{keyword}%") ||
                    EF.Functions.Like(item.StudentName, $"%{keyword}%") ||
                    EF.Functions.Like(item.AcademicYear, $"%{keyword}%") ||
                    EF.Functions.Like(item.Semester, $"%{keyword}%") ||
                    EF.Functions.Like(item.Status, $"%{keyword}%"));
            }

            return query.OrderBy(item => item.StudentNo)
                .ToList()
                .Select(NormalizeRegistrationSemester)
                .ToList();
        }

        private static List<StatusChangeRecord> QueryChanges(string? keyword)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var query = context.StatusChangeRecords.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(item =>
                    EF.Functions.Like(item.StudentNo, $"%{keyword}%") ||
                    EF.Functions.Like(item.StudentName, $"%{keyword}%") ||
                    EF.Functions.Like(item.ChangeType, $"%{keyword}%") ||
                    EF.Functions.Like(item.OriginalInfo, $"%{keyword}%") ||
                    EF.Functions.Like(item.NewInfo, $"%{keyword}%") ||
                    EF.Functions.Like(item.Reason, $"%{keyword}%") ||
                    EF.Functions.Like(item.ApprovalStatus, $"%{keyword}%"));
            }

            return query.OrderByDescending(item => item.ChangeDate).ThenBy(item => item.StudentNo).ToList();
        }

        private static List<ScholarshipInfo> QueryScholarships(string? keyword)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var query = context.ScholarshipInfos.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(item =>
                    EF.Functions.Like(item.StudentNo, $"%{keyword}%") ||
                    EF.Functions.Like(item.StudentName, $"%{keyword}%") ||
                    EF.Functions.Like(item.AcademicYear, $"%{keyword}%") ||
                    EF.Functions.Like(item.Semester, $"%{keyword}%") ||
                    EF.Functions.Like(item.ScholarshipType, $"%{keyword}%") ||
                    EF.Functions.Like(item.ScholarshipLevel, $"%{keyword}%") ||
                    EF.Functions.Like(item.Status, $"%{keyword}%"));
            }

            return query.OrderByDescending(item => item.AwardDate).ThenBy(item => item.StudentNo).ToList();
        }

        private static List<GraduationInfo> QueryGraduations(string? keyword)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var query = context.GraduationInfos.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(item =>
                    EF.Functions.Like(item.StudentNo, $"%{keyword}%") ||
                    EF.Functions.Like(item.StudentName, $"%{keyword}%") ||
                    EF.Functions.Like(item.GraduationType, $"%{keyword}%") ||
                    EF.Functions.Like(item.DegreeType, $"%{keyword}%") ||
                    EF.Functions.Like(item.CertificateNo, $"%{keyword}%") ||
                    EF.Functions.Like(item.DegreeNo, $"%{keyword}%"));
            }

            return query.OrderByDescending(item => item.GraduationDate).ThenBy(item => item.StudentNo).ToList();
        }

        private static StudentRegistration NormalizeRegistrationSemester(StudentRegistration record)
        {
            record.Semester = record.Semester switch
            {
                "1" => "第一学期",
                "2" => "第二学期",
                _ => record.Semester
            };
            return record;
        }

        private static bool ExistsById<TEntity>(int id) where TEntity : class
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Set<TEntity>().Any(item => EF.Property<int>(item, "Id") == id);
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

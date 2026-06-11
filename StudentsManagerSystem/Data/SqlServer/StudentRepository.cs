using Microsoft.EntityFrameworkCore;
using StudentsManagerSystem.Data;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Data.SqlServer
{
    /// <summary>
    /// 学生信息仓储。
    /// </summary>
    internal sealed class StudentRepository
    {
        public List<Student> GetAll()
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Students.AsNoTracking().OrderBy(student => student.StudentNo).ToList();
        }

        public Task<List<Student>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Students.AsNoTracking().OrderBy(student => student.StudentNo).ToListAsync(cancellationToken);
        }

        public List<Student> Search(string keyword)
        {
            keyword = keyword.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return GetAll();
            }

            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Students.AsNoTracking()
                .Where(student =>
                    EF.Functions.Like(student.StudentNo, $"%{keyword}%") ||
                    EF.Functions.Like(student.Name, $"%{keyword}%") ||
                    EF.Functions.Like(student.Department, $"%{keyword}%") ||
                    EF.Functions.Like(student.Major, $"%{keyword}%") ||
                    EF.Functions.Like(student.Class, $"%{keyword}%"))
                .OrderBy(student => student.StudentNo)
                .ToList();
        }

        public Student? GetById(int id)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Students.AsNoTracking().FirstOrDefault(student => student.Id == id);
        }

        public List<FamilyInfo> GetFamilyInfos()
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.FamilyInfos.AsNoTracking().OrderBy(item => item.StudentId).ThenBy(item => item.RelationName).ToList();
        }

        public List<RewardRecord> GetRewardRecords()
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.RewardRecords.AsNoTracking().OrderByDescending(item => item.RewardDate).ThenBy(item => item.StudentId).ToList();
        }

        public List<PunishmentRecord> GetPunishmentRecords()
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.PunishmentRecords.AsNoTracking().OrderByDescending(item => item.PunishmentDate).ThenBy(item => item.StudentId).ToList();
        }

        public List<HealthRecord> GetHealthRecords()
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.HealthRecords.AsNoTracking().OrderByDescending(item => item.CheckDate).ThenBy(item => item.StudentId).ToList();
        }

        public int Add(Student student)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.Students.Add(student);
            context.SaveChanges();
            return student.Id;
        }

        public async Task<int> AddAsync(Student student, CancellationToken cancellationToken = default)
        {
            await using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.Students.Add(student);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return student.Id;
        }

        public void Update(Student student)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.Students.Update(student);
            context.SaveChanges();
        }

        public async Task UpdateAsync(Student student, CancellationToken cancellationToken = default)
        {
            await using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.Students.Update(student);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public void Delete(int id)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var student = context.Students.FirstOrDefault(item => item.Id == id);
            if (student == null)
            {
                return;
            }

            context.FamilyInfos.RemoveRange(context.FamilyInfos.Where(item => item.StudentId == id));
            context.RewardRecords.RemoveRange(context.RewardRecords.Where(item => item.StudentId == id));
            context.PunishmentRecords.RemoveRange(context.PunishmentRecords.Where(item => item.StudentId == id));
            context.HealthRecords.RemoveRange(context.HealthRecords.Where(item => item.StudentId == id));
            context.StudentRegistrations.RemoveRange(context.StudentRegistrations.Where(item => item.StudentId == id));
            context.StatusChangeRecords.RemoveRange(context.StatusChangeRecords.Where(item => item.StudentId == id));
            context.ScholarshipInfos.RemoveRange(context.ScholarshipInfos.Where(item => item.StudentId == id));
            context.GraduationInfos.RemoveRange(context.GraduationInfos.Where(item => item.StudentId == id));
            context.Scores.RemoveRange(context.Scores.Where(item => item.StudentId == id));
            context.Students.Remove(student);
            context.SaveChanges();
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var student = await context.Students.FirstOrDefaultAsync(item => item.Id == id, cancellationToken).ConfigureAwait(false);
            if (student == null)
            {
                return;
            }

            context.FamilyInfos.RemoveRange(context.FamilyInfos.Where(item => item.StudentId == id));
            context.RewardRecords.RemoveRange(context.RewardRecords.Where(item => item.StudentId == id));
            context.PunishmentRecords.RemoveRange(context.PunishmentRecords.Where(item => item.StudentId == id));
            context.HealthRecords.RemoveRange(context.HealthRecords.Where(item => item.StudentId == id));
            context.StudentRegistrations.RemoveRange(context.StudentRegistrations.Where(item => item.StudentId == id));
            context.StatusChangeRecords.RemoveRange(context.StatusChangeRecords.Where(item => item.StudentId == id));
            context.ScholarshipInfos.RemoveRange(context.ScholarshipInfos.Where(item => item.StudentId == id));
            context.GraduationInfos.RemoveRange(context.GraduationInfos.Where(item => item.StudentId == id));
            context.Scores.RemoveRange(context.Scores.Where(item => item.StudentId == id));
            context.Students.Remove(student);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public bool StudentNoExists(string studentNo, int excludeId = 0)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Students.Any(student => student.StudentNo == studentNo && student.Id != excludeId);
        }

        public bool IdCardExists(string idCard, int excludeId = 0)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Students.Any(student => student.IdCard == idCard && student.Id != excludeId);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using StudentsManagerSystem.Data;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Data.Repositories
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

        public Student? GetByStudentNo(string studentNo)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Students.AsNoTracking().FirstOrDefault(student => student.StudentNo == studentNo);
        }

        public List<FamilyInfo> GetFamilyInfos()
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.FamilyInfos.AsNoTracking().OrderBy(item => item.StudentNo).ThenBy(item => item.RelationName).ToList();
        }

        public List<RewardRecord> GetRewardRecords()
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.RewardRecords.AsNoTracking().OrderByDescending(item => item.RewardDate).ThenBy(item => item.StudentNo).ToList();
        }

        public List<PunishmentRecord> GetPunishmentRecords()
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.PunishmentRecords.AsNoTracking().OrderByDescending(item => item.PunishmentDate).ThenBy(item => item.StudentNo).ToList();
        }

        public List<HealthRecord> GetHealthRecords()
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.HealthRecords.AsNoTracking().OrderByDescending(item => item.CheckDate).ThenBy(item => item.StudentNo).ToList();
        }

        public int SaveFamilyInfo(FamilyInfo familyInfo)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            if (familyInfo.Id == 0)
            {
                context.FamilyInfos.Add(familyInfo);
            }
            else
            {
                context.FamilyInfos.Update(familyInfo);
            }

            context.SaveChanges();
            return familyInfo.Id;
        }

        public int SaveRewardRecord(RewardRecord rewardRecord)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            if (rewardRecord.Id == 0)
            {
                context.RewardRecords.Add(rewardRecord);
            }
            else
            {
                context.RewardRecords.Update(rewardRecord);
            }

            context.SaveChanges();
            return rewardRecord.Id;
        }

        public int SavePunishmentRecord(PunishmentRecord punishmentRecord)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            if (punishmentRecord.Id == 0)
            {
                context.PunishmentRecords.Add(punishmentRecord);
            }
            else
            {
                context.PunishmentRecords.Update(punishmentRecord);
            }

            context.SaveChanges();
            return punishmentRecord.Id;
        }

        public int SaveHealthRecord(HealthRecord healthRecord)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            if (healthRecord.Id == 0)
            {
                context.HealthRecords.Add(healthRecord);
            }
            else
            {
                context.HealthRecords.Update(healthRecord);
            }

            context.SaveChanges();
            return healthRecord.Id;
        }

        public void DeleteFamilyInfo(int id)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var item = context.FamilyInfos.FirstOrDefault(record => record.Id == id);
            if (item != null)
            {
                context.FamilyInfos.Remove(item);
                context.SaveChanges();
            }
        }

        public void DeleteRewardRecord(int id)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var item = context.RewardRecords.FirstOrDefault(record => record.Id == id);
            if (item != null)
            {
                context.RewardRecords.Remove(item);
                context.SaveChanges();
            }
        }

        public void DeletePunishmentRecord(int id)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var item = context.PunishmentRecords.FirstOrDefault(record => record.Id == id);
            if (item != null)
            {
                context.PunishmentRecords.Remove(item);
                context.SaveChanges();
            }
        }

        public void DeleteHealthRecord(int id)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var item = context.HealthRecords.FirstOrDefault(record => record.Id == id);
            if (item != null)
            {
                context.HealthRecords.Remove(item);
                context.SaveChanges();
            }
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
            var original = context.Students.AsNoTracking().FirstOrDefault(s => s.Id == student.Id);
            if (original != null && original.Name != student.Name)
            {
                SyncStudentName(context, student.StudentNo, student.Name);
            }

            context.Students.Update(student);
            context.SaveChanges();
        }

        public async Task UpdateAsync(Student student, CancellationToken cancellationToken = default)
        {
            await using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var original = await context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == student.Id, cancellationToken).ConfigureAwait(false);
            if (original != null && original.Name != student.Name)
            {
                await SyncStudentNameAsync(context, student.StudentNo, student.Name, cancellationToken).ConfigureAwait(false);
            }

            context.Students.Update(student);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        private static void SyncStudentName(StudentsManagerDbContext context, string studentNo, string newName)
        {
            context.Scores.Where(s => s.StudentNo == studentNo).ExecuteUpdate(setters => setters.SetProperty(s => s.StudentName, newName));
            context.StudentRegistrations.Where(s => s.StudentNo == studentNo).ExecuteUpdate(setters => setters.SetProperty(s => s.StudentName, newName));
            context.StatusChangeRecords.Where(s => s.StudentNo == studentNo).ExecuteUpdate(setters => setters.SetProperty(s => s.StudentName, newName));
            context.ScholarshipInfos.Where(s => s.StudentNo == studentNo).ExecuteUpdate(setters => setters.SetProperty(s => s.StudentName, newName));
            context.GraduationInfos.Where(s => s.StudentNo == studentNo).ExecuteUpdate(setters => setters.SetProperty(s => s.StudentName, newName));
        }

        private static async Task SyncStudentNameAsync(StudentsManagerDbContext context, string studentNo, string newName, CancellationToken cancellationToken = default)
        {
            await context.Scores.Where(s => s.StudentNo == studentNo).ExecuteUpdateAsync(setters => setters.SetProperty(s => s.StudentName, newName), cancellationToken).ConfigureAwait(false);
            await context.StudentRegistrations.Where(s => s.StudentNo == studentNo).ExecuteUpdateAsync(setters => setters.SetProperty(s => s.StudentName, newName), cancellationToken).ConfigureAwait(false);
            await context.StatusChangeRecords.Where(s => s.StudentNo == studentNo).ExecuteUpdateAsync(setters => setters.SetProperty(s => s.StudentName, newName), cancellationToken).ConfigureAwait(false);
            await context.ScholarshipInfos.Where(s => s.StudentNo == studentNo).ExecuteUpdateAsync(setters => setters.SetProperty(s => s.StudentName, newName), cancellationToken).ConfigureAwait(false);
            await context.GraduationInfos.Where(s => s.StudentNo == studentNo).ExecuteUpdateAsync(setters => setters.SetProperty(s => s.StudentName, newName), cancellationToken).ConfigureAwait(false);
        }

        public void Delete(int id)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var student = context.Students.FirstOrDefault(item => item.Id == id);
            if (student == null)
            {
                return;
            }

            context.FamilyInfos.RemoveRange(context.FamilyInfos.Where(item => item.StudentNo == student.StudentNo));
            context.RewardRecords.RemoveRange(context.RewardRecords.Where(item => item.StudentNo == student.StudentNo));
            context.PunishmentRecords.RemoveRange(context.PunishmentRecords.Where(item => item.StudentNo == student.StudentNo));
            context.HealthRecords.RemoveRange(context.HealthRecords.Where(item => item.StudentNo == student.StudentNo));
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

            context.FamilyInfos.RemoveRange(context.FamilyInfos.Where(item => item.StudentNo == student.StudentNo));
            context.RewardRecords.RemoveRange(context.RewardRecords.Where(item => item.StudentNo == student.StudentNo));
            context.PunishmentRecords.RemoveRange(context.PunishmentRecords.Where(item => item.StudentNo == student.StudentNo));
            context.HealthRecords.RemoveRange(context.HealthRecords.Where(item => item.StudentNo == student.StudentNo));
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

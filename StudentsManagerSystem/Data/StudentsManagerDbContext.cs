using Microsoft.EntityFrameworkCore;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Data
{
    /// <summary>
    /// 学生管理系统数据库上下文。
    /// </summary>
    public sealed class StudentsManagerDbContext : DbContext
    {
        public StudentsManagerDbContext(DbContextOptions<StudentsManagerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students => Set<Student>();

        public DbSet<FamilyInfo> FamilyInfos => Set<FamilyInfo>();

        public DbSet<RewardRecord> RewardRecords => Set<RewardRecord>();

        public DbSet<PunishmentRecord> PunishmentRecords => Set<PunishmentRecord>();

        public DbSet<HealthRecord> HealthRecords => Set<HealthRecord>();

        public DbSet<StudentRegistration> StudentRegistrations => Set<StudentRegistration>();

        public DbSet<StatusChangeRecord> StatusChangeRecords => Set<StatusChangeRecord>();

        public DbSet<ScholarshipInfo> ScholarshipInfos => Set<ScholarshipInfo>();

        public DbSet<GraduationInfo> GraduationInfos => Set<GraduationInfo>();

        public DbSet<Score> Scores => Set<Score>();

        public DbSet<Course> Courses => Set<Course>();

        public DbSet<Department> Departments => Set<Department>();

        public DbSet<Major> Majors => Set<Major>();

        public DbSet<Class> Classes => Set<Class>();

        public DbSet<User> Users => Set<User>();

        public DbSet<LookupOption> LookupOptions => Set<LookupOption>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(DatabaseConfiguration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Students");
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.StudentNo).IsUnique();
                entity.HasIndex(x => x.IdCard).IsUnique();
                entity.Property(x => x.StudentNo).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Gender).HasMaxLength(20).IsRequired();
                entity.Property(x => x.IdCard).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Nation).HasMaxLength(50).IsRequired();
                entity.Property(x => x.PoliticalStatus).HasMaxLength(50).IsRequired();
                entity.Property(x => x.PhoneNumber).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Email).HasMaxLength(100);
                entity.Property(x => x.Address).HasMaxLength(200);
                entity.Property(x => x.Department).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Major).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Class).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Photo).HasMaxLength(500);
            });

            modelBuilder.Entity<FamilyInfo>(entity =>
            {
                entity.ToTable("FamilyInfos");
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.StudentNo);
                entity.Property(x => x.StudentNo).HasMaxLength(50).IsRequired();
                entity.Property(x => x.RelationName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Relationship).HasMaxLength(50).IsRequired();
                entity.Property(x => x.PhoneNumber).HasMaxLength(50);
                entity.Property(x => x.WorkUnit).HasMaxLength(200);
                entity.Property(x => x.Address).HasMaxLength(200);
            });

            modelBuilder.Entity<RewardRecord>(entity =>
            {
                entity.ToTable("RewardRecords");
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.StudentNo);
                entity.Property(x => x.StudentNo).HasMaxLength(50).IsRequired();
                entity.Property(x => x.RewardType).HasMaxLength(100).IsRequired();
                entity.Property(x => x.RewardLevel).HasMaxLength(100).IsRequired();
                entity.Property(x => x.RewardReason).HasMaxLength(200);
                entity.Property(x => x.RewardUnit).HasMaxLength(200);
            });

            modelBuilder.Entity<PunishmentRecord>(entity =>
            {
                entity.ToTable("PunishmentRecords");
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.StudentNo);
                entity.Property(x => x.StudentNo).HasMaxLength(50).IsRequired();
                entity.Property(x => x.PunishmentType).HasMaxLength(100).IsRequired();
                entity.Property(x => x.PunishmentLevel).HasMaxLength(100).IsRequired();
                entity.Property(x => x.PunishmentReason).HasMaxLength(200);
                entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            });

            modelBuilder.Entity<HealthRecord>(entity =>
            {
                entity.ToTable("HealthRecords");
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.StudentNo);
                entity.Property(x => x.StudentNo).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Height).HasPrecision(6, 2);
                entity.Property(x => x.Weight).HasPrecision(6, 2);
                entity.Property(x => x.BloodType).HasMaxLength(20).IsRequired();
                entity.Property(x => x.Vision).HasMaxLength(20).IsRequired();
                entity.Property(x => x.HealthStatus).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Remarks).HasMaxLength(200);
            });

            modelBuilder.Entity<StudentRegistration>(entity =>
            {
                entity.ToTable("StudentRegistrations");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.StudentNo).HasMaxLength(50).IsRequired();
                entity.Property(x => x.StudentName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.AcademicYear).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Semester).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Remarks).HasMaxLength(200);
            });

            modelBuilder.Entity<StatusChangeRecord>(entity =>
            {
                entity.ToTable("StatusChangeRecords");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.StudentNo).HasMaxLength(50).IsRequired();
                entity.Property(x => x.StudentName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.ChangeType).HasMaxLength(100).IsRequired();
                entity.Property(x => x.OriginalInfo).HasMaxLength(200);
                entity.Property(x => x.NewInfo).HasMaxLength(200);
                entity.Property(x => x.Reason).HasMaxLength(200);
                entity.Property(x => x.ApprovalStatus).HasMaxLength(50).IsRequired();
            });

            modelBuilder.Entity<ScholarshipInfo>(entity =>
            {
                entity.ToTable("ScholarshipInfos");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.StudentNo).HasMaxLength(50).IsRequired();
                entity.Property(x => x.StudentName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.AcademicYear).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Semester).HasMaxLength(50).IsRequired();
                entity.Property(x => x.ScholarshipType).HasMaxLength(100).IsRequired();
                entity.Property(x => x.ScholarshipLevel).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Amount).HasPrecision(18, 2);
                entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            });

            modelBuilder.Entity<GraduationInfo>(entity =>
            {
                entity.ToTable("GraduationInfos");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.StudentNo).HasMaxLength(50).IsRequired();
                entity.Property(x => x.StudentName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.GraduationType).HasMaxLength(100).IsRequired();
                entity.Property(x => x.DegreeType).HasMaxLength(100).IsRequired();
                entity.Property(x => x.CertificateNo).HasMaxLength(100);
                entity.Property(x => x.DegreeNo).HasMaxLength(100);
                entity.Property(x => x.Remarks).HasMaxLength(200);
            });

            modelBuilder.Entity<Score>(entity =>
            {
                entity.ToTable("Scores");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.StudentNo).HasMaxLength(50).IsRequired();
                entity.Property(x => x.StudentName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.AcademicYear).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Semester).HasMaxLength(50).IsRequired();
                entity.Property(x => x.CourseNo).HasMaxLength(50).IsRequired();
                entity.Property(x => x.CourseName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Credit).HasPrecision(6, 2);
                entity.Property(x => x.RegularScore).HasPrecision(6, 2);
                entity.Property(x => x.ExamScore).HasPrecision(6, 2);
                entity.Property(x => x.TotalScore).HasPrecision(6, 2);
                entity.Property(x => x.Grade).HasMaxLength(20).IsRequired();
                entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Remarks).HasMaxLength(200);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses");
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.CourseNo).IsUnique();
                entity.Property(x => x.CourseNo).HasMaxLength(50).IsRequired();
                entity.Property(x => x.CourseName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Credit).HasPrecision(6, 2);
                entity.Property(x => x.CourseType).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Department).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Remarks).HasMaxLength(200);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Departments");
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.DepartmentNo).IsUnique();
                entity.Property(x => x.DepartmentNo).HasMaxLength(50).IsRequired();
                entity.Property(x => x.DepartmentName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.DepartmentHead).HasMaxLength(100);
                entity.Property(x => x.PhoneNumber).HasMaxLength(50);
                entity.Property(x => x.Remarks).HasMaxLength(200);
            });

            modelBuilder.Entity<Major>(entity =>
            {
                entity.ToTable("Majors");
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.MajorNo).IsUnique();
                entity.Property(x => x.MajorNo).HasMaxLength(50).IsRequired();
                entity.Property(x => x.MajorName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.DepartmentName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.DegreeType).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Remarks).HasMaxLength(200);
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("Classes");
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.ClassNo).IsUnique();
                entity.Property(x => x.ClassNo).HasMaxLength(50).IsRequired();
                entity.Property(x => x.ClassName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.DepartmentName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.MajorName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Grade).HasMaxLength(20).IsRequired();
                entity.Property(x => x.ClassTeacher).HasMaxLength(100);
                entity.Property(x => x.Remarks).HasMaxLength(200);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.Username).IsUnique();
                entity.Property(x => x.Username).HasMaxLength(50).IsRequired();
                entity.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();
                entity.Property(x => x.DisplayName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Role).HasMaxLength(50).IsRequired();
            });

            modelBuilder.Entity<LookupOption>(entity =>
            {
                entity.ToTable("LookupOptions");
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => new { x.Category, x.Value }).IsUnique();
                entity.Property(x => x.Category).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Value).HasMaxLength(100).IsRequired();
            });
        }
    }
}

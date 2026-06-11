using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StudentsManagerSystem.Common;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Data
{
    /// <summary>
    /// 数据库初始化与种子数据。
    /// </summary>
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            AppLogger.Info("开始初始化 SQLite 数据库。");

            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.Database.EnsureCreated();
            EnsureLookupOptionsTable(context);

            SeedLookupOptions(context);
            SeedDepartments(context);
            SeedMajors(context);
            SeedClasses(context);
            SeedCourses(context);
            SeedStudents(context);
            SeedStudentArchives(context);
            SeedScores(context);
            SeedUsers(context);

            AppLogger.Info("SQLite 数据库初始化完成。");
        }

        private static void EnsureLookupOptionsTable(StudentsManagerDbContext context)
        {
            context.Database.ExecuteSqlRaw(@"
CREATE TABLE IF NOT EXISTS LookupOptions (
    Id INTEGER NOT NULL CONSTRAINT PK_LookupOptions PRIMARY KEY AUTOINCREMENT,
    Category TEXT NOT NULL,
    Value TEXT NOT NULL,
    SortOrder INTEGER NOT NULL,
    IsActive INTEGER NOT NULL
);");
            context.Database.ExecuteSqlRaw("CREATE UNIQUE INDEX IF NOT EXISTS IX_LookupOptions_Category_Value ON LookupOptions (Category, Value);");
        }

        private static void SeedLookupOptions(StudentsManagerDbContext context)
        {
            if (context.LookupOptions.Any())
            {
                return;
            }

            context.LookupOptions.AddRange(
                new LookupOption { Category = "Gender", Value = "男", SortOrder = 1 },
                new LookupOption { Category = "Gender", Value = "女", SortOrder = 2 },
                new LookupOption { Category = "PoliticalStatus", Value = "群众", SortOrder = 1 },
                new LookupOption { Category = "PoliticalStatus", Value = "团员", SortOrder = 2 },
                new LookupOption { Category = "PoliticalStatus", Value = "党员", SortOrder = 3 },
                new LookupOption { Category = "Semester", Value = "第一学期", SortOrder = 1 },
                new LookupOption { Category = "Semester", Value = "第二学期", SortOrder = 2 },
                new LookupOption { Category = "ScholarshipStatus", Value = "待发放", SortOrder = 1 },
                new LookupOption { Category = "ScholarshipStatus", Value = "已发放", SortOrder = 2 },
                new LookupOption { Category = "ApprovalStatus", Value = "待审批", SortOrder = 1 },
                new LookupOption { Category = "ApprovalStatus", Value = "已批准", SortOrder = 2 },
                new LookupOption { Category = "ApprovalStatus", Value = "已驳回", SortOrder = 3 });
            context.SaveChanges();
        }

        private static void SeedDepartments(StudentsManagerDbContext context)
        {
            if (context.Departments.Any())
            {
                return;
            }

            context.Departments.AddRange(
                new Department { DepartmentNo = "CS", DepartmentName = "计算机学院", DepartmentHead = "李主任", PhoneNumber = "010-10000001", Remarks = "软件与计算机相关专业" },
                new Department { DepartmentNo = "EE", DepartmentName = "电子信息学院", DepartmentHead = "王主任", PhoneNumber = "010-10000002", Remarks = "电子信息与通信方向" },
                new Department { DepartmentNo = "ME", DepartmentName = "机械工程学院", DepartmentHead = "赵主任", PhoneNumber = "010-10000003", Remarks = "机械设计与制造" });
            context.SaveChanges();
        }

        private static void SeedMajors(StudentsManagerDbContext context)
        {
            if (context.Majors.Any())
            {
                return;
            }

            context.Majors.AddRange(
                new Major { MajorNo = "CS001", MajorName = "软件工程", DepartmentName = "计算机学院", Duration = 4, DegreeType = "工学学士", Remarks = "课程体系完善" },
                new Major { MajorNo = "CS002", MajorName = "计算机科学与技术", DepartmentName = "计算机学院", Duration = 4, DegreeType = "工学学士", Remarks = "通用计算机方向" },
                new Major { MajorNo = "EE001", MajorName = "电子信息工程", DepartmentName = "电子信息学院", Duration = 4, DegreeType = "工学学士", Remarks = "电子与通信方向" });
            context.SaveChanges();
        }

        private static void SeedClasses(StudentsManagerDbContext context)
        {
            if (context.Classes.Any())
            {
                return;
            }

            context.Classes.AddRange(
                new Class { ClassNo = "CS2024-1", ClassName = "软工2024-1班", DepartmentName = "计算机学院", MajorName = "软件工程", Grade = "2024", ClassTeacher = "张老师", StudentCount = 2, Remarks = "示例班级" },
                new Class { ClassNo = "CS2024-2", ClassName = "计科2024-1班", DepartmentName = "计算机学院", MajorName = "计算机科学与技术", Grade = "2024", ClassTeacher = "李老师", StudentCount = 1, Remarks = "示例班级" },
                new Class { ClassNo = "EE2024-1", ClassName = "电信2024-1班", DepartmentName = "电子信息学院", MajorName = "电子信息工程", Grade = "2024", ClassTeacher = "王老师", StudentCount = 1, Remarks = "示例班级" });
            context.SaveChanges();
        }

        private static void SeedCourses(StudentsManagerDbContext context)
        {
            if (context.Courses.Any())
            {
                return;
            }

            context.Courses.AddRange(
                new Course { CourseNo = "C001", CourseName = "C#程序设计", Credit = 4, CourseType = "必修", Hours = 64, Department = "计算机学院", Remarks = "核心课程" },
                new Course { CourseNo = "C002", CourseName = "数据库原理", Credit = 3, CourseType = "必修", Hours = 48, Department = "计算机学院", Remarks = "核心课程" },
                new Course { CourseNo = "C003", CourseName = "数据结构", Credit = 4, CourseType = "必修", Hours = 64, Department = "计算机学院", Remarks = "核心课程" },
                new Course { CourseNo = "C004", CourseName = "计算机网络", Credit = 3, CourseType = "必修", Hours = 48, Department = "计算机学院", Remarks = "核心课程" });
            context.SaveChanges();
        }

        private static void SeedStudents(StudentsManagerDbContext context)
        {
            if (context.Students.Any())
            {
                return;
            }

            context.Students.AddRange(
                new Student
                {
                    StudentNo = "2024001",
                    Name = "张三",
                    Gender = "男",
                    BirthDate = new DateTime(2005, 3, 15),
                    IdCard = "110101200503150011",
                    Nation = "汉族",
                    PoliticalStatus = "团员",
                    PhoneNumber = "13800138001",
                    Email = "zhangsan@example.com",
                    Address = "北京市海淀区",
                    Department = "计算机学院",
                    Major = "软件工程",
                    Class = "软工2024-1班",
                    EnrollmentDate = new DateTime(2024, 9, 1)
                },
                new Student
                {
                    StudentNo = "2024002",
                    Name = "李四",
                    Gender = "女",
                    BirthDate = new DateTime(2005, 6, 20),
                    IdCard = "110101200506200022",
                    Nation = "汉族",
                    PoliticalStatus = "群众",
                    PhoneNumber = "13800138002",
                    Email = "lisi@example.com",
                    Address = "北京市朝阳区",
                    Department = "计算机学院",
                    Major = "软件工程",
                    Class = "软工2024-1班",
                    EnrollmentDate = new DateTime(2024, 9, 1)
                },
                new Student
                {
                    StudentNo = "2024003",
                    Name = "王五",
                    Gender = "男",
                    BirthDate = new DateTime(2005, 8, 10),
                    IdCard = "110101200508100033",
                    Nation = "汉族",
                    PoliticalStatus = "团员",
                    PhoneNumber = "13800138003",
                    Email = "wangwu@example.com",
                    Address = "北京市丰台区",
                    Department = "计算机学院",
                    Major = "计算机科学与技术",
                    Class = "计科2024-1班",
                    EnrollmentDate = new DateTime(2024, 9, 1)
                },
                new Student
                {
                    StudentNo = "2024004",
                    Name = "赵六",
                    Gender = "女",
                    BirthDate = new DateTime(2005, 4, 25),
                    IdCard = "110101200504250044",
                    Nation = "汉族",
                    PoliticalStatus = "党员",
                    PhoneNumber = "13800138004",
                    Email = "zhaoliu@example.com",
                    Address = "北京市通州区",
                    Department = "电子信息学院",
                    Major = "电子信息工程",
                    Class = "电信2024-1班",
                    EnrollmentDate = new DateTime(2024, 9, 1)
                });
            context.SaveChanges();
        }

        private static void SeedStudentArchives(StudentsManagerDbContext context)
        {
            if (context.FamilyInfos.Any() || context.RewardRecords.Any() || context.PunishmentRecords.Any() || context.HealthRecords.Any() || context.StudentRegistrations.Any() || context.StatusChangeRecords.Any() || context.ScholarshipInfos.Any() || context.GraduationInfos.Any())
            {
                return;
            }

            context.FamilyInfos.Add(new FamilyInfo { StudentId = 1, RelationName = "张父", Relationship = "父亲", PhoneNumber = "13900139001", WorkUnit = "某公司", Address = "北京市海淀区" });
            context.RewardRecords.Add(new RewardRecord { StudentId = 1, RewardDate = DateTime.Now.AddMonths(-2), RewardType = "奖学金", RewardLevel = "一等奖", RewardReason = "成绩优异", RewardUnit = "学校" });
            context.PunishmentRecords.Add(new PunishmentRecord { StudentId = 2, PunishmentDate = DateTime.Now.AddMonths(-1), PunishmentType = "警告", PunishmentLevel = "一般", PunishmentReason = "课堂违纪", Status = "未撤销" });
            context.HealthRecords.Add(new HealthRecord { StudentId = 1, CheckDate = DateTime.Now.AddMonths(-3), Height = 175.2m, Weight = 68.5m, BloodType = "A", Vision = "5.0", HealthStatus = "正常", Remarks = "体检合格" });
            context.StudentRegistrations.Add(new StudentRegistration { StudentId = 1, StudentNo = "2024001", StudentName = "张三", RegistrationDate = DateTime.Now.Date, AcademicYear = "2024", Semester = "第一学期", Status = "已注册", Remarks = "示例数据" });
            context.StatusChangeRecords.Add(new StatusChangeRecord { StudentId = 2, StudentNo = "2024002", StudentName = "李四", ChangeDate = DateTime.Now.AddMonths(-1).Date, ChangeType = "休学", OriginalInfo = "在校", NewInfo = "休学", Reason = "个人原因", ApprovalStatus = "已批准" });
            context.ScholarshipInfos.Add(new ScholarshipInfo { StudentId = 1, StudentNo = "2024001", StudentName = "张三", AcademicYear = "2024", Semester = "第一学期", ScholarshipType = "国家励志奖学金", ScholarshipLevel = "一等奖", Amount = 8000m, AwardDate = DateTime.Now.AddMonths(-1).Date, Status = "已发放" });
            context.GraduationInfos.Add(new GraduationInfo { StudentId = 4, StudentNo = "2024004", StudentName = "赵六", GraduationDate = DateTime.Now.AddYears(3).Date, GraduationType = "本科毕业", DegreeType = "工学学士", CertificateNo = "CERT-2028-0001", DegreeNo = "DEG-2028-0001", Remarks = "示例毕业信息" });
            context.SaveChanges();
        }

        private static void SeedScores(StudentsManagerDbContext context)
        {
            if (context.Scores.Any())
            {
                return;
            }

            context.Scores.AddRange(
                new Score { StudentId = 1, StudentNo = "2024001", StudentName = "张三", AcademicYear = "2024", Semester = "第一学期", CourseNo = "C001", CourseName = "C#程序设计", Credit = 4, RegularScore = 92, ExamScore = 95, TotalScore = 93.5m, Grade = "优秀", Status = "正常", Remarks = "示例成绩" },
                new Score { StudentId = 1, StudentNo = "2024001", StudentName = "张三", AcademicYear = "2024", Semester = "第一学期", CourseNo = "C002", CourseName = "数据库原理", Credit = 3, RegularScore = 88, ExamScore = 90, TotalScore = 89m, Grade = "良好", Status = "正常", Remarks = "示例成绩" },
                new Score { StudentId = 2, StudentNo = "2024002", StudentName = "李四", AcademicYear = "2024", Semester = "第一学期", CourseNo = "C001", CourseName = "C#程序设计", Credit = 4, RegularScore = 85, ExamScore = 86, TotalScore = 85.5m, Grade = "良好", Status = "正常", Remarks = "示例成绩" });
            context.SaveChanges();
        }

        private static void SeedUsers(StudentsManagerDbContext context)
        {
            if (context.Users.Any())
            {
                return;
            }

            var adminPassword = System.Configuration.ConfigurationManager.AppSettings["AdminPassword"] ?? "Admin@123";
            var adminUsername = System.Configuration.ConfigurationManager.AppSettings["AdminUsername"] ?? "admin";

            context.Users.AddRange(
                new User
                {
                    Username = adminUsername,
                    PasswordHash = ComputeSha256(adminPassword),
                    DisplayName = "系统管理员",
                    Role = "Admin",
                    IsActive = true
                },
                new User
                {
                    Username = "teacher",
                    PasswordHash = ComputeSha256("Teacher@123"),
                    DisplayName = "普通教师",
                    Role = "User",
                    IsActive = true
                });
            context.SaveChanges();
        }

        private static string ComputeSha256(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}

using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
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
            Initialize(false);
        }

        public static void Initialize(bool forceSeed)
        {
            AppLogger.Info("开始初始化 SQLite 数据库。");

            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var databaseExists = File.Exists(DatabaseConfiguration.ResolveDatabaseFileName());
            context.Database.EnsureCreated();
            EnsureLookupOptionsTable(context);
            EnsureStudentArchiveStudentNoColumns(context);

            if (!databaseExists || forceSeed)
            {
                SeedLookupOptions(context);
                SeedDepartments(context);
                SeedMajors(context);
                SeedClasses(context);
                SeedCourses(context);
                SeedStudents(context);
                RecalculateClassStudentCounts(context);
                SeedStudentArchives(context);
                SeedScores(context);
                SeedUsers(context);
            }

            AppLogger.Info("SQLite 数据库初始化完成。");
        }

        private static void EnsureStudentArchiveStudentNoColumns(StudentsManagerDbContext context)
        {
            EnsureColumn(context, "FamilyInfos", "StudentNo", "TEXT NOT NULL DEFAULT ''");
            EnsureColumn(context, "RewardRecords", "StudentNo", "TEXT NOT NULL DEFAULT ''");
            EnsureColumn(context, "PunishmentRecords", "StudentNo", "TEXT NOT NULL DEFAULT ''");
            EnsureColumn(context, "HealthRecords", "StudentNo", "TEXT NOT NULL DEFAULT ''");

            context.Database.ExecuteSqlRaw("UPDATE FamilyInfos SET StudentNo = COALESCE((SELECT StudentNo FROM Students WHERE Students.Id = FamilyInfos.StudentId), StudentNo) WHERE StudentNo = '';");
            context.Database.ExecuteSqlRaw("UPDATE RewardRecords SET StudentNo = COALESCE((SELECT StudentNo FROM Students WHERE Students.Id = RewardRecords.StudentId), StudentNo) WHERE StudentNo = '';");
            context.Database.ExecuteSqlRaw("UPDATE PunishmentRecords SET StudentNo = COALESCE((SELECT StudentNo FROM Students WHERE Students.Id = PunishmentRecords.StudentId), StudentNo) WHERE StudentNo = '';");
            context.Database.ExecuteSqlRaw("UPDATE HealthRecords SET StudentNo = COALESCE((SELECT StudentNo FROM Students WHERE Students.Id = HealthRecords.StudentId), StudentNo) WHERE StudentNo = '';");

            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_FamilyInfos_StudentNo ON FamilyInfos (StudentNo);");
            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_RewardRecords_StudentNo ON RewardRecords (StudentNo);");
            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_PunishmentRecords_StudentNo ON PunishmentRecords (StudentNo);");
            context.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_HealthRecords_StudentNo ON HealthRecords (StudentNo);");
        }

        private static void EnsureColumn(StudentsManagerDbContext context, string tableName, string columnName, string definition)
        {
            if (!IsSafeIdentifier(tableName) || !IsSafeIdentifier(columnName))
            {
                throw new InvalidOperationException("数据库标识符不合法。");
            }

            var connection = context.Database.GetDbConnection();
            var shouldClose = connection.State != System.Data.ConnectionState.Open;
            if (shouldClose)
            {
                connection.Open();
            }

            try
            {
                using var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = "SELECT COUNT(1) FROM pragma_table_info($tableName) WHERE name = $columnName;";
                var tableParameter = checkCommand.CreateParameter();
                tableParameter.ParameterName = "$tableName";
                tableParameter.Value = tableName;
                checkCommand.Parameters.Add(tableParameter);
                var columnParameter = checkCommand.CreateParameter();
                columnParameter.ParameterName = "$columnName";
                columnParameter.Value = columnName;
                checkCommand.Parameters.Add(columnParameter);

                var exists = Convert.ToInt32(checkCommand.ExecuteScalar());
                if (exists == 0)
                {
                    using var alterCommand = connection.CreateCommand();
                    alterCommand.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {definition};";
                    alterCommand.ExecuteNonQuery();
                }
            }
            finally
            {
                if (shouldClose)
                {
                    connection.Close();
                }
            }
        }

        private static bool IsSafeIdentifier(string value)
        {
            return Regex.IsMatch(value, "^[A-Za-z_][A-Za-z0-9_]*$");
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
            var options = new[]
            {
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
                new LookupOption { Category = "ApprovalStatus", Value = "已驳回", SortOrder = 3 }
            };

            foreach (var option in options)
            {
                if (!context.LookupOptions.Any(item => item.Category == option.Category && item.Value == option.Value))
                {
                    context.LookupOptions.Add(option);
                }
            }

            context.SaveChanges();
        }

        private static void SeedDepartments(StudentsManagerDbContext context)
        {
            var departments = new[]
            {
                new Department { DepartmentNo = "CS", DepartmentName = "计算机学院", DepartmentHead = "TeacherA", PhoneNumber = "010-10000001", Remarks = "软件与计算机相关专业" },
                new Department { DepartmentNo = "EE", DepartmentName = "电子信息学院", DepartmentHead = "TeacherB", PhoneNumber = "010-10000002", Remarks = "电子信息与通信方向" },
                new Department { DepartmentNo = "ME", DepartmentName = "机械工程学院", DepartmentHead = "TeacherC", PhoneNumber = "010-10000003", Remarks = "机械设计与制造" },
                new Department { DepartmentNo = "BA", DepartmentName = "管理学院", DepartmentHead = "TeacherD", PhoneNumber = "010-10000004", Remarks = "工商管理与市场营销" },
                new Department { DepartmentNo = "AR", DepartmentName = "艺术学院", DepartmentHead = "TeacherE", PhoneNumber = "010-10000005", Remarks = "设计与艺术表达" },
                new Department { DepartmentNo = "LA", DepartmentName = "外国语学院", DepartmentHead = "TeacherF", PhoneNumber = "010-10000006", Remarks = "英语与翻译方向" },
                new Department { DepartmentNo = "MS", DepartmentName = "数学学院", DepartmentHead = "TeacherG", PhoneNumber = "010-10000007", Remarks = "数学与应用数学" },
                new Department { DepartmentNo = "PH", DepartmentName = "物理学院", DepartmentHead = "TeacherH", PhoneNumber = "010-10000008", Remarks = "基础物理与实验" },
                new Department { DepartmentNo = "CH", DepartmentName = "化学学院", DepartmentHead = "TeacherI", PhoneNumber = "010-10000009", Remarks = "应用化学与实验" },
                new Department { DepartmentNo = "BI", DepartmentName = "生命科学学院", DepartmentHead = "TeacherJ", PhoneNumber = "010-10000010", Remarks = "生物技术方向" },
                new Department { DepartmentNo = "ED", DepartmentName = "教育学院", DepartmentHead = "TeacherK", PhoneNumber = "010-10000011", Remarks = "教育学与心理学" },
                new Department { DepartmentNo = "SE", DepartmentName = "软件学院", DepartmentHead = "TeacherL", PhoneNumber = "010-10000012", Remarks = "工程实践与项目开发" }
            };

            foreach (var department in departments)
            {
                if (!context.Departments.Any(item => item.DepartmentNo == department.DepartmentNo))
                {
                    context.Departments.Add(department);
                }
            }

            context.SaveChanges();
        }

        private static void SeedMajors(StudentsManagerDbContext context)
        {
            var majors = new[]
            {
                new Major { MajorNo = "CS001", MajorName = "软件工程", DepartmentName = "计算机学院", Duration = 4, DegreeType = "工学学士", Remarks = "课程体系完善" },
                new Major { MajorNo = "CS002", MajorName = "计算机科学与技术", DepartmentName = "计算机学院", Duration = 4, DegreeType = "工学学士", Remarks = "通用计算机方向" },
                new Major { MajorNo = "EE001", MajorName = "电子信息工程", DepartmentName = "电子信息学院", Duration = 4, DegreeType = "工学学士", Remarks = "电子与通信方向" },
                new Major { MajorNo = "ME001", MajorName = "机械设计制造及其自动化", DepartmentName = "机械工程学院", Duration = 4, DegreeType = "工学学士", Remarks = "机械结构与制造" },
                new Major { MajorNo = "BA001", MajorName = "工商管理", DepartmentName = "管理学院", Duration = 4, DegreeType = "管理学学士", Remarks = "企业管理方向" },
                new Major { MajorNo = "AR001", MajorName = "视觉传达设计", DepartmentName = "艺术学院", Duration = 4, DegreeType = "艺术学学士", Remarks = "视觉设计方向" },
                new Major { MajorNo = "LA001", MajorName = "英语", DepartmentName = "外国语学院", Duration = 4, DegreeType = "文学学士", Remarks = "语言与翻译方向" },
                new Major { MajorNo = "MS001", MajorName = "数学与应用数学", DepartmentName = "数学学院", Duration = 4, DegreeType = "理学学士", Remarks = "数学建模与分析" },
                new Major { MajorNo = "PH001", MajorName = "物理学", DepartmentName = "物理学院", Duration = 4, DegreeType = "理学学士", Remarks = "基础物理方向" },
                new Major { MajorNo = "CH001", MajorName = "化学", DepartmentName = "化学学院", Duration = 4, DegreeType = "理学学士", Remarks = "化学实验方向" },
                new Major { MajorNo = "BI001", MajorName = "生物技术", DepartmentName = "生命科学学院", Duration = 4, DegreeType = "理学学士", Remarks = "生物工程方向" },
                new Major { MajorNo = "ED001", MajorName = "小学教育", DepartmentName = "教育学院", Duration = 4, DegreeType = "教育学学士", Remarks = "师范方向" }
            };

            foreach (var major in majors)
            {
                if (!context.Majors.Any(item => item.MajorNo == major.MajorNo))
                {
                    context.Majors.Add(major);
                }
            }

            context.SaveChanges();
        }

        private static void SeedClasses(StudentsManagerDbContext context)
        {
            var classes = new[]
            {
                new Class { ClassNo = "CS2024-1", ClassName = "软工2024-1班", DepartmentName = "计算机学院", MajorName = "软件工程", Grade = "2024", ClassTeacher = "teacher1", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "CS2024-2", ClassName = "计科2024-1班", DepartmentName = "计算机学院", MajorName = "计算机科学与技术", Grade = "2024", ClassTeacher = "teacher2", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "EE2024-1", ClassName = "电信2024-1班", DepartmentName = "电子信息学院", MajorName = "电子信息工程", Grade = "2024", ClassTeacher = "teacher3", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "ME2024-1", ClassName = "机械2024-1班", DepartmentName = "机械工程学院", MajorName = "机械设计制造及其自动化", Grade = "2024", ClassTeacher = "teacher4", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "BA2024-1", ClassName = "管理2024-1班", DepartmentName = "管理学院", MajorName = "工商管理", Grade = "2024", ClassTeacher = "teacher5", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "AR2024-1", ClassName = "视觉2024-1班", DepartmentName = "艺术学院", MajorName = "视觉传达设计", Grade = "2024", ClassTeacher = "teacher6", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "LA2024-1", ClassName = "英语2024-1班", DepartmentName = "外国语学院", MajorName = "英语", Grade = "2024", ClassTeacher = "teacher7", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "MS2024-1", ClassName = "数学2024-1班", DepartmentName = "数学学院", MajorName = "数学与应用数学", Grade = "2024", ClassTeacher = "teacher8", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "PH2024-1", ClassName = "物理2024-1班", DepartmentName = "物理学院", MajorName = "物理学", Grade = "2024", ClassTeacher = "teacher9", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "CH2024-1", ClassName = "化学2024-1班", DepartmentName = "化学学院", MajorName = "化学", Grade = "2024", ClassTeacher = "teacher10", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "BI2024-1", ClassName = "生科2024-1班", DepartmentName = "生命科学学院", MajorName = "生物技术", Grade = "2024", ClassTeacher = "teacher11", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "ED2024-1", ClassName = "教育2024-1班", DepartmentName = "教育学院", MajorName = "小学教育", Grade = "2024", ClassTeacher = "teacher12", StudentCount = 0, Remarks = "示例班级" }
            };

            foreach (var classInfo in classes)
            {
                if (!context.Classes.Any(item => item.ClassNo == classInfo.ClassNo))
                {
                    context.Classes.Add(classInfo);
                }
            }

            context.SaveChanges();
        }

        private static void SeedCourses(StudentsManagerDbContext context)
        {
            var courses = new[]
            {
                new Course { CourseNo = "C001", CourseName = "C#程序设计", Credit = 4, CourseType = "必修", Hours = 64, Department = "计算机学院", Remarks = "核心课程" },
                new Course { CourseNo = "C002", CourseName = "数据库原理", Credit = 3, CourseType = "必修", Hours = 48, Department = "计算机学院", Remarks = "核心课程" },
                new Course { CourseNo = "C003", CourseName = "数据结构", Credit = 4, CourseType = "必修", Hours = 64, Department = "计算机学院", Remarks = "核心课程" },
                new Course { CourseNo = "C004", CourseName = "计算机网络", Credit = 3, CourseType = "必修", Hours = 48, Department = "计算机学院", Remarks = "核心课程" },
                new Course { CourseNo = "C005", CourseName = "操作系统", Credit = 3, CourseType = "必修", Hours = 48, Department = "计算机学院", Remarks = "核心课程" },
                new Course { CourseNo = "C006", CourseName = "离散数学", Credit = 3, CourseType = "必修", Hours = 48, Department = "计算机学院", Remarks = "基础课程" },
                new Course { CourseNo = "C007", CourseName = "高等数学", Credit = 5, CourseType = "必修", Hours = 80, Department = "数学学院", Remarks = "基础课程" },
                new Course { CourseNo = "C008", CourseName = "大学英语", Credit = 4, CourseType = "必修", Hours = 64, Department = "外国语学院", Remarks = "公共基础课" },
                new Course { CourseNo = "C009", CourseName = "思想道德与法治", Credit = 2, CourseType = "必修", Hours = 32, Department = "马克思主义学院", Remarks = "公共基础课" },
                new Course { CourseNo = "C010", CourseName = "体育", Credit = 2, CourseType = "必修", Hours = 32, Department = "体育学院", Remarks = "公共基础课" },
                new Course { CourseNo = "C011", CourseName = "Python程序设计", Credit = 3, CourseType = "选修", Hours = 48, Department = "软件学院", Remarks = "编程拓展" },
                new Course { CourseNo = "C012", CourseName = "项目管理", Credit = 2, CourseType = "选修", Hours = 32, Department = "管理学院", Remarks = "实践课程" }
            };

            foreach (var course in courses)
            {
                if (!context.Courses.Any(item => item.CourseNo == course.CourseNo))
                {
                    context.Courses.Add(course);
                }
            }

            context.SaveChanges();
        }

        private static void SeedStudents(StudentsManagerDbContext context)
        {
            var departments = new[] { "计算机学院", "计算机学院", "电子信息学院", "机械工程学院", "管理学院", "艺术学院", "外国语学院", "数学学院", "物理学院", "化学学院", "生命科学学院", "教育学院" };
            var majors = new[] { "软件工程", "计算机科学与技术", "电子信息工程", "机械设计制造及其自动化", "工商管理", "视觉传达设计", "英语", "数学与应用数学", "物理学", "化学", "生物技术", "小学教育" };
            var classNames = new[] { "软工2024-1班", "计科2024-1班", "电信2024-1班", "机械2024-1班", "管理2024-1班", "视觉2024-1班", "英语2024-1班", "数学2024-1班", "物理2024-1班", "化学2024-1班", "生科2024-1班", "教育2024-1班" };
            var genders = new[] { "男", "女" };
            var statuses = new[] { "团员", "群众", "党员" };

            var students = new Student[20];
            for (int i = 0; i < 20; i++)
            {
                var num = i + 1;
                var idx = i % 12;
                students[i] = new Student
                {
                    StudentNo = $"2024{num:D4}",
                    Name = $"Student{(char)('A' + num - 1)}",
                    Gender = genders[num % 2],
                    BirthDate = new DateTime(2005, (num % 12) + 1, (num % 28) + 1),
                    IdCard = $"1101012005{num % 10}{(num % 12) + 1:D2}{(num % 28) + 1:D2}{(num % 10)}{(num % 10)}{(num % 10)}",
                    Nation = "汉族",
                    PoliticalStatus = statuses[num % 3],
                    PhoneNumber = $"13800138{num:D3}",
                    Email = $"student{(char)('a' + num - 1)}@example.com",
                    Address = "北京市海淀区",
                    Department = departments[idx],
                    Major = majors[idx],
                    Class = classNames[idx],
                    EnrollmentDate = new DateTime(2024, 9, 1)
                };
            }

            foreach (var student in students)
            {
                if (!context.Students.Any(item => item.StudentNo == student.StudentNo))
                {
                    context.Students.Add(student);
                }
            }

            context.SaveChanges();
        }

        private static void RecalculateClassStudentCounts(StudentsManagerDbContext context)
        {
            var classes = context.Classes.ToList();
            foreach (var classInfo in classes)
            {
                classInfo.StudentCount = context.Students.Count(item => item.Class == classInfo.ClassName);
            }

            context.SaveChanges();
        }

        private static void SeedStudentArchives(StudentsManagerDbContext context)
        {
            var rewardTypes = new[] { "奖学金", "三好学生", "优秀班干部", "技能竞赛奖", "社会实践先进个人", "学习标兵", "文艺活动奖", "志愿服务奖", "科研创新奖", "体育竞赛奖", "优秀寝室长", "年度优秀学生", "道德风尚奖", "创新创业奖", "优秀团员", "文明学生", "优秀志愿者", "学习进步奖", "社会实践奖", "文体活动奖" };
            var punishmentTypes = new[] { "提醒", "警告", "记过", "通报批评", "警告", "提醒", "记过", "警告", "通报批评", "提醒", "警告", "记过", "提醒", "通报批评", "警告", "提醒", "记过", "警告", "通报批评", "提醒" };
            var bloodTypes = new[] { "A", "B", "O", "AB", "A", "B", "O", "AB", "A", "B", "O", "AB", "A", "B", "O", "AB", "A", "B", "O", "AB" };
            var changeTypes = new[] { "休学", "转专业", "复学", "休学", "转班", "复学", "休学", "转专业", "复学", "休学", "转班", "复学", "休学", "转专业", "复学", "休学", "转班", "复学", "休学", "转专业" };

            for (int i = 1; i <= 20; i++)
            {
                var no = $"2024{i:D4}";
                var name = $"Student{(char)('A' + i - 1)}";

                AddFamilyInfoIfMissing(context, no, $"{name}父亲", "父亲", $"13900139{i:D3}", "某单位", "北京市海淀区");
                AddRewardRecordIfMissing(context, no, DateTime.Now.AddMonths(-(i % 12 + 1)), rewardTypes[i - 1], i % 3 == 0 ? "一等奖" : (i % 3 == 1 ? "二等奖" : "三等奖"), "表现优秀", "学校");
                AddPunishmentRecordIfMissing(context, no, DateTime.Now.AddMonths(-(i % 6 + 1)), punishmentTypes[i - 1], i % 3 == 0 ? "较重" : "一般", "违纪行为", i % 2 == 0 ? "未撤销" : "已撤销");
                AddHealthRecordIfMissing(context, no, DateTime.Now.AddMonths(-3), 165m + i, 55m + i, bloodTypes[i - 1], "5.0", "正常", "体检正常");
                AddRegistrationIfMissing(context, no, name, DateTime.Now.Date, "2024", "第一学期", "已注册", "示例数据");
                AddChangeIfMissing(context, no, name, DateTime.Now.AddMonths(-(i % 6 + 1)).Date, changeTypes[i - 1], "原状态", "新状态", "个人原因", i % 3 == 0 ? "已批准" : (i % 3 == 1 ? "待审批" : "已驳回"));
                AddScholarshipIfMissing(context, no, name, "2024", "第一学期", "校级奖学金", i % 3 == 0 ? "一等奖" : (i % 3 == 1 ? "二等奖" : "三等奖"), (decimal)(2000 + i * 300), DateTime.Now.AddMonths(-(i % 6 + 1)).Date, i % 2 == 0 ? "已发放" : "待发放");
                AddGraduationIfMissing(context, no, name, DateTime.Now.AddYears(3).Date, "本科毕业", "学士", $"CERT-2028-{i:D4}", $"DEG-2028-{i:D4}", "示例毕业信息");
            }

            context.SaveChanges();
        }

        private static void SeedScores(StudentsManagerDbContext context)
        {
            var courseNos = new[] { "C001", "C002", "C003", "C004", "C005", "C006", "C007", "C008", "C009", "C010" };
            var courseNames = new[] { "C#程序设计", "数据库原理", "数据结构", "计算机网络", "操作系统", "离散数学", "高等数学", "大学英语", "思想道德与法治", "体育" };
            var credits = new[] { 4m, 3m, 4m, 3m, 3m, 3m, 5m, 4m, 2m, 2m };

            var scores = new List<Score>();
            for (int i = 1; i <= 20; i++)
            {
                var no = $"2024{i:D4}";
                var name = $"Student{(char)('A' + i - 1)}";
                var courseIdx = (i - 1) % 10;
                var regular = 70 + (i % 20);
                var exam = 70 + ((i * 3) % 20);
                var total = (regular * 0.4m + exam * 0.6m);
                var grade = total >= 90 ? "优秀" : (total >= 80 ? "良好" : (total >= 70 ? "中等" : "及格"));

                scores.Add(new Score
                {
                    StudentNo = no,
                    StudentName = name,
                    AcademicYear = "2024",
                    Semester = "第一学期",
                    CourseNo = courseNos[courseIdx],
                    CourseName = courseNames[courseIdx],
                    Credit = credits[courseIdx],
                    RegularScore = regular,
                    ExamScore = exam,
                    TotalScore = Math.Round(total, 1),
                    Grade = grade,
                    Status = "正常",
                    Remarks = "示例成绩"
                });
            }

            foreach (var score in scores)
            {
                var student = GetStudentByNo(context, score.StudentNo);
                if (student == null)
                {
                    continue;
                }

                score.StudentId = student.Id;
                if (!context.Scores.Any(item => item.StudentNo == score.StudentNo && item.AcademicYear == score.AcademicYear && item.Semester == score.Semester && item.CourseNo == score.CourseNo))
                {
                    context.Scores.Add(score);
                }
            }

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

            var users = new List<User>
            {
                new User
                {
                    Username = adminUsername,
                    PasswordHash = ComputeSha256(adminPassword),
                    DisplayName = "系统管理员",
                    Role = "Admin",
                    IsActive = true
                }
            };

            for (int i = 1; i <= 20; i++)
            {
                users.Add(new User
                {
                    Username = $"teacher{i}",
                    PasswordHash = ComputeSha256("Teacher@123"),
                    DisplayName = $"教师{i}",
                    Role = "User",
                    IsActive = true
                });
            }

            context.Users.AddRange(users);
            context.SaveChanges();
        }

        private static Student? GetStudentByNo(StudentsManagerDbContext context, string studentNo)
        {
            return context.Students.FirstOrDefault(item => item.StudentNo == studentNo);
        }

        private static int? GetStudentIdByNo(StudentsManagerDbContext context, string studentNo)
        {
            return GetStudentByNo(context, studentNo)?.Id;
        }

        private static void AddFamilyInfoIfMissing(StudentsManagerDbContext context, string studentNo, string relationName, string relationship, string phoneNumber, string workUnit, string address)
        {
            if (context.FamilyInfos.Any(item => item.StudentNo == studentNo && item.RelationName == relationName))
            {
                return;
            }

            var studentId = GetStudentIdByNo(context, studentNo);
            if (studentId == null)
            {
                return;
            }

            context.FamilyInfos.Add(new FamilyInfo { StudentId = studentId.Value, StudentNo = studentNo, RelationName = relationName, Relationship = relationship, PhoneNumber = phoneNumber, WorkUnit = workUnit, Address = address });
        }

        private static void AddRewardRecordIfMissing(StudentsManagerDbContext context, string studentNo, DateTime rewardDate, string rewardType, string rewardLevel, string rewardReason, string rewardUnit)
        {
            if (context.RewardRecords.Any(item => item.StudentNo == studentNo && item.RewardType == rewardType && item.RewardDate == rewardDate))
            {
                return;
            }

            var studentId = GetStudentIdByNo(context, studentNo);
            if (studentId == null)
            {
                return;
            }

            context.RewardRecords.Add(new RewardRecord { StudentId = studentId.Value, StudentNo = studentNo, RewardDate = rewardDate, RewardType = rewardType, RewardLevel = rewardLevel, RewardReason = rewardReason, RewardUnit = rewardUnit });
        }

        private static void AddPunishmentRecordIfMissing(StudentsManagerDbContext context, string studentNo, DateTime punishmentDate, string punishmentType, string punishmentLevel, string punishmentReason, string status)
        {
            if (context.PunishmentRecords.Any(item => item.StudentNo == studentNo && item.PunishmentType == punishmentType && item.PunishmentDate == punishmentDate))
            {
                return;
            }

            var studentId = GetStudentIdByNo(context, studentNo);
            if (studentId == null)
            {
                return;
            }

            context.PunishmentRecords.Add(new PunishmentRecord { StudentId = studentId.Value, StudentNo = studentNo, PunishmentDate = punishmentDate, PunishmentType = punishmentType, PunishmentLevel = punishmentLevel, PunishmentReason = punishmentReason, Status = status });
        }

        private static void AddHealthRecordIfMissing(StudentsManagerDbContext context, string studentNo, DateTime checkDate, decimal height, decimal weight, string bloodType, string vision, string healthStatus, string remarks)
        {
            if (context.HealthRecords.Any(item => item.StudentNo == studentNo && item.CheckDate == checkDate))
            {
                return;
            }

            var studentId = GetStudentIdByNo(context, studentNo);
            if (studentId == null)
            {
                return;
            }

            context.HealthRecords.Add(new HealthRecord { StudentId = studentId.Value, StudentNo = studentNo, CheckDate = checkDate, Height = height, Weight = weight, BloodType = bloodType, Vision = vision, HealthStatus = healthStatus, Remarks = remarks });
        }

        private static void AddRegistrationIfMissing(StudentsManagerDbContext context, string studentNo, string studentName, DateTime registrationDate, string academicYear, string semester, string status, string remarks)
        {
            if (context.StudentRegistrations.Any(item => item.StudentNo == studentNo && item.AcademicYear == academicYear && item.Semester == semester))
            {
                return;
            }

            var studentId = GetStudentIdByNo(context, studentNo);
            if (studentId == null)
            {
                return;
            }

            context.StudentRegistrations.Add(new StudentRegistration { StudentId = studentId.Value, StudentNo = studentNo, StudentName = studentName, RegistrationDate = registrationDate, AcademicYear = academicYear, Semester = semester, Status = status, Remarks = remarks });
        }

        private static void AddChangeIfMissing(StudentsManagerDbContext context, string studentNo, string studentName, DateTime changeDate, string changeType, string originalInfo, string newInfo, string reason, string approvalStatus)
        {
            if (context.StatusChangeRecords.Any(item => item.StudentNo == studentNo && item.ChangeType == changeType && item.ChangeDate == changeDate))
            {
                return;
            }

            var studentId = GetStudentIdByNo(context, studentNo);
            if (studentId == null)
            {
                return;
            }

            context.StatusChangeRecords.Add(new StatusChangeRecord { StudentId = studentId.Value, StudentNo = studentNo, StudentName = studentName, ChangeDate = changeDate, ChangeType = changeType, OriginalInfo = originalInfo, NewInfo = newInfo, Reason = reason, ApprovalStatus = approvalStatus });
        }

        private static void AddScholarshipIfMissing(StudentsManagerDbContext context, string studentNo, string studentName, string academicYear, string semester, string scholarshipType, string scholarshipLevel, decimal amount, DateTime awardDate, string status)
        {
            if (context.ScholarshipInfos.Any(item => item.StudentNo == studentNo && item.AcademicYear == academicYear && item.Semester == semester && item.ScholarshipType == scholarshipType))
            {
                return;
            }

            var studentId = GetStudentIdByNo(context, studentNo);
            if (studentId == null)
            {
                return;
            }

            context.ScholarshipInfos.Add(new ScholarshipInfo { StudentId = studentId.Value, StudentNo = studentNo, StudentName = studentName, AcademicYear = academicYear, Semester = semester, ScholarshipType = scholarshipType, ScholarshipLevel = scholarshipLevel, Amount = amount, AwardDate = awardDate, Status = status });
        }

        private static void AddGraduationIfMissing(StudentsManagerDbContext context, string studentNo, string studentName, DateTime graduationDate, string graduationType, string degreeType, string certificateNo, string degreeNo, string remarks)
        {
            if (context.GraduationInfos.Any(item => item.StudentNo == studentNo && item.CertificateNo == certificateNo))
            {
                return;
            }

            var studentId = GetStudentIdByNo(context, studentNo);
            if (studentId == null)
            {
                return;
            }

            context.GraduationInfos.Add(new GraduationInfo { StudentId = studentId.Value, StudentNo = studentNo, StudentName = studentName, GraduationDate = graduationDate, GraduationType = graduationType, DegreeType = degreeType, CertificateNo = certificateNo, DegreeNo = degreeNo, Remarks = remarks });
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

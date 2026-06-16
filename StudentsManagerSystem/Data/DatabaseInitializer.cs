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
                new Department { DepartmentNo = "CS", DepartmentName = "计算机学院", DepartmentHead = "李主任", PhoneNumber = "010-10000001", Remarks = "软件与计算机相关专业" },
                new Department { DepartmentNo = "EE", DepartmentName = "电子信息学院", DepartmentHead = "王主任", PhoneNumber = "010-10000002", Remarks = "电子信息与通信方向" },
                new Department { DepartmentNo = "ME", DepartmentName = "机械工程学院", DepartmentHead = "赵主任", PhoneNumber = "010-10000003", Remarks = "机械设计与制造" },
                new Department { DepartmentNo = "BA", DepartmentName = "管理学院", DepartmentHead = "陈主任", PhoneNumber = "010-10000004", Remarks = "工商管理与市场营销" },
                new Department { DepartmentNo = "AR", DepartmentName = "艺术学院", DepartmentHead = "刘主任", PhoneNumber = "010-10000005", Remarks = "设计与艺术表达" },
                new Department { DepartmentNo = "LA", DepartmentName = "外国语学院", DepartmentHead = "孙主任", PhoneNumber = "010-10000006", Remarks = "英语与翻译方向" },
                new Department { DepartmentNo = "MS", DepartmentName = "数学学院", DepartmentHead = "周主任", PhoneNumber = "010-10000007", Remarks = "数学与应用数学" },
                new Department { DepartmentNo = "PH", DepartmentName = "物理学院", DepartmentHead = "吴主任", PhoneNumber = "010-10000008", Remarks = "基础物理与实验" },
                new Department { DepartmentNo = "CH", DepartmentName = "化学学院", DepartmentHead = "郑主任", PhoneNumber = "010-10000009", Remarks = "应用化学与实验" },
                new Department { DepartmentNo = "BI", DepartmentName = "生命科学学院", DepartmentHead = "冯主任", PhoneNumber = "010-10000010", Remarks = "生物技术方向" },
                new Department { DepartmentNo = "ED", DepartmentName = "教育学院", DepartmentHead = "何主任", PhoneNumber = "010-10000011", Remarks = "教育学与心理学" },
                new Department { DepartmentNo = "SE", DepartmentName = "软件学院", DepartmentHead = "黄主任", PhoneNumber = "010-10000012", Remarks = "工程实践与项目开发" }
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
                new Class { ClassNo = "CS2024-1", ClassName = "软工2024-1班", DepartmentName = "计算机学院", MajorName = "软件工程", Grade = "2024", ClassTeacher = "张老师", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "CS2024-2", ClassName = "计科2024-1班", DepartmentName = "计算机学院", MajorName = "计算机科学与技术", Grade = "2024", ClassTeacher = "李老师", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "EE2024-1", ClassName = "电信2024-1班", DepartmentName = "电子信息学院", MajorName = "电子信息工程", Grade = "2024", ClassTeacher = "王老师", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "ME2024-1", ClassName = "机械2024-1班", DepartmentName = "机械工程学院", MajorName = "机械设计制造及其自动化", Grade = "2024", ClassTeacher = "赵老师", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "BA2024-1", ClassName = "管理2024-1班", DepartmentName = "管理学院", MajorName = "工商管理", Grade = "2024", ClassTeacher = "陈老师", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "AR2024-1", ClassName = "视觉2024-1班", DepartmentName = "艺术学院", MajorName = "视觉传达设计", Grade = "2024", ClassTeacher = "刘老师", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "LA2024-1", ClassName = "英语2024-1班", DepartmentName = "外国语学院", MajorName = "英语", Grade = "2024", ClassTeacher = "孙老师", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "MS2024-1", ClassName = "数学2024-1班", DepartmentName = "数学学院", MajorName = "数学与应用数学", Grade = "2024", ClassTeacher = "周老师", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "PH2024-1", ClassName = "物理2024-1班", DepartmentName = "物理学院", MajorName = "物理学", Grade = "2024", ClassTeacher = "吴老师", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "CH2024-1", ClassName = "化学2024-1班", DepartmentName = "化学学院", MajorName = "化学", Grade = "2024", ClassTeacher = "郑老师", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "BI2024-1", ClassName = "生科2024-1班", DepartmentName = "生命科学学院", MajorName = "生物技术", Grade = "2024", ClassTeacher = "冯老师", StudentCount = 0, Remarks = "示例班级" },
                new Class { ClassNo = "ED2024-1", ClassName = "教育2024-1班", DepartmentName = "教育学院", MajorName = "小学教育", Grade = "2024", ClassTeacher = "何老师", StudentCount = 0, Remarks = "示例班级" }
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
            var students = new[]
            {
                new Student { StudentNo = "2024001", Name = "张三", Gender = "男", BirthDate = new DateTime(2005, 3, 15), IdCard = "110101200503150011", Nation = "汉族", PoliticalStatus = "团员", PhoneNumber = "13800138001", Email = "zhangsan@example.com", Address = "北京市海淀区", Department = "计算机学院", Major = "软件工程", Class = "软工2024-1班", EnrollmentDate = new DateTime(2024, 9, 1) },
                new Student { StudentNo = "2024002", Name = "李四", Gender = "女", BirthDate = new DateTime(2005, 6, 20), IdCard = "110101200506200022", Nation = "汉族", PoliticalStatus = "群众", PhoneNumber = "13800138002", Email = "lisi@example.com", Address = "北京市朝阳区", Department = "计算机学院", Major = "软件工程", Class = "软工2024-1班", EnrollmentDate = new DateTime(2024, 9, 1) },
                new Student { StudentNo = "2024003", Name = "王五", Gender = "男", BirthDate = new DateTime(2005, 8, 10), IdCard = "110101200508100033", Nation = "汉族", PoliticalStatus = "团员", PhoneNumber = "13800138003", Email = "wangwu@example.com", Address = "北京市丰台区", Department = "计算机学院", Major = "计算机科学与技术", Class = "计科2024-1班", EnrollmentDate = new DateTime(2024, 9, 1) },
                new Student { StudentNo = "2024004", Name = "赵六", Gender = "女", BirthDate = new DateTime(2005, 4, 25), IdCard = "110101200504250044", Nation = "汉族", PoliticalStatus = "党员", PhoneNumber = "13800138004", Email = "zhaoliu@example.com", Address = "北京市通州区", Department = "电子信息学院", Major = "电子信息工程", Class = "电信2024-1班", EnrollmentDate = new DateTime(2024, 9, 1) },
                new Student { StudentNo = "2024005", Name = "钱七", Gender = "男", BirthDate = new DateTime(2005, 1, 8), IdCard = "110101200501080055", Nation = "汉族", PoliticalStatus = "群众", PhoneNumber = "13800138005", Email = "qianqi@example.com", Address = "北京市东城区", Department = "机械工程学院", Major = "机械设计制造及其自动化", Class = "机械2024-1班", EnrollmentDate = new DateTime(2024, 9, 1) },
                new Student { StudentNo = "2024006", Name = "孙八", Gender = "女", BirthDate = new DateTime(2005, 11, 11), IdCard = "110101200511110066", Nation = "回族", PoliticalStatus = "团员", PhoneNumber = "13800138006", Email = "sunba@example.com", Address = "北京市西城区", Department = "管理学院", Major = "工商管理", Class = "管理2024-1班", EnrollmentDate = new DateTime(2024, 9, 1) },
                new Student { StudentNo = "2024007", Name = "周九", Gender = "男", BirthDate = new DateTime(2005, 2, 14), IdCard = "110101200502140077", Nation = "满族", PoliticalStatus = "党员", PhoneNumber = "13800138007", Email = "zhoujiu@example.com", Address = "北京市石景山区", Department = "艺术学院", Major = "视觉传达设计", Class = "视觉2024-1班", EnrollmentDate = new DateTime(2024, 9, 1) },
                new Student { StudentNo = "2024008", Name = "吴十", Gender = "女", BirthDate = new DateTime(2005, 9, 9), IdCard = "110101200509090088", Nation = "汉族", PoliticalStatus = "群众", PhoneNumber = "13800138008", Email = "wushi@example.com", Address = "北京市顺义区", Department = "外国语学院", Major = "英语", Class = "英语2024-1班", EnrollmentDate = new DateTime(2024, 9, 1) },
                new Student { StudentNo = "2024009", Name = "郑十一", Gender = "男", BirthDate = new DateTime(2005, 7, 7), IdCard = "110101200507070099", Nation = "汉族", PoliticalStatus = "团员", PhoneNumber = "13800138009", Email = "zhengshiyi@example.com", Address = "北京市昌平区", Department = "数学学院", Major = "数学与应用数学", Class = "数学2024-1班", EnrollmentDate = new DateTime(2024, 9, 1) },
                new Student { StudentNo = "2024010", Name = "王十二", Gender = "女", BirthDate = new DateTime(2005, 12, 2), IdCard = "11010120051202010X", Nation = "藏族", PoliticalStatus = "党员", PhoneNumber = "13800138010", Email = "wangshier@example.com", Address = "北京市房山区", Department = "物理学院", Major = "物理学", Class = "物理2024-1班", EnrollmentDate = new DateTime(2024, 9, 1) },
                new Student { StudentNo = "2024011", Name = "冯十三", Gender = "男", BirthDate = new DateTime(2005, 10, 18), IdCard = "110101200510180111", Nation = "汉族", PoliticalStatus = "群众", PhoneNumber = "13800138011", Email = "fengshisan@example.com", Address = "北京市门头沟区", Department = "化学学院", Major = "化学", Class = "化学2024-1班", EnrollmentDate = new DateTime(2024, 9, 1) },
                new Student { StudentNo = "2024012", Name = "陈十四", Gender = "女", BirthDate = new DateTime(2005, 5, 5), IdCard = "110101200505050122", Nation = "壮族", PoliticalStatus = "团员", PhoneNumber = "13800138012", Email = "chenshisi@example.com", Address = "北京市怀柔区", Department = "生命科学学院", Major = "生物技术", Class = "生科2024-1班", EnrollmentDate = new DateTime(2024, 9, 1) }
            };

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
            AddFamilyInfoIfMissing(context, "2024001", "张父", "父亲", "13900139001", "某公司", "北京市海淀区");
            AddFamilyInfoIfMissing(context, "2024002", "李母", "母亲", "13900139002", "某医院", "北京市朝阳区");
            AddFamilyInfoIfMissing(context, "2024003", "王父", "父亲", "13900139003", "某工厂", "北京市丰台区");
            AddFamilyInfoIfMissing(context, "2024004", "赵母", "母亲", "13900139004", "某学校", "北京市通州区");
            AddFamilyInfoIfMissing(context, "2024005", "钱父", "父亲", "13900139005", "某公司", "北京市东城区");
            AddFamilyInfoIfMissing(context, "2024006", "孙母", "母亲", "13900139006", "某银行", "北京市西城区");
            AddFamilyInfoIfMissing(context, "2024007", "周父", "父亲", "13900139007", "某出版社", "北京市石景山区");
            AddFamilyInfoIfMissing(context, "2024008", "吴母", "母亲", "13900139008", "某外企", "北京市顺义区");
            AddFamilyInfoIfMissing(context, "2024009", "郑父", "父亲", "13900139009", "某科研院所", "北京市昌平区");
            AddFamilyInfoIfMissing(context, "2024010", "王母", "母亲", "13900139010", "某中学", "北京市房山区");
            AddFamilyInfoIfMissing(context, "2024011", "冯父", "父亲", "13900139011", "某工厂", "北京市门头沟区");
            AddFamilyInfoIfMissing(context, "2024012", "陈母", "母亲", "13900139012", "某社区服务中心", "北京市怀柔区");

            AddRewardRecordIfMissing(context, "2024001", DateTime.Now.AddMonths(-2), "奖学金", "一等奖", "成绩优异", "学校");
            AddRewardRecordIfMissing(context, "2024002", DateTime.Now.AddMonths(-3), "三好学生", "校级", "综合表现突出", "学校");
            AddRewardRecordIfMissing(context, "2024003", DateTime.Now.AddMonths(-4), "优秀班干部", "校级", "班级管理表现良好", "学校");
            AddRewardRecordIfMissing(context, "2024004", DateTime.Now.AddMonths(-2), "技能竞赛奖", "一等奖", "竞赛成绩优秀", "学院");
            AddRewardRecordIfMissing(context, "2024005", DateTime.Now.AddMonths(-5), "社会实践先进个人", "校级", "参与社会实践积极", "学校");
            AddRewardRecordIfMissing(context, "2024006", DateTime.Now.AddMonths(-1), "学习标兵", "学院级", "学习成绩稳定", "学院");
            AddRewardRecordIfMissing(context, "2024007", DateTime.Now.AddMonths(-6), "文艺活动奖", "二等奖", "活动表现突出", "学校");
            AddRewardRecordIfMissing(context, "2024008", DateTime.Now.AddMonths(-2), "志愿服务奖", "校级", "志愿服务时长达标", "学校");
            AddRewardRecordIfMissing(context, "2024009", DateTime.Now.AddMonths(-3), "科研创新奖", "一等奖", "创新项目立项", "学院");
            AddRewardRecordIfMissing(context, "2024010", DateTime.Now.AddMonths(-4), "体育竞赛奖", "校级", "体育竞赛成绩良好", "学校");
            AddRewardRecordIfMissing(context, "2024011", DateTime.Now.AddMonths(-1), "优秀寝室长", "学院级", "宿舍管理优秀", "学院");
            AddRewardRecordIfMissing(context, "2024012", DateTime.Now.AddMonths(-2), "年度优秀学生", "校级", "综合评价优秀", "学校");

            AddPunishmentRecordIfMissing(context, "2024001", DateTime.Now.AddMonths(-1), "提醒", "一般", "迟到", "未撤销");
            AddPunishmentRecordIfMissing(context, "2024002", DateTime.Now.AddMonths(-2), "警告", "一般", "课堂违纪", "未撤销");
            AddPunishmentRecordIfMissing(context, "2024003", DateTime.Now.AddMonths(-3), "记过", "较重", "考试纪律问题", "已撤销");
            AddPunishmentRecordIfMissing(context, "2024004", DateTime.Now.AddMonths(-4), "通报批评", "一般", "宿舍卫生不达标", "未撤销");
            AddPunishmentRecordIfMissing(context, "2024005", DateTime.Now.AddMonths(-5), "警告", "一般", "无故缺勤", "已撤销");
            AddPunishmentRecordIfMissing(context, "2024006", DateTime.Now.AddMonths(-1), "提醒", "一般", "上课未签到", "未撤销");
            AddPunishmentRecordIfMissing(context, "2024007", DateTime.Now.AddMonths(-2), "记过", "较重", "晚归", "未撤销");
            AddPunishmentRecordIfMissing(context, "2024008", DateTime.Now.AddMonths(-3), "警告", "一般", "课堂玩手机", "已撤销");
            AddPunishmentRecordIfMissing(context, "2024009", DateTime.Now.AddMonths(-4), "通报批评", "一般", "考勤异常", "未撤销");
            AddPunishmentRecordIfMissing(context, "2024010", DateTime.Now.AddMonths(-2), "提醒", "一般", "实验迟到", "未撤销");
            AddPunishmentRecordIfMissing(context, "2024011", DateTime.Now.AddMonths(-1), "警告", "一般", "宿舍违规用电", "已撤销");
            AddPunishmentRecordIfMissing(context, "2024012", DateTime.Now.AddMonths(-5), "记过", "较重", "课堂缺席", "未撤销");

            AddHealthRecordIfMissing(context, "2024001", DateTime.Now.AddMonths(-3), 175.2m, 68.5m, "A", "5.0", "正常", "体检合格");
            AddHealthRecordIfMissing(context, "2024002", DateTime.Now.AddMonths(-3), 162.4m, 52.3m, "B", "4.9", "正常", "体检正常");
            AddHealthRecordIfMissing(context, "2024003", DateTime.Now.AddMonths(-4), 180.1m, 72.0m, "O", "5.1", "正常", "体检正常");
            AddHealthRecordIfMissing(context, "2024004", DateTime.Now.AddMonths(-2), 168.0m, 55.2m, "AB", "5.0", "正常", "视力良好");
            AddHealthRecordIfMissing(context, "2024005", DateTime.Now.AddMonths(-5), 178.6m, 74.8m, "A", "4.8", "轻度近视", "建议注意用眼");
            AddHealthRecordIfMissing(context, "2024006", DateTime.Now.AddMonths(-1), 160.3m, 49.7m, "B", "5.2", "正常", "体检正常");
            AddHealthRecordIfMissing(context, "2024007", DateTime.Now.AddMonths(-2), 172.9m, 66.4m, "O", "5.0", "正常", "体检正常");
            AddHealthRecordIfMissing(context, "2024008", DateTime.Now.AddMonths(-4), 165.5m, 54.1m, "A", "4.9", "正常", "体检正常");
            AddHealthRecordIfMissing(context, "2024009", DateTime.Now.AddMonths(-3), 181.2m, 76.3m, "AB", "5.0", "正常", "体检正常");
            AddHealthRecordIfMissing(context, "2024010", DateTime.Now.AddMonths(-2), 170.0m, 60.0m, "O", "5.1", "正常", "体检正常");
            AddHealthRecordIfMissing(context, "2024011", DateTime.Now.AddMonths(-1), 176.4m, 69.2m, "B", "4.9", "正常", "体检正常");
            AddHealthRecordIfMissing(context, "2024012", DateTime.Now.AddMonths(-5), 163.8m, 51.5m, "A", "5.0", "正常", "体检正常");

            AddRegistrationIfMissing(context, "2024001", "张三", DateTime.Now.Date, "2024", "第一学期", "已注册", "示例数据");
            AddRegistrationIfMissing(context, "2024002", "李四", DateTime.Now.Date, "2024", "第一学期", "已注册", "示例数据");
            AddRegistrationIfMissing(context, "2024003", "王五", DateTime.Now.Date, "2024", "第一学期", "已注册", "示例数据");
            AddRegistrationIfMissing(context, "2024004", "赵六", DateTime.Now.Date, "2024", "第一学期", "已注册", "示例数据");
            AddRegistrationIfMissing(context, "2024005", "钱七", DateTime.Now.Date, "2024", "第一学期", "已注册", "示例数据");
            AddRegistrationIfMissing(context, "2024006", "孙八", DateTime.Now.Date, "2024", "第一学期", "已注册", "示例数据");
            AddRegistrationIfMissing(context, "2024007", "周九", DateTime.Now.Date, "2024", "第一学期", "已注册", "示例数据");
            AddRegistrationIfMissing(context, "2024008", "吴十", DateTime.Now.Date, "2024", "第一学期", "已注册", "示例数据");
            AddRegistrationIfMissing(context, "2024009", "郑十一", DateTime.Now.Date, "2024", "第一学期", "已注册", "示例数据");
            AddRegistrationIfMissing(context, "2024010", "王十二", DateTime.Now.Date, "2024", "第一学期", "已注册", "示例数据");
            AddRegistrationIfMissing(context, "2024011", "冯十三", DateTime.Now.Date, "2024", "第一学期", "已注册", "示例数据");
            AddRegistrationIfMissing(context, "2024012", "陈十四", DateTime.Now.Date, "2024", "第一学期", "已注册", "示例数据");

            AddChangeIfMissing(context, "2024001", "张三", DateTime.Now.AddMonths(-1).Date, "休学", "在校", "休学", "个人原因", "已批准");
            AddChangeIfMissing(context, "2024002", "李四", DateTime.Now.AddMonths(-2).Date, "转专业", "软件工程", "计算机科学与技术", "兴趣调整", "已批准");
            AddChangeIfMissing(context, "2024003", "王五", DateTime.Now.AddMonths(-3).Date, "复学", "休学", "在校", "恢复学习", "已批准");
            AddChangeIfMissing(context, "2024004", "赵六", DateTime.Now.AddMonths(-1).Date, "休学", "在校", "休学", "家庭原因", "已批准");
            AddChangeIfMissing(context, "2024005", "钱七", DateTime.Now.AddMonths(-4).Date, "转班", "原班级", "新班级", "班级调整", "已驳回");
            AddChangeIfMissing(context, "2024006", "孙八", DateTime.Now.AddMonths(-2).Date, "复学", "休学", "在校", "完成手续", "已批准");
            AddChangeIfMissing(context, "2024007", "周九", DateTime.Now.AddMonths(-3).Date, "休学", "在校", "休学", "健康原因", "已批准");
            AddChangeIfMissing(context, "2024008", "吴十", DateTime.Now.AddMonths(-1).Date, "转专业", "英语", "小学教育", "职业规划", "待审批");
            AddChangeIfMissing(context, "2024009", "郑十一", DateTime.Now.AddMonths(-2).Date, "复学", "休学", "在校", "补办完成", "已批准");
            AddChangeIfMissing(context, "2024010", "王十二", DateTime.Now.AddMonths(-5).Date, "休学", "在校", "休学", "家庭原因", "已批准");
            AddChangeIfMissing(context, "2024011", "冯十三", DateTime.Now.AddMonths(-3).Date, "转班", "原班级", "新班级", "教学安排", "已驳回");
            AddChangeIfMissing(context, "2024012", "陈十四", DateTime.Now.AddMonths(-4).Date, "复学", "休学", "在校", "完成休学期", "已批准");

            AddScholarshipIfMissing(context, "2024001", "张三", "2024", "第一学期", "国家励志奖学金", "一等奖", 8000m, DateTime.Now.AddMonths(-1).Date, "已发放");
            AddScholarshipIfMissing(context, "2024002", "李四", "2024", "第一学期", "校级奖学金", "二等奖", 3000m, DateTime.Now.AddMonths(-2).Date, "已发放");
            AddScholarshipIfMissing(context, "2024003", "王五", "2024", "第一学期", "学习奖学金", "一等奖", 5000m, DateTime.Now.AddMonths(-3).Date, "已发放");
            AddScholarshipIfMissing(context, "2024004", "赵六", "2024", "第一学期", "单项奖学金", "一等奖", 2000m, DateTime.Now.AddMonths(-1).Date, "已发放");
            AddScholarshipIfMissing(context, "2024005", "钱七", "2024", "第一学期", "国家奖学金", "特等奖", 10000m, DateTime.Now.AddMonths(-4).Date, "已发放");
            AddScholarshipIfMissing(context, "2024006", "孙八", "2024", "第一学期", "助学金", "一等", 4000m, DateTime.Now.AddMonths(-2).Date, "已发放");
            AddScholarshipIfMissing(context, "2024007", "周九", "2024", "第一学期", "科研奖学金", "二等奖", 3500m, DateTime.Now.AddMonths(-3).Date, "已发放");
            AddScholarshipIfMissing(context, "2024008", "吴十", "2024", "第一学期", "综合奖学金", "二等奖", 2500m, DateTime.Now.AddMonths(-1).Date, "待发放");
            AddScholarshipIfMissing(context, "2024009", "郑十一", "2024", "第一学期", "国家励志奖学金", "一等奖", 8000m, DateTime.Now.AddMonths(-2).Date, "已发放");
            AddScholarshipIfMissing(context, "2024010", "王十二", "2024", "第一学期", "校级奖学金", "三等奖", 1500m, DateTime.Now.AddMonths(-5).Date, "已发放");
            AddScholarshipIfMissing(context, "2024011", "冯十三", "2024", "第一学期", "学习奖学金", "二等奖", 3000m, DateTime.Now.AddMonths(-3).Date, "已发放");
            AddScholarshipIfMissing(context, "2024012", "陈十四", "2024", "第一学期", "国家奖学金", "一等奖", 10000m, DateTime.Now.AddMonths(-4).Date, "待发放");

            AddGraduationIfMissing(context, "2024001", "张三", DateTime.Now.AddYears(3).Date, "本科毕业", "工学学士", "CERT-2028-0001", "DEG-2028-0001", "示例毕业信息");
            AddGraduationIfMissing(context, "2024002", "李四", DateTime.Now.AddYears(3).Date, "本科毕业", "工学学士", "CERT-2028-0002", "DEG-2028-0002", "示例毕业信息");
            AddGraduationIfMissing(context, "2024003", "王五", DateTime.Now.AddYears(3).Date, "本科毕业", "工学学士", "CERT-2028-0003", "DEG-2028-0003", "示例毕业信息");
            AddGraduationIfMissing(context, "2024004", "赵六", DateTime.Now.AddYears(3).Date, "本科毕业", "工学学士", "CERT-2028-0004", "DEG-2028-0004", "示例毕业信息");
            AddGraduationIfMissing(context, "2024005", "钱七", DateTime.Now.AddYears(3).Date, "本科毕业", "工学学士", "CERT-2028-0005", "DEG-2028-0005", "示例毕业信息");
            AddGraduationIfMissing(context, "2024006", "孙八", DateTime.Now.AddYears(3).Date, "本科毕业", "管理学学士", "CERT-2028-0006", "DEG-2028-0006", "示例毕业信息");
            AddGraduationIfMissing(context, "2024007", "周九", DateTime.Now.AddYears(3).Date, "本科毕业", "艺术学学士", "CERT-2028-0007", "DEG-2028-0007", "示例毕业信息");
            AddGraduationIfMissing(context, "2024008", "吴十", DateTime.Now.AddYears(3).Date, "本科毕业", "文学学士", "CERT-2028-0008", "DEG-2028-0008", "示例毕业信息");
            AddGraduationIfMissing(context, "2024009", "郑十一", DateTime.Now.AddYears(3).Date, "本科毕业", "理学学士", "CERT-2028-0009", "DEG-2028-0009", "示例毕业信息");
            AddGraduationIfMissing(context, "2024010", "王十二", DateTime.Now.AddYears(3).Date, "本科毕业", "理学学士", "CERT-2028-0010", "DEG-2028-0010", "示例毕业信息");
            AddGraduationIfMissing(context, "2024011", "冯十三", DateTime.Now.AddYears(3).Date, "本科毕业", "理学学士", "CERT-2028-0011", "DEG-2028-0011", "示例毕业信息");
            AddGraduationIfMissing(context, "2024012", "陈十四", DateTime.Now.AddYears(3).Date, "本科毕业", "理学学士", "CERT-2028-0012", "DEG-2028-0012", "示例毕业信息");

            context.SaveChanges();
        }

        private static void SeedScores(StudentsManagerDbContext context)
        {
            var scores = new[]
            {
                new Score { StudentNo = "2024001", StudentName = "张三", AcademicYear = "2024", Semester = "第一学期", CourseNo = "C001", CourseName = "C#程序设计", Credit = 4, RegularScore = 92, ExamScore = 95, TotalScore = 93.5m, Grade = "优秀", Status = "正常", Remarks = "示例成绩" },
                new Score { StudentNo = "2024002", StudentName = "李四", AcademicYear = "2024", Semester = "第一学期", CourseNo = "C002", CourseName = "数据库原理", Credit = 3, RegularScore = 88, ExamScore = 90, TotalScore = 89m, Grade = "良好", Status = "正常", Remarks = "示例成绩" },
                new Score { StudentNo = "2024003", StudentName = "王五", AcademicYear = "2024", Semester = "第一学期", CourseNo = "C003", CourseName = "数据结构", Credit = 4, RegularScore = 84, ExamScore = 87, TotalScore = 85.5m, Grade = "良好", Status = "正常", Remarks = "示例成绩" },
                new Score { StudentNo = "2024004", StudentName = "赵六", AcademicYear = "2024", Semester = "第一学期", CourseNo = "C004", CourseName = "计算机网络", Credit = 3, RegularScore = 90, ExamScore = 92, TotalScore = 91m, Grade = "优秀", Status = "正常", Remarks = "示例成绩" },
                new Score { StudentNo = "2024005", StudentName = "钱七", AcademicYear = "2024", Semester = "第一学期", CourseNo = "C005", CourseName = "操作系统", Credit = 3, RegularScore = 78, ExamScore = 82, TotalScore = 80m, Grade = "良好", Status = "正常", Remarks = "示例成绩" },
                new Score { StudentNo = "2024006", StudentName = "孙八", AcademicYear = "2024", Semester = "第一学期", CourseNo = "C006", CourseName = "离散数学", Credit = 3, RegularScore = 81, ExamScore = 83, TotalScore = 82m, Grade = "良好", Status = "正常", Remarks = "示例成绩" },
                new Score { StudentNo = "2024007", StudentName = "周九", AcademicYear = "2024", Semester = "第一学期", CourseNo = "C007", CourseName = "高等数学", Credit = 5, RegularScore = 75, ExamScore = 79, TotalScore = 77m, Grade = "中等", Status = "正常", Remarks = "示例成绩" },
                new Score { StudentNo = "2024008", StudentName = "吴十", AcademicYear = "2024", Semester = "第一学期", CourseNo = "C008", CourseName = "大学英语", Credit = 4, RegularScore = 86, ExamScore = 88, TotalScore = 87m, Grade = "良好", Status = "正常", Remarks = "示例成绩" },
                new Score { StudentNo = "2024009", StudentName = "郑十一", AcademicYear = "2024", Semester = "第一学期", CourseNo = "C009", CourseName = "思想道德与法治", Credit = 2, RegularScore = 91, ExamScore = 94, TotalScore = 92.5m, Grade = "优秀", Status = "正常", Remarks = "示例成绩" },
                new Score { StudentNo = "2024010", StudentName = "王十二", AcademicYear = "2024", Semester = "第一学期", CourseNo = "C010", CourseName = "体育", Credit = 2, RegularScore = 88, ExamScore = 90, TotalScore = 89m, Grade = "良好", Status = "正常", Remarks = "示例成绩" },
                new Score { StudentNo = "2024011", StudentName = "冯十三", AcademicYear = "2024", Semester = "第一学期", CourseNo = "C011", CourseName = "Python程序设计", Credit = 3, RegularScore = 83, ExamScore = 85, TotalScore = 84m, Grade = "良好", Status = "正常", Remarks = "示例成绩" },
                new Score { StudentNo = "2024012", StudentName = "陈十四", AcademicYear = "2024", Semester = "第一学期", CourseNo = "C012", CourseName = "项目管理", Credit = 2, RegularScore = 79, ExamScore = 81, TotalScore = 80m, Grade = "良好", Status = "正常", Remarks = "示例成绩" }
            };

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

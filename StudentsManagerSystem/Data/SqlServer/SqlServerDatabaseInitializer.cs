using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace StudentsManagerSystem.Data.SqlServer
{
    internal static class SqlServerDatabaseInitializer
    {
        private const string DatabaseName = "StudentsManagerSystemDb";
        private static string logPath = Path.Combine(Path.GetTempPath(), "StudentsManagerSystem_Init.log");

        public static void Initialize()
        {
            try
            {
                Log("========== 数据库初始化开始 ==========");
                Log($"连接字符串: {SqlServerConnectionFactory.ConnectionString}");
                
                Log("1. 检查/创建数据库...");
                EnsureDatabaseExists();
                Log("   ✓ 数据库检查完成");
                
                Log("2. 检查/创建表...");
                EnsureTablesExist();
                Log("   ✓ 表创建完成");
                
                Log("3. 种子数据 - 院系...");
                SeedReferenceData();
                Log("   ✓ 参考数据完成");

                Log("4. 种子数据 - 课程...");
                SeedCourses();
                Log("   ✓ 课程数据完成");
                
                Log("5. 种子数据 - 学生...");
                SeedStudents();
                Log("   ✓ 学生数据完成");

                Log("6. 种子数据 - 成绩...");
                SeedScores();
                Log("   ✓ 成绩数据完成");

                Log("7. 种子数据 - 学籍/奖助/毕业...");
                SeedStudentStatusData();
                Log("   ✓ 学籍/奖助/毕业数据完成");
                
                Log("========== 数据库初始化成功! ==========");
            }
            catch (Exception ex)
            {
                Log($"========== 数据库初始化失败! ==========");
                Log($"错误类型: {ex.GetType().Name}");
                Log($"错误信息: {ex.Message}");
                Log($"堆栈跟踪: {ex.StackTrace}");
                Log($"连接字符串: {SqlServerConnectionFactory.ConnectionString}");
                Log("=======================================");
                throw;
            }
        }

        private static void Log(string message)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                File.AppendAllText(logPath, $"[{timestamp}] {message}\n");
                Console.WriteLine(message); // 还是试试控制台
            }
            catch { }
        }

        private static void EnsureDatabaseExists()
        {
            using var connection = SqlServerConnectionFactory.CreateMasterConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = $"IF DB_ID(N'{DatabaseName}') IS NULL CREATE DATABASE [{DatabaseName}];";
            command.ExecuteNonQuery();
        }

        private static void EnsureTablesExist()
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            var commands = new[]
            {
                @"IF OBJECT_ID(N'dbo.Students', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Students
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StudentNo NVARCHAR(50) NOT NULL UNIQUE,
        Name NVARCHAR(100) NOT NULL,
        Gender NVARCHAR(20) NOT NULL,
        BirthDate DATE NULL,
        IdCard NVARCHAR(50) NOT NULL,
        Nation NVARCHAR(50) NOT NULL,
        PoliticalStatus NVARCHAR(50) NOT NULL,
        PhoneNumber NVARCHAR(50) NOT NULL,
        Email NVARCHAR(100) NULL,
        Address NVARCHAR(200) NULL,
        Department NVARCHAR(100) NOT NULL,
        Major NVARCHAR(100) NOT NULL,
        [Class] NVARCHAR(100) NOT NULL,
        EnrollmentDate DATE NULL,
        Photo NVARCHAR(500) NULL
    );
END;",
                @"IF OBJECT_ID(N'dbo.FamilyInfos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FamilyInfos
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StudentId INT NOT NULL,
        RelationName NVARCHAR(100) NOT NULL,
        Relationship NVARCHAR(50) NOT NULL,
        PhoneNumber NVARCHAR(50) NULL,
        WorkUnit NVARCHAR(200) NULL,
        Address NVARCHAR(200) NULL
    );
END;",
                @"IF OBJECT_ID(N'dbo.RewardRecords', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RewardRecords
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StudentId INT NOT NULL,
        RewardDate DATE NULL,
        RewardType NVARCHAR(100) NOT NULL,
        RewardLevel NVARCHAR(100) NOT NULL,
        RewardReason NVARCHAR(200) NULL,
        RewardUnit NVARCHAR(200) NULL
    );
END;",
                @"IF OBJECT_ID(N'dbo.PunishmentRecords', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PunishmentRecords
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StudentId INT NOT NULL,
        PunishmentDate DATE NULL,
        PunishmentType NVARCHAR(100) NOT NULL,
        PunishmentLevel NVARCHAR(100) NOT NULL,
        PunishmentReason NVARCHAR(200) NULL,
        CancelDate DATE NULL,
        Status NVARCHAR(50) NOT NULL
    );
END;",
                @"IF OBJECT_ID(N'dbo.HealthRecords', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HealthRecords
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StudentId INT NOT NULL,
        CheckDate DATE NULL,
        Height DECIMAL(6,2) NOT NULL,
        Weight DECIMAL(6,2) NOT NULL,
        BloodType NVARCHAR(20) NOT NULL,
        Vision NVARCHAR(20) NOT NULL,
        HealthStatus NVARCHAR(100) NOT NULL,
        Remarks NVARCHAR(200) NULL
    );
END;",
                @"IF OBJECT_ID(N'dbo.StudentRegistrations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.StudentRegistrations
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StudentId INT NOT NULL,
        StudentNo NVARCHAR(50) NOT NULL,
        StudentName NVARCHAR(100) NOT NULL,
        RegistrationDate DATE NULL,
        AcademicYear NVARCHAR(50) NOT NULL,
        Semester NVARCHAR(50) NOT NULL,
        Status NVARCHAR(50) NOT NULL,
        Remarks NVARCHAR(200) NULL
    );
END;",
                @"IF OBJECT_ID(N'dbo.StatusChangeRecords', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.StatusChangeRecords
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StudentId INT NOT NULL,
        StudentNo NVARCHAR(50) NOT NULL,
        StudentName NVARCHAR(100) NOT NULL,
        ChangeDate DATE NULL,
        ChangeType NVARCHAR(100) NOT NULL,
        OriginalInfo NVARCHAR(200) NULL,
        NewInfo NVARCHAR(200) NULL,
        Reason NVARCHAR(200) NULL,
        ApprovalStatus NVARCHAR(50) NOT NULL
    );
END;",
                @"IF OBJECT_ID(N'dbo.ScholarshipInfos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ScholarshipInfos
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StudentId INT NOT NULL,
        StudentNo NVARCHAR(50) NOT NULL,
        StudentName NVARCHAR(100) NOT NULL,
        AcademicYear NVARCHAR(50) NOT NULL,
        Semester NVARCHAR(50) NOT NULL,
        ScholarshipType NVARCHAR(100) NOT NULL,
        ScholarshipLevel NVARCHAR(100) NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        AwardDate DATE NULL,
        Status NVARCHAR(50) NOT NULL
    );
END;",
                @"IF OBJECT_ID(N'dbo.GraduationInfos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.GraduationInfos
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StudentId INT NOT NULL,
        StudentNo NVARCHAR(50) NOT NULL,
        StudentName NVARCHAR(100) NOT NULL,
        GraduationDate DATE NULL,
        GraduationType NVARCHAR(100) NOT NULL,
        DegreeType NVARCHAR(100) NOT NULL,
        CertificateNo NVARCHAR(100) NULL,
        DegreeNo NVARCHAR(100) NULL,
        Remarks NVARCHAR(200) NULL
    );
END;",
                @"IF OBJECT_ID(N'dbo.Courses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Courses
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CourseNo NVARCHAR(50) NOT NULL UNIQUE,
        CourseName NVARCHAR(100) NOT NULL,
        Credit DECIMAL(6,2) NOT NULL,
        CourseType NVARCHAR(50) NOT NULL,
        Hours INT NOT NULL,
        Department NVARCHAR(100) NOT NULL,
        Remarks NVARCHAR(200) NULL
    );
END;",
                @"IF OBJECT_ID(N'dbo.Scores', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Scores
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StudentId INT NOT NULL,
        StudentNo NVARCHAR(50) NOT NULL,
        StudentName NVARCHAR(100) NOT NULL,
        AcademicYear NVARCHAR(50) NOT NULL,
        Semester NVARCHAR(50) NOT NULL,
        CourseNo NVARCHAR(50) NOT NULL,
        CourseName NVARCHAR(100) NOT NULL,
        Credit DECIMAL(6,2) NOT NULL,
        RegularScore DECIMAL(6,2) NULL,
        ExamScore DECIMAL(6,2) NULL,
        TotalScore DECIMAL(6,2) NULL,
        Grade NVARCHAR(20) NOT NULL,
        Status NVARCHAR(50) NOT NULL,
        Remarks NVARCHAR(200) NULL
    );
END;",
                @"IF OBJECT_ID(N'dbo.Departments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Departments
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        DepartmentNo NVARCHAR(50) NOT NULL UNIQUE,
        DepartmentName NVARCHAR(100) NOT NULL UNIQUE,
        DepartmentHead NVARCHAR(100) NULL,
        PhoneNumber NVARCHAR(50) NULL,
        Remarks NVARCHAR(200) NULL
    );
END;",
                @"IF OBJECT_ID(N'dbo.Majors', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Majors
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        MajorNo NVARCHAR(50) NOT NULL UNIQUE,
        MajorName NVARCHAR(100) NOT NULL UNIQUE,
        DepartmentName NVARCHAR(100) NOT NULL,
        Duration INT NOT NULL,
        DegreeType NVARCHAR(100) NOT NULL,
        Remarks NVARCHAR(200) NULL
    );
END;",
                @"IF OBJECT_ID(N'dbo.Classes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Classes
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ClassNo NVARCHAR(50) NOT NULL UNIQUE,
        ClassName NVARCHAR(100) NOT NULL UNIQUE,
        DepartmentName NVARCHAR(100) NOT NULL,
        MajorName NVARCHAR(100) NOT NULL,
        Grade NVARCHAR(50) NOT NULL,
        ClassTeacher NVARCHAR(100) NULL,
        StudentCount INT NOT NULL,
        Remarks NVARCHAR(200) NULL
    );
END;"
            };

            foreach (var sql in commands)
            {
                using var command = connection.CreateCommand();
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        private static void SeedReferenceData()
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            if (CountRows(connection, "dbo.Departments") == 0)
            {
                ExecuteNonQuery(connection, @"
INSERT INTO dbo.Departments (DepartmentNo, DepartmentName, DepartmentHead, PhoneNumber, Remarks)
VALUES
    (N'D001', N'计算机学院', N'王教授', N'010-12345678', N''),
    (N'D002', N'电子信息学院', N'李教授', N'010-12345679', N''),
    (N'D003', N'机械工程学院', N'张教授', N'010-12345680', N'');");
            }

            if (CountRows(connection, "dbo.Majors") == 0)
            {
                ExecuteNonQuery(connection, @"
INSERT INTO dbo.Majors (MajorNo, MajorName, DepartmentName, Duration, DegreeType, Remarks)
VALUES
    (N'M001', N'软件工程', N'计算机学院', 4, N'工学学士', N''),
    (N'M002', N'计算机科学与技术', N'计算机学院', 4, N'工学学士', N''),
    (N'M003', N'网络工程', N'计算机学院', 4, N'工学学士', N'');");
            }

            if (CountRows(connection, "dbo.Classes") == 0)
            {
                ExecuteNonQuery(connection, @"
INSERT INTO dbo.Classes (ClassNo, ClassName, DepartmentName, MajorName, Grade, ClassTeacher, StudentCount, Remarks)
VALUES
    (N'C001', N'软工2024-1班', N'计算机学院', N'软件工程', N'2024', N'赵老师', 45, N''),
    (N'C002', N'软工2024-2班', N'计算机学院', N'软件工程', N'2024', N'钱老师', 48, N'');");
            }
        }

        private static void SeedCourses()
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            if (CountRows(connection, "dbo.Courses") > 0)
            {
                return;
            }

            ExecuteNonQuery(connection, @"
INSERT INTO dbo.Courses (CourseNo, CourseName, Credit, CourseType, Hours, Department, Remarks)
VALUES
    (N'CS101', N'程序设计基础', 4, N'必修', 64, N'计算机学院', N''),
    (N'MA101', N'高等数学', 5, N'必修', 80, N'计算机学院', N''),
    (N'CS102', N'数据结构', 4, N'必修', 64, N'计算机学院', N'');");
        }

        private static void SeedStudents()
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            if (CountRows(connection, "dbo.Students") > 0)
            {
                return;
            }

            ExecuteNonQuery(connection, @"
INSERT INTO dbo.Students
    (StudentNo, Name, Gender, BirthDate, IdCard, Nation, PoliticalStatus, PhoneNumber, Email, Address, Department, Major, [Class], EnrollmentDate, Photo)
VALUES
    (N'2024001', N'张三', N'男', '2005-03-15', N'110101200503151234', N'汉族', N'团员', N'13800138001', N'zhangsan@example.com', N'北京市朝阳区某某街道某某小区', N'计算机学院', N'软件工程', N'软工2024-1班', '2024-09-01', N''),
    (N'2024002', N'李四', N'女', '2005-06-20', N'110101200506201234', N'汉族', N'群众', N'13800138002', N'lisi@example.com', N'北京市海淀区某某街道某某小区', N'计算机学院', N'软件工程', N'软工2024-1班', '2024-09-01', N'');");
        }

        private static void SeedScores()
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            if (CountRows(connection, "dbo.Scores") > 0)
            {
                return;
            }

            ExecuteNonQuery(connection, @"
INSERT INTO dbo.Scores
    (StudentId, StudentNo, StudentName, AcademicYear, Semester, CourseNo, CourseName, Credit, RegularScore, ExamScore, TotalScore, Grade, Status, Remarks)
VALUES
    (1, N'2024001', N'张三', N'2024-2025', N'第一学期', N'CS101', N'程序设计基础', 4, 85, 90, 87.5, N'良好', N'正常', N''),
    (1, N'2024001', N'张三', N'2024-2025', N'第一学期', N'MA101', N'高等数学', 5, 80, 85, 82.5, N'良好', N'正常', N''),
    (2, N'2024002', N'李四', N'2024-2025', N'第一学期', N'CS101', N'程序设计基础', 4, 90, 92, 91, N'优秀', N'正常', N'');");
        }

        private static void SeedStudentStatusData()
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            if (CountRows(connection, "dbo.StudentRegistrations") == 0)
            {
                ExecuteNonQuery(connection, @"
INSERT INTO dbo.StudentRegistrations
    (StudentId, StudentNo, StudentName, RegistrationDate, AcademicYear, Semester, Status, Remarks)
VALUES
    (1, N'2024001', N'张三', '2024-09-01', N'2024-2025', N'第一学期', N'正常', N''),
    (2, N'2024002', N'李四', '2024-09-01', N'2024-2025', N'第一学期', N'正常', N'');");
            }

            if (CountRows(connection, "dbo.StatusChangeRecords") == 0)
            {
                ExecuteNonQuery(connection, @"
INSERT INTO dbo.StatusChangeRecords
    (StudentId, StudentNo, StudentName, ChangeDate, ChangeType, OriginalInfo, NewInfo, Reason, ApprovalStatus)
VALUES
    (1, N'2024001', N'张三', '2024-10-01', N'转专业', N'计算机科学与技术', N'软件工程', N'个人意愿', N'已批准');");
            }

            if (CountRows(connection, "dbo.ScholarshipInfos") == 0)
            {
                ExecuteNonQuery(connection, @"
INSERT INTO dbo.ScholarshipInfos
    (StudentId, StudentNo, StudentName, AcademicYear, Semester, ScholarshipType, ScholarshipLevel, Amount, AwardDate, Status)
VALUES
    (1, N'2024001', N'张三', N'2024-2025', N'第一学期', N'国家奖学金', N'一等', 8000, '2024-12-20', N'已发放');");
            }

            if (CountRows(connection, "dbo.GraduationInfos") == 0)
            {
                ExecuteNonQuery(connection, @"
INSERT INTO dbo.GraduationInfos
    (StudentId, StudentNo, StudentName, GraduationDate, GraduationType, DegreeType, CertificateNo, DegreeNo, Remarks)
VALUES
    (2, N'2024002', N'李四', '2028-06-30', N'正常毕业', N'工学学士', N'GR2028001', N'DEG2028001', N'');");
            }
        }

        private static int CountRows(SqlConnection connection, string tableName)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(1) FROM {tableName};";
            return Convert.ToInt32(command.ExecuteScalar());
        }

        private static void ExecuteNonQuery(SqlConnection connection, string sql)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }
    }
}
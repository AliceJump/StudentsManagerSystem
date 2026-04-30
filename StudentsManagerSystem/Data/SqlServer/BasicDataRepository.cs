using Microsoft.Data.SqlClient;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Data.SqlServer
{
    internal sealed class BasicDataRepository
    {
        public List<Department> GetDepartments()
        {
            var items = new List<Department>();

            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
SELECT Id, DepartmentNo, DepartmentName, DepartmentHead, PhoneNumber, Remarks
FROM dbo.Departments
ORDER BY DepartmentNo;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new Department
                {
                    Id = reader.GetInt32(0),
                    DepartmentNo = reader.GetString(1),
                    DepartmentName = reader.GetString(2),
                    DepartmentHead = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    PhoneNumber = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    Remarks = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
                });
            }

            return items;
        }

        public List<Major> GetMajors()
        {
            var items = new List<Major>();

            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
SELECT Id, MajorNo, MajorName, DepartmentName, Duration, DegreeType, Remarks
FROM dbo.Majors
ORDER BY MajorNo;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new Major
                {
                    Id = reader.GetInt32(0),
                    MajorNo = reader.GetString(1),
                    MajorName = reader.GetString(2),
                    DepartmentName = reader.GetString(3),
                    Duration = reader.GetInt32(4),
                    DegreeType = reader.GetString(5),
                    Remarks = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                });
            }

            return items;
        }

        public List<Class> GetClasses()
        {
            var items = new List<Class>();

            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
SELECT Id, ClassNo, ClassName, DepartmentName, MajorName, Grade, ClassTeacher, StudentCount, Remarks
FROM dbo.Classes
ORDER BY ClassNo;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new Class
                {
                    Id = reader.GetInt32(0),
                    ClassNo = reader.GetString(1),
                    ClassName = reader.GetString(2),
                    DepartmentName = reader.GetString(3),
                    MajorName = reader.GetString(4),
                    Grade = reader.GetString(5),
                    ClassTeacher = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    StudentCount = reader.GetInt32(7),
                    Remarks = reader.IsDBNull(8) ? string.Empty : reader.GetString(8)
                });
            }

            return items;
        }

        public int AddDepartment(Department department)
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
INSERT INTO dbo.Departments (DepartmentNo, DepartmentName, DepartmentHead, PhoneNumber, Remarks)
OUTPUT INSERTED.Id
VALUES (@DepartmentNo, @DepartmentName, @DepartmentHead, @PhoneNumber, @Remarks);";

            AddDepartmentParameters(command, department);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public void UpdateDepartment(Department department)
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
UPDATE dbo.Departments
SET DepartmentNo = @DepartmentNo,
    DepartmentName = @DepartmentName,
    DepartmentHead = @DepartmentHead,
    PhoneNumber = @PhoneNumber,
    Remarks = @Remarks
WHERE Id = @Id;";

            AddDepartmentParameters(command, department);
            command.Parameters.AddWithValue("@Id", department.Id);
            command.ExecuteNonQuery();
        }

        public void DeleteDepartment(int id)
        {
            ExecuteDelete("dbo.Departments", id);
        }

        public int AddMajor(Major major)
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
INSERT INTO dbo.Majors (MajorNo, MajorName, DepartmentName, Duration, DegreeType, Remarks)
OUTPUT INSERTED.Id
VALUES (@MajorNo, @MajorName, @DepartmentName, @Duration, @DegreeType, @Remarks);";

            AddMajorParameters(command, major);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public void UpdateMajor(Major major)
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
UPDATE dbo.Majors
SET MajorNo = @MajorNo,
    MajorName = @MajorName,
    DepartmentName = @DepartmentName,
    Duration = @Duration,
    DegreeType = @DegreeType,
    Remarks = @Remarks
WHERE Id = @Id;";

            AddMajorParameters(command, major);
            command.Parameters.AddWithValue("@Id", major.Id);
            command.ExecuteNonQuery();
        }

        public void DeleteMajor(int id)
        {
            ExecuteDelete("dbo.Majors", id);
        }

        public int AddClass(Class classInfo)
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
INSERT INTO dbo.Classes (ClassNo, ClassName, DepartmentName, MajorName, Grade, ClassTeacher, StudentCount, Remarks)
OUTPUT INSERTED.Id
VALUES (@ClassNo, @ClassName, @DepartmentName, @MajorName, @Grade, @ClassTeacher, @StudentCount, @Remarks);";

            AddClassParameters(command, classInfo);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public void UpdateClass(Class classInfo)
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
UPDATE dbo.Classes
SET ClassNo = @ClassNo,
    ClassName = @ClassName,
    DepartmentName = @DepartmentName,
    MajorName = @MajorName,
    Grade = @Grade,
    ClassTeacher = @ClassTeacher,
    StudentCount = @StudentCount,
    Remarks = @Remarks
WHERE Id = @Id;";

            AddClassParameters(command, classInfo);
            command.Parameters.AddWithValue("@Id", classInfo.Id);
            command.ExecuteNonQuery();
        }

        public void DeleteClass(int id)
        {
            ExecuteDelete("dbo.Classes", id);
        }

        private static void ExecuteDelete(string tableName, int id)
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = $"DELETE FROM {tableName} WHERE Id = @Id;";
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }

        private static void AddDepartmentParameters(SqlCommand command, Department department)
        {
            command.Parameters.AddWithValue("@DepartmentNo", department.DepartmentNo);
            command.Parameters.AddWithValue("@DepartmentName", department.DepartmentName);
            command.Parameters.AddWithValue("@DepartmentHead", string.IsNullOrWhiteSpace(department.DepartmentHead) ? DBNull.Value : department.DepartmentHead);
            command.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrWhiteSpace(department.PhoneNumber) ? DBNull.Value : department.PhoneNumber);
            command.Parameters.AddWithValue("@Remarks", string.IsNullOrWhiteSpace(department.Remarks) ? DBNull.Value : department.Remarks);
        }

        private static void AddMajorParameters(SqlCommand command, Major major)
        {
            command.Parameters.AddWithValue("@MajorNo", major.MajorNo);
            command.Parameters.AddWithValue("@MajorName", major.MajorName);
            command.Parameters.AddWithValue("@DepartmentName", major.DepartmentName);
            command.Parameters.AddWithValue("@Duration", major.Duration);
            command.Parameters.AddWithValue("@DegreeType", major.DegreeType);
            command.Parameters.AddWithValue("@Remarks", string.IsNullOrWhiteSpace(major.Remarks) ? DBNull.Value : major.Remarks);
        }

        private static void AddClassParameters(SqlCommand command, Class classInfo)
        {
            command.Parameters.AddWithValue("@ClassNo", classInfo.ClassNo);
            command.Parameters.AddWithValue("@ClassName", classInfo.ClassName);
            command.Parameters.AddWithValue("@DepartmentName", classInfo.DepartmentName);
            command.Parameters.AddWithValue("@MajorName", classInfo.MajorName);
            command.Parameters.AddWithValue("@Grade", classInfo.Grade);
            command.Parameters.AddWithValue("@ClassTeacher", string.IsNullOrWhiteSpace(classInfo.ClassTeacher) ? DBNull.Value : classInfo.ClassTeacher);
            command.Parameters.AddWithValue("@StudentCount", classInfo.StudentCount);
            command.Parameters.AddWithValue("@Remarks", string.IsNullOrWhiteSpace(classInfo.Remarks) ? DBNull.Value : classInfo.Remarks);
        }
    }
}
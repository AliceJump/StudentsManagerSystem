using Microsoft.Data.SqlClient;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Data.SqlServer
{
    internal sealed class StudentRepository
    {
        public List<Student> GetAll()
        {
            var students = new List<Student>();

            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
SELECT Id, StudentNo, Name, Gender, BirthDate, IdCard, Nation, PoliticalStatus, PhoneNumber,
       Email, Address, Department, Major, [Class], EnrollmentDate, Photo
FROM dbo.Students
ORDER BY StudentNo;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                students.Add(MapStudent(reader));
            }

            return students;
        }

        public List<Student> Search(string keyword)
        {
            keyword = keyword.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return GetAll();
            }

            var students = new List<Student>();

            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
SELECT Id, StudentNo, Name, Gender, BirthDate, IdCard, Nation, PoliticalStatus, PhoneNumber,
       Email, Address, Department, Major, [Class], EnrollmentDate, Photo
FROM dbo.Students
WHERE StudentNo LIKE @Keyword
   OR Name LIKE @Keyword
   OR Department LIKE @Keyword
   OR Major LIKE @Keyword
   OR [Class] LIKE @Keyword
ORDER BY StudentNo;";
            command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                students.Add(MapStudent(reader));
            }

            return students;
        }

        public Student? GetById(int id)
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
SELECT Id, StudentNo, Name, Gender, BirthDate, IdCard, Nation, PoliticalStatus, PhoneNumber,
       Email, Address, Department, Major, [Class], EnrollmentDate, Photo
FROM dbo.Students
WHERE Id = @Id;";
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            return reader.Read() ? MapStudent(reader) : null;
        }

        public int Add(Student student)
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
INSERT INTO dbo.Students
    (StudentNo, Name, Gender, BirthDate, IdCard, Nation, PoliticalStatus, PhoneNumber, Email, Address, Department, Major, [Class], EnrollmentDate, Photo)
OUTPUT INSERTED.Id
VALUES
    (@StudentNo, @Name, @Gender, @BirthDate, @IdCard, @Nation, @PoliticalStatus, @PhoneNumber, @Email, @Address, @Department, @Major, @Class, @EnrollmentDate, @Photo);";

            AddStudentParameters(command, student);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public void Update(Student student)
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
UPDATE dbo.Students
SET StudentNo = @StudentNo,
    Name = @Name,
    Gender = @Gender,
    BirthDate = @BirthDate,
    IdCard = @IdCard,
    Nation = @Nation,
    PoliticalStatus = @PoliticalStatus,
    PhoneNumber = @PhoneNumber,
    Email = @Email,
    Address = @Address,
    Department = @Department,
    Major = @Major,
    [Class] = @Class,
    EnrollmentDate = @EnrollmentDate,
    Photo = @Photo
WHERE Id = @Id;";

            AddStudentParameters(command, student);
            command.Parameters.AddWithValue("@Id", student.Id);
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                DeleteRelatedRecords(connection, transaction, id, "dbo.FamilyInfos");
                DeleteRelatedRecords(connection, transaction, id, "dbo.RewardRecords");
                DeleteRelatedRecords(connection, transaction, id, "dbo.PunishmentRecords");
                DeleteRelatedRecords(connection, transaction, id, "dbo.HealthRecords");
                DeleteRelatedRecords(connection, transaction, id, "dbo.StudentRegistrations");
                DeleteRelatedRecords(connection, transaction, id, "dbo.StatusChangeRecords");
                DeleteRelatedRecords(connection, transaction, id, "dbo.ScholarshipInfos");
                DeleteRelatedRecords(connection, transaction, id, "dbo.GraduationInfos");
                DeleteRelatedRecords(connection, transaction, id, "dbo.Scores");

                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = "DELETE FROM dbo.Students WHERE Id = @Id;";
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private static void DeleteRelatedRecords(SqlConnection connection, SqlTransaction transaction, int studentId, string tableName)
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $"DELETE FROM {tableName} WHERE StudentId = @StudentId;";
            command.Parameters.AddWithValue("@StudentId", studentId);
            command.ExecuteNonQuery();
        }

        private static void AddStudentParameters(SqlCommand command, Student student)
        {
            command.Parameters.AddWithValue("@StudentNo", student.StudentNo);
            command.Parameters.AddWithValue("@Name", student.Name);
            command.Parameters.AddWithValue("@Gender", student.Gender);
            command.Parameters.AddWithValue("@BirthDate", (object?)student.BirthDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@IdCard", student.IdCard);
            command.Parameters.AddWithValue("@Nation", student.Nation);
            command.Parameters.AddWithValue("@PoliticalStatus", student.PoliticalStatus);
            command.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
            command.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(student.Email) ? DBNull.Value : student.Email);
            command.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(student.Address) ? DBNull.Value : student.Address);
            command.Parameters.AddWithValue("@Department", student.Department);
            command.Parameters.AddWithValue("@Major", student.Major);
            command.Parameters.AddWithValue("@Class", student.Class);
            command.Parameters.AddWithValue("@EnrollmentDate", (object?)student.EnrollmentDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@Photo", string.IsNullOrWhiteSpace(student.Photo) ? DBNull.Value : student.Photo);
        }

        private static Student MapStudent(SqlDataReader reader)
        {
            return new Student
            {
                Id = reader.GetInt32(0),
                StudentNo = reader.GetString(1),
                Name = reader.GetString(2),
                Gender = reader.GetString(3),
                BirthDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                IdCard = reader.GetString(5),
                Nation = reader.GetString(6),
                PoliticalStatus = reader.GetString(7),
                PhoneNumber = reader.GetString(8),
                Email = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                Address = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                Department = reader.GetString(11),
                Major = reader.GetString(12),
                Class = reader.GetString(13),
                EnrollmentDate = reader.IsDBNull(14) ? null : reader.GetDateTime(14),
                Photo = reader.IsDBNull(15) ? string.Empty : reader.GetString(15)
            };
        }
    }
}
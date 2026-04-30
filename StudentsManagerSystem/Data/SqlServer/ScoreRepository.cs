using Microsoft.Data.SqlClient;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Data.SqlServer
{
    internal sealed class ScoreRepository
    {
        public List<Score> GetAll()
        {
            return QueryScores(@"
SELECT Id, StudentId, StudentNo, StudentName, AcademicYear, Semester, CourseNo, CourseName,
       Credit, RegularScore, ExamScore, TotalScore, Grade, Status, Remarks
FROM dbo.Scores
ORDER BY StudentNo, AcademicYear DESC, Semester, CourseNo;");
        }

        public List<Score> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return GetAll();
            }

            keyword = keyword.Trim();
            return QueryScores(@"
SELECT Id, StudentId, StudentNo, StudentName, AcademicYear, Semester, CourseNo, CourseName,
       Credit, RegularScore, ExamScore, TotalScore, Grade, Status, Remarks
FROM dbo.Scores
WHERE StudentNo LIKE @Keyword
   OR StudentName LIKE @Keyword
   OR CourseNo LIKE @Keyword
   OR CourseName LIKE @Keyword
ORDER BY StudentNo, AcademicYear DESC, Semester, CourseNo;",
                command => command.Parameters.AddWithValue("@Keyword", $"%{keyword}%"));
        }

        public List<Score> GetByAcademicYearSemester(string academicYear, string semester)
        {
            if (string.IsNullOrWhiteSpace(academicYear) || string.IsNullOrWhiteSpace(semester))
            {
                return GetAll();
            }

            return QueryScores(@"
SELECT Id, StudentId, StudentNo, StudentName, AcademicYear, Semester, CourseNo, CourseName,
       Credit, RegularScore, ExamScore, TotalScore, Grade, Status, Remarks
FROM dbo.Scores
WHERE AcademicYear = @AcademicYear
  AND Semester = @Semester
ORDER BY StudentNo, AcademicYear DESC, Semester, CourseNo;",
                command =>
                {
                    command.Parameters.AddWithValue("@AcademicYear", academicYear);
                    command.Parameters.AddWithValue("@Semester", semester);
                });
        }

        public Score? GetById(int id)
        {
            var items = QueryScores(@"
SELECT Id, StudentId, StudentNo, StudentName, AcademicYear, Semester, CourseNo, CourseName,
       Credit, RegularScore, ExamScore, TotalScore, Grade, Status, Remarks
FROM dbo.Scores
WHERE Id = @Id;",
                command => command.Parameters.AddWithValue("@Id", id));

            return items.FirstOrDefault();
        }

        public List<Course> GetCourses()
        {
            var items = new List<Course>();

            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
SELECT Id, CourseNo, CourseName, Credit, CourseType, Hours, Department, Remarks
FROM dbo.Courses
ORDER BY CourseNo;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new Course
                {
                    Id = reader.GetInt32(0),
                    CourseNo = reader.GetString(1),
                    CourseName = reader.GetString(2),
                    Credit = reader.GetDecimal(3),
                    CourseType = reader.GetString(4),
                    Hours = reader.GetInt32(5),
                    Department = reader.GetString(6),
                    Remarks = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                });
            }

            return items;
        }

        public int Add(Score score)
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
INSERT INTO dbo.Scores
    (StudentId, StudentNo, StudentName, AcademicYear, Semester, CourseNo, CourseName, Credit,
     RegularScore, ExamScore, TotalScore, Grade, Status, Remarks)
OUTPUT INSERTED.Id
VALUES
    (@StudentId, @StudentNo, @StudentName, @AcademicYear, @Semester, @CourseNo, @CourseName, @Credit,
     @RegularScore, @ExamScore, @TotalScore, @Grade, @Status, @Remarks);";

            AddParameters(command, score);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public void Update(Score score)
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
UPDATE dbo.Scores
SET StudentId = @StudentId,
    StudentNo = @StudentNo,
    StudentName = @StudentName,
    AcademicYear = @AcademicYear,
    Semester = @Semester,
    CourseNo = @CourseNo,
    CourseName = @CourseName,
    Credit = @Credit,
    RegularScore = @RegularScore,
    ExamScore = @ExamScore,
    TotalScore = @TotalScore,
    Grade = @Grade,
    Status = @Status,
    Remarks = @Remarks
WHERE Id = @Id;";

            AddParameters(command, score);
            command.Parameters.AddWithValue("@Id", score.Id);
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM dbo.Scores WHERE Id = @Id;";
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }

        private static List<Score> QueryScores(string sql, Action<SqlCommand>? configure = null)
        {
            var items = new List<Score>();

            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = sql;
            configure?.Invoke(command);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(MapScore(reader));
            }

            return items;
        }

        private static void AddParameters(SqlCommand command, Score score)
        {
            command.Parameters.AddWithValue("@StudentId", score.StudentId);
            command.Parameters.AddWithValue("@StudentNo", score.StudentNo);
            command.Parameters.AddWithValue("@StudentName", score.StudentName);
            command.Parameters.AddWithValue("@AcademicYear", score.AcademicYear);
            command.Parameters.AddWithValue("@Semester", score.Semester);
            command.Parameters.AddWithValue("@CourseNo", score.CourseNo);
            command.Parameters.AddWithValue("@CourseName", score.CourseName);
            command.Parameters.AddWithValue("@Credit", score.Credit);
            command.Parameters.AddWithValue("@RegularScore", (object?)score.RegularScore ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExamScore", (object?)score.ExamScore ?? DBNull.Value);
            command.Parameters.AddWithValue("@TotalScore", (object?)score.TotalScore ?? DBNull.Value);
            command.Parameters.AddWithValue("@Grade", score.Grade);
            command.Parameters.AddWithValue("@Status", score.Status);
            command.Parameters.AddWithValue("@Remarks", string.IsNullOrWhiteSpace(score.Remarks) ? DBNull.Value : score.Remarks);
        }

        private static Score MapScore(SqlDataReader reader)
        {
            return new Score
            {
                Id = reader.GetInt32(0),
                StudentId = reader.GetInt32(1),
                StudentNo = reader.GetString(2),
                StudentName = reader.GetString(3),
                AcademicYear = reader.GetString(4),
                Semester = reader.GetString(5),
                CourseNo = reader.GetString(6),
                CourseName = reader.GetString(7),
                Credit = reader.GetDecimal(8),
                RegularScore = reader.IsDBNull(9) ? null : reader.GetDecimal(9),
                ExamScore = reader.IsDBNull(10) ? null : reader.GetDecimal(10),
                TotalScore = reader.IsDBNull(11) ? null : reader.GetDecimal(11),
                Grade = reader.GetString(12),
                Status = reader.GetString(13),
                Remarks = reader.IsDBNull(14) ? string.Empty : reader.GetString(14)
            };
        }
    }
}
using Microsoft.Data.SqlClient;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Data.SqlServer
{
    internal sealed class StudentStatusRepository
    {
        public List<StudentRegistration> GetRegistrations() => QueryRegistrations(null, null);

        public List<StudentRegistration> SearchRegistrations(string keyword)
        {
            keyword = keyword.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return GetRegistrations();
            }

            return QueryRegistrations(@"
SELECT Id, StudentId, StudentNo, StudentName, RegistrationDate, AcademicYear, Semester, Status, Remarks
FROM dbo.StudentRegistrations
WHERE StudentNo LIKE @Keyword
   OR StudentName LIKE @Keyword
   OR AcademicYear LIKE @Keyword
   OR Semester LIKE @Keyword
   OR Status LIKE @Keyword
ORDER BY StudentNo;",
                command => command.Parameters.AddWithValue("@Keyword", $"%{keyword}%"));
        }

        public List<StatusChangeRecord> GetChanges() => QueryChanges(null, null);

        public List<StatusChangeRecord> SearchChanges(string keyword)
        {
            keyword = keyword.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return GetChanges();
            }

            return QueryChanges(@"
SELECT Id, StudentId, StudentNo, StudentName, ChangeDate, ChangeType, OriginalInfo, NewInfo, Reason, ApprovalStatus
FROM dbo.StatusChangeRecords
WHERE StudentNo LIKE @Keyword
   OR StudentName LIKE @Keyword
   OR ChangeType LIKE @Keyword
   OR OriginalInfo LIKE @Keyword
   OR NewInfo LIKE @Keyword
   OR Reason LIKE @Keyword
   OR ApprovalStatus LIKE @Keyword
ORDER BY ChangeDate DESC, StudentNo;",
                command => command.Parameters.AddWithValue("@Keyword", $"%{keyword}%"));
        }

        public List<ScholarshipInfo> GetScholarships() => QueryScholarships(null, null);

        public List<ScholarshipInfo> SearchScholarships(string keyword)
        {
            keyword = keyword.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return GetScholarships();
            }

            return QueryScholarships(@"
SELECT Id, StudentId, StudentNo, StudentName, AcademicYear, Semester, ScholarshipType, ScholarshipLevel, Amount, AwardDate, Status
FROM dbo.ScholarshipInfos
WHERE StudentNo LIKE @Keyword
   OR StudentName LIKE @Keyword
   OR AcademicYear LIKE @Keyword
   OR Semester LIKE @Keyword
   OR ScholarshipType LIKE @Keyword
   OR ScholarshipLevel LIKE @Keyword
   OR Status LIKE @Keyword
ORDER BY AwardDate DESC, StudentNo;",
                command => command.Parameters.AddWithValue("@Keyword", $"%{keyword}%"));
        }

        public List<GraduationInfo> GetGraduations() => QueryGraduations(null, null);

        public List<GraduationInfo> SearchGraduations(string keyword)
        {
            keyword = keyword.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return GetGraduations();
            }

            return QueryGraduations(@"
SELECT Id, StudentId, StudentNo, StudentName, GraduationDate, GraduationType, DegreeType, CertificateNo, DegreeNo, Remarks
FROM dbo.GraduationInfos
WHERE StudentNo LIKE @Keyword
   OR StudentName LIKE @Keyword
   OR GraduationType LIKE @Keyword
   OR DegreeType LIKE @Keyword
   OR CertificateNo LIKE @Keyword
   OR DegreeNo LIKE @Keyword
ORDER BY GraduationDate DESC, StudentNo;",
                command => command.Parameters.AddWithValue("@Keyword", $"%{keyword}%"));
        }

        public void DeleteRegistration(int id) => ExecuteDelete("dbo.StudentRegistrations", id);
        public void DeleteChange(int id) => ExecuteDelete("dbo.StatusChangeRecords", id);
        public void DeleteScholarship(int id) => ExecuteDelete("dbo.ScholarshipInfos", id);
        public void DeleteGraduation(int id) => ExecuteDelete("dbo.GraduationInfos", id);

        private static List<StudentRegistration> QueryRegistrations(string? sql, Action<SqlCommand>? configure)
        {
            sql ??= @"
SELECT Id, StudentId, StudentNo, StudentName, RegistrationDate, AcademicYear, Semester, Status, Remarks
FROM dbo.StudentRegistrations
ORDER BY StudentNo;";
            return Query(sql, configure, reader => new StudentRegistration
            {
                Id = reader.GetInt32(0),
                StudentId = reader.GetInt32(1),
                StudentNo = reader.GetString(2),
                StudentName = reader.GetString(3),
                RegistrationDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                AcademicYear = reader.GetString(5),
                Semester = reader.GetString(6),
                Status = reader.GetString(7),
                Remarks = reader.IsDBNull(8) ? string.Empty : reader.GetString(8)
            });
        }

        private static List<StatusChangeRecord> QueryChanges(string? sql, Action<SqlCommand>? configure)
        {
            sql ??= @"
SELECT Id, StudentId, StudentNo, StudentName, ChangeDate, ChangeType, OriginalInfo, NewInfo, Reason, ApprovalStatus
FROM dbo.StatusChangeRecords
ORDER BY ChangeDate DESC, StudentNo;";
            return Query(sql, configure, reader => new StatusChangeRecord
            {
                Id = reader.GetInt32(0),
                StudentId = reader.GetInt32(1),
                StudentNo = reader.GetString(2),
                StudentName = reader.GetString(3),
                ChangeDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                ChangeType = reader.GetString(5),
                OriginalInfo = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                NewInfo = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                Reason = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                ApprovalStatus = reader.GetString(9)
            });
        }

        private static List<ScholarshipInfo> QueryScholarships(string? sql, Action<SqlCommand>? configure)
        {
            sql ??= @"
SELECT Id, StudentId, StudentNo, StudentName, AcademicYear, Semester, ScholarshipType, ScholarshipLevel, Amount, AwardDate, Status
FROM dbo.ScholarshipInfos
ORDER BY AwardDate DESC, StudentNo;";
            return Query(sql, configure, reader => new ScholarshipInfo
            {
                Id = reader.GetInt32(0),
                StudentId = reader.GetInt32(1),
                StudentNo = reader.GetString(2),
                StudentName = reader.GetString(3),
                AcademicYear = reader.GetString(4),
                Semester = reader.GetString(5),
                ScholarshipType = reader.GetString(6),
                ScholarshipLevel = reader.GetString(7),
                Amount = reader.GetDecimal(8),
                AwardDate = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                Status = reader.GetString(10)
            });
        }

        private static List<GraduationInfo> QueryGraduations(string? sql, Action<SqlCommand>? configure)
        {
            sql ??= @"
SELECT Id, StudentId, StudentNo, StudentName, GraduationDate, GraduationType, DegreeType, CertificateNo, DegreeNo, Remarks
FROM dbo.GraduationInfos
ORDER BY GraduationDate DESC, StudentNo;";
            return Query(sql, configure, reader => new GraduationInfo
            {
                Id = reader.GetInt32(0),
                StudentId = reader.GetInt32(1),
                StudentNo = reader.GetString(2),
                StudentName = reader.GetString(3),
                GraduationDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                GraduationType = reader.GetString(5),
                DegreeType = reader.GetString(6),
                CertificateNo = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                DegreeNo = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                Remarks = reader.IsDBNull(9) ? string.Empty : reader.GetString(9)
            });
        }

        private static List<T> Query<T>(string sql, Action<SqlCommand>? configure, Func<SqlDataReader, T> map)
        {
            var items = new List<T>();
            using var connection = SqlServerConnectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            configure?.Invoke(command);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(map(reader));
            }
            return items;
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
    }
}
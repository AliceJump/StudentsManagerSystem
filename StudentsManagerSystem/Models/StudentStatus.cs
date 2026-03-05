namespace StudentsManagerSystem.Models
{
    /// <summary>
    /// 学籍注册信息
    /// </summary>
    public class StudentRegistration
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentNo { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public DateTime? RegistrationDate { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }

    /// <summary>
    /// 学籍变动记录
    /// </summary>
    public class StatusChangeRecord
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentNo { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public DateTime? ChangeDate { get; set; }
        public string ChangeType { get; set; } = string.Empty;
        public string OriginalInfo { get; set; } = string.Empty;
        public string NewInfo { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string ApprovalStatus { get; set; } = string.Empty;
    }

    /// <summary>
    /// 奖助学金信息
    /// </summary>
    public class ScholarshipInfo
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentNo { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string ScholarshipType { get; set; } = string.Empty;
        public string ScholarshipLevel { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime? AwardDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// 毕业信息
    /// </summary>
    public class GraduationInfo
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentNo { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public DateTime? GraduationDate { get; set; }
        public string GraduationType { get; set; } = string.Empty;
        public string DegreeType { get; set; } = string.Empty;
        public string CertificateNo { get; set; } = string.Empty;
        public string DegreeNo { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }
}

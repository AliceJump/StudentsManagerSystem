namespace StudentsManagerSystem.Models
{
    /// <summary>
    /// 学生成绩
    /// </summary>
    public class Score
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentNo { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string CourseNo { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public decimal Credit { get; set; }
        public decimal? RegularScore { get; set; }
        public decimal? ExamScore { get; set; }
        public decimal? TotalScore { get; set; }
        public string Grade { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }

    /// <summary>
    /// 课程信息
    /// </summary>
    public class Course
    {
        public int Id { get; set; }
        public string CourseNo { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public decimal Credit { get; set; }
        public string CourseType { get; set; } = string.Empty;
        public int Hours { get; set; }
        public string Department { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }
}

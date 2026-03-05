namespace StudentsManagerSystem.Models
{
    /// <summary>
    /// 院系信息
    /// </summary>
    public class Department
    {
        public int Id { get; set; }
        public string DepartmentNo { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentHead { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }

    /// <summary>
    /// 专业信息
    /// </summary>
    public class Major
    {
        public int Id { get; set; }
        public string MajorNo { get; set; } = string.Empty;
        public string MajorName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string DegreeType { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }

    /// <summary>
    /// 班级信息
    /// </summary>
    public class Class
    {
        public int Id { get; set; }
        public string ClassNo { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string MajorName { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string ClassTeacher { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }
}

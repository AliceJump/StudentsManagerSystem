namespace StudentsManagerSystem.Models
{
    /// <summary>
    /// 学生基本信息
    /// </summary>
    public class Student
    {
        public int Id { get; set; }
        public string StudentNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public string IdCard { get; set; } = string.Empty;
        public string Nation { get; set; } = string.Empty;
        public string PoliticalStatus { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Major { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public DateTime? EnrollmentDate { get; set; }
        public string Photo { get; set; } = string.Empty;
    }

    /// <summary>
    /// 学生家庭信息
    /// </summary>
    public class FamilyInfo
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentNo { get; set; } = string.Empty;
        public string RelationName { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string WorkUnit { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    /// <summary>
    /// 奖励记录
    /// </summary>
    public class RewardRecord
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentNo { get; set; } = string.Empty;
        public DateTime? RewardDate { get; set; }
        public string RewardType { get; set; } = string.Empty;
        public string RewardLevel { get; set; } = string.Empty;
        public string RewardReason { get; set; } = string.Empty;
        public string RewardUnit { get; set; } = string.Empty;
    }

    /// <summary>
    /// 处分记录
    /// </summary>
    public class PunishmentRecord
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentNo { get; set; } = string.Empty;
        public DateTime? PunishmentDate { get; set; }
        public string PunishmentType { get; set; } = string.Empty;
        public string PunishmentLevel { get; set; } = string.Empty;
        public string PunishmentReason { get; set; } = string.Empty;
        public DateTime? CancelDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// 体检信息
    /// </summary>
    public class HealthRecord
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentNo { get; set; } = string.Empty;
        public DateTime? CheckDate { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string BloodType { get; set; } = string.Empty;
        public string Vision { get; set; } = string.Empty;
        public string HealthStatus { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }
}

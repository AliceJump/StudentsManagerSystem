using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentsManagerSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClassNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ClassName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DepartmentName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MajorName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Grade = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ClassTeacher = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    StudentCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CourseNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CourseName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Credit = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: false),
                    CourseType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Hours = table.Column<int>(type: "INTEGER", nullable: false),
                    Department = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DepartmentNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DepartmentName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DepartmentHead = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FamilyInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RelationName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Relationship = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    WorkUnit = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GraduationInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StudentName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    GraduationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    GraduationType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DegreeType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CertificateNo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DegreeNo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraduationInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HealthRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CheckDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Height = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: false),
                    Weight = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: false),
                    BloodType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Vision = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    HealthStatus = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LookupOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Majors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MajorNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    MajorName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DepartmentName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Duration = table.Column<int>(type: "INTEGER", nullable: false),
                    DegreeType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Majors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PunishmentRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PunishmentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PunishmentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PunishmentLevel = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PunishmentReason = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CancelDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PunishmentRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RewardRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RewardDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RewardType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RewardLevel = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RewardReason = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    RewardUnit = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScholarshipInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StudentName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AcademicYear = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Semester = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ScholarshipType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ScholarshipLevel = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    AwardDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScholarshipInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StudentName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AcademicYear = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Semester = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CourseNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CourseName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Credit = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: false),
                    RegularScore = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: true),
                    ExamScore = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: true),
                    TotalScore = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: true),
                    Grade = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusChangeRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StudentName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ChangeType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OriginalInfo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NewInfo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ApprovalStatus = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusChangeRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StudentName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AcademicYear = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Semester = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentRegistrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentNo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Gender = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IdCard = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Nation = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PoliticalStatus = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Department = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Major = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Class = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Photo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ClassNo",
                table: "Classes",
                column: "ClassNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseNo",
                table: "Courses",
                column: "CourseNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DepartmentNo",
                table: "Departments",
                column: "DepartmentNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FamilyInfos_StudentNo",
                table: "FamilyInfos",
                column: "StudentNo");

            migrationBuilder.CreateIndex(
                name: "IX_HealthRecords_StudentNo",
                table: "HealthRecords",
                column: "StudentNo");

            migrationBuilder.CreateIndex(
                name: "IX_LookupOptions_Category_Value",
                table: "LookupOptions",
                columns: new[] { "Category", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Majors_MajorNo",
                table: "Majors",
                column: "MajorNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PunishmentRecords_StudentNo",
                table: "PunishmentRecords",
                column: "StudentNo");

            migrationBuilder.CreateIndex(
                name: "IX_RewardRecords_StudentNo",
                table: "RewardRecords",
                column: "StudentNo");

            migrationBuilder.CreateIndex(
                name: "IX_Students_IdCard",
                table: "Students",
                column: "IdCard",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentNo",
                table: "Students",
                column: "StudentNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "FamilyInfos");

            migrationBuilder.DropTable(
                name: "GraduationInfos");

            migrationBuilder.DropTable(
                name: "HealthRecords");

            migrationBuilder.DropTable(
                name: "LookupOptions");

            migrationBuilder.DropTable(
                name: "Majors");

            migrationBuilder.DropTable(
                name: "PunishmentRecords");

            migrationBuilder.DropTable(
                name: "RewardRecords");

            migrationBuilder.DropTable(
                name: "ScholarshipInfos");

            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.DropTable(
                name: "StatusChangeRecords");

            migrationBuilder.DropTable(
                name: "StudentRegistrations");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

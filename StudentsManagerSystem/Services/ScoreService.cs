using StudentsManagerSystem.Common;
using StudentsManagerSystem.Data;
using StudentsManagerSystem.Data.Repositories;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Services
{
    internal sealed class ScoreService
    {
        private readonly ScoreRepository repository = new();

        private static string ResolveStudentName(string studentNo)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Students.Where(s => s.StudentNo == studentNo).Select(s => s.Name).FirstOrDefault() ?? string.Empty;
        }

        public List<Score> GetAll() => repository.GetAll();

        public List<Score> GetByAcademicYearSemester(string academicYear, string semester) => repository.GetByAcademicYearSemester(academicYear, semester);

        public List<Course> GetCourses() => repository.GetCourses();

        public List<string> GetAcademicYears() => repository.GetAcademicYears();

        public List<string> GetSemesters() => repository.GetSemesters();

        public List<string> GetSemesters(string academicYear) => repository.GetSemesters(academicYear);

        public ServiceResult Add(Score score)
        {
            score.StudentName = ResolveStudentName(score.StudentNo);
            if (string.IsNullOrEmpty(score.StudentName))
            {
                return ServiceResult.Failure("学号不存在，请先维护学生基本信息");
            }

            var validation = Validate(score);
            if (!validation.Succeeded)
            {
                return validation;
            }

            repository.Add(score);
            AppLogger.Info($"新增成绩：{score.StudentNo} {score.CourseNo}");
            return ServiceResult.Success("成绩新增成功");
        }

        public ServiceResult Update(Score score)
        {
            if (score.Id <= 0)
            {
                return ServiceResult.Failure("请选择要修改的成绩记录");
            }

            score.StudentName = ResolveStudentName(score.StudentNo);
            if (string.IsNullOrEmpty(score.StudentName))
            {
                return ServiceResult.Failure("学号不存在，请先维护学生基本信息");
            }

            var validation = Validate(score);
            if (!validation.Succeeded)
            {
                return validation;
            }

            repository.Update(score);
            AppLogger.Info($"修改成绩：{score.StudentNo} {score.CourseNo}");
            return ServiceResult.Success("成绩修改成功");
        }

        public ServiceResult SaveImported(Score score)
        {
            var existing = repository.GetAll().FirstOrDefault(item =>
                item.StudentNo == score.StudentNo &&
                item.AcademicYear == score.AcademicYear &&
                item.Semester == score.Semester &&
                item.CourseNo == score.CourseNo);

            if (existing == null)
            {
                return Add(score);
            }

            score.Id = existing.Id;
            return Update(score);
        }

        public void Delete(int id)
        {
            repository.Delete(id);
            AppLogger.Info($"删除成绩：Id={id}");
        }

        private static ServiceResult Validate(Score score)
        {
            if (!InputValidator.ValidateStudentNo(score.StudentNo) || !InputValidator.ValidateName(score.StudentName))
            {
                return ServiceResult.Failure("请检查学号和姓名格式");
            }

            if (!InputValidator.ValidateAcademicYear(score.AcademicYear) || !InputValidator.ValidateSemester(score.Semester))
            {
                return ServiceResult.Failure("请检查学年和学期格式");
            }

            if (string.IsNullOrWhiteSpace(score.CourseNo) || string.IsNullOrWhiteSpace(score.CourseName) || score.Credit < 0)
            {
                return ServiceResult.Failure("请检查课程和学分信息");
            }

            if (!IsScoreInRange(score.RegularScore) || !IsScoreInRange(score.ExamScore) || !IsScoreInRange(score.TotalScore))
            {
                return ServiceResult.Failure("成绩应在0-100之间");
            }

            return ServiceResult.Success();
        }

        private static bool IsScoreInRange(decimal? score) => !score.HasValue || score is >= 0 and <= 100;
    }
}

using Microsoft.EntityFrameworkCore;
using StudentsManagerSystem.Data;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Data.SqlServer
{
    /// <summary>
    /// 学生成绩仓储。
    /// </summary>
    internal sealed class ScoreRepository
    {
        public List<Score> GetAll()
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Scores.AsNoTracking().OrderByDescending(score => score.AcademicYear).ThenBy(score => score.Semester).ThenBy(score => score.StudentNo).ThenBy(score => score.CourseNo).ToList();
        }

        public Task<List<Score>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Scores.AsNoTracking().OrderByDescending(score => score.AcademicYear).ThenBy(score => score.Semester).ThenBy(score => score.StudentNo).ThenBy(score => score.CourseNo).ToListAsync(cancellationToken);
        }

        public List<Score> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return GetAll();
            }

            keyword = keyword.Trim();
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Scores.AsNoTracking()
                .Where(score =>
                    EF.Functions.Like(score.StudentNo, $"%{keyword}%") ||
                    EF.Functions.Like(score.StudentName, $"%{keyword}%") ||
                    EF.Functions.Like(score.CourseNo, $"%{keyword}%") ||
                    EF.Functions.Like(score.CourseName, $"%{keyword}%"))
                .OrderByDescending(score => score.AcademicYear)
                .ThenBy(score => score.Semester)
                .ThenBy(score => score.StudentNo)
                .ThenBy(score => score.CourseNo)
                .ToList();
        }

        public List<Score> GetByAcademicYearSemester(string academicYear, string semester)
        {
            if (string.IsNullOrWhiteSpace(academicYear) || string.IsNullOrWhiteSpace(semester))
            {
                return GetAll();
            }

            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Scores.AsNoTracking()
                .Where(score => score.AcademicYear == academicYear && score.Semester == semester)
                .OrderBy(score => score.StudentNo)
                .ThenBy(score => score.CourseNo)
                .ToList();
        }

        public Score? GetById(int id)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Scores.AsNoTracking().FirstOrDefault(score => score.Id == id);
        }

        public List<Course> GetCourses()
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            return context.Courses.AsNoTracking().OrderBy(course => course.CourseNo).ToList();
        }

        public int Add(Score score)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.Scores.Add(score);
            context.SaveChanges();
            return score.Id;
        }

        public void Update(Score score)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            context.Scores.Update(score);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            using var context = StudentsManagerDbContextFactory.CreateDbContext();
            var score = context.Scores.FirstOrDefault(item => item.Id == id);
            if (score == null)
            {
                return;
            }

            context.Scores.Remove(score);
            context.SaveChanges();
        }
    }
}
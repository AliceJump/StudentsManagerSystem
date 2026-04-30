using System.Globalization;
using System.Windows;
using StudentsManagerSystem.Data.SqlServer;
using ScoreModel = StudentsManagerSystem.Models.Score;
using CourseModel = StudentsManagerSystem.Models.Course;

namespace StudentsManagerSystem.Views.Score
{
    public partial class ScoreEditWindow : Window
    {
        private readonly ScoreRepository scoreRepository = new ScoreRepository();
        private readonly List<CourseModel> courses;
        private readonly ScoreModel? existingScore;

        public ScoreModel? Result { get; private set; }

        public ScoreEditWindow()
        {
            InitializeComponent();
            courses = scoreRepository.GetCourses();
            existingScore = null;
            LoadCourses();
        }

        public ScoreEditWindow(ScoreModel score)
        {
            InitializeComponent();
            courses = scoreRepository.GetCourses();
            existingScore = score;
            LoadCourses();
            LoadScore(score);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStudentNo.Text) ||
                string.IsNullOrWhiteSpace(txtStudentName.Text) ||
                cmbAcademicYear.SelectedItem == null ||
                cmbSemester.SelectedItem == null ||
                cmbCourse.SelectedItem == null)
            {
                MessageBox.Show("请填写所有必填项（标*的字段）！", "提示",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TryBuildScore(out var score))
            {
                return;
            }

            Result = score;
            DialogResult = true;
            Close();
        }

        private void LoadCourses()
        {
            cmbCourse.ItemsSource = courses;
            cmbCourse.DisplayMemberPath = nameof(CourseModel.CourseName);
            cmbCourse.SelectedValuePath = nameof(CourseModel.CourseNo);

            if (courses.Count > 0)
            {
                cmbCourse.SelectedIndex = 0;
            }

            UpdateCredit();
        }

        private void LoadScore(ScoreModel score)
        {
            txtStudentNo.Text = score.StudentNo;
            txtStudentName.Text = score.StudentName;
            cmbAcademicYear.Text = score.AcademicYear;
            cmbSemester.Text = score.Semester;
            cmbCourse.SelectedItem = courses.FirstOrDefault(item => item.CourseNo == score.CourseNo);
            txtRegularScore.Text = score.RegularScore?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
            txtExamScore.Text = score.ExamScore?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
            txtRemarks.Text = score.Remarks;
            UpdateCredit();
        }

        private void cmbCourse_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateCredit();
        }

        private void UpdateCredit()
        {
            if (cmbCourse.SelectedItem is CourseModel course)
            {
                txtCredit.Text = course.Credit.ToString(CultureInfo.InvariantCulture);
            }
        }

        private bool TryBuildScore(out ScoreModel score)
        {
            score = new ScoreModel();

            if (cmbCourse.SelectedItem is not CourseModel course)
            {
                MessageBox.Show("请选择课程！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(txtCredit.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var credit))
            {
                MessageBox.Show("学分格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            decimal? regularScore = null;
            if (!string.IsNullOrWhiteSpace(txtRegularScore.Text))
            {
                if (!decimal.TryParse(txtRegularScore.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var regularValue))
                {
                    MessageBox.Show("平时成绩格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                regularScore = regularValue;
            }

            decimal? examScore = null;
            if (!string.IsNullOrWhiteSpace(txtExamScore.Text))
            {
                if (!decimal.TryParse(txtExamScore.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var examValue))
                {
                    MessageBox.Show("考试成绩格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                examScore = examValue;
            }

            decimal? totalScore = null;
            if (regularScore.HasValue && examScore.HasValue)
            {
                totalScore = Math.Round((regularScore.Value + examScore.Value) / 2m, 1);
            }

            score = new ScoreModel
            {
                Id = existingScore?.Id ?? 0,
                StudentId = existingScore?.StudentId ?? 0,
                StudentNo = txtStudentNo.Text.Trim(),
                StudentName = txtStudentName.Text.Trim(),
                AcademicYear = GetComboText(cmbAcademicYear),
                Semester = GetComboText(cmbSemester),
                CourseNo = course.CourseNo,
                CourseName = course.CourseName,
                Credit = credit,
                RegularScore = regularScore,
                ExamScore = examScore,
                TotalScore = totalScore,
                Grade = CalculateGrade(totalScore),
                Status = "正常",
                Remarks = txtRemarks.Text.Trim()
            };

            return true;
        }

        private static string GetComboText(System.Windows.Controls.ComboBox comboBox)
        {
            return comboBox.SelectedItem is System.Windows.Controls.ComboBoxItem item
                ? item.Content?.ToString() ?? string.Empty
                : comboBox.Text.Trim();
        }

        private static string CalculateGrade(decimal? totalScore)
        {
            if (!totalScore.HasValue)
            {
                return "未评定";
            }

            return totalScore.Value >= 90 ? "优秀"
                : totalScore.Value >= 80 ? "良好"
                : totalScore.Value >= 70 ? "中等"
                : totalScore.Value >= 60 ? "及格"
                : "不及格";
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Views.Score
{
    public partial class ScoreView : Page
    {
        private List<Models.Score> scores = new List<Models.Score>();

        public ScoreView()
        {
            InitializeComponent();
            LoadSampleData();
            LoadScoreData();
        }

        private void LoadSampleData()
        {
            scores = new List<Models.Score>
            {
                new Models.Score 
                { 
                    Id = 1, StudentId = 1, StudentNo = "2024001", StudentName = "张三", 
                    AcademicYear = "2024-2025", Semester = "第一学期", 
                    CourseNo = "CS101", CourseName = "程序设计基础", Credit = 4,
                    RegularScore = 85, ExamScore = 90, TotalScore = 88, Grade = "优秀", Status = "正常"
                },
                new Models.Score 
                { 
                    Id = 2, StudentId = 1, StudentNo = "2024001", StudentName = "张三", 
                    AcademicYear = "2024-2025", Semester = "第一学期", 
                    CourseNo = "MA101", CourseName = "高等数学", Credit = 5,
                    RegularScore = 80, ExamScore = 85, TotalScore = 83, Grade = "良好", Status = "正常"
                },
                new Models.Score 
                { 
                    Id = 3, StudentId = 2, StudentNo = "2024002", StudentName = "李四", 
                    AcademicYear = "2024-2025", Semester = "第一学期", 
                    CourseNo = "CS101", CourseName = "程序设计基础", Credit = 4,
                    RegularScore = 90, ExamScore = 92, TotalScore = 91, Grade = "优秀", Status = "正常"
                }
            };
        }

        private void LoadScoreData()
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = scores;
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            LoadScoreData();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var window = new ScoreEditWindow();
            window.ShowDialog();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                var window = new ScoreEditWindow();
                window.ShowDialog();
            }
            else
            {
                MessageBox.Show("请先选择要修改的成绩记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                var result = MessageBox.Show("确定要删除选中的成绩记录吗？", "确认删除", 
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show("删除操作将在后续实现", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("请先选择要删除的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                MessageBox.Show("查看详情功能将在后续实现", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("请先选择要查看的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("导入成绩功能将在后续实现", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("导出成绩功能将在后续实现", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadScoreData();
        }
    }
}

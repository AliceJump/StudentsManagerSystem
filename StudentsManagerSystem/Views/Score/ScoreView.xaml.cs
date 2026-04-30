using System.Windows;
using System.Windows.Controls;
using StudentsManagerSystem.Data.SqlServer;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Views.Score
{
    public partial class ScoreView : Page
    {
        private readonly ScoreRepository scoreRepository = new ScoreRepository();
        private List<Models.Score> scores = new List<Models.Score>();

        public ScoreView()
        {
            InitializeComponent();
            LoadScoreData();
        }

        private void LoadScoreData()
        {
            var academicYear = GetComboText(cmbAcademicYear);
            var semester = GetComboText(cmbSemester);
            scores = scoreRepository.GetByAcademicYearSemester(academicYear, semester);

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
            if (window.ShowDialog() == true && window.Result is not null)
            {
                scoreRepository.Add(window.Result);
                LoadScoreData();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Models.Score selectedScore)
            {
                var window = new ScoreEditWindow(selectedScore);
                if (window.ShowDialog() == true && window.Result is not null)
                {
                    scoreRepository.Update(window.Result);
                    LoadScoreData();
                }
            }
            else
            {
                MessageBox.Show("请先选择要修改的成绩记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Models.Score selectedScore)
            {
                var result = MessageBox.Show("确定要删除选中的成绩记录吗？", "确认删除",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    scoreRepository.Delete(selectedScore.Id);
                    LoadScoreData();
                }
            }
            else
            {
                MessageBox.Show("请先选择要删除的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Models.Score selectedScore)
            {
                MessageBox.Show(
                    $"学号：{selectedScore.StudentNo}\n姓名：{selectedScore.StudentName}\n课程：{selectedScore.CourseName}\n总评：{selectedScore.TotalScore?.ToString() ?? "未录入"}\n等级：{selectedScore.Grade}",
                    "成绩详情",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
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

        private static string GetComboText(System.Windows.Controls.ComboBox comboBox)
        {
            return comboBox.SelectedItem is System.Windows.Controls.ComboBoxItem item
                ? item.Content?.ToString() ?? string.Empty
                : comboBox.Text.Trim();
        }
    }
}
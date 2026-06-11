using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using StudentsManagerSystem.Common;
using StudentsManagerSystem.Models;
using StudentsManagerSystem.Services;

namespace StudentsManagerSystem.Views.Query
{
    public partial class QueryView : Page
    {
        private readonly StudentService studentService = new StudentService();
        private readonly BasicDataService basicDataService = new BasicDataService();
        private List<Student> students = new List<Student>();
        private bool _initialized = false;

        public QueryView()
        {
            InitializeComponent();
            LoadBusinessOptions();
            LoadDataFromRepository();
            ApplyStudentFilter();
            _initialized = true;
        }

        private void LoadDataFromRepository()
        {
            students = studentService.GetAll();
        }

        private void LoadBusinessOptions()
        {
            cmbDepartment.ItemsSource = new[] { "全部" }.Concat(basicDataService.GetDepartmentNames()).ToList();
            cmbDepartment.SelectedIndex = 0;
            cmbClass.ItemsSource = new[] { "全部" }.Concat(basicDataService.GetClassNames()).ToList();
            cmbClass.SelectedIndex = 0;
        }

        private void ApplyStudentFilter()
        {
            LoadDataFromRepository();
            var filtered = FilterStudents();
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = filtered;
            UpdateStatistics(filtered);
        }

        private void cmbQueryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized) return;

            if (cmbQueryType.SelectedIndex == 0)
            {
                ApplyStudentFilter();
            }
            else
            {
                dataGrid.ItemsSource = null;
                UpdateStatistics(Array.Empty<Student>());
            }
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            if (cmbQueryType.SelectedIndex == 0)
            {
                ApplyStudentFilter();
            }
            else
            {
                var result = FilterStudents();
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = result;
                UpdateStatistics(result, true);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtStudentNo.Text = "";
            txtName.Text = "";
            cmbDepartment.SelectedIndex = 0;
            cmbClass.SelectedIndex = 0;
            ApplyStudentFilter();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "CSV 文件 (*.csv)|*.csv",
                FileName = $"查询结果_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                Title = "导出查询结果"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                var current = dataGrid.ItemsSource?.Cast<Student>().ToList() ?? new List<Student>();
                CsvExportHelper.ExportToCsv(current, dialog.FileName);
                AppLogger.Info($"导出查询结果：{current.Count} 条，文件={dialog.FileName}");
                MessageBox.Show("查询结果导出成功。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                AppLogger.Error("导出查询结果失败。", ex);
                MessageBox.Show($"导出失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<Student> FilterStudents()
        {
            IEnumerable<Student> filtered = students;

            if (!string.IsNullOrWhiteSpace(txtStudentNo.Text))
            {
                filtered = filtered.Where(student => student.StudentNo.Contains(txtStudentNo.Text, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(txtName.Text))
            {
                filtered = filtered.Where(student => student.Name.Contains(txtName.Text, StringComparison.OrdinalIgnoreCase));
            }

            if (cmbDepartment.SelectedIndex > 0)
            {
                var department = GetComboText(cmbDepartment);
                if (!string.IsNullOrWhiteSpace(department))
                {
                    filtered = filtered.Where(student => string.Equals(student.Department, department, StringComparison.OrdinalIgnoreCase));
                }
            }

            if (cmbClass.SelectedIndex > 0)
            {
                var className = GetComboText(cmbClass);
                if (!string.IsNullOrWhiteSpace(className))
                {
                    filtered = filtered.Where(student => string.Equals(student.Class, className, StringComparison.OrdinalIgnoreCase));
                }
            }

            return filtered.ToList();
        }

        private static string GetComboText(ComboBox comboBox) => comboBox.SelectedItem is ComboBoxItem item
            ? item.Content?.ToString() ?? string.Empty
            : comboBox.SelectedItem?.ToString() ?? comboBox.Text.Trim();

        private void UpdateStatistics(IReadOnlyCollection<Student> currentStudents, bool includeSummary = false)
        {
            if (!currentStudents.Any())
            {
                txtStatistics.Text = "当前没有匹配的数据";
                return;
            }

            var maleCount = currentStudents.Count(student => string.Equals(student.Gender, "男", StringComparison.OrdinalIgnoreCase));
            var femaleCount = currentStudents.Count(student => string.Equals(student.Gender, "女", StringComparison.OrdinalIgnoreCase));
            var topDepartment = currentStudents.GroupBy(student => student.Department).OrderByDescending(group => group.Count()).FirstOrDefault();
            var topDepartmentText = topDepartment == null ? "-" : $"{topDepartment.Key}({topDepartment.Count()}人)";
            txtStatistics.Text = includeSummary
                ? $"共 {currentStudents.Count} 条，男 {maleCount}，女 {femaleCount}，最多院系：{topDepartmentText}"
                : $"共找到 {currentStudents.Count} 条记录，男 {maleCount}，女 {femaleCount}，最多院系：{topDepartmentText}";
        }
    }
}

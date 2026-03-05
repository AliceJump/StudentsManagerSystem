using System.Windows;
using System.Windows.Controls;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Views.Query
{
    public partial class QueryView : Page
    {
        private List<Student> students = new List<Student>();
        private bool _initialized = false;
        public QueryView()
        {

            InitializeComponent();
            LoadSampleData();
            LoadStudentData();
            _initialized = true;
        }

        private void LoadSampleData()
        {
            students = new List<Student>
            {
                new Student 
                { 
                    Id = 1, StudentNo = "2024001", Name = "张三", Gender = "男", 
                    BirthDate = new DateTime(2005, 3, 15), 
                    Department = "计算机学院", Major = "软件工程", Class = "软工2024-1班",
                    PhoneNumber = "13800138001", EnrollmentDate = new DateTime(2024, 9, 1)
                },
                new Student 
                { 
                    Id = 2, StudentNo = "2024002", Name = "李四", Gender = "女", 
                    BirthDate = new DateTime(2005, 6, 20), 
                    Department = "计算机学院", Major = "软件工程", Class = "软工2024-1班",
                    PhoneNumber = "13800138002", EnrollmentDate = new DateTime(2024, 9, 1)
                },
                new Student 
                { 
                    Id = 3, StudentNo = "2024003", Name = "王五", Gender = "男", 
                    BirthDate = new DateTime(2005, 8, 10), 
                    Department = "计算机学院", Major = "计算机科学与技术", Class = "计科2024-1班",
                    PhoneNumber = "13800138003", EnrollmentDate = new DateTime(2024, 9, 1)
                },
                new Student 
                { 
                    Id = 4, StudentNo = "2024004", Name = "赵六", Gender = "女", 
                    BirthDate = new DateTime(2005, 4, 25), 
                    Department = "电子信息学院", Major = "电子信息工程", Class = "电信2024-1班",
                    PhoneNumber = "13800138004", EnrollmentDate = new DateTime(2024, 9, 1)
                }
            };
        }

        private void LoadStudentData()
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = students;
            txtStatistics.Text = $"共找到 {students.Count} 条记录";
        }

        private void cmbQueryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized) return;

            if (cmbQueryType.SelectedIndex == 0)
            {
                LoadStudentData();
            }
            else
            {
                dataGrid.ItemsSource = null;
                txtStatistics.Text = "请点击查询按钮执行统计";
            }
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            if (cmbQueryType.SelectedIndex == 0)
            {
                // 按学生信息查询
                var filtered = students.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(txtStudentNo.Text))
                {
                    filtered = filtered.Where(s => s.StudentNo.Contains(txtStudentNo.Text));
                }

                if (!string.IsNullOrWhiteSpace(txtName.Text))
                {
                    filtered = filtered.Where(s => s.Name.Contains(txtName.Text));
                }

                if (cmbDepartment.SelectedIndex > 0)
                {
                    var dept = (cmbDepartment.SelectedItem as ComboBoxItem)?.Content.ToString();
                    filtered = filtered.Where(s => s.Department == dept);
                }

                if (cmbClass.SelectedIndex > 0)
                {
                    var cls = (cmbClass.SelectedItem as ComboBoxItem)?.Content.ToString();
                    filtered = filtered.Where(s => s.Class == cls);
                }

                var result = filtered.ToList();
                dataGrid.ItemsSource = result;
                txtStatistics.Text = $"共找到 {result.Count} 条记录";
            }
            else
            {
                // 统计查询
                MessageBox.Show("统计功能将在后续实现", "提示", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtStudentNo.Text = "";
            txtName.Text = "";
            cmbDepartment.SelectedIndex = 0;
            cmbClass.SelectedIndex = 0;
            LoadStudentData();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("导出功能将在后续实现", "提示", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

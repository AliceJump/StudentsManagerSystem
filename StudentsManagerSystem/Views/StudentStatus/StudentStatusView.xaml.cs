using System.Windows;
using System.Windows.Controls;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Views.StudentStatus
{
    public partial class StudentStatusView : Page
    {
        private List<StudentRegistration> registrations = new List<StudentRegistration>();
        private List<StatusChangeRecord> changes = new List<StatusChangeRecord>();
        private List<ScholarshipInfo> scholarships = new List<ScholarshipInfo>();
        private List<GraduationInfo> graduations = new List<GraduationInfo>();

        public StudentStatusView()
        {
            InitializeComponent();
            LoadSampleData();
            LoadRegistrationData();
        }

        private void LoadSampleData()
        {
            registrations = new List<StudentRegistration>
            {
                new StudentRegistration { Id = 1, StudentId = 1, StudentNo = "2024001", 
                    StudentName = "张三", RegistrationDate = new DateTime(2024, 9, 1), 
                    AcademicYear = "2024-2025", Semester = "第一学期", Status = "正常" }
            };

            changes = new List<StatusChangeRecord>
            {
                new StatusChangeRecord { Id = 1, StudentId = 1, StudentNo = "2024001", 
                    StudentName = "张三", ChangeDate = DateTime.Now.AddMonths(-1), 
                    ChangeType = "转专业", OriginalInfo = "计算机科学与技术", 
                    NewInfo = "软件工程", Reason = "个人意愿", ApprovalStatus = "已批准" }
            };

            scholarships = new List<ScholarshipInfo>
            {
                new ScholarshipInfo { Id = 1, StudentId = 1, StudentNo = "2024001", 
                    StudentName = "张三", AcademicYear = "2024-2025", Semester = "第一学期", 
                    ScholarshipType = "国家奖学金", ScholarshipLevel = "一等", 
                    Amount = 8000, AwardDate = DateTime.Now.AddDays(-10), Status = "已发放" }
            };

            graduations = new List<GraduationInfo>();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
                LoadRegistrationData();
            else if (tabControl.SelectedIndex == 1)
                LoadChangeData();
            else if (tabControl.SelectedIndex == 2)
                LoadScholarshipData();
            else if (tabControl.SelectedIndex == 3)
                LoadGraduationData();
        }

        private void LoadRegistrationData()
        {
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学号", Binding = new System.Windows.Data.Binding("StudentNo"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "姓名", Binding = new System.Windows.Data.Binding("StudentName"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "注册日期", Binding = new System.Windows.Data.Binding("RegistrationDate") { StringFormat = "yyyy-MM-dd" }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学年", Binding = new System.Windows.Data.Binding("AcademicYear"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学期", Binding = new System.Windows.Data.Binding("Semester"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "状态", Binding = new System.Windows.Data.Binding("Status"), Width = new DataGridLength(80) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "备注", Binding = new System.Windows.Data.Binding("Remarks"), Width = new DataGridLength(200) });
            
            dataGrid.ItemsSource = registrations;
        }

        private void LoadChangeData()
        {
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学号", Binding = new System.Windows.Data.Binding("StudentNo"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "姓名", Binding = new System.Windows.Data.Binding("StudentName"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "变动日期", Binding = new System.Windows.Data.Binding("ChangeDate") { StringFormat = "yyyy-MM-dd" }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "变动类型", Binding = new System.Windows.Data.Binding("ChangeType"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "原信息", Binding = new System.Windows.Data.Binding("OriginalInfo"), Width = new DataGridLength(150) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "新信息", Binding = new System.Windows.Data.Binding("NewInfo"), Width = new DataGridLength(150) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "原因", Binding = new System.Windows.Data.Binding("Reason"), Width = new DataGridLength(150) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "审批状态", Binding = new System.Windows.Data.Binding("ApprovalStatus"), Width = new DataGridLength(100) });
            
            dataGrid.ItemsSource = changes;
        }

        private void LoadScholarshipData()
        {
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学号", Binding = new System.Windows.Data.Binding("StudentNo"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "姓名", Binding = new System.Windows.Data.Binding("StudentName"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学年", Binding = new System.Windows.Data.Binding("AcademicYear"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学期", Binding = new System.Windows.Data.Binding("Semester"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "奖学金类型", Binding = new System.Windows.Data.Binding("ScholarshipType"), Width = new DataGridLength(150) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "等级", Binding = new System.Windows.Data.Binding("ScholarshipLevel"), Width = new DataGridLength(80) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "金额", Binding = new System.Windows.Data.Binding("Amount"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "发放日期", Binding = new System.Windows.Data.Binding("AwardDate") { StringFormat = "yyyy-MM-dd" }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "状态", Binding = new System.Windows.Data.Binding("Status"), Width = new DataGridLength(80) });
            
            dataGrid.ItemsSource = scholarships;
        }

        private void LoadGraduationData()
        {
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学号", Binding = new System.Windows.Data.Binding("StudentNo"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "姓名", Binding = new System.Windows.Data.Binding("StudentName"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "毕业日期", Binding = new System.Windows.Data.Binding("GraduationDate") { StringFormat = "yyyy-MM-dd" }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "毕业类型", Binding = new System.Windows.Data.Binding("GraduationType"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学位类型", Binding = new System.Windows.Data.Binding("DegreeType"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "毕业证书号", Binding = new System.Windows.Data.Binding("CertificateNo"), Width = new DataGridLength(150) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学位证书号", Binding = new System.Windows.Data.Binding("DegreeNo"), Width = new DataGridLength(150) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "备注", Binding = new System.Windows.Data.Binding("Remarks"), Width = new DataGridLength(150) });
            
            dataGrid.ItemsSource = graduations;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("新增功能将在后续实现", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                MessageBox.Show("修改功能将在后续实现", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("请先选择要修改的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                var result = MessageBox.Show("确定要删除选中的记录吗？", "确认删除", 
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
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

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            TabControl_SelectionChanged(null!, null!);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("搜索功能将在后续实现", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using StudentsManagerSystem.Models;
using StudentsManagerSystem.Services;

namespace StudentsManagerSystem.Views.StudentArchive
{
    public partial class StudentArchiveView : Page
    {
        private readonly StudentService studentService = new StudentService();
        private readonly System.Collections.ObjectModel.ObservableCollection<Student> students = new System.Collections.ObjectModel.ObservableCollection<Student>();
        private List<FamilyInfo> familyInfos = new List<FamilyInfo>();
        private List<RewardRecord> rewards = new List<RewardRecord>();
        private List<PunishmentRecord> punishments = new List<PunishmentRecord>();
        private List<HealthRecord> healthRecords = new List<HealthRecord>();

        public StudentArchiveView()
        {
            InitializeComponent();
            LoadSampleData();
            LoadStudentData();
        }

        private void LoadSampleData()
        {
            familyInfos = new List<FamilyInfo>
            {
                new FamilyInfo { Id = 1, StudentId = 1, RelationName = "张父", 
                    Relationship = "父亲", PhoneNumber = "13900139001", WorkUnit = "某公司" }
            };

            rewards = new List<RewardRecord>
            {
                new RewardRecord { Id = 1, StudentId = 1, RewardDate = DateTime.Now.AddMonths(-2), 
                    RewardType = "奖学金", RewardLevel = "一等奖", RewardReason = "成绩优异" }
            };

            punishments = new List<PunishmentRecord>();
            healthRecords = new List<HealthRecord>();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
                LoadStudentData();
            else if (tabControl.SelectedIndex == 1)
                LoadFamilyData();
            else if (tabControl.SelectedIndex == 2)
                LoadRewardData();
            else if (tabControl.SelectedIndex == 3)
                LoadPunishmentData();
            else if (tabControl.SelectedIndex == 4)
                LoadHealthData();
        }

        private void LoadStudentData()
        {
            var loadedStudents = string.IsNullOrWhiteSpace(txtSearch.Text)
                ? studentService.GetAll()
                : studentService.Search(txtSearch.Text);

            students.Clear();
            foreach (var student in loadedStudents)
            {
                students.Add(student);
            }

            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学号", Binding = new System.Windows.Data.Binding("StudentNo"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "姓名", Binding = new System.Windows.Data.Binding("Name"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "性别", Binding = new System.Windows.Data.Binding("Gender"), Width = new DataGridLength(60) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "出生日期", Binding = new System.Windows.Data.Binding("BirthDate") { StringFormat = "yyyy-MM-dd" }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "身份证号", Binding = new System.Windows.Data.Binding("IdCard"), Width = new DataGridLength(180) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "民族", Binding = new System.Windows.Data.Binding("Nation"), Width = new DataGridLength(80) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "政治面貌", Binding = new System.Windows.Data.Binding("PoliticalStatus"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "联系电话", Binding = new System.Windows.Data.Binding("PhoneNumber"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "院系", Binding = new System.Windows.Data.Binding("Department"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "专业", Binding = new System.Windows.Data.Binding("Major"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "班级", Binding = new System.Windows.Data.Binding("Class"), Width = new DataGridLength(100) });
            dataGrid.ItemsSource = students;
        }

        private void LoadFamilyData()
        {
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学生ID", Binding = new System.Windows.Data.Binding("StudentId"), Width = new DataGridLength(80) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "关系人姓名", Binding = new System.Windows.Data.Binding("RelationName"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "关系", Binding = new System.Windows.Data.Binding("Relationship"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "联系电话", Binding = new System.Windows.Data.Binding("PhoneNumber"), Width = new DataGridLength(150) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "工作单位", Binding = new System.Windows.Data.Binding("WorkUnit"), Width = new DataGridLength(200) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "地址", Binding = new System.Windows.Data.Binding("Address"), Width = new DataGridLength(250) });
            
            dataGrid.ItemsSource = familyInfos;
        }

        private void LoadRewardData()
        {
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学生ID", Binding = new System.Windows.Data.Binding("StudentId"), Width = new DataGridLength(80) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "奖励日期", Binding = new System.Windows.Data.Binding("RewardDate") { StringFormat = "yyyy-MM-dd" }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "奖励类型", Binding = new System.Windows.Data.Binding("RewardType"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "奖励等级", Binding = new System.Windows.Data.Binding("RewardLevel"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "奖励原因", Binding = new System.Windows.Data.Binding("RewardReason"), Width = new DataGridLength(250) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "颁发单位", Binding = new System.Windows.Data.Binding("RewardUnit"), Width = new DataGridLength(200) });
            
            dataGrid.ItemsSource = rewards;
        }

        private void LoadPunishmentData()
        {
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学生ID", Binding = new System.Windows.Data.Binding("StudentId"), Width = new DataGridLength(80) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "处分日期", Binding = new System.Windows.Data.Binding("PunishmentDate") { StringFormat = "yyyy-MM-dd" }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "处分类型", Binding = new System.Windows.Data.Binding("PunishmentType"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "处分等级", Binding = new System.Windows.Data.Binding("PunishmentLevel"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "处分原因", Binding = new System.Windows.Data.Binding("PunishmentReason"), Width = new DataGridLength(200) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "撤销日期", Binding = new System.Windows.Data.Binding("CancelDate") { StringFormat = "yyyy-MM-dd" }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "状态", Binding = new System.Windows.Data.Binding("Status"), Width = new DataGridLength(100) });
            
            dataGrid.ItemsSource = punishments;
        }

        private void LoadHealthData()
        {
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学生ID", Binding = new System.Windows.Data.Binding("StudentId"), Width = new DataGridLength(80) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "体检日期", Binding = new System.Windows.Data.Binding("CheckDate") { StringFormat = "yyyy-MM-dd" }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "身高(cm)", Binding = new System.Windows.Data.Binding("Height"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "体重(kg)", Binding = new System.Windows.Data.Binding("Weight"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "血型", Binding = new System.Windows.Data.Binding("BloodType"), Width = new DataGridLength(80) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "视力", Binding = new System.Windows.Data.Binding("Vision"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "健康状况", Binding = new System.Windows.Data.Binding("HealthStatus"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "备注", Binding = new System.Windows.Data.Binding("Remarks"), Width = new DataGridLength(200) });
            
            dataGrid.ItemsSource = healthRecords;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != 0)
            {
                MessageBox.Show("当前数据库功能已接入学生基本信息模块。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var window = new StudentEditWindow();
            if (window.ShowDialog() == true && window.ResultStudent != null)
            {
                var saveResult = studentService.Add(window.ResultStudent);
                if (saveResult.Succeeded)
                {
                    MessageBox.Show(saveResult.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadStudentData();
                }
                else
                {
                    MessageBox.Show(saveResult.Message, "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != 0)
            {
                MessageBox.Show("当前数据库功能已接入学生基本信息模块。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (dataGrid.SelectedItem is Student selectedStudent)
            {
                var window = new StudentEditWindow(selectedStudent);
                if (window.ShowDialog() == true && window.ResultStudent != null)
                {
                    var saveResult = studentService.Update(window.ResultStudent);
                    if (saveResult.Succeeded)
                    {
                        MessageBox.Show(saveResult.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadStudentData();
                    }
                    else
                    {
                        MessageBox.Show(saveResult.Message, "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("请先选择要修改的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != 0)
            {
                MessageBox.Show("当前数据库功能已接入学生基本信息模块。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (dataGrid.SelectedItem is Student selectedStudent)
            {
                var result = MessageBox.Show("确定要删除选中的记录吗？", "确认删除", 
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    studentService.Delete(selectedStudent.Id);
                    LoadStudentData();
                }
            }
            else
            {
                MessageBox.Show("请先选择要删除的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != 0)
            {
                MessageBox.Show("当前数据库功能已接入学生基本信息模块。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (dataGrid.SelectedItem is Student selectedStudent)
            {
                var window = new StudentDetailWindow(selectedStudent);
                window.ShowDialog();
            }
            else
            {
                MessageBox.Show("请先选择要查看的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            TabControl_SelectionChanged(null!, null!);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                LoadStudentData();
                return;
            }

            MessageBox.Show("当前仅为学生基本信息模块接入了数据库搜索。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Views.StudentArchive
{
    public partial class StudentArchiveView : Page
    {
        private List<Student> students = new List<Student>();
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
            // 示例数据
            students = new List<Student>
            {
                new Student { Id = 1, StudentNo = "2024001", Name = "张三", Gender = "男", 
                    BirthDate = new DateTime(2005, 3, 15), IdCard = "110101200503151234", 
                    Nation = "汉族", PoliticalStatus = "团员", PhoneNumber = "13800138001", 
                    Department = "计算机学院", Major = "软件工程", Class = "软工2024-1班" },
                new Student { Id = 2, StudentNo = "2024002", Name = "李四", Gender = "女", 
                    BirthDate = new DateTime(2005, 6, 20), IdCard = "110101200506201234", 
                    Nation = "汉族", PoliticalStatus = "群众", PhoneNumber = "13800138002", 
                    Department = "计算机学院", Major = "软件工程", Class = "软工2024-1班" }
            };

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
            var window = new StudentEditWindow();
            window.ShowDialog();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                var window = new StudentEditWindow();
                window.ShowDialog();
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
                var window = new StudentDetailWindow();
                window.ShowDialog();
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

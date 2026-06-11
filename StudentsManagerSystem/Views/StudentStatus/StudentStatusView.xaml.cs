using System.Windows;
using System.Windows.Controls;
using StudentsManagerSystem.Common;
using StudentsManagerSystem.Data.SqlServer;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Views.StudentStatus
{
    public partial class StudentStatusView : Page
    {
        private readonly StudentStatusRepository repository = new StudentStatusRepository();

        public StudentStatusView()
        {
            InitializeComponent();
            LoadCurrentTabData();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadCurrentTabData();
        }

        private void LoadCurrentTabData()
        {
            if (tabControl.SelectedIndex == 0)
            {
                LoadRegistrationData();
            }
            else if (tabControl.SelectedIndex == 1)
            {
                LoadChangeData();
            }
            else if (tabControl.SelectedIndex == 2)
            {
                LoadScholarshipData();
            }
            else if (tabControl.SelectedIndex == 3)
            {
                LoadGraduationData();
            }
        }

        private string GetKeyword() => txtSearch.Text.Trim();

        private void LoadRegistrationData()
        {
            var keyword = GetKeyword();
            var items = string.IsNullOrWhiteSpace(keyword)
                ? repository.GetRegistrations()
                : repository.SearchRegistrations(keyword);

            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();

            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学号", Binding = new System.Windows.Data.Binding("StudentNo"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "姓名", Binding = new System.Windows.Data.Binding("StudentName"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "注册日期", Binding = new System.Windows.Data.Binding("RegistrationDate") { StringFormat = "yyyy-MM-dd" }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学年", Binding = new System.Windows.Data.Binding("AcademicYear") { Converter = new AcademicYearDisplayConverter() }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学期", Binding = new System.Windows.Data.Binding("Semester"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "状态", Binding = new System.Windows.Data.Binding("Status"), Width = new DataGridLength(80) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "备注", Binding = new System.Windows.Data.Binding("Remarks"), Width = new DataGridLength(200) });

            dataGrid.ItemsSource = items;
        }

        private void LoadChangeData()
        {
            var keyword = GetKeyword();
            var items = string.IsNullOrWhiteSpace(keyword)
                ? repository.GetChanges()
                : repository.SearchChanges(keyword);

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

            dataGrid.ItemsSource = items;
        }

        private void LoadScholarshipData()
        {
            var keyword = GetKeyword();
            var items = string.IsNullOrWhiteSpace(keyword)
                ? repository.GetScholarships()
                : repository.SearchScholarships(keyword);

            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();

            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学号", Binding = new System.Windows.Data.Binding("StudentNo"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "姓名", Binding = new System.Windows.Data.Binding("StudentName"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学年", Binding = new System.Windows.Data.Binding("AcademicYear") { Converter = new AcademicYearDisplayConverter() }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学期", Binding = new System.Windows.Data.Binding("Semester"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "奖学金类型", Binding = new System.Windows.Data.Binding("ScholarshipType"), Width = new DataGridLength(150) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "等级", Binding = new System.Windows.Data.Binding("ScholarshipLevel"), Width = new DataGridLength(80) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "金额", Binding = new System.Windows.Data.Binding("Amount"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "发放日期", Binding = new System.Windows.Data.Binding("AwardDate") { StringFormat = "yyyy-MM-dd" }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "状态", Binding = new System.Windows.Data.Binding("Status"), Width = new DataGridLength(80) });

            dataGrid.ItemsSource = items;
        }

        private void LoadGraduationData()
        {
            var keyword = GetKeyword();
            var items = string.IsNullOrWhiteSpace(keyword)
                ? repository.GetGraduations()
                : repository.SearchGraduations(keyword);

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

            dataGrid.ItemsSource = items;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                var win = new RegistrationEditWindow { Owner = Window.GetWindow(this) };
                if (win.ShowDialog() == true) LoadCurrentTabData();
            }
            else if (tabControl.SelectedIndex == 1)
            {
                var win = new StatusChangeEditWindow { Owner = Window.GetWindow(this) };
                if (win.ShowDialog() == true) LoadCurrentTabData();
            }
            else if (tabControl.SelectedIndex == 2)
            {
                var win = new ScholarshipEditWindow { Owner = Window.GetWindow(this) };
                if (win.ShowDialog() == true) LoadCurrentTabData();
            }
            else if (tabControl.SelectedIndex == 3)
            {
                var win = new GraduationEditWindow { Owner = Window.GetWindow(this) };
                if (win.ShowDialog() == true) LoadCurrentTabData();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("请先选择要修改的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (tabControl.SelectedIndex == 0 && dataGrid.SelectedItem is StudentRegistration registration)
            {
                var win = new RegistrationEditWindow(registration);
                win.Owner = Window.GetWindow(this);
                if (win.ShowDialog() == true)
                {
                    LoadCurrentTabData();
                }
            }
            else if (tabControl.SelectedIndex == 1 && dataGrid.SelectedItem is StatusChangeRecord change)
            {
                var win = new StatusChangeEditWindow(change) { Owner = Window.GetWindow(this) };
                if (win.ShowDialog() == true) LoadCurrentTabData();
            }
            else if (tabControl.SelectedIndex == 2 && dataGrid.SelectedItem is ScholarshipInfo scholarship)
            {
                var win = new ScholarshipEditWindow(scholarship) { Owner = Window.GetWindow(this) };
                if (win.ShowDialog() == true) LoadCurrentTabData();
            }
            else if (tabControl.SelectedIndex == 3 && dataGrid.SelectedItem is GraduationInfo graduation)
            {
                var win = new GraduationEditWindow(graduation) { Owner = Window.GetWindow(this) };
                if (win.ShowDialog() == true) LoadCurrentTabData();
            }
            else
            {
                MessageBox.Show("非学籍登记记录的编辑暂未实现", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("请先选择要删除的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show("确定要删除选中的记录吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            if (tabControl.SelectedIndex == 0 && dataGrid.SelectedItem is StudentRegistration registration)
            {
                repository.DeleteRegistration(registration.Id);
            }
            else if (tabControl.SelectedIndex == 1 && dataGrid.SelectedItem is StatusChangeRecord change)
            {
                repository.DeleteChange(change.Id);
            }
            else if (tabControl.SelectedIndex == 2 && dataGrid.SelectedItem is ScholarshipInfo scholarship)
            {
                repository.DeleteScholarship(scholarship.Id);
            }
            else if (tabControl.SelectedIndex == 3 && dataGrid.SelectedItem is GraduationInfo graduation)
            {
                repository.DeleteGraduation(graduation.Id);
            }

            LoadCurrentTabData();
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("请先选择要查看的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (tabControl.SelectedIndex == 0 && dataGrid.SelectedItem is StudentRegistration reg)
            {
                var win = new RegistrationEditWindow(reg);
                win.Owner = Window.GetWindow(this);
                // 只读查看
                win.MakeReadOnly();
                win.ShowDialog();
            }
            else if (tabControl.SelectedIndex == 1 && dataGrid.SelectedItem is StatusChangeRecord change)
            {
                var win = new StatusChangeEditWindow(change) { Owner = Window.GetWindow(this) };
                win.MakeReadOnly();
                win.ShowDialog();
            }
            else if (tabControl.SelectedIndex == 2 && dataGrid.SelectedItem is ScholarshipInfo scholarship)
            {
                var win = new ScholarshipEditWindow(scholarship) { Owner = Window.GetWindow(this) };
                win.MakeReadOnly();
                win.ShowDialog();
            }
            else if (tabControl.SelectedIndex == 3 && dataGrid.SelectedItem is GraduationInfo graduation)
            {
                var win = new GraduationEditWindow(graduation) { Owner = Window.GetWindow(this) };
                win.MakeReadOnly();
                win.ShowDialog();
            }
            else
            {
                MessageBox.Show("详细查看窗体暂未实现。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadCurrentTabData();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadCurrentTabData();
        }
    }
}
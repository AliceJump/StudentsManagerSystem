using System.Windows;
using System.Windows.Controls;
using StudentsManagerSystem.Models;
using StudentsManagerSystem.Services;

namespace StudentsManagerSystem.Views.BasicData
{
    public partial class BasicDataView : Page
    {
        private readonly BasicDataService basicDataService = new BasicDataService();
        private List<Department> departments = new List<Department>();
        private List<Major> majors = new List<Major>();
        private List<Models.Class> classes = new List<Models.Class>();

        public BasicDataView()
        {
            InitializeComponent();
            LoadData();
            LoadDepartmentData();
        }

        private void LoadData()
        {
            departments = basicDataService.GetDepartments();
            majors = basicDataService.GetMajors();
            classes = basicDataService.GetClasses();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
                LoadDepartmentData();
            else if (tabControl.SelectedIndex == 1)
                LoadMajorData();
            else if (tabControl.SelectedIndex == 2)
                LoadClassData();
        }

        private void LoadDepartmentData()
        {
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "院系编号", Binding = new System.Windows.Data.Binding("DepartmentNo"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "院系名称", Binding = new System.Windows.Data.Binding("DepartmentName"), Width = new DataGridLength(200) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "院系负责人", Binding = new System.Windows.Data.Binding("DepartmentHead"), Width = new DataGridLength(150) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "联系电话", Binding = new System.Windows.Data.Binding("PhoneNumber"), Width = new DataGridLength(150) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "备注", Binding = new System.Windows.Data.Binding("Remarks"), Width = new DataGridLength(250) });
            
            dataGrid.ItemsSource = departments;
        }

        private void LoadMajorData()
        {
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "专业编号", Binding = new System.Windows.Data.Binding("MajorNo"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "专业名称", Binding = new System.Windows.Data.Binding("MajorName"), Width = new DataGridLength(200) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "所属院系", Binding = new System.Windows.Data.Binding("DepartmentName"), Width = new DataGridLength(150) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学制(年)", Binding = new System.Windows.Data.Binding("Duration"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学位类型", Binding = new System.Windows.Data.Binding("DegreeType"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "备注", Binding = new System.Windows.Data.Binding("Remarks"), Width = new DataGridLength(200) });
            
            dataGrid.ItemsSource = majors;
        }

        private void LoadClassData()
        {
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "班级编号", Binding = new System.Windows.Data.Binding("ClassNo"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "班级名称", Binding = new System.Windows.Data.Binding("ClassName"), Width = new DataGridLength(150) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "所属院系", Binding = new System.Windows.Data.Binding("DepartmentName"), Width = new DataGridLength(150) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "所属专业", Binding = new System.Windows.Data.Binding("MajorName"), Width = new DataGridLength(150) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "年级", Binding = new System.Windows.Data.Binding("Grade"), Width = new DataGridLength(80) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "班主任", Binding = new System.Windows.Data.Binding("ClassTeacher"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学生人数", Binding = new System.Windows.Data.Binding("StudentCount"), Width = new DataGridLength(100) });
            
            dataGrid.ItemsSource = classes;
        }

        private BasicDataItemKind GetCurrentKind()
        {
            return tabControl.SelectedIndex switch
            {
                0 => BasicDataItemKind.Department,
                1 => BasicDataItemKind.Major,
                _ => BasicDataItemKind.Class
            };
        }

        private string GetCurrentTitle(string action)
        {
            return GetCurrentKind() switch
            {
                BasicDataItemKind.Department => $"{action}院系",
                BasicDataItemKind.Major => $"{action}专业",
                _ => $"{action}班级"
            };
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var kind = GetCurrentKind();
            var window = new BasicDataEditWindow(kind, GetCurrentTitle("新增"));
            if (window.ShowDialog() == true && window.ResultEntity != null)
            {
                if (window.ResultEntity is Department department)
                {
                    var saveResult = basicDataService.AddDepartment(department);
                    if (!saveResult.Succeeded) { MessageBox.Show(saveResult.Message, "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
                }
                else if (window.ResultEntity is Major major)
                {
                    var saveResult = basicDataService.AddMajor(major);
                    if (!saveResult.Succeeded) { MessageBox.Show(saveResult.Message, "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
                }
                else if (window.ResultEntity is Models.Class classInfo)
                {
                    var saveResult = basicDataService.AddClass(classInfo);
                    if (!saveResult.Succeeded) { MessageBox.Show(saveResult.Message, "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
                }

                LoadData();
                TabControl_SelectionChanged(null!, null!);
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                var kind = GetCurrentKind();
                var window = new BasicDataEditWindow(kind, GetCurrentTitle("修改"), dataGrid.SelectedItem);
                if (window.ShowDialog() == true && window.ResultEntity != null)
                {
                    if (window.ResultEntity is Department department)
                    {
                        var saveResult = basicDataService.UpdateDepartment(department);
                        if (!saveResult.Succeeded) { MessageBox.Show(saveResult.Message, "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
                    }
                    else if (window.ResultEntity is Major major)
                    {
                        var saveResult = basicDataService.UpdateMajor(major);
                        if (!saveResult.Succeeded) { MessageBox.Show(saveResult.Message, "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
                    }
                    else if (window.ResultEntity is Models.Class classInfo)
                    {
                        var saveResult = basicDataService.UpdateClass(classInfo);
                        if (!saveResult.Succeeded) { MessageBox.Show(saveResult.Message, "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
                    }

                    LoadData();
                    TabControl_SelectionChanged(null!, null!);
                }
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
                    switch (GetCurrentKind())
                    {
                        case BasicDataItemKind.Department:
                            basicDataService.DeleteDepartment(((Department)dataGrid.SelectedItem).Id);
                            break;
                        case BasicDataItemKind.Major:
                            basicDataService.DeleteMajor(((Major)dataGrid.SelectedItem).Id);
                            break;
                        default:
                            basicDataService.DeleteClass(((Models.Class)dataGrid.SelectedItem).Id);
                            break;
                    }

                    LoadData();
                    TabControl_SelectionChanged(null!, null!);
                }
            }
            else
            {
                MessageBox.Show("请先选择要删除的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            TabControl_SelectionChanged(null!, null!);
        }
    }
}

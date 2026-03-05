using System.Windows;
using System.Windows.Controls;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Views.BasicData
{
    public partial class BasicDataView : Page
    {
        private List<Department> departments = new List<Department>();
        private List<Major> majors = new List<Major>();
        private List<Models.Class> classes = new List<Models.Class>();

        public BasicDataView()
        {
            InitializeComponent();
            LoadSampleData();
            LoadDepartmentData();
        }

        private void LoadSampleData()
        {
            departments = new List<Department>
            {
                new Department { Id = 1, DepartmentNo = "D001", DepartmentName = "计算机学院", 
                    DepartmentHead = "王教授", PhoneNumber = "010-12345678" },
                new Department { Id = 2, DepartmentNo = "D002", DepartmentName = "电子信息学院", 
                    DepartmentHead = "李教授", PhoneNumber = "010-12345679" },
                new Department { Id = 3, DepartmentNo = "D003", DepartmentName = "机械工程学院", 
                    DepartmentHead = "张教授", PhoneNumber = "010-12345680" }
            };

            majors = new List<Major>
            {
                new Major { Id = 1, MajorNo = "M001", MajorName = "软件工程", 
                    DepartmentName = "计算机学院", Duration = 4, DegreeType = "工学学士" },
                new Major { Id = 2, MajorNo = "M002", MajorName = "计算机科学与技术", 
                    DepartmentName = "计算机学院", Duration = 4, DegreeType = "工学学士" },
                new Major { Id = 3, MajorNo = "M003", MajorName = "网络工程", 
                    DepartmentName = "计算机学院", Duration = 4, DegreeType = "工学学士" }
            };

            classes = new List<Models.Class>
            {
                new Models.Class { Id = 1, ClassNo = "C001", ClassName = "软工2024-1班", 
                    DepartmentName = "计算机学院", MajorName = "软件工程", 
                    Grade = "2024", ClassTeacher = "赵老师", StudentCount = 45 },
                new Models.Class { Id = 2, ClassNo = "C002", ClassName = "软工2024-2班", 
                    DepartmentName = "计算机学院", MajorName = "软件工程", 
                    Grade = "2024", ClassTeacher = "钱老师", StudentCount = 48 }
            };
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string title = "";
            if (tabControl.SelectedIndex == 0)
                title = "新增院系";
            else if (tabControl.SelectedIndex == 1)
                title = "新增专业";
            else if (tabControl.SelectedIndex == 2)
                title = "新增班级";

            var window = new BasicDataEditWindow(title);
            window.ShowDialog();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                string title = "";
                if (tabControl.SelectedIndex == 0)
                    title = "修改院系";
                else if (tabControl.SelectedIndex == 1)
                    title = "修改专业";
                else if (tabControl.SelectedIndex == 2)
                    title = "修改班级";

                var window = new BasicDataEditWindow(title);
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

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            TabControl_SelectionChanged(null!, null!);
        }
    }
}

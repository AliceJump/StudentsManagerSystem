using StudentsManagerSystem.Models;
using System.Windows;

namespace StudentsManagerSystem.Views.BasicData
{
    public enum BasicDataItemKind
    {
        Department,
        Major,
        Class
    }

    public partial class BasicDataEditWindow : Window
    {
        private readonly BasicDataItemKind itemKind;
        private readonly int editingId;

        public object? ResultEntity { get; private set; }

        public BasicDataEditWindow(BasicDataItemKind kind, string title, object? item = null)
        {
            InitializeComponent();
            itemKind = kind;
            txtTitle.Text = title;
            this.Title = title;
            ConfigurePanels();

            if (item is Department department)
            {
                editingId = department.Id;
                txtNo.Text = department.DepartmentNo;
                txtName.Text = department.DepartmentName;
                txtHead.Text = department.DepartmentHead;
                txtPhone.Text = department.PhoneNumber;
                txtRemarks.Text = department.Remarks;
            }
            else if (item is Major major)
            {
                editingId = major.Id;
                txtMajorNo.Text = major.MajorNo;
                txtMajorName.Text = major.MajorName;
                txtMajorDepartment.Text = major.DepartmentName;
                txtDuration.Text = major.Duration.ToString();
                txtDegreeType.Text = major.DegreeType;
                txtMajorRemarks.Text = major.Remarks;
            }
            else if (item is Class classInfo)
            {
                editingId = classInfo.Id;
                txtClassNo.Text = classInfo.ClassNo;
                txtClassName.Text = classInfo.ClassName;
                txtClassDepartment.Text = classInfo.DepartmentName;
                txtClassMajor.Text = classInfo.MajorName;
                txtGrade.Text = classInfo.Grade;
                txtClassTeacher.Text = classInfo.ClassTeacher;
                txtStudentCount.Text = classInfo.StudentCount.ToString();
                txtClassRemarks.Text = classInfo.Remarks;
            }
        }

        private void ConfigurePanels()
        {
            departmentPanel.Visibility = itemKind == BasicDataItemKind.Department ? Visibility.Visible : Visibility.Collapsed;
            majorPanel.Visibility = itemKind == BasicDataItemKind.Major ? Visibility.Visible : Visibility.Collapsed;
            classPanel.Visibility = itemKind == BasicDataItemKind.Class ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (itemKind == BasicDataItemKind.Department)
            {
                if (string.IsNullOrWhiteSpace(txtNo.Text) || string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("请填写所有必填项（标*的字段）！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                ResultEntity = new Department
                {
                    Id = editingId,
                    DepartmentNo = txtNo.Text.Trim(),
                    DepartmentName = txtName.Text.Trim(),
                    DepartmentHead = txtHead.Text.Trim(),
                    PhoneNumber = txtPhone.Text.Trim(),
                    Remarks = txtRemarks.Text.Trim()
                };
            }
            else if (itemKind == BasicDataItemKind.Major)
            {
                if (string.IsNullOrWhiteSpace(txtMajorNo.Text) ||
                    string.IsNullOrWhiteSpace(txtMajorName.Text) ||
                    string.IsNullOrWhiteSpace(txtMajorDepartment.Text) ||
                    string.IsNullOrWhiteSpace(txtDuration.Text) ||
                    string.IsNullOrWhiteSpace(txtDegreeType.Text) ||
                    !int.TryParse(txtDuration.Text.Trim(), out var duration))
                {
                    MessageBox.Show("请填写所有必填项（标*的字段），且学制必须是数字！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                ResultEntity = new Major
                {
                    Id = editingId,
                    MajorNo = txtMajorNo.Text.Trim(),
                    MajorName = txtMajorName.Text.Trim(),
                    DepartmentName = txtMajorDepartment.Text.Trim(),
                    Duration = duration,
                    DegreeType = txtDegreeType.Text.Trim(),
                    Remarks = txtMajorRemarks.Text.Trim()
                };
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtClassNo.Text) ||
                    string.IsNullOrWhiteSpace(txtClassName.Text) ||
                    string.IsNullOrWhiteSpace(txtClassDepartment.Text) ||
                    string.IsNullOrWhiteSpace(txtClassMajor.Text) ||
                    string.IsNullOrWhiteSpace(txtGrade.Text) ||
                    string.IsNullOrWhiteSpace(txtStudentCount.Text) ||
                    !int.TryParse(txtStudentCount.Text.Trim(), out var studentCount))
                {
                    MessageBox.Show("请填写所有必填项（标*的字段），且学生人数必须是数字！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                ResultEntity = new Class
                {
                    Id = editingId,
                    ClassNo = txtClassNo.Text.Trim(),
                    ClassName = txtClassName.Text.Trim(),
                    DepartmentName = txtClassDepartment.Text.Trim(),
                    MajorName = txtClassMajor.Text.Trim(),
                    Grade = txtGrade.Text.Trim(),
                    ClassTeacher = txtClassTeacher.Text.Trim(),
                    StudentCount = studentCount,
                    Remarks = txtClassRemarks.Text.Trim()
                };
            }

            MessageBox.Show("保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

using StudentsManagerSystem.Models;
using System.Windows;

namespace StudentsManagerSystem.Views.StudentArchive
{
    public partial class StudentEditWindow : Window
    {
        private readonly int editingStudentId;

        public Student? ResultStudent { get; private set; }

        public StudentEditWindow() : this(null)
        {
        }

        public StudentEditWindow(Student? student)
        {
            InitializeComponent();

            if (student != null)
            {
                editingStudentId = student.Id;
                txtStudentNo.Text = student.StudentNo;
                txtName.Text = student.Name;
                SetComboBoxSelectedText(cmbGender, student.Gender);
                dpBirthDate.SelectedDate = student.BirthDate;
                txtIdCard.Text = student.IdCard;
                txtNation.Text = student.Nation;
                SetComboBoxSelectedText(cmbPoliticalStatus, student.PoliticalStatus);
                txtPhone.Text = student.PhoneNumber;
                txtEmail.Text = student.Email;
                dpEnrollment.SelectedDate = student.EnrollmentDate;
                SetComboBoxSelectedText(cmbDepartment, student.Department);
                SetComboBoxSelectedText(cmbMajor, student.Major);
                SetComboBoxSelectedText(cmbClass, student.Class);
                txtAddress.Text = student.Address;
            }
        }

        private static void SetComboBoxSelectedText(System.Windows.Controls.ComboBox comboBox, string text)
        {
            foreach (var item in comboBox.Items)
            {
                if (item is System.Windows.Controls.ComboBoxItem comboBoxItem &&
                    string.Equals(comboBoxItem.Content?.ToString(), text, StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedItem = comboBoxItem;
                    return;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStudentNo.Text) ||
                string.IsNullOrWhiteSpace(txtName.Text) ||
                cmbGender.SelectedItem == null ||
                dpBirthDate.SelectedDate == null ||
                string.IsNullOrWhiteSpace(txtIdCard.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                cmbDepartment.SelectedItem == null ||
                cmbMajor.SelectedItem == null ||
                cmbClass.SelectedItem == null)
            {
                MessageBox.Show("请填写所有必填项（标*的字段）！", "提示", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ResultStudent = new Student
            {
                Id = editingStudentId,
                StudentNo = txtStudentNo.Text.Trim(),
                Name = txtName.Text.Trim(),
                Gender = ((System.Windows.Controls.ComboBoxItem)cmbGender.SelectedItem).Content?.ToString() ?? string.Empty,
                BirthDate = dpBirthDate.SelectedDate,
                IdCard = txtIdCard.Text.Trim(),
                Nation = txtNation.Text.Trim(),
                PoliticalStatus = cmbPoliticalStatus.SelectedItem is System.Windows.Controls.ComboBoxItem politicalStatusItem ? politicalStatusItem.Content?.ToString() ?? string.Empty : string.Empty,
                PhoneNumber = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                Department = cmbDepartment.SelectedItem is System.Windows.Controls.ComboBoxItem departmentItem ? departmentItem.Content?.ToString() ?? string.Empty : string.Empty,
                Major = cmbMajor.SelectedItem is System.Windows.Controls.ComboBoxItem majorItem ? majorItem.Content?.ToString() ?? string.Empty : string.Empty,
                Class = cmbClass.SelectedItem is System.Windows.Controls.ComboBoxItem classItem ? classItem.Content?.ToString() ?? string.Empty : string.Empty,
                EnrollmentDate = dpEnrollment.SelectedDate,
                Photo = string.Empty
            };

            MessageBox.Show("保存成功！", "提示", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

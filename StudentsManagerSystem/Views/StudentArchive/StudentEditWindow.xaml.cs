using StudentsManagerSystem.Models;
using System.Windows;
using StudentsManagerSystem.Services;

namespace StudentsManagerSystem.Views.StudentArchive
{
    public partial class StudentEditWindow : Window
    {
        private readonly int editingStudentId;
        private readonly StudentService studentService = new StudentService();
        private readonly BasicDataService basicDataService = new BasicDataService();

        public Student? ResultStudent { get; private set; }

        public StudentEditWindow() : this(null)
        {
        }

        public StudentEditWindow(Student? student)
        {
            InitializeComponent();
            LoadBusinessOptions();

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
            comboBox.SelectedItem = comboBox.Items.Cast<object?>()
                .FirstOrDefault(item => string.Equals(GetComboItemText(item), text, StringComparison.OrdinalIgnoreCase));
        }

        private void LoadBusinessOptions()
        {
            cmbGender.ItemsSource = basicDataService.GetLookupValues("Gender");
            cmbPoliticalStatus.ItemsSource = basicDataService.GetLookupValues("PoliticalStatus");
            cmbDepartment.ItemsSource = basicDataService.GetDepartmentNames();
            cmbMajor.ItemsSource = basicDataService.GetMajorNames();
            cmbClass.ItemsSource = basicDataService.GetClassNames();

            SelectFirstIfAny(cmbGender);
            SelectFirstIfAny(cmbPoliticalStatus);
            SelectFirstIfAny(cmbDepartment);
            SelectFirstIfAny(cmbMajor);
            SelectFirstIfAny(cmbClass);
        }

        private void cmbDepartment_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var dept = GetComboItemText(cmbDepartment.SelectedItem);
            cmbClass.ItemsSource = basicDataService.GetClassNames(dept);
            cmbClass.SelectedIndex = cmbClass.Items.Count > 0 ? 0 : -1;
        }

        private static void SelectFirstIfAny(System.Windows.Controls.ComboBox comboBox)
        {
            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }

        private static string GetComboItemText(object? item) => item is System.Windows.Controls.ComboBoxItem comboBoxItem
            ? comboBoxItem.Content?.ToString() ?? string.Empty
            : item?.ToString() ?? string.Empty;

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

            var studentNo = txtStudentNo.Text.Trim();
            var name = txtName.Text.Trim();
            var idCard = txtIdCard.Text.Trim();
            var phone = txtPhone.Text.Trim();
            var email = txtEmail.Text.Trim();

            ResultStudent = new Student
            {
                Id = editingStudentId,
                StudentNo = studentNo,
                Name = name,
                Gender = GetComboItemText(cmbGender.SelectedItem),
                BirthDate = dpBirthDate.SelectedDate,
                IdCard = idCard,
                Nation = txtNation.Text.Trim(),
                PoliticalStatus = GetComboItemText(cmbPoliticalStatus.SelectedItem),
                PhoneNumber = phone,
                Email = email,
                Address = txtAddress.Text.Trim(),
                Department = GetComboItemText(cmbDepartment.SelectedItem),
                Major = GetComboItemText(cmbMajor.SelectedItem),
                Class = GetComboItemText(cmbClass.SelectedItem),
                EnrollmentDate = dpEnrollment.SelectedDate,
                Photo = string.Empty
            };

            var validation = studentService.Validate(ResultStudent, editingStudentId);
            if (!validation.Succeeded)
            {
                MessageBox.Show(validation.Message, "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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

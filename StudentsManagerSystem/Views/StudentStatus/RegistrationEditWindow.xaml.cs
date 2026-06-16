using System;
using System.Windows;
using StudentsManagerSystem.Common;
using StudentsManagerSystem.Models;
using StudentsManagerSystem.Services;

namespace StudentsManagerSystem.Views.StudentStatus
{
    public partial class RegistrationEditWindow : Window
    {
        private readonly StudentStatusService studentStatusService = new StudentStatusService();
        private readonly BasicDataService basicDataService = new BasicDataService();
        private StudentRegistration model;

        public StudentRegistration Result => model;

        public RegistrationEditWindow(StudentRegistration? existing = null)
        {
            InitializeComponent();
            LoadBusinessOptions();
            model = existing != null ? Clone(existing) : new StudentRegistration { RegistrationDate = DateTime.Now };
            BindModelToControls();
        }

        private void LoadBusinessOptions()
        {
            cmbSemester.ItemsSource = basicDataService.GetLookupValues("Semester");
            if (cmbSemester.Items.Count > 0)
            {
                cmbSemester.SelectedIndex = 0;
            }
        }

        public void MakeReadOnly()
        {
            btnOk.Visibility = Visibility.Collapsed;
            txtStudentNo.IsReadOnly = true;
            txtStudentName.IsReadOnly = true;
            dpRegistrationDate.IsEnabled = false;
            txtAcademicYear.IsReadOnly = true;
            cmbSemester.IsEnabled = false;
            txtRemarks.IsReadOnly = true;
        }

        private StudentRegistration Clone(StudentRegistration src)
        {
            return new StudentRegistration
            {
                Id = src.Id,
                StudentId = src.StudentId,
                StudentNo = src.StudentNo,
                StudentName = src.StudentName,
                RegistrationDate = src.RegistrationDate,
                AcademicYear = src.AcademicYear,
                Semester = src.Semester,
                Status = src.Status,
                Remarks = src.Remarks
            };
        }

        private void BindModelToControls()
        {
            txtStudentNo.Text = model.StudentNo;
            txtStudentName.Text = model.StudentName;
            dpRegistrationDate.SelectedDate = model.RegistrationDate ?? DateTime.Now;
            txtAcademicYear.Text = AcademicYearHelper.NormalizeStartYear(model.AcademicYear);
            cmbSemester.SelectedItem = NormalizeSemester(model.Semester);
            txtRemarks.Text = model.Remarks;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            var no = txtStudentNo.Text.Trim();
            if (!InputValidator.ValidateStudentNo(no))
            {
                MessageBox.Show("学号格式错误", "验证", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtStudentName.Text) || !InputValidator.ValidateName(txtStudentName.Text.Trim()))
            {
                MessageBox.Show("姓名格式错误", "验证", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (dpRegistrationDate.SelectedDate == null)
            {
                MessageBox.Show("请选择注册日期", "验证", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!InputValidator.ValidateAcademicYear(txtAcademicYear.Text.Trim()))
            {
                MessageBox.Show("学年格式错误，应输入4位起始学年，例如 2024", "验证", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!InputValidator.ValidateSemester(cmbSemester.SelectedItem?.ToString() ?? string.Empty))
            {
                MessageBox.Show("学期格式错误", "验证", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            model.StudentNo = no;
            model.StudentName = txtStudentName.Text.Trim();
            model.RegistrationDate = dpRegistrationDate.SelectedDate;
            model.AcademicYear = AcademicYearHelper.NormalizeStartYear(txtAcademicYear.Text.Trim());
            model.Semester = NormalizeSemester(cmbSemester.SelectedItem?.ToString() ?? string.Empty);
            model.Remarks = txtRemarks.Text.Trim();

            try
            {
                var result = studentStatusService.SaveRegistration(model);
                if (!result.Succeeded)
                {
                    MessageBox.Show(result.Message, "验证", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                model.Id = result.Data;

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private static string NormalizeSemester(string semester)
        {
            return semester switch
            {
                "1" => "第一学期",
                "2" => "第二学期",
                "第一学期" => "第一学期",
                "第二学期" => "第二学期",
                _ => semester
            };
        }
    }
}

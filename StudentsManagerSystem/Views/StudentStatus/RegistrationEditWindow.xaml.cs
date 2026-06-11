using System;
using System.Windows;
using StudentsManagerSystem.Common;
using StudentsManagerSystem.Data.SqlServer;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Views.StudentStatus
{
    public partial class RegistrationEditWindow : Window
    {
        private readonly StudentStatusRepository repository = new StudentStatusRepository();
        private StudentRegistration model;

        public StudentRegistration Result => model;

        public RegistrationEditWindow(StudentRegistration? existing = null)
        {
            InitializeComponent();
            cmbSemester.ItemsSource = new[] { "1", "2" };
            model = existing != null ? Clone(existing) : new StudentRegistration { RegistrationDate = DateTime.Now };
            BindModelToControls();
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
            cmbSemester.SelectedItem = string.IsNullOrEmpty(model.Semester) ? "1" : model.Semester;
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
            model.Semester = cmbSemester.SelectedItem?.ToString() ?? string.Empty;
            model.Remarks = txtRemarks.Text.Trim();

            try
            {
                if (model.Id != 0 && !repository.RegistrationExists(model.Id))
                {
                    MessageBox.Show("当前记录已不存在，请刷新后重试", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                if (repository.RegistrationDuplicateExists(model))
                {
                    MessageBox.Show("该学号已经存在登记记录", "验证", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (model.Id == 0)
                {
                    var id = repository.AddRegistration(model);
                    model.Id = id;
                }
                else
                {
                    repository.UpdateRegistration(model);
                }

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
    }
}

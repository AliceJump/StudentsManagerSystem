using System;
using System.Windows;
using StudentsManagerSystem.Common;
using StudentsManagerSystem.Data.SqlServer;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Views.StudentStatus
{
    public partial class ScholarshipEditWindow : Window
    {
        private readonly StudentStatusRepository repository = new StudentStatusRepository();
        private ScholarshipInfo model; public ScholarshipInfo Result => model;
        public ScholarshipEditWindow(ScholarshipInfo? existing = null){ InitializeComponent(); model = existing != null ? new ScholarshipInfo { Id=existing.Id, StudentId=existing.StudentId, StudentNo=existing.StudentNo, StudentName=existing.StudentName, AcademicYear=existing.AcademicYear, Semester=existing.Semester, ScholarshipType=existing.ScholarshipType, ScholarshipLevel=existing.ScholarshipLevel, Amount=existing.Amount, AwardDate=existing.AwardDate, Status=existing.Status } : new ScholarshipInfo { AwardDate=DateTime.Now, Status="待发放" }; txtStudentNo.Text=model.StudentNo; txtStudentName.Text=model.StudentName; txtAcademicYear.Text=AcademicYearHelper.NormalizeStartYear(model.AcademicYear); cmbSemester.SelectedIndex = model.Semester == "第二学期" ? 1 : 0; txtType.Text=model.ScholarshipType; txtLevel.Text=model.ScholarshipLevel; txtAmount.Text=model.Amount.ToString(); dpAwardDate.SelectedDate=model.AwardDate ?? DateTime.Now; cmbStatus.SelectedIndex = model.Status == "已发放" ? 1 : 0; }
        public void MakeReadOnly(){ btnOk.Visibility=Visibility.Collapsed; txtStudentNo.IsReadOnly=txtStudentName.IsReadOnly=txtAcademicYear.IsReadOnly=txtType.IsReadOnly=txtLevel.IsReadOnly=txtAmount.IsReadOnly=true; cmbSemester.IsEnabled=dpAwardDate.IsEnabled=cmbStatus.IsEnabled=false; }
        private static string ComboContent(System.Windows.Controls.ComboBox comboBox) => (comboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? string.Empty;
        private void btnOk_Click(object sender,RoutedEventArgs e){ var semester = ComboContent(cmbSemester); var status = ComboContent(cmbStatus); var academicYear = AcademicYearHelper.NormalizeStartYear(txtAcademicYear.Text.Trim()); if(!InputValidator.ValidateStudentNo(txtStudentNo.Text.Trim())||!InputValidator.ValidateName(txtStudentName.Text.Trim())||!InputValidator.ValidateAcademicYear(academicYear)||!InputValidator.ValidateSemester(semester)||!InputValidator.ValidateMoney(txtAmount.Text.Trim())||dpAwardDate.SelectedDate==null||string.IsNullOrWhiteSpace(status)){ MessageBox.Show("请检查必填项与格式","验证",MessageBoxButton.OK,MessageBoxImage.Warning); return; } model.StudentNo=txtStudentNo.Text.Trim(); model.StudentName=txtStudentName.Text.Trim(); model.AcademicYear=academicYear; model.Semester=semester; model.ScholarshipType=txtType.Text.Trim(); model.ScholarshipLevel=txtLevel.Text.Trim(); model.Amount=decimal.Parse(txtAmount.Text.Trim()); model.AwardDate=dpAwardDate.SelectedDate; model.Status=status; if(model.Id!=0&&!repository.ScholarshipExists(model.Id)){ MessageBox.Show("当前记录已不存在，请刷新后重试","提示",MessageBoxButton.OK,MessageBoxImage.Information); return; } if(repository.ScholarshipDuplicateExists(model)){ MessageBox.Show("该学号已经存在奖助记录","验证",MessageBoxButton.OK,MessageBoxImage.Warning); return; } if(model.Id==0) model.Id=repository.AddScholarship(model); else repository.UpdateScholarship(model); DialogResult=true; Close(); }
        private void btnCancel_Click(object sender,RoutedEventArgs e){ DialogResult=false; Close(); }
    }
}

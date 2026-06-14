using System;
using System.Windows;
using StudentsManagerSystem.Common;
using StudentsManagerSystem.Models;
using StudentsManagerSystem.Services;

namespace StudentsManagerSystem.Views.StudentStatus
{
    public partial class ScholarshipEditWindow : Window
    {
        private readonly StudentStatusService studentStatusService = new StudentStatusService();
        private readonly BasicDataService basicDataService = new BasicDataService();
        private ScholarshipInfo model; public ScholarshipInfo Result => model;
        public ScholarshipEditWindow(ScholarshipInfo? existing = null){ InitializeComponent(); LoadBusinessOptions(); model = existing != null ? new ScholarshipInfo { Id=existing.Id, StudentId=existing.StudentId, StudentNo=existing.StudentNo, StudentName=existing.StudentName, AcademicYear=existing.AcademicYear, Semester=existing.Semester, ScholarshipType=existing.ScholarshipType, ScholarshipLevel=existing.ScholarshipLevel, Amount=existing.Amount, AwardDate=existing.AwardDate, Status=existing.Status } : new ScholarshipInfo { AwardDate=DateTime.Now, Status=FirstLookupValue("ScholarshipStatus") }; txtStudentNo.Text=model.StudentNo; txtStudentName.Text=model.StudentName; txtAcademicYear.Text=AcademicYearHelper.NormalizeStartYear(model.AcademicYear); SelectComboValue(cmbSemester, model.Semester); txtType.Text=model.ScholarshipType; txtLevel.Text=model.ScholarshipLevel; txtAmount.Text=model.Amount.ToString(); dpAwardDate.SelectedDate=model.AwardDate ?? DateTime.Now; SelectComboValue(cmbStatus, model.Status); }
        public void MakeReadOnly(){ btnOk.Visibility=Visibility.Collapsed; txtStudentNo.IsReadOnly=txtStudentName.IsReadOnly=txtAcademicYear.IsReadOnly=txtType.IsReadOnly=txtLevel.IsReadOnly=txtAmount.IsReadOnly=true; cmbSemester.IsEnabled=dpAwardDate.IsEnabled=cmbStatus.IsEnabled=false; }
        private void LoadBusinessOptions(){ cmbSemester.ItemsSource = basicDataService.GetLookupValues("Semester"); cmbStatus.ItemsSource = basicDataService.GetLookupValues("ScholarshipStatus"); if(cmbSemester.Items.Count>0) cmbSemester.SelectedIndex=0; if(cmbStatus.Items.Count>0) cmbStatus.SelectedIndex=0; }
        private string FirstLookupValue(string category) => basicDataService.GetLookupValues(category).FirstOrDefault() ?? string.Empty;
        private static void SelectComboValue(System.Windows.Controls.ComboBox comboBox, string value){ var selected = comboBox.Items.Cast<object?>().FirstOrDefault(item => string.Equals(item?.ToString(), value, StringComparison.OrdinalIgnoreCase)); if(selected != null) comboBox.SelectedItem = selected; }
        private static string ComboContent(System.Windows.Controls.ComboBox comboBox) => (comboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? comboBox.SelectedItem?.ToString() ?? string.Empty;
        private void btnOk_Click(object sender,RoutedEventArgs e){ var semester = ComboContent(cmbSemester); var status = ComboContent(cmbStatus); var academicYear = AcademicYearHelper.NormalizeStartYear(txtAcademicYear.Text.Trim()); if(!InputValidator.ValidateStudentNo(txtStudentNo.Text.Trim())||!InputValidator.ValidateName(txtStudentName.Text.Trim())||!InputValidator.ValidateAcademicYear(academicYear)||!InputValidator.ValidateSemester(semester)||!InputValidator.ValidateMoney(txtAmount.Text.Trim())||dpAwardDate.SelectedDate==null||string.IsNullOrWhiteSpace(status)){ MessageBox.Show("请检查必填项与格式","验证",MessageBoxButton.OK,MessageBoxImage.Warning); return; } model.StudentNo=txtStudentNo.Text.Trim(); model.StudentName=txtStudentName.Text.Trim(); model.AcademicYear=academicYear; model.Semester=semester; model.ScholarshipType=txtType.Text.Trim(); model.ScholarshipLevel=txtLevel.Text.Trim(); model.Amount=decimal.Parse(txtAmount.Text.Trim()); model.AwardDate=dpAwardDate.SelectedDate; model.Status=status; var result = studentStatusService.SaveScholarship(model); if(!result.Succeeded){ MessageBox.Show(result.Message,"验证",MessageBoxButton.OK,MessageBoxImage.Warning); return; } model.Id=result.Data; DialogResult=true; Close(); }
        private void btnCancel_Click(object sender,RoutedEventArgs e){ DialogResult=false; Close(); }
    }
}

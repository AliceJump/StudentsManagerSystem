using System;
using System.Windows;
using StudentsManagerSystem.Common;
using StudentsManagerSystem.Models;
using StudentsManagerSystem.Services;

namespace StudentsManagerSystem.Views.StudentStatus
{
    public partial class GraduationEditWindow : Window
    {
        private readonly StudentStatusService studentStatusService = new StudentStatusService();
        private GraduationInfo model; public GraduationInfo Result => model;
        public GraduationEditWindow(GraduationInfo? existing = null){ InitializeComponent(); model = existing != null ? new GraduationInfo { Id=existing.Id, StudentId=existing.StudentId, StudentNo=existing.StudentNo, StudentName=existing.StudentName, GraduationDate=existing.GraduationDate, GraduationType=existing.GraduationType, DegreeType=existing.DegreeType, CertificateNo=existing.CertificateNo, DegreeNo=existing.DegreeNo, Remarks=existing.Remarks } : new GraduationInfo { GraduationDate=DateTime.Now }; txtStudentNo.Text=model.StudentNo; txtStudentName.Text=model.StudentName; dpGraduationDate.SelectedDate=model.GraduationDate ?? DateTime.Now; txtGraduationType.Text=model.GraduationType; txtDegreeType.Text=model.DegreeType; txtCertificateNo.Text=model.CertificateNo; txtDegreeNo.Text=model.DegreeNo; txtRemarks.Text=model.Remarks; }
        public void MakeReadOnly(){ btnOk.Visibility=Visibility.Collapsed; txtStudentNo.IsReadOnly=txtStudentName.IsReadOnly=txtGraduationType.IsReadOnly=txtDegreeType.IsReadOnly=txtCertificateNo.IsReadOnly=txtDegreeNo.IsReadOnly=txtRemarks.IsReadOnly=true; dpGraduationDate.IsEnabled=false; }
        private void btnOk_Click(object sender,RoutedEventArgs e){ if(!InputValidator.ValidateStudentNo(txtStudentNo.Text.Trim())||!InputValidator.ValidateName(txtStudentName.Text.Trim())||dpGraduationDate.SelectedDate==null||string.IsNullOrWhiteSpace(txtGraduationType.Text)||string.IsNullOrWhiteSpace(txtDegreeType.Text)){ MessageBox.Show("请检查必填项与格式","验证",MessageBoxButton.OK,MessageBoxImage.Warning); return; } model.StudentNo=txtStudentNo.Text.Trim(); model.StudentName=txtStudentName.Text.Trim(); model.GraduationDate=dpGraduationDate.SelectedDate; model.GraduationType=txtGraduationType.Text.Trim(); model.DegreeType=txtDegreeType.Text.Trim(); model.CertificateNo=txtCertificateNo.Text.Trim(); model.DegreeNo=txtDegreeNo.Text.Trim(); model.Remarks=txtRemarks.Text.Trim(); var result = studentStatusService.SaveGraduation(model); if(!result.Succeeded){ MessageBox.Show(result.Message,"验证",MessageBoxButton.OK,MessageBoxImage.Warning); return; } model.Id=result.Data; DialogResult=true; Close(); }
        private void btnCancel_Click(object sender,RoutedEventArgs e){ DialogResult=false; Close(); }
    }
}

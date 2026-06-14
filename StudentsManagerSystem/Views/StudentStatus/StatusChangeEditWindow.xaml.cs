using System;
using System.Windows;
using System.Windows.Controls;
using StudentsManagerSystem.Common;
using StudentsManagerSystem.Models;
using StudentsManagerSystem.Services;

namespace StudentsManagerSystem.Views.StudentStatus
{
    public partial class StatusChangeEditWindow : Window
    {
        private readonly StudentStatusService studentStatusService = new StudentStatusService();
        private readonly BasicDataService basicDataService = new BasicDataService();
        private StatusChangeRecord model;
        public StatusChangeRecord Result => model;
        public StatusChangeEditWindow(StatusChangeRecord? existing = null)
        {
            InitializeComponent();
            LoadBusinessOptions();
            model = existing != null ? new StatusChangeRecord { Id = existing.Id, StudentId = existing.StudentId, StudentNo = existing.StudentNo, StudentName = existing.StudentName, ChangeDate = existing.ChangeDate, ChangeType = existing.ChangeType, OriginalInfo = existing.OriginalInfo, NewInfo = existing.NewInfo, Reason = existing.Reason, ApprovalStatus = existing.ApprovalStatus } : new StatusChangeRecord { ChangeDate = DateTime.Now, ApprovalStatus = basicDataService.GetLookupValues("ApprovalStatus").FirstOrDefault() ?? string.Empty };
            txtStudentNo.Text = model.StudentNo; txtStudentName.Text = model.StudentName; dpChangeDate.SelectedDate = model.ChangeDate ?? DateTime.Now; txtChangeType.Text = model.ChangeType; txtOriginalInfo.Text = model.OriginalInfo; txtNewInfo.Text = model.NewInfo; txtReason.Text = model.Reason; SelectComboValue(cmbApprovalStatus, model.ApprovalStatus);
        }
        private void LoadBusinessOptions(){ cmbApprovalStatus.ItemsSource = basicDataService.GetLookupValues("ApprovalStatus"); if(cmbApprovalStatus.Items.Count>0) cmbApprovalStatus.SelectedIndex=0; }
        private static void SelectComboValue(ComboBox comboBox, string value){ var selected = comboBox.Items.Cast<object?>().FirstOrDefault(item => string.Equals(item?.ToString(), value, StringComparison.OrdinalIgnoreCase)); if(selected != null) comboBox.SelectedItem = selected; }
        public void MakeReadOnly(){ btnOk.Visibility = Visibility.Collapsed; txtStudentNo.IsReadOnly = txtStudentName.IsReadOnly = txtChangeType.IsReadOnly = txtOriginalInfo.IsReadOnly = txtNewInfo.IsReadOnly = txtReason.IsReadOnly = true; dpChangeDate.IsEnabled = false; cmbApprovalStatus.IsEnabled = false; }
        private static string GetSelectedContent(ComboBox comboBox) => (comboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? comboBox.SelectedItem?.ToString() ?? string.Empty;
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!InputValidator.ValidateStudentNo(txtStudentNo.Text.Trim()) || !InputValidator.ValidateName(txtStudentName.Text.Trim()) || dpChangeDate.SelectedDate == null || string.IsNullOrWhiteSpace(txtChangeType.Text) || cmbApprovalStatus.SelectedItem == null) { MessageBox.Show("请检查必填项与格式", "验证", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            model.StudentNo = txtStudentNo.Text.Trim(); model.StudentName = txtStudentName.Text.Trim(); model.ChangeDate = dpChangeDate.SelectedDate; model.ChangeType = txtChangeType.Text.Trim(); model.OriginalInfo = txtOriginalInfo.Text.Trim(); model.NewInfo = txtNewInfo.Text.Trim(); model.Reason = txtReason.Text.Trim(); model.ApprovalStatus = GetSelectedContent(cmbApprovalStatus);
            var result = studentStatusService.SaveChange(model);
            if (!result.Succeeded) { MessageBox.Show(result.Message, "验证", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            model.Id = result.Data; DialogResult = true; Close();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e){ DialogResult = false; Close(); }
    }
}

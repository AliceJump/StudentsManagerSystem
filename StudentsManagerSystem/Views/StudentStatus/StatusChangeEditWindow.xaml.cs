using System;
using System.Windows;
using System.Windows.Controls;
using StudentsManagerSystem.Common;
using StudentsManagerSystem.Data.SqlServer;
using StudentsManagerSystem.Models;

namespace StudentsManagerSystem.Views.StudentStatus
{
    public partial class StatusChangeEditWindow : Window
    {
        private readonly StudentStatusRepository repository = new StudentStatusRepository();
        private StatusChangeRecord model;
        public StatusChangeRecord Result => model;
        public StatusChangeEditWindow(StatusChangeRecord? existing = null)
        {
            InitializeComponent();
            model = existing != null ? new StatusChangeRecord { Id = existing.Id, StudentId = existing.StudentId, StudentNo = existing.StudentNo, StudentName = existing.StudentName, ChangeDate = existing.ChangeDate, ChangeType = existing.ChangeType, OriginalInfo = existing.OriginalInfo, NewInfo = existing.NewInfo, Reason = existing.Reason, ApprovalStatus = existing.ApprovalStatus } : new StatusChangeRecord { ChangeDate = DateTime.Now, ApprovalStatus = "待审批" };
            txtStudentNo.Text = model.StudentNo; txtStudentName.Text = model.StudentName; dpChangeDate.SelectedDate = model.ChangeDate ?? DateTime.Now; txtChangeType.Text = model.ChangeType; txtOriginalInfo.Text = model.OriginalInfo; txtNewInfo.Text = model.NewInfo; txtReason.Text = model.Reason; cmbApprovalStatus.SelectedIndex = FindStatusIndex(model.ApprovalStatus);
        }
        private static int FindStatusIndex(string text) => text == "已批准" ? 1 : text == "已驳回" ? 2 : 0;
        public void MakeReadOnly(){ btnOk.Visibility = Visibility.Collapsed; txtStudentNo.IsReadOnly = txtStudentName.IsReadOnly = txtChangeType.IsReadOnly = txtOriginalInfo.IsReadOnly = txtNewInfo.IsReadOnly = txtReason.IsReadOnly = true; dpChangeDate.IsEnabled = false; cmbApprovalStatus.IsEnabled = false; }
        private static string GetSelectedContent(ComboBox comboBox) => (comboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? string.Empty;
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!InputValidator.ValidateStudentNo(txtStudentNo.Text.Trim()) || !InputValidator.ValidateName(txtStudentName.Text.Trim()) || dpChangeDate.SelectedDate == null || string.IsNullOrWhiteSpace(txtChangeType.Text) || cmbApprovalStatus.SelectedItem == null) { MessageBox.Show("请检查必填项与格式", "验证", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            model.StudentNo = txtStudentNo.Text.Trim(); model.StudentName = txtStudentName.Text.Trim(); model.ChangeDate = dpChangeDate.SelectedDate; model.ChangeType = txtChangeType.Text.Trim(); model.OriginalInfo = txtOriginalInfo.Text.Trim(); model.NewInfo = txtNewInfo.Text.Trim(); model.Reason = txtReason.Text.Trim(); model.ApprovalStatus = GetSelectedContent(cmbApprovalStatus);
            if (model.Id != 0 && !repository.ChangeExists(model.Id)) { MessageBox.Show("当前记录已不存在，请刷新后重试", "提示", MessageBoxButton.OK, MessageBoxImage.Information); return; }
            if (repository.ChangeDuplicateExists(model)) { MessageBox.Show("该学号已经存在变动记录", "验证", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (model.Id == 0) model.Id = repository.AddChange(model); else repository.UpdateChange(model); DialogResult = true; Close();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e){ DialogResult = false; Close(); }
    }
}

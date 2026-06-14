using StudentsManagerSystem.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace StudentsManagerSystem.Views.StudentArchive
{
    public partial class ArchiveRecordEditWindow : Window
    {
        private readonly int recordType;
        private readonly object? editingRecord;
        private readonly Dictionary<string, Control> editors = new();

        public object? ResultRecord { get; private set; }

        public ArchiveRecordEditWindow(int recordType, object? record = null)
        {
            InitializeComponent();
            this.recordType = recordType;
            editingRecord = record;
            ConfigureForm();
            LoadRecord(record);
        }

        private void ConfigureForm()
        {
            txtTitle.Text = recordType switch
            {
                1 => "家庭信息编辑",
                2 => "奖励记录编辑",
                3 => "处分记录编辑",
                4 => "体检信息编辑",
                _ => "档案记录编辑"
            };
            Title = txtTitle.Text;

            AddTextField("StudentNo", "学号：*", 0, 0);
            switch (recordType)
            {
                case 1:
                    AddTextField("RelationName", "关系人姓名：*", 0, 2);
                    AddTextField("Relationship", "关系：*", 1, 0);
                    AddTextField("PhoneNumber", "联系电话：", 1, 2);
                    AddTextField("WorkUnit", "工作单位：", 2, 0);
                    AddTextField("Address", "地址：", 2, 2);
                    break;
                case 2:
                    AddDateField("RewardDate", "奖励日期：*", 0, 2);
                    AddTextField("RewardType", "奖励类型：*", 1, 0);
                    AddTextField("RewardLevel", "奖励等级：*", 1, 2);
                    AddTextField("RewardReason", "奖励原因：", 2, 0);
                    AddTextField("RewardUnit", "颁发单位：", 2, 2);
                    break;
                case 3:
                    AddDateField("PunishmentDate", "处分日期：*", 0, 2);
                    AddTextField("PunishmentType", "处分类型：*", 1, 0);
                    AddTextField("PunishmentLevel", "处分等级：*", 1, 2);
                    AddTextField("PunishmentReason", "处分原因：", 2, 0);
                    AddDateField("CancelDate", "撤销日期：", 2, 2);
                    AddTextField("Status", "状态：*", 3, 0);
                    break;
                case 4:
                    AddDateField("CheckDate", "体检日期：*", 0, 2);
                    AddTextField("Height", "身高(cm)：*", 1, 0);
                    AddTextField("Weight", "体重(kg)：*", 1, 2);
                    AddTextField("BloodType", "血型：*", 2, 0);
                    AddTextField("Vision", "视力：*", 2, 2);
                    AddTextField("HealthStatus", "健康状况：*", 3, 0);
                    AddTextField("Remarks", "备注：", 3, 2);
                    break;
            }
        }

        private void AddTextField(string name, string label, int row, int column)
        {
            EnsureRow(row);
            AddLabel(label, row, column);
            var textBox = new TextBox { Style = (Style)FindResource("TextBoxStyle") };
            Grid.SetRow(textBox, row);
            Grid.SetColumn(textBox, column + 1);
            formGrid.Children.Add(textBox);
            editors[name] = textBox;
        }

        private void AddDateField(string name, string label, int row, int column)
        {
            EnsureRow(row);
            AddLabel(label, row, column);
            var datePicker = new DatePicker { Style = (Style)FindResource("DatePickerStyle") };
            Grid.SetRow(datePicker, row);
            Grid.SetColumn(datePicker, column + 1);
            formGrid.Children.Add(datePicker);
            editors[name] = datePicker;
        }

        private void AddLabel(string label, int row, int column)
        {
            var textBlock = new TextBlock { Text = label, Style = (Style)FindResource("LabelStyle") };
            Grid.SetRow(textBlock, row);
            Grid.SetColumn(textBlock, column);
            formGrid.Children.Add(textBlock);
        }

        private void EnsureRow(int row)
        {
            while (formGrid.RowDefinitions.Count <= row)
            {
                formGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
        }

        private void LoadRecord(object? record)
        {
            switch (record)
            {
                case FamilyInfo familyInfo:
                    SetText("StudentNo", familyInfo.StudentNo);
                    SetText("RelationName", familyInfo.RelationName);
                    SetText("Relationship", familyInfo.Relationship);
                    SetText("PhoneNumber", familyInfo.PhoneNumber);
                    SetText("WorkUnit", familyInfo.WorkUnit);
                    SetText("Address", familyInfo.Address);
                    break;
                case RewardRecord rewardRecord:
                    SetText("StudentNo", rewardRecord.StudentNo);
                    SetDate("RewardDate", rewardRecord.RewardDate);
                    SetText("RewardType", rewardRecord.RewardType);
                    SetText("RewardLevel", rewardRecord.RewardLevel);
                    SetText("RewardReason", rewardRecord.RewardReason);
                    SetText("RewardUnit", rewardRecord.RewardUnit);
                    break;
                case PunishmentRecord punishmentRecord:
                    SetText("StudentNo", punishmentRecord.StudentNo);
                    SetDate("PunishmentDate", punishmentRecord.PunishmentDate);
                    SetText("PunishmentType", punishmentRecord.PunishmentType);
                    SetText("PunishmentLevel", punishmentRecord.PunishmentLevel);
                    SetText("PunishmentReason", punishmentRecord.PunishmentReason);
                    SetDate("CancelDate", punishmentRecord.CancelDate);
                    SetText("Status", punishmentRecord.Status);
                    break;
                case HealthRecord healthRecord:
                    SetText("StudentNo", healthRecord.StudentNo);
                    SetDate("CheckDate", healthRecord.CheckDate);
                    SetText("Height", healthRecord.Height.ToString(CultureInfo.CurrentCulture));
                    SetText("Weight", healthRecord.Weight.ToString(CultureInfo.CurrentCulture));
                    SetText("BloodType", healthRecord.BloodType);
                    SetText("Vision", healthRecord.Vision);
                    SetText("HealthStatus", healthRecord.HealthStatus);
                    SetText("Remarks", healthRecord.Remarks);
                    break;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!TryBuildRecord(out var record, out var error))
            {
                MessageBox.Show(error, "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ResultRecord = record;
            DialogResult = true;
            Close();
        }

        private bool TryBuildRecord(out object? record, out string error)
        {
            record = null;
            error = string.Empty;

            switch (recordType)
            {
                case 1:
                    record = new FamilyInfo { Id = editingRecord is FamilyInfo familyInfo ? familyInfo.Id : 0, StudentNo = GetText("StudentNo"), RelationName = GetText("RelationName"), Relationship = GetText("Relationship"), PhoneNumber = GetText("PhoneNumber"), WorkUnit = GetText("WorkUnit"), Address = GetText("Address") };
                    break;
                case 2:
                    record = new RewardRecord { Id = editingRecord is RewardRecord rewardRecord ? rewardRecord.Id : 0, StudentNo = GetText("StudentNo"), RewardDate = GetDate("RewardDate"), RewardType = GetText("RewardType"), RewardLevel = GetText("RewardLevel"), RewardReason = GetText("RewardReason"), RewardUnit = GetText("RewardUnit") };
                    break;
                case 3:
                    record = new PunishmentRecord { Id = editingRecord is PunishmentRecord punishmentRecord ? punishmentRecord.Id : 0, StudentNo = GetText("StudentNo"), PunishmentDate = GetDate("PunishmentDate"), PunishmentType = GetText("PunishmentType"), PunishmentLevel = GetText("PunishmentLevel"), PunishmentReason = GetText("PunishmentReason"), CancelDate = GetDate("CancelDate"), Status = GetText("Status") };
                    break;
                case 4:
                    if (!decimal.TryParse(GetText("Height"), out var height) || !decimal.TryParse(GetText("Weight"), out var weight))
                    {
                        error = "身高和体重必须为数字";
                        return false;
                    }

                    record = new HealthRecord { Id = editingRecord is HealthRecord healthRecord ? healthRecord.Id : 0, StudentNo = GetText("StudentNo"), CheckDate = GetDate("CheckDate"), Height = height, Weight = weight, BloodType = GetText("BloodType"), Vision = GetText("Vision"), HealthStatus = GetText("HealthStatus"), Remarks = GetText("Remarks") };
                    break;
            }

            return true;
        }

        private string GetText(string name) => editors[name] is TextBox textBox ? textBox.Text.Trim() : string.Empty;

        private DateTime? GetDate(string name) => editors[name] is DatePicker datePicker ? datePicker.SelectedDate : null;

        private void SetText(string name, string value)
        {
            if (editors[name] is TextBox textBox)
            {
                textBox.Text = value;
            }
        }

        private void SetDate(string name, DateTime? value)
        {
            if (editors[name] is DatePicker datePicker)
            {
                datePicker.SelectedDate = value;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

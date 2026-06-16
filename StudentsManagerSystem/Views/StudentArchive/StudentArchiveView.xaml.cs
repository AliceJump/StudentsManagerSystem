using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using StudentsManagerSystem.Common;
using StudentsManagerSystem.Models;
using StudentsManagerSystem.Services;

namespace StudentsManagerSystem.Views.StudentArchive
{
    public partial class StudentArchiveView : Page
    {
        private readonly StudentService studentService = new StudentService();
        private readonly System.Collections.ObjectModel.ObservableCollection<Student> students = new System.Collections.ObjectModel.ObservableCollection<Student>();
        private List<Student> filteredStudents = new List<Student>();
        private int currentPage = 1;
        private List<FamilyInfo> familyInfos = new List<FamilyInfo>();
        private List<RewardRecord> rewards = new List<RewardRecord>();
        private List<PunishmentRecord> punishments = new List<PunishmentRecord>();
        private List<HealthRecord> healthRecords = new List<HealthRecord>();

        public StudentArchiveView()
        {
            InitializeComponent();
            ApplyPermissions();
            LoadStudentData();
        }

        private bool IsAdmin() => string.Equals(App.CurrentUserRole, "Admin", StringComparison.OrdinalIgnoreCase);

        private void ApplyPermissions()
        {
            if (IsAdmin())
            {
                return;
            }

            btnAdd.Visibility = Visibility.Collapsed;
            btnEdit.Visibility = Visibility.Collapsed;
            btnDelete.Visibility = Visibility.Collapsed;
            btnImport.Visibility = Visibility.Collapsed;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
                LoadStudentData();
            else if (tabControl.SelectedIndex == 1)
                LoadFamilyData();
            else if (tabControl.SelectedIndex == 2)
                LoadRewardData();
            else if (tabControl.SelectedIndex == 3)
                LoadPunishmentData();
            else if (tabControl.SelectedIndex == 4)
                LoadHealthData();
        }

        private void LoadStudentData()
        {
            filteredStudents = string.IsNullOrWhiteSpace(txtSearch.Text)
                ? studentService.GetAll()
                : studentService.Search(txtSearch.Text);

            currentPage = 1;
            ApplySortAndPaging();
        }

        private void ApplySortAndPaging()
        {
            var sortedStudents = SortStudents(filteredStudents).ToList();
            var pageSize = GetPageSize();
            var totalPages = Math.Max(1, (int)Math.Ceiling(sortedStudents.Count / (double)pageSize));
            currentPage = Math.Clamp(currentPage, 1, totalPages);
            var pageStudents = sortedStudents.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            students.Clear();
            foreach (var student in pageStudents)
            {
                students.Add(student);
            }

            ConfigureStudentColumns();
            txtPageInfo.Text = $"第 {currentPage}/{totalPages} 页，共 {sortedStudents.Count} 条";
        }

        private IEnumerable<Student> SortStudents(IEnumerable<Student> source)
        {
            var sortText = (cmbSort.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "学号升序";
            return sortText switch
            {
                "学号降序" => source.OrderByDescending(student => student.StudentNo),
                "姓名升序" => source.OrderBy(student => student.Name),
                "院系升序" => source.OrderBy(student => student.Department).ThenBy(student => student.StudentNo),
                "班级升序" => source.OrderBy(student => student.Class).ThenBy(student => student.StudentNo),
                _ => source.OrderBy(student => student.StudentNo)
            };
        }

        private int GetPageSize()
        {
            var text = (cmbPageSize.SelectedItem as ComboBoxItem)?.Content?.ToString();
            return int.TryParse(text, out var pageSize) && pageSize > 0 ? pageSize : 10;
        }

        private void ConfigureStudentColumns()
        {
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学号", Binding = new System.Windows.Data.Binding("StudentNo"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "姓名", Binding = new System.Windows.Data.Binding("Name"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "性别", Binding = new System.Windows.Data.Binding("Gender"), Width = new DataGridLength(60) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "出生日期", Binding = new System.Windows.Data.Binding("BirthDate") { StringFormat = "yyyy-MM-dd" }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "身份证号", Binding = new System.Windows.Data.Binding("IdCard"), Width = new DataGridLength(180) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "民族", Binding = new System.Windows.Data.Binding("Nation"), Width = new DataGridLength(80) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "政治面貌", Binding = new System.Windows.Data.Binding("PoliticalStatus"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "联系电话", Binding = new System.Windows.Data.Binding("PhoneNumber"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "院系", Binding = new System.Windows.Data.Binding("Department"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "专业", Binding = new System.Windows.Data.Binding("Major"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "班级", Binding = new System.Windows.Data.Binding("Class"), Width = new DataGridLength(100) });
            dataGrid.ItemsSource = students;
        }

        private void LoadFamilyData()
        {
            familyInfos = studentService.GetFamilyInfos();
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学号", Binding = new System.Windows.Data.Binding("StudentNo"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "关系人姓名", Binding = new System.Windows.Data.Binding("RelationName"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "关系", Binding = new System.Windows.Data.Binding("Relationship"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "联系电话", Binding = new System.Windows.Data.Binding("PhoneNumber"), Width = new DataGridLength(150) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "工作单位", Binding = new System.Windows.Data.Binding("WorkUnit"), Width = new DataGridLength(200) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "地址", Binding = new System.Windows.Data.Binding("Address"), Width = new DataGridLength(250) });
            
            dataGrid.ItemsSource = familyInfos;
        }

        private void LoadRewardData()
        {
            rewards = studentService.GetRewardRecords();
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学号", Binding = new System.Windows.Data.Binding("StudentNo"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "奖励日期", Binding = new System.Windows.Data.Binding("RewardDate") { StringFormat = "yyyy-MM-dd" }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "奖励类型", Binding = new System.Windows.Data.Binding("RewardType"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "奖励等级", Binding = new System.Windows.Data.Binding("RewardLevel"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "奖励原因", Binding = new System.Windows.Data.Binding("RewardReason"), Width = new DataGridLength(250) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "颁发单位", Binding = new System.Windows.Data.Binding("RewardUnit"), Width = new DataGridLength(200) });
            
            dataGrid.ItemsSource = rewards;
        }

        private void LoadPunishmentData()
        {
            punishments = studentService.GetPunishmentRecords();
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学号", Binding = new System.Windows.Data.Binding("StudentNo"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "处分日期", Binding = new System.Windows.Data.Binding("PunishmentDate") { StringFormat = "yyyy-MM-dd" }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "处分类型", Binding = new System.Windows.Data.Binding("PunishmentType"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "处分等级", Binding = new System.Windows.Data.Binding("PunishmentLevel"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "处分原因", Binding = new System.Windows.Data.Binding("PunishmentReason"), Width = new DataGridLength(200) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "撤销日期", Binding = new System.Windows.Data.Binding("CancelDate") { StringFormat = "yyyy-MM-dd" }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "状态", Binding = new System.Windows.Data.Binding("Status"), Width = new DataGridLength(100) });
            
            dataGrid.ItemsSource = punishments;
        }

        private void LoadHealthData()
        {
            healthRecords = studentService.GetHealthRecords();
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "学号", Binding = new System.Windows.Data.Binding("StudentNo"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "体检日期", Binding = new System.Windows.Data.Binding("CheckDate") { StringFormat = "yyyy-MM-dd" }, Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "身高(cm)", Binding = new System.Windows.Data.Binding("Height"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "体重(kg)", Binding = new System.Windows.Data.Binding("Weight"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "血型", Binding = new System.Windows.Data.Binding("BloodType"), Width = new DataGridLength(80) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "视力", Binding = new System.Windows.Data.Binding("Vision"), Width = new DataGridLength(100) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "健康状况", Binding = new System.Windows.Data.Binding("HealthStatus"), Width = new DataGridLength(120) });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "备注", Binding = new System.Windows.Data.Binding("Remarks"), Width = new DataGridLength(200) });
            
            dataGrid.ItemsSource = healthRecords;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin())
            {
                MessageBox.Show("当前用户仅支持查看，不允许新增。", "权限提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (tabControl.SelectedIndex != 0)
            {
                EditArchiveRecord(null);
                return;
            }

            var window = new StudentEditWindow();
            if (window.ShowDialog() == true && window.ResultStudent != null)
            {
                var saveResult = studentService.Add(window.ResultStudent);
                if (saveResult.Succeeded)
                {
                    MessageBox.Show(saveResult.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadStudentData();
                }
                else
                {
                    MessageBox.Show(saveResult.Message, "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin())
            {
                MessageBox.Show("当前用户仅支持查看，不允许修改。", "权限提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (tabControl.SelectedIndex != 0)
            {
                if (dataGrid.SelectedItem == null)
                {
                    MessageBox.Show("请先选择要修改的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                EditArchiveRecord(dataGrid.SelectedItem);
                return;
            }

            if (dataGrid.SelectedItem is Student selectedStudent)
            {
                var window = new StudentEditWindow(selectedStudent);
                if (window.ShowDialog() == true && window.ResultStudent != null)
                {
                    var saveResult = studentService.Update(window.ResultStudent);
                    if (saveResult.Succeeded)
                    {
                        MessageBox.Show(saveResult.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadStudentData();
                    }
                    else
                    {
                        MessageBox.Show(saveResult.Message, "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("请先选择要修改的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin())
            {
                MessageBox.Show("当前用户仅支持查看，不允许删除。", "权限提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (tabControl.SelectedIndex != 0)
            {
                DeleteArchiveRecord();
                return;
            }

            if (dataGrid.SelectedItem is Student selectedStudent)
            {
                var result = MessageBox.Show("确定要删除选中的记录吗？", "确认删除", 
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    studentService.Delete(selectedStudent.Id);
                    LoadStudentData();
                }
            }
            else
            {
                MessageBox.Show("请先选择要删除的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != 0)
            {
                if (dataGrid.SelectedItem == null)
                {
                    MessageBox.Show("请先选择要查看的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                MessageBox.Show(BuildArchiveRecordDetail(dataGrid.SelectedItem), "记录详情", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (dataGrid.SelectedItem is Student selectedStudent)
            {
                var window = new StudentDetailWindow(selectedStudent);
                window.ShowDialog();
            }
            else
            {
                MessageBox.Show("请先选择要查看的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            currentPage = 1;
            TabControl_SelectionChanged(null!, null!);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                LoadStudentData();
                return;
            }

            MessageBox.Show("当前仅为学生基本信息模块接入了数据库搜索。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditArchiveRecord(object? record)
        {
            var window = new ArchiveRecordEditWindow(tabControl.SelectedIndex, record);
            if (window.ShowDialog() != true || window.ResultRecord == null)
            {
                return;
            }

            var saveResult = SaveArchiveRecord(window.ResultRecord);
            if (saveResult.Succeeded)
            {
                MessageBox.Show(saveResult.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadCurrentArchiveTab();
            }
            else
            {
                MessageBox.Show(saveResult.Message, "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private ServiceResult SaveArchiveRecord(object record)
        {
            return record switch
            {
                FamilyInfo familyInfo => ToServiceResult(studentService.SaveFamilyInfo(familyInfo)),
                RewardRecord rewardRecord => ToServiceResult(studentService.SaveRewardRecord(rewardRecord)),
                PunishmentRecord punishmentRecord => ToServiceResult(studentService.SavePunishmentRecord(punishmentRecord)),
                HealthRecord healthRecord => ToServiceResult(studentService.SaveHealthRecord(healthRecord)),
                _ => ServiceResult.Failure("不支持的档案记录类型")
            };
        }

        private static ServiceResult ToServiceResult<T>(ServiceResult<T> result)
        {
            return result.Succeeded ? ServiceResult.Success(result.Message) : ServiceResult.Failure(result.Message);
        }

        private void DeleteArchiveRecord()
        {
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("请先选择要删除的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show("确定要删除选中的记录吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            switch (dataGrid.SelectedItem)
            {
                case FamilyInfo familyInfo:
                    studentService.DeleteFamilyInfo(familyInfo.Id);
                    break;
                case RewardRecord rewardRecord:
                    studentService.DeleteRewardRecord(rewardRecord.Id);
                    break;
                case PunishmentRecord punishmentRecord:
                    studentService.DeletePunishmentRecord(punishmentRecord.Id);
                    break;
                case HealthRecord healthRecord:
                    studentService.DeleteHealthRecord(healthRecord.Id);
                    break;
            }

            LoadCurrentArchiveTab();
        }

        private void LoadCurrentArchiveTab()
        {
            if (tabControl.SelectedIndex == 1)
                LoadFamilyData();
            else if (tabControl.SelectedIndex == 2)
                LoadRewardData();
            else if (tabControl.SelectedIndex == 3)
                LoadPunishmentData();
            else if (tabControl.SelectedIndex == 4)
                LoadHealthData();
        }

        private static string BuildArchiveRecordDetail(object record)
        {
            return record switch
            {
                FamilyInfo item => $"学号：{item.StudentNo}\n关系人姓名：{item.RelationName}\n关系：{item.Relationship}\n联系电话：{item.PhoneNumber}\n工作单位：{item.WorkUnit}\n地址：{item.Address}",
                RewardRecord item => $"学号：{item.StudentNo}\n奖励日期：{item.RewardDate:yyyy-MM-dd}\n奖励类型：{item.RewardType}\n奖励等级：{item.RewardLevel}\n奖励原因：{item.RewardReason}\n颁发单位：{item.RewardUnit}",
                PunishmentRecord item => $"学号：{item.StudentNo}\n处分日期：{item.PunishmentDate:yyyy-MM-dd}\n处分类型：{item.PunishmentType}\n处分等级：{item.PunishmentLevel}\n处分原因：{item.PunishmentReason}\n撤销日期：{item.CancelDate:yyyy-MM-dd}\n状态：{item.Status}",
                HealthRecord item => $"学号：{item.StudentNo}\n体检日期：{item.CheckDate:yyyy-MM-dd}\n身高：{item.Height} cm\n体重：{item.Weight} kg\n血型：{item.BloodType}\n视力：{item.Vision}\n健康状况：{item.HealthStatus}\n备注：{item.Remarks}",
                _ => "不支持的档案记录类型"
            };
        }

        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl?.SelectedIndex == 0 && filteredStudents.Count > 0)
            {
                currentPage = 1;
                ApplySortAndPaging();
            }
        }

        private void cmbPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl?.SelectedIndex == 0 && filteredStudents.Count > 0)
            {
                currentPage = 1;
                ApplySortAndPaging();
            }
        }

        private void btnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != 0)
            {
                return;
            }

            currentPage--;
            ApplySortAndPaging();
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != 0)
            {
                return;
            }

            currentPage++;
            ApplySortAndPaging();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAdmin())
            {
                MessageBox.Show("当前用户仅支持查看，不允许导入。", "权限提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (tabControl.SelectedIndex != 0)
            {
                MessageBox.Show("当前仅支持学生基本信息导入。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new OpenFileDialog
            {
                Filter = "CSV 文件 (*.csv)|*.csv",
                Title = "导入学生基本信息"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                var importResult = ImportStudents(dialog.FileName);
                foreach (var student in importResult.Students)
                {
                    var saveResult = studentService.SaveImported(student);
                    if (!saveResult.Succeeded)
                    {
                        throw new InvalidOperationException($"{student.StudentNo}：{saveResult.Message}");
                    }
                }

                AppLogger.Info($"导入学生基本信息：{importResult.Students.Count} 条，跳过 {importResult.SkippedRows} 条，文件={dialog.FileName}");
                LoadStudentData();
                MessageBox.Show($"成功处理 {importResult.Students.Count} 条学生记录，跳过 {importResult.SkippedRows} 条无效记录。\n导入规则：学号存在则更新，不存在则新增。", "导入完成", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                AppLogger.Error("导入学生基本信息失败。", ex);
                MessageBox.Show($"导入失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != 0)
            {
                MessageBox.Show("当前仅支持学生基本信息导出。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "CSV 文件 (*.csv)|*.csv",
                FileName = $"学生基本信息_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                Title = "导出学生基本信息"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                CsvExportHelper.ExportToCsv(SortStudents(filteredStudents), dialog.FileName);
                AppLogger.Info($"导出学生基本信息：{filteredStudents.Count} 条，文件={dialog.FileName}");
                MessageBox.Show("学生基本信息导出成功。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                AppLogger.Error("导出学生基本信息失败。", ex);
                MessageBox.Show($"导出失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static CsvImportResult ImportStudents(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            if (lines.Length <= 1)
            {
                return new CsvImportResult(new List<Student>(), 0);
            }

            var headerColumns = SplitCsvLine(lines[0]).Select((value, index) => (Value: value.TrimStart('\uFEFF'), Index: index))
                .ToDictionary(item => item.Value, item => item.Index, StringComparer.OrdinalIgnoreCase);
            var imported = new List<Student>();
            var skippedRows = 0;
            for (var index = 1; index < lines.Length; index++)
            {
                var rawLine = lines[index].Trim();
                if (string.IsNullOrWhiteSpace(rawLine))
                {
                    continue;
                }

                var columns = SplitCsvLine(rawLine);
                if (!TryParseStudent(columns, headerColumns, out var student))
                {
                    skippedRows++;
                    continue;
                }

                imported.Add(student);
            }

            return new CsvImportResult(imported, skippedRows);
        }

        private static bool TryParseStudent(IReadOnlyList<string> columns, IReadOnlyDictionary<string, int> headers, out Student student)
        {
            student = new Student();

            var studentNoIndex = ResolveColumnIndex(headers, columns.Count, "StudentNo", 0);
            var nameIndex = ResolveColumnIndex(headers, columns.Count, "Name", 1);
            var genderIndex = ResolveColumnIndex(headers, columns.Count, "Gender", 2);
            var birthDateIndex = ResolveColumnIndex(headers, columns.Count, "BirthDate", 3);
            var idCardIndex = ResolveColumnIndex(headers, columns.Count, "IdCard", 4);
            var nationIndex = ResolveColumnIndex(headers, columns.Count, "Nation", 5);
            var politicalStatusIndex = ResolveColumnIndex(headers, columns.Count, "PoliticalStatus", 6);
            var phoneNumberIndex = ResolveColumnIndex(headers, columns.Count, "PhoneNumber", 7);
            var emailIndex = ResolveColumnIndex(headers, columns.Count, "Email", 8);
            var addressIndex = ResolveColumnIndex(headers, columns.Count, "Address", 9);
            var departmentIndex = ResolveColumnIndex(headers, columns.Count, "Department", 10);
            var majorIndex = ResolveColumnIndex(headers, columns.Count, "Major", 11);
            var classIndex = ResolveColumnIndex(headers, columns.Count, "Class", 12);
            var enrollmentDateIndex = ResolveColumnIndex(headers, columns.Count, "EnrollmentDate", 13);
            var photoIndex = ResolveColumnIndex(headers, columns.Count, "Photo", 14);

            if (studentNoIndex < 0 || nameIndex < 0 || genderIndex < 0 || birthDateIndex < 0 || idCardIndex < 0 || nationIndex < 0 || politicalStatusIndex < 0 || phoneNumberIndex < 0 || emailIndex < 0 || addressIndex < 0 || departmentIndex < 0 || majorIndex < 0 || classIndex < 0 || enrollmentDateIndex < 0)
            {
                return false;
            }

            student.StudentNo = GetColumn(columns, studentNoIndex);
            student.Name = GetColumn(columns, nameIndex);
            student.Gender = GetColumn(columns, genderIndex);
            student.BirthDate = ParseDate(columns, birthDateIndex);
            student.IdCard = GetColumn(columns, idCardIndex);
            student.Nation = GetColumn(columns, nationIndex);
            student.PoliticalStatus = GetColumn(columns, politicalStatusIndex);
            student.PhoneNumber = GetColumn(columns, phoneNumberIndex);
            student.Email = GetColumn(columns, emailIndex);
            student.Address = GetColumn(columns, addressIndex);
            student.Department = GetColumn(columns, departmentIndex);
            student.Major = GetColumn(columns, majorIndex);
            student.Class = GetColumn(columns, classIndex);
            student.EnrollmentDate = ParseDate(columns, enrollmentDateIndex);
            student.Photo = photoIndex >= 0 ? GetColumn(columns, photoIndex) : string.Empty;
            return !string.IsNullOrWhiteSpace(student.StudentNo);
        }

        private static int ResolveColumnIndex(IReadOnlyDictionary<string, int> headers, int columnCount, string headerName, int fallbackIndex)
        {
            if (headers.TryGetValue(headerName, out var headerIndex))
            {
                return headerIndex;
            }

            return fallbackIndex < columnCount ? fallbackIndex : -1;
        }

        private static List<string> SplitCsvLine(string line)
        {
            var values = new List<string>();
            var builder = new System.Text.StringBuilder();
            var insideQuotes = false;

            for (var index = 0; index < line.Length; index++)
            {
                var current = line[index];
                if (current == '"')
                {
                    if (insideQuotes && index + 1 < line.Length && line[index + 1] == '"')
                    {
                        builder.Append('"');
                        index++;
                    }
                    else
                    {
                        insideQuotes = !insideQuotes;
                    }
                }
                else if (current == ',' && !insideQuotes)
                {
                    values.Add(builder.ToString());
                    builder.Clear();
                }
                else
                {
                    builder.Append(current);
                }
            }

            values.Add(builder.ToString());
            return values;
        }

        private static string GetColumn(IReadOnlyList<string> columns, int index)
        {
            return index < columns.Count ? columns[index].Trim() : string.Empty;
        }

        private static DateTime? ParseDate(IReadOnlyList<string> columns, int index)
        {
            return DateTime.TryParse(GetColumn(columns, index), CultureInfo.InvariantCulture, DateTimeStyles.None, out var value)
                ? value
                : null;
        }

        private sealed record CsvImportResult(List<Student> Students, int SkippedRows);
    }
}

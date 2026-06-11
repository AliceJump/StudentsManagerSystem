using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using StudentsManagerSystem.Common;
using StudentsManagerSystem.Models;
using StudentsManagerSystem.Services;
using ScoreModel = StudentsManagerSystem.Models.Score;

namespace StudentsManagerSystem.Views.Score
{
    public partial class ScoreView : Page
    {
        private readonly ScoreService scoreService = new ScoreService();
        private readonly System.Collections.ObjectModel.ObservableCollection<ScoreModel> scores = new System.Collections.ObjectModel.ObservableCollection<ScoreModel>();

        public ScoreView()
        {
            InitializeComponent();
            LoadBusinessOptions();
            LoadScoreData();
        }

        private void LoadBusinessOptions()
        {
            var years = scoreService.GetAcademicYears();
            cmbAcademicYear.ItemsSource = years;
            if (years.Count > 0)
            {
                cmbAcademicYear.SelectedIndex = 0;
            }

            var semesters = scoreService.GetSemesters();
            cmbSemester.ItemsSource = semesters;
            if (semesters.Count > 0)
            {
                cmbSemester.SelectedIndex = 0;
            }
        }

        private void LoadScoreData()
        {
            var academicYear = GetComboText(cmbAcademicYear);
            var semester = GetComboText(cmbSemester);
            var loadedScores = scoreService.GetByAcademicYearSemester(academicYear, semester);
            dataGrid.ItemsSource = null;
            scores.Clear();
            foreach (var score in loadedScores)
            {
                scores.Add(score);
            }

            dataGrid.ItemsSource = scores;
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            LoadScoreData();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var window = new ScoreEditWindow();
            if (window.ShowDialog() == true && window.Result is not null)
            {
                var saveResult = scoreService.Add(window.Result);
                if (saveResult.Succeeded)
                {
                    MessageBox.Show(saveResult.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadScoreData();
                }
                else
                {
                    MessageBox.Show(saveResult.Message, "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Models.Score selectedScore)
            {
                var window = new ScoreEditWindow(selectedScore);
                if (window.ShowDialog() == true && window.Result is not null)
                {
                    var saveResult = scoreService.Update(window.Result);
                    if (saveResult.Succeeded)
                    {
                        MessageBox.Show(saveResult.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadScoreData();
                    }
                    else
                    {
                        MessageBox.Show(saveResult.Message, "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("请先选择要修改的成绩记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Models.Score selectedScore)
            {
                var result = MessageBox.Show("确定要删除选中的成绩记录吗？", "确认删除",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    scoreService.Delete(selectedScore.Id);
                    LoadScoreData();
                }
            }
            else
            {
                MessageBox.Show("请先选择要删除的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Models.Score selectedScore)
            {
                MessageBox.Show(
                    $"学号：{selectedScore.StudentNo}\n姓名：{selectedScore.StudentName}\n课程：{selectedScore.CourseName}\n总评：{selectedScore.TotalScore?.ToString() ?? "未录入"}\n等级：{selectedScore.Grade}",
                    "成绩详情",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("请先选择要查看的记录！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "CSV 文件 (*.csv)|*.csv",
                Title = "导入成绩"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                var importedItems = ImportScores(dialog.FileName);
                foreach (var item in importedItems)
                {
                    var saveResult = scoreService.SaveImported(item);
                    if (!saveResult.Succeeded)
                    {
                        throw new InvalidOperationException(saveResult.Message);
                    }
                }

                LoadScoreData();
                AppLogger.Info($"导入成绩：{importedItems.Count} 条，文件={dialog.FileName}");
                MessageBox.Show($"成功导入 {importedItems.Count} 条成绩记录。", "导入完成", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                AppLogger.Error("导入成绩失败。", ex);
                MessageBox.Show($"导入失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "CSV 文件 (*.csv)|*.csv",
                FileName = $"成绩导出_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                Title = "导出成绩"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                CsvExportHelper.ExportToCsv(scores, dialog.FileName);
                AppLogger.Info($"导出成绩：{scores.Count} 条，文件={dialog.FileName}");
                MessageBox.Show("成绩导出成功。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                AppLogger.Error("导出成绩失败。", ex);
                MessageBox.Show($"导出失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadScoreData();
        }

        private static string GetComboText(System.Windows.Controls.ComboBox comboBox)
        {
            return comboBox.SelectedItem is System.Windows.Controls.ComboBoxItem item
                ? item.Content?.ToString() ?? string.Empty
                : comboBox.SelectedItem?.ToString() ?? comboBox.Text.Trim();
        }

        private static List<ScoreModel> ImportScores(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            if (lines.Length <= 1)
            {
                return new List<ScoreModel>();
            }

            var imported = new List<ScoreModel>();
            for (var index = 1; index < lines.Length; index++)
            {
                var rawLine = lines[index].Trim();
                if (string.IsNullOrWhiteSpace(rawLine))
                {
                    continue;
                }

                var columns = SplitCsvLine(rawLine);
                if (columns.Count < 14)
                {
                    continue;
                }

                var baseIndex = columns.Count >= 15 ? 1 : 0;

                imported.Add(new ScoreModel
                {
                    StudentId = ParseInt(columns, baseIndex),
                    StudentNo = GetColumn(columns, baseIndex + 1),
                    StudentName = GetColumn(columns, baseIndex + 2),
                    AcademicYear = GetColumn(columns, baseIndex + 3),
                    Semester = GetColumn(columns, baseIndex + 4),
                    CourseNo = GetColumn(columns, baseIndex + 5),
                    CourseName = GetColumn(columns, baseIndex + 6),
                    Credit = ParseDecimal(columns, baseIndex + 7),
                    RegularScore = ParseNullableDecimal(columns, baseIndex + 8),
                    ExamScore = ParseNullableDecimal(columns, baseIndex + 9),
                    TotalScore = ParseNullableDecimal(columns, baseIndex + 10),
                    Grade = GetColumn(columns, baseIndex + 11),
                    Status = GetColumn(columns, baseIndex + 12),
                    Remarks = GetColumn(columns, baseIndex + 13)
                });
            }

            return imported;
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

        private static int ParseInt(IReadOnlyList<string> columns, int index)
        {
            return int.TryParse(GetColumn(columns, index), out var value) ? value : 0;
        }

        private static decimal ParseDecimal(IReadOnlyList<string> columns, int index)
        {
            return decimal.TryParse(GetColumn(columns, index), out var value) ? value : 0m;
        }

        private static decimal? ParseNullableDecimal(IReadOnlyList<string> columns, int index)
        {
            return decimal.TryParse(GetColumn(columns, index), out var value) ? value : null;
        }
    }
}

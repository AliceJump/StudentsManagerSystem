using System.Windows;

namespace StudentsManagerSystem.Views.Score
{
    public partial class ScoreEditWindow : Window
    {
        public ScoreEditWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStudentNo.Text) ||
                string.IsNullOrWhiteSpace(txtStudentName.Text) ||
                cmbAcademicYear.SelectedItem == null ||
                cmbSemester.SelectedItem == null ||
                cmbCourse.SelectedItem == null)
            {
                MessageBox.Show("请填写所有必填项（标*的字段）！", "提示", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("保存成功！\n（数据保存功能将在后续实现）", "提示", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

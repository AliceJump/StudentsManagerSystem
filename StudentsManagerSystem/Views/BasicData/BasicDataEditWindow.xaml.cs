using System.Windows;

namespace StudentsManagerSystem.Views.BasicData
{
    public partial class BasicDataEditWindow : Window
    {
        public BasicDataEditWindow(string title)
        {
            InitializeComponent();
            txtTitle.Text = title;
            this.Title = title;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNo.Text) ||
                string.IsNullOrWhiteSpace(txtName.Text))
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

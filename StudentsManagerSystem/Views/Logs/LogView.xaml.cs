using System.IO;
using System.Windows;
using System.Windows.Controls;
using StudentsManagerSystem.Common;

namespace StudentsManagerSystem.Views.Logs
{
    public partial class LogView : Page
    {
        public LogView()
        {
            InitializeComponent();
            LoadLogs();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadLogs();
        }

        private void LoadLogs()
        {
            lstLogFiles.ItemsSource = AppLogger.GetLogFiles().Select(Path.GetFileName).ToList();
            txtLogContent.Text = AppLogger.ReadRecentText();
        }
    }
}

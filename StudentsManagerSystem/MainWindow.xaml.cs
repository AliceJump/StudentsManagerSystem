using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using StudentsManagerSystem.ViewModels;

namespace StudentsManagerSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer timer;
        private readonly MainViewModel viewModel = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
            ApplyPermissions();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void ApplyPermissions()
        {
            if (!string.Equals(App.CurrentUserRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                btnBasicData.IsEnabled = false;
                btnBasicData.ToolTip = "当前用户无基础数据管理权限";
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            viewModel.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton button)
            {
                switch (button.Name)
                {
                    case "btnHome":
                        viewModel.CurrentPageTitle = "欢迎使用学生管理系统";
                        MainFrame.Content = null;
                        break;
                    case "btnStudentArchive":
                        viewModel.CurrentPageTitle = "学生档案管理";
                        MainFrame.Navigate(new Uri("Views/StudentArchive/StudentArchiveView.xaml", UriKind.Relative));
                        break;
                    case "btnStudentStatus":
                        viewModel.CurrentPageTitle = "学生学籍管理";
                        MainFrame.Navigate(new Uri("Views/StudentStatus/StudentStatusView.xaml", UriKind.Relative));
                        break;
                    case "btnScoreManage":
                        viewModel.CurrentPageTitle = "学生成绩管理";
                        MainFrame.Navigate(new Uri("Views/Score/ScoreView.xaml", UriKind.Relative));
                        break;
                    case "btnBasicData":
                        if (!string.Equals(App.CurrentUserRole, "Admin", StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show("当前用户无基础数据管理权限。", "权限提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            btnHome.IsChecked = true;
                            return;
                        }

                        viewModel.CurrentPageTitle = "基础数据管理";
                        MainFrame.Navigate(new Uri("Views/BasicData/BasicDataView.xaml", UriKind.Relative));
                        break;
                    case "btnQuery":
                        viewModel.CurrentPageTitle = "信息查询统计";
                        MainFrame.Navigate(new Uri("Views/Query/QueryView.xaml", UriKind.Relative));
                        break;
                }
            }
        }
    }
}

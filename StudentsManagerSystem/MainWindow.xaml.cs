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

namespace StudentsManagerSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            txtCurrentTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton button)
            {
                switch (button.Name)
                {
                    case "btnHome":
                        txtPageTitle.Text = "欢迎使用学生管理系统";
                        MainFrame.Content = null;
                        break;
                    case "btnStudentArchive":
                        txtPageTitle.Text = "学生档案管理";
                        MainFrame.Navigate(new Uri("Views/StudentArchive/StudentArchiveView.xaml", UriKind.Relative));
                        break;
                    case "btnStudentStatus":
                        txtPageTitle.Text = "学生学籍管理";
                        MainFrame.Navigate(new Uri("Views/StudentStatus/StudentStatusView.xaml", UriKind.Relative));
                        break;
                    case "btnScoreManage":
                        txtPageTitle.Text = "学生成绩管理";
                        MainFrame.Navigate(new Uri("Views/Score/ScoreView.xaml", UriKind.Relative));
                        break;
                    case "btnBasicData":
                        txtPageTitle.Text = "基础数据管理";
                        MainFrame.Navigate(new Uri("Views/BasicData/BasicDataView.xaml", UriKind.Relative));
                        break;
                    case "btnQuery":
                        txtPageTitle.Text = "信息查询统计";
                        MainFrame.Navigate(new Uri("Views/Query/QueryView.xaml", UriKind.Relative));
                        break;
                }
            }
        }
    }
}
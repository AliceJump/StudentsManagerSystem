using System.Collections.ObjectModel;
using System.Windows.Input;
using StudentsManagerSystem.Common;

namespace StudentsManagerSystem.ViewModels
{
    /// <summary>
    /// 主窗口视图模型。
    /// </summary>
    public sealed class MainViewModel : ViewModelBase
    {
        private string currentPageTitle = "欢迎使用学生管理系统";
        private string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        public MainViewModel()
        {
            MenuItems = new ObservableCollection<string>
            {
                "首页",
                "学生档案管理",
                "学生学籍管理",
                "学生成绩管理",
                "基础数据管理",
                "信息查询统计"
            };

            SelectModuleCommand = new RelayCommand(parameter =>
            {
                if (parameter is string title && !string.IsNullOrWhiteSpace(title))
                {
                    CurrentPageTitle = title;
                }
            });

            UpdateTimeCommand = new RelayCommand(_ => CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public ObservableCollection<string> MenuItems { get; }

        public string CurrentPageTitle
        {
            get => currentPageTitle;
            set => SetProperty(ref currentPageTitle, value);
        }

        public string CurrentTime
        {
            get => currentTime;
            set => SetProperty(ref currentTime, value);
        }

        public ICommand SelectModuleCommand { get; }

        public ICommand UpdateTimeCommand { get; }
    }
}
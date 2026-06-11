using System.Configuration;
using System.Data;
using System.Windows;
using StudentsManagerSystem.Common;
using StudentsManagerSystem.Data;
using StudentsManagerSystem.Data.SqlServer;

namespace StudentsManagerSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppLogger.Info("应用启动。");

            DispatcherUnhandledException += (_, args) =>
            {
                AppLogger.Error("UI 线程未处理异常。", args.Exception);
                MessageBox.Show($"发生未处理异常：{args.Exception.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };

            AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            {
                if (args.ExceptionObject is Exception exception)
                {
                    AppLogger.Error("非 UI 线程未处理异常。", exception);
                }
            };
            
            try
            {
                DatabaseInitializer.Initialize();
                AppLogger.Info("数据库初始化完成。");
            }
            catch (Exception ex)
            {
                AppLogger.Error("数据库初始化失败。", ex);
                MessageBox.Show($"数据库初始化失败：{ex.Message}", "启动错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(-1);
                return;
            }
            
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var login = new Views.Login.LoginWindow();
            var ok = login.ShowDialog();
            if (ok != true)
            {
                Shutdown();
                return;
            }

            base.OnStartup(e);
            var main = new MainWindow();
            if (!string.IsNullOrEmpty(login.DisplayName))
            {
                main.Title = $"学生管理系统 - {login.DisplayName}";
            }
            MainWindow = main;
            main.Show();
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            AppLogger.Info("应用启动完成。");
        }
    }

}

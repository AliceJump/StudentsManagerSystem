using System.Configuration;
using System.Data;
using System.Windows;
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
            Console.WriteLine("[APP] ========== 应用启动 ==========");
            
            try
            {
                Console.WriteLine("[APP] 开始数据库初始化...");
                SqlServerDatabaseInitializer.Initialize();
                Console.WriteLine("[APP] 数据库初始化完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[APP] 初始化异常: {ex.GetType().Name}");
                Console.WriteLine($"[APP] 错误信息: {ex.Message}");
                Console.WriteLine($"[APP] 详细: {ex.StackTrace}");
            }
            
            Console.WriteLine("[APP] 调用 base.OnStartup");
            base.OnStartup(e);
            Console.WriteLine("[APP] ========== 启动完成 ==========");
        }
    }

}

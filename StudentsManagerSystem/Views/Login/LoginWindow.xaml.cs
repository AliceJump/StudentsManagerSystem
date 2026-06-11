using System;
using System.Windows;
using StudentsManagerSystem.Common;
using StudentsManagerSystem.Data.SqlServer;

namespace StudentsManagerSystem.Views.Login
{
    public partial class LoginWindow : Window
    {
        private readonly UsersRepository repo = new UsersRepository();

        public string? DisplayName { get; private set; }

        public string? Role { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            txtUsername.Focus();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var user = txtUsername.Text.Trim();
            var pwd = txtPassword.Password ?? string.Empty;
            if (string.IsNullOrEmpty(user))
            {
                txtError.Text = "请输入用户名";
                return;
            }

            try
            {
                if (repo.ValidateCredentials(user, pwd, out var display, out var role))
                {
                    DisplayName = display ?? user;
                    Role = role;
                    AppLogger.Info($"用户登录成功：{user}，角色={Role}");
                    MessageBox.Show($"登录成功：{DisplayName}", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    AppLogger.Warn($"用户登录失败：{user}");
                    txtError.Text = "用户名或密码错误";
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error($"用户登录异常：{user}", ex);
                txtError.Text = "登录失败：" + ex.Message;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

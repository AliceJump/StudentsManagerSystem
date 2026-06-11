using System;
using System.Windows;
using StudentsManagerSystem.Data.SqlServer;

namespace StudentsManagerSystem.Views.Login
{
    public partial class LoginWindow : Window
    {
        private readonly UsersRepository repo = new UsersRepository();

        public string? DisplayName { get; private set; }

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
                if (repo.ValidateCredentials(user, pwd, out var display))
                {
                    DisplayName = display ?? user;
                    MessageBox.Show($"登录成功：{DisplayName}", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    txtError.Text = "用户名或密码错误";
                }
            }
            catch (Exception ex)
            {
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

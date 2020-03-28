using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Atlassian.Jira;
using JiraTask.Business;
using JiraTask.Helper;

namespace JiraTask
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void HeaderGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        public event EventHandler UserLogined;
        private void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            var projectName = ProjectNameTextBox.Text ?? string.Empty;
            var account = AccountTextBox.Text ?? string.Empty;
            var password = PasswordTextBox.Password ?? string.Empty;
            UserSettingConfigHelper.SetUserSetting(new UserSetting()
            {
                ProjectName = projectName,
                Account = account,
                Password = password
            });
            CustomUtils.ProjectName = projectName;
            CustomUtils.Account = account;
            CustomUtils.Password = password;
            UserLogined?.Invoke(this,EventArgs.Empty);
            this.Close();
        }
    }
}

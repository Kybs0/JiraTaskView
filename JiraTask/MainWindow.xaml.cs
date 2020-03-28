using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class MainWindow : Window
    {
        private const string UserOperationSection = "UserOperation";
        public MainWindow()
        {
            InitializeComponent();
            InitViewByOperationConfig();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserSettingConfigHelper.IsLogined())
            {
                var loginWindow = new LoginWindow();
            loginWindow.Owner = this;
            loginWindow.UserLogined += LoginWindow_UserLogined;
            loginWindow.ShowDialog();
            }
        }

        private void LoginWindow_UserLogined(object sender, EventArgs e)
        {

        }

        private void InitViewByOperationConfig()
        {
            var lastView = IniFileHelper.IniReadValue(UserOperationSection, CustomUtils.LastViewKey);
            var tabItems = ViewTabControl.Items.Cast<TabItem>();
            ViewTabControl.SelectedItem = tabItems.FirstOrDefault(i => i.Name == lastView);
        }

        private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Hide(sender, e);
        }
        private void Hide(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Visibility = Visibility.Hidden;
        }

        private void HeaderGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void ViewTabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ViewTabControl.SelectedItem as TabItem;
            if (!string.IsNullOrEmpty(selectedItem?.Name))
            {
                IniFileHelper.IniWriteValue(UserOperationSection, CustomUtils.LastViewKey, selectedItem.Name);
            }
        }
    }
}

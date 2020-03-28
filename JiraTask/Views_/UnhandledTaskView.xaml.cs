using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JiraTask.Business;
using JiraTask.Helper;

namespace JiraTask
{
    /// <summary>
    /// 未完成客户问题 的交互逻辑
    /// </summary>
    public partial class UnhandledTaskView : UserControl
    {
        List<UserQuestionMode> _allSearchedIssues = new List<UserQuestionMode>();
        private string _emptyModuleName = "空";
        private string _allModuleTypes = "所有";
        public UnhandledTaskView()
        {
            InitializeComponent();
            Loaded += (s, e) => { RequestJiraData(); };
        }

        private async void RequestJiraData()
        {
            IsSearching = true;
            Issues = null;

            var issues = await new JiraRequestService().GetUnhandledIssuesAsync();
            var userQuestionModes = issues.Select(i => new UserQuestionMode()
            {
                JiraKey = i.Key.Value,
                Summary = i.Summary,
                TypeName = i.Type.Name,
                Assignee = i.Assignee,
                CreateTime = i.Created?.ToString("yyyy-MM-dd HH:mm"),
                CompleteDays = DateTimeDiffHelper.DateDiff(DateTime.Now, i.Created ?? new DateTime()),
                ModuleNames = string.Join(",", i.Components.Select(j => j.Name)).Trim()
            }).OrderByDescending(i => i.CreateTime).ThenBy(i => i.JiraKey).ThenBy(I => I.Assignee).ToList();

            _allSearchedIssues = userQuestionModes.OrderByDescending(i => i.CreateTime).ThenBy(i => i.JiraKey).ThenBy(I => I.Assignee).ToList();
            Issues = _allSearchedIssues;

            var moduleTypes = userQuestionModes.Select(i => string.IsNullOrWhiteSpace(i.ModuleNames) ? _emptyModuleName : i.ModuleNames).Distinct().OrderBy(i => i).ToList();
            if (moduleTypes.Contains(_emptyModuleName))
            {
                moduleTypes.Remove(_emptyModuleName);
                moduleTypes.Insert(0, _emptyModuleName);
            }
            moduleTypes.Insert(0, _allModuleTypes);
            ModuleTypes = moduleTypes;

            IsSearching = false;
        }

        private void LinkedButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is UserQuestionMode mode)
            {
                BrowserHelper.OpenBrowserUrlWidthDefaultChrome(mode.JiraUri);
            }
        }
        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            RequestJiraData();
        }
        private void CopyMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is UserQuestionMode userQuestionMode)
            {
                Clipboard.SetDataObject($"{userQuestionMode.JiraUri} {userQuestionMode.Summary}");
            }
        }

        private void ModuleTypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ModuleTypeComboBox.SelectedItem.ToString();
            if (selectedItem == _emptyModuleName)
            {
                Issues = _allSearchedIssues?.Where(i => string.IsNullOrWhiteSpace(i.ModuleNames))?.ToList();
            }
            else if (selectedItem == _allModuleTypes)
            {
                Issues = _allSearchedIssues;
            }
            else
            {
                Issues = _allSearchedIssues?.Where(i => i.ModuleNames == selectedItem)?.ToList();
            }
        }

        #region 属性

        public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching",
            typeof(bool), typeof(UnhandledTaskView), new PropertyMetadata(default(bool)));

        public bool IsSearching
        {
            get { return (bool)GetValue(IsSearchingProperty); }
            set { SetValue(IsSearchingProperty, value); }
        }
        public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.Register(
            "Issues", typeof(List<UserQuestionMode>), typeof(UnhandledTaskView), new PropertyMetadata(default(List<UserQuestionMode>)));

        public List<UserQuestionMode> Issues
        {
            get { return (List<UserQuestionMode>)GetValue(PropertyTypeProperty); }
            set { SetValue(PropertyTypeProperty, value); }
        }

        public static readonly DependencyProperty ModuleTypesProperty = DependencyProperty.Register(
            "ModuleTypes", typeof(List<string>), typeof(UnhandledTaskView), new PropertyMetadata(default(List<string>)));

        public List<string> ModuleTypes
        {
            get { return (List<string>)GetValue(ModuleTypesProperty); }
            set { SetValue(ModuleTypesProperty, value); }
        }

        #endregion
    }
}

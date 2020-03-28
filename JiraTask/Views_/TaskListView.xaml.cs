using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Atlassian.Jira;
using JiraTask.Business;
using JiraTask.Controls;
using JiraTask.Helper;
using JiraTask.Business;

namespace JiraTask
{
    /// <summary>
    /// 未完成客户问题 的交互逻辑
    /// </summary>
    public partial class TaskListView : UserControl
    {
        public TaskListView()
        {
            InitializeComponent();
            Loaded += (s, e) => { RequestJiraData(); };
        }

        private void LinkedButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is UserQuestionMode mode)
            {
                BrowserHelper.OpenBrowserUrlWidthDefaultChrome(mode.JiraUri);
            }
        }

        #region 查询

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            RequestJiraData();
        }

        private List<UserQuestionMode> _lastSearchedIssues;
        private async void RequestJiraData()
        {
            IsSearching = true;
            Issues = null;

            var issuesResult = await new JiraRequestService().GetCurrentSprintIssuesAsync();
            var userQuestionModes = new List<UserQuestionMode>();
            foreach (var issue in issuesResult)
            {
                var userQuestion = new UserQuestionMode()
                {
                    JiraKey = issue.Key.Value,
                    Priority = issue.Priority.Name,
                    TypeName = issue.Type.Name,
                    Summary = issue.Summary,
                    Description = issue.Description,
                    Assignee = issue.Assignee,
                    CreateTime = issue.Created?.ToString("yyyy-MM-dd HH:mm"),
                    CompleteDays = DateTimeDiffHelper.DateDiff(issue.ResolutionDate ?? new DateTime(), issue.Created ?? new DateTime()),
                    IsCompleted = IsIssueCompleted(issue.Status),
                    Status = issue.Status.Name,
                    ResolutionDate = issue.ResolutionDate?.ToString("yyyy-MM-dd HH:mm"),
                    Creator = issue.Reporter,
                    OriginalEstimate = issue.TimeTrackingData.OriginalEstimate,
                    TimeSpent = issue.TimeTrackingData.TimeSpent
                };
                // get comments
                var comments = await issue.GetCommentsAsync();
                var commentBodyList = comments.Select(i => i.Body).ToList();
                userQuestion.Comment = commentBodyList;
                userQuestionModes.Add(userQuestion);
            }
            _lastSearchedIssues = userQuestionModes.OrderBy(i => i.SortingStatus).ToList();

            var statusList = userQuestionModes.Select(i => i.Status).Distinct().ToList();
            statusList.Insert(0, "所有");
            IssueStatusTypes = statusList;

            SetCurrentIssues(SearchJiraTextBox.SearchedText, SearchTypeComboBox.Text);
            //issues = issues.Where(i => i.Priority.Id == "1").ToList();
            //var sum = itemsSource.Sum(i => i.CompleteDays);
            IsSearching = false;
        }

        private List<string> _completedStatusList = new List<string>() { "无法处理", "完成", "已解决" };
        private bool IsIssueCompleted(IssueStatus argStatus)
        {
            if (_completedStatusList.Any(i => i == argStatus.Name))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region 筛选

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchTypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCurrentIssues(SearchJiraTextBox.SearchedText, SearchTypeComboBox.SelectedValue.ToString());
        }
        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchJiraTextBox_OnOnSearch(object sender, SearchEventArgs e)
        {
            SetCurrentIssues(e.SearchText, SearchTypeComboBox.Text);
        }

        private void SetCurrentIssues(string searchedText, string searchedType)
        {
            var lastSearchedIssues = _lastSearchedIssues ?? new List<UserQuestionMode>();
            var issues = string.IsNullOrEmpty(searchedType) || searchedType == "所有"
                ? lastSearchedIssues
                : lastSearchedIssues.Where(i => i.Status == searchedType).ToList();
            searchedText = searchedText ?? string.Empty;
            issues = issues.Where(i => !string.IsNullOrEmpty(i.Summary) &&
                                       (i.Summary.Contains(searchedText) ||
                                        i.Assignee.Contains(searchedText) ||
                                        i.Creator.Contains(searchedText))).OrderBy(i => i.SortingStatus).ThenBy(i => i.Status).ThenBy(i => i.Summary).ToList();
            Issues = issues;
        }

        #endregion

        #region 属性

        public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching",
            typeof(bool), typeof(TaskListView), new PropertyMetadata(default(bool)));

        public bool IsSearching
        {
            get { return (bool)GetValue(IsSearchingProperty); }
            set { SetValue(IsSearchingProperty, value); }
        }

        public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.Register(
            "Issues", typeof(List<UserQuestionMode>), typeof(TaskListView), new PropertyMetadata(default(List<UserQuestionMode>)));

        public List<UserQuestionMode> Issues
        {
            get { return (List<UserQuestionMode>)GetValue(PropertyTypeProperty); }
            set { SetValue(PropertyTypeProperty, value); }
        }

        public static readonly DependencyProperty IssueStatusTypesProperty = DependencyProperty.Register(
            "IssueStatusTypes", typeof(List<string>), typeof(TaskListView), new PropertyMetadata(default(List<string>)));

        public List<string> IssueStatusTypes
        {
            get { return (List<string>)GetValue(IssueStatusTypesProperty); }
            set { SetValue(IssueStatusTypesProperty, value); }
        }
        //public List<string> IssueStatusTypes => new List<string>() { "所有", "新建", "无法处理", "完成", "已解决" };

        #endregion

        #region 其它

        private void CopyMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is UserQuestionMode userQuestionMode)
            {
                Clipboard.SetDataObject($"{userQuestionMode.JiraUri} {userQuestionMode.Summary}");
            }
        }

        private void CopyUriMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is UserQuestionMode userQuestionMode)
            {
                Clipboard.SetDataObject($"{userQuestionMode.JiraUri}");
            }
        }

        private void CopyMergeInfoItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is UserQuestionMode userQuestionMode)
            {
                Clipboard.SetDataObject($"{userQuestionMode.Summary} {userQuestionMode.JiraKey}");
            }
        }

        #endregion

    }
}

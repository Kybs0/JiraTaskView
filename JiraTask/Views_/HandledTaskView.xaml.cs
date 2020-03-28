using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
using JiraTask.Models;

namespace JiraTask
{
    /// <summary>
    /// 已完成客户问题列表 的交互逻辑
    /// </summary>
    public partial class HandledTaskView : UserControl
    {
        List<UserQuestionMode> _allSearchedIssues = new List<UserQuestionMode>();
        private string _emptyModuleName = "空";
        private string _allModuleTypes = "所有";
        /// <summary>
        /// 完成客户问题展示列表
        /// </summary>
        public HandledTaskView()
        {
            InitializeComponent();
            SelectedPeriodType = DuePeriodType.昨日;
        }

        private async void RequestJiraByDuePeriodType(DuePeriodType selectedPeriodType)
        {
            var issuesTask = GetSolvedIssuesByCurrentAuthor(selectedPeriodType);
            await RequestJiraData(issuesTask);
        }

        private async void RequestJiraByDatePeriod(DateTime startDate, DateTime endDate)
        {
            var issuesTask = GetSolvedIssuesByCurrentAuthor(startDate, endDate);
            await RequestJiraData(issuesTask);
        }

        private async Task RequestJiraData(Task<List<(DateTime userUpdatedTime, Issue issue)>> issuesTask)
        {
            IsSearching = true;
            Issues = null;

            var issues = await issuesTask;
            var userQuestionModes = new List<UserQuestionMode>();
            foreach (var issueData in issues)
            {
                var issue = issueData.issue;
                var actualUpdateTime = issueData.userUpdatedTime;
                var userQuestion = new UserQuestionMode()
                {
                    JiraKey = issue.Key.Value,
                    Priority = issue.Priority.Name,
                    TypeName = issue.Type.Name,
                    Summary = issue.Summary,
                    Description = issue.Description,
                    Assignee = issue.Assignee,
                    ResolutionDate = issue.ResolutionDate?.ToString("yyyy-MM-dd HH:mm"),
                    Status = issue.Status.Name,
                    CompleteDays = DateTimeDiffHelper.DateDiff(issue.ResolutionDate ?? new DateTime(), issue.Created ?? new DateTime()),
                    ModuleNames = string.Join(",", issue.Components.Select(i => i.Name)).Trim(),
                    TimeSpent = issue.TimeTrackingData.TimeSpent,
                    TimeSpentInSeconds = issue.TimeTrackingData.TimeSpentInSeconds,
                    FinishTime = actualUpdateTime,
                };
                // get comments
                var comments = await issue.GetCommentsAsync();
                var commentBodyList = comments.Select(i => i.Body).ToList();
                userQuestion.Comment = commentBodyList;
                userQuestionModes.Add(userQuestion);
            }

            _allSearchedIssues = userQuestionModes.ToList();
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

        /// <summary>
        /// 获取当前用户解决的问题列表
        /// 解决:"验收", "测试中", "无法处理", "完成", "已解决"（暂时泛型，之后可以考虑以单个类型的状态来确定解决状态）
        /// </summary>
        /// <returns></returns>
        private async Task<List<(DateTime userUpdatedTime, Issue issue)>> GetSolvedIssuesByCurrentAuthor(DateTime startDate, DateTime endDate)
        {
            var resultList = new List<(DateTime userUpdatedTime, Issue issue)>();
            var solvedStatusList = new List<string>() { "验收", "测试中", "无法处理", "完成", "已解决" };
            //获取对应时间范围开始后的“完成”问题列表
            //为何通过时间范围起始之后来获取问题？因为用户变更为“完成”状态，问题真正关闭的时间在这之后。
            var issues = await new JiraRequestService().GetHandledIssuesAfterTimeAsync(startDate, endDate, solvedStatusList);
            foreach (var issue in issues)
            {
                //通过修改日志，查找当前作者的Transition。
                var changeLogs = await issue.GetChangeLogsAsync();
                //获取当前作者,在时间段范围内，是否有主动变更状态为角色对应完成状态的日志
                var currentUserStatusLogs = changeLogs.ToList().Where(i => i.Author.Username == CustomUtils.Account
                                                                           //&& DueTimeTrackingHelper.IsInDuePeriodRange(i.CreatedDate, selectedPeriodType)
                                                                           && i.Items.Any(item => item.FieldName == "status" && solvedStatusList.Contains(item.ToValue))).ToList();
                currentUserStatusLogs = currentUserStatusLogs
                    .Where(i => i.CreatedDate >= startDate && i.CreatedDate <= endDate).ToList();

                //如果此段时间范围内作者变更为完成状态，那么当前处于“完成”状态的问题，是作者完成的。
                if (currentUserStatusLogs.Count > 0)
                {
                    var lastStatusLog = currentUserStatusLogs.OrderBy(i => i.CreatedDate).Last();
                    resultList.Add((lastStatusLog.CreatedDate, issue));
                }
            }

            var orderedResultList = resultList.OrderBy(i => i.userUpdatedTime).ToList();
            return orderedResultList;
        }



        /// <summary>
        /// 获取当前用户解决的问题列表
        /// 解决:"验收", "测试中", "无法处理", "完成", "已解决"（暂时泛型，之后可以考虑以单个类型的状态来确定解决状态）
        /// </summary>
        /// <param name="selectedPeriodType"></param>
        /// <returns></returns>
        private async Task<List<(DateTime userUpdatedTime, Issue issue)>> GetSolvedIssuesByCurrentAuthor(DuePeriodType selectedPeriodType)
        {
            var resultList = new List<(DateTime userUpdatedTime, Issue issue)>();
            var solvedStatusList = new List<string>() { "验收", "测试中", "无法处理", "完成", "已解决" };
            //获取对应时间范围开始后的“完成”问题列表
            //为何通过时间范围起始之后来获取问题？因为用户变更为“完成”状态，问题真正关闭的时间在这之后。
            var issues = await new JiraRequestService().GetHandledIssuesAfterTimeAsync(selectedPeriodType, solvedStatusList);
            foreach (var issue in issues)
            {
                //通过修改日志，查找当前作者的Transition。
                var changeLogs = await issue.GetChangeLogsAsync();
                //获取当前作者,在时间段范围内，是否有主动变更状态为角色对应完成状态的日志
                var currentUserStatusLogs = changeLogs.ToList().Where(i => i.Author.Username == CustomUtils.Account
                                                                           //&& DueTimeTrackingHelper.IsInDuePeriodRange(i.CreatedDate, selectedPeriodType)
                                                                           && i.Items.Any(item => item.FieldName == "status" && solvedStatusList.Contains(item.ToValue))).ToList();
                currentUserStatusLogs = currentUserStatusLogs
                    .Where(i => DueTimeTrackingHelper.IsInDuePeriodRange(i.CreatedDate, selectedPeriodType)).ToList();
                //如果此段时间范围内作者变更为完成状态，那么当前处于“完成”状态的问题，是作者完成的。
                if (currentUserStatusLogs.Count > 0)
                {
                    var lastStatusLog = currentUserStatusLogs.OrderBy(i => i.CreatedDate).Last();
                    resultList.Add((lastStatusLog.CreatedDate, issue));
                }
            }

            var orderedResultList = resultList.OrderBy(i => i.userUpdatedTime).ToList();
            return orderedResultList;
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
        private void LinkedButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is UserQuestionMode mode)
            {
                BrowserHelper.OpenBrowserUrlWidthDefaultChrome(mode.JiraUri);
            }
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            RequestJiraByDuePeriodType(SelectedPeriodType);
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

        public List<DuePeriodType> DueWeekTypeList { get; set; } = new List<DuePeriodType>()
        {
            DuePeriodType.今日,DuePeriodType.昨日,DuePeriodType.本周,DuePeriodType.上周,DuePeriodType.上上周,DuePeriodType.本月,DuePeriodType.上月,DuePeriodType.自定义,
        };

        public static readonly DependencyProperty SelectedPeriodTypeProperty = DependencyProperty.Register(
            "SelectedPeriodType", typeof(DuePeriodType), typeof(HandledTaskView),
            new PropertyMetadata(default(DuePeriodType), OnSelectedWeekTypePropertyChanged));

        private static void OnSelectedWeekTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HandledTaskView userHandledQuestionView && e.NewValue is DuePeriodType selectedWeekType)
            {
                if (selectedWeekType == DuePeriodType.自定义)
                {
                    userHandledQuestionView.UserDefinedDateRangePanel.Visibility = Visibility.Visible;
                }
                else
                {
                    userHandledQuestionView.UserDefinedDateRangePanel.Visibility = Visibility.Collapsed;
                    userHandledQuestionView.RequestJiraByDuePeriodType(selectedWeekType);
                }
            }
        }

        public static readonly DependencyProperty ModuleTypesProperty = DependencyProperty.Register(
            "ModuleTypes", typeof(List<string>), typeof(HandledTaskView), new PropertyMetadata(default(List<string>)));

        public List<string> ModuleTypes
        {
            get { return (List<string>)GetValue(ModuleTypesProperty); }
            set { SetValue(ModuleTypesProperty, value); }
        }

        public DuePeriodType SelectedPeriodType
        {
            get { return (DuePeriodType)GetValue(SelectedPeriodTypeProperty); }
            set { SetValue(SelectedPeriodTypeProperty, value); }
        }

        public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching",
            typeof(bool), typeof(HandledTaskView), new PropertyMetadata(default(bool)));

        public bool IsSearching
        {
            get { return (bool)GetValue(IsSearchingProperty); }
            set { SetValue(IsSearchingProperty, value); }
        }
        public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.Register(
            "Issues", typeof(List<UserQuestionMode>), typeof(HandledTaskView), new PropertyMetadata(default(List<UserQuestionMode>)));

        public List<UserQuestionMode> Issues
        {
            get { return (List<UserQuestionMode>)GetValue(PropertyTypeProperty); }
            set { SetValue(PropertyTypeProperty, value); }
        }

        #endregion

        #region 右键复制

        private void CopyTextMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var userQuestionModes = HandledTaskListBox.SelectedItems.OfType<UserQuestionMode>().ToList();
            string clipboardText = string.Empty;
            foreach (var userQuestionMode in userQuestionModes)
            {
                clipboardText += $"{userQuestionMode.Summary}\r\n";
            }
            Clipboard.SetDataObject(clipboardText);
        }

        private void CopyLinkItem_OnClick(object sender, RoutedEventArgs e)
        {
            var userQuestionModes = HandledTaskListBox.SelectedItems.OfType<UserQuestionMode>().ToList();
            string clipboardText = string.Empty;
            foreach (var userQuestionMode in userQuestionModes)
            {
                clipboardText += $"{userQuestionMode.JiraUri}\r\n";
            }
            Clipboard.SetDataObject(clipboardText);
        }
        private void CopyMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var userQuestionModes = HandledTaskListBox.SelectedItems.OfType<UserQuestionMode>().ToList();
            string clipboardText = string.Empty;
            foreach (var userQuestionMode in userQuestionModes)
            {
                clipboardText += $"{userQuestionMode.JiraUri} {userQuestionMode.Summary}\r\n";
            }
            Clipboard.SetDataObject(clipboardText);
        }

        #endregion

        private void DatePickerButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                return;
            }

            RequestJiraByDatePeriod(StartDatePicker.SelectedDate.Value, EndDatePicker.SelectedDate.Value);
        }
    }
}

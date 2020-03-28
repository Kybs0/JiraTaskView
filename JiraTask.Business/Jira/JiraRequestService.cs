using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTask.Helper;

namespace JiraTask.Business
{
    public class JiraRequestService
    {
        private const string JiraCvteCom = "https://jira.cvte.com";

        public async Task<List<Issue>> GetCurrentSprintIssuesAsync()
        {
            var requestText = $"project = {CustomUtils.ProjectName} AND Assignee = {CustomUtils.Account} AND Sprint in openSprints() ";
            requestText = $"{requestText} ORDER BY Rank ASC";
            var issues = await JiraConnectionHelper.RequestAsync(JiraCvteCom, requestText);
            return issues;
        }

        /// <summary>
        /// 获取对应时间段新建的客户问题列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Issue>> GetIssuesByPeriodAsync(DuePeriodType periodType)
        {
            var timeFilter = DueTimeTrackingHelper.GetTimeFilter(periodType, "ResolutionDate");
            var requestText = $"project = {CustomUtils.ProjectName} AND Assignee = {CustomUtils.Account}";
            if (!string.IsNullOrEmpty(timeFilter))
            {
                requestText = $"{requestText} AND {timeFilter}";
            }
            requestText = $"{requestText} ORDER BY Rank ASC";
            var issues = await JiraConnectionHelper.RequestAsync(JiraCvteCom, requestText);
            return issues;
        }

        /// <summary>
        /// 获取当前未完成的客户问题列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Issue>> GetUnhandledIssuesAsync()
        {
            var requestText = $"project = {CustomUtils.ProjectName} AND Assignee = {CustomUtils.Account} AND status in (新建, 处理中) ORDER BY Rank ASC";
            var issues = await JiraConnectionHelper.RequestAsync(JiraCvteCom, requestText);

            return issues;
        }

        /// <summary>
        /// 获取已完成的客户问题数
        /// </summary>
        /// <param name="periodType">查看周次类型</param>
        /// <returns></returns>
        public async Task<List<Issue>> GetHandledIssuesByWeekAsync(DuePeriodType periodType, List<string> solvedStatusList)
        {
            //使用更新日期（因为测试中、验收等的问题，并没有解决，所以没有解决日期）
            var timeFilter = DueTimeTrackingHelper.GetTimeFilter(periodType, "updatedDate");
            var requestText = $"project = {CustomUtils.ProjectName} AND Assignee = {CustomUtils.Account} AND status in ({string.Join(",", solvedStatusList)})";
            if (!string.IsNullOrEmpty(timeFilter))
            {
                requestText = $"{requestText} AND {timeFilter}";
            }
            requestText = $"{requestText} ORDER BY Rank ASC";
            var issues = await JiraConnectionHelper.RequestAsync(JiraCvteCom, requestText);
            return issues;
        }
        /// <summary>
        /// 获取已完成的客户问题数
        /// </summary>
        /// <returns></returns>
        public async Task<List<Issue>> GetHandledIssuesAfterTimeAsync(DuePeriodType periodType, List<string> solvedStatusList)
        {
            //使用更新日期（因为测试中、验收等的问题，并没有解决，所以没有解决日期）
            var timeFilter = DueTimeTrackingHelper.GetAfterTimeFilter(periodType, "updatedDate");
            var requestText = $"project = {CustomUtils.ProjectName} AND Assignee = {CustomUtils.Account} AND status in ({string.Join(",", solvedStatusList)})";
            if (!string.IsNullOrEmpty(timeFilter))
            {
                requestText = $"{requestText} AND {timeFilter}";
            }
            requestText = $"{requestText} ORDER BY Rank ASC";
            var issues = await JiraConnectionHelper.RequestAsync(JiraCvteCom, requestText);
            return issues;
        }
    }

    public enum DuePeriodType
    {
        今日,
        昨日,
        本周,
        上周,
        上上周,
        上上上周,
        本月,
        上月,
        去年,
        今年,
        自定义
    }
}

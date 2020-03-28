using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTask.Helper;

namespace JiraTask.Business
{
    public static class JiraConnectionHelper
    {
        /// <summary>
        /// 请求Issues
        /// </summary>
        /// <param name="jiraUri">公司/组织的Jira地址，如https://jira.huawei.com</param>
        /// <param name="jqlText"></param>
        /// <returns></returns>
        public static async Task<List<Issue>> RequestAsync(string jiraUri, string jqlText)
        {
            IPagedQueryResult<Issue> issues = null;
            try
            {
                //之前的版本，Atlassian已经弃用
                // Jira jira = new Jira(url, "admin", "password");
                var jira = Jira.CreateRestClient(jiraUri, CustomUtils.Account, CustomUtils.Password);
                //GetIssuesFromJqlAsync相当于 $"{jiraUri}/rest/api/2/search?jql={requestText}";
                issues = await jira.Issues.GetIssuesFromJqlAsync(jqlText, 100000);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return issues?.ToList() ?? new List<Issue>();
        }
    }
}

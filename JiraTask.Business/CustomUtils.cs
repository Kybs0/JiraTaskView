using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraTask.Helper
{
    public static class CustomUtils
    {
        public static string ProjectName { get; set; }
        public static string Account { get; set; } = string.Empty;
        public static string Password { get; set; } = string.Empty;

        public const string LastViewKey = "LastView";

    }
}

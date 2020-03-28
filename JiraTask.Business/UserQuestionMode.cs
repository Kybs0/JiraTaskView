using System;
using System.Collections.Generic;
using System.Linq;

namespace JiraTask.Business
{
    public class UserQuestionMode
    {
        public string JiraUri => "https://jira.cvte.com/browse/" + JiraKey;
        public string JiraKey { get; set; }
        public string Summary { get; set; }
        public string Assignee { get; set; }
        public string CreateTime { get; set; }

        public string ResolutionDate { get; set; }
        public int CompleteDays { get; set; }
        public bool IsCompleted { get; set; }
        public string Status { get; set; }

        public JiraSortingStatus SortingStatus=>new JiraSortingStatus(Status);

        /// <summary>
        /// 备注
        /// </summary>
        public List<string> Comment { get; set; }

        /// <summary>
        /// 备注显示
        /// </summary>
        public string DisplayComment
        {
            get
            {
                var questionInfoFromComment = JiraHelper.AnalysisCommentData(Comment);

                var displayComment = string.Empty;
                if (!string.IsNullOrEmpty(questionInfoFromComment.Reason))
                {
                    displayComment += $"原因：{questionInfoFromComment.Reason}";
                }
                if (!string.IsNullOrEmpty(questionInfoFromComment.Solution))
                {
                    if (!string.IsNullOrWhiteSpace(displayComment))
                    {
                        displayComment += "\n";
                    }
                    displayComment += $"解决：{questionInfoFromComment.Solution}";
                }
                if (!string.IsNullOrEmpty(questionInfoFromComment.ScopeInfluence))
                {
                    if (!string.IsNullOrWhiteSpace(displayComment))
                    {
                        displayComment += "\n";
                    }
                    displayComment += $"影响范围：{questionInfoFromComment.ScopeInfluence}";
                }

                return displayComment;
            }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public string Priority { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleNames { get; set; }
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        public string OriginalEstimate { get; set; }
        /// <summary>
        /// 耗时
        /// </summary>
        public string TimeSpent { get; set; }
        public int? TimeSpentInSeconds { get; set; }
        /// <summary>
        /// 操作人在对应角色的完成时间
        /// </summary>
        public DateTime FinishTime { get; set; }
    }
}

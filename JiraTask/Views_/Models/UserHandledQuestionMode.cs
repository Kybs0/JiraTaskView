using System.Collections.Generic;
using System.Linq;
using JiraTask.Business;

namespace JiraTask.Models
{
    public class UserHandledQuestionMode
    {
        public string Assignee { get; set; }
        public int HandledCount => QuestionList.Count;
        public List<QuestionDetailMode> QuestionList { get; set; } = new List<QuestionDetailMode>();
        public int PerformanceBeans { get; set; }

        public void UpdateBeans()
        {
            var performanceBeans = 0;
            foreach (var questionDetailMode in QuestionList)
            {
                switch (questionDetailMode.Status)
                {
                    case QuestionStatus.无法处理:
                        {
                            performanceBeans += 1;
                        }
                        break;
                    case QuestionStatus.完成:
                        {
                            if (questionDetailMode.CompleteDays < 3)
                            {
                                performanceBeans += 3;
                            }
                            else if (questionDetailMode.CompleteDays < 5)
                            {
                                performanceBeans += 2;
                            }
                            else
                            {
                                performanceBeans += 1;
                            }
                        }
                        break;
                    case QuestionStatus.已解决:
                        {
                            if (questionDetailMode.CompleteDays < 3)
                            {
                                performanceBeans += 4;
                            }
                            else if (questionDetailMode.CompleteDays < 5)
                            {
                                performanceBeans += 3;
                            }
                            else
                            {
                                performanceBeans += 2;
                            }
                        }
                        break;
                }
            }

            PerformanceBeans = performanceBeans;
        }
    }

    public class QuestionDetailMode
    {
        public string JiraUri => "https://jira.cvte.com/browse/" + JiraKey;
        public string JiraKey { get; set; }
        public QuestionStatus Status { get; set; }
        public int CompleteDays { get; set; }
    }
    public enum QuestionStatus
    {
        新建,
        处理中,
        完成,
        已解决,
        无法处理
    }
}

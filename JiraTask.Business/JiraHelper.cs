using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTask.Business
{
    public static class JiraHelper
    {
        public static QuestionInfoFromComment AnalysisCommentData(List<string> questionComments)
        {
            var questionInfoFromComment = new QuestionInfoFromComment();
            if (questionComments != null && questionComments.Count > 0)
            {
                List<string> lines = new List<string>();
                questionComments.ForEach(comment => lines.AddRange(comment.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList()));
                string contactInfoLine = string.Empty;
                if (lines.Any(i => ContainsKey(i, "原因")))
                {
                    questionInfoFromComment.Reason = GetContentFromKey(lines, "原因", out contactInfoLine);
                }
                else if (lines.Any(i => ContainsKey(i, "why")))
                {
                    questionInfoFromComment.Reason = GetContentFromKey(lines, "why", out contactInfoLine);
                }

                if (lines.Any(i => i.Contains("解决方案")))
                {
                    questionInfoFromComment.Solution = GetContentFromKey(lines, "解决方案", out contactInfoLine);
                }
                else if (lines.Any(i => i.Contains("解决:")))
                {
                    questionInfoFromComment.Solution = GetContentFromKey(lines, "解决:", out contactInfoLine);
                }
                if (lines.Any(i => i.Contains("解决：")))
                {
                    questionInfoFromComment.Solution = GetContentFromKey(lines, "解决：", out contactInfoLine);
                }
                else if (lines.Any(i => i.Contains("how")))
                {
                    questionInfoFromComment.Solution = GetContentFromKey(lines, "how", out contactInfoLine);
                }

                if (lines.Any(i => i.Contains("影响范围")))
                {
                    questionInfoFromComment.ScopeInfluence = GetContentFromKey(lines, "影响范围", out contactInfoLine);
                }
            }

            return questionInfoFromComment;
        }

        public static string GetContentFromKey(List<string> list, string key, out string contactInfoLine)
        {
            contactInfoLine = list.First(i => i.Contains(key));
            var reason = contactInfoLine.RemovesKey(key);
            if (string.IsNullOrEmpty(reason))
            {
                var indexOfLine = list.IndexOf(contactInfoLine);
                if (indexOfLine + 1 < list.Count && !(list[indexOfLine + 1].ContainsKey("原因") || list[indexOfLine + 1].ContainsKey("解决方案") || list[indexOfLine + 1].ContainsKey("影响范围")))
                {
                    reason = list[indexOfLine + 1];
                }
            }

            return reason;
        }

        public static QuestionInfoFromDescription AnalysisDescriptionData(string questionSummary)
        {
            var questionInfoFromDescription = new QuestionInfoFromDescription();
            if (!string.IsNullOrEmpty(questionSummary))
            {
                var list = questionSummary.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList();
                string contactInfoLine = string.Empty;
                if (list.Any(i => i.Contains("联系方式")))
                {
                    contactInfoLine = list.First(i => i.Contains("联系方式"));
                    questionInfoFromDescription.ContactInformation = contactInfoLine.RemovesKey("联系方式").Replace("用户", string.Empty).Replace("的", string.Empty);
                }
                else if (list.Any(i => i.Contains("用户")))
                {
                    contactInfoLine = list.First(i => i.Contains("用户"));
                    questionInfoFromDescription.ContactInformation = contactInfoLine.RemovesKey("用户").Replace("的", string.Empty);
                }

                if (!string.IsNullOrEmpty(contactInfoLine))
                {
                    list.Remove(contactInfoLine);
                }

                var newList = new List<string>();
                foreach (var line in list)
                {
                    var removedImageLine = RemovedImage(line);
                    if (!string.IsNullOrWhiteSpace(removedImageLine))
                    {
                        newList.Add(removedImageLine);
                    }
                }
                questionInfoFromDescription.Description = string.Join("\r\n", newList);
            }

            return questionInfoFromDescription;
        }

        public static bool ContainsKey(this string content, string key)
        {
            if (content.Contains($"{key}:") || content.Contains($"{key}：")
                                  || content.Contains($"【{key}】") || content.Contains($"[{key}]"))
            {
                return true;
            }
            return false;
        }

        public static string RemovesKey(this string content, string key)
        {
            var removedContent = content.Replace(key, string.Empty).Replace(":", string.Empty).Replace("：", string.Empty)
                .Replace("【", string.Empty).Replace("】", string.Empty)
                .Replace("[", string.Empty).Replace("]", string.Empty).Trim();
            //!image-2019-04-30-14-45-03-437.png!
            removedContent = RemovedImage(removedContent);

            return removedContent;
        }

        public static string RemovedImage(string removedContent)
        {
            while (removedContent.Contains("!image") && removedContent.Contains(".png!"))
            {
                var startIndex = removedContent.IndexOf("!image");
                var endIndex = removedContent.IndexOf(".png!");
                var toRemovedStr = removedContent.Substring(startIndex, endIndex - startIndex + ".png!".Length);
                removedContent = removedContent.Replace(toRemovedStr, string.Empty);
            }

            return removedContent;
        }

        public static string GetPorityName(string questionPriority)
        {
            if (questionPriority == "最高")
            {
                return "P0";
            }
            else if (questionPriority == "高")
            {
                return "P1";
            }
            else if (questionPriority == "中")
            {
                return "P2";
            }
            else if (questionPriority == "低")
            {
                return "P3";
            }
            else if (questionPriority == "最低")
            {
                return "P4";
            }

            return string.Empty;
        }
    }
    public class QuestionInfoFromComment
    {
        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 解决方案
        /// </summary>
        public string Solution { get; set; }
        //影响范围
        public string ScopeInfluence { get; set; }
    }

    public class QuestionInfoFromDescription
    {
        public string Description { get; set; }
        public string ContactInformation { get; set; }
    }
}

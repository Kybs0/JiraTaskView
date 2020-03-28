using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTask.Business
{
    public class JiraSortingStatus : IComparable
    {
        public string Status { get; set; }

        public int CompareTo(object obj)
        {
            var sortingStatus1Order = _sortedStatusList.Any(i => i == this.Status)
                ? _sortedStatusList.IndexOf(this.Status)
                : -1;

            var sortingStatus2Order = _sortedStatusList.Any(i => i == ((JiraSortingStatus)obj).Status)
                ? _sortedStatusList.IndexOf(((JiraSortingStatus)obj).Status)
                : -1;

            return sortingStatus1Order > sortingStatus2Order ? 1 : -1;
        }
        List<string> _sortedStatusList = new List<string>() { "新建", "待需求评审", "待设计评审", "待开发", "重新打开", "开发中", "处理中", "验收", "测试中", "完成", "已解决" };

        public JiraSortingStatus(string status)
        {
            Status = status;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTask.Helper
{
    public static class DateTimeDiffHelper
    {
        public static int DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            try
            {
                TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                return ts.Days;
            }
            catch
            {
            }
            return 0;
        }
    }
}

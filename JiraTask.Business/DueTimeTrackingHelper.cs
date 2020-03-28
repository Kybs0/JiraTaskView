using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTask.Business
{
    public static class DueTimeTrackingHelper
    {
        /// <summary>
        /// 获取时间筛选参数
        /// </summary>
        /// <param name="periodType"></param>
        /// <param name="searchParameter"></param>
        /// <returns></returns>
        public static string GetTimeFilter(DuePeriodType periodType, string searchParameter)
        {
            //created >= 2017 - 07 - 20 AND created <= now()
            var timeFilter = string.Empty;
            var dateTimeNow = DateTime.Now;
            switch (periodType)
            {
                case DuePeriodType.今日:
                    {
                        //今日  
                        string startDay = dateTimeNow.ToString("yyyy-MM-dd");
                        timeFilter = $"{searchParameter} >= {startDay}";
                    }
                    break;
                case DuePeriodType.昨日:
                    {
                        //今日  
                        var endDay = dateTimeNow.ToString("yyyy-MM-dd");
                        var startDay = dateTimeNow.AddDays(-1).ToString("yyyy-MM-dd");
                        timeFilter = $"{searchParameter} >= {startDay} AND {searchParameter} < {endDay}";
                    }
                    break;
                case DuePeriodType.本周:
                    {
                        var startDay = GetWeekFirstDay(dateTimeNow, 0);
                        timeFilter = $"{searchParameter} >= {startDay}";
                    }
                    break;
                case DuePeriodType.上周:
                    {
                        var endDay = GetWeekFirstDay(dateTimeNow, 0);
                        var startDay = GetWeekFirstDay(dateTimeNow, 1);
                        timeFilter = $"{searchParameter} >= {startDay} AND {searchParameter} < {endDay}";
                    }
                    break;
                case DuePeriodType.上上周:
                    {
                        var endDay = GetWeekFirstDay(dateTimeNow, 1);
                        var startDay = GetWeekFirstDay(dateTimeNow, 2);
                        timeFilter = $"{searchParameter} >= {startDay} AND {searchParameter} < {endDay}";
                    }
                    break;
                case DuePeriodType.上上上周:
                    {
                        var endDay = GetWeekFirstDay(dateTimeNow, 2);
                        var startDay = GetWeekFirstDay(dateTimeNow, 3);
                        timeFilter = $"{searchParameter} >= {startDay} AND {searchParameter} < {endDay}";
                    }
                    break;
                case DuePeriodType.本月:
                    {
                        var startDay = GetMonthFirstDay(dateTimeNow);
                        timeFilter = $"{searchParameter} >= {startDay}";
                    }
                    break;
                case DuePeriodType.上月:
                    {
                        var endDay = GetMonthFirstDay(dateTimeNow);
                        var startDay = GetMonthFirstDay(dateTimeNow, 1);
                        timeFilter = $"{searchParameter} >= {startDay} AND {searchParameter} < {endDay}";
                    }
                    break;
                case DuePeriodType.今年:
                    {
                        var startDay = GetYearFirstDay(dateTimeNow);
                        timeFilter = $"{searchParameter} >= {startDay}";
                    }
                    break;
                case DuePeriodType.去年:
                    {
                        var endDay = GetYearFirstDay(dateTimeNow);
                        var startDay = GetYearFirstDay(dateTimeNow, 1);
                        timeFilter = $"{searchParameter} >= {startDay} AND {searchParameter} < {endDay}";
                    }
                    break;
            }

            return timeFilter;
        }

        /// <summary>  
        /// 得到本周第一天(以星期一为第一天)  
        /// </summary>  
        /// <param name="datetime">当前时间</param>
        /// <param name="rangeWeeks">跨跃星期数</param>
        /// <returns></returns>  
        public static string GetWeekFirstDay(DateTime datetime, int rangeWeeks)
        {
            //星期一为第一天  
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);

            //因为是以星期一为第一天，所以要判断weeknow等于0时，要向前推6天。
            weeknow = (weeknow == 0 ? 7 : weeknow);
            int daydiff = rangeWeeks * 7 + weeknow - 1;

            //本周第一天  
            string FirstDay = datetime.AddDays(-daydiff).ToString("yyyy-MM-dd");
            //return Convert.ToDateTime(FirstDay);
            return FirstDay;
        }
        /// <summary>  
        /// 得到本年第一天
        /// </summary>  
        /// <param name="datetime">当前时间</param>
        /// <param name="rangYears">跨跃年数</param>
        /// <returns></returns>  
        public static string GetYearFirstDay(DateTime datetime, int rangYears = 0)
        {
            var dateTime = DateTime.Now.AddDays(1 - DateTime.Now.DayOfYear).Date;
            var startDateOfYear = dateTime.AddYears(-rangYears);
            //本周第一天  
            string firstDay = startDateOfYear.ToString("yyyy-MM-dd");
            return firstDay;
        }
        /// <summary>  
        /// 得到本月第一天
        /// </summary>  
        /// <param name="datetime">当前时间</param>
        /// <param name="rangMonths">跨跃月数</param>
        /// <returns></returns>  
        public static string GetMonthFirstDay(DateTime datetime, int rangMonths = 0)
        {
            var dateTime = DateTime.Now.AddDays(1 - DateTime.Now.Day).Date;
            var dayOfMonth = dateTime.AddMonths(-rangMonths);
            //月第一天  
            string firstDay = dayOfMonth.ToString("yyyy-MM-dd");
            return firstDay;
        }

        /// <summary>
        /// 时间是否在相应的时间段
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="duePeriodType"></param>
        /// <returns></returns>
        public static bool IsInDuePeriodRange(DateTime dateTime, DuePeriodType duePeriodType)
        {
            var dateTimeNow = DateTime.Now;
            switch (duePeriodType)
            {
                case DuePeriodType.今日:
                    {
                        //今日  
                        string startDay = dateTimeNow.ToString("yyyy-MM-dd");
                        DateTime dt = Convert.ToDateTime(startDay);
                        return dateTime >= dt;
                    }
                    break;
                case DuePeriodType.昨日:
                    {
                        //今日  
                        var endDay = dateTimeNow.ToString("yyyy-MM-dd");
                        var startDay = dateTimeNow.AddDays(-1).ToString("yyyy-MM-dd");
                        DateTime startTime = Convert.ToDateTime(startDay);
                        DateTime endTime = Convert.ToDateTime(endDay);
                        return dateTime >= startTime && dateTime < endTime;
                    }
                    break;
                case DuePeriodType.本周:
                    {
                        var startDay = GetWeekFirstDay(dateTimeNow, 0);
                        DateTime dt = Convert.ToDateTime(startDay);
                        return dateTime >= dt;
                    }
                    break;
                case DuePeriodType.上周:
                    {
                        var endDay = GetWeekFirstDay(dateTimeNow, 0);
                        var startDay = GetWeekFirstDay(dateTimeNow, 1);
                        DateTime startTime = Convert.ToDateTime(startDay);
                        DateTime endTime = Convert.ToDateTime(endDay);
                        return dateTime >= startTime && dateTime< endTime;
                    }
                    break;
                case DuePeriodType.上上周:
                    {
                        var endDay = GetWeekFirstDay(dateTimeNow, 1);
                        var startDay = GetWeekFirstDay(dateTimeNow, 2);
                        DateTime startTime = Convert.ToDateTime(startDay);
                        DateTime endTime = Convert.ToDateTime(endDay);
                        return dateTime >= startTime && dateTime < endTime;
                    }
                    break;
                case DuePeriodType.上上上周:
                    {
                        var endDay = GetWeekFirstDay(dateTimeNow, 2);
                        var startDay = GetWeekFirstDay(dateTimeNow, 3);
                        DateTime startTime = Convert.ToDateTime(startDay);
                        DateTime endTime = Convert.ToDateTime(endDay);
                        return dateTime >= startTime && dateTime < endTime;
                    }
                    break;
                case DuePeriodType.本月:
                    {
                        var startDay = GetMonthFirstDay(dateTimeNow);
                        DateTime dt = Convert.ToDateTime(startDay);
                        return dateTime >= dt;
                    }
                    break;
                case DuePeriodType.上月:
                    {
                        var endDay = GetMonthFirstDay(dateTimeNow);
                        var startDay = GetMonthFirstDay(dateTimeNow, 1);
                        DateTime startTime = Convert.ToDateTime(startDay);
                        DateTime endTime = Convert.ToDateTime(endDay);
                        return dateTime >= startTime && dateTime < endTime;
                    }
                    break;
                case DuePeriodType.今年:
                    {
                        var startDay = GetYearFirstDay(dateTimeNow);
                        DateTime startTime = Convert.ToDateTime(startDay);
                        return dateTime >= startTime;
                    }
                    break;
                case DuePeriodType.去年:
                    {
                        var endDay = GetYearFirstDay(dateTimeNow);
                        var startDay = GetYearFirstDay(dateTimeNow, 1);
                        DateTime startTime = Convert.ToDateTime(startDay);
                        DateTime endTime = Convert.ToDateTime(endDay);
                        return dateTime >= startTime && dateTime < endTime;
                    }
                    break;
            }

            return false;
        }
        /// <summary>
        /// 获取大于时间段的时间筛选
        /// </summary>
        /// <param name="periodType"></param>
        /// <param name="searchParameter"></param>
        /// <returns></returns>
        public static string GetAfterTimeFilter(DuePeriodType periodType, string searchParameter)
        {
            //created >= 2017 - 07 - 20 AND created <= now()
            var timeFilter = string.Empty;
            var dateTimeNow = DateTime.Now;
            switch (periodType)
            {
                case DuePeriodType.今日:
                    {
                        //今日  
                        string startDay = dateTimeNow.ToString("yyyy-MM-dd");
                        timeFilter = $"{searchParameter} >= {startDay}";
                    }
                    break;
                case DuePeriodType.昨日:
                    {
                        //今日  
                        var startDay = dateTimeNow.AddDays(-1).ToString("yyyy-MM-dd");
                        timeFilter = $"{searchParameter} >= {startDay}";
                    }
                    break;
                case DuePeriodType.本周:
                    {
                        var startDay = GetWeekFirstDay(dateTimeNow, 0);
                        timeFilter = $"{searchParameter} >= {startDay}";
                    }
                    break;
                case DuePeriodType.上周:
                    {
                        var startDay = GetWeekFirstDay(dateTimeNow, 1);
                        timeFilter = $"{searchParameter} >= {startDay}";
                    }
                    break;
                case DuePeriodType.上上周:
                    {
                        var startDay = GetWeekFirstDay(dateTimeNow, 2);
                        timeFilter = $"{searchParameter} >= {startDay}";
                    }
                    break;
                case DuePeriodType.上上上周:
                    {
                        var startDay = GetWeekFirstDay(dateTimeNow, 3);
                        timeFilter = $"{searchParameter} >= {startDay}";
                    }
                    break;
                case DuePeriodType.本月:
                    {
                        var startDay = GetMonthFirstDay(dateTimeNow);
                        timeFilter = $"{searchParameter} >= {startDay}";
                    }
                    break;
                case DuePeriodType.上月:
                    {
                        var startDay = GetMonthFirstDay(dateTimeNow, 1);
                        timeFilter = $"{searchParameter} >= {startDay}";
                    }
                    break;
                case DuePeriodType.今年:
                    {
                        var startDay = GetYearFirstDay(dateTimeNow);
                        timeFilter = $"{searchParameter} >= {startDay}";
                    }
                    break;
                case DuePeriodType.去年:
                    {
                        var startDay = GetYearFirstDay(dateTimeNow, 1);
                        timeFilter = $"{searchParameter} >= {startDay}";
                    }
                    break;
            }

            return timeFilter;
        }
    }
}

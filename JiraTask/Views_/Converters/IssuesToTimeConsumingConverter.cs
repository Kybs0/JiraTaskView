using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using JiraTask.Business;

namespace JiraTask
{
    class IssuesToTimeConsumingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeSpent = string.Empty;
            if (value is List<UserQuestionMode> issues)
            {
                var totalSeconds = issues.Sum(i => i.TimeSpentInSeconds??0);
                var secondToHour = TimeTransferHelper.SecondToHour(totalSeconds);
                timeSpent+= $"耗时：{secondToHour}";
            }
            return timeSpent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoutPlan.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool IsLastTuesday(this DateTime dateTime)
        {
            var lastDayOfMonth = new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));

            while (lastDayOfMonth.DayOfWeek != DayOfWeek.Thursday)
                lastDayOfMonth = lastDayOfMonth.AddDays(-1);

            return dateTime.DayOfYear == lastDayOfMonth.DayOfYear;
        }
    }
}

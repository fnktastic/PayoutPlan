using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoutPlan.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool IsLastTuesdayInMonth(this DateTime dateTime)
        {
            var lastDayOfMonth = new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));

            while (lastDayOfMonth.DayOfWeek != DayOfWeek.Tuesday)
                lastDayOfMonth = lastDayOfMonth.AddDays(-1);

            return dateTime.DayOfYear == lastDayOfMonth.DayOfYear;
        }

        public static bool IsLastDayInYear(this DateTime dateTime)
        {
            if (dateTime.Month == 12 && dateTime.Day == 31) return true;

            return false;
        }
    }
}

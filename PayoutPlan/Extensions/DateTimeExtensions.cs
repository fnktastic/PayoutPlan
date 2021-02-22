using System;

namespace Qcentris.Monitoring.ServiceExtensions
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

        public static bool IsMonthDay(this DateTime dateTime, DateTime now)
        {
            if (dateTime.Day == now.Day) return true;

            return false;
        }

        public static bool IsQuarterDay(this DateTime dateTime, DateTime now)
        {
            if (dateTime.Day == now.Day)
            {
                if (now.Month == 3 ||
                    now.Month == 6 ||
                    now.Month == 9 ||
                    now.Month == 12)
                    return true;
            }

            return false;
        }

        public static bool IsYearDay(this DateTime dateTime, DateTime now)
        {
            if (dateTime.Month == now.Month && dateTime.Day == now.Day) return true;

            return false;
        }

        public static bool IsDateAs(this DateTime dateTime, DateTime now)
        {
            if (dateTime.Year == now.Year && dateTime.Month == now.Month && dateTime.Day == now.Day) return true;

            return false;
        }
    }
}

using PayoutPlan.Interfaces.Common;
using System;

namespace PayoutPlan.Common
{
    public class DateTimeNow : IDateTimeNow
    {
        private DateTime _now;
        public DateTime Now => _now;
        public DateTimeNow()
        {
            _now = DateTime.UtcNow;
        }

        public void AddYear()
        {
            _now = _now.AddYears(1);
        }

        public void AddDay()
        {
            _now = _now.AddDays(1);
        }
    }
}

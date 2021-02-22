using PayoutPlan.Enum;
using PayoutPlan.Interfaces.Common;
using PayoutPlan.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoutPlan.Extensions
{
    public static class PayoutExtensions
    {
        public static bool IsPayoutTriggered(this IPayout payoutable, IDateTimeNow dateTimeNow)
        {
            if (payoutable.Created.IsDateAs(dateTimeNow.Now)) return false;

            switch (payoutable.PayoutFreequency)
            {
                case PayoutFreequency.Month:
                    return payoutable.Created.IsMonthDay(dateTimeNow.Now);
                case PayoutFreequency.Quarter:
                    return payoutable.Created.IsQuarterDay(dateTimeNow.Now);
                default:
                    return payoutable.Created.IsYearDay(dateTimeNow.Now);
            }
        }
    }
}

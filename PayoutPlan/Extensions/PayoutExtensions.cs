using PayoutPlan.Enum;
using PayoutPlan.Interfaces.Common;
using PayoutPlan.Model;

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

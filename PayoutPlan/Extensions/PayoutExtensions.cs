using PayoutPlan.Enum;
using PayoutPlan.Exceptions;
using PayoutPlan.Interfaces.Common;
using PayoutPlan.Models;

namespace PayoutPlan.Extensions
{
    public static class PayoutExtensions
    {
        public static bool IsPayoutTriggered(this IPayoutable payoutable, IDateTimeNow dateTimeNow)
        {
            if (payoutable.Created.IsDateAs(dateTimeNow.Now)) return false;

            switch (payoutable.PayoutFreequency)
            {
                case PayoutFreequencyEnum.Month:
                    return payoutable.Created.IsMonthDay(dateTimeNow.Now);
                case PayoutFreequencyEnum.Quarter:
                    return payoutable.Created.IsQuarterDay(dateTimeNow.Now);
                case PayoutFreequencyEnum.Year:
                    return payoutable.Created.IsYearDay(dateTimeNow.Now);
                default:
                    throw new PayoutFreequencyNotFoundException();
            }
        }
    }
}

using Qcentris.Monitoring.ServiceEnum;
using Qcentris.Monitoring.ServiceExceptions;
using Qcentris.Monitoring.ServiceInterfaces.Common;
using Qcentris.Monitoring.ServiceModels;

namespace Qcentris.Monitoring.ServiceExtensions
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

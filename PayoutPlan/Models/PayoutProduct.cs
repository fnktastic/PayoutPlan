using Qcentris.Monitoring.ServiceEnum;
using Qcentris.Monitoring.ServiceInterfaces.Common;
using System;

namespace Qcentris.Monitoring.ServiceModels
{
    public interface IPayoutable
    {
        PayoutFreequencyEnum PayoutFreequency { get; }
        double Payout { get; }
        DateTime Created { get; }
    }

    public class PayoutProduct : ProductBase, IPayoutable
    {
        public PayoutFreequencyEnum PayoutFreequency { get; set; }
        public double Payout { get; set; }
        public PayoutProduct(IDateTimeNow dateTimeNow) : base(dateTimeNow)
        {
            ProductType = ProductTypeEnum.Payout;
            FinalDerisking = true;
        }
    }
}

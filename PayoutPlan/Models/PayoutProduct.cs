using PayoutPlan.Enum;
using PayoutPlan.Interfaces.Common;
using System;

namespace PayoutPlan.Models
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

using PayoutPlan.Enum;
using PayoutPlan.Interfaces.Common;
using System;

namespace PayoutPlan.Model
{
    public interface IPayout
    {
        PayoutFreequency PayoutFreequency { get; }
        double Payout { get; }
        DateTime Created { get; }
    }

    public class PayoutProduct : ProductBase, IPayout
    {
        public PayoutFreequency PayoutFreequency { get; set; }
        public double Payout { get; set; }
        public PayoutProduct(IDateTimeNow dateTimeNow) : base(dateTimeNow)
        {
            ProductType = ProductType.Payout;
            FinalDerisking = true;
        }
    }
}

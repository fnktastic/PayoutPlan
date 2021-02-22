using PayoutPlan.Interfaces.Common;

namespace PayoutPlan.Model
{
    public class InvestmentProduct : ProductBase
    {
        public InvestmentProduct(IDateTimeNow dateTimeNow) : base(dateTimeNow)
        {

        }
    }
}

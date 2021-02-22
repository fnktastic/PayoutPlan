using PayoutPlan.Interfaces.Common;

namespace PayoutPlan.Models
{
    public class InvestmentProduct : ProductBase
    {
        public InvestmentProduct(IDateTimeNow dateTimeNow) : base(dateTimeNow)
        {

        }
    }
}

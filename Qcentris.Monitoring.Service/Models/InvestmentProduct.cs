using Qcentris.Monitoring.ServiceEnum;
using Qcentris.Monitoring.ServiceInterfaces.Common;

namespace Qcentris.Monitoring.ServiceModels
{
    public class InvestmentProduct : ProductBase
    {
        public InvestmentProduct(IDateTimeNow dateTimeNow) : base(dateTimeNow)
        {
            ProductType = ProductTypeEnum.Investment;
        }
    }
}

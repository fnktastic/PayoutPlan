using Qcentris.Monitoring.ServiceEnum;
using Qcentris.Monitoring.ServiceInterfaces;
using Qcentris.Monitoring.ServiceInterfaces.Common;
using System;

namespace Qcentris.Monitoring.ServiceModels
{
    public abstract class ProductBase : IProduct
    {
        private readonly IDateTimeNow _dateTimeNow;
        public ProductBase(IDateTimeNow dateTimeNow)
        {
            Created = DateTime.UtcNow;
            _dateTimeNow = dateTimeNow;
        }

        public ProductTypeEnum ProductType { get; set; }
        public double Investment { get; set; }
        public bool FinalDerisking { get; set; }
        public bool AnnualDerisking { get; set; }
        public double Balance { get; set; }
        public int InvestmentLength { get; set; }
        public DateTime Created { get; set; }
        public IModelPortfolio ModelPortfolio { get; set; }
        public int InvestmentYear => _dateTimeNow.Now.Year - Created.Year;
        public bool LastTwoYearsPeriod => (InvestmentLength - InvestmentYear) <= 2;
        public IDateTimeNow DateTimeNow => _dateTimeNow;
    }
}

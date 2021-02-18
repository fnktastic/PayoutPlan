using PayoutPlan.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoutPlan.Repository
{
    public interface IProductRepository
    {
        ProductBase Get(ProductType productType);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly IModelPortfolioRepository _modelPortfolioRepository;
        private readonly IDateTimeNow _dateTimeNow;

        public ProductRepository(IModelPortfolioRepository modelPortfolioRepository, IDateTimeNow dateTimeNow)
        {
            _modelPortfolioRepository = modelPortfolioRepository;
            _dateTimeNow = dateTimeNow;
        }

        public ProductBase Get(ProductType productType)
        {
            IModelPortfolio modelPortfolio = _modelPortfolioRepository.Get(ProductType.Investment, RiskCategory.Income);

            switch (productType)
            {
                case ProductType.Payout:
                    modelPortfolio = _modelPortfolioRepository.Get(ProductType.Investment, RiskCategory.Income);
                    return new PayoutProduct(modelPortfolio, annualDerisking: true, investment: 100_000.0D, _dateTimeNow)
                    {
                        PayoutFreequency = PayoutFreequency.Year,
                        Payout = 1200.50D,
                        InvestmentLength = 20,
                    };
                default:
                    return new InvestmentProduct(modelPortfolio, finalDerisking: true, annualDerisking: true, investment: 100_000.0D, _dateTimeNow)
                    {
                        InvestmentLength = 20
                    };
            }
        }
    }
}

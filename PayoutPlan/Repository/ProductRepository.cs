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
            IModelPortfolio modelPortfolio;

            switch (productType)
            {
                case ProductType.Payout:
                    modelPortfolio = _modelPortfolioRepository.Get(ProductType.Investment, RiskCategory.Income);
                    return new PayoutProduct(modelPortfolio, 
                                             annualDerisking: true,
                                             investment: 100_000.0D, 
                                             PayoutFreequency.Quarter,
                                             500.50D, 
                                             20,
                                             _dateTimeNow);
                default:
                    modelPortfolio = _modelPortfolioRepository.Get(ProductType.Investment, RiskCategory.Income);
                    return new InvestmentProduct(modelPortfolio, 
                                                 finalDerisking: true, 
                                                 annualDerisking: true, 
                                                 investment: 100_000.0D, 20,
                                                 _dateTimeNow);
            }
        }
    }
}

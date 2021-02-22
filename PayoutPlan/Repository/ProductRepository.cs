using PayoutPlan.Enum;
using PayoutPlan.Factories;
using PayoutPlan.Interfaces.Common;
using PayoutPlan.Models;

namespace PayoutPlan.Repository
{
    public interface IProductRepository
    {
        ProductBase Get(ProductTypeEnum productType);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly IModelPortfolioRepository _modelPortfolioRepository;
        private readonly IBehaviourFactory _behaviourFactory;
        private readonly IDateTimeNow _dateTimeNow;

        public ProductRepository(IModelPortfolioRepository modelPortfolioRepository, IBehaviourFactory behaviourFactory, IDateTimeNow dateTimeNow)
        {
            _modelPortfolioRepository = modelPortfolioRepository;
            _behaviourFactory = behaviourFactory;
            _dateTimeNow = dateTimeNow;
        }

        public ProductBase Get(ProductTypeEnum productType)
        {
            IModelPortfolio modelPortfolio;

            switch (productType)
            {
                case ProductTypeEnum.Payout:
                    modelPortfolio = _modelPortfolioRepository.Get(ProductTypeEnum.Investment, RiskCategoryEnum.Income);
                    return new PayoutProduct(_dateTimeNow)
                    {
                        ModelPortfolio = modelPortfolio,
                        AnnualDerisking = true,
                        Investment = 100_000.0D,
                        Balance = 100_000.0D,
                        PayoutFreequency = PayoutFreequencyEnum.Month,
                        Payout = 150.50D,
                        InvestmentLength = 20,
                    };
                default:
                    modelPortfolio = _modelPortfolioRepository.Get(ProductTypeEnum.Investment, RiskCategoryEnum.Income);
                    return new InvestmentProduct(_dateTimeNow)
                    {
                        ModelPortfolio = modelPortfolio,
                        FinalDerisking = true,
                        AnnualDerisking = true,
                        Investment = 100_000.0D,
                        Balance = 100_000.0D,
                        InvestmentLength = 20,
                    };
            }
        }
    }
}

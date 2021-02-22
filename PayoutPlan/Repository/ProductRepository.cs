using PayoutPlan.Enum;
using PayoutPlan.Factories;
using PayoutPlan.Interfaces.Common;
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
        private readonly IBehaviourFactory _behaviourFactory;
        private readonly IDateTimeNow _dateTimeNow;

        public ProductRepository(IModelPortfolioRepository modelPortfolioRepository, IBehaviourFactory behaviourFactory, IDateTimeNow dateTimeNow)
        {
            _modelPortfolioRepository = modelPortfolioRepository;
            _behaviourFactory = behaviourFactory;
            _dateTimeNow = dateTimeNow;
        }

        public ProductBase Get(ProductType productType)
        {
            IModelPortfolio modelPortfolio;

            switch (productType)
            {
                case ProductType.Payout:
                    modelPortfolio = _modelPortfolioRepository.Get(ProductType.Investment, RiskCategory.Income);
                    var payoutProduct = new PayoutProduct(_dateTimeNow)
                    {
                        ModelPortfolio = modelPortfolio,
                        AnnualDerisking = true,
                        Investment = 100_000.0D,
                        PayoutFreequency = PayoutFreequency.Month,
                        Payout = 150.50D,
                        InvestmentLength = 20,
                    };
                    payoutProduct.Withdrawal = _behaviourFactory.Instance(payoutProduct);
                    return payoutProduct;
                default:
                    modelPortfolio = _modelPortfolioRepository.Get(ProductType.Investment, RiskCategory.Income);
                    var investmentProduct = new InvestmentProduct(_dateTimeNow)
                    {
                        ModelPortfolio = modelPortfolio,
                        FinalDerisking = true,
                        AnnualDerisking = true,
                        Investment = 100_000.0D,
                        InvestmentLength = 20,
                    };
                    investmentProduct.Withdrawal = _behaviourFactory.Instance(investmentProduct);
                    return investmentProduct;
            }
        }
    }
}

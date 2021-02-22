using PayoutPlan.Enum;
using PayoutPlan.Extensions;
using PayoutPlan.Models;
using System.Collections.Generic;

namespace PayoutPlan.Repository
{
    public interface IModelPortfolioRepository
    {
        List<IModelPortfolio> Get(ProductTypeEnum productType);
        IModelPortfolio Get(ProductTypeEnum productType, RiskCategoryEnum riskCategory);
    }

    public class ModelPortfolioRepository : IModelPortfolioRepository
    {
        private List<IModelPortfolio> _payoutPortfolios = new List<IModelPortfolio>()
        {
            new ModelPortfolio(RiskCategoryEnum.Security, 15, 90, 10),
            new ModelPortfolio(RiskCategoryEnum.Income, 40, 70, 30),
            new ModelPortfolio(RiskCategoryEnum.Balance, 55, 55, 45),
            new ModelPortfolio(RiskCategoryEnum.Growth, 70, 40, 60)
        };

        private List<IModelPortfolio> _investmentPortfolios = new List<IModelPortfolio>()
        {
            new ModelPortfolio(RiskCategoryEnum.Security, 15, 90, 10),
            new ModelPortfolio(RiskCategoryEnum.Income, 40, 70, 30),
            new ModelPortfolio(RiskCategoryEnum.Balance, 55, 55, 45),
            new ModelPortfolio(RiskCategoryEnum.Growth, 70, 40, 60),
            new ModelPortfolio(RiskCategoryEnum.ActionOriented, 95, 10, 90),
        };

        public List<IModelPortfolio> Get(ProductTypeEnum productType)
        {
            switch (productType)
            {
                case ProductTypeEnum.Payout:
                    return _payoutPortfolios;
                default:
                    return _investmentPortfolios;
            }
        }

        public IModelPortfolio Get(ProductTypeEnum productType, RiskCategoryEnum riskCategory)
        {
            switch (productType)
            {
                case ProductTypeEnum.Payout:
                    return _payoutPortfolios.Find(x => x.RiskCategory == riskCategory).Clone();
                default:
                    return _investmentPortfolios.Find(x => x.RiskCategory == riskCategory).Clone();
            }
        }
    }
}

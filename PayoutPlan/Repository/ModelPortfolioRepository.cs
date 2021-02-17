using PayoutPlan.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoutPlan.Repository
{
    public interface IModelPortfolioRepository
    {
        List<IModelPortfolio> Get(ProductType productType);
        IModelPortfolio Get(ProductType productType, RiskCategory riskCategory);
    }

    public class ModelPortfolioRepository : IModelPortfolioRepository
    {
        private List<IModelPortfolio> _payoutPortfolios = new List<IModelPortfolio>()
        {
            new ModelPortfolio(RiskCategory.Security, 15, 90, 10),
            new ModelPortfolio(RiskCategory.Income, 40, 70, 30),
            new ModelPortfolio(RiskCategory.Balance, 55, 55, 45),
            new ModelPortfolio(RiskCategory.Growth, 70, 40, 60)
        };

        private List<IModelPortfolio> _investmentPortfolios = new List<IModelPortfolio>()
        {
            new ModelPortfolio(RiskCategory.Security, 15, 90, 10),
            new ModelPortfolio(RiskCategory.Income, 40, 70, 30),
            new ModelPortfolio(RiskCategory.Balance, 55, 55, 45),
            new ModelPortfolio(RiskCategory.Growth, 70, 40, 60),
            new ModelPortfolio(RiskCategory.ActionOriented, 95, 10, 90),
        };

        public List<IModelPortfolio> Get(ProductType productType)
        {
            switch(productType)
            {
                case ProductType.Payout: 
                    return _payoutPortfolios;
                default: 
                    return _investmentPortfolios;
            }
        }

        public IModelPortfolio Get(ProductType productType, RiskCategory riskCategory)
        {
            switch (productType)
            {
                case ProductType.Payout: 
                    return _payoutPortfolios.Find(x => x.RiskCategory == riskCategory);
                default: 
                    return _investmentPortfolios.Find(x => x.RiskCategory == riskCategory);
            }
        }
    }
}

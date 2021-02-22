using PayoutPlan.Enum;

namespace PayoutPlan.Models
{
    public interface IModelPortfolio
    {
        RiskCategoryEnum RiskCategory { get; set; }
        int RebalancingTreshold { get; set; }
        int Defensive { get; set; }
        int Dynamic { get; set; }
    }

    public class ModelPortfolio : IModelPortfolio
    {
        public ModelPortfolio()
        {

        }

        public ModelPortfolio(RiskCategoryEnum riskCategory, int rebalancingTreshold, int defensive, int dynamic)
        {
            RiskCategory = riskCategory;
            RebalancingTreshold = rebalancingTreshold;
            Defensive = defensive;
            Dynamic = dynamic;
        }

        public RiskCategoryEnum RiskCategory { get; set; }
        public int RebalancingTreshold { get; set; }
        public int Defensive { get; set; }
        public int Dynamic { get; set; }
    }
}

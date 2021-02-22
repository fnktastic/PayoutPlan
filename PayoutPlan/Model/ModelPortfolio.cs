using PayoutPlan.Enum;

namespace PayoutPlan.Model
{
    public interface IModelPortfolio
    {
        RiskCategory RiskCategory { get; set; }
        int RebalancingTreshold { get; set; }
        int Defensive { get; set; }
        int Dynamic { get; set; }
    }

    public class ModelPortfolio : IModelPortfolio
    {
        public ModelPortfolio()
        {

        }

        public ModelPortfolio(RiskCategory riskCategory, int rebalancingTreshold, int defensive, int dynamic)
        {
            RiskCategory = riskCategory;
            RebalancingTreshold = rebalancingTreshold;
            Defensive = defensive;
            Dynamic = dynamic;
        }

        public RiskCategory RiskCategory { get; set; }
        public int RebalancingTreshold { get; set; }
        public int Defensive { get; set; }
        public int Dynamic { get; set; }
    }
}

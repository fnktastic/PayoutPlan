using PayoutPlan.Behaviours;
using PayoutPlan.Enum;
using PayoutPlan.Interfaces;
using PayoutPlan.Model;

namespace PayoutPlan.Factories
{
    public interface IBehaviourFactory
    {
        IBehaviour Instance(ProductBase productBase);
    }

    public class BehaviourFactory : IBehaviourFactory
    {
        public IBehaviour Instance(ProductBase productBase)
        {
            switch (productBase.ProductType)
            {
                case ProductType.Payout:
                    return new PayoutWithdrawalBehaviour(productBase);
                default:
                    return new InvestmentWithdrawalBehaviour(productBase);
            }
        }
    }
}

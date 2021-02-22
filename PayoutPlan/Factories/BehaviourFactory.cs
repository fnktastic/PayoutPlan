using PayoutPlan.Behaviours;
using PayoutPlan.Enum;
using PayoutPlan.Exceptions;
using PayoutPlan.Interfaces;
using PayoutPlan.Models;

namespace PayoutPlan.Factories
{
    public interface IBehaviourFactory
    {
        IBehaviour Instance(ProductBase productBase, BehaviourEnum behaviour);
    }

    public class BehaviourFactory : IBehaviourFactory
    {
        public IBehaviour Instance(ProductBase productBase, BehaviourEnum behaviour)
        {
            switch (behaviour)
            {
                case BehaviourEnum.Payout:
                    return new PayoutWithdrawalBehaviour(productBase);
                case BehaviourEnum.FinalRebalancing:
                    return new FinalRebalancingBehaviour(productBase);
                case BehaviourEnum.AnnualRebalancing:
                    return new AnnualRebalanceBehaviour(productBase);
                case BehaviourEnum.FlexibleAllocationsRebalancing:
                    return new FlexibleAllocationRebalancingBehaviour(productBase);
                default:
                    throw new BehaviourNotFoundException();
            }
        }
    }
}

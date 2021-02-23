using Qcentris.Monitoring.ServiceBehaviours;
using Qcentris.Monitoring.ServiceEnum;
using Qcentris.Monitoring.ServiceExceptions;
using Qcentris.Monitoring.ServiceInterfaces;
using Qcentris.Monitoring.ServiceModels;

namespace Qcentris.Monitoring.ServiceFactories
{
    public interface IBehaviourFactory
    {
        IBehaviour Create(ProductBase productBase, BehaviourEnum behaviour);
    }

    public class BehaviourFactory : IBehaviourFactory
    {
        public IBehaviour Create(ProductBase productBase, BehaviourEnum behaviour)
        {
            switch (behaviour)
            {
                case BehaviourEnum.Payout:
                    return new PayoutWithdrawalBehaviour(productBase);
                case BehaviourEnum.FinalRebalancing:
                    return new FinalRebalancingBehaviour(productBase);
                case BehaviourEnum.AnnualRebalancing:
                    return new AnnualRebalancingBehaviour(productBase);
                case BehaviourEnum.FlexibleAllocationsRebalancing:
                    return new FlexibleAllocationRebalancingBehaviour(productBase);
                default:
                    throw new BehaviourNotFoundException();
            }
        }
    }
}

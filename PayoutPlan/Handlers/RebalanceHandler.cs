using PayoutPlan.Factories;
using PayoutPlan.Interfaces;
using PayoutPlan.Models;

namespace PayoutPlan.Handlers
{
    public interface IRebalanceHandler
    {
        void Execute(IRabalanceMonitor monitor, ProductBase product);
    }

    public class RebalanceHandler : IRebalanceHandler
    {
        private readonly IBehaviourFactory _behaviourFactory;

        public RebalanceHandler(IBehaviourFactory behaviourFactory)
        {
            _behaviourFactory = behaviourFactory;
        }

        public void Execute(IRabalanceMonitor monitor, ProductBase product)
        {
            if (monitor.IsAnnualRebalancingTriggered)
            {
                _behaviourFactory.Instance(product, Enum.BehaviourEnum.AnnualRebalancing).Execute();
            }

            if (monitor.IsFinalRebalancingTriggered)
            {
                _behaviourFactory.Instance(product, Enum.BehaviourEnum.FinalRebalancing).Execute();
            }

            if (monitor.IsFlexibleAllocationRebalancingTriggered)
            {
                _behaviourFactory.Instance(product, Enum.BehaviourEnum.FlexibleAllocationsRebalancing).Execute();
            }
        }
    }
}

using Qcentris.Monitoring.ServiceEnum;
using Qcentris.Monitoring.ServiceFactories;
using Qcentris.Monitoring.ServiceInterfaces;
using Qcentris.Monitoring.ServiceModels;

namespace Qcentris.Monitoring.ServiceHandlers
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
                _behaviourFactory.Create(product, BehaviourEnum.AnnualRebalancing).Execute();
            }

            if (monitor.IsFinalRebalancingTriggered)
            {
                _behaviourFactory.Create(product, BehaviourEnum.FinalRebalancing).Execute();
            }

            if (monitor.IsFlexibleAllocationRebalancingTriggered)
            {
                _behaviourFactory.Create(product, BehaviourEnum.FlexibleAllocationsRebalancing).Execute();
            }
        }
    }
}

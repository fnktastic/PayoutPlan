using Qcentris.Monitoring.ServiceEnum;
using Qcentris.Monitoring.ServiceFactories;
using Qcentris.Monitoring.ServiceInterfaces;
using Qcentris.Monitoring.ServiceModels;

namespace Qcentris.Monitoring.ServiceHandlers
{
    public interface IPayoutHandler
    {
        void Execute(IPayoutMonitor monitor, ProductBase product);
    }

    public class PayoutHandler : IPayoutHandler
    {
        private readonly IBehaviourFactory _behaviourFactory;

        public PayoutHandler(IBehaviourFactory behaviourFactory)
        {
            _behaviourFactory = behaviourFactory;
        }

        public void Execute(IPayoutMonitor monitor, ProductBase product)
        {
            if (monitor.IsPayoutTriggered)
            {
                _behaviourFactory.Create(product, BehaviourEnum.Payout).Execute();
            }
        }
    }
}

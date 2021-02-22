using PayoutPlan.Enum;
using PayoutPlan.Factories;
using PayoutPlan.Interfaces;
using PayoutPlan.Models;

namespace PayoutPlan.Handlers
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
                _behaviourFactory.Instance(product, BehaviourEnum.Payout).Execute();
            }
        }
    }
}

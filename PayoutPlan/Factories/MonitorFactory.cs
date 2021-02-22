using PayoutPlan.Enum;
using PayoutPlan.Handlers;
using PayoutPlan.Interfaces.Common;
using PayoutPlan.Model;
using PayoutPlan.Monitor;

namespace PayoutPlan.Factories
{
    public interface IMonitorFactory
    {
        IMonitor Instance(ProductBase productBase);
    }

    public class MonitorFactory : IMonitorFactory
    {
        private readonly IRebalanceHandler _rebalancerHandler;
        private readonly IPayoutHandler _payoutHandler;
        private readonly IDateTimeNow _dateTimeNow;

        public MonitorFactory(IDateTimeNow dateTimeNow, IRebalanceHandler rebalancerHandler, IPayoutHandler payoutHandler)
        {
            _dateTimeNow = dateTimeNow;
            _rebalancerHandler = rebalancerHandler;
            _payoutHandler = payoutHandler;
        }

        public IMonitor Instance(ProductBase productBase)
        {
            switch (productBase.ProductType)
            {
                case ProductType.Payout:
                    return new PayoutMonitor(_rebalancerHandler, _payoutHandler, productBase, _dateTimeNow);
                default:
                    return new InvestmentMonitor(_rebalancerHandler, productBase, _dateTimeNow);
            }
        }
    }
}

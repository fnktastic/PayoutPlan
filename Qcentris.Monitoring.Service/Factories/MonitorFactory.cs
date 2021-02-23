using Qcentris.Monitoring.ServiceEnum;
using Qcentris.Monitoring.ServiceExceptions;
using Qcentris.Monitoring.ServiceHandlers;
using Qcentris.Monitoring.ServiceInterfaces.Common;
using Qcentris.Monitoring.ServiceModels;
using Qcentris.Monitoring.ServiceMonitor;

namespace Qcentris.Monitoring.ServiceFactories
{
    public interface IMonitorFactory
    {
        IMonitor Create(ProductBase productBase);
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

        public IMonitor Create(ProductBase productBase)
        {
            switch (productBase.ProductType)
            {
                case ProductTypeEnum.Payout:
                    return new PayoutMonitor(_rebalancerHandler, _payoutHandler, productBase, _dateTimeNow);
                case ProductTypeEnum.Investment:
                    return new InvestmentMonitor(_rebalancerHandler, productBase, _dateTimeNow);
                default:
                    throw new MonitorNotFoundException();
            }
        }
    }
}

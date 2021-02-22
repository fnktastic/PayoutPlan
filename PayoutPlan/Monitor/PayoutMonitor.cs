using PayoutPlan.Extensions;
using PayoutPlan.Handlers;
using PayoutPlan.Interfaces;
using PayoutPlan.Interfaces.Common;
using PayoutPlan.Model;

namespace PayoutPlan.Monitor
{


    public class PayoutMonitor : MonitorBase, IRabalanceMonitor, IPayoutMonitor
    {
        private const int REBALANCING_TRESHOLD = 90;

        private readonly IRebalanceHandler _rebalancerHandler;
        private readonly IPayoutHandler _payoutHandler;

        public PayoutMonitor(IRebalanceHandler rebalancerHandler, IPayoutHandler payoutHandler, ProductBase productBase, IDateTimeNow now) : base(productBase, now)
        {
            _rebalancerHandler = rebalancerHandler;
            _payoutHandler = payoutHandler;
        }
        public PayoutProduct Product => (PayoutProduct)_productBase;
        public bool IsFlexibleAllocationRebalancingTriggered => _dateTime.Now.IsLastTuesdayInMonth() && Product.ModelPortfolio.Dynamic >= Product.ModelPortfolio.RebalancingTreshold;
        public bool IsFinalRebalancingTriggered => Product.LastTwoYearsPeriod && _dateTime.Now.IsLastTuesdayInMonth() && Product.ModelPortfolio.Defensive < REBALANCING_TRESHOLD;
        public bool IsAnnualRebalancingTriggered => Product.AnnualDerisking && _dateTime.Now.IsLastDayInYear() && Product.ModelPortfolio.Defensive < REBALANCING_TRESHOLD;
        public bool IsPayoutTriggered => Product.IsPayoutTriggered(_dateTime);

        public override void Invoke()
        {
            _rebalancerHandler.Execute(this, Product);
            _payoutHandler.Execute(this, Product);
        }
    }
}

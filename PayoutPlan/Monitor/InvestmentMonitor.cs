using PayoutPlan.Extensions;
using PayoutPlan.Handlers;
using PayoutPlan.Interfaces;
using PayoutPlan.Interfaces.Common;
using PayoutPlan.Models;

namespace PayoutPlan.Monitor
{
    public class InvestmentMonitor : MonitorBase, IRabalanceMonitor
    {
        private const int REBALANCING_TRESHOLD = 90;

        private readonly IRebalanceHandler _rebalancerHandler;
        public InvestmentMonitor(IRebalanceHandler rebalancerHandler, ProductBase productBase, IDateTimeNow now) : base(productBase, now)
        {
            _rebalancerHandler = rebalancerHandler;
        }

        public InvestmentProduct Product => (InvestmentProduct)_productBase;
        public bool IsFlexibleAllocationRebalancingTriggered => false;
        public bool IsFinalRebalancingTriggered => Product.LastTwoYearsPeriod && Product.FinalDerisking && _dateTime.Now.IsLastTuesdayInMonth() && Product.ModelPortfolio.Defensive < REBALANCING_TRESHOLD;
        public bool IsAnnualRebalancingTriggered => Product.AnnualDerisking && _dateTime.Now.IsLastDayInYear() && Product.ModelPortfolio.Defensive < REBALANCING_TRESHOLD;

        public override void Invoke()
        {
            _rebalancerHandler.Execute(this, Product);
        }
    }
}

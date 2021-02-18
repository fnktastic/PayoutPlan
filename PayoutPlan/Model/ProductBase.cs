using PayoutPlan.Extensions;
using PayoutPlan.Repository;
using PayoutPlan.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoutPlan.Model
{
    public interface IPayoutable
    {
        PayoutFreequency PayoutFreequency { get; }
        double Payout { get; }
        DateTime Created { get; }
    }

    public interface IDateTimeNow
    {
        void AddYear();
        void AddDay();
        DateTime Now { get; }
    }


    public abstract class MonitorBase
    {
        protected readonly ProductBase _productBase;
        
        protected readonly IDateTimeNow _dateTime;
        protected IModelPortfolio _modelPortfolio => _productBase.ModelPortfolio;

        public MonitorBase(ProductBase productBase, IDateTimeNow dateTime)
        {
            _productBase = productBase;
            _dateTime = dateTime;
        }
        public ProductBase ProductBase => _productBase;
        public abstract void Invoke();
    }

    public interface IRabalanceMonitor
    {
        bool IsFlexibleAllocationRebalancing { get; }
        bool IsFinalRebalancing { get; }
        bool IsAnnualRebalancing { get; }
    }

    public interface IPayoutMonitor
    {
        bool IsPayout { get; }
    }

    public class InvestmentMonitor : MonitorBase, IRabalanceMonitor
    {
        protected readonly IRebalanceHandler _rebalancerHandler;
        public InvestmentMonitor(IRebalanceHandler rebalancerHandler, ProductBase productBase, IDateTimeNow now) : base(productBase, now)
        {
            _rebalancerHandler = rebalancerHandler;
        }

        public bool IsFlexibleAllocationRebalancing => false;

        public bool IsFinalRebalancing => _productBase.LastTwoYearsPeriod && _productBase.FinalDerisking && _dateTime.Now.IsLastTuesdayInMonth();

        public bool IsAnnualRebalancing => _productBase.AnnualDerisking && _productBase.ModelPortfolio.Defensive < 90 && _dateTime.Now.IsLastDayInYear();

        public override void Invoke()
        {
            _rebalancerHandler.Execute(this, this.ProductBase);
        }
    }

    public class PayoutMonitor : MonitorBase, IRabalanceMonitor, IPayoutMonitor
    {
        private readonly IPayoutHelper _payoutHelper;
        protected readonly IRebalanceHandler _rebalancerHandler;
        private readonly IPayoutHandler _payoutHandler;

        public PayoutMonitor(IPayoutHelper payoutHelper, IRebalanceHandler rebalancerHandler, IPayoutHandler payoutHandler, ProductBase productBase, IDateTimeNow now) : base(productBase, now)
        {
            _payoutHelper = payoutHelper;
            _rebalancerHandler = rebalancerHandler;
            _payoutHandler = payoutHandler;
        }

        public bool IsFlexibleAllocationRebalancing => _dateTime.Now.IsLastTuesdayInMonth() && _modelPortfolio.Dynamic >= _modelPortfolio.RebalancingTreshold;
        public bool IsFinalRebalancing => _productBase.LastTwoYearsPeriod && _dateTime.Now.IsLastTuesdayInMonth();
        public bool IsAnnualRebalancing => _productBase.AnnualDerisking && _productBase.ModelPortfolio.Defensive < 90 && _dateTime.Now.IsLastDayInYear();
        public bool IsPayout => _payoutHelper.IsTodayPayoutDate((PayoutProduct)_productBase, _dateTime);

        public override void Invoke()
        {
            _rebalancerHandler.Execute(this, this.ProductBase);
            _payoutHandler.Execute(this, this.ProductBase);
        }
    }

    public interface IModelPortfolio
    {
        RiskCategory RiskCategory { get; set; }
        int RebalancingTreshold { get; set; }
        int Defensive { get; set; }
        int Dynamic { get; set; }
    }

    public class ModelPortfolio : IModelPortfolio
    {
        public ModelPortfolio()
        {

        }

        public ModelPortfolio(RiskCategory riskCategory, int rebalancingTreshold, int defensive, int dynamic)
        {
            RiskCategory = riskCategory;
            RebalancingTreshold = rebalancingTreshold;
            Defensive = defensive;
            Dynamic = dynamic;
        }

        public RiskCategory RiskCategory { get; set; }
        public int RebalancingTreshold { get; set; }
        public int Defensive { get; set; }
        public int Dynamic { get; set; }
    }

    public class InvestmentProduct : ProductBase
    {
        public InvestmentProduct(IModelPortfolio modelPortfolio, bool finalDerisking, bool annualDerisking, double investment, IDateTimeNow dateTimeNow) : base(dateTimeNow)
        {
            ModelPortfolio = modelPortfolio;
            Investment = investment;
            Balance = investment;
            FinalDerisking = finalDerisking;
            AnnualDerisking = annualDerisking;
            ProductType = ProductType.Investment;
        }

        public override void Withdraw(double? amount)
        {
            if (amount.HasValue == false) return;

            this.Balance -= amount.Value;
        }
    }

    public class PayoutProduct : ProductBase, IPayoutable
    {
        public PayoutFreequency PayoutFreequency { get; set; }
        public double Payout { get; set; }
        public PayoutProduct(IModelPortfolio modelPortfolio, bool annualDerisking, double investment, IDateTimeNow dateTimeNow) : base(dateTimeNow)
        {
            ModelPortfolio = modelPortfolio;
            Investment = investment;
            Balance = investment;
            AnnualDerisking = annualDerisking;
            ProductType = ProductType.Payout;
            FinalDerisking = true;
        }

        public override void Withdraw(double? amount)
        {
            this.Balance -= this.Payout;
        }
    }

    public abstract class ProductBase
    {
        private readonly IDateTimeNow _dateTimeNow;
        public ProductBase(IDateTimeNow dateTimeNow)
        {
            Created = DateTime.UtcNow;
            _dateTimeNow = dateTimeNow;
        }

        public ProductType ProductType { get; protected set; }
        protected double Investment { get; set; }
        public bool FinalDerisking { get; protected set; }
        public bool AnnualDerisking { get; protected set; }
        public double Balance { get; set; }
        public int InvestmentLength { get; set; }
        public DateTime Created { get; protected set; }
        public IModelPortfolio ModelPortfolio { get; protected set; }
        public int InvestmentYear => _dateTimeNow.Now.Year - Created.Year;
        public bool LastTwoYearsPeriod => (InvestmentLength - InvestmentYear) <= 2 ? true : false;
        public abstract void Withdraw(double? amount = null);
    }

    public enum PayoutFreequency
    {
        Year = 0,
        Quarter = 1,
        Month = 2
    }

    public enum ProductType
    {
        Investment = 9,
        Payout = 8
    }

    public enum RiskCategory
    {
        Security,
        Income,
        Balance,
        Growth,
        ActionOriented
    }

    public interface IPayoutHandler
    {
        void Execute(IPayoutMonitor monitor, ProductBase product);
    }

    public class PayoutHandler : IPayoutHandler
    {
        public void Execute(IPayoutMonitor monitor, ProductBase product)
        {
            //withdrawal and payout logic
            if(monitor.IsPayout)
            {
                //example
                product.Withdraw();
            }
        }
    }

    public interface IRebalanceHandler
    {
        void Execute(IRabalanceMonitor monitor, ProductBase product);
    }

    public class RebalanceHandler : IRebalanceHandler
    {
        public void Execute(IRabalanceMonitor monitor, ProductBase product)
        {
            //rebalance logic
            if (monitor.IsAnnualRebalancing)
            {
                //example
                product.ModelPortfolio.Defensive++;
                product.ModelPortfolio.Dynamic--;
            }

            if (monitor.IsFinalRebalancing)
            {
                //example
                product.ModelPortfolio.Defensive++;
                product.ModelPortfolio.Dynamic--;
            }

            if (monitor.IsFlexibleAllocationRebalancing)
            {
                //example
                product.ModelPortfolio.Defensive++;
                product.ModelPortfolio.Dynamic--;
            }
        }
    }

    public interface IMonitorHandler
    {
        void Monitor(ProductBase productBase);
    }

    public class MonitorHandler : IMonitorHandler
    {
        private readonly IMonitorFactory _monitorFactory;
        public MonitorHandler(IMonitorFactory monitorFactory)
        {
            _monitorFactory = monitorFactory;
        }

        public void Monitor(ProductBase productBase)
        {
            var monitor = _monitorFactory.Instance(productBase);

            monitor.Invoke();
        }
    }

    public class DateTimeNow : IDateTimeNow
    {
        private DateTime _now;
        public DateTime Now => _now;
        public DateTimeNow()
        {
            _now = DateTime.UtcNow;
        }

        public void AddYear()
        {
            _now = _now.AddYears(1);
        }

        public void AddDay()
        {
            _now = _now.AddDays(1);
        }
    }

    public interface IMonitorFactory
    {
        MonitorBase Instance(ProductBase productBase);
    }

    public class MonitorFactory : IMonitorFactory
    {
        private readonly IRebalanceHandler _rebalancerHandler;
        private readonly IPayoutHandler _payoutHandler;
        private readonly IPayoutHelper _payoutHelper;

        private readonly IDateTimeNow _dateTimeNow;

        public MonitorFactory(IDateTimeNow dateTimeNow, IRebalanceHandler rebalancerHandler, IPayoutHandler payoutHandler, IPayoutHelper payoutHelper)
        {
            _dateTimeNow = dateTimeNow;
            _rebalancerHandler = rebalancerHandler;
            _payoutHandler = payoutHandler;
            _payoutHelper = payoutHelper;
        }

        public MonitorBase Instance(ProductBase productBase)
        {
            switch (productBase.ProductType)
            {
                case ProductType.Payout:
                    return new PayoutMonitor(_payoutHelper, _rebalancerHandler, _payoutHandler, productBase, _dateTimeNow);
                default:
                    return new InvestmentMonitor(_rebalancerHandler, productBase, _dateTimeNow);
            }
        }
    }
}

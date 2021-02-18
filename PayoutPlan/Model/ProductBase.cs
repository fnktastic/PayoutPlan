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
        private const int ANNUAL_REBALANCING_DEFENSIVE_TRESHOLD = 90;

        protected readonly ProductBase _productBase;
        protected readonly IRebalancerHandler _rebalancerHandler;
        protected readonly IWithdrawalHandler _withdrawalHandler;
        protected readonly IDateTimeNow _dateTime;
        protected IModelPortfolio _modelPortfolio => _productBase.ModelPortfolio;
        protected void Rebalance()
        {
            _rebalancerHandler.Rebalance(this);
        }

        protected void Withdraw()
        {
            _withdrawalHandler.Withdraw(this);
        }

        public MonitorBase(ProductBase productBase, IRebalancerHandler rebalancerHandler, IWithdrawalHandler withdrawalHandler, IDateTimeNow dateTime)
        {
            _productBase = productBase;
            _rebalancerHandler = rebalancerHandler;
            _withdrawalHandler = withdrawalHandler;
            _dateTime = dateTime;
        }

        public abstract bool IsFlexibleAllocationRebalancing { get; }
        public abstract bool IsFinalRebalancing { get; }
        public abstract bool IsPayout { get; }
        public ProductBase ProductBase => _productBase;
        public bool IsAnnualRebalancing => _productBase.AnnualDerisking && _productBase.ModelPortfolio.Defensive < ANNUAL_REBALANCING_DEFENSIVE_TRESHOLD && _dateTime.Now.IsLastDayInYear();
        public void Invoke()
        {
            Rebalance();
            Withdraw();
        }
    }

    public class InvestmentMonitor : MonitorBase
    {
        public InvestmentMonitor(ProductBase productBase, IRebalancerHandler rebalancerHandler, IWithdrawalHandler withdrawalHandler, IDateTimeNow now) : base(productBase, rebalancerHandler, withdrawalHandler, now)
        {

        }

        public override bool IsFlexibleAllocationRebalancing => false;

        public override bool IsFinalRebalancing => _productBase.LastTwoYearsPeriod && _productBase.FinalDerisking && _dateTime.Now.IsLastTuesdayInMonth();

        public override bool IsPayout => false;
    }

    public class PayoutMonitor : MonitorBase
    {
        private readonly IPayoutHelper _payoutHelper;

        public PayoutMonitor(ProductBase productBase, IRebalancerHandler rebalancerHandler, IWithdrawalHandler withdrawalHandler, IDateTimeNow now, IPayoutHelper payoutHelper) : base(productBase, rebalancerHandler, withdrawalHandler, now)
        {
            _payoutHelper = payoutHelper;
        }

        public override bool IsFlexibleAllocationRebalancing => _dateTime.Now.IsLastTuesdayInMonth() && _modelPortfolio.Dynamic >= _modelPortfolio.RebalancingTreshold;
        
        public override bool IsFinalRebalancing => _productBase.LastTwoYearsPeriod && _dateTime.Now.IsLastTuesdayInMonth();
        
        public override bool IsPayout => _payoutHelper.IsTodayPayoutDate((PayoutProduct)_productBase, _dateTime);
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

    public interface IWithdrawalable
    {
        void Withdraw(double? amount);
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
        protected double Balance { get; set; }
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

    public interface IWithdrawalHandler
    {
        void Withdraw(MonitorBase monitorBase);
    }

    public class WithdrawalHandler : IWithdrawalHandler
    {
        public void Withdraw(MonitorBase monitorBase)
        {
            //withdrawal and payout logic
            if(monitorBase.IsPayout)
            {
                monitorBase.ProductBase.Withdraw();
            }
        }
    }

    public interface IRebalancerHandler
    {
        void Rebalance(MonitorBase productBase);
    }

    public class RebalancerHandler : IRebalancerHandler
    {
        public void Rebalance(MonitorBase rebalancerBase)
        {
            //rebalance logic
            if (rebalancerBase.IsAnnualRebalancing)
            {
                rebalancerBase.ProductBase.ModelPortfolio.Defensive++;
                rebalancerBase.ProductBase.ModelPortfolio.Dynamic--;
            }

            if (rebalancerBase.IsFinalRebalancing)
            {
                rebalancerBase.ProductBase.ModelPortfolio.Defensive++;
                rebalancerBase.ProductBase.ModelPortfolio.Dynamic--;
            }

            if (rebalancerBase.IsFlexibleAllocationRebalancing)
            {
                rebalancerBase.ProductBase.ModelPortfolio.Defensive++;
                rebalancerBase.ProductBase.ModelPortfolio.Dynamic--;
            }
        }
    }

    public interface IMonitorHandler
    {
        void Monitor(ProductBase productBase);
    }

    public class MonitorHandler : IMonitorHandler
    {
        private readonly IMonitorFactory _rebalancerFactory;
        public MonitorHandler(IMonitorFactory rebalancerFactory)
        {
            _rebalancerFactory = rebalancerFactory;
        }

        public void Monitor(ProductBase productBase)
        {
            var monitor = _rebalancerFactory.Instance(productBase);

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
        private readonly IRebalancerHandler _rebalancerHandler;
        private readonly IWithdrawalHandler _withdrawalHandler;
        private readonly IPayoutHelper _payoutHelper;

        private readonly IDateTimeNow _dateTimeNow;

        public MonitorFactory(IDateTimeNow dateTimeNow, IRebalancerHandler rebalancerHandler, IWithdrawalHandler withdrawalHandler, IPayoutHelper payoutHelper)
        {
            _dateTimeNow = dateTimeNow;
            _rebalancerHandler = rebalancerHandler;
            _withdrawalHandler = withdrawalHandler;
            _payoutHelper = payoutHelper;
        }

        public MonitorBase Instance(ProductBase productBase)
        {
            switch (productBase.ProductType)
            {
                case ProductType.Payout:
                    return new PayoutMonitor(productBase, _rebalancerHandler, _withdrawalHandler, _dateTimeNow, _payoutHelper);
                default:
                    return new InvestmentMonitor(productBase, _rebalancerHandler, _withdrawalHandler, _dateTimeNow);
            }
        }
    }

    public static class Imitator
    {
        static IDateTimeNow dateTimeNow = new DateTimeNow();

        static IModelPortfolioRepository modelPortfolioRepository = new ModelPortfolioRepository();
        static IRebalancerHandler rebalancerHandler = new RebalancerHandler();
        static IWithdrawalHandler withdrawalHandler = new WithdrawalHandler();
        static IPayoutHelper _payoutHelper = new PayoutHelper();
        static IMonitorFactory monitorFactory = new MonitorFactory(dateTimeNow, rebalancerHandler, withdrawalHandler, _payoutHelper);
        static IMonitorHandler monitorHandler = new MonitorHandler(monitorFactory);

        public static void DailyRun()
        {
            var modelPortfolio1 = modelPortfolioRepository.Get(ProductType.Investment, RiskCategory.Growth);
            var modelPortfolio2 = modelPortfolioRepository.Get(ProductType.Investment, RiskCategory.Income);

            var payoutProduct = new PayoutProduct(modelPortfolio1, annualDerisking: true, investment: 100_000.0D, dateTimeNow)
            {
                PayoutFreequency = PayoutFreequency.Quarter,
                Payout = 500.0D,
                InvestmentLength = 20,
            };

            var investmentProduct = new InvestmentProduct(modelPortfolio2, finalDerisking: true, annualDerisking: true, investment: 100_000.0D, dateTimeNow)
            {
                InvestmentLength = 20
            };

            var endOfProductLife = dateTimeNow.Now.AddYears(20);

            while (dateTimeNow.Now < endOfProductLife)
            {
                monitorHandler.Monitor(payoutProduct);
                monitorHandler.Monitor(investmentProduct);

                dateTimeNow.AddDay();
            }

            Console.WriteLine("Finished");
        }
    }
}

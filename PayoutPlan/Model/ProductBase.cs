using PayoutPlan.Extensions;
using PayoutPlan.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoutPlan.Model
{

    public interface IRebalanceable
    {
        void Rebalance();
    }

    public interface IDateTimeNow
    {
        DateTime Now { get; }
    }


    public abstract class RebalancerBase : IRebalanceable
    {
        private const int ANNUAL_TRESHOLD = 90;

        protected readonly ProductBase _productBase;
        protected readonly IMonitorHandler _monitorHandler;
        protected readonly IDateTimeNow _dateTime;
        protected IModelPortfolio _modelPortfolio => _productBase.ModelPortfolio;

        public RebalancerBase(ProductBase productBase, IMonitorHandler rebalancerHandler, IDateTimeNow dateTime)
        {
            _productBase = productBase;
            _monitorHandler = rebalancerHandler;
            _dateTime = dateTime;
        }

        public abstract bool IsFlexibleAllocationRebalancing { get; }
        public abstract bool IsFinalRebalancing { get; }
        public bool IsAnnualRebalancing => _productBase.AnnualDerisking && _productBase.ModelPortfolio.Defensive <= ANNUAL_TRESHOLD && _dateTime.Now.IsLastDayInYear();
        public void Rebalance()
        {
            _monitorHandler.Monitor(this);
        }
    }

    public class InvestmentRebalancer : RebalancerBase
    {
        public InvestmentRebalancer(ProductBase productBase, IMonitorHandler rebalancerHandler, IDateTimeNow now) : base(productBase, rebalancerHandler, now)
        {

        }

        public override bool IsFlexibleAllocationRebalancing => false;

        public override bool IsFinalRebalancing => _productBase.LastTwoYearsPeriod && _productBase.FinalDerisking;
    }

    public class PayoutRebalancer : RebalancerBase
    {
        public PayoutRebalancer(ProductBase productBase, IMonitorHandler rebalancerHandler, IDateTimeNow now) : base(productBase, rebalancerHandler, now)
        {

        }

        public override bool IsFlexibleAllocationRebalancing => _dateTime.Now.IsLastTuesdayInMonth() && _modelPortfolio.Dynamic >= _modelPortfolio.RebalancingTreshold;

        public override bool IsFinalRebalancing => _productBase.LastTwoYearsPeriod;
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

    public interface IWithdrawal
    {
        void Withdraw(double? amount);
    }

    public class InvestmentProduct : ProductBase, IWithdrawal
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

        public void Withdraw(double? amount)
        {
            if (amount.HasValue == false) return;

            this.Balance -= amount.Value;
        }
    }

    public class PayoutProduct : ProductBase, IWithdrawal
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

        public void Withdraw(double? amount)
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

    public interface IMonitorHandler
    {
        void Monitor(RebalancerBase productBase);
    }

    public class MonitorHandler : IMonitorHandler
    {
        public void Monitor(RebalancerBase productBase)
        {
            if (productBase.IsAnnualRebalancing)
            {

            }

            if (productBase.IsFinalRebalancing)
            {

            }

            if (productBase.IsFlexibleAllocationRebalancing)
            {

            }
        }
    }

    public class DateTimeNow : IDateTimeNow
    {
        private DateTime _now;
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

        public DateTime Now => _now;
    }

    public static class Imitator
    {
        public static void DailyRun()
        {
            var dateTimeNow = new DateTimeNow();

            var modelPortfolioRepository = new ModelPortfolioRepository();

            var modelPortfolio = modelPortfolioRepository.Get(ProductType.Investment, RiskCategory.Growth);

            var rebalancerHandler = new MonitorHandler();

            var payoutProduct = new PayoutProduct(modelPortfolio, annualDerisking: true, investment: 100_000.0D, dateTimeNow)
            {
                PayoutFreequency = PayoutFreequency.Quarter,
                Payout = 10_000.0D,
                InvestmentLength = 10,
            };

            var investmentProduct = new InvestmentProduct(modelPortfolio, finalDerisking: true, annualDerisking: true, investment: 100_000.0D, dateTimeNow)
            {
                InvestmentLength = 20
            };

            var endOfProductLife = dateTimeNow.Now.AddYears(20);

            while (dateTimeNow.Now < endOfProductLife)
            {
                var payoutRebalancer = new PayoutRebalancer(payoutProduct, rebalancerHandler, dateTimeNow);

                var investmentRebalancer = new InvestmentRebalancer(investmentProduct, rebalancerHandler, dateTimeNow);

                investmentRebalancer.Rebalance();

                payoutRebalancer.Rebalance();

                dateTimeNow.AddDay();
            }
        }
    }
}

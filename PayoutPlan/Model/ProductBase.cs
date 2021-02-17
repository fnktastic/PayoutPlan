using PayoutPlan.Extensions;
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
        protected readonly ProductBase _productBase;
        protected readonly IRebalancerHandler _rebalancerHandler;
        protected readonly IDateTimeNow _dateTime;
        protected IModelPortfolio _modelPortfolio => _productBase.ModelPortfolio;

        public RebalancerBase(ProductBase productBase, IRebalancerHandler rebalancerHandler, IDateTimeNow dateTime)
        {
            _productBase = productBase;
            _rebalancerHandler = rebalancerHandler;
            _dateTime = dateTime;
        }

        public abstract bool IsFlexibleAllocationRebalancing { get; }
        public abstract bool IsFinalRebalancing { get; }
        public bool IsAnnualRebalancing => _productBase.AnnualDerisking && _productBase.ModelPortfolio.Defensive <= 90;
        public void Rebalance()
        {
            _rebalancerHandler.Rebalance(this);
        }
    }

    public class InvestmentRebalancer : RebalancerBase
    {
        public InvestmentRebalancer(ProductBase productBase, IRebalancerHandler rebalancerHandler, IDateTimeNow now) : base(productBase, rebalancerHandler, now)
        {

        }

        public override bool IsFlexibleAllocationRebalancing => false;

        public override bool IsFinalRebalancing => _productBase.LastTwoYearsPeriod && _productBase.FinalDerisking;
    }

    public class PayoutRebalancer : RebalancerBase
    {
        public PayoutRebalancer(ProductBase productBase, IRebalancerHandler rebalancerHandler, IDateTimeNow now) : base(productBase, rebalancerHandler, now)
        {

        }

        public override bool IsFlexibleAllocationRebalancing => _dateTime.Now.IsLastTuesday() && _modelPortfolio.Dynamic >= _modelPortfolio.RebalancingTreshold;

        public override bool IsFinalRebalancing => _productBase.LastTwoYearsPeriod;
    }

    public interface IModelPortfolio
    {
        int RebalancingTreshold { get; set; }
        int Defensive { get; set; }
        int Dynamic { get; set; }
    }

    public class ModelPortfolio : IModelPortfolio
    {
        public string RiskCategory { get; set; }
        public int RebalancingTreshold { get; set; }
        public int Defensive { get; set; }
        public int Dynamic { get; set; }
    }

    public interface IWithdrawal
    {
        void Withdraw(double amount);
    }

    public class InvestmentProduct : ProductBase
    {
        public InvestmentProduct(ModelPortfolio modelPortfolio, bool finalDerisking, bool annualDerisking, double investment, IDateTimeNow dateTimeNow) : base(dateTimeNow)
        {
            ModelPortfolio = modelPortfolio;
            Investment = investment;
            Balance = investment;
            FinalDerisking = finalDerisking;
            AnnualDerisking = annualDerisking;
            ProductId = 8;
        }
    }

    public class PayoutProduct : ProductBase, IWithdrawal
    {
        public PayoutFreequency PayoutFreequency { get; set; }
        public double Payout { get; set; }
        public PayoutProduct(ModelPortfolio modelPortfolio, bool annualDerisking, double investment, IDateTimeNow dateTimeNow): base(dateTimeNow)
        {
            ModelPortfolio = modelPortfolio;
            Investment = investment;
            Balance = investment;
            AnnualDerisking = annualDerisking;
            ProductId = 9;
            FinalDerisking = true;
        }

        public void Withdraw(double amount)
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

        public int ProductId { get; protected set; }
        protected double Investment { get; set; }
        public bool FinalDerisking { get; protected set; }
        public bool AnnualDerisking { get; protected set; }
        protected double Balance { get; set; }
        public int InvestmentLength { get; set; }
        public DateTime Created { get; protected set; }
        public IModelPortfolio ModelPortfolio { get; protected set; }
        public int InvestmentYear =>_dateTimeNow.Now.Year - Created.Year;
        public bool LastTwoYearsPeriod => (InvestmentLength - InvestmentYear) <= 2 ? true : false;
    }

    public enum PayoutFreequency
    {
        Year = 0,
        Quarter = 1,
        Month = 2
    }

    public interface IRebalancerHandler
    {
        void Rebalance(RebalancerBase productBase);
    }

    public class RebalancerHandler : IRebalancerHandler
    {
        public void Rebalance(RebalancerBase productBase)
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

        public DateTime Now => _now;
    }

    public static class Imitator
    {
        public static void DailyRun()
        {
            var dateTimeNow = new DateTimeNow();

            for (int i = 0; i < 20; i++)
            {
                if (i == 19)
                {

                }

                var modelPortfolio = new ModelPortfolio()
                {
                    Defensive = 55,
                    Dynamic = 45,
                    RebalancingTreshold = 55,
                    RiskCategory = "Balance",
                };

                var rebalancerHandler = new RebalancerHandler();

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

                var payoutRebalancer = new PayoutRebalancer(payoutProduct, rebalancerHandler, dateTimeNow);

                var investmentRebalancer = new InvestmentRebalancer(investmentProduct, rebalancerHandler, dateTimeNow);

                investmentRebalancer.Rebalance();

                payoutRebalancer.Rebalance();

                dateTimeNow.AddYear();
            }
        }
    }
}

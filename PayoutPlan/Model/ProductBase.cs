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


    public abstract class RebalancerBase : IRebalanceable
    {
        protected readonly ProductBase _productBase;
        protected IModelPortfolio _modelPortfolio => _productBase.ModelPortfolio;
        protected IRebalancerHandler _rebalancerHandler;

        public RebalancerBase(ProductBase productBase, IRebalancerHandler rebalancerHandler)
        {
            _productBase = productBase;
            _rebalancerHandler = rebalancerHandler;
        }

        public abstract bool StopRebalancing { get; }
        public abstract bool IsFlexibleAllocationRebalancing { get; }
        public abstract bool IsFinalRebalancing { get; }
        public bool IsAnnualRebalancing => _productBase.AnnualDerisking;
        protected bool IsLastTuesday()
        {
            var lastDayOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month));

            while (lastDayOfMonth.DayOfWeek != DayOfWeek.Thursday)
                lastDayOfMonth = lastDayOfMonth.AddDays(-1);

            return DateTime.UtcNow.DayOfYear == lastDayOfMonth.DayOfYear;
        }
        public void Rebalance()
        {
            _rebalancerHandler.Rebalance(this);
        }
    }

    public class InvestmentRebalancer : RebalancerBase
    {
        public InvestmentRebalancer(ProductBase productBase, IRebalancerHandler rebalancerHandler) : base(productBase, rebalancerHandler)
        {

        }

        public override bool IsFlexibleAllocationRebalancing => false;

        public override bool IsFinalRebalancing => _productBase.FinalDerisking;

        public override bool StopRebalancing => _modelPortfolio.Defensive >= 90;
    }

    public class PayoutRebalancer : RebalancerBase
    {
        public PayoutRebalancer(ProductBase productBase, IRebalancerHandler rebalancerHandler) : base(productBase, rebalancerHandler)
        {

        }

        public override bool IsFlexibleAllocationRebalancing => IsLastTuesday() && _modelPortfolio.Dynamic >= _modelPortfolio.RebalancingTreshold;

        public override bool IsFinalRebalancing => _productBase.LastTwoYearsPeriod;

        public override bool StopRebalancing => _modelPortfolio.Defensive >= 90;
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
        public InvestmentProduct(ModelPortfolio modelPortfolio, bool finalDerisking, bool annualDerisking, double investment)
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
        public PayoutProduct(ModelPortfolio modelPortfolio, bool annualDerisking, double investment)
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
        public ProductBase()
        {
            Created = DateTime.UtcNow;
        }

        public int ProductId { get; protected set; }
        protected double Investment { get; set; }
        public bool FinalDerisking { get; protected set; }
        public bool AnnualDerisking { get; protected set; }
        protected double Balance { get; set; }
        public int InvestmentLength { get; set; }
        public DateTime Created { get; protected set; }
        public IModelPortfolio ModelPortfolio { get; protected set; }
        public int InvestmentYear => DateTime.UtcNow.Year - Created.Year;
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
            if (productBase.StopRebalancing) return;

            if(productBase.IsAnnualRebalancing)
            {

            }

            if(productBase.IsFinalRebalancing)
            {

            }

            if(productBase.IsFlexibleAllocationRebalancing)
            {

            }
        }
    }

    public static class Imitator
    {
        public static void DailyRun()
        {
            var modelPortfolio = new ModelPortfolio()
            {
                Defensive = 55,
                Dynamic = 45,
                RebalancingTreshold = 55,
                RiskCategory = "Balance",
            };

            var rebalancerHandler = new RebalancerHandler();

            var payoutProduct = new PayoutProduct(modelPortfolio, annualDerisking: true, investment: 100_000.0D)
            {
                PayoutFreequency = PayoutFreequency.Quarter,
                Payout = 10_000.0D,
                InvestmentLength = 10,
            };

            var investmentProduct = new InvestmentProduct(modelPortfolio, finalDerisking: true, annualDerisking: true, investment: 100_000.0D)
            {
                InvestmentLength = 20
            };

            var payoutRebalancer = new PayoutRebalancer(payoutProduct, rebalancerHandler);

            var investmentRebalancer = new InvestmentRebalancer(investmentProduct, rebalancerHandler);

            investmentRebalancer.Rebalance();

            payoutRebalancer.Rebalance();
        }
    }
}

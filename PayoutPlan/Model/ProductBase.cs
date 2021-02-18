using PayoutPlan.Extensions;
using System;
using System.Collections.Generic;

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
        protected ProductBase ProductBase => _productBase;

        public MonitorBase(ProductBase productBase, IDateTimeNow dateTime)
        {
            _productBase = productBase;
            _dateTime = dateTime;
        }
        public abstract void Invoke();
    }

    public interface IRabalanceMonitor
    {
        bool IsFlexibleAllocationRebalancingTriggered { get; }
        bool IsFinalRebalancingTriggered { get; }
        bool IsAnnualRebalancingTriggered { get; }
    }

    public interface IPayoutMonitor
    {
        bool IsPayoutTriggered { get; }
    }

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
        public bool IsFinalRebalancingTriggered => _productBase.LastTwoYearsPeriod && _productBase.FinalDerisking && _dateTime.Now.IsLastTuesdayInMonth() && _productBase.ModelPortfolio.Defensive < REBALANCING_TRESHOLD;
        public bool IsAnnualRebalancingTriggered => _productBase.AnnualDerisking && _dateTime.Now.IsLastDayInYear() && _productBase.ModelPortfolio.Defensive < REBALANCING_TRESHOLD;

        public override void Invoke()
        {
            _rebalancerHandler.Execute(this, Product);
        }
    }

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
        public bool IsFlexibleAllocationRebalancingTriggered => _dateTime.Now.IsLastTuesdayInMonth() && _modelPortfolio.Dynamic >= _modelPortfolio.RebalancingTreshold;
        public bool IsFinalRebalancingTriggered => _productBase.LastTwoYearsPeriod && _dateTime.Now.IsLastTuesdayInMonth() && _productBase.ModelPortfolio.Defensive < REBALANCING_TRESHOLD;
        public bool IsAnnualRebalancingTriggered => _productBase.AnnualDerisking && _dateTime.Now.IsLastDayInYear() && _productBase.ModelPortfolio.Defensive < REBALANCING_TRESHOLD;
        public bool IsPayoutTriggered => Product.IsPayoutTriggered(_dateTime);

        public override void Invoke()
        {
            _rebalancerHandler.Execute(this, Product);
            _payoutHandler.Execute(this, Product);
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
        public InvestmentProduct(IModelPortfolio modelPortfolio, bool finalDerisking, bool annualDerisking, double investment, int investmentLength, IDateTimeNow dateTimeNow) : base(dateTimeNow)
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

        public PayoutProduct(IModelPortfolio modelPortfolio, 
            bool annualDerisking, 
            double investment, 
            PayoutFreequency payoutFreequency, 
            double payout, 
            int investmentLength, 
            IDateTimeNow dateTimeNow) : base(dateTimeNow)
        {
            ModelPortfolio = modelPortfolio;
            Investment = investment;
            Balance = investment;
            AnnualDerisking = annualDerisking;
            ProductType = ProductType.Payout;
            FinalDerisking = true;
            PayoutFreequency = payoutFreequency;
            Payout = payout;
            InvestmentLength = investmentLength;
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
        public IDateTimeNow DateTimeNow => _dateTimeNow;
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
        Security = 1,
        Income = 2,
        Balance = 3,
        Growth = 4,
        ActionOriented = 5
    }

    public interface IPayoutHandler
    {
        void Execute(IPayoutMonitor monitor, ProductBase product);
    }

    public class PayoutHandler : IPayoutHandler
    {
        public void Execute(IPayoutMonitor monitor, ProductBase product)
        {
            if (monitor.IsPayoutTriggered)
            {
                //example
                product.Withdraw();

                Console.WriteLine("{1} | Payout: {0}", product.Balance, product.DateTimeNow.Now);
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
            if (monitor.IsAnnualRebalancingTriggered)
            {
                //example
                product.ModelPortfolio.Defensive++;
                product.ModelPortfolio.Dynamic--;

                Console.WriteLine("{2} | Annual Rebalancing: {0} {1}", product.ModelPortfolio.Defensive, product.ModelPortfolio.Dynamic, product.DateTimeNow.Now);
            }

            if (monitor.IsFinalRebalancingTriggered)
            {
                //example
                product.ModelPortfolio.Defensive++;
                product.ModelPortfolio.Dynamic--;

                Console.WriteLine("{2} | Final Rebalancing: {0} {1}", product.ModelPortfolio.Defensive, product.ModelPortfolio.Dynamic, product.DateTimeNow.Now);
            }

            if (monitor.IsFlexibleAllocationRebalancingTriggered)
            {
                //example
                product.ModelPortfolio.Defensive++;
                product.ModelPortfolio.Dynamic--;

                Console.WriteLine("{2} | FlexibleA llocation Rebalancing: {0} {1}", product.ModelPortfolio.Defensive, product.ModelPortfolio.Dynamic, product.DateTimeNow.Now);
            }
        }
    }

    public interface IMonitorHandler
    {
        void Monitor(IEnumerable<ProductBase> productsBase);
    }

    public class MonitorHandler : IMonitorHandler
    {
        private readonly IMonitorFactory _monitorFactory;
        public MonitorHandler(IMonitorFactory monitorFactory)
        {
            _monitorFactory = monitorFactory;
        }

        public void Monitor(IEnumerable<ProductBase> productsBase)
        {
            foreach(var productBase in productsBase)
            {
                var monitor = _monitorFactory.Instance(productBase);

                monitor.Invoke();
            }
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
        private readonly IDateTimeNow _dateTimeNow;

        public MonitorFactory(IDateTimeNow dateTimeNow, IRebalanceHandler rebalancerHandler, IPayoutHandler payoutHandler)
        {
            _dateTimeNow = dateTimeNow;
            _rebalancerHandler = rebalancerHandler;
            _payoutHandler = payoutHandler;
        }

        public MonitorBase Instance(ProductBase productBase)
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

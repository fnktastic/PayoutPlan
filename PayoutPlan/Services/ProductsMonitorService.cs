using PayoutPlan.Helpers;
using PayoutPlan.Model;
using PayoutPlan.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoutPlan.Services
{
    public interface IProductsMonitorService
    {
        void Monitor();
    }

    public class ProductsMonitorService : IProductsMonitorService
    {
        private readonly IDateTimeNow _dateTimeNow;
        private readonly IModelPortfolioRepository _modelPortfolioRepository;
        private readonly IProductRepository _productRepository; 
        private readonly IRebalancerHandler _rebalancerHandler; 
        private readonly IWithdrawalHandler _withdrawalHandler;
        private readonly IPayoutHelper _payoutHelper; 
        private readonly IMonitorFactory _monitorFactory; 
        private readonly IMonitorHandler _monitorHandler;

        public ProductsMonitorService()
        {
             _dateTimeNow = new DateTimeNow();
            _modelPortfolioRepository = new ModelPortfolioRepository();
            _productRepository = new ProductRepository(_modelPortfolioRepository, _dateTimeNow);
            _rebalancerHandler = new RebalancerHandler();
            _withdrawalHandler = new WithdrawalHandler();
            _payoutHelper = new PayoutHelper();
            _monitorFactory = new MonitorFactory(_dateTimeNow, _rebalancerHandler, _withdrawalHandler, _payoutHelper);
            _monitorHandler = new MonitorHandler(_monitorFactory);
        }

        public void Monitor()
        {
            DailyRun();
        }

        private void DailyRun()
        {
            var payoutProduct = _productRepository.Get(ProductType.Payout);
            var investmentProduct = _productRepository.Get(ProductType.Investment);

            int day = 1;
            var endOfProductLife = _dateTimeNow.Now.AddYears(20);
            while (_dateTimeNow.Now < endOfProductLife) //going through the 20 years of product life
            {
                _monitorHandler.Monitor(payoutProduct);
                _monitorHandler.Monitor(investmentProduct);

                if ((day % 10) == 0)
                {
                    PrintProductState(payoutProduct, day);
                }

                day++;
                _dateTimeNow.AddDay();
            }
        }

        private void PrintProductState(ProductBase productBase, int day)
        {
            Console.WriteLine($"Day {day}: {productBase.ProductType}, Balance: {productBase.Balance} | Defensive: {productBase.ModelPortfolio.Defensive} Dynamic: {productBase.ModelPortfolio.Dynamic} ");
        }
    }
}

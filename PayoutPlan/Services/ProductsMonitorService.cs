using PayoutPlan.Extensions;
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

        public ProductsMonitorService(IDateTimeNow dateTimeNow)
        {
            _dateTimeNow = dateTimeNow;
            _modelPortfolioRepository = new ModelPortfolioRepository();
            _productRepository = new ProductRepository(_modelPortfolioRepository, dateTimeNow);
            _rebalancerHandler = new RebalancerHandler();
            _withdrawalHandler = new WithdrawalHandler();
            _payoutHelper = new PayoutHelper();
            _monitorFactory = new MonitorFactory(dateTimeNow, _rebalancerHandler, _withdrawalHandler, _payoutHelper);
            _monitorHandler = new MonitorHandler(_monitorFactory);
        }

        public void Monitor()
        {
            var payoutProduct = _productRepository.Get(ProductType.Payout);

            var investmentProduct = _productRepository.Get(ProductType.Investment);

            var endOfProductLife = _dateTimeNow.Now.AddYears(20);

            while (_dateTimeNow.Now < endOfProductLife) //going through the 20 years of product life
            {
                DailyMonitoring(payoutProduct); //daily monitor payout

                DailyMonitoring(investmentProduct); //daily monitor investment

                _dateTimeNow.AddDay(); // imitate next day
            }
            
        }

        private void DailyMonitoring(ProductBase productBase)
        {
            _monitorHandler.Monitor(productBase);

            /*if (_dateTimeNow.Now.IsLastTuesdayInMonth())
            {
                Console.WriteLine($"Date {_dateTimeNow.Now}: {productBase.ProductType}, Balance: {productBase.Balance} | Defensive: {productBase.ModelPortfolio.Defensive} Dynamic: {productBase.ModelPortfolio.Dynamic} ");
            }*/
        }
    }
}

﻿using PayoutPlan.Model;
using PayoutPlan.Repository;
using System.Collections.Generic;

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
        private readonly IRebalanceHandler _rebalancerHandler;
        private readonly IPayoutHandler _payoutHandler;
        private readonly IMonitorFactory _monitorFactory;
        private readonly IMonitorHandler _monitorHandler;

        public ProductsMonitorService(IDateTimeNow dateTimeNow)
        {
            _dateTimeNow = dateTimeNow;
            _modelPortfolioRepository = new ModelPortfolioRepository();
            _productRepository = new ProductRepository(_modelPortfolioRepository, dateTimeNow);
            _rebalancerHandler = new RebalanceHandler();
            _payoutHandler = new PayoutHandler();
            _monitorFactory = new MonitorFactory(dateTimeNow, _rebalancerHandler, _payoutHandler);
            _monitorHandler = new MonitorHandler(_monitorFactory);
        }

        public void Monitor()
        {
            var payoutProduct = _productRepository.Get(ProductType.Payout);

            var investmentProduct = _productRepository.Get(ProductType.Investment);

            var endOfProductLife = _dateTimeNow.Now.AddYears(20);

            while (_dateTimeNow.Now <= endOfProductLife) //going through the 20 years of product life
            {
                _monitorHandler.Monitor(new List<ProductBase>() { payoutProduct/*, investmentProduct*/ });

                _dateTimeNow.AddDay(); // imitate next day
            }
            
        }
    }
}

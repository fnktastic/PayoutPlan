﻿using Qcentris.Monitoring.ServiceEnum;
using Qcentris.Monitoring.ServiceFactories;
using Qcentris.Monitoring.ServiceHandlers;
using Qcentris.Monitoring.ServiceInterfaces.Common;
using Qcentris.Monitoring.ServiceModels;
using Qcentris.Monitoring.ServiceRepository;
using System.Collections.Generic;

namespace Qcentris.Monitoring.ServiceServices
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
        private readonly IBehaviourFactory _behaviourFactory;

        public ProductsMonitorService(IDateTimeNow dateTimeNow)
        {
            _dateTimeNow = dateTimeNow;
            _modelPortfolioRepository = new ModelPortfolioRepository();
            _behaviourFactory = new BehaviourFactory();
            _productRepository = new ProductRepository(_modelPortfolioRepository, _behaviourFactory, _dateTimeNow);
            _rebalancerHandler = new RebalanceHandler(_behaviourFactory);
            _payoutHandler = new PayoutHandler(_behaviourFactory);
            _monitorFactory = new MonitorFactory(_dateTimeNow, _rebalancerHandler, _payoutHandler);
            _monitorHandler = new MonitorHandler(_monitorFactory);
        }

        public void Monitor()
        {
            var payoutProduct = _productRepository.Get(ProductTypeEnum.Payout);

            var investmentProduct = _productRepository.Get(ProductTypeEnum.Investment);

            var products = new List<ProductBase>()
            {
                payoutProduct,
                investmentProduct
            };

            var endOfProductLife = _dateTimeNow.Now.AddYears(20);

            while (_dateTimeNow.Now <= endOfProductLife) //going through the 20 years of product life
            {
                _monitorHandler.Monitor(products);

                _dateTimeNow.AddDay(); // imitate next day
            }

        }
    }
}

using PayoutPlan.Factories;
using PayoutPlan.Models;
using System.Collections.Generic;

namespace PayoutPlan.Handlers
{
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
            foreach (var productBase in productsBase)
            {
                _monitorFactory.Create(productBase).Invoke();
            }
        }
    }
}

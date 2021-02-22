using Qcentris.Monitoring.ServiceFactories;
using Qcentris.Monitoring.ServiceModels;
using System.Collections.Generic;

namespace Qcentris.Monitoring.ServiceHandlers
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

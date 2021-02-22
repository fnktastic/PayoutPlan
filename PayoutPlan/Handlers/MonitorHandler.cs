using PayoutPlan.Factories;
using PayoutPlan.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                _monitorFactory.Instance(productBase).Invoke();
            }
        }
    }
}

using Qcentris.Monitoring.ServiceInterfaces;
using Qcentris.Monitoring.ServiceInterfaces.Common;

namespace Qcentris.Monitoring.ServiceMonitor
{
    public interface IMonitor
    {
        void Invoke();
    }

    public abstract class MonitorBase : IMonitor
    {
        protected readonly IProduct _productBase;

        protected readonly IDateTimeNow _dateTime;
        protected IProduct ProductBase => _productBase;

        public MonitorBase(IProduct productBase, IDateTimeNow dateTime)
        {
            _productBase = productBase;
            _dateTime = dateTime;
        }
        public abstract void Invoke();
    }




}

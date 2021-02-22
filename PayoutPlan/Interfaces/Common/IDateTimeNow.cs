using System;

namespace Qcentris.Monitoring.ServiceInterfaces.Common
{
    public interface IDateTimeNow
    {
        void AddYear();
        void AddDay();
        DateTime Now { get; }
    }
}

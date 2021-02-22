using System;

namespace PayoutPlan.Interfaces.Common
{
    public interface IDateTimeNow
    {
        void AddYear();
        void AddDay();
        DateTime Now { get; }
    }
}

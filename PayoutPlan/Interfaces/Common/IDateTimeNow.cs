using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoutPlan.Interfaces.Common
{
    public interface IDateTimeNow
    {
        void AddYear();
        void AddDay();
        DateTime Now { get; }
    }
}

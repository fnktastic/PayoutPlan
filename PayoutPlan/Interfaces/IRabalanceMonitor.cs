using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoutPlan.Interfaces
{
    public interface IRabalanceMonitor
    {
        bool IsFlexibleAllocationRebalancingTriggered { get; }
        bool IsFinalRebalancingTriggered { get; }
        bool IsAnnualRebalancingTriggered { get; }
    }
}

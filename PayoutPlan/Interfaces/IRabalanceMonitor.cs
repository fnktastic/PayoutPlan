namespace PayoutPlan.Interfaces
{
    public interface IRabalanceMonitor
    {
        bool IsFlexibleAllocationRebalancingTriggered { get; }
        bool IsFinalRebalancingTriggered { get; }
        bool IsAnnualRebalancingTriggered { get; }
    }
}

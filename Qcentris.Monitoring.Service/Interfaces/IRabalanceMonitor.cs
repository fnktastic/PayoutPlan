namespace Qcentris.Monitoring.ServiceInterfaces
{
    public interface IRabalanceMonitor
    {
        bool IsFlexibleAllocationRebalancingTriggered { get; }
        bool IsFinalRebalancingTriggered { get; }
        bool IsAnnualRebalancingTriggered { get; }
    }
}

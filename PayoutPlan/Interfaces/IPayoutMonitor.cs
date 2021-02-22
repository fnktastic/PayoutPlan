namespace PayoutPlan.Interfaces
{
    public interface IPayoutMonitor
    {
        bool IsPayoutTriggered { get; }
    }
}

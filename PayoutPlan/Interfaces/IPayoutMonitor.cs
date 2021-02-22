namespace Qcentris.Monitoring.ServiceInterfaces
{
    public interface IPayoutMonitor
    {
        bool IsPayoutTriggered { get; }
    }
}

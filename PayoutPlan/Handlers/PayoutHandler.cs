using PayoutPlan.Interfaces;
using PayoutPlan.Model;
using System;

namespace PayoutPlan.Handlers
{
    public interface IPayoutHandler
    {
        void Execute(IPayoutMonitor monitor, ProductBase product);
    }

    public class PayoutHandler : IPayoutHandler
    {
        public void Execute(IPayoutMonitor monitor, ProductBase product)
        {
            if (monitor.IsPayoutTriggered)
            {
                //example
                product.Withdrawal.Execute();

                Console.WriteLine("{1} | Payout: {0}", product.Balance, product.DateTimeNow.Now);
            }
        }
    }
}

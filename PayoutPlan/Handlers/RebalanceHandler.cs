using PayoutPlan.Interfaces;
using PayoutPlan.Model;
using System;

namespace PayoutPlan.Handlers
{
    public interface IRebalanceHandler
    {
        void Execute(IRabalanceMonitor monitor, ProductBase product);
    }

    public class RebalanceHandler : IRebalanceHandler
    {
        public void Execute(IRabalanceMonitor monitor, ProductBase product)
        {
            if (monitor.IsAnnualRebalancingTriggered)
            {
                //example
                product.ModelPortfolio.Defensive++;
                product.ModelPortfolio.Dynamic--;

                Console.WriteLine("{2} | Annual Rebalancing: {0} {1}", product.ModelPortfolio.Defensive, product.ModelPortfolio.Dynamic, product.DateTimeNow.Now);
            }

            if (monitor.IsFinalRebalancingTriggered)
            {
                //example
                product.ModelPortfolio.Defensive++;
                product.ModelPortfolio.Dynamic--;

                Console.WriteLine("{2} | Final Rebalancing: {0} {1}", product.ModelPortfolio.Defensive, product.ModelPortfolio.Dynamic, product.DateTimeNow.Now);
            }

            if (monitor.IsFlexibleAllocationRebalancingTriggered)
            {
                //example
                product.ModelPortfolio.Defensive++;
                product.ModelPortfolio.Dynamic--;

                Console.WriteLine("{2} | FlexibleA llocation Rebalancing: {0} {1}", product.ModelPortfolio.Defensive, product.ModelPortfolio.Dynamic, product.DateTimeNow.Now);
            }
        }
    }
}

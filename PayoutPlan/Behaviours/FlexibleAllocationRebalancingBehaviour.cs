using PayoutPlan.Interfaces;
using PayoutPlan.Models;
using System;

namespace PayoutPlan.Behaviours
{
    public class FlexibleAllocationRebalancingBehaviour : IBehaviour
    {
        protected readonly ProductBase _productBase;

        public FlexibleAllocationRebalancingBehaviour(ProductBase productBase)
        {
            _productBase = productBase;
        }

        public void Execute()
        {
            //example
            _productBase.ModelPortfolio.Defensive++;
            _productBase.ModelPortfolio.Dynamic--;

            Console.WriteLine("{2} | FlexibleA llocation Rebalancing: {0} {1}", _productBase.ModelPortfolio.Defensive, _productBase.ModelPortfolio.Dynamic, _productBase.DateTimeNow.Now);
        }
    }
}

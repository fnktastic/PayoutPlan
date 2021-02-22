using PayoutPlan.Interfaces;
using PayoutPlan.Models;
using System;

namespace PayoutPlan.Behaviours
{

    public class AnnualRebalancingBehaviour : IBehaviour
    {
        protected readonly ProductBase _productBase;

        public AnnualRebalancingBehaviour(ProductBase productBase)
        {
            _productBase = productBase;
        }

        public void Execute()
        {
            //example
            _productBase.ModelPortfolio.Defensive++;
            _productBase.ModelPortfolio.Dynamic--;

            Console.WriteLine("{2} | Annual Rebalancing: {0} {1}", _productBase.ModelPortfolio.Defensive, _productBase.ModelPortfolio.Dynamic, _productBase.DateTimeNow.Now);
        }
    }
}

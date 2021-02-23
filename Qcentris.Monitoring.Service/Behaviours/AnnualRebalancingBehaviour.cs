using Qcentris.Monitoring.ServiceInterfaces;
using Qcentris.Monitoring.ServiceModels;
using System;

namespace Qcentris.Monitoring.ServiceBehaviours
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

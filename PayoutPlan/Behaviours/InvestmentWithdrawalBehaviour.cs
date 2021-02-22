using PayoutPlan.Interfaces;
using PayoutPlan.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoutPlan.Behaviours
{
    public class InvestmentWithdrawalBehaviour : IBehaviour
    {
        protected readonly ProductBase _productBase;

        public InvestmentWithdrawalBehaviour(ProductBase productBase)
        {
            _productBase = productBase;
        }

        public void Execute()
        {
            //behave
        }
    }
}

using PayoutPlan.Interfaces;
using PayoutPlan.Model;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoutPlan.Behaviours
{
    public class PayoutWithdrawalBehaviour : IBehaviour
    {
        protected readonly ProductBase _productBase;

        public PayoutWithdrawalBehaviour(ProductBase productBase)
        {
            _productBase = productBase;
        }

        public void Execute()
        {
            //withrdawal
        }
    }
}

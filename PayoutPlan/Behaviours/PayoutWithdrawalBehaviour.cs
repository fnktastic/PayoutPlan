using PayoutPlan.Interfaces;
using PayoutPlan.Model;

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

using PayoutPlan.Interfaces;
using PayoutPlan.Model;

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

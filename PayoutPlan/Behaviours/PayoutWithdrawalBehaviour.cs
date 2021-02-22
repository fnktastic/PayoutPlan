using PayoutPlan.Interfaces;
using PayoutPlan.Models;
using System;

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
            if (_productBase is PayoutProduct payoutProduct)
            {
                payoutProduct.Balance -= payoutProduct.Payout;

                Console.WriteLine("{1} | Payout: {0}", payoutProduct.Balance, payoutProduct.DateTimeNow.Now);
            }
        }
    }
}

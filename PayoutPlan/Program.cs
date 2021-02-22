using PayoutPlan.Common;
using PayoutPlan.Interfaces.Common;
using PayoutPlan.Services;
using System;

namespace PayoutPlan
{
    class Program
    {
        static void Main(string[] args)
        {
            IDateTimeNow dateTimeNow = new DateTimeNow();

            IProductsMonitorService _productsMonitorService = new ProductsMonitorService(dateTimeNow);

            _productsMonitorService.Monitor();

            Console.ReadKey();
        }
    }
}

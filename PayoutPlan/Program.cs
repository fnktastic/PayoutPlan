using PayoutPlan.Common;
using PayoutPlan.Interfaces.Common;
using PayoutPlan.Model;
using PayoutPlan.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

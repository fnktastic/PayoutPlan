using Qcentris.Monitoring.ServiceCommon;
using Qcentris.Monitoring.ServiceInterfaces.Common;
using Qcentris.Monitoring.ServiceServices;
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

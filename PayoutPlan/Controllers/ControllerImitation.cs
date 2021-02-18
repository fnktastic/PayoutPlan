using PayoutPlan.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoutPlan.Controllers
{
    public static class ControllerImitation
    {
        private static readonly IProductsMonitorService _productsMonitorService = new ProductsMonitorService();

        public static void DailyRun()
        {
            _productsMonitorService.Monitor();
        }
    }
}

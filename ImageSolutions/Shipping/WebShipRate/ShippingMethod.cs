using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Shipping.WebShipRate
{
    public class ShippingMethod
    {
        public double? rate { get; set; }
        public string name { get;set; }
        public string shipmethod { get; set; }
        public string shipcarrier { get; set; }
        public string rate_formatted { get; set; }
        public int service_days { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Shipping.WebShipRate
{
    public class WebShipRateResponse
    {
        public List<ShippingMethod> shippingMethods { get; set; }
        public double cartWeight { get; set; }
        public Available available { get; set; }
        public Available backOrderNonInvt { get; set; }
    }
}

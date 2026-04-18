using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Shipping
{
    public class ShipHawkFinalResponse
    {
        public string ShipHawkID { get; set; }
        public decimal Cost { get; set; }
        public string ShippingLabel { get; set; }
        public string PackingSlip { get; set; }
        public string TrackingNumber { get; set; }
    }
}

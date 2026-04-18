using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Shipping
{
    public class ShipHawkCreateProposedShipment
    {
        public string rate_id { get; set; }
        public Origin_Address origin_address { get; set; }
        public Destination_Address destination_address { get; set; }
        public List<Package> packages { get; set; }
        public List<Shipment_Line_Items> shipment_line_items { get; set; }
    }    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Shipping.WebShipRate
{
    public class WebShipRate
    {
        public string SiteNumber { get; set; }
        public List<Item> lineItems { get; set; }
        public string FromLocation { get; set; }
        public Address ShipAddress { get; set; }
        public List<SiteMethods> SiteMethods { get; set; }
        public List<string> Promos { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Shipping.WebShipRate
{
    public class Item
    {
        public string internalid { get; set; }
        public string itemid { get; set; }
        public int quantity { get; set; }
        public double weight { get; set; }
        public string itemtype { get; set; }
        public bool flatOk { get; set; }
        public double amount { get; set; }
    }
}

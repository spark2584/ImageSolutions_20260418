using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.ShoppingCart
{
    public class ShoppingCartTransfer
    {
        public class ReturnCart
        {
            public Body body { get; set; }
        }

        public class Body
        {
            public int edit_mode { get; set; }
            public Items items { get; set; }
            public double total { get; set; }
            public string currency { get; set; }
        }

        public class Items
        {
            public List<Item> items { get; set; }
        }

        public class Item
        {
            public int quantity { get; set; }
            public string supplierid { get; set; }
            public string supplierauxid { get; set; }
            public string unitprice { get; set; }
            public string currency { get; set; }
            public object classification { get; set; }
            public object classdomain { get; set; }
            public string uom { get; set; }
            public string description { get; set; }
            public string language { get; set; }
        }

        public class ItemOptions
        {
            public List<Option> Opts { get; set; }
        }

        public class Option
        {
            public string id { get; set; }
            public string value { get; set; }
        }

    }
}

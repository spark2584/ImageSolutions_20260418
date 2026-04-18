using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Shipping
{
    public class ShipHawkCreateShipmentBody
    {
        public string rate_id { get; set; }
        public string order_number { get; set; }
        public Origin_Address origin_address { get; set; }
        public Destination_Address destination_address { get; set; }
        public package[] packages { get; set; }
    }

    public class Origin_Address
    {
        public string name { get; set; }
        public string company { get; set; }
        public string street1 { get; set; }
        public string street2 { get; set; }
        public string zip { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string phone_number { get; set; }
        public string country { get; set; }
    }

    public class Destination_Address
    {
        public string name { get; set; }
        public string company { get; set; }
        public string street1 { get; set; }
        public string street2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string phone_number { get; set; }
        public string country { get; set; }
    }

    public class package
    {
        public package_item[] package_items { get; set; }
        public string packing_type { get; set; }
        public string type { get; set; }
        public string length { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string weight { get; set; }
        public float value { get; set; }
    }
    public class package_item
    {
        public string id { get; set; }
        public object product_sku { get; set; }
        public object product_upc { get; set; }
        public object product_sku_packing_code { get; set; }
        public string item_name { get; set; }
        public int length { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public float weight { get; set; }
        public float volume { get; set; }
        public object freight_class { get; set; }
        public object nmfc { get; set; }
        public object hs_code { get; set; }
        public object country_of_origin { get; set; }
        public int value { get; set; }
        public string description { get; set; }
        public object order_line_item_id { get; set; }
        public int quantity { get; set; }
        public object order_line_item_source_system_id { get; set; }
        public object order_line_item_source_system_line_number { get; set; }

    }
}

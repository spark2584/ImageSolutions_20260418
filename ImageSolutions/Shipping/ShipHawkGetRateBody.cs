using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Shipping
{
    public class ShipHawkGetRateBody
    {
        public ShiphawkParcel[] items { get; set; }
        public Origin_Address origin_address { get; set; }
        public Destination_Address destination_address { get; set; }
    }

    public class ShiphawkParcel
    {
        public string item_type { get; set; }
        public string type { get; set; }
        public string length { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string weight { get; set; }
        public string weight_uom { get; set; }
        public float value { get; set; }
        public int quantity { get; set; }
        public string product_sku { get; set; }
        public string hs_code { get; set; }
        public string country_of_origin { get; set; }
        //public HarmonizedCodeMapping harmonized_codes { get; set; }
    }

    public class HarmonizedCodeMapping
    {
        public string harmonized_code { get; set; }
        public string country_code { get; set; }
    }

}

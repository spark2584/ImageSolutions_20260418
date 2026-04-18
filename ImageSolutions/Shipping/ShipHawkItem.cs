using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Shipping
{
    public class ShipHawkItem
    {
        public string sku { get; set; }
        public object upc { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public object child_sku { get; set; }
        public object child_sku_qty { get; set; }
        public object child_sku_qty_min { get; set; }
        public string image_url { get; set; }
        public object packing_code { get; set; }
        public object price { get; set; }
        public bool valid_for_rating { get; set; }
        public bool hazmat { get; set; }
        public bool orm_d { get; set; }
        public object units_per_sku { get; set; }
        public bool aggregate_weight { get; set; }
        public object ship_individually { get; set; }
        public string external_id { get; set; }
        public Kit_Skus kit_skus { get; set; }
        public object[] kit_components { get; set; }
        public Kit_Sku_Value_Percentages kit_sku_value_percentages { get; set; }
        public bool kit_skus_as_one_line_item_for_customs { get; set; }
        public Items[] items { get; set; }
        public bool is_kit { get; set; }
        public object inventory_identification { get; set; }
        public object category { get; set; }
        public object subcategory { get; set; }
        public object[] parent_skus { get; set; }
    }

    public class Kit_Skus
    {
    }

    public class Kit_Sku_Value_Percentages
    {
    }

    public class Items
    {
        public int id { get; set; }
        public object length { get; set; }
        public object width { get; set; }
        public object height { get; set; }
        public float weight { get; set; }
        public object unpacked_item_type_id { get; set; }
        public string item_type { get; set; }
        public string type_of_item { get; set; }
        public string description { get; set; }
        public object handling_unit_type { get; set; }
        public object package_type { get; set; }
        public object package_quantity { get; set; }
        public bool requires_crating { get; set; }
        public object do_not_turn { get; set; }
        public object put_on_pallet { get; set; }
        public bool allowed_in_envelope { get; set; }
        public object[] envelope_types { get; set; }
        public int quantity { get; set; }
        public object freight_class { get; set; }
        public object nmfc { get; set; }
        public bool hazmat { get; set; }
        public bool orm_d { get; set; }
        public object commodity_description { get; set; }
        public object[] harmonized_codes { get; set; }
        public object country_of_origin { get; set; }
        public object value { get; set; }
        public object kit_sku { get; set; }
        public object kit_sku_upc { get; set; }
        public object kit_sku_name { get; set; }
        public object inventory_identification { get; set; }
        public bool do_not_pack_with_other_items { get; set; }
        public bool do_not_palletize { get; set; }
        public bool do_not_pack_before_palletize { get; set; }
        public object can_be_rolled { get; set; }
        public object can_be_folded { get; set; }
        public object can_be_stacked { get; set; }
        public object stacking_offset { get; set; }
        public string product_sku { get; set; }
    }
}

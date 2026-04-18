using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Shipping
{
    public class ShipHawkShipment
    {
        public string id { get; set; }
        public int shid { get; set; }
        public string status { get; set; }
        public Origin_Address origin_address { get; set; }
        public Destination_Address destination_address { get; set; }
        public object ultimate_consignee_address { get; set; }
        public string carrier { get; set; }
        public string carrier_code { get; set; }
        public string carrier_scac { get; set; }
        public string carrier_type_code { get; set; }
        public bool is_external { get; set; }
        public object external_carrier_scac { get; set; }
        public bool is_customer_tariff { get; set; }
        public bool is_international { get; set; }
        public string service_name { get; set; }
        public object service_level { get; set; }
        public string insurance_type { get; set; }
        public float total_price { get; set; }
        public string tracking_number { get; set; }
        public string tracking_page_url { get; set; }
        public object last_mile_tracking_number { get; set; }
        public Document[] documents { get; set; }
        public Price_Details price_details { get; set; }
        public Account_Cost_Details account_cost_details { get; set; }
        public object dispatch { get; set; }
        public Package[] packages { get; set; }
        public Shipment_Line_Items[] shipment_line_items { get; set; }
        public Reference_Numbers[] reference_numbers { get; set; }
        public object origin_instructions { get; set; }
        public object destination_instructions { get; set; }
        public bool print_package_labels_enabled { get; set; }
        public object[] exceptions { get; set; }
        public DateTime est_delivery_date { get; set; }
        public object service_days { get; set; }
        public object order_id { get; set; }
        public object order_number { get; set; }
        public object[] combined_order_numbers { get; set; }
        public object[] order_line_items_quantity { get; set; }
        public string label_url { get; set; }
        public string label_format { get; set; }
        public string label_document_id { get; set; }
        public string label_pdf_url { get; set; }
        public string label_pdf_document_id { get; set; }
        public string label_zpl_url { get; set; }
        public string label_zpl_document_id { get; set; }
        public object end_of_day_manifest_date { get; set; }
        public DateTime created_at { get; set; }
        //public Created_By created_by { get; set; }
        public DateTime updated_at { get; set; }
        public object actual_pickup_time { get; set; }
        public Shipment_Billing shipment_billing { get; set; }
        public object duties_taxes_billing { get; set; }
        public object packing_slip_template { get; set; }
        public bool can_attach_bol { get; set; }
        public string cannot_attach_bol_reason { get; set; }
        public object[] accessorials { get; set; }
        public bool is_generate_commercial_invoice { get; set; }
        public bool can_dispatch { get; set; }
        public DateTime ship_date { get; set; }
        public object pro_number { get; set; }
        public string recipient_name { get; set; }
        public object proposed_shipment_id { get; set; }
        public Email_Docs email_docs { get; set; }
        public object[] customs_data_list { get; set; }
        public Eei eei { get; set; }
        //public Carrier_Account carrier_account { get; set; }
        public object warehouse { get; set; }
        public External_Shipping_Price_Markup external_shipping_price_markup { get; set; }
        public bool local_cancellation_only { get; set; }
        public string[] warnings { get; set; }
        public object license_plate_number { get; set; }
        public object[] all_license_plate_numbers { get; set; }
    }

    public class Price_Details
    {
        public float shipping { get; set; }
        public float packing { get; set; }
        public float insurance { get; set; }
        public float pickup { get; set; }
        public float delivery { get; set; }
        public int accessorials { get; set; }
    }

    public class Account_Cost_Details
    {
        public float account_total_cost { get; set; }
        public float account_total_price { get; set; }
        public float account_shipping_cost { get; set; }
        public float account_packing_cost { get; set; }
        public float account_pickup_cost { get; set; }
        public float account_delivery_cost { get; set; }
        public float account_insurance_cost { get; set; }
        public float account_accessorial_cost { get; set; }
    }


    public class Shipment_Billing
    {
        public object shortcut_account_number { get; set; }
        public object account_number { get; set; }
        public object name { get; set; }
        public object company { get; set; }
        public object phone_number { get; set; }
        public object street1 { get; set; }
        public object street2 { get; set; }
        public object city { get; set; }
        public object state { get; set; }
        public string country { get; set; }
        public object zip { get; set; }
        public string bill_to { get; set; }
        public string carrier_code { get; set; }
        public string service_code { get; set; }
        public string service_name { get; set; }
    }

    public class Email_Docs
    {
        public object recipient_email { get; set; }
        public string email_content { get; set; }
    }

    public class Eei
    {
        public object compliance { get; set; }
        public object compliance_code { get; set; }
    }


    public class External_Shipping_Price_Markup
    {
        public string type { get; set; }
        public int value { get; set; }
    }

    public class Document
    {
        public string id { get; set; }
        public bool customer_uploaded { get; set; }
        public string type { get; set; }
        public object template_level { get; set; }
        public string extension { get; set; }
        public string code { get; set; }
        public string url { get; set; }
        public Meta_Data meta_data { get; set; }
        public DateTime created_at { get; set; }
    }

    public class Meta_Data
    {
    }

    public class Package
    {
        public string id { get; set; }
        public string tracking_number { get; set; }
        public string tracking_url { get; set; }
        public object return_tracking_number { get; set; }
        public string freight_class { get; set; }
        public object nmfc { get; set; }
        public string packing_type { get; set; }
        public object package_type { get; set; }
        public object handling_unit_type { get; set; }
        public int quantity { get; set; }
        public int package_quantity { get; set; }
        public float? length { get; set; }
        public float? width { get; set; }
        public float? height { get; set; }
        public float? weight { get; set; }
        public object dry_ice_weight { get; set; }
        public float? value { get; set; }
        public float? volume { get; set; }
        public string commodity_description { get; set; }
        public string label_document_id { get; set; }
        public bool dangerous { get; set; }
        public object carrier_container { get; set; }
        public object carrier_container_mapped { get; set; }
        public object preset_container { get; set; }
        public object material_container_kind { get; set; }
        public float? materials_weight { get; set; }
        public int? number_of_units { get; set; }
        public object license_plate_number { get; set; }
        public object[] sscc_serial_references { get; set; }
        public float? total_packing_cost { get; set; }
        public object[] accessorials { get; set; }
        public object[] hazmat_data_list { get; set; }
        public Package_Items[] package_items { get; set; }
        public object[] handling_unit_packages { get; set; }
        public object[] materials { get; set; }
        public object[] labors { get; set; }
    }

    public class Package_Items
    {
        public object sequence_number { get; set; }
        public object product_sku { get; set; }
        public string product_name { get; set; }
        public int quantity { get; set; }
        public float value { get; set; }
        public float weight { get; set; }
        public object inventory_identification { get; set; }
        public object inventory_identifier_code { get; set; }
        public object inventory_identifier_id { get; set; }
        public object order_line_item_id { get; set; }
        public object line_item_sku_component_id { get; set; }
    }

    public class Shipment_Line_Items
    {
        public object order_line_item_id { get; set; }
        public int? quantity { get; set; }
        public object name { get; set; }
        public object description { get; set; }
        public object sku { get; set; }
        public object upc { get; set; }
        public float value { get; set; }
        public float weight { get; set; }
        public object hs_code { get; set; }
        public object country_of_origin { get; set; }
        public object child_sku { get; set; }
        public object child_sku_quantity { get; set; }
        public object line_number { get; set; }
        public object source_system_line_number { get; set; }
        public object source_system_id { get; set; }
        public object origin_order_number { get; set; }
        public Line_Item_Skus[] line_item_skus { get; set; }
    }

    public class Line_Item_Skus
    {
        public string id { get; set; }
        public string name { get; set; }
        public object sku { get; set; }
        public int? quantity { get; set; }
        public int? quantity_aggregated { get; set; }
        public object child_sku { get; set; }
        public object child_sku_quantity { get; set; }
        public float? value { get; set; }
        public float? length { get; set; }
        public float? width { get; set; }
        public float? height { get; set; }
        public float? weight { get; set; }
        public object item_type { get; set; }
        public object unpacked_item_type_id { get; set; }
        public object handling_unit_type { get; set; }
        public object hs_code { get; set; }
        public object freight_class { get; set; }
        public object package_type { get; set; }
        public object package_quantity { get; set; }
        public object upc { get; set; }
        public object country_of_origin { get; set; }
        public object description { get; set; }
        public object commodity_description { get; set; }
        public object bin_number { get; set; }
        public object mpn { get; set; }
        public object gtin { get; set; }
        public object asin { get; set; }
        public object inventory_identification { get; set; }
        public object[] inventory_identifier_codes { get; set; }
        public object[] line_item_sku_components { get; set; }
        public object order_line_item_sku_id { get; set; }
    }

    public class Reference_Numbers
    {
        public string id { get; set; }
        public string code { get; set; }
        public string value { get; set; }
        public string name { get; set; }
    }
}

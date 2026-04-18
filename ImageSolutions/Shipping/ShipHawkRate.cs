using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Shipping
{
    public class Rates
    {
        public Rate[] rates { get; set; }
    }
    public class Rate
    {
        public string id { get; set; }
        public string carrier { get; set; }
        public string carrier_code { get; set; }
        public string tariff_insurance_rule { get; set; }
        public string service_name { get; set; }
        public string service_code { get; set; }
        public string service_level { get; set; }
        public string standardized_service_name { get; set; }
        public string rate_display_name { get; set; }
        public string price { get; set; }
        public string currency_code { get; set; }
        public string est_delivery_date { get; set; }
        public string est_delivery_time { get; set; }
        public int service_days { get; set; }
        public string rates_provider { get; set; }
        public string rates_provider_code { get; set; }
        public string rates_provider_type { get; set; }
        public Carrier_Accounts carrier_account { get; set; }
        public object carrier_validated_destination_location_type { get; set; }
    }

    public class Carrier_Accounts
    {
        public string id { get; set; }
        public string name { get; set; }
        public string provider_code { get; set; }
        public string provider_name { get; set; }
        public bool _default { get; set; }
        public object integration_id { get; set; }
        public string warehouse_id { get; set; }
        public object[] packing_types { get; set; }
    }
}

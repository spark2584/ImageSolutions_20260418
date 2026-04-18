using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSuiteLibrary.Fulfillment
{
    public class FulfillmentFilter
    {
        public List<string> InternalIDs { get; set; }
        public string APIExternalID { get; set; }
        public bool? APIExternalIDEmpty { get; set; }
        public string FulfillmentPlanAPIExternalID { get; set; }
        public bool? FulfillmnetPlanAPIExternalIDEmpty { get; set; }
        public List<string> CustomerInternalIDs { get; set; }
        public string SalesOrderInternalID;
        public SalesOrder.NetSuiteTransactionStatus? Status { get; set; }

        public bool? MarkedShipped { get; set; }

        public string TranID { get; set; }
    }
}

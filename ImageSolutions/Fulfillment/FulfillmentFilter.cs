using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Fulfillment
{
    public class FulfillmentFilter
    {
        public Database.Filter.StringSearch.SearchFilter FulfillmentID { get; set; }
        public Database.Filter.StringSearch.SearchFilter InternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter TrackingNumber { get; set; }
        public Database.Filter.StringSearch.SearchFilter ExternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter SalesOrderID { get; set; }
        public Database.Filter.StringSearch.SearchFilter PurchaseOrderID { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter UpdatedOn { get; set; }
        public Database.Filter.StringSearch.SearchFilter IncrementID { get; set; }
        public bool? ShipConfirmationSent { get; set; }
    }
}

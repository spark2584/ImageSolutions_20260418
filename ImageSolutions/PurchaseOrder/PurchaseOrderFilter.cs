using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.PurchaseOrder
{
    public class PurchaseOrderFilter
    {
        public Database.Filter.StringSearch.SearchFilter PurchaseOrderID { get; set; }
        public Database.Filter.StringSearch.SearchFilter PurchaseOrderNumber { get; set; }

        public Database.Filter.StringSearch.SearchFilter InternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ExternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter VendorID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ShipToAddressID { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter UpdatedOn { get; set; }
    }
}

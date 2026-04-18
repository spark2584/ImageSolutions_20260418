using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSuiteLibrary.Invoice
{
    public class InvoiceFilter
    {
        public string APIExternalID { get; set; }
        public List<string> CustomerInternalIDs { get; set; }
        public string SalesOrderInternalID;
        public string PurchaseOrderInternalID;

        public string PONumber { get; set; }
        public NetSuiteLibrary.com.netsuite.webservices.SearchMultiSelectFieldOperator? SalesOrderInternalIDOperator;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using NetSuiteLibrary.com.netsuite.webservices;
using System.Net;

namespace NetSuiteLibrary.PurchaseOrder
{
    public class PurchaseOrderFilter
    {
        public Database.Filter.StringSearch.SearchFilter APIExternalID { get; set; }
        public bool? RequestFBAShipment { get; set; }
        public string Memo { get; set; }
        public List<string> InternalIDs { get; set; }

    }
}


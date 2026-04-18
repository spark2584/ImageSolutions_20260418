using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary.com.netsuite.webservices;

namespace NetSuiteLibrary.TransferOrder
{
    public class TransferOrderFilter
    {
        public Database.Filter.StringSearch.SearchFilter APIExternalID { get; set; }
        public bool? RequestFBAShipment { get; set; }
        public string Memo { get; set; }
    }
}

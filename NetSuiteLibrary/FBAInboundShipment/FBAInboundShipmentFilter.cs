using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary.com.netsuite.webservices;

namespace NetSuiteLibrary.FBAInboundShipment
{
    public class FBAInboundShipmentFilter
    {
        public Database.Filter.StringSearch.SearchFilter APIExternalID { get; set; }
        public bool? RequestFBAShipmentUpdate { get; set; }
        public bool? FBAShipmentUpdating { get; set; }
    }
}

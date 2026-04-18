using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary.com.netsuite.webservices;

namespace NetSuiteLibrary.Vendor
{
    public class VendorFilter
    {
        public string APIExternalID { get; set; }
        public List<string> VendorInternalIDs { get; set; }
        public SearchDateField LastModified { get; set; }
    }
}

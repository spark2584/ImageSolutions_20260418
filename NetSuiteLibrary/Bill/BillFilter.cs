using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSuiteLibrary.Bill
{
    public class BillFilter
    {
        public string APIExternalID { get; set; }
        public List<string> VendorInternalIDs { get; set; }
    }
}

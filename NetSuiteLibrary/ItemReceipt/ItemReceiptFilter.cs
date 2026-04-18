using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSuiteLibrary.ItemReceipt
{
    public class ItemReceiptFilter
    {
        public string APIExternalID { get; set; }
        public List<string> InternalIDs { get; set; }
        public SearchDateField LastModified { get; set; }
    }
}

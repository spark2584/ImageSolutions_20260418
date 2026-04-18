using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSuiteLibrary.Payment
{
    public class CustomerPaymentFilter
    {
        public string APIExternalID { get; set; }
        public List<string> CustomerInternalIDs { get; set; }
        public string InvoiceInternalID { get; set; }
        public SearchMultiSelectFieldOperator? InvoiceInternalIDOperator { get; set; }
    }
}

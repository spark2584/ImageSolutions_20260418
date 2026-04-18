using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSuiteLibrary.Refund
{
    public class CustomerRefundFilter
    {
        public string APIExternalID { get; set; }
        public List<string> CustomerInternalIDs { get; set; }
        public string CreditMemoInternalID { get; set; }
        public SearchMultiSelectFieldOperator? CreditMemoInternalIDOperator { get; set; }
    }
}

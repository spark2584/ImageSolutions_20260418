using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSuiteLibrary.CreditMemo
{
    public class CreditMemoFilter
    {
        public string APIExternalID { get; set; }
        public List<string> CustomerInternalIDs { get; set; }
        public string SalesOrderInternalID;
        public NetSuiteLibrary.com.netsuite.webservices.SearchMultiSelectFieldOperator? SalesOrderInternalIDOperator;
        public string DocumentNumber { get; set; }
    }
}

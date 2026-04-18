using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using NetSuiteLibrary.com.netsuite.webservices;
using System.Net;

namespace NetSuiteLibrary.SalesOrder
{
    public class SalesOrderFilter
    {
        public string APIExternalID { get; set; }
        public string Class { get; set; }
        public List<string> CustomerInternalIDs { get; set; }
        public SearchDateField LastModified { get; set; }
        public SearchDateField FulfillmentIssueOccurredOn { get; set; }
        public List<NetSuiteTransactionStatus> Status { get; set; }
        public List<string> InternalIDs { get; set; }
        public bool? FulfillmentCompleted { get; set; }
        public SearchDateField TransactionDate { get; set; }

        public string PONumber { get; set; }
        public string tranid { get; set; }
        public bool? ApproveSalesOrder { get; set; }


        //public SearchTextNumberFieldOperator? PONumberOperator;
        //public List<string> CustomerInternalIDs { get; set; }
        //public string PONumber { get; set; }
        //public List<string> NetSuiteInternalIDs { get; set; }
    }
}


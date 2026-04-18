using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary.com.netsuite.webservices;

namespace NetSuiteLibrary.SupportCase
{
    public class SupportCaseFilter
    {
        public string APIExternalID { get; set; }
        public List<string> NetSuiteInternalIDs { get; set; }
        public SearchDateField LastModified { get; set; }
    }
}

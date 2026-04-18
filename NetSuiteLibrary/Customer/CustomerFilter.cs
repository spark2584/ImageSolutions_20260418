using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary.com.netsuite.webservices;

namespace NetSuiteLibrary.Customer
{
    public class CustomerFilter
    {
        public string APIExternalID { get; set; }
        public string ExternalID { get; set; }
        public List<string> ParentInternalIDs { get; set; }
        public string StoreOrgNumber { get; set; }
        public string StoreEmpID { get; set; }
        public string Email { get; set; }
        public bool? IsPerson { get; set; }
        public List<string> CustomerInternalIDs { get; set; }
        public SearchDateField LastModified { get; set; }
        public List<string> Status { get; set; }
    }
}

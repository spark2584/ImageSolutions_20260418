using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary.com.netsuite.webservices;

namespace NetSuiteLibrary.Employee
{
    public class EmployeeFilter
    {
        public string APIExternalID { get; set; }
        public List<string> EmployeeInternalIDs { get; set; }
        public SearchDateField LastModified { get; set; }
        public bool? IsPM { get; set; }
    }
}

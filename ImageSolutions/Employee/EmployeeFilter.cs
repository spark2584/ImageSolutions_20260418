using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Employee
{
    public class EmployeeFilter
    {
        public bool? IsPM { get; set; }
        public bool? IsSalesRep { get; set; }
        public Database.Filter.StringSearch.SearchFilter ExternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter NetSuiteInternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ScanNumber { get; set; }
        public Database.Filter.StringSearch.SearchFilter Email { get; set; }
    }
}

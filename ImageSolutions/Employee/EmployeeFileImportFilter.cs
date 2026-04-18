using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Employee
{
    public class EmployeeFileImportFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter EmployeeID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Status { get; set; }
    }
}

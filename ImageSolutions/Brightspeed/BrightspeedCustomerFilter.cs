using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Brightspeed
{
    public class BrightspeedCustomerFilter
    {
        public Database.Filter.StringSearch.SearchFilter EmployeeID { get; set; }
        public Database.Filter.StringSearch.SearchFilter InternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Email { get; set; }
        public bool? IsUpdated { get; set; }
        public Database.Filter.StringSearch.SearchFilter SyncID { get; set; }
    }
}

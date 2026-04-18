using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Customer
{
    public class CustomerFilter
    {
        public Database.Filter.StringSearch.SearchFilter EmployeeID { get; set; }
        public Database.Filter.StringSearch.SearchFilter StoreNumber { get; set; }
        public Database.Filter.StringSearch.SearchFilter InternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ExternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ParentID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Email { get; set; }
        public bool? IsUpdated { get; set; }
        public bool? IsIndividual { get; set; }
    }
}

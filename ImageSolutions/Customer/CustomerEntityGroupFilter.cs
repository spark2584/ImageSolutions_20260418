using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Customer
{
    public class CustomerEntityGroupFilter
    {
        public Database.Filter.StringSearch.SearchFilter CustomerID { get; set; }
        public Database.Filter.StringSearch.SearchFilter EntityGroupID { get; set; }
        public bool? IsUpdated { get; set; }
    }
}

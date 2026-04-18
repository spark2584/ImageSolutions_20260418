using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSuiteLibrary.Entity
{
    public class EntityGroupFilter
    {
        public List<string> InternalIDs { get; set; }
        public Database.Filter.StringSearch.SearchFilter APIExternalID { get; set; }

    }
}

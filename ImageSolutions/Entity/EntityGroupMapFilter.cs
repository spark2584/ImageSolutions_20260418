using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Entity
{
    public class EntityGroupMapFilter
    {
        public Database.Filter.StringSearch.SearchFilter EntityGroupID { get; set; }
        public Database.Filter.StringSearch.SearchFilter GroupName { get; set; }
        public Database.Filter.StringSearch.SearchFilter Code { get; set; }
    }
}

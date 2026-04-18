using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Package
{
    public class PackageGroupFilter
    {
        public Database.Filter.StringSearch.SearchFilter PackageGroupID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Name { get; set; }
        public Database.Filter.StringSearch.SearchFilter InternalID { get; set; }
    }
}

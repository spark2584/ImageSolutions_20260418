using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Package
{
    public class PackageFilter
    {
        public Database.Filter.StringSearch.SearchFilter PackageID { get; set; }
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Name { get; set; }
        public bool? InActive { get; set; }
    }
}

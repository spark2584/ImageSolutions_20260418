using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Custom
{
    public class CustomFieldFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Location { get; set; }
        public Database.Filter.StringSearch.SearchFilter Name { get; set; }
        public bool? Inactive { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Website
{
    public class WebsiteTabFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ParentID { get; set; }
        public Database.Filter.StringSearch.SearchFilter TabName { get; set; }
        public bool? AllowAllGroups { get; set; }
        public int? Sort { get; set; }
    }
}

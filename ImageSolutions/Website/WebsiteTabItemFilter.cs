using ImageSolutions.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Website
{
    public class WebsiteTabItemFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteTabID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
        public int? Sort { get; set; }
        public List<string> mAttributeValues { get; set; }
        public bool? Inactive { get; set; }
        public bool? IsOnline { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Website
{
    public class WebsiteFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter InternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter GUID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Name { get; set; }
        public Database.Filter.StringSearch.SearchFilter Domain { get; set; }
        public Database.Filter.StringSearch.SearchFilter InActive { get; set; }
        public Database.Filter.StringSearch.SearchFilter BannerInternalID { get; set; }
        public bool? EnableSMSOptIn { get; set; }
        public bool? EnableEmailOptIn { get; set; }
    }
}

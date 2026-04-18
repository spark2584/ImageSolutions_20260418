using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Website
{
    public class WebsiteCountryFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public bool? Exclude { get; set; }
    }
}

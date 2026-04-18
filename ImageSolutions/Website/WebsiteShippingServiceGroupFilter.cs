using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Website
{
    public class WebsiteShippingServiceGroupFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteShippingServiceID { get; set; }
        public Database.Filter.StringSearch.SearchFilter WebsiteGroupID { get; set; }
    }
}

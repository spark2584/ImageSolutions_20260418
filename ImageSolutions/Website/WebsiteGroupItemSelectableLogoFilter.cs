using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Website
{
    public class WebsiteGroupItemSelectableLogoFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter WebsiteGroupID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemSelectableLogoID { get; set; }
    }
}

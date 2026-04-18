using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Customization
{
    public class CustomizationFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
        public Database.Filter.StringSearch.SearchFilter WebsiteTabID { get; set; }
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
    }
}

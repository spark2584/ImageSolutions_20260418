using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Item
{
    public class ItemPricingFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
        public Database.Filter.StringSearch.SearchFilter WebsiteGroupID { get; set; }
    }
}

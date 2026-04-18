using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Item
{
    public class ItemPersonalizationValueApprovedFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemPersonalizationID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemPersonalizationName { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemPersonalizationApprovedValue { get; set; }

    }
}

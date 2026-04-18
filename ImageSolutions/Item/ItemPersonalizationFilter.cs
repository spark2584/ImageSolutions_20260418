using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Item
{
    public class ItemPersonalizationFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
        public int? Sort { get; set; }
    }
}

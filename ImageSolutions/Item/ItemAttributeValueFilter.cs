using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Item
{
    public class ItemAttributeValueFilter
    {
        public Database.Filter.StringSearch.SearchFilter AttributeValueID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
    }
}

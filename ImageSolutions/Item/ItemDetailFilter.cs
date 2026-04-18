using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Item
{
    public class ItemDetailFilter
    {
        public Database.Filter.StringSearch.SearchFilter Attribute { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
    }
}

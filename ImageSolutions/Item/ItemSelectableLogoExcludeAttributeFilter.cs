using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Item
{
    public class ItemSelectableLogoExcludeAttributeFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemSelectableLogoID { get; set; }
        public Database.Filter.StringSearch.SearchFilter AttributeValueID { get; set; }
    }
}

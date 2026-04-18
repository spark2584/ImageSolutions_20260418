using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Custom
{
    public class CustomListValueFilter
    {
        public Database.Filter.StringSearch.SearchFilter CustomListID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ListValue { get; set; }
        public int? Sort { get; set; }
    }
}

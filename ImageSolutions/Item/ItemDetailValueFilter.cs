using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Item
{
    public class ItemDetailValueFilter
    {
        public Database.Filter.StringSearch.SearchFilter Value { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemDetailID { get; set; }
    }
}

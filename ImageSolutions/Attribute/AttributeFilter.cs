using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Attribute
{
    public class AttributeFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
        public Database.Filter.StringSearch.SearchFilter AttributeName { get; set; }
    }
}

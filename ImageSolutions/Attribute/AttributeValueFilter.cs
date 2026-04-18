using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Attribute
{
    public class AttributeValueFilter
    {
        public Database.Filter.StringSearch.SearchFilter AttributeID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Value { get; set; }
        public Database.Filter.StringSearch.SearchFilter BackgroundColor { get; set; }
        public bool? IsDefault { get; set; }
        public int? Sort { get; set; }
    }
}

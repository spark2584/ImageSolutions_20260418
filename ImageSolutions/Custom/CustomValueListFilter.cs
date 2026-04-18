using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Custom
{
    public class CustomValueListFilter
    {
        public Database.Filter.StringSearch.SearchFilter CustomFieldID { get; set; }
        public Database.Filter.StringSearch.SearchFilter FilterAccountID { get; set; }
        public bool? Inactive { get; set; }
    }
}

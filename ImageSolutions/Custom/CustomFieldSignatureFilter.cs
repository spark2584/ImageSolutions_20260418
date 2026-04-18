using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Custom
{
    public class CustomFieldSignatureFilter
    {
        public Database.Filter.StringSearch.SearchFilter CustomFieldID { get; set; }
        public Database.Filter.StringSearch.SearchFilter UserWebsiteID { get; set; }
    }
}

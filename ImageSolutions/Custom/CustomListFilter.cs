using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Custom
{
    public class CustomListFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ListName { get; set; }
        public int? Sort { get; set; }
    }
}

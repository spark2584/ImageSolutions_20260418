using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.MavisTire
{
    public class MavisTireCustomerImportFilter
    {
        public Database.Filter.StringSearch.SearchFilter MavisTireCustomerImportID { get; set; }
        public Database.Filter.StringSearch.SearchFilter FileName { get; set; }
        public bool? IsProcessed { get; set; }
        public bool? IsEncrypted { get; set; }
    }
}

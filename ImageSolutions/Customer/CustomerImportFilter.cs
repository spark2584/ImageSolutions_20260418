using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Customer
{
    public class CustomerImportFilter
    {
        public Database.Filter.StringSearch.SearchFilter FileName { get; set; }
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public bool? IsProcessed { get; set; }
        public bool? IsStore { get; set; }
        public bool? IsEncrypted { get; set; }
    }
}

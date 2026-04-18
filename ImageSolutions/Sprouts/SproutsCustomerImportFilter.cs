using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Sprouts
{
    public class SproutsCustomerImportFilter
    {
        public Database.Filter.StringSearch.SearchFilter SproutsCustomerImportID { get; set; }
        public Database.Filter.StringSearch.SearchFilter FileName { get; set; }
        public bool? IsProcessed { get; set; }
        public bool? IsEncrypted { get; set; }
    }
}

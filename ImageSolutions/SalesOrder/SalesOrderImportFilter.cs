using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.SalesOrder
{
    public class SalesOrderImportFilter
    {
        public Database.Filter.StringSearch.SearchFilter FileName { get; set; }
        public bool? IsProcessed { get; set; }
    }
}

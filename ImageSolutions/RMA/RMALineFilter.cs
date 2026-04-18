using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.RMA
{
    public class RMALineFilter
    {
        public Database.Filter.StringSearch.SearchFilter RMAID;
        public Database.Filter.StringSearch.SearchFilter SalesOrderLineID;
        public Database.Filter.StringSearch.SearchFilter ItemID;
    }
}

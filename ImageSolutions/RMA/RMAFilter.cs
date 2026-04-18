using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.RMA
{
    public  class RMAFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter SalesOrderID { get; set; }
        public Database.Filter.StringSearch.SearchFilter InternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter DocumentNumber { get; set; }
        public Database.Filter.StringSearch.SearchFilter ErrorMessage { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter UpdatedOn { get; set; }
    }
}

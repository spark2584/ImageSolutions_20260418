using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Enterprise
{
    public class EnterpriseInvoiceFilter
    {
        public Database.Filter.StringSearch.SearchFilter InvoiceInternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter SalesOrderInternalID { get; set; }
        public bool? IsExported { get; set; }
        public bool? IsNSUpdated { get; set; }
    }
}

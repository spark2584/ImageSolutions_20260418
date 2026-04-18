using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Payment
{
    public class PaymentTermAdjustmentFilter
    {
        public Database.Filter.StringSearch.SearchFilter UserWebsiteID { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter TransactionDate { get; set; }
    }
}

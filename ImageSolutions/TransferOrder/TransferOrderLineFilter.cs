using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.TransferOrder
{
    public class TransferOrderLineFilter
    {
        public string TransferOrderID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
        public string RetailerSKU { get; set; }
        public Database.Filter.StringSearch.SearchFilter NetSuiteLineID { get; set; }
        public Database.Filter.StringSearch.SearchFilter TransferOrder_NetSuiteInternalID { get; set; }
    }
}

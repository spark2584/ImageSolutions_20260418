using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.ItemReceipt
{
    public class ItemReceiptLineFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemReceiptID { get; set; }
        public Database.Filter.StringSearch.SearchFilter RMALineID { get; set; }
        public Database.Filter.StringSearch.SearchFilter NetSuiteLineID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }

    }
}

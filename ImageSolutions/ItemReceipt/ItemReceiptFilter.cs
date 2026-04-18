using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.ItemReceipt
{
    public class ItemReceiptFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemReceiptID { get; set; }
        public Database.Filter.StringSearch.SearchFilter InternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter DocumentNumber { get; set; }
        public Database.Filter.StringSearch.SearchFilter RMAID { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter UpdatedOn { get; set; }

    }
}

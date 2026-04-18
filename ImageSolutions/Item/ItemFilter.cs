using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Item
{
    public class ItemFilter
    {
        public Database.Filter.StringSearch.SearchFilter ParentID { get; set; }
        public Database.Filter.StringSearch.SearchFilter InternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ExternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter StyleID { get; set; }
        public Database.Filter.StringSearch.SearchFilter SizeCode { get; set; }
        public Database.Filter.StringSearch.SearchFilter Color { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemNumber { get; set; }
        public bool? IsNonInventory { get; set; }
        public Database.Filter.StringSearch.SearchFilter VendorName { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter UpdatedOn { get; set; }
        public bool? InActive { get; set; }
        public bool? IsOnline { get; set; }

    }
}

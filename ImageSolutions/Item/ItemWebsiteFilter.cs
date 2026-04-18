using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Item
{
    public class ItemWebsiteFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemWebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemNumber { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemName { get; set; }
        public Database.Filter.StringSearch.SearchFilter SalesDescription { get; set; }
        public Database.Filter.StringSearch.SearchFilter ParentID { get; set; }
        public Database.Filter.StringSearch.SearchFilter InternalID { get; set; }
        public bool? InActive { get; set; }
        public bool? IsOnline { get; set; }
        public bool? IsNonInventory { get; set; }
    }
}

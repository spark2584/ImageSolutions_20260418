using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.ShoppingCart
{
    public class ShoppingCartLineFilter
    {
        public Database.Filter.StringSearch.SearchFilter ShoppingCartLineID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ShoppingCartID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ItemID { get; set; }
        public Database.Filter.StringSearch.SearchFilter UserInfoID { get; set; }
        public Database.Filter.StringSearch.SearchFilter CustomListID_1 { get; set; }
        public Database.Filter.StringSearch.SearchFilter CustomListValueID_1 { get; set; }
        public Database.Filter.StringSearch.SearchFilter CustomListID_2 { get; set; }
        public Database.Filter.StringSearch.SearchFilter CustomListValueID_2 { get; set; }
        public Database.Filter.StringSearch.SearchFilter CustomDesignImagePath { get; set; }
        public Database.Filter.StringSearch.SearchFilter CustomDesignName { get; set; }
    }
}

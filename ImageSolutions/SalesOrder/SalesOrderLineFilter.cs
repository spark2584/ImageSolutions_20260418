using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.SalesOrder
{
    public class SalesOrderLineFilter
    {
        public Database.Filter.StringSearch.SearchFilter SalesOrderID;
        public Database.Filter.StringSearch.SearchFilter ExternalID;
        public Database.Filter.StringSearch.SearchFilter ParentLineExternalID;
        public Database.Filter.StringSearch.SearchFilter ItemID;

        public Database.Filter.StringSearch.SearchFilter ParentSalesOrderLineID;
        public Database.Filter.StringSearch.SearchFilter SalesOrderStatus;
        public Database.Filter.StringSearch.SearchFilter VendorID;

        public Database.Filter.StringSearch.SearchFilter UserInfoID { get; set; }
        public Database.Filter.StringSearch.SearchFilter CustomListID_1 { get; set; }
        public Database.Filter.StringSearch.SearchFilter CustomListValueID_1 { get; set; }
        public Database.Filter.StringSearch.SearchFilter CustomListID_2 { get; set; }
        public Database.Filter.StringSearch.SearchFilter CustomListValueID_2 { get; set; }
        public Database.Filter.StringSearch.SearchFilter CustomDesignImagePath { get; set; }
        public Database.Filter.StringSearch.SearchFilter CustomDesignName { get; set; }
    }
}

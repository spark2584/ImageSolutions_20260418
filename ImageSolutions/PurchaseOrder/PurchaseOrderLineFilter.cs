using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.PurchaseOrder
{
    public class PurchaseOrderLineFilter
    {
        public Database.Filter.StringSearch.SearchFilter PurchaseOrderLineID;
        public Database.Filter.StringSearch.SearchFilter PurchaseOrderID;
        public Database.Filter.StringSearch.SearchFilter ItemID;
        public Database.Filter.StringSearch.SearchFilter SalesOrderLineID;
    }
}

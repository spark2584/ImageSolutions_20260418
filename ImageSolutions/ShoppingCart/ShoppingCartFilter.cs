using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.ShoppingCart
{
    public class ShoppingCartFilter
    {
        public Database.Filter.StringSearch.SearchFilter ShoppingCartID { get; set; }
        public Database.Filter.StringSearch.SearchFilter UserWebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter SalesOrderID { get; set; }
        public Database.Filter.StringSearch.SearchFilter PunchoutSessionID { get; set; }
    }
}

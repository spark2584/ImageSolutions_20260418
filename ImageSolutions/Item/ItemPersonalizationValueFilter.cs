using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Item
{
    public class ItemPersonalizationValueFilter
    {
        public Database.Filter.StringSearch.SearchFilter ItemPersonalizationID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ShoppingCartLineID { get; set; }
        public Database.Filter.StringSearch.SearchFilter SalesOrderLineID { get; set; }
    }
}

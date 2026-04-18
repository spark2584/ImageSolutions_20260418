using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.SalesOrder
{
    public class SalesOrderLineSelectableLogoFilter
    {
        public Database.Filter.StringSearch.SearchFilter SalesOrderLineID { get; set; }
        public Database.Filter.StringSearch.SearchFilter SelectableLogoID { get; set; }
    }
}

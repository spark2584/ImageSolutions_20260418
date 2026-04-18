using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.ShoppingCart
{
    public class ShoppingCartLineSelectableLogoFilter
    {
        public Database.Filter.StringSearch.SearchFilter ShoppingCartLineID { get; set; }
        public Database.Filter.StringSearch.SearchFilter SelectableLogoID { get; set; }
    }
}

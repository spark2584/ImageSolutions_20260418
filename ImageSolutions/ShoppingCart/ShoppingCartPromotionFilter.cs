using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.ShoppingCart
{
    public class ShoppingCartPromotionFilter
    {
        public Database.Filter.StringSearch.SearchFilter ShoppingCartID { get; set; }
        public Database.Filter.StringSearch.SearchFilter PromotionID { get; set; }
    }
}

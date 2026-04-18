using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Category
{
    public class CategoryFilter
    {
        public Database.Filter.StringSearch.SearchFilter CategoryID { get; set; }
        public Database.Filter.StringSearch.SearchFilter CategoryName { get; set; }
    }
}

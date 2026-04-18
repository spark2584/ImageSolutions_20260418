using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Group
{
    public class GroupCategoryFilter
    {
        public Database.Filter.StringSearch.SearchFilter GroupCategoryID { get; set; }
        public Database.Filter.StringSearch.SearchFilter GroupID { get; set; }
        public Database.Filter.StringSearch.SearchFilter CategoryID { get; set; }
    }
}

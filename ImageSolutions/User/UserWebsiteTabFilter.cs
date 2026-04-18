using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.User
{
    public class UserWebsiteTabFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteTabID { get; set; }
        public Database.Filter.StringSearch.SearchFilter UserWebsiteID { get; set; }
    }
}

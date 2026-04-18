using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.User
{
    public class UserAccountFilter
    {
        public Database.Filter.StringSearch.SearchFilter UserAccountID { get; set; }
        public Database.Filter.StringSearch.SearchFilter UserInfoID { get; set; }
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter AccountID { get; set; }
        public Database.Filter.StringSearch.SearchFilter UserWebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter WebsiteGroupID { get; set; }
    }
}

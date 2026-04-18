using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.User
{
    public class UserInfoFilter
    {
        public Database.Filter.StringSearch.SearchFilter UserInfoID { get; set; }
        public Database.Filter.StringSearch.SearchFilter EmailAddress { get; set; }
        public Database.Filter.StringSearch.SearchFilter UserName { get; set; }
        public Database.Filter.StringSearch.SearchFilter GUID { get; set; }
        public Database.Filter.StringSearch.SearchFilter PasswordResetGUID { get; set; }
        public Database.Filter.StringSearch.SearchFilter UserPoolUserName { get; set; }
        public Database.Filter.StringSearch.SearchFilter CustomerInternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ExternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Password { get; set; }
        public bool? EmailWhiteListed { get; set; }
        public bool? IsGuest { get; set; }
    }
}

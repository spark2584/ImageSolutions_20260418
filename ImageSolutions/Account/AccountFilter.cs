using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Account
{
    public class AccountFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ParentID { get; set; }
        public Database.Filter.StringSearch.SearchFilter AccountName { get; set; }
        public string AccountNameExact { get; set; }
        public Database.Filter.StringSearch.SearchFilter RegistrationKey { get; set; }
        public Database.Filter.StringSearch.SearchFilter EmailAddress { get; set; }
        public Database.Filter.StringSearch.SearchFilter StoreNumber { get; set; }
        public bool? IsPendingApproval { get; set; }
        public List<string> AccountIDs { get; set; }
    }
}

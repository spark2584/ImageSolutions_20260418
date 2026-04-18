using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Account
{
    public class AccountOrderApprovalFilter
    {
        public Database.Filter.StringSearch.SearchFilter UserInfoID { get; set; }
        public Database.Filter.StringSearch.SearchFilter AccountID { get; set; }
    }
}

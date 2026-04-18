using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.User
{
    public class UserCreditCardFilter
    {
        public Database.Filter.StringSearch.SearchFilter UserInfoID { get; set; }
        public Database.Filter.StringSearch.SearchFilter CreditCardID { get; set; }
    }
}

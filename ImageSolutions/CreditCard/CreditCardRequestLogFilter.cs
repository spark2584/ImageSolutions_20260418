using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.CreditCard
{
    public class CreditCardRequestLogFilter
    {
        public Database.Filter.StringSearch.SearchFilter LastFourDigit { get; set; }
        public Database.Filter.StringSearch.SearchFilter PayerExternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter CardExternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter FullName { get; set; }
        public decimal? Amount { get; set; }
        public Database.Filter.StringSearch.SearchFilter IPAddress { get; set; }
    }
}

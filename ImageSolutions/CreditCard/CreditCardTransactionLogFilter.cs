using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.CreditCard
{
    public class CreditCardTransactionLogFilter
    {
        public decimal? Amount { get; set; }
        public string UserInfoID { get; set; }
        public string CCName { get; set; }
        public string CCLastFourNumber { get; set; }
        public Database.Filter.StringSearch.SearchFilter TransactionID { get; set; }
        public CreditCard.enumCreditCardType? CCType { get; set; }
        public DateTime? CreatedOnFrom { get; set; }
        public DateTime? CreatedOnTo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Paystand
{
    public class Payment
    {
        public string id { get; set; }        
        public string amount { get; set; }
        public string currency { get; set; }
        public string settlementAmount { get; set; }
        public string settlementCurrency { get; set; }
        public string sourceId { get; set; }
        public string sourceType { get; set; }
        public string datePosted { get; set; }
        public string datePaid { get; set; }
        public string dateFailed { get; set; }
        public string assuretyPosted { get; set; }
        public string status { get; set; }
        public string ownerId { get; set; }
        public string created { get; set; }
        public string lastUpdated { get; set; }
        public string payerId { get; set; }
        public Payer payer { get; set; }
        public Source source { get; set; }
        public Fees fees { get; set; }
        public List<Hold> holds { get; set; }
        public string accountKey { get; set; }
        public string accountId { get; set; }
        public string balanceChangeId { get; set; }
        public string meta { get; set; }
        public string description { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Paystand
{
    public class PaymentRequest
    {
        public string amount { get; set; }
        public string currency { get; set; }
        public string cardId { get; set; }
        public CardRequest card { get; set; }
        //public string bankId { get; set; }
        //public Bank bank { get; set; }
        public string payerId { get; set; }
        //public Payer payer { get; set; }
        //public string accountKey { get; set; }
        //public string meta { get; set; }
        public PersonalContact personalContact { get; set; }
        public string description { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Paystand
{
    public class Bank
    {
        public string nameOnAccount { get; set; }
        public string accountHolderType { get; set; }
        public string accountNumber { get; set; }
        public string routingNumber { get; set; }
        public string accountType { get; set; } //checking, savings
        public string country { get; set; } //USA
        public string currency { get; set; } //USD
    }
}

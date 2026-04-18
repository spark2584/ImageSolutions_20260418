using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Paystand
{
    public class CardRequest
    {
        public string nameOnCard { get; set; }
        public string cardNumber { get; set; }
        public string expirationMonth { get; set; }
        public string expirationYear { get; set; }
        public string securityCode { get; set; }
        public AddressRequest billingAddress { get; set; }
    }
}

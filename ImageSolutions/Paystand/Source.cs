using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Paystand
{
    public class Source
    {
        public string id { get; set; }
        public string nameOnCard { get; set; }
        public string brand { get; set; }
        public string last4 { get; set; }
        public string expirationMonth { get; set; }
        public string expirationYear { get; set; }
        public string fingerprint { get; set; }
        public string payerId { get; set; }
        public Address billingAddress { get; set; }
        public string created { get; set; }
        public string lastUpdated { get; set; }
        public string status { get; set; }

    }
}

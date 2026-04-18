using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Paystand
{
    public class PayerRequest
    {
        public string name { get; set; }
        public string email { get; set; }
        public AddressRequest address { get; set; }
    }
}

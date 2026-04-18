using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Paystand
{
    public class Payer
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public Address address { get; set; }
        public string ownerId { get; set; }
        public string status { get; set; }
        public DateTime created { get; set; }
        public DateTime lastUpdated { get; set; }
    }
}

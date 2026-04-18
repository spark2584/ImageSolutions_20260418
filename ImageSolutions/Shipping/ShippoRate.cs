using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Shipping
{
    public class ShippoRate
    {
        public string RateResponseID { get; set; }
        public string Carrier { get; set; }
        public decimal Amount { get; set; }
        public string ServiceName { get; set; }
        public string ServiceTemplate { get; set; }
        public string Token { get; set; }
        public string label { get; set; }
        public string QuoteID { get; set; }
        public string Message { get; set; }
        public string QuoteResponse { get; set; }
        public int PackageDimensionID { get; set; }
       
    }
}

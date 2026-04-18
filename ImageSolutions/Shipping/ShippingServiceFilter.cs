using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Shipping
{
    public class ShippingServiceFilter
    {
        public Database.Filter.StringSearch.SearchFilter Carrier { get; set; }
        public Database.Filter.StringSearch.SearchFilter ServiceCode { get; set; }
        public Database.Filter.StringSearch.SearchFilter ServiceName { get; set; }
        public Database.Filter.StringSearch.SearchFilter Description { get; set; }
    }
}

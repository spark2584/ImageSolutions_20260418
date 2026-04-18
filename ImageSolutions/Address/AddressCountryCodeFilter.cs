using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Address
{
    public class AddressCountryCodeFilter
    {
        public Database.Filter.StringSearch.SearchFilter Alpha2Code { get; set; }
        public Database.Filter.StringSearch.SearchFilter Alpha3Code { get; set; }
        public Database.Filter.StringSearch.SearchFilter Numeric { get; set; }
    }
}

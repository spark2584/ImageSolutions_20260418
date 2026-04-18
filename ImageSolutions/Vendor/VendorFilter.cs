using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Vendor
{
    public class VendorFilter
    {
        public Database.Filter.StringSearch.SearchFilter VendorID { get; set; }
        public Database.Filter.StringSearch.SearchFilter InternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter NetSuiteEntityID { get; set; }
        public Database.Filter.StringSearch.SearchFilter NetSuiteLeadInternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter NetSuiteLeadEntityID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ExternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter MerchantID { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter UpdatedOn { get; set; }
        public Database.Filter.StringSearch.SearchFilter MemberID { get; set; }
    }
}

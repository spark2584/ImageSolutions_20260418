using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Marketing
{
    public class SMSOutboxFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter MarketingTemplateID { get; set; }
        public Database.Filter.StringSearch.SearchFilter UserWebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Message { get; set; }
        public Database.Filter.StringSearch.SearchFilter SMSMobileNumber { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsSent { get; set; }
        public Database.Filter.StringSearch.SearchFilter ErrorMessage { get; set; }
    }
}

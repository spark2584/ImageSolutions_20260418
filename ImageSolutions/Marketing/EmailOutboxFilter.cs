using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Marketing
{
    public class EmailOutboxFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter MarketingTemplateID { get; set; }
        public Database.Filter.StringSearch.SearchFilter UserWebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Subject { get; set; }
        public Database.Filter.StringSearch.SearchFilter HTMLContent { get; set; }
        public Database.Filter.StringSearch.SearchFilter ToEmail { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsEmailed { get; set; }
        public Database.Filter.StringSearch.SearchFilter ErrorMessage { get; set; }
    }
}

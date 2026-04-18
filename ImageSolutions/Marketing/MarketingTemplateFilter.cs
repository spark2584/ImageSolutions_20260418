using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ImageSolutions.Marketing.MarketingTemplate;

namespace ImageSolutions.Marketing
{
    public class MarketingTemplateFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter EmailSubject { get; set; }
        public Database.Filter.StringSearch.SearchFilter EmailContent { get; set; }
        public Database.Filter.StringSearch.SearchFilter SMSContent { get; set; }
        public List<enumMarketingCampaign> MarketingCampaigns { get; set; }
        public bool? IsEnabled { get; set; }
    }
}

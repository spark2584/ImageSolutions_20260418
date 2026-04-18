using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Website
{
    public class WebsiteMessageFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Subject { get; set; }
        public Database.Filter.StringSearch.SearchFilter Message { get; set; }
        public bool? IsAnnouncment { get; set; }
        public bool? IsNotification { get; set; }
    }
}

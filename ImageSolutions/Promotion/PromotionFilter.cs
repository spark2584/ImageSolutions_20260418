using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSolutions.Promotion
{
    [Serializable]
    public class PromotionFilter
    {
        public string WebsiteID { get; set; }
        public string PromotionCode { get; set; }
        public string PromotionName { get; set; }
        //public string CustomerEmail { get; set; }
        //public string CustomerFirstName { get; set; }
        //public string CustomerLastName { get; set; }

        public Database.Filter.DateTimeSearch.SearchFilter FromDate { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter ToDate { get; set; }
        public bool? IsActive { get; set; }

    }
}

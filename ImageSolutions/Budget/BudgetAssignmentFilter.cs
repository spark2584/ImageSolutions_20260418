using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Budget
{
    public class BudgetAssignmentFilter
    {
        public Database.Filter.StringSearch.SearchFilter BudgetAssignmentID { get; set; }
        //public Database.Filter.StringSearch.SearchFilter UserInfoID { get; set; }
        public Database.Filter.StringSearch.SearchFilter UserWebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter WebsiteGroupID { get; set; }
        public List<string> WebsiteGroupIDs { get; set; }
        public Database.Filter.StringSearch.SearchFilter BudgetID { get; set; }
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public bool? InActive { get; set; }

    }
}

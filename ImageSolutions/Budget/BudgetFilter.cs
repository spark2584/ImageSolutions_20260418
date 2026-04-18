using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Budget
{
    public class BudgetFilter
    {
        public Database.Filter.StringSearch.SearchFilter BudgetID { get; set; }
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter BudgetName { get; set; }
        public Database.Filter.StringSearch.SearchFilter Division { get; set; }
        public Database.Filter.StringSearch.SearchFilter Email { get; set; }
        public Database.Filter.StringSearch.SearchFilter ApproverUserWebsiteID { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter StartDate { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter EndDate { get; set; }
    }
}

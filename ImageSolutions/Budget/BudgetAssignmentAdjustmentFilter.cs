using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Budget
{
    public class BudgetAssignmentAdjustmentFilter
    {
        public Database.Filter.StringSearch.SearchFilter BudgetAssignmentID { get; set; }
        public Database.Filter.StringSearch.SearchFilter BudgetID { get; set; }
    }
}

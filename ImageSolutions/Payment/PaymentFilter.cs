using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Payment
{
    public class PaymentFilter
    {
        public Database.Filter.StringSearch.SearchFilter SalesOrderID { get; set; }
        public Database.Filter.StringSearch.SearchFilter BudgetAssignmentID { get; set; }
        public Database.Filter.StringSearch.SearchFilter PromotionID { get; set; }
        public Database.Filter.StringSearch.SearchFilter UserInfoID { get; set; }
        public Database.Filter.StringSearch.SearchFilter NetSuiteInternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter PaymentSource { get; set; }
        public Database.Filter.StringSearch.SearchFilter SalesOrderNetSuiteInternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter CreditCardTransactionLogID { get; set; }

        public Database.Filter.StringSearch.SearchFilter PaymentTermID { get; set; }
        public Database.Filter.StringSearch.SearchFilter UserWebsiteID { get; set; }

        public Database.Filter.DateTimeSearch.SearchFilter CreatedOn { get; set; }
    }
}

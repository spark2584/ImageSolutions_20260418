using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.SalesOrder
{
    public  class SalesOrderFilter
    {
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter AccountID { get; set; }
        public Database.Filter.StringSearch.SearchFilter SalesOrderID { get; set; }
        public Database.Filter.StringSearch.SearchFilter NetSuiteInternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter SalesOrderNumber { get; set; }
        public Database.Filter.StringSearch.SearchFilter ExternalID { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter UpdatedOn { get; set; }
        public Database.Filter.StringSearch.SearchFilter IncrementID { get; set; }
        public Database.Filter.StringSearch.SearchFilter CustomerID { get; set; }
        public Database.Filter.StringSearch.SearchFilter BudgetID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ErrorMessage { get; set; }
        public Database.Filter.StringSearch.SearchFilter UserInfoID { get; set; }
        public bool? IsPendingApproval { get; set; }
        public bool? IsPendingItemPersonalizationApproval { get; set; }
        public bool? IsClosed { get; set; }
        public bool? ApprovalNotificationSent { get; set; }
        public bool? OrderConfirmationSent { get; set; }

    }
}

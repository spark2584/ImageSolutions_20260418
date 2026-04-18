using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.User
{
    public class UserWebsiteFilter
    {
        public Database.Filter.StringSearch.SearchFilter UserWebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter GUID { get; set; }
        public Database.Filter.StringSearch.SearchFilter UserInfoID { get; set; }
        public Database.Filter.StringSearch.SearchFilter WebsiteID { get; set; }
        public Database.Filter.StringSearch.SearchFilter CustomerInternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter FirstName { get; set; }
        public Database.Filter.StringSearch.SearchFilter LastName { get; set; }
        public Database.Filter.StringSearch.SearchFilter EmailAddress { get; set; }
        public Database.Filter.StringSearch.SearchFilter EmployeeID { get; set; }
        public Database.Filter.StringSearch.SearchFilter PunchoutGUID { get; set; }
        public Database.Filter.StringSearch.SearchFilter PunchoutSessionID { get; set; }
        public List<string> AccountIDs { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsPendingApproval { get; set; }
        public bool? WebsiteManagement { get; set; }
        public bool? StoreManagement { get; set; }
        public bool? UserManagement { get; set; }
        public bool? GroupManagement { get; set; }
        public bool? TabManagement { get; set; }
        public bool? ItemManagement { get; set; }
        public bool? BudgetManagement { get; set; }
        public bool? IsBudgetAdmin { get; set; }
        public bool? OrderManagement { get; set; }
        public bool? CreditCardManagement { get; set; }
        public bool? ShippingManagement { get; set; }
        public bool? MessageManagement { get; set; }
        public bool? OptInForNotification { get; set; }
        public bool? SendWelcomeEmail { get; set; }
        public bool? Inactive { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter HireDate { get; set; }
        public Database.Filter.StringSearch.SearchFilter SMSMobileNumber { get; set; }
        //public bool? SMSOptIn { get; set; }
        //public bool? MarketingWelcomeSent { get; set; }
        public bool? EnableSMSOptIn { get; set; }
        public bool? EnableEmailOptIn { get; set; }
        public bool? MarketingWelcome { get; set; }
        public bool? MarketingOutreach { get; set; }
        public bool? MarketingCartAbandonment { get; set; }
        public bool? MarketingBudgetExpiration { get; set; }
        public bool? MarketingBudgetRenewal { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter MarketingWelcomeSentOn { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter MarketingOutreachSentOn { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter MarketingCartAbandonmentSentOn { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter MarketingBudgetExpirationSentOn { get; set; }
        public Database.Filter.DateTimeSearch.SearchFilter MarketingBudgetRenewalSentOn { get; set; }
    }
}

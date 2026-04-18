using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Control
{
    public partial class MyAccountNavigation : BaseControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            liNotificationSettings.Visible = ThisPage.CurrentWebsite.EnableSMSOptIn || ThisPage.CurrentUser.CurrentUserWebSite.EnableSMSOptIn;

            if (!Page.IsPostBack)
            {
                string strURL = Request.Url.ToString();

                if (strURL.ToLower().Contains("/myaccount/dashboard.aspx"))
                {
                    aDashboard.Attributes.Remove("class");
                    aDashboard.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/myaccount/addressbook.aspx"))
                {
                    aAddressBook.Attributes.Remove("class");
                    aAddressBook.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/myaccount/orders.aspx"))
                {
                    aMyOrders.Attributes.Remove("class");
                    aMyOrders.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/myaccount/budgets.aspx"))
                {
                    aBudgets.Attributes.Remove("class");
                    aBudgets.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/myaccount/creditcard.aspx"))
                {
                    aCreditCard.Attributes.Remove("class");
                    aCreditCard.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/myaccount/profile.aspx"))
                {
                    aProfile.Attributes.Remove("class");
                    aProfile.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/myaccount/notificationsettings.aspx"))
                {
                    aNotificationSettings.Attributes.Remove("class");
                    aNotificationSettings.Attributes.Add("class", "nav-link active");
                }

                lblFullName.Text = string.Format("{0} {1}", ThisPage.CurrentUser.FirstName, ThisPage.CurrentUser.LastName);
                lblEmail.Text = string.Format("{0}", string.IsNullOrEmpty(ThisPage.CurrentUser.UserName) ? ThisPage.CurrentUser.EmailAddress : ThisPage.CurrentUser.UserName);

                this.aBudgets.Visible = ThisPage.CurrentWebsite.EnableEmployeeCredit;

                if (!string.IsNullOrEmpty(ThisPage.CurrentWebsite.BudgetAlias))
                {
                    aBudgets.InnerText = string.Format("My {0}", ThisPage.CurrentUser.CurrentUserWebSite.WebSite.BudgetAlias);
                }

                liCreditCard.Visible = ThisPage.CurrentWebsite.EnableCreditCard;

                if (ThisPage.CurrentUser.CurrentUserWebSite != null)
                {
                    liOrderReport.Visible = ThisPage.CurrentUser.CurrentUserWebSite.IsAdmin && ThisPage.CurrentUser.CurrentUserWebSite.OrderManagement && !ThisPage.CurrentWebsite.HideMyAccountOrderReport;
                    liOrderApproval.Visible = ThisPage.CurrentUser.CurrentUserWebSite.IsAdmin && ThisPage.CurrentUser.CurrentUserWebSite.OrderManagement && !ThisPage.CurrentWebsite.HideMyAccountOrderApproval;
                    aAddressBook.Visible = string.IsNullOrEmpty(ThisPage.CurrentUser.CurrentUserWebSite.AddressPermission) || ThisPage.CurrentUser.CurrentUserWebSite.UserManagement || ThisPage.CurrentUser.IsSuperAdmin;
                }

                if (ThisPage.CurrentWebsite.WebsiteID == "7" || ThisPage.CurrentWebsite.WebsiteID == "8")
                {
                    aBudgets.InnerText = "My Store Budget";
                }
            }
        }
    }
}
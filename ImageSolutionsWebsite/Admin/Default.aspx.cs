using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class Default : BasePageAdminUserWebSiteAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentUser.CurrentUserWebSite.WebsiteManagement)
                Response.Redirect("/Admin/WebsiteOverview.aspx");
            else if (CurrentUser.CurrentUserWebSite.StoreManagement)
                Response.Redirect("/Admin/Account.aspx?id=" + CurrentUser.CurrentUserWebSite.CurrentUserAccount.AccountID);
            else if (CurrentUser.CurrentUserWebSite.UserManagement)
                Response.Redirect("/Admin/UserWebsiteOverview.aspx");
            else if (CurrentUser.CurrentUserWebSite.GroupManagement)
                Response.Redirect("/Admin/GroupOverview.aspx");
            else if (CurrentUser.CurrentUserWebSite.TabManagement)
                Response.Redirect("/Admin/TabOverview.aspx");
            else if (CurrentUser.CurrentUserWebSite.ItemManagement)
                Response.Redirect("/Admin/ItemOverview.aspx");
            else if (CurrentUser.CurrentUserWebSite.OrderManagement)
                Response.Redirect("/Admin/OrderOverview.aspx");
            else if (CurrentUser.CurrentUserWebSite.BudgetManagement)
                Response.Redirect("/Admin/BudgetOverview.aspx");
            else if (CurrentUser.CurrentUserWebSite.CreditCardManagement)
                Response.Redirect("/Admin/CreditCardOverview.aspx");
            else if (CurrentUser.CurrentUserWebSite.ShippingManagement)
                Response.Redirect("/Admin/ShippingOverview.aspx");
            else if (CurrentUser.CurrentUserWebSite.MessageManagement)
                Response.Redirect("/Admin/MessageOverview.aspx");
        }
    }
}
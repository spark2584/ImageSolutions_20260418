using ImageSolutionsWebsite.MyAccount;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Control
{
    public partial class AdminNavigation : BaseControlUserAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                litUser.Text = Convert.ToString(ThisPage.CurrentUser.FullName);
                litEmailAddress.Text = Convert.ToString(ThisPage.CurrentUser.EmailAddress);

                if (ThisPage.CurrentUser == null
                    || ThisPage.CurrentUser.CurrentUserWebSite == null
                    || ThisPage.CurrentUser.CurrentUserWebSite.CurrentUserAccount == null)
                {
                    Response.Redirect("/myaccount/dashboard.aspx");
                }

                aAccount.HRef += "?id=" + ThisPage.CurrentUser.CurrentUserWebSite.CurrentUserAccount.AccountID;

                aWebsite.Visible = ThisPage.CurrentUser.CurrentUserWebSite.WebsiteManagement;
                aCustomList.Visible = ThisPage.CurrentUser.CurrentUserWebSite.WebsiteManagement && ThisPage.CurrentUser.IsSuperAdmin;
                aAccount.Visible = ThisPage.CurrentUser.CurrentUserWebSite.StoreManagement;
                aSubAccount.Visible = ThisPage.CurrentUser.CurrentUserWebSite.StoreManagement;
                aUser.Visible = ThisPage.CurrentUser.CurrentUserWebSite.UserManagement;
                aGroup.Visible = ThisPage.CurrentUser.CurrentUserWebSite.GroupManagement;
                aTab.Visible = ThisPage.CurrentUser.CurrentUserWebSite.TabManagement;
                aItem.Visible = ThisPage.CurrentUser.CurrentUserWebSite.ItemManagement;
                aCustomization.Visible = ThisPage.CurrentUser.CurrentUserWebSite.ItemManagement && ThisPage.CurrentUser.IsSuperAdmin;
                aOrder.Visible = ThisPage.CurrentUser.CurrentUserWebSite.OrderManagement;

                aOrderApproval.Visible = ThisPage.CurrentUser.CurrentUserWebSite.OrderManagement && !ThisPage.CurrentWebsite.HideAdminOrderApproval && !ThisPage.CurrentUser.CurrentUserWebSite.HideOrderApproval;

                aBudget.Visible = ThisPage.CurrentWebsite.EnableEmployeeCredit && ThisPage.CurrentUser.CurrentUserWebSite.BudgetManagement;
                aCreditCard.Visible = ThisPage.CurrentUser.CurrentUserWebSite.CreditCardManagement;

                aPromotion.Visible = ThisPage.CurrentUser.IsSuperAdmin;

                aShipping.Visible = ThisPage.CurrentUser.CurrentUserWebSite.ShippingManagement;
                aMessage.Visible = ThisPage.CurrentUser.CurrentUserWebSite.MessageManagement;
                //aBudget.Visible = ThisPage.CurrentWebsite.EnableEmployeeCredit; //ThisPage.CurrentUser.CurrentUserWebSite.WebSite.EnableEmployeeCredit;

                if (ThisPage.CurrentWebsite.WebsiteID == "1" //ThisPage.CurrentUser.CurrentUserWebSite.WebSite.WebsiteID == "1"
                    && (
                        ThisPage.CurrentUser.UserInfoID != "1"
                        && ThisPage.CurrentUser.UserInfoID != "2"
                        && ThisPage.CurrentUser.UserInfoID != "3"
                        && ThisPage.CurrentUser.UserInfoID != "107"
                    )
                )
                {
                    aBudget.Visible = false;
                }

                if(!string.IsNullOrEmpty(ThisPage.CurrentWebsite.BudgetAlias))
                {
                    aBudget.InnerText = string.Format("{0} Management", ThisPage.CurrentWebsite.BudgetAlias); //ThisPage.CurrentUser.CurrentUserWebSite.WebSite.BudgetAlias); 
                }


                aInventoryReport.Visible = !ThisPage.CurrentUser.CurrentUserWebSite.HideInventoryReport;

                if (ThisPage.CurrentWebsite.WebsiteID == "2" && !ThisPage.CurrentUser.IsSuperAdmin)
                {
                    aInventoryReport.Visible = false;
                }

                //if(ThisPage.CurrentUser.CurrentUserWebSite.WebSite.WebsiteID == "7" || ThisPage.CurrentUser.CurrentUserWebSite.WebSite.WebsiteID == "8")
                if (ThisPage.CurrentWebsite.WebsiteID == "7" || ThisPage.CurrentWebsite.WebsiteID == "8")
                {
                    aInventoryReport.Visible = false;
                }



                string strURL = Request.Url.ToString();

                if (strURL.ToLower().Contains("/admin/website.aspx") || strURL.ToLower().Contains("/admin/websiteoverview.aspx"))
                {
                    aWebsite.Attributes.Remove("class");
                    aWebsite.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/admin/customlist.aspx") || strURL.ToLower().Contains("/admin/customlistoverview.aspx"))
                {
                    aCustomList.Attributes.Remove("class");
                    aCustomList.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/admin/account.aspx"))
                {
                    if (Request.QueryString.Get("id") == ThisPage.CurrentUser.CurrentUserWebSite.CurrentUserAccount.AccountID)
                    {
                        aAccount.Attributes.Remove("class");
                        aAccount.Attributes.Add("class", "nav-link active");
                    }
                    else
                    {
                        aSubAccount.Attributes.Remove("class");
                        aSubAccount.Attributes.Add("class", "nav-link active");
                    }
                }
                else if (strURL.ToLower().Contains("/admin/accountoverview.aspx"))
                {
                    aSubAccount.Attributes.Remove("class");
                    aSubAccount.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/admin/userwebsite.aspx") || strURL.ToLower().Contains("/admin/userwebsiteoverview.aspx"))
                {
                    aUser.Attributes.Remove("class");
                    aUser.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/admin/group.aspx") || strURL.ToLower().Contains("/admin/groupoverview.aspx"))
                {
                    aGroup.Attributes.Remove("class");
                    aGroup.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/admin/tab.aspx") || strURL.ToLower().Contains("/admin/taboverview.aspx"))
                {
                    aTab.Attributes.Remove("class");
                    aTab.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/admin/item.aspx") || strURL.ToLower().Contains("/admin/itemoverview.aspx"))
                {
                    aItem.Attributes.Remove("class");
                    aItem.Attributes.Add("class", "nav-link active");

                    aCustomization.Attributes.Remove("class");
                    aCustomization.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/admin/order.aspx") || strURL.ToLower().Contains("/admin/orderoverview.aspx"))
                {
                    aOrder.Attributes.Remove("class");
                    aOrder.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/admin/order.aspx") || strURL.ToLower().Contains("/admin/orderpendingapproval.aspx"))
                {
                    aOrderApproval.Attributes.Remove("class");
                    aOrderApproval.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/admin/budget.aspx") || strURL.ToLower().Contains("/admin/budgetoverview.aspx"))
                {
                    aBudget.Attributes.Remove("class");
                    aBudget.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/admin/creditcard.aspx") || strURL.ToLower().Contains("/admin/creditcardoverview.aspx"))
                {
                    aCreditCard.Attributes.Remove("class");
                    aCreditCard.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/admin/promotion.aspx") || strURL.ToLower().Contains("/admin/promotionoverview.aspx"))
                {
                    aPromotion.Attributes.Remove("class");
                    aPromotion.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/admin/shipping.aspx") || strURL.ToLower().Contains("/admin/shippingoverview.aspx"))
                {
                    aShipping.Attributes.Remove("class");
                    aShipping.Attributes.Add("class", "nav-link active");
                }
                else if (strURL.ToLower().Contains("/admin/message.aspx") || strURL.ToLower().Contains("/admin/messageoverview.aspx"))
                {
                    aMessage.Attributes.Remove("class");
                    aMessage.Attributes.Add("class", "nav-link active");
                }
                //else if (strURL.ToLower().Contains("/admin/customization.aspx") || strURL.ToLower().Contains("/admin/customizationoverview.aspx"))
                //{
                //    aCustomization.Attributes.Remove("class");
                //    aCustomization.Attributes.Add("class", "nav-link active");
                //}


                //if(ThisPage.CurrentWebsite.WebsiteID == "43" || ThisPage.CurrentWebsite.WebsiteID == "45")
                if (
                    (ConfigurationManager.AppSettings["Environment"] == "production" && (ThisPage.CurrentWebsite.WebsiteID == "7" || ThisPage.CurrentWebsite.WebsiteID == "8"))
                    ||
                    (ConfigurationManager.AppSettings["Environment"] != "production" && (ThisPage.CurrentWebsite.WebsiteID == "43" || ThisPage.CurrentWebsite.WebsiteID == "45"))
                )
                {
                aBudget.InnerText = "Store Budget Management";
                }
            }
        }
    }
}
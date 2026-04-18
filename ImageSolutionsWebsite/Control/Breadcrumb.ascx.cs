using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Controls
{
    public partial class Breadcrumb : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string strURL = Request.Url.ToString();

                if (strURL.ToLower().Contains("/userwebsite.aspx"))
                {
                    this.phUserWebSite.Visible = true;
                    this.lblPageName.Text = "User Website";
                }
                else if (strURL.ToLower().Contains("/usergroup.aspx"))
                {
                    this.phUserGroup.Visible = true;
                    this.lblPageName.Text = "User Group";
                }
                else if (strURL.ToLower().Contains("/websitetab.aspx"))
                {
                    this.phWebsiteTab.Visible = true;
                    this.lblPageName.Text = "Website Tab";
                }
                else if (strURL.ToLower().Contains("/shoppingcart.aspx"))
                {
                    this.phShoppingCart.Visible = true;
                    this.lblPageName.Text = "Shopping Cart";
                }
                else if (strURL.ToLower().Contains("/useraccount.aspx"))
                {
                    this.phUserAccount.Visible = true;
                    this.lblPageName.Text = "Assigned Accounts";
                }
                else if (strURL.ToLower().Contains("/register.aspx") || strURL.ToLower().Contains("/registrationcomplete.aspx"))
                {
                    this.phRegistration.Visible = true;
                    this.lblPageName.Text = "Sub-Account Registration";
                }
                else if (strURL.ToLower().Contains("/myaccount/"))
                {
                    this.phMyAccount.Visible = true;
                    if (strURL.ToLower().Contains("/myaccount/addressbook.aspx"))
                    {
                        this.aSecondTier.InnerText = "Address Book";
                        //this.aSecondTier.HRef = "/myaccount/addressbook.aspx";
                    }
                    else if (strURL.ToLower().Contains("/myaccount/orders.aspx"))
                    {
                        this.aSecondTier.InnerText = "My Orders";
                        //this.aSecondTier.HRef = "/myaccount/orders.aspx";
                    }
                    else if (strURL.ToLower().Contains("/myaccount/budgets.aspx"))
                    {
                        this.aSecondTier.InnerText = "My Employee Credits";
                        //this.aSecondTier.HRef = "/myaccount/orders.aspx";
                    }
                    else if (strURL.ToLower().Contains("/myaccount/creditcard.aspx"))
                    {
                        this.aSecondTier.InnerText = "Saved Cards";
                        //this.aSecondTier.HRef = "/myaccount/orders.aspx";
                    }
                    else if (strURL.ToLower().Contains("/myaccount/profile.aspx"))
                    {
                        this.aSecondTier.InnerText = "Profile";
                        //this.aSecondTier.HRef = "/myaccount/orders.aspx";
                    }
                    this.lblPageName.Text = this.aSecondTier.InnerText;
                    if (string.IsNullOrEmpty(this.lblPageName.Text)) this.lblPageName.Text = "My Dashboard";
                }
                else if (strURL.ToLower().Contains("/itemlist.aspx"))
                {
                    this.phItemCategory.Visible = true;

                    string mWebSiteTabID = Request.QueryString.Get("WebSiteTabID");
                    if (!string.IsNullOrEmpty(mWebSiteTabID))
                    {
                        ImageSolutions.Website.WebsiteTab objWebsiteTab = new ImageSolutions.Website.WebsiteTab(mWebSiteTabID);
                        if (objWebsiteTab != null)
                        {
                            this.lblPageName.Text = string.IsNullOrEmpty(Convert.ToString(objWebsiteTab.DisplayName).Trim()) ? objWebsiteTab.TabName : objWebsiteTab.DisplayName;
                        }
                    }
                }
                else if (strURL.ToLower().Contains("/admin/"))
                {
                    this.phAdmin.Visible = true;
                    if (strURL.ToLower().Contains("/admin/website.aspx") || strURL.ToLower().Contains("/admin/websiteoverview.aspx"))
                    {
                        this.aSecondTier.InnerText = "Website Management";
                        //this.aSecondTier.HRef = "/myaccount/addressbook.aspx";
                    }
                    else if (strURL.ToLower().Contains("/admin/user.aspx") || strURL.ToLower().Contains("/admin/useroverview.aspx"))
                    {
                        this.aSecondTier.InnerText = "User Management";
                        //this.aSecondTier.HRef = "/myaccount/orders.aspx";
                    }
                    else if (strURL.ToLower().Contains("/admin/group.aspx") || strURL.ToLower().Contains("/admin/groupoverview.aspx"))
                    {
                        this.aSecondTier.InnerText = "Group Management";
                        //this.aSecondTier.HRef = "/myaccount/orders.aspx";
                    }
                    else if (strURL.ToLower().Contains("/admin/tab.aspx") || strURL.ToLower().Contains("/admin/taboverview.aspx"))
                    {
                        this.aSecondTier.InnerText = "Tab Management";
                        //this.aSecondTier.HRef = "/myaccount/orders.aspx";
                    }
                    else if (strURL.ToLower().Contains("/admin/item.aspx") || strURL.ToLower().Contains("/admin/itemoverview.aspx"))
                    {
                        this.aSecondTier.InnerText = "Item Management";
                        //this.aSecondTier.HRef = "/myaccount/orders.aspx";
                    }
                    else if (strURL.ToLower().Contains("/admin/account.aspx") || strURL.ToLower().Contains("/admin/accountoverview.aspx"))
                    {
                        this.aSecondTier.InnerText = "Account Management";
                        //this.aSecondTier.HRef = "/myaccount/orders.aspx";
                    }
                    else if (strURL.ToLower().Contains("/admin/userwebsite.aspx") || strURL.ToLower().Contains("/admin/userwebsiteoverview.aspx"))
                    {
                        this.aSecondTier.InnerText = "User Management";
                        //this.aSecondTier.HRef = "/myaccount/orders.aspx";
                    }
                    else if (strURL.ToLower().Contains("/admin/order.aspx") || strURL.ToLower().Contains("/admin/orderoverview.aspx"))
                    {
                        this.aSecondTier.InnerText = "Order Management";
                        //this.aSecondTier.HRef = "/myaccount/orders.aspx";
                    }
                    else if (strURL.ToLower().Contains("/admin/creditcard.aspx") || strURL.ToLower().Contains("/admin/creditcardoverview.aspx"))
                    {
                        this.aSecondTier.InnerText = "Credit Card Management";
                        //this.aSecondTier.HRef = "/myaccount/orders.aspx";
                    }
                    else if (strURL.ToLower().Contains("/admin/shipping.aspx") || strURL.ToLower().Contains("/admin/shippingoverview.aspx"))
                    {
                        this.aSecondTier.InnerText = "Shipping Management";
                        //this.aSecondTier.HRef = "/myaccount/orders.aspx";
                    }
                    else if (strURL.ToLower().Contains("/admin/message.aspx") || strURL.ToLower().Contains("/admin/messageoverview.aspx"))
                    {
                        this.aSecondTier.InnerText = "Message Management";
                        //this.aSecondTier.HRef = "/myaccount/orders.aspx";
                    }
                    this.lblPageName.Text = this.aSecondTier.InnerText;
                    if (string.IsNullOrEmpty(this.lblPageName.Text)) this.lblPageName.Text = "Administration";
                }
                else if (strURL.ToLower().Contains("/checkout.aspx"))
                {
                    this.phCheckout.Visible = true;
                    this.lblPageName.Text = "Checkout";
                }
                else if (strURL.ToLower().Contains("/search.aspx"))
                {
                    this.phSearch.Visible = true;
                    this.lblPageName.Text = "Search";
                }
                //else this.Visible = false;
            }
        }
    }
}
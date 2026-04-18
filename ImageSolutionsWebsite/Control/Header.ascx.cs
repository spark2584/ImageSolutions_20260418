using CLRFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Controls
{
    public partial class Header : BaseControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ThisPage.CurrentUser.IsLoggedIn)
            {
                if(ThisPage.CurrentUser.CurrentUserWebSite != null && ThisPage.CurrentUser.CurrentUserWebSite.IsStore)
                {
                    this.lblUserName.Text = "Welcome " + ThisPage.CurrentUser.FirstName + ' ' + ThisPage.CurrentUser.LastName + "! ";
                }
                else
                {
                    this.lblUserName.Text = "Welcome " + ThisPage.CurrentUser.FirstName + "! ";
                }

                if (ThisPage.CurrentUser.CurrentUserWebSite.CurrentUserAccount.IsLoggedIn)
                {
                    if (ThisPage.CurrentUser.CurrentUserWebSite.UserAccounts != null && ThisPage.CurrentUser.CurrentUserWebSite.UserAccounts.Count() > 1)
                    {
                        this.lblUserName.Text += "(Account: " + ThisPage.CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.AccountName + ", User Group: " + ThisPage.CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroup.GroupName + ")";
                    }
                    //else
                    //{
                    //    this.lblUserName.Text += "(Store: " + ThisPage.CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.AccountName + ")";
                    //}

                    if (ThisPage.CurrentUser.CurrentUserWebSite.MyBudgetAssignments != null
                        && ThisPage.CurrentUser.CurrentUserWebSite.MyBudgetAssignments.FindAll(x =>
                            !x.BudgetAssignment.InActive
                            && x.BudgetAssignment.Budget.StartDate <= DateTime.UtcNow
                            && x.BudgetAssignment.Budget.EndDate >= DateTime.UtcNow.AddDays(-1)
                            && x.Balance > 0
                        ).Count > 0
                    )
                    {
                        List<ImageSolutions.Budget.MyBudgetAssignment> MyBudgetAssignments = ThisPage.CurrentUser.CurrentUserWebSite.MyBudgetAssignments.FindAll(x =>
                            !x.BudgetAssignment.InActive
                            && x.BudgetAssignment.Budget.StartDate <= DateTime.UtcNow
                            && x.BudgetAssignment.Budget.EndDate >= DateTime.UtcNow.AddDays(-1)
                            && x.Balance > 0
                        );

                        if (MyBudgetAssignments != null && MyBudgetAssignments.Count > 0)
                        {
                            this.lblUserName.Text += String.Format(" - Remaining Budget: ${0}", MyBudgetAssignments.Sum(x => x.Balance));
                        }
                    }
                }
            }

            this.phLogin.Visible = !ThisPage.CurrentUser.IsLoggedIn || ThisPage.CurrentUser.IsGuest;
            this.phLogout.Visible = ThisPage.CurrentUser.IsLoggedIn && !ThisPage.CurrentUser.IsGuest;

            liMyAccount.Visible = ThisPage.CurrentUser.IsLoggedIn;
            divSideMenu.Visible = ThisPage.CurrentUser.IsLoggedIn;

            if (ThisPage.CurrentWebsite.IsLoggedIn)
            {
                this.imgLogo.Src = ThisPage.CurrentWebsite.LogoPath;

                if (ThisPage.CurrentUser.CurrentUserWebSite.UserAccounts != null && ThisPage.CurrentUser.CurrentUserWebSite.UserAccounts.Exists(x => !string.IsNullOrEmpty(x.WebsiteGroup.LogoPath)))
                {
                    this.imgLogo.Src = ThisPage.CurrentUser.CurrentUserWebSite.UserAccounts.Find(x => !string.IsNullOrEmpty(x.WebsiteGroup.LogoPath)).WebsiteGroup.LogoPath;
                }
            }
            if (!Page.IsPostBack)
            {               
                this.aUserWebsite.Visible = ThisPage.CurrentUser.UserWebsites != null 
                    && (ThisPage.CurrentUser.IsSuperAdmin || (ThisPage.CurrentWebsite != null && !ThisPage.CurrentWebsite.IsPunchout))
                    //&& ThisPage.CurrentUser.UserWebsites.FindAll(m => !m.InActive && !m.WebSite.IsPunchout).Count > 1;
                    && ThisPage.CurrentUser.UserWebsites.Where(m => !m.InActive && !m.WebSite.IsPunchout).ToList().Count > 1;

                this.aUserAccount.Visible = ThisPage.CurrentUser.CurrentUserWebSite != null 
                    && ThisPage.CurrentUser.CurrentUserWebSite.UserAccounts != null 
                    && ThisPage.CurrentUser.CurrentUserWebSite.UserAccounts != null 
                    && (
                        (!ThisPage.CurrentWebsite.CombineWebsiteGroup && ThisPage.CurrentUser.CurrentUserWebSite.UserAccounts.Count > 1)
                        ||
                        (ThisPage.CurrentWebsite.CombineWebsiteGroup && ThisPage.CurrentUser.CurrentUserWebSite.UserAccounts.GroupBy(x => x.AccountID).Distinct().ToList().Count  > 1)
                    );

                this.aAdmin.Visible = ThisPage.CurrentUser.CurrentUserWebSite != null && ThisPage.CurrentUser.CurrentUserWebSite.IsAdmin 
                    && ThisPage.CurrentUser.CurrentUserWebSite.CurrentUserAccount != null && !string.IsNullOrEmpty(ThisPage.CurrentUser.CurrentUserWebSite.CurrentUserAccount.UserAccountID);

                //this.litWebsiteName.Text = ThisPage.CurrentWebsite == null ? string.Empty :　"Welcome to " + ThisPage.CurrentWebsite.Name;

                BindShoppingCart();
            }

            if(ThisPage.CurrentWebsite != null && ThisPage.CurrentWebsite.IsPunchout)
            {
                if (!ThisPage.CurrentUser.IsSuperAdmin)
                {
                    aUserWebsite.Visible = false;
                    aLogout.Visible = false;
                }

                lnkReturn.Visible = true;
                //lnkReturn.NavigateUrl = ThisPage.CurrentUser.CurrentUserWebSite.PunchoutReturnURL;
            }
            else
            {
                aLogout.Visible = true;
                lnkReturn.Visible = false;
            }

            if (!ThisPage.CurrentUser.IsSuperAdmin 
                && (
                    (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && ThisPage.CurrentWebsite.WebsiteID == "65")
                    || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && ThisPage.CurrentWebsite.WebsiteID == "31")
                )
            )
            {
                liMyAccount.Visible = false;
            }

            if (!IsPostBack)
            {
                string strReload = Request.QueryString.Get("reload");
                if (strReload == "1")
                {
                    string clientId = ulShoppingCart.ClientID;

                    // Register the JavaScript that triggers the hover effect
                    string script = $"window.onload = function() {{ triggerHover('{clientId}'); }};";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "TriggerHoverOnLoad", script, true);
                }

                if (ThisPage.CurrentUser != null)
                {
                    if (HttpContext.Current.Request.Cookies["ISAdminUserGUID"] != null)
                    {
                        string strAdminGuid = Convert.ToString(HttpContext.Current.Request.Cookies["ISAdminUserGUID"].Value);
                        btnReturn.Visible = !string.IsNullOrEmpty(strAdminGuid) && ThisPage.CurrentUser.GUID != strAdminGuid;
                    }
                }
            }
        }

        protected void BindShoppingCart()
        {
            if (ThisPage.CurrentUser.CurrentUserWebSite != null && ThisPage.CurrentUser.CurrentUserWebSite.ShoppingCart != null)
            {
                ThisPage.CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines = null;
                this.rptShoppingCart.DataSource = ThisPage.CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines;
                this.rptShoppingCart.DataBind();

                this.lblShoppingCartQuantity.Text = ThisPage.CurrentUser.CurrentUserWebSite.ShoppingCart.TotalQuantity.ToString();
                this.lblShoppingCartTotal.Text = ThisPage.CurrentUser.CurrentUserWebSite.ShoppingCart.LineTotal.ToString("C"); //ThisPage.CurrentUser.CurrentUserWebSite.ShoppingCart.GetTotal(ThisPage.CurrentUser.CurrentUserWebSite.IsTaxExempt).ToString("C");
            }
        }

        protected void rptShoppingCart_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            ImageSolutions.ShoppingCart.ShoppingCartLine objShoppingCartLine = null;
            string strShoppingCartLineID = string.Empty;

            try
            {
                strShoppingCartLineID = Convert.ToString(e.CommandArgument);
                objShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine(strShoppingCartLineID);

                if (objShoppingCartLine != null)
                {
                    if (e.CommandName == "DeleteLine")
                    {
                        objShoppingCartLine.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objShoppingCartLine = null;
            }
            BindShoppingCart();
        }

        protected void txtSearchText_TextChanged(object sender, EventArgs e)
        {
            //Response.Redirect("/search.aspx?txt=" + Server.UrlEncode(this.txtSearchText.Text.Trim()));

            if(ThisPage.CurrentUser != null && ThisPage.CurrentUser.CurrentUserWebSite != null && !string.IsNullOrEmpty(ThisPage.CurrentUser.CurrentUserWebSite.WebsiteID))
            {
                if (ThisPage.CurrentUser.CurrentUserWebSite.WebSite.ItemDisplayType == "List")
                {
                    Response.Redirect("/itemlist.aspx?Search=" + Server.UrlEncode(this.txtSearchText.Text.Trim()));
                }
                else
                {
                    Response.Redirect("/items.aspx?Search=" + Server.UrlEncode(this.txtSearchText.Text.Trim()));
                }
            }
            else
            {
                Response.Redirect("/Login.aspx");
            }
        }

        protected void ddlStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            Response.Redirect(WebUtility.URL.RawUrlNoQuery);
        }

        protected void lnkReturn_Click(object sender, EventArgs e)
        {
            if (ThisPage.CurrentUser != null)
            {
                ThisPage.CurrentUser.Logout();

                if (ThisPage.CurrentUser.CurrentUserWebSite != null)
                {
                    ThisPage.CurrentUser.CurrentUserWebSite.Logout();

                    if (ThisPage.CurrentUser.CurrentUserWebSite.CurrentUserAccount != null)
                    {
                        ThisPage.CurrentUser.CurrentUserWebSite.CurrentUserAccount.Logout();
                    }

                    if (!string.IsNullOrEmpty(ThisPage.CurrentUser.CurrentUserWebSite.PunchoutReturnURL))
                    {
                        Response.Redirect(ThisPage.CurrentUser.CurrentUserWebSite.PunchoutReturnURL);
                    }
                }
            }

            Response.Redirect("/Login.aspx");
        }

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.Cookies["ISAdminUserGUID"] != null)
            {
                string strAdminGuid = Convert.ToString(HttpContext.Current.Request.Cookies["ISAdminUserGUID"].Value);

                if (!string.IsNullOrEmpty(strAdminGuid))
                {
                    ImageSolutions.User.UserInfo AdminUserInfo = new ImageSolutions.User.UserInfo();
                    ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                    UserInfoFilter.GUID = new Database.Filter.StringSearch.SearchFilter();
                    UserInfoFilter.GUID.SearchString = strAdminGuid;
                    AdminUserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

                    ThisPage.CurrentUser.LoginAs(AdminUserInfo.EmailAddress);

                    Response.Redirect("/myaccount/dashboard.aspx");
                }
                else
                {
                    Response.Redirect("/myaccount/dashboard.aspx");
                }
            }
            else
            {
                Response.Redirect("/myaccount/dashboard.aspx");
            }
        }
    }
}
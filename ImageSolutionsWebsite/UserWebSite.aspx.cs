using ImageSolutionsWebsite.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class UserWebSite : BasePageUserAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindUserWebSites();
            }
        }

        protected void BindUserWebSites()
        {
            string strAdminGuid = string.Empty;
            if (HttpContext.Current.Request.Cookies["ISAdminUserGUID"] != null)
                strAdminGuid = Convert.ToString(HttpContext.Current.Request.Cookies["ISAdminUserGUID"].Value);

            this.gvUserWebSites.DataSource = CurrentUser.UserWebsites == null ? null : CurrentUser.UserWebsites.FindAll(m => !m.InActive && ( CurrentUser.IsSuperAdmin || !m.WebSite.IsPunchout || !string.IsNullOrEmpty(strAdminGuid)));
            this.gvUserWebSites.DataBind();
            if (this.gvUserWebSites.Rows.Count == 1)
            {
                string strUserWebSiteID = this.gvUserWebSites.DataKeys[0].Value.ToString();

                ImageSolutions.User.UserWebsite objUserWebsite = new ImageSolutions.User.UserWebsite(strUserWebSiteID);

                SetUserWebSite(objUserWebsite.GUID);
            }
            if (this.gvUserWebSites.HeaderRow != null) this.gvUserWebSites.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void gvUserWebSites_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string strUserWebSiteID = this.gvUserWebSites.DataKeys[e.RowIndex].Value.ToString();

            ImageSolutions.User.UserWebsite objUserWebsite = new ImageSolutions.User.UserWebsite(strUserWebSiteID);

            SetUserWebSite(objUserWebsite.GUID);
        }

        protected void SetUserWebSite(string GUID)
        {
            CurrentUser.CurrentUserWebSite.Login(GUID);
            CurrentWebsite.Login(CurrentUser.CurrentUserWebSite.WebSite.GUID);
            CurrentUser.LastVisitedUserWebsiteID = CurrentUser.CurrentUserWebSite.UserWebsiteID;

            ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
            ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
            UserAccountFilter.UserAccountID = new Database.Filter.StringSearch.SearchFilter();
            UserAccountFilter.UserAccountID.SearchString = CurrentUser.LastVisitedUserAccountID;
            UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);

            if (UserAccount == null || (CurrentUser.LastVisitedUserAccount != null && CurrentUser.CurrentUserWebSite.WebsiteID != CurrentUser.LastVisitedUserAccount.WebsiteGroup.WebsiteID))
            {
                CurrentUser.CurrentUserWebSite.CurrentUserAccount.Logout();
                CurrentUser.LastVisitedUserAccountID = String.Empty;
            }
            CurrentUser.Update();

            if (string.IsNullOrEmpty(Request.QueryString.Get("target")))
            {
                if (!string.IsNullOrEmpty(CurrentWebsite.StartingPath))
                {
                    Response.Redirect(CurrentWebsite.StartingPath);
                }
                else
                {
                    Response.Redirect("/myaccount/dashboard.aspx");
                }
            }
            else
            {
                Response.Redirect(Request.QueryString.Get("target"));
            }
        }

        protected void gvUserWebSites_Sorting(object sender, GridViewSortEventArgs e)
        {

        }

        protected void gvUserWebSites_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string strUserWebsiteID = Convert.ToString(gvUserWebSites.DataKeys[e.Row.RowIndex].Value.ToString());
                    ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite(strUserWebsiteID);

                    LinkButton lkImage = (LinkButton)e.Row.FindControl("lkImage");
                    LinkButton lkBtnBusiness = (LinkButton)e.Row.FindControl("lkBtnBusiness");

                    string strAdminGuid = string.Empty;
                    if (HttpContext.Current.Request.Cookies["ISAdminUserGUID"] != null)
                        strAdminGuid = Convert.ToString(HttpContext.Current.Request.Cookies["ISAdminUserGUID"].Value);

                    lkImage.Enabled = !UserWebsite.WebSite.IsPunchout || CurrentUser.IsSuperAdmin || !string.IsNullOrEmpty(strAdminGuid);
                    lkBtnBusiness.Enabled = !UserWebsite.WebSite.IsPunchout || CurrentUser.IsSuperAdmin || !string.IsNullOrEmpty(strAdminGuid);
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
    }
}
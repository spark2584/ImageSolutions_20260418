using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class WebsiteOverview : BasePageAdminUserAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindWebsite();
            }
        }
        protected void BindWebsite()
        {
            try
            {
                this.gvUserWebsites.DataSource = CurrentUser.UserWebsites.FindAll(m => m.IsAdmin);
                this.gvUserWebsites.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void gvUserWebsites_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string strUserWebSiteID = this.gvUserWebsites.DataKeys[e.RowIndex].Value.ToString();

            ImageSolutions.User.UserWebsite objUserWebsite = new ImageSolutions.User.UserWebsite(strUserWebSiteID);

            SetUserWebSite(objUserWebsite.GUID);
        }

        protected void SetUserWebSite(string GUID)
        {
            CurrentUser.CurrentUserWebSite.Login(GUID);
            CurrentWebsite.Login(CurrentUser.CurrentUserWebSite.WebSite.GUID);
            CurrentUser.LastVisitedUserWebsiteID = CurrentUser.CurrentUserWebSite.UserWebsiteID;
            if (CurrentUser.LastVisitedUserAccount != null && CurrentUser.CurrentUserWebSite.WebsiteID != CurrentUser.LastVisitedUserAccount.WebsiteGroup.WebsiteID)
            {
                CurrentUser.CurrentUserWebSite.CurrentUserAccount.Logout();
                CurrentUser.LastVisitedUserAccountID = String.Empty;
            }
            CurrentUser.Update();
            Response.Redirect("/Admin/Website.aspx");
        }
    }
}
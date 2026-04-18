using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class Punchout : BasePage
    {
        protected string mGUID = string.Empty;
        protected string mPunchoutGUID = string.Empty;
        protected string mPunchoutSession = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            mGUID = Request.QueryString.Get("website");
            mPunchoutSession = Request.QueryString.Get("session");
            mPunchoutGUID = Request.QueryString.Get("guid");

            //if (HttpContext.Current.Request.Cookies["WebsiteGUID"] != null) HttpContext.Current.Response.Cookies["WebsiteGUID"].Expires = System.DateTime.Now.AddDays(-1);            
            //if (HttpContext.Current.Request.Cookies["WebsiteMessage"] != null) HttpContext.Current.Response.Cookies["WebsiteMessage"].Expires = System.DateTime.Now.AddDays(-1);
            //if (HttpContext.Current.Request.Cookies["UserWebSiteGUID"] != null) HttpContext.Current.Response.Cookies["UserWebSiteGUID"].Expires = System.DateTime.Now.AddDays(-1);
            //if (HttpContext.Current.Request.Cookies["UserAccountGUID"] != null) HttpContext.Current.Response.Cookies["UserAccountGUID"].Expires = System.DateTime.Now.AddDays(-1);

            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
            ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
            UserWebsiteFilter.PunchoutSessionID = new Database.Filter.StringSearch.SearchFilter();
            UserWebsiteFilter.PunchoutSessionID.SearchString = mPunchoutSession;
            UserWebsiteFilter.PunchoutGUID = new Database.Filter.StringSearch.SearchFilter();
            UserWebsiteFilter.PunchoutGUID.SearchString = mPunchoutGUID;
            UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

            //LogOut
            if (CurrentUser != null && !string.IsNullOrEmpty(CurrentUser.UserInfoID))
            {
                CurrentUser.Logout();
                CurrentUser.CurrentUserWebSite.Logout();
                CurrentUser.CurrentUserWebSite.CurrentUserAccount.Logout();
                CurrentWebsite.Logout();

                Response.Redirect(String.Format("/punchoutredirect.aspx?website={0}&session={1}&guid={2}", mGUID, mPunchoutSession, mPunchoutGUID));
            }

            //Set Website
            CurrentWebsite.Login(mGUID);
            
            //LogIn           
            CurrentUser.Login(UserWebsite.UserInfo.EmailAddress, UserWebsite.UserInfo.Password, true);
            CurrentUser.CurrentUserWebSite.Login(UserWebsite.GUID);

            Response.Redirect("/myaccount/dashboard.aspx?login=t");
            
        }
    }
}
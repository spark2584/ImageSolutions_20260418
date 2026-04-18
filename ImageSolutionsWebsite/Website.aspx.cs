using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class Website : BasePage
    {
        protected string mGUID = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            mGUID = Request.QueryString.Get("website");

            if (!Page.IsPostBack)
            {
                Initialize();
            }

            if (CurrentWebsite.WebsiteID == "20") Response.Redirect("/ssologin");

            if (CurrentWebsite.AllowGuestCheckout)
            {
                CurrentUser.Logout();
                CurrentUser.CurrentUserWebSite.Logout();
                CurrentUser.CurrentUserWebSite.CurrentUserAccount.Logout();

                CurrentUser.LoginAsGuest(CurrentWebsite);
                Response.Redirect("/");
            }
            else
            {
                if (CurrentUser.IsLoggedIn) Response.Redirect("/");
                else
                    Response.Redirect("/login.aspx");
            }
        }

        protected void Initialize()
        {
            if (!string.IsNullOrEmpty(mGUID))
            {
                if (CurrentUser != null && CurrentUser.IsGuest)
                {
                    CurrentUser.Logout();
                    CurrentUser.CurrentUserWebSite.Logout();
                    CurrentUser.CurrentUserWebSite.CurrentUserAccount.Logout();
                }

                CurrentWebsite.Login(mGUID);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.HomePage
{
    public partial class ABSCompaniesUniforms : BasePageUserAccountAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HideBreadcrumb = true;
            if (CurrentWebsite != null && !string.IsNullOrEmpty(CurrentWebsite.StartingPath))
            {
                if (CurrentWebsite.StartingPath.ToLower() != WebUtility.URL.RawUrlNoQuery.ToLower())
                {
                    Response.Redirect(CurrentWebsite.StartingPath);
                }
            }
            else
            {
                Response.Redirect("/myaccount/dashboard.aspx");
            }

            if (CurrentUser.CurrentUserWebSite != null
                && CurrentUser.CurrentUserWebSite.UserAccounts != null
                && CurrentUser.CurrentUserWebSite.UserAccounts.Exists(x => !string.IsNullOrEmpty(x.WebsiteGroup.HomePageImagePath))
            )
            {
                imgHomePage.ImageUrl = CurrentUser.CurrentUserWebSite.UserAccounts.Find(x => !string.IsNullOrEmpty(x.WebsiteGroup.HomePageImagePath)).WebsiteGroup.HomePageImagePath;
                imgHomePageMobile.ImageUrl = imgHomePage.ImageUrl;
            }


            if (CurrentUser.CurrentUserWebSite != null
                && CurrentUser.CurrentUserWebSite.UserAccounts != null
                && CurrentUser.CurrentUserWebSite.UserAccounts.Exists(x => !string.IsNullOrEmpty(x.WebsiteGroup.HomePageMobileImagePath))
            )
            {
                imgHomePageMobile.ImageUrl = CurrentUser.CurrentUserWebSite.UserAccounts.Find(x => !string.IsNullOrEmpty(x.WebsiteGroup.HomePageMobileImagePath)).WebsiteGroup.HomePageMobileImagePath;
            }
        }
    }
}
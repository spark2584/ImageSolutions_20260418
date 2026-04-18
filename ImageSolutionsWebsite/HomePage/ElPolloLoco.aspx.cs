using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.HomePage
{
    public partial class ElPolloLoco : BasePageUserAccountAuth
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
                litBannerHTML.Text = CurrentWebsite.BannerHTML;
                litFeaturedProduct.Text = CurrentWebsite.FeaturedProductHTML;
            }
            else
            {
                Response.Redirect("/myaccount/dashboard.aspx");
            }

            if (CurrentUser.CurrentUserWebSite != null)
            {
                if (
                    (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && !CurrentUser.CurrentUserWebSite.UserAccounts.Exists(x => x.WebsiteGroupID == "292"))
                    ||
                    (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && !CurrentUser.CurrentUserWebSite.UserAccounts.Exists(x => x.WebsiteGroupID == "278"))
                )
                {
                    Response.Redirect("/myaccount/dashboard.aspx");
                }
            }
        }
    }
}
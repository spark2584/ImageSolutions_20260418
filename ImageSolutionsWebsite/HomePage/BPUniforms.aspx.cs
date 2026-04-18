using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.HomePage
{
    public partial class BPUniforms : BasePageUserAccountAuth
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

            if (
                (ConfigurationManager.AppSettings["Environment"] == "production" && (CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID == "233" || CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID == "234"))
                ||
                (CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID == "278" || CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID == "279")
            )
            {
                pnlBP.Visible = false;
                pnlAMPM.Visible = !pnlBP.Visible;
            }

        }
    }
}
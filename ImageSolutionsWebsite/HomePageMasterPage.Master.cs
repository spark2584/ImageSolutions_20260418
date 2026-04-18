using ImageSolutionsWebsite.Controls;
using ImageSolutionsWebsite.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class HomePageMasterPage : BaseMaster
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (WebUtility.URL.AbsoluteDomain.ToLower().Contains("portal.imageinc.com")) litGA.Visible = true;

            if (ThisPage.HideBreadcrumb)
            {
                ucBreadcrumb.Visible = false;
            }

            //if (ThisPage.CurrentWebsite != null 
            //    && (ThisPage.CurrentWebsite.WebsiteID == "1" || ThisPage.CurrentWebsite.WebsiteID == "33" 
            //        || ThisPage.CurrentWebsite.WebsiteID == "13" || ThisPage.CurrentWebsite.WebsiteID == "14" || ThisPage.CurrentWebsite.WebsiteID == "16"
            //        || ThisPage.CurrentWebsite.WebsiteID == "15" || ThisPage.CurrentWebsite.WebsiteID == "18" || ThisPage.CurrentWebsite.WebsiteID == "19"
            //        || ThisPage.CurrentWebsite.WebsiteID == "17") 
            //)
            if (ThisPage.CurrentWebsite != null && ThisPage.CurrentWebsite.EnableZendesk)
            {
                string strScript = "<script id=\"ze-snippet\" src=\"https://static.zdassets.com/ekr/snippet.js?key=4381d13f-4273-4ac1-a473-6fe5a0bdbae8\"> </script>";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "zendesk", strScript, false);
            }

            litLocalize.Visible = ThisPage.CurrentWebsite.EnableLocalize;
        }
    }
}
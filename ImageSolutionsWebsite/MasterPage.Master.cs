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
    public partial class MasterPage : BaseMaster
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (WebUtility.URL.AbsoluteDomain.ToLower().Contains("portal.imageinc.com"))
                litGA.Visible = true;

            if (litGA.Visible)
            {
                litGA.Text = String.Format(@"
<!-- Google tag (gtag.js) -->
<script async src=""https://www.googletagmanager.com/gtag/js?id=G-1ZJ8Y8TVS7""></script>
<script>
    window.dataLayer = window.dataLayer || [];
    function gtag() {{ dataLayer.push(arguments); }}
    gtag('js', new Date()); 
    {0}
    gtag('config', 'G-1ZJ8Y8TVS7');
</script>
"
                    , string.IsNullOrEmpty(ThisPage.CurrentWebsite.Name) ? String.Empty : String.Format("gtag('set','user_properties',{{ banner: '{0}' }});", Convert.ToString(ThisPage.CurrentWebsite.Name))
                ); ;
            }


//            if (litGA.Visible && !string.IsNullOrEmpty(ThisPage.CurrentWebsite.Name))
//            {
//                litGAWebsite.Text = String.Format(
//    "<script>gtag('set','user_properties',{{ banner: '{0}' }});</script>",
//    Convert.ToString(ThisPage.CurrentWebsite.Name)
//);
//            }

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
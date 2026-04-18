using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Controls
{
    public partial class Footer : BaseControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (ThisPage.CurrentWebsite.IsLoggedIn) this.imgLogo.Src = ThisPage.CurrentWebsite.LogoPath;
            //divBKLinks.Visible = ThisPage.CurrentWebsite.WebsiteID == "2"; //"26";

            if (ThisPage.CurrentWebsite.WebsiteUsefulLinks != null && ThisPage.CurrentWebsite.WebsiteUsefulLinks.Count > 0)
            {
                divUsefulLinks.Visible = true;
                rptUsefulLink.DataSource = ThisPage.CurrentWebsite.WebsiteUsefulLinks;
                rptUsefulLink.DataBind();
            }
            else
            {
                divUsefulLinks.Visible = false;
            }

            liSupportFAQ.Visible = ThisPage.CurrentWebsite.DisplaySupportFAQ;
            liContactUsAddress.Visible = ThisPage.CurrentWebsite.DisplayContactUsAddress;
            liContactUsPhoneNumber.Visible = ThisPage.CurrentWebsite.DisplayContactUsPhoneNumber;

            if (
                (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && ThisPage.CurrentWebsite.WebsiteID == "25")
                ||
                (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && ThisPage.CurrentWebsite.WebsiteID == "26")
                ||
                (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && ThisPage.CurrentWebsite.WebsiteID == "58")
            )
            {
                liContactUsPhoneNumber.Visible = false;
                liContactUsPhoneNumberMavis.Visible = true;
            }

            if (ThisPage.CurrentWebsite.HideReturnPolicy)
            {
                aReturnPolicy.Visible = false;
            }

            liNewAccountSetupForm.Visible = ThisPage.CurrentWebsite.DisplayNewAccountSetupForm;
        }
    }
}
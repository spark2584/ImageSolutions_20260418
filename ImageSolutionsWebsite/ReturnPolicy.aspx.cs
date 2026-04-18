using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class ReturnPolicy : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentWebsite != null &&
                ( (ConfigurationManager.AppSettings["Environment"] == "staging" && CurrentWebsite.WebsiteID == "53") || (ConfigurationManager.AppSettings["Environment"] == "production" && CurrentWebsite.WebsiteID == "20") )
            )
            {
                Response.Redirect("/ReturnPolicyRMA.aspx");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class PunchoutRedirect : BasePage
    {
        protected string mGUID = string.Empty;
        protected string mPunchoutGUID = string.Empty;
        protected string mPunchoutSession = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            mGUID = Request.QueryString.Get("website");
            mPunchoutSession = Request.QueryString.Get("session");
            mPunchoutGUID = Request.QueryString.Get("guid");

            Response.Redirect(String.Format("/punchout.aspx?website={0}&session={1}&guid={2}", mGUID, mPunchoutSession, mPunchoutGUID));
        }
    }
}
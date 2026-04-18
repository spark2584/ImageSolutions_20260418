using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class CreateGroup : BasePageAdminUserWebSiteAuth
    {
        private string mWebsiteID = string.Empty;
        private string mReturnWebsiteID = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString.Get("returnwebsiteid")))
            {
                mReturnWebsiteID = Request.QueryString.Get("returnwebsiteid");
            }

            if (!string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Request.Cookies["AdminWebsiteID"].Value)))
            {
                mWebsiteID = Convert.ToString(HttpContext.Current.Request.Cookies["AdminWebsiteID"].Value);
                GetWebsite();
            }
            else
            {
                Response.Redirect(String.Format("/Admin/GroupOverview.aspx"));
            }
        }

        protected void GetWebsite()
        {
            ImageSolutions.Website.Website Website = new ImageSolutions.Website.Website(mWebsiteID);
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                WebsiteGroup.WebsiteID = Convert.ToString(mWebsiteID);
                WebsiteGroup.GroupName = txtName.Text;
                WebsiteGroup.CreatedBy = CurrentUser.UserInfoID;
                WebsiteGroup.Create();

                if (!string.IsNullOrEmpty(mReturnWebsiteID))
                {
                    Response.Redirect(String.Format("/Admin/Website.aspx?id={0}", mReturnWebsiteID));
                }
                else
                {
                    Response.Redirect(String.Format("/Admin/GroupOverview.aspx?websiteid={0}", mWebsiteID));
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }            
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(mReturnWebsiteID))
            {
                Response.Redirect(String.Format("/Admin/Website.aspx?id={0}", mReturnWebsiteID));
            }

            Response.Redirect(String.Format("/Admin/GroupOverview.aspx"));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class UserOverview : BasePageAdminUserWebSiteAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindUser();
            }
        }
        protected void BindUser()
        {
            try
            {
                List<ImageSolutions.User.UserInfo> UserInfos = new List<ImageSolutions.User.UserInfo>();
                UserInfos = ImageSolutions.User.UserInfo.GetUserInfos();

                this.gvUser.DataSource = UserInfos;
                this.gvUser.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void gvUser_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "LineUpdate")
            {
                string strUserID = Convert.ToString(e.CommandArgument);
                Response.Redirect(String.Format("/Admin/User.aspx?id={0}", strUserID));
            }
        }
    }
}
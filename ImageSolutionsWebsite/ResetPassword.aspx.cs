using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class ResetPassword : BasePage
    {
        private string mPasswordResetGUID = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (CurrentWebsite != null && !string.IsNullOrEmpty(CurrentWebsite.WebsiteID) && !CurrentWebsite.EnablePasswordReset)
            {
                Response.Redirect("/Login.aspx");
            }


            if (!string.IsNullOrEmpty(Request.QueryString.Get("reset")))
            {
                mPasswordResetGUID = Request.QueryString.Get("reset");
            }
            else
            {
                Response.Redirect("/Login.aspx");
            }

            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(mPasswordResetGUID))
                {
                    ValidateUser();
                }
            }
        }

        protected void ValidateUser()
        {
            ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
            ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
            UserInfoFilter.PasswordResetGUID = new Database.Filter.StringSearch.SearchFilter();
            UserInfoFilter.PasswordResetGUID.SearchString = mPasswordResetGUID;
            UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

            if(UserInfo == null)
            {
                Response.Redirect("/Login.aspx");
            }
            //else if(DateTime.UtcNow > Convert.ToDateTime(UserInfo.PasswordResetRequestedOn).AddMinutes(10))
            //{
            //    Response.Redirect("/Login.aspx");
            //}
        }

        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (Convert.ToString(txtPassword.Text) != Convert.ToString(txtConfirmPassword.Text))
            {
                lblMessage.Text = "Confirm Password must match Password";
            }
            else
            {
                ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                UserInfoFilter.PasswordResetGUID = new Database.Filter.StringSearch.SearchFilter();
                UserInfoFilter.PasswordResetGUID.SearchString = mPasswordResetGUID;
                UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

                UserInfo.Password = Convert.ToString(txtPassword.Text);
                UserInfo.PasswordResetGUID = Convert.ToString(Guid.NewGuid());
                UserInfo.RequirePasswordReset = false;
                UserInfo.Update();

                Response.Redirect("/Login.aspx");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.MyAccount
{
    public partial class ChangePassword : BasePageUserAccountAuth
    {
        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/MyAccount/Dashboard.aspx";
                else
                    return ViewState["ReturnURL"].ToString();
            }
            set
            {
                ViewState["ReturnURL"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo(CurrentUser.UserInfoID);

                if (UserInfo.Password != txtPassword.Text)
                {
                    lblMessage.Text = "Invalid Password";

                    txtPassword.Text = String.Empty;
                    txtNewPassword.Text = String.Empty;
                    txtConfirmNewPassword.Text = String.Empty;
                }
                else if (Convert.ToString(txtNewPassword.Text) != Convert.ToString(txtConfirmNewPassword.Text))
                {
                    lblMessage.Text = "Confirm New Password must match the New Password";

                    txtPassword.Text = String.Empty;
                    txtNewPassword.Text = String.Empty;
                    txtConfirmNewPassword.Text = String.Empty;
                }
                else
                {
                    UserInfo.Password = Convert.ToString(txtNewPassword.Text);
                    blnReturn = UserInfo.Update();

                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }


            if (blnReturn)
            {
                Response.Redirect(ReturnURL);
            }            
        }
    }
}
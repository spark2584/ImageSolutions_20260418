using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class Passcode : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentUser.IsLoggedIn) Response.Redirect("/myaccount/dashboard.aspx");

            if(!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString.Get("email")))
                {
                    string strEmail = Convert.ToString(Request.QueryString.Get("email"));

                    if(ValidateEmail(strEmail))
                    {
                        txtEmail.Text = strEmail;
                        SendPasscode(strEmail);

                        btnSendPasscode.Text = "Send New Passcode";
                    }
                }
            }

            //if (string.IsNullOrEmpty(Request.QueryString.Get("id")))
            //{
            //    Response.Redirect("/login.aspx");
            //}
            //else
            //{
            //    ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
            //    ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
            //    UserInfoFilter.GUID = new Database.Filter.StringSearch.SearchFilter();
            //    UserInfoFilter.GUID.SearchString = Convert.ToString(Request.QueryString.Get("id"));
            //    UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

            //    txtEmail.Text = UserInfo.EmailAddress;
            //}
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lblMessage.Text = String.Empty;
            if (Page.IsValid)
            {
                ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                UserInfoFilter.EmailAddress.SearchString = txtEmail.Text;
                UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

                UserInfo.Passcode = RandomString(6);

                if (string.IsNullOrEmpty(Request.QueryString.Get("target")))
                {
                    Response.Redirect("/myaccount/dashboard.aspx");
                }
                else
                {
                    Response.Redirect(Request.QueryString.Get("target"));
                }
            }
        }
        protected void custValidLogin_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = CurrentUser.LoginPasscode(this.txtEmail.Text.Trim(), this.txtPasscode.Text.Trim());
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                args.IsValid = false;
            }
            finally { }
        }
        protected bool ValidateEmail(string emailaddress)
        {
            ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
            ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
            UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
            UserInfoFilter.EmailAddress.SearchString = emailaddress;
            UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

            return UserInfo != null && !string.IsNullOrEmpty(UserInfo.UserInfoID);
        }
        protected void btnSendPasscode_Click(object sender, EventArgs e)
        {
            lblMessage.Text = String.Empty;
            SendPasscode(txtEmail.Text);

            btnSendPasscode.Text = "Send New Passcode";
        }
        protected void SendPasscode(string emailaddress)
        {
            ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
            ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
            UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
            UserInfoFilter.EmailAddress.SearchString = emailaddress;
            UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

            if (UserInfo.AllowOnlySSO)
            {
                lblMessage.Text = String.Format(@"User is restricted to SSO login only");
            }
            else if (UserInfo != null)
            {
                string strPasscode = RandomString(6);
                UserInfo.Passcode = strPasscode = RandomString(6);
                UserInfo.PasscodeRequestedOn = DateTime.UtcNow;

                string strHTMLContent = string.Empty;
                strHTMLContent = String.Format(@"Hi {0},<br /><br />Your one time verification code is: <b>{1}</b><br /><br />{2}", string.Format(@"{0} {1}", UserInfo.FirstName, UserInfo.LastName), strPasscode, CurrentWebsite.Name);
                SendEmail(Convert.ToString(emailaddress), CurrentWebsite.Name + " Login Verification", strHTMLContent);
                UserInfo.Update();

                lblMessage.Text = String.Format(@"Passcode has been sent");
            }
            else
            {
                lblMessage.Text = String.Format(@"Invalid Email");
            }
        }
        protected string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        protected void txtEmail_TextChanged(object sender, EventArgs e)
        {
            lblMessage.Text = String.Empty;
        }

        protected void txtPasscode_TextChanged(object sender, EventArgs e)
        {
            lblMessage.Text = String.Empty;
        }
    }
}
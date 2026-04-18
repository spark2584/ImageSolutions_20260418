using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class ForgetPassword : BasePageUserNoAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CurrentWebsite.EnablePasswordReset)
            {
                Response.Redirect("/Login.aspx");
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {

                ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                UserInfoFilter.EmailAddress.SearchString = txtEmail.Text;
                UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

                if (UserInfo == null)
                {
                    throw new Exception("Invalid Email");
                }
                else if (UserInfo.AllowOnlySSO)
                {
                    throw new Exception("User is restricted to SSO login only");
                }
                else
                {
                    UserInfo.PasswordResetGUID = Convert.ToString(Guid.NewGuid());
                    UserInfo.PasswordResetRequestedOn = DateTime.UtcNow;

                    string strHTMLContent = string.Empty;
                    //strHTMLContent = string.Format(@"<a clicktracking=""off"" href=""{0}"" target=""_blank"">RESET PASSWORD</a>", String.Format(@"{0}/ResetPassword.aspx?reset={1}", Convert.ToString(ConfigurationManager.AppSettings["WebsiteURL"]), UserInfo.PasswordResetGUID));
                    strHTMLContent = String.Format(@"
Hi {0},<br /><br />There was a request to change your password!<br /><br />If you did not make this request then please ignore this email.<br /><br />Otherwise, please click this link to change your password:<br /> <a clicktracking=""off"" href=""{1}"" target=""_blank"">RESET PASSWORD</a>
"
                        , string.Format(@"{0} {1}", UserInfo.FirstName, UserInfo.LastName) 
                        , String.Format(@"{0}/ResetPassword.aspx?reset={1}"
                            , string.IsNullOrEmpty(CurrentWebsite.Domain) ? Convert.ToString(ConfigurationManager.AppSettings["WebsiteURL"]) : "https://" + CurrentWebsite.Domain
                            , UserInfo.PasswordResetGUID) 
                    );

                    string strEmail = UserInfo.EmailAddress;
                    ImageSolutions.User.UserWebsite UserWebsite = UserInfo.UserWebsites.Find(x => x.WebsiteID == CurrentWebsite.WebsiteID);
                    if (UserWebsite != null && !string.IsNullOrEmpty(UserWebsite.NotificationEmail))
                    {
                        strEmail = UserWebsite.NotificationEmail;
                    }

                    SendEmail(strEmail, CurrentWebsite.Name + " Password Reset", strHTMLContent);

                    lblMessage.Text = "Email has been sent";
                    UserInfo.Update();

                    pnlSendEmail.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = ex.Message; //"Invalid Email";
            }
            finally { }

        }

        protected void txtEmail_TextChanged(object sender, EventArgs e)
        {
            lblMessage.ForeColor = System.Drawing.Color.Black;
            lblMessage.Text = String.Empty;
        }
    }
}
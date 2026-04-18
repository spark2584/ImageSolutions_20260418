using Microsoft.Owin.Security;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class Login : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string strDomainName = Request.Url.Host;
            if (strDomainName.Contains("bpuniform") && Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production")
            {
                CurrentWebsite.Login("75560411-66D8-4C34-8625-1CE7B17CB5DC");
            }

            //If Cookie["UserGuid"} != null
            //Load UserUnfo based on UserGUID Cookie
            //Display hey objUserInfo.FirstName  you are arealdy logged in

            if (CurrentUser.IsLoggedIn && !CurrentUser.IsGuest) Response.Redirect("/myaccount/dashboard.aspx?login=t");

            if (CurrentWebsite.IsLoggedIn)
            {
                //this.btnRegister.Visible = CurrentWebsite.UserRegistration;
                this.pnlUserRegistrationEnabled.Visible = CurrentWebsite.UserRegistration;
                this.pnlAccountRegistrationEnabled.Visible = CurrentWebsite.AccountRegistration;

                if (!string.IsNullOrEmpty(CurrentWebsite.RegistrationFormPath))
                {
                    this.pnlRegistrationFormEnabled.Visible = true;
                    aRegistrationForm.HRef = String.Format(CurrentWebsite.RegistrationFormPath);
                }

                this.divRegistartion.Visible = pnlUserRegistrationEnabled.Visible || pnlAccountRegistrationEnabled.Visible || pnlRegistrationFormEnabled.Visible;

            }

            if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production")
            {
                switch (CurrentWebsite.WebsiteID)
                {
                    case "2": //Burger King
                        lblCreateUser.Text = "Create a User Account";
                        lblBtnCreateUser.Text = "Create a User Account";
                        lblCreateAccount.Text = "Create a Sub-Account";
                        lblBtnCreateAccount.Text = "Create a Sub-Account";
                        break;
                    case "5": //Securitas
                        lblCreateUser.Text = "User Registration";
                        lblBtnCreateUser.Text = "User Registration";
                        lblCreateAccount.Text = "AVP Registration";
                        lblBtnCreateAccount.Text = "AVP Registration";
                        break;
                    case "7": //ABS Companies
                        pnlWebsiteMessage.Visible = true;
                        litWebsiteMessage.Text = "Welcome to the Albertsons Companies internal uniform ordering website!";
                        break;
                    case "17":
                        btnLoginPasscode.Visible = false;
                        break;
                    default:
                        break;
                }                
            }
            else
            {
                switch (CurrentWebsite.WebsiteID)
                {
                    case "26": //Burger King
                        lblCreateUser.Text = "Create A User Account";
                        lblBtnCreateUser.Text = "Create A User Account";
                        lblCreateAccount.Text = "Create A Sub-Account";
                        lblBtnCreateAccount.Text = "Create An Sub-Account";
                        break;
                    case "42": //Securitas
                        lblCreateUser.Text = "User Registration";
                        lblBtnCreateUser.Text = "User Registration";
                        lblCreateAccount.Text = "AVP Registration";
                        lblBtnCreateAccount.Text = "AVP Registration";
                        btnLoginPasscode.Visible = false;
                        break;
                    case "43": //ABS Companies
                        pnlWebsiteMessage.Visible = true;
                        litWebsiteMessage.Text = "Welcome to the Albertsons Companies internal uniform ordering website!";
                        break;
                    default:
                        break;
                }
            }
          
            pnlForgotPassword.Visible = string.IsNullOrEmpty(CurrentWebsite.WebsiteID) || CurrentWebsite.EnablePasswordReset;
            if (!string.IsNullOrEmpty(CurrentWebsite.PasswordHint))
            {
                pnlPasswordHint.Visible = true;
                litPasswordHint.Text = CurrentWebsite.PasswordHint;
            }

            if(!string.IsNullOrEmpty(CurrentWebsite.RegistrationPath))
            {
                btnUserRegister.HRef = CurrentWebsite.RegistrationPath;
            }

            btnSamlLogin.Visible = CurrentWebsite.EnableSSO;

            //Brinker
            if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "30"
                || Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && CurrentWebsite.WebsiteID == "64")
            {
                lblLoginHeader.Text = "Franchisee Login";
                lblAlternativeSSOLoginHeader.Text = "Corporate Login";
                divRegistartion.Visible = false;
                divAlternativeLogin.Visible = true;

                btnSamlLogin.Visible = false;
                btnAlternativeSSOButton.Text = "Login with Brinker SSO";
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                UserInfoFilter.EmailAddress.SearchString = this.txtEmail.Text.Trim();
                UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

                if (UserInfo != null && UserInfo.AllowOnlySSO)
                {
                    if (CurrentUser.IsLoggedIn)
                    {
                        CurrentUser.Logout();
                        CurrentUser.CurrentUserWebSite.Logout();
                        CurrentUser.CurrentUserWebSite.CurrentUserAccount.Logout();
                    }

                    throw new Exception("User is restricted to SSO login only");
                }

                if (UserInfo != null && UserInfo.RequirePasswordReset)
                {
                    ResetPassword(UserInfo);
                }
                else if (Page.IsValid)
                {


                    if (HttpContext.Current.Response.Cookies["ISAdminUserGUID"] != null)
                    {
                        HttpContext.Current.Response.Cookies["ISAdminUserGUID"].Expires = System.DateTime.Now.AddDays(-1);
                    }


                    if (string.IsNullOrEmpty(Request.QueryString.Get("target")))
                    {
                        if (!string.IsNullOrEmpty(CurrentWebsite.StartingPath))
                        {
                            Response.Redirect(CurrentWebsite.StartingPath);
                        }
                        else
                        {
                            Response.Redirect("/myaccount/dashboard.aspx?login=t");
                        }
                    }
                    else
                    {
                        Response.Redirect(Request.QueryString.Get("target"));
                    }
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }

        }
        protected void ResetPassword(ImageSolutions.User.UserInfo userinfo)
        {
            ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
            ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
            UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
            UserInfoFilter.EmailAddress.SearchString = userinfo.EmailAddress;
            UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

            UserInfo.PasswordResetGUID = Convert.ToString(Guid.NewGuid());
            UserInfo.PasswordResetRequestedOn = DateTime.UtcNow;

            string strHTMLContent = string.Empty;
            strHTMLContent = String.Format(@"
Hi {0},<br /><br />Password reset required.<br /><br />Please click this link to change your password:<br /> <a clicktracking=""off"" href=""{1}"" target=""_blank"">RESET PASSWORD</a>
"
                , string.Format(@"{0} {1}", UserInfo.FirstName, UserInfo.LastName)
                , String.Format(@"{0}/ResetPassword.aspx?reset={1}", Convert.ToString(ConfigurationManager.AppSettings["WebsiteURL"]), UserInfo.PasswordResetGUID)
            );
            //SendEmail(Convert.ToString(userinfo.EmailAddress), CurrentWebsite.Name + " Password Reset" + DateTime.ParseExact(DateTime.UtcNow.ToString(), "DD/MM/YYYY hh:mm:ss", CultureInfo.InvariantCulture) + " (UTC)", strHTMLContent);

            string strEmail = userinfo.EmailAddress;
            ImageSolutions.User.UserWebsite UserWebsite = userinfo.UserWebsites.Find(x => x.WebsiteID == CurrentWebsite.WebsiteID);
            if (UserWebsite != null && !string.IsNullOrEmpty(UserWebsite.NotificationEmail))
            {
                strEmail = UserWebsite.NotificationEmail;
            }
            SendEmail(strEmail, string.Format("{0} Password Reset - {1:MM/dd/yyyy HH:mm:ss} (UTC)", CurrentWebsite.Name, DateTime.UtcNow), strHTMLContent);
            UserInfo.Update();
        }

        protected void custValidLogin_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = CurrentUser.Login(this.txtEmail.Text.Trim(), this.txtPassword.Text.Trim());
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                args.IsValid = false;
            }
            finally { }
        }

        protected void btnLoginPasscode_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtEmail.Text))
                {
                    Response.Redirect(string.Format("/Passcode.aspx?email={0}", txtEmail.Text));
                }
                else
                {
                    Response.Redirect(string.Format("/Passcode.aspx"));
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }

        }

        protected async void btnTestPayment_Click(object sender, EventArgs e)
        {
            var username = "vincent@imageinc.com";
            var password = "ImageSolutions$1";

            var base64Auth = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{username}:{password}")
            );

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://rest.avatax.com/");
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Auth);

                var endpoint = "api/v2/companies/DEFAULT/transactions/64445889";

                try
                {
                    var response = await client.GetAsync(endpoint);

                    var result = await response.Content.ReadAsStringAsync();

                }
                catch (Exception ex)
                {

                }
            }
        }

        protected void btnSamlLogin_Click(object sender, EventArgs e)
        {
            //Context.GetOwinContext().Authentication.Challenge(new AuthenticationProperties
            //{
            //    RedirectUri = "/"
            //}, "Saml2");

            // Enterprise
            if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "20"
                || Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && CurrentWebsite.WebsiteID == "53")
            {
                AuthenticationProperties props = new AuthenticationProperties
                {
                    RedirectUri = "/"
                };
                props.Dictionary["idp"] = String.Format("{0}", ConfigurationManager.AppSettings["SSOIdentityProvider"]); //"https://sts.windows.net/5a9bb941-ba53-48d3-b086-2927fea7bf01/";

                Context.GetOwinContext().Authentication.Challenge(
                    props,
                    "Saml2");
            }
            else
            {
                //Ping One
                AuthenticationProperties props = new AuthenticationProperties
                {
                    RedirectUri = "/"
                };
                props.Dictionary["idp"] = String.Format("{0}", ConfigurationManager.AppSettings["SSOIdentityProviderPingOne"]);  //"https://sso.connect.pingidentity.com/sso/idp/SSO.saml2?idpid=<your-idpid>";

                Context.GetOwinContext().Authentication.Challenge(
                    props,
                    "Saml2");
            }
        }
    }
}
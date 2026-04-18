using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace ImageSolutionsWebsite.RegistrationPage
{
    public partial class AholdEmployeePurchase : BasePageUserNoAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CurrentWebsite.IsLoggedIn || !CurrentWebsite.UserRegistration) Response.Redirect("/login.aspx");

            if (!Page.IsPostBack)
            {
                Initialize();
            }
        }

        protected void Initialize()
        {
            BindAccount();
        }

        protected void BindAccount()
        {
            List<ImageSolutions.Account.Account> Accounts = new List<ImageSolutions.Account.Account>();
            ImageSolutions.Account.AccountFilter AccountFilter = new ImageSolutions.Account.AccountFilter();
            AccountFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();

            if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging")
            {
                AccountFilter.ParentID.SearchString = "8504";
            }
            else
            {
                AccountFilter.ParentID.SearchString = "6642";
            }
            Accounts = ImageSolutions.Account.Account.GetAccounts(AccountFilter);

            this.ddlAccount.DataSource = Accounts.OrderBy(m => m.AccountName);
            this.ddlAccount.DataBind();
            this.ddlAccount.Items.Insert(0, new ListItem("", ""));
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;
            bool blnReturn = false;
            bool blnIsExistingUser = false;

            ImageSolutions.User.UserInfo UserInfo = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();


                ImageSolutions.User.UserInfo ExistUserInfo = new ImageSolutions.User.UserInfo();
                ImageSolutions.User.UserInfoFilter ExistUserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                ExistUserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                ExistUserInfoFilter.EmailAddress.SearchString = txtEmail.Text;
                ExistUserInfo = ImageSolutions.User.UserInfo.GetUserInfo(ExistUserInfoFilter);

                string strUserInfoID = string.Empty;
                string strMessage = string.Empty;

                if (ExistUserInfo != null && !string.IsNullOrEmpty(ExistUserInfo.UserInfoID))
                {
                    ImageSolutions.User.UserWebsite ExistUserWebsite = new ImageSolutions.User.UserWebsite();
                    ImageSolutions.User.UserWebsiteFilter ExistUserWebSiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                    ExistUserWebSiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    ExistUserWebSiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                    ExistUserWebSiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                    ExistUserWebSiteFilter.UserInfoID.SearchString = ExistUserInfo.UserInfoID;
                    ExistUserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(ExistUserWebSiteFilter);

                    if (ExistUserWebsite != null && !string.IsNullOrEmpty(ExistUserWebsite.UserWebsiteID))
                    {
                        throw new Exception("User already exists");
                    }
                    else
                    {
                        strMessage = String.Format("User {0} already exists.  User has been added to {1}.", txtEmail.Text, CurrentWebsite.Name);
                        blnIsExistingUser = true;
                    }

                    strUserInfoID = ExistUserInfo.UserInfoID;
                }
                else
                {
                    UserInfo = new ImageSolutions.User.UserInfo();
                    UserInfo.FirstName = txtFirstName.Text;
                    UserInfo.LastName = txtLastName.Text;
                    UserInfo.EmailAddress = txtEmail.Text;
                    UserInfo.Password = txtPassword.Text.Trim();
                    UserInfo.Create(objConn, objTran);

                    strUserInfoID = UserInfo.UserInfoID;
                }

                //UserInfo = new ImageSolutions.User.UserInfo();
                //UserInfo.FirstName = txtFirstName.Text;
                //UserInfo.LastName = txtLastName.Text;
                //UserInfo.EmailAddress = txtEmail.Text;
                //UserInfo.Password = txtPassword.Text.Trim();
                //UserInfo.Create(objConn, objTran);

                ImageSolutions.User.UserWebsite objUserWebsite = new ImageSolutions.User.UserWebsite();
                objUserWebsite.UserInfoID = strUserInfoID; //UserInfo.UserInfoID;
                objUserWebsite.WebsiteID = CurrentWebsite.WebsiteID;
                objUserWebsite.IsAdmin = false;
                objUserWebsite.OptInForNotification = true;

                objUserWebsite.CreatedBy = strUserInfoID; //UserInfo.UserInfoID;
                blnReturn = objUserWebsite.Create(objConn, objTran);

                ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account(ddlAccount.SelectedValue);

                ImageSolutions.User.UserAccount objUserAccount = new ImageSolutions.User.UserAccount();
                objUserAccount.UserWebsiteID = objUserWebsite.UserWebsiteID;
                objUserAccount.AccountID = ddlAccount.SelectedValue;
                objUserAccount.WebsiteGroupID = String.IsNullOrEmpty(Account.DefaultWebsiteGroupID) ? CurrentWebsite.DefaultWebsiteGroup.WebsiteGroupID : Account.DefaultWebsiteGroupID;
                objUserAccount.CreatedBy = strUserInfoID; //UserInfo.UserInfoID;
                blnReturn = objUserAccount.Create(objConn, objTran);

                //if (blnReturn && CurrentWebsite.UserApprovalRequired)
                //{
                //    SendUserRegistration(objAccount, objUserWebsite);
                //}

                objTran.Commit();
            }
            catch (Exception ex)
            {
                objTran.Rollback();
                blnReturn = false;

                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;

                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }



            if (blnReturn)
            {
                if (blnIsExistingUser)
                {
                    Response.Redirect("/RegistrationExistingUser.aspx");
                }
                else
                {
                    Response.Redirect("/RegistrationComplete.aspx");
                }
            }
        }

        protected bool SendUserRegistration(ImageSolutions.Account.Account account, ImageSolutions.User.UserWebsite userwebsite)
        {
            try
            {
                List<string> SentEmails = new List<string>();

                foreach (ImageSolutions.User.UserAccount _UserAccount in account.UserAccounts)
                {

                    string strHTMLContent = @"<!DOCTYPE html>
                                    <html>
                                        <head></head>
                                        <body>
                                            <div style='font-size:14px;padding-bottom:30px;height:100%;position:relative;'>
                                                <div style='float:right;width:45%; font-weight:bold; font-size:20px; text-align:right; padding-top:20px;'></div>

                                                <div style='float:left; width:45%;'><img src='${Logo}' style='width:2.5in; height:0.76in ' /></div>

                                                <div style='clear:left;padding-top:40px;'>
                                                    <div style='font-size:16px; font-weight:bold;'>Hi ${FirstName},</div>
                                                    <p>A user account has been registered by ${AccountFullName}, please <a href='${ApprovalURL}'>login</a> to the portal to review and approve the subaccount registration.</p>
                                                </div>
                                            </div>
                                        </body>
                                    </html>";

                    strHTMLContent = strHTMLContent.Replace("${Logo}", string.IsNullOrEmpty(account.Website.EmailLogoPath) ? account.Website.LogoPath : account.Website.EmailLogoPath);
                    strHTMLContent = strHTMLContent.Replace("${FirstName}", _UserAccount.UserWebsite.UserInfo.FirstName);
                    strHTMLContent = strHTMLContent.Replace("${AccountFullName}", userwebsite.UserInfo.FullName);
                    strHTMLContent = strHTMLContent.Replace("${ApprovalURL}", WebUtility.PageSURL("/admin/userwebsite.aspx?id=" + userwebsite.UserWebsiteID));

                    if (_UserAccount.UserWebsite.IsAdmin && _UserAccount.UserWebsite.UserManagement && _UserAccount.UserWebsite.OptInForNotification)
                    {
                        if (!SentEmails.Contains(_UserAccount.UserWebsite.UserInfo.EmailAddress))
                        {
                            SendEmail(_UserAccount.UserWebsite.UserInfo.EmailAddress, CurrentWebsite.Name + " User Account Registration", strHTMLContent);
                            SentEmails.Add(_UserAccount.UserWebsite.UserInfo.EmailAddress);
                        }
                    }
                }
            }
            catch { }
            finally { }
            return true;
        }
    }
}
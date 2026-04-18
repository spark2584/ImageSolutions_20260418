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
    public partial class AlbertsonsGear : BasePageUserNoAuth
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
                AccountFilter.ParentID.SearchString = "5393";
            }
            else
            {
                AccountFilter.ParentID.SearchString = "4929";
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

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                UserInfo.FirstName = txtFirstName.Text;
                UserInfo.LastName = txtLastName.Text;
                UserInfo.EmailAddress = txtEmail.Text;
                UserInfo.Password = txtPassword.Text.Trim();
                UserInfo.Create(objConn, objTran);

                ImageSolutions.User.UserWebsite objUserWebsite = new ImageSolutions.User.UserWebsite();
                objUserWebsite.UserInfoID = UserInfo.UserInfoID;
                objUserWebsite.WebsiteID = CurrentWebsite.WebsiteID;
                objUserWebsite.IsAdmin = false;
                //objUserWebsite.IsPendingApproval = CurrentWebsite.UserApprovalRequired;
                objUserWebsite.OptInForNotification = true;

                objUserWebsite.CreatedBy = UserInfo.UserInfoID;
                blnReturn = objUserWebsite.Create(objConn, objTran);

                ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account(ddlAccount.SelectedValue);

                ImageSolutions.User.UserAccount objUserAccount = new ImageSolutions.User.UserAccount();
                objUserAccount.UserWebsiteID = objUserWebsite.UserWebsiteID;
                objUserAccount.AccountID = ddlAccount.SelectedValue;
                objUserAccount.WebsiteGroupID = String.IsNullOrEmpty(Account.DefaultWebsiteGroupID) ? CurrentWebsite.DefaultWebsiteGroup.WebsiteGroupID : Account.DefaultWebsiteGroupID;
                objUserAccount.CreatedBy = UserInfo.UserInfoID;
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
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }

            if (blnReturn) Response.Redirect("/RegistrationComplete.aspx");
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
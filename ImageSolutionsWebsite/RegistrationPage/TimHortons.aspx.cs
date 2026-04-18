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
    public partial class TimHortons : BasePageUserNoAuth
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
                AccountFilter.ParentID.SearchString = "69802";
            }
            else
            {
                AccountFilter.ParentID.SearchString = "107311";
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

                if (ddlType.SelectedValue == "FranchiseeOwner")
                {
                    objUserWebsite.IsAdmin = true;
                    objUserWebsite.OrderManagement = true;
                    objUserWebsite.IsPendingApproval = true;
                }

                if (ddlType.SelectedValue == "Store")
                {
                    if (txtRestNo.Text == String.Empty)
                    {
                        throw new Exception("'Rest No' is required");
                    }

                    objUserWebsite.EmployeeID = txtRestNo.Text;
                }

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

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlRestNo.Visible = ddlType.SelectedValue == "Store";
        }
    }
}
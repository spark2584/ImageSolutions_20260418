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
    public partial class ElPolloLoco : BasePageUserNoAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CurrentWebsite.IsLoggedIn || !CurrentWebsite.UserRegistration) Response.Redirect("/login.aspx");


            if (
                (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID != "17")
                ||
                (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID != "59")
            )
            {
                Response.Redirect("/login.aspx");
            }

            if (!Page.IsPostBack)
            {
                Initialize();
            }
        }
        protected void Initialize()
        {
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string strAccountID = string.Empty;
            string strGroupID = string.Empty;

            strAccountID = Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" ? "107752" : "69738"; //El Pollo Loco Branded Items
            strGroupID = Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" ? "278" : "292"; //El Pollo Loco Branded Items

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
                objUserWebsite.IsPendingApproval = false;
                objUserWebsite.OptInForNotification = true;

                objUserWebsite.CreatedBy = UserInfo.UserInfoID;
                blnReturn = objUserWebsite.Create(objConn, objTran);


                ImageSolutions.User.UserAccount objUserAccount = new ImageSolutions.User.UserAccount();
                objUserAccount.UserWebsiteID = objUserWebsite.UserWebsiteID;
                objUserAccount.AccountID = strAccountID;
                objUserAccount.WebsiteGroupID = strGroupID;
                objUserAccount.CreatedBy = UserInfo.UserInfoID;
                blnReturn = objUserAccount.Create(objConn, objTran);

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
    }
}
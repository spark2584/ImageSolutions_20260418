using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class AccountRegistrationComplete : BasePageUserNoAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CurrentWebsite.IsLoggedIn || !CurrentWebsite.AccountRegistration) Response.Redirect("/login.aspx");

            if (!Page.IsPostBack)
            {
                Initialize();
            }
        }

        protected void Initialize()
        {
            this.phRegistrationApproval.Visible = CurrentWebsite.AccountApprovalRequired;

            if (
                (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "2")
                ||
                (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "26")
            )
            {
                lblHeader.Text = "THANK YOU!";
                lblMessage.Text = "Your Franchisee Owner Account(FOA) will receive an email notification of your request and once approved, you can then login to your account.";

                btnLogin.Visible = false;
            }
        }
    }
}
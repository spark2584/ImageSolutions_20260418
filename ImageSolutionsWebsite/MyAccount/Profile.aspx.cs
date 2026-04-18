using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.MyAccount
{
    public partial class Profile : BasePageUserAccountAuth
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindProfile();
            }

            txtFirstName.Enabled = CurrentWebsite.AllowNameChange;
            txtLastName.Enabled = CurrentWebsite.AllowNameChange;

            pnlEmail.Visible = !CurrentWebsite.HideEmail;

            txtNotificationEmailAddress.Enabled = !CurrentUser.CurrentUserWebSite.DisableNotificationEmail;
        }

        protected void BindProfile()
        {
            try
            {
                txtFirstName.Text = string.Format("{0}", CurrentUser.FirstName);
                txtLastName.Text = string.Format("{0}", CurrentUser.LastName);
                txtEmailAddress.Text = CurrentUser.EmailAddress;
                txtNotificationEmailAddress.Text = CurrentUser.CurrentUserWebSite.NotificationEmail.Trim();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CurrentUser.FirstName = txtFirstName.Text;
                CurrentUser.LastName = txtLastName.Text;
                CurrentUser.EmailAddress = txtEmailAddress.Text;
                CurrentUser.Update();

                CurrentUser.CurrentUserWebSite.NotificationEmail = txtNotificationEmailAddress.Text.Trim();
                CurrentUser.CurrentUserWebSite.Update();

                Response.Redirect("/myaccount/dashboard.aspx");
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
    }
}
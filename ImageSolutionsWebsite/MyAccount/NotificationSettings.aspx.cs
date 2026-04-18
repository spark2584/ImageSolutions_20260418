using ImageSolutionsWebsite.Library;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.MyAccount
{
    public partial class NotificationSettings : BasePageUserAccountAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindProfile();
            }
        }

        protected void BindProfile()
        {
            try
            {
                this.phEnableEmailOptIn.Visible = CurrentWebsite.EnableEmailOptIn || CurrentUser.CurrentUserWebSite.EnableEmailOptIn;
                this.phEnableSMSOptIn.Visible = CurrentWebsite.EnableSMSOptIn || CurrentUser.CurrentUserWebSite.EnableSMSOptIn;

                this.txtNotificationEmail.Text = CurrentUser.CurrentUserWebSite.NotificationEmail;
                this.txtMobileNumber.Text = CurrentUser.CurrentUserWebSite.SMSMobileNumber;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    CurrentUser.CurrentUserWebSite.NotificationEmail = this.txtNotificationEmail.Text.Trim();
                    CurrentUser.CurrentUserWebSite.SMSMobileNumber = this.txtMobileNumber.Text.Trim();
                    CurrentUser.CurrentUserWebSite.Update();

                    Response.Redirect("/myaccount/dashboard.aspx");
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(this, ex.Message);
                }
            }
        }

        protected void cvdOptIn_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //if (CurrentWebsite.EnableSMSOptIn && CurrentWebsite.EnableEmailOptIn)
            //{
            //    if (string.IsNullOrEmpty(txtNotificationEmail.Text.Trim()) && string.IsNullOrEmpty(txtMobileNumber.Text.Trim()))
            //    {
            //        args.IsValid = false;
            //        cvdOptIn.ErrorMessage = "Please enter at least an email or mobile phone number";
            //    }
            //}
            //else if (CurrentWebsite.EnableSMSOptIn)
            //{
            //    if (string.IsNullOrEmpty(txtMobileNumber.Text.Trim()))
            //    {
            //        args.IsValid = false;
            //        cvdOptIn.ErrorMessage = "Please enter a 10-digit phone number";
            //    }
            //}
            //else if (CurrentWebsite.EnableEmailOptIn)
            //{
            //    if (string.IsNullOrEmpty(txtNotificationEmail.Text.Trim()))
            //    {
            //        args.IsValid = false;
            //        cvdOptIn.ErrorMessage = "Please enter an email address";
            //    }
            //}
        }
    }
}
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Control
{
	public partial class OptIn : BaseControl
    {
		protected void Page_Load(object sender, EventArgs e)
		{
            bool blnDisplayOptIn = false;

            if (!Page.IsPostBack)
            {
                this.Visible = false;

                if (ThisPage.CurrentWebsite.IsLoggedIn && ThisPage.CurrentUser.CurrentUserWebSite.IsLoggedIn && HttpContext.Current.Request.Cookies["OptIn"] == null && !Request.Url.ToString().ToLower().Contains("login.aspx"))
                {
                    if ((ThisPage.CurrentWebsite.EnableEmailOptIn && ThisPage.CurrentWebsite.EnableSMSOptIn) || (ThisPage.CurrentUser.CurrentUserWebSite.EnableEmailOptIn && ThisPage.CurrentUser.CurrentUserWebSite.EnableSMSOptIn))
                    {
                        if (!ThisPage.CurrentUser.CurrentUserWebSite.SMSOptIn && !ThisPage.CurrentUser.CurrentUserWebSite.EmailOptIn) blnDisplayOptIn = true;
                    }
                    else if (ThisPage.CurrentWebsite.EnableEmailOptIn || ThisPage.CurrentUser.CurrentUserWebSite.EnableEmailOptIn)
                    {
                        if (!ThisPage.CurrentUser.CurrentUserWebSite.EmailOptIn) blnDisplayOptIn = true;
                    }
                    else if (ThisPage.CurrentWebsite.EnableSMSOptIn || ThisPage.CurrentUser.CurrentUserWebSite.EnableSMSOptIn)
                    {
                        if (!ThisPage.CurrentUser.CurrentUserWebSite.SMSOptIn) blnDisplayOptIn = true;
                    }

                    if (blnDisplayOptIn)
                    {
                        BindProfile();
                        this.divForm.Visible = true;
                        this.divThankyou.Visible = false;
                        this.Visible = true;
                    }
                    HttpContext.Current.Response.Cookies["OptIn"].Value = "1";

                    BindProfile();
                }
            }
        }

        protected void BindProfile()
        {
            try
            {
                this.phEnableEmailOptIn.Visible = ThisPage.CurrentWebsite.EnableEmailOptIn || ThisPage.CurrentUser.CurrentUserWebSite.EnableEmailOptIn;
                this.phEnableSMSOptIn.Visible = ThisPage.CurrentWebsite.EnableSMSOptIn || ThisPage.CurrentUser.CurrentUserWebSite.EnableSMSOptIn;

                this.txtNotificationEmail.Text = ThisPage.CurrentUser.CurrentUserWebSite.NotificationEmail;
                this.txtMobileNumber.Text = ThisPage.CurrentUser.CurrentUserWebSite.SMSMobileNumber;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(ThisPage, ex.Message);
            }
        }

        protected void btnOptIn_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    ThisPage.CurrentUser.CurrentUserWebSite.NotificationEmail = this.txtNotificationEmail.Text.Trim();
                    ThisPage.CurrentUser.CurrentUserWebSite.SMSMobileNumber = this.txtMobileNumber.Text.Trim();
                    ThisPage.CurrentUser.CurrentUserWebSite.Update();
                    this.divForm.Visible = false;
                    this.divThankyou.Visible = true;

                    if (!string.IsNullOrEmpty(ThisPage.CurrentUser.CurrentUserWebSite.NotificationEmail) && !string.IsNullOrEmpty(ThisPage.CurrentUser.CurrentUserWebSite.SMSMobileNumber))
                    {
                        this.litThankyou.Text = "You're all set! Thanks for opting in! We'll send your order confirmations, tracking updates and budget alerts by text/email. Reply STOP/UNSUBSCRIBE to opt out.";
                    }
                    else if (!string.IsNullOrEmpty(ThisPage.CurrentUser.CurrentUserWebSite.NotificationEmail))
                    {
                        this.litThankyou.Text = "You're all set! Thanks for opting in! We'll send your order confirmations, tracking updates and budget alerts by email. Click UNSUBSCRIBE to opt out.";
                    }
                    else if (!string.IsNullOrEmpty(ThisPage.CurrentUser.CurrentUserWebSite.SMSMobileNumber))
                    {
                        this.litThankyou.Text = "You're all set! Thanks for opting in! We'll send your order confirmations, tracking updates and budget alerts by text. Reply STOP to opt out.";
                    }
                    //Response.Redirect("/myaccount/dashboard.aspx");
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(ThisPage, ex.Message);
                }
            }
        }

        protected void cvdOptIn_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (ThisPage.CurrentWebsite.EnableSMSOptIn && ThisPage.CurrentWebsite.EnableEmailOptIn)
            {
                if (string.IsNullOrEmpty(txtNotificationEmail.Text.Trim()) && string.IsNullOrEmpty(txtMobileNumber.Text.Trim()))
                {
                    args.IsValid = false;
                    cvdOptIn.ErrorMessage = "Please enter at least an email or mobile phone number";
                }
            }
            else if (ThisPage.CurrentWebsite.EnableSMSOptIn)
            {
                if (string.IsNullOrEmpty(txtMobileNumber.Text.Trim()))
                {
                    args.IsValid = false;
                    cvdOptIn.ErrorMessage = "Please enter a 10-digit phone number";
                }
            }
            else if (ThisPage.CurrentWebsite.EnableEmailOptIn)
            {
                if (string.IsNullOrEmpty(txtNotificationEmail.Text.Trim()))
                {
                    args.IsValid = false;
                    cvdOptIn.ErrorMessage = "Please enter an email address";
                }
            }
        }
    }
}
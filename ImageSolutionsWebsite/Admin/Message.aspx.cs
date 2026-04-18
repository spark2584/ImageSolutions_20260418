using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class Message : BasePageAdminUserWebSiteAuth
    {
        private string mMessageID = string.Empty;

        private ImageSolutions.Website.WebsiteMessage mWebsiteMessage = null;
        protected ImageSolutions.Website.WebsiteMessage _WebsiteMessage
        {
            get
            {
                if (mWebsiteMessage == null)
                {
                    if (string.IsNullOrEmpty(mMessageID))
                        mWebsiteMessage = new ImageSolutions.Website.WebsiteMessage();
                    else
                        mWebsiteMessage = new ImageSolutions.Website.WebsiteMessage(mMessageID);
                }
                return mWebsiteMessage;
            }
            set
            {
                mWebsiteMessage = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/MessageOverview.aspx";
                else
                    return ViewState["ReturnURL"].ToString();
            }
            set
            {
                ViewState["ReturnURL"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            mMessageID = Request.QueryString.Get("id");

            if (!Page.IsPostBack)
            {
                InitializePage();
            }
        }

        public void InitializePage()
        {
            try
            {
                if (!_WebsiteMessage.IsNew)
                {
                    txtSubject.Text = _WebsiteMessage.Subject;
                    txtStartDate.Text = _WebsiteMessage.StartDate.ToShortDateString();
                    txtEndDate.Text = _WebsiteMessage.EndDate.ToShortDateString();
                    txtMessage.Text = _WebsiteMessage.Message.ToString();
                    cbIsAnnouncement.Checked = Convert.ToBoolean(_WebsiteMessage.IsAnnouncement);
                    cbIsNotification.Checked = Convert.ToBoolean(_WebsiteMessage.IsNotification);
                    btnSave.Text = "Save";
                }
                else
                {
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                }
                aCancel.HRef = ReturnURL;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                _WebsiteMessage.Subject = txtSubject.Text;
                _WebsiteMessage.StartDate = Convert.ToDateTime(txtStartDate.Text.Trim());
                _WebsiteMessage.EndDate = Convert.ToDateTime(txtEndDate.Text.Trim());
                _WebsiteMessage.Message = txtMessage.Text.Trim();
                _WebsiteMessage.IsAnnouncement = cbIsAnnouncement.Checked;
                _WebsiteMessage.IsNotification = cbIsNotification.Checked;

                if (_WebsiteMessage.IsNew)
                {
                    _WebsiteMessage.WebsiteID = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;
                    _WebsiteMessage.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _WebsiteMessage.Create();
                }
                else
                {
                    blnReturn = _WebsiteMessage.Update();
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }


            if (blnReturn)
            {
                Response.Redirect(ReturnURL);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                blnReturn = _WebsiteMessage.Delete();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

            if (blnReturn)
            {
                Response.Redirect(ReturnURL);
            }
        }
    }
}
using ImageSolutions.User;
using ImageSolutions.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class User : BasePageAdminUserWebSiteAuth
    {
        private string mUserInfoID = string.Empty;

        private ImageSolutions.User.UserInfo mUserInfo = null;
        protected ImageSolutions.User.UserInfo UserInfo
        {
            get
            {
                if (mUserInfo == null)
                {
                    if (string.IsNullOrEmpty(mUserInfoID))
                        mUserInfo = new ImageSolutions.User.UserInfo();
                    else
                        mUserInfo = new ImageSolutions.User.UserInfo(mUserInfoID);
                }
                return mUserInfo;
            }
            set
            {
                mUserInfo = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/UserOverview.aspx";
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
            mUserInfoID = Request.QueryString.Get("id");

            if (!Page.IsPostBack)
            {
                if (Request.UrlReferrer != null) ReturnURL = Request.UrlReferrer.PathAndQuery;

                InitializePage();
            }
        }

        public void InitializePage()
        {
            try
            {
                if (!UserInfo.IsNew)
                {
                    txtFirstName.Text = UserInfo.FirstName;
                    txtLastName.Text = UserInfo.LastName;
                    txtEmail.Text = UserInfo.EmailAddress;
                    cbIsAdmin.Checked = Convert.ToBoolean(UserInfo.IsAdmin);
                    cbInactive.Checked = Convert.ToBoolean(UserInfo.InActive);
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

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                blnReturn = UserInfo.Delete();
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                UserInfo.FirstName = txtFirstName.Text;
                UserInfo.LastName = txtLastName.Text;
                UserInfo.EmailAddress = txtEmail.Text;
                UserInfo.IsAdmin = cbIsAdmin.Checked;
                UserInfo.InActive = cbInactive.Checked;

                if (UserInfo.IsNew)
                {
                    //UserInfo.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = UserInfo.Create();
                }
                else
                {
                    blnReturn = UserInfo.Update();
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
    }
}
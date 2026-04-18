using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class AccountOrderApproval : BasePageAdminUserWebSiteAuth
    {
        protected string mAccountOrderApprovalID = string.Empty;
        protected string mAccountID = string.Empty;

        private ImageSolutions.Account.AccountOrderApproval mAccountOrderApproval = null;
        protected ImageSolutions.Account.AccountOrderApproval _AccountOrderApproval
        {
            get
            {
                if (mAccountOrderApproval == null)
                {
                    if (string.IsNullOrEmpty(mAccountOrderApprovalID))
                        mAccountOrderApproval = new ImageSolutions.Account.AccountOrderApproval();
                    else
                        mAccountOrderApproval = new ImageSolutions.Account.AccountOrderApproval(mAccountOrderApprovalID);
                }
                return mAccountOrderApproval;
            }
            set
            {
                mAccountOrderApproval = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/Account.aspx?id=" + (_AccountOrderApproval.IsNew ? mAccountID : _AccountOrderApproval.AccountID) + "&tab=4";
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
            mAccountOrderApprovalID = Request.QueryString.Get("id");
            mAccountID = Request.QueryString.Get("accountid");

            UpdateUserWebsite();

            if (!Page.IsPostBack)
            {
                //if (Request.UrlReferrer != null) ReturnURL = Request.UrlReferrer.PathAndQuery;

                InitializePage();
            }
        }

        protected void UpdateUserWebsite()
        {
            ucUserWebsiteSearchModal.SendMessageToThePage += delegate (string message)
            {
                try
                {
                    hfUserWebsiteID.Value = message;

                    if (!string.IsNullOrEmpty(hfUserWebsiteID.Value))
                    {
                        ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite(hfUserWebsiteID.Value);

                        if (UserWebsite != null && !string.IsNullOrEmpty(UserWebsite.UserWebsiteID))
                            txtUserWebsite.Text = UserWebsite.Description;
                    }
                    else
                    {
                        txtUserWebsite.Text = string.Empty;
                    }

                    btnUserWebsiteRemove.Visible = !string.IsNullOrEmpty(hfUserWebsiteID.Value);
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(this, ex.Message);
                }
            };

        }

        public void InitializePage()
        {
            try
            {
                InitializeTabs(top_1_tab, top_1, null, null, null, null, null, null);
                //BindUserWebsite();

                if (CurrentWebsite.UserWebsites.Count <= 100)
                {
                    BindUserWebsite();
                    ddlUserWebsite.Visible = true;
                    txtUserWebsite.Visible = false;
                }
                else
                {
                    ddlUserWebsite.Visible = false;
                    txtUserWebsite.Visible = true;

                    hfUserWebsiteID.Value = Convert.ToString(_AccountOrderApproval.UserWebsiteID);
                }
                if (ddlUserWebsite.Visible)
                {
                    btnUserWebsiteSearch.Visible = false;
                    btnUserWebsiteRemove.Visible = false;
                }

                if (!_AccountOrderApproval.IsNew)
                {
                    //ddlUserWebsite.SelectedValue = _AccountOrderApproval.UserWebsiteID;
                    if (ddlUserWebsite.Visible)
                    {
                        ddlUserWebsite.SelectedValue = Convert.ToString(_AccountOrderApproval.UserWebsiteID);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(hfUserWebsiteID.Value))
                        {
                            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite(hfUserWebsiteID.Value);
                            if (UserWebsite != null && !string.IsNullOrEmpty(UserWebsite.UserWebsiteID))
                            {
                                txtUserWebsite.Text = UserWebsite.Description;
                            }

                        }
                        else
                        {
                            txtUserWebsite.Text = String.Empty;
                        }
                    }

                    txtAmount.Text = Convert.ToString(_AccountOrderApproval.Amount);
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
        protected void BindUserWebsite()
        {
            try
            {
                ddlUserWebsite.DataSource = CurrentUser.CurrentUserWebSite.WebSite.UserWebsites.OrderBy(m => m.Description);
                ddlUserWebsite.DataBind();
                ddlUserWebsite.Items.Insert(0, new ListItem(String.Empty, string.Empty));
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
                //_AccountOrderApproval.UserWebsiteID = ddlUserWebsite.SelectedValue;
                _AccountOrderApproval.UserWebsiteID = ddlUserWebsite.Visible && !string.IsNullOrEmpty(ddlUserWebsite.SelectedValue) ? Convert.ToString(ddlUserWebsite.SelectedValue) : Convert.ToString(hfUserWebsiteID.Value);
                _AccountOrderApproval.Amount = Convert.ToDecimal(txtAmount.Text);

                if (string.IsNullOrEmpty(_AccountOrderApproval.UserWebsiteID))
                {
                    throw new Exception("Approver is required");
                }

                if (_AccountOrderApproval.IsNew)
                {
                    _AccountOrderApproval.AccountID = mAccountID;
                    blnReturn = _AccountOrderApproval.Create();
                    ReturnURL = "/Admin/Account.aspx?id=" + mAccountID + "&tab=4";
                }
                else
                {
                    blnReturn = _AccountOrderApproval.Update();
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
                blnReturn = _AccountOrderApproval.Delete();
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
            if (blnReturn)
            {
                Response.Redirect(ReturnURL);
            }
        }

        protected void btnUserWebsiteSearch_Click(object sender, EventArgs e)
        {
            ucUserWebsiteSearchModal.WebsiteID = CurrentWebsite.WebsiteID;
            ucUserWebsiteSearchModal.Show();
        }

        protected void btnUserWebsiteRemove_Click(object sender, EventArgs e)
        {
            ddlUserWebsite.SelectedValue = String.Empty;
            txtUserWebsite.Text = String.Empty;
            hfUserWebsiteID.Value = String.Empty;

            btnUserWebsiteRemove.Visible = !string.IsNullOrEmpty(hfUserWebsiteID.Value);
        }
    }
}
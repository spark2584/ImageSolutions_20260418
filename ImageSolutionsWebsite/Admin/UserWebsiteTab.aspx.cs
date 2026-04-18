using ImageSolutions.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class UserWebsiteTab : BasePageAdminUserWebSiteAuth
    {
        protected string mUserWebsiteTabID = string.Empty;
        protected string mWebsiteTabID = string.Empty;

        private ImageSolutions.User.UserWebsiteTab mUserWebsiteTab = null;
        protected ImageSolutions.User.UserWebsiteTab _UserWebsiteTab
        {
            get
            {
                if (mUserWebsiteTab == null)
                {
                    if (string.IsNullOrEmpty(mUserWebsiteTabID))
                        mUserWebsiteTab = new ImageSolutions.User.UserWebsiteTab();
                    else
                        mUserWebsiteTab = new ImageSolutions.User.UserWebsiteTab(mUserWebsiteTabID);
                }
                return mUserWebsiteTab;
            }
            set
            {
                mUserWebsiteTab = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/Tab.aspx?id=" + (_UserWebsiteTab.IsNew ? mWebsiteTabID : _UserWebsiteTab.WebsiteTabID) + "&tab=5";
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
            mUserWebsiteTabID = Request.QueryString.Get("id");
            mWebsiteTabID = Request.QueryString.Get("websitetabid");

            UpdateUserWebsite();

            if (!Page.IsPostBack)
            {
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

        protected void InitializePage()
        {
            try
            {
                //BindUserWebsite();

                if (CurrentWebsite.UserWebsites.Count <= 100)
                {
                    ddlUserWebsite.Visible = true;
                    BindUserWebsite();
                    txtUserWebsite.Visible = false;
                    btnUserWebsiteSearch.Visible = false;
                    btnUserWebsiteRemove.Visible = false;
                }
                else
                {
                    ddlUserWebsite.Visible = false;
                    txtUserWebsite.Visible = true;

                    hfUserWebsiteID.Value = Convert.ToString(_UserWebsiteTab.UserWebsiteID);

                    //if (!string.IsNullOrEmpty(hfUserWebsiteID.Value))
                    //{
                    //    ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite(Convert.ToString(hfUserWebsiteID.Value));
                    //    txtUserWebsite.Text = UserWebsite.Description;
                    //}

                    btnUserWebsiteSearch.Visible = true;
                    btnUserWebsiteSearch.Enabled = true;
                    btnUserWebsiteRemove.Enabled = true;
                }

                if (!_UserWebsiteTab.IsNew)
                {
                    //this.ddlUserWebsite.SelectedIndex = this.ddlUserWebsite.Items.IndexOf(this.ddlUserWebsite.Items.FindByValue(_UserWebsiteTab.UserWebsiteID));

                    if (ddlUserWebsite.Visible)
                    {
                        this.ddlUserWebsite.SelectedIndex = this.ddlUserWebsite.Items.IndexOf(this.ddlUserWebsite.Items.FindByValue(_UserWebsiteTab.UserWebsiteID));
                        //ddlUserWebsite.SelectedValue = Convert.ToString(_Account.PersonalizationApproverUserWebsiteID);
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

                        btnUserWebsiteRemove.Visible = !string.IsNullOrEmpty(hfUserWebsiteID.Value);
                    }

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
            this.ddlUserWebsite.DataSource = CurrentUser.CurrentUserWebSite.WebSite.UserWebsites.OrderBy(m => m.Description);
            this.ddlUserWebsite.DataBind();
            this.ddlUserWebsite.Items.Insert(0, new ListItem(String.Empty, string.Empty));
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                //_UserWebsiteTab.UserWebsiteID = Convert.ToString(ddlUserWebsite.SelectedValue);
                _UserWebsiteTab.UserWebsiteID = ddlUserWebsite.Visible && !string.IsNullOrEmpty(ddlUserWebsite.SelectedValue) ? Convert.ToString(ddlUserWebsite.SelectedValue) : Convert.ToString(hfUserWebsiteID.Value);

                if (string.IsNullOrEmpty(_UserWebsiteTab.UserWebsiteID))
                {
                    throw new Exception("User Required");
                }

                if (_UserWebsiteTab.IsNew)
                {
                    _UserWebsiteTab.WebsiteTabID = mWebsiteTabID;
                    _UserWebsiteTab.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _UserWebsiteTab.Create();
                }
                else
                {
                    blnReturn = _UserWebsiteTab.Update();
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
                blnReturn = _UserWebsiteTab.Delete();
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
using ImageSolutions.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class UserAccount : BasePageAdminUserWebSiteAuth
    {
        protected string mWebsiteGroupID = string.Empty;
        protected string mUserWebsiteID = string.Empty;
        protected string mAccountID = string.Empty;
        protected string mUserAccountID = string.Empty;

        private ImageSolutions.User.UserAccount mUserAccount = null;
        protected ImageSolutions.User.UserAccount _UserAccount
        {
            get
            {
                if (mUserAccount == null)
                {
                    if (string.IsNullOrEmpty(mUserAccountID))
                        mUserAccount = new ImageSolutions.User.UserAccount();
                    else
                        mUserAccount = new ImageSolutions.User.UserAccount(mUserAccountID);
                }
                return mUserAccount;
            }
            set
            {
                mUserAccount = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                {
                    if (!string.IsNullOrEmpty(mWebsiteGroupID))
                        return "/Admin/Group.aspx?id=" + mWebsiteGroupID + "&tab=2";
                    else
                        return "/Admin/UserWebsite.aspx?id=" + (_UserAccount.IsNew ? mUserWebsiteID : _UserAccount.UserWebsiteID) + "&tab=2";
                }
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
            mUserAccountID = Request.QueryString.Get("id");
            mWebsiteGroupID = Request.QueryString.Get("websitegroupid");
            mAccountID = Request.QueryString.Get("accountid");
            mUserWebsiteID = Request.QueryString.Get("userwebsiteid");

            UpdateUserWebsite();
            UpdateAccount();

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

        protected void UpdateAccount()
        {
            ucAccountSearchModal.SendMessageToThePage += delegate (string message)
            {
                try
                {
                    hfAccountID.Value = message;

                    if (!string.IsNullOrEmpty(hfAccountID.Value))
                    {
                        ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account(hfAccountID.Value);

                        if (Account != null && !string.IsNullOrEmpty(Account.AccountID))
                        {
                            txtAccount.Text = Account.AccountName;
                        }
                    }
                    else
                    {
                        txtAccount.Text = string.Empty;
                    }

                    //btnAccountRemove.Visible = !string.IsNullOrEmpty(hfAccountID.Value);
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

                    hfUserWebsiteID.Value = Convert.ToString(_UserAccount.UserWebsiteID);

                    btnUserWebsiteSearch.Visible = true;
                    btnUserWebsiteSearch.Enabled = true;
                    btnUserWebsiteRemove.Enabled = true;
                }
                
                if (CurrentUser.CurrentUserWebSite.WebSite.Accounts.Count <= 100)
                {
                    ddlAccount.Visible = true;
                    BindAccount();
                    txtAccount.Visible = false;
                    btnAccountSearch.Visible = false;
                    btnAccountRemove.Visible = false;
                }
                else
                {
                    ddlAccount.Visible = false;
                    txtAccount.Visible = true;

                    hfAccountID.Value = Convert.ToString(_UserAccount.AccountID);

                    btnAccountSearch.Visible = true;
                    btnAccountSearch.Enabled = true;
                    btnAccountRemove.Enabled = true;
                }

                BindWebsiteGroup();

                if (!_UserAccount.IsNew)
                {
                    if (ddlUserWebsite.Visible)
                    {
                        this.ddlUserWebsite.SelectedIndex = this.ddlUserWebsite.Items.IndexOf(this.ddlUserWebsite.Items.FindByValue(_UserAccount.UserWebsiteID));
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

                    if (ddlAccount.Visible)
                    {
                        this.ddlAccount.SelectedIndex = this.ddlAccount.Items.IndexOf(this.ddlAccount.Items.FindByValue(_UserAccount.AccountID));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(hfAccountID.Value))
                        {
                            ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account(hfAccountID.Value);
                            if (Account != null && !string.IsNullOrEmpty(Account.AccountID))
                            {
                                txtAccount.Text = Account.AccountName;
                            }
                        }
                        else
                        {
                            txtUserWebsite.Text = String.Empty;
                        }

                        btnAccountRemove.Visible = !string.IsNullOrEmpty(hfUserWebsiteID.Value);
                    }

                    //this.ddlUserWebsite.SelectedIndex = this.ddlUserWebsite.Items.IndexOf(this.ddlUserWebsite.Items.FindByValue(_UserAccount.UserWebsiteID));
                    //this.ddlAccount.SelectedIndex = this.ddlAccount.Items.IndexOf(this.ddlAccount.Items.FindByValue(_UserAccount.AccountID));

                    this.ddlGroup.SelectedIndex = this.ddlGroup.Items.IndexOf(this.ddlGroup.Items.FindByValue(_UserAccount.WebsiteGroupID));
                    this.cbIsPrimary.Checked = _UserAccount.IsPrimary;
                    btnSave.Text = "Save";
                    this.ddlUserWebsite.Enabled = false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(mUserWebsiteID))
                    {
                        if (ddlUserWebsite.Visible)
                        {
                            this.ddlUserWebsite.SelectedIndex = this.ddlUserWebsite.Items.IndexOf(this.ddlUserWebsite.Items.FindByValue(mUserWebsiteID));
                        }
                        else
                        {
                            hfUserWebsiteID.Value = mUserWebsiteID;

                            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite(hfUserWebsiteID.Value);
                            if (UserWebsite != null && !string.IsNullOrEmpty(UserWebsite.UserWebsiteID))
                            {
                                txtUserWebsite.Text = UserWebsite.Description;
                            }
                            else
                            {
                                throw new Exception("Invalid Account");
                            }

                            btnUserWebsiteRemove.Visible = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(mAccountID))
                    {
                        if (ddlAccount.Visible)
                        {
                            this.ddlAccount.SelectedIndex = this.ddlAccount.Items.IndexOf(this.ddlAccount.Items.FindByValue(mAccountID));
                        }
                        else
                        {
                            hfAccountID.Value = mAccountID;

                            ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account(hfAccountID.Value);
                            if (Account != null && !string.IsNullOrEmpty(Account.AccountID))
                            {
                                txtAccount.Text = Account.AccountName;
                            }
                            else
                            {
                                throw new Exception("Invalid Account");
                            }

                            btnAccountRemove.Visible = !string.IsNullOrEmpty(hfUserWebsiteID.Value);
                        }
                    }
                    
                    //this.ddlUserWebsite.SelectedIndex = this.ddlUserWebsite.Items.IndexOf(this.ddlUserWebsite.Items.FindByValue(mUserWebsiteID));
                    //this.ddlAccount.SelectedIndex = this.ddlAccount.Items.IndexOf(this.ddlAccount.Items.FindByValue(mAccountID));

                    this.ddlGroup.SelectedIndex = this.ddlGroup.Items.IndexOf(this.ddlGroup.Items.FindByValue(mWebsiteGroupID));
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
                ddlUserWebsite.DataSource = CurrentUser.CurrentUserWebSite.WebSite.UserWebsites.OrderBy(x => x.Description);
                ddlUserWebsite.DataBind();
                ddlUserWebsite.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindAccount()
        {
            try
            {
                ddlAccount.DataSource = CurrentUser.CurrentUserWebSite.WebSite.Accounts.OrderBy(x => x.AccountName);
                ddlAccount.DataBind();
                ddlAccount.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindWebsiteGroup()
        {
            try
            {
                ddlGroup.DataSource = CurrentUser.CurrentUserWebSite.WebSite.WebsiteGroups.OrderBy(x => x.GroupName);
                ddlGroup.DataBind();
                ddlGroup.Items.Insert(0, new ListItem(String.Empty, string.Empty));
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
                //_UserAccount.UserWebsiteID = ddlUserWebsite.SelectedValue;
                //_UserAccount.AccountID = ddlAccount.SelectedValue;

                _UserAccount.UserWebsiteID = ddlUserWebsite.Visible && !string.IsNullOrEmpty(ddlUserWebsite.SelectedValue) ? Convert.ToString(ddlUserWebsite.SelectedValue) : Convert.ToString(hfUserWebsiteID.Value);
                _UserAccount.AccountID = ddlAccount.Visible && !string.IsNullOrEmpty(ddlAccount.SelectedValue) ? Convert.ToString(ddlAccount.SelectedValue) : Convert.ToString(hfAccountID.Value);

                _UserAccount.WebsiteGroupID = ddlGroup.SelectedValue;
                _UserAccount.IsPrimary = cbIsPrimary.Checked;

                if (_UserAccount.IsNew)
                {
                    _UserAccount.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _UserAccount.Create();
                }
                else
                {
                    blnReturn = _UserAccount.Update();
                }

                if (_UserAccount.IsPrimary)
                {
                    List<ImageSolutions.User.UserAccount> UserAccounts = new List<ImageSolutions.User.UserAccount>();
                    ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                    UserAccountFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                    UserAccountFilter.AccountID.SearchString = _UserAccount.AccountID;
                    UserAccounts = ImageSolutions.User.UserAccount.GetUserAccounts(UserAccountFilter);

                    foreach (ImageSolutions.User.UserAccount useraccount in UserAccounts)
                    {
                        if (useraccount.UserAccountID != _UserAccount.UserAccountID)
                        {
                            useraccount.IsPrimary = false;
                            useraccount.Update();
                        }
                    }
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
                blnReturn = _UserAccount.Delete();
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

        protected void btnAccountSearch_Click(object sender, EventArgs e)
        {
            ucUserWebsiteSearchModal.Hide();

            ucAccountSearchModal.WebsiteID = CurrentWebsite.WebsiteID;
            ucAccountSearchModal.Show();
        }

        protected void btnAccountRemove_Click(object sender, EventArgs e)
        {
            ddlAccount.SelectedValue = String.Empty;
            txtAccount.Text = String.Empty;
            hfAccountID.Value = String.Empty;

            btnAccountRemove.Visible = !string.IsNullOrEmpty(hfAccountID.Value);
        }

        protected void btnUserWebsiteSearch_Click(object sender, EventArgs e)
        {
            ucAccountSearchModal.Hide();

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
using ImageSolutions.Website;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class Account : BasePageAdminUserWebSiteAuth
    {
        protected string mParentID = string.Empty;
        protected string mAccountID = string.Empty;

        private ImageSolutions.Account.Account mAccount = null;
        protected ImageSolutions.Account.Account _Account
        {
            get
            {
                if (mAccount == null)
                {
                    if (string.IsNullOrEmpty(mAccountID))
                        mAccount = new ImageSolutions.Account.Account();
                    else
                        mAccount = new ImageSolutions.Account.Account(mAccountID);
                }
                return mAccount;
            }
            set
            {
                mAccount = value;
            }
        }
        
        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/AccountOverview.aspx";
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
            mAccountID = Request.QueryString.Get("id");
            mParentID = Request.QueryString.Get("parentid");
            //this.chkIsTaxExempt.Enabled = CurrentUser.IsSuperAdmin;

            UpdateUserWebsite();
            UpdateUserWebsite2();

            SetAdminControl(CurrentUser.IsSuperAdmin);

            if (HttpContext.Current.Request.Url.AbsoluteUri.Contains(""))
            if (!Page.IsPostBack)
            {
                if (Request.UrlReferrer != null) ReturnURL = Request.UrlReferrer.PathAndQuery;

                InitializePage();
            }

            if(!string.IsNullOrEmpty(Request.QueryString.Get("tab")))
            {
                InitializeTabs(top_1_tab, top_1, top_2_tab, top_2, top_3_tab, top_3, top_4_tab, top_4, top_5_tab, top_5);
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

        protected void UpdateUserWebsite2()
        {
            ucUserWebsiteSearchModal2.SendMessageToThePage += delegate (string message)
            {
                try
                {
                    hfUserWebsiteID2.Value = message;

                    if (!string.IsNullOrEmpty(hfUserWebsiteID2.Value))
                    {
                        ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite(hfUserWebsiteID2.Value);

                        if (UserWebsite != null && !string.IsNullOrEmpty(UserWebsite.UserWebsiteID))
                            txtUserWebsite2.Text = UserWebsite.Description;
                    }
                    else
                    {
                        txtUserWebsite2.Text = string.Empty;
                    }

                    btnUserWebsiteRemove2.Visible = !string.IsNullOrEmpty(hfUserWebsiteID2.Value);
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(this, ex.Message);
                }
            };

        }

        public void SetAdminControl(bool enable)
        {
            top_6_tab.Visible = enable;
            top_6.Visible = enable;
            txtInternalID.Enabled = enable;
            txtSiteNumber.Enabled = enable;
            txtStoreNumber.Enabled = enable;
            ddlUserWebsite.Enabled = enable;
            btnUserWebsiteSearch.Enabled = enable;
            btnUserWebsiteRemove.Enabled = enable;
            ddlUserWebsite2.Enabled = enable;
            btnUserWebsiteSearch2.Enabled = enable;
            btnUserWebsiteRemove2.Enabled = enable;
            btnSave2.Enabled = enable;
        }

        public void InitializePage()
        {
            try
            {
                InitializeTabs(top_1_tab, top_1, top_2_tab, top_2, top_3_tab, top_3, top_4_tab, top_4, top_5_tab, top_5);

                top_4_tab.Visible = CurrentUser.IsSuperAdmin;

                BindWebsiteGroup();
                BindParentAccount();
                BindCountry();
                BindAccountOrderApproval();
                BindCustomField();

                if (CurrentWebsite.UserWebsites.Count <= 100)
                {
                    BindUser();                    ddlUserWebsite.Visible = true;
                    txtUserWebsite.Visible = false;

                    ddlUserWebsite2.Visible = true;
                    txtUserWebsite2.Visible = false;
                }
                else
                {
                    ddlUserWebsite.Visible = false;
                    txtUserWebsite.Visible = true;

                    ddlUserWebsite2.Visible = false;
                    txtUserWebsite2.Visible = true;

                    hfUserWebsiteID.Value = Convert.ToString(_Account.PersonalizationApproverUserWebsiteID);
                    hfUserWebsiteID2.Value = Convert.ToString(_Account.PersonalizationApprover2UserWebsiteID);
                }

                divParentAccount.Visible = CurrentUser.IsSuperAdmin;
                phDefaultWebsiteGroup.Visible = CurrentWebsite.DisplayDefaultGroupPerAccount || CurrentUser.IsSuperAdmin;

                rfvRegistrationKey.Enabled = CurrentWebsite.UserRegistrationKeyRequired;

                if (!_Account.IsNew)
                {
                    this.txtAccountName.Text = _Account.AccountName;
                    this.txtRegistrationKey.Text = _Account.RegistrationKey;
                    this.ddlWebsiteGroup.SelectedIndex = this.ddlWebsiteGroup.Items.IndexOf(this.ddlWebsiteGroup.Items.FindByValue(_Account.DefaultWebsiteGroupID));
                    this.ddlParentAccount.SelectedIndex = this.ddlParentAccount.Items.IndexOf(this.ddlParentAccount.Items.FindByValue(_Account.ParentID));
                    //this.chkIsTaxExempt.Checked = _Account.IsTaxExempt;

                    if (_Account.DefaultShippingAddressBook != null)
                    {
                        this.txtFirstName.Text = _Account.DefaultShippingAddressBook.FirstName;
                        this.txtLastName.Text = _Account.DefaultShippingAddressBook.LastName;
                        this.txtAddress.Text = _Account.DefaultShippingAddressBook.AddressLine1;
                        this.txtAddress2.Text = _Account.DefaultShippingAddressBook.AddressLine2;
                        this.txtCity.Text = _Account.DefaultShippingAddressBook.City;
                        this.txtState.Text = _Account.DefaultShippingAddressBook.State;
                        this.txtZip.Text = _Account.DefaultShippingAddressBook.PostalCode;
                        this.ddlCountry.SelectedIndex = !string.IsNullOrEmpty(_Account.DefaultShippingAddressBook.CountryCode) ? this.ddlCountry.Items.IndexOf(this.ddlCountry.Items.FindByValue(_Account.DefaultShippingAddressBook.CountryCode)) : this.ddlCountry.Items.IndexOf(this.ddlCountry.Items.FindByValue("US"));
                        this.txtPhoneNumber.Text = _Account.DefaultShippingAddressBook.PhoneNumber;
                    }
                    this.chkGetSubAccountNotification.Checked = _Account.GetSubAccountNotification;
                    this.chkIsPendingApproval.Checked = _Account.IsPendingApproval;

                    this.txtInternalID.Text = _Account.CustomerInternalID;
                    this.txtSiteNumber.Text = _Account.SiteNumber;
                    this.txtStoreNumber.Text = _Account.StoreNumber;
                    //this.ddlUser.SelectedIndex = this.ddlParentAccount.Items.IndexOf(this.ddlUser.Items.FindByValue(_Account.PersonalizationApproverUserWebsiteID));

                    if (ddlUserWebsite.Visible)
                    {
                        ddlUserWebsite.SelectedValue = Convert.ToString(_Account.PersonalizationApproverUserWebsiteID);
                        ddlUserWebsite2.SelectedValue = Convert.ToString(_Account.PersonalizationApprover2UserWebsiteID);
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

                        if (!string.IsNullOrEmpty(hfUserWebsiteID2.Value))
                        {
                            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite(hfUserWebsiteID2.Value);
                            if (UserWebsite != null && !string.IsNullOrEmpty(UserWebsite.UserWebsiteID))
                            {
                                txtUserWebsite2.Text = UserWebsite.Description;
                            }

                        }
                        else
                        {
                            txtUserWebsite2.Text = String.Empty;
                        }
                    }


                    //this.chkAutoAssignBudget.Checked = _Account.AutoAssignBudget;
                    this.ddlBudgetSetting.SelectedValue = _Account.BudgetSetting;
                    this.chkDoNotAllowCreditCard.Checked = _Account.DoNotAllowCreditCard;

                    BindChildAccounts();
                    BindUserAccounts();

                    if (_Account.IsPendingApproval)
                        btnSave.Text = "Approve";
                    else
                        btnSave.Text = "Save";
                    //this.btnDelete.Visible = _Account.AccountID != CurrentUser.CurrentUserWebSite.CurrentUserAccount.AccountID;
                    this.btnDelete.Visible = CurrentUser.IsSuperAdmin;
                }
                else
                {
                    this.ddlParentAccount.SelectedIndex = this.ddlParentAccount.Items.IndexOf(this.ddlParentAccount.Items.FindByValue(mParentID));
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                    this.top_2_tab.Visible = false;
                    this.top_3_tab.Visible = false;
                }
                aCancel.HRef = ReturnURL;

                pnlAddress.Visible = _Account.IsNew || !string.IsNullOrEmpty(_Account.DefaultShippingAddressBookID);
                pnlDisplayAddress.Visible = !_Account.IsNew && string.IsNullOrEmpty(_Account.DefaultShippingAddressBookID);
                if (pnlDisplayAddress.Visible) chkAddress.Checked = false;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
        protected void BindUser()
        {
            this.ddlUserWebsite.DataSource = CurrentUser.CurrentUserWebSite.WebSite.UserWebsites.OrderBy(m => m.Description);
            this.ddlUserWebsite.DataBind();
            this.ddlUserWebsite.Items.Insert(0, new ListItem(String.Empty, string.Empty));

            this.ddlUserWebsite2.DataSource = CurrentUser.CurrentUserWebSite.WebSite.UserWebsites.OrderBy(m => m.Description);
            this.ddlUserWebsite2.DataBind();
            this.ddlUserWebsite2.Items.Insert(0, new ListItem(String.Empty, string.Empty));


            //try
            //{
            //    if(_Account.UserAccounts != null)
            //    {
            //        List<ImageSolutions.User.UserWebsite> UserWebsites = new List<ImageSolutions.User.UserWebsite>();
            //        foreach (ImageSolutions.User.UserAccount _UserAccount in _Account.UserAccounts)
            //        {
            //            if (!UserWebsites.Exists(x => x.UserWebsiteID == _UserAccount.UserWebsiteID))
            //            {
            //                UserWebsites.Add(_UserAccount.UserWebsite);
            //            }
            //        }

            //        //ddlUser.DataSource = CurrentUser.CurrentUserWebSite.WebSite.UserWebsites.OrderBy(x => x.Description);
            //        ddlUser.DataSource = UserWebsites;
            //        ddlUser.DataBind();
            //        ddlUser.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            //    }
            //}
            //catch (Exception ex)
            //{
            //    WebUtility.DisplayJavascriptMessage(this, ex.Message);
            //}
            //finally { }
        }
        protected void BindWebsiteGroup()
        {
            try
            {
                this.ddlWebsiteGroup.DataSource = CurrentUser.CurrentUserWebSite.WebSite.WebsiteGroups;
                this.ddlWebsiteGroup.DataBind();
                this.ddlWebsiteGroup.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
        protected void BindCountry()
        {
            try
            {
                List<ImageSolutions.Address.AddressCountryCode> AddressCountryCodes = new List<ImageSolutions.Address.AddressCountryCode>();
                AddressCountryCodes = ImageSolutions.Address.AddressCountryCode.GetAddressCountryCodes();
                List<ImageSolutions.Address.AddressCountryCode> FilterAddressCountryCodes = new List<ImageSolutions.Address.AddressCountryCode>();
                if (CurrentWebsite.WebsiteCountries.Count > 0)
                {
                    if (CurrentWebsite.WebsiteCountries.FindAll(x => !Convert.ToBoolean(x.Exclude)).Count > 0)
                    {
                        foreach (ImageSolutions.Website.WebsiteCountry _WebsiteCountry in CurrentWebsite.WebsiteCountries.FindAll(x => !Convert.ToBoolean(x.Exclude)).ToList())
                        {
                            FilterAddressCountryCodes.Add(AddressCountryCodes.Find(x => x.Alpha2Code == _WebsiteCountry.CountryCode));
                        }
                    }
                    FilterAddressCountryCodes = FilterAddressCountryCodes != null && FilterAddressCountryCodes.Count > 0 ? FilterAddressCountryCodes : AddressCountryCodes;
                    if (CurrentWebsite.WebsiteCountries.FindAll(x => Convert.ToBoolean(x.Exclude)).Count > 0)
                    {
                        foreach (ImageSolutions.Website.WebsiteCountry _WebsiteCountry in CurrentWebsite.WebsiteCountries.FindAll(x => Convert.ToBoolean(x.Exclude)).ToList())
                        {
                            FilterAddressCountryCodes.Remove(FilterAddressCountryCodes.Find(x => x.Alpha2Code == _WebsiteCountry.CountryCode));
                        }
                    }
                }

                ddlCountry.DataSource = FilterAddressCountryCodes != null && FilterAddressCountryCodes.Count > 0 ? FilterAddressCountryCodes : AddressCountryCodes;
                ddlCountry.DataBind();
                ddlCountry.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        protected void BindParentAccount()
        {
            try
            {
                if (CurrentUser.IsSuperAdmin)
                    this.ddlParentAccount.DataSource = CurrentUser.CurrentUserWebSite.WebSite.Accounts.FindAll(m => m.AccountID != mAccountID).OrderBy(m => m.AccountNamePath);
                else
                    this.ddlParentAccount.DataSource = CurrentUser.CurrentUserWebSite.WebSite.Accounts.FindAll(m => m.AccountID == CurrentUser.CurrentUserWebSite.CurrentUserAccount.AccountID || m.AccountID == CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.ParentID).OrderBy(m => m.AccountNamePath); ;
                this.ddlParentAccount.DataBind();
                this.ddlParentAccount.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        protected void BindChildAccounts()
        {
            try
            {
                gvSubAccounts.DataSource = _Account.ChildAccounts;
                gvSubAccounts.DataBind();
                if (this.gvSubAccounts.HeaderRow != null) this.gvSubAccounts.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindUserAccounts()
        {
            int intTotalRecord = 0;

            try
            {
                List<ImageSolutions.User.UserAccount> UserAccounts = new List<ImageSolutions.User.UserAccount>();
                ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                UserAccountFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                UserAccountFilter.AccountID.SearchString = _Account.AccountID;
                UserAccounts = ImageSolutions.User.UserAccount.GetUserAccounts(UserAccountFilter, ucUserAccountPager.PageSize, ucUserAccountPager.CurrentPageNumber, out intTotalRecord);

                gvUserAccounts.DataSource = UserAccounts; //_Account.UserAccounts;
                gvUserAccounts.DataBind();
                if (this.gvUserAccounts.HeaderRow != null) this.gvUserAccounts.HeaderRow.TableSection = TableRowSection.TableHeader;

                ucUserAccountPager.TotalRecord = intTotalRecord;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
        protected void BindAccountOrderApproval()
        {
            try
            {
                List<ImageSolutions.Account.AccountOrderApproval> AccountOrderApprovals = new List<ImageSolutions.Account.AccountOrderApproval>();
                ImageSolutions.Account.AccountOrderApprovalFilter AccountOrderApprovalFilter = new ImageSolutions.Account.AccountOrderApprovalFilter();
                AccountOrderApprovalFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                AccountOrderApprovalFilter.AccountID.SearchString = mAccountID;
                AccountOrderApprovals = ImageSolutions.Account.AccountOrderApproval.GetAccountOrderApprovals(AccountOrderApprovalFilter);

                gvAccountOrderApproval.DataSource = AccountOrderApprovals;
                this.gvAccountOrderApproval.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                if ((ddlCountry.SelectedValue == "US" || ddlCountry.SelectedValue == "CA") && string.IsNullOrEmpty(txtState.Text))
                {
                    throw new Exception("State required");
                }

                _Account.AccountName = this.txtAccountName.Text;
                _Account.DefaultWebsiteGroupID = this.ddlWebsiteGroup.SelectedValue;
                if (divParentAccount.Visible) _Account.ParentID = this.ddlParentAccount.SelectedValue;
                _Account.AccountName = this.txtAccountName.Text.Trim();
                _Account.RegistrationKey = this.txtRegistrationKey.Text.Trim();
                //_Account.IsTaxExempt = chkIsTaxExempt.Checked;

                if (pnlAddress.Visible)
                {
                    if (_Account.DefaultShippingAddressBook == null) _Account.DefaultShippingAddressBook = new ImageSolutions.Address.AddressBook();
                    _Account.DefaultShippingAddressBook.AddressLabel = txtFirstName.Text + " " + txtLastName.Text;
                    _Account.DefaultShippingAddressBook.FirstName = txtFirstName.Text;
                    _Account.DefaultShippingAddressBook.LastName = txtLastName.Text;
                    _Account.DefaultShippingAddressBook.AddressLine1 = txtAddress.Text;
                    _Account.DefaultShippingAddressBook.AddressLine2 = txtAddress2.Text;
                    _Account.DefaultShippingAddressBook.City = txtCity.Text;
                    _Account.DefaultShippingAddressBook.State = txtState.Text;
                    _Account.DefaultShippingAddressBook.PostalCode = txtZip.Text;
                    _Account.DefaultShippingAddressBook.CountryCode = ddlCountry.SelectedValue;
                    _Account.DefaultShippingAddressBook.PhoneNumber = txtPhoneNumber.Text;
                    _Account.DefaultShippingAddressBook.CreatedBy = CurrentUser.UserInfoID;
                }

                _Account.GetSubAccountNotification = chkGetSubAccountNotification.Checked;
                _Account.CustomerInternalID = txtInternalID.Text;
                _Account.SiteNumber = txtSiteNumber.Text;
                _Account.StoreNumber = txtStoreNumber.Text;
                //_Account.PersonalizationApproverUserWebsiteID = ddlUser.SelectedValue;
                _Account.PersonalizationApproverUserWebsiteID = ddlUserWebsite.Visible && !string.IsNullOrEmpty(ddlUserWebsite.SelectedValue) ? Convert.ToString(ddlUserWebsite.SelectedValue) : Convert.ToString(hfUserWebsiteID.Value);
                
                _Account.PersonalizationApprover2UserWebsiteID = ddlUserWebsite2.Visible && !string.IsNullOrEmpty(ddlUserWebsite2.SelectedValue) ? Convert.ToString(ddlUserWebsite2.SelectedValue) : Convert.ToString(hfUserWebsiteID2.Value);
                
                //_Account.AutoAssignBudget = chkAutoAssignBudget.Checked;
                _Account.BudgetSetting = ddlBudgetSetting.SelectedValue;
                _Account.DoNotAllowCreditCard = chkDoNotAllowCreditCard.Checked;

                if (_Account.IsNew)
                {
                    _Account.WebsiteID = CurrentUser.CurrentUserWebSite.WebsiteID;
                    _Account.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _Account.Create();
                }
                else
                {
                    _Account.IsPendingApproval = false;
                    blnReturn = _Account.Update();
                }

                if(btnSave.Text == "Approve")
                {
                    foreach(ImageSolutions.User.UserInfo _UserInfo in _Account.UserInfos)
                    {
                        string strHTMLContent = string.Empty;
                        strHTMLContent = String.Format(@"
Hi {0},<br /><br />Account {1} has been approved<br /><br /><a clicktracking=""off"" href=""{2}"" target=""_blank"">Go To My Account</a>
"
                            , string.Format(@"{0} {1}", _UserInfo.FirstName, _UserInfo.LastName)
                            , _Account.AccountName
                            , String.Format(@"{0}/myaccount/dashboard.aspx", string.IsNullOrEmpty(CurrentWebsite.Domain) ? Convert.ToString(ConfigurationManager.AppSettings["WebsiteURL"]) : "https://" + CurrentWebsite.Domain)
                        );
                        SendEmail(Convert.ToString(_UserInfo.EmailAddress), CurrentWebsite.Name + " Account Approved", strHTMLContent);

                        if(CurrentWebsite.WebsiteID == "2") // If Burger King - temporarily hard coded
                        {
                            SendEmail("hfebres@rsilink.com", CurrentWebsite.Name + " Account Approved", strHTMLContent);
                            SendEmail("scaballero@rsilink.com", CurrentWebsite.Name + " Account Approved", strHTMLContent);
                            SendEmail("smccarthy@rsilink.com", CurrentWebsite.Name + " Account Approved", strHTMLContent);
                            SendEmail("jprat@rsilink.com", CurrentWebsite.Name + " Account Approved", strHTMLContent);                            
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
                blnReturn = _Account.Delete();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

            if (blnReturn)
            {
                Response.Redirect("/Admin/AccountOverview.aspx");
            }
        }

        protected void BindCustomField()
        {
            try
            {
                List<ImageSolutions.Custom.CustomField> CustomFields = new List<ImageSolutions.Custom.CustomField>();
                ImageSolutions.Custom.CustomFieldFilter CustomFieldFilter = new ImageSolutions.Custom.CustomFieldFilter();
                CustomFieldFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                CustomFieldFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                CustomFieldFilter.Location = new Database.Filter.StringSearch.SearchFilter();
                CustomFieldFilter.Location.SearchString = "account";
                CustomFieldFilter.Inactive = false;
                CustomFields = ImageSolutions.Custom.CustomField.GetCustomFields(CustomFieldFilter);
                rptCustomField.DataSource = CustomFields;
                rptCustomField.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        protected void rptCustomField_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    ImageSolutions.Custom.CustomField CustomField = (ImageSolutions.Custom.CustomField)e.Item.DataItem;
                    TextBox txtCustomValue = (TextBox)e.Item.FindControl("txtCustomValue");
                    DropDownList ddlCustomValueList = (DropDownList)e.Item.FindControl("ddlCustomValueList");
                    Panel pnlCustomList = (Panel)e.Item.FindControl("pnlCustomList");
                    RequiredFieldValidator valCustomField = (RequiredFieldValidator)e.Item.FindControl("valCustomField");
                    HiddenField hfCustomValueID = (HiddenField)e.Item.FindControl("hfCustomValueID");
                    if(CustomField.Type == "customvaluelist")
                    {
                        txtCustomValue.Visible = false;
                        ddlCustomValueList.Visible = false;
                        pnlCustomList.Visible = true;
                        Repeater rptCustomValueList = (Repeater)e.Item.FindControl("rptCustomValueList");
                        List<ImageSolutions.Custom.CustomValueList> CustomValueLists = new List<ImageSolutions.Custom.CustomValueList>();
                        ImageSolutions.Custom.CustomValueListFilter CustomValueListFilter = new ImageSolutions.Custom.CustomValueListFilter();
                        CustomValueListFilter.CustomFieldID = new Database.Filter.StringSearch.SearchFilter();
                        CustomValueListFilter.CustomFieldID.SearchString = CustomField.CustomFieldID;
                        CustomValueListFilter.FilterAccountID = new Database.Filter.StringSearch.SearchFilter();
                        CustomValueListFilter.FilterAccountID.SearchString = mAccountID;
                        CustomValueLists = ImageSolutions.Custom.CustomValueList.GetCustomValueLists(CustomValueListFilter);
                        rptCustomValueList.DataSource = CustomValueLists.OrderBy(m => m.Value);                        
                        rptCustomValueList.DataBind();
                    }
                    else if (CustomField.Type == "dropdown")
                    {
                        txtCustomValue.Visible = false;
                        ddlCustomValueList.Visible = true;
                        pnlCustomList.Visible = false;
                        List<ImageSolutions.Custom.CustomValueList> CustomValueLists = new List<ImageSolutions.Custom.CustomValueList>();
                        ImageSolutions.Custom.CustomValueListFilter CustomValueListFilter = new ImageSolutions.Custom.CustomValueListFilter();
                        CustomValueListFilter.CustomFieldID = new Database.Filter.StringSearch.SearchFilter();
                        CustomValueListFilter.CustomFieldID.SearchString = CustomField.CustomFieldID;
                        CustomValueListFilter.Inactive = false;
                        CustomValueLists = ImageSolutions.Custom.CustomValueList.GetCustomValueLists(CustomValueListFilter);
                        ddlCustomValueList.DataSource = CustomValueLists;
                        ddlCustomValueList.DataBind();
                        ddlCustomValueList.Items.Insert(0, new ListItem(String.Empty, string.Empty));
                    }
                    else
                    {
                        txtCustomValue.Visible = true;
                        ddlCustomValueList.Visible = false;
                        pnlCustomList.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
        protected void btnUpdateCustomField_Click(object sender, EventArgs e)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;
            foreach (RepeaterItem _Item in this.rptCustomField.Items)
            {
                string strCustomFieldID = ((HiddenField)_Item.FindControl("hfCustomFieldID")).Value;
                string strCustomValueID = ((HiddenField)_Item.FindControl("hfCustomValueID")).Value;
                try
                {
                    objConn = new SqlConnection(Database.DefaultConnectionString);
                    objConn.Open();
                    objTran = objConn.BeginTransaction();
                    ImageSolutions.Custom.CustomValue CustomValue = null;
                    if (string.IsNullOrEmpty(strCustomValueID))
                    {
                        CustomValue = new ImageSolutions.Custom.CustomValue();
                        CustomValue.CustomFieldID = strCustomFieldID;
                        CustomValue.UserWebsiteID = CurrentUser.CurrentUserWebSite.UserWebsiteID;
                    }
                    else
                    {
                        CustomValue = new ImageSolutions.Custom.CustomValue(strCustomValueID);
                    }
                    ImageSolutions.Custom.CustomField CustomField = new ImageSolutions.Custom.CustomField(strCustomFieldID);
                    if (CustomField.Type == "customvaluelist")
                    {
                        Repeater rptCustomValueList = (Repeater)_Item.FindControl("rptCustomValueList");
                        List<string> Values = new List<string>();
                        foreach(RepeaterItem _CustomValueList in rptCustomValueList.Items)
                        {
                            HiddenField hfCustomFieldID = (HiddenField)_CustomValueList.FindControl("hfCustomFieldID");
                            HiddenField hfCustomValueListID = (HiddenField)_CustomValueList.FindControl("hfCustomValueListID");
                            Label lblCustomValueListValue = (Label)_CustomValueList.FindControl("lblCustomValueListValue");
                            Values.Add(lblCustomValueListValue.Text);
                            ImageSolutions.Custom.CustomValueList CustomValueList = null;
                            if (string.IsNullOrEmpty(hfCustomValueListID.Value))
                            {
                                CustomValueList = new ImageSolutions.Custom.CustomValueList();
                                CustomValueList.CustomFieldID = Convert.ToString(hfCustomFieldID.Value);
                                CustomValueList.FilterAccountID = mAccountID;
                                CustomValueList.Value = Convert.ToString(lblCustomValueListValue.Text);
                                CustomValueList.Create(objConn, objTran);
                            }
                        }
                        //Remove missing 
                        List<ImageSolutions.Custom.CustomValueList> CustomValueLists = new List<ImageSolutions.Custom.CustomValueList>();
                        ImageSolutions.Custom.CustomValueListFilter CustomValueListFilter = new ImageSolutions.Custom.CustomValueListFilter();
                        CustomValueListFilter.CustomFieldID = new Database.Filter.StringSearch.SearchFilter();
                        CustomValueListFilter.CustomFieldID.SearchString = CustomField.CustomFieldID;
                        CustomValueListFilter.FilterAccountID = new Database.Filter.StringSearch.SearchFilter();
                        CustomValueListFilter.FilterAccountID.SearchString = mAccountID;
                        CustomValueLists = ImageSolutions.Custom.CustomValueList.GetCustomValueLists(CustomValueListFilter);
                        foreach(ImageSolutions.Custom.CustomValueList _CustomValueList in CustomValueLists)
                        {
                            if (!Values.Exists(x => x == _CustomValueList.Value))
                            {
                                _CustomValueList.Delete(objConn, objTran);
                            }
                        }
                    }
                    else if (CustomField.Type == "dropdown")
                    {
                        DropDownList ddlCustomValueList = (DropDownList)_Item.FindControl("ddlCustomValueList");
                        CustomValue.Value = ddlCustomValueList.SelectedValue;
                    }
                    else
                    {
                        TextBox txtCustomValue = (TextBox)_Item.FindControl("txtCustomValue");
                        CustomValue.Value = txtCustomValue.Text;
                    }
                    if (CustomField.IsRequired && string.IsNullOrEmpty(CustomValue.Value))
                    {
                        throw new Exception(String.Format("{0} is required", CustomField.Name));
                    }
                    if (CustomValue.IsNew)
                    {
                        CustomValue.Create(objConn, objTran);
                    }
                    else
                    {
                        CustomValue.Update(objConn, objTran);
                    }
                    objTran.Commit();
                }
                catch (Exception ex)
                {
                    if (objTran != null) objTran.Rollback();
                    WebUtility.DisplayJavascriptMessage(this, ex.Message);
                }
                finally
                {
                    if (objTran != null) objTran.Dispose();
                    objTran = null;
                    if (objConn != null) objConn.Dispose();
                    objConn = null;
                }
            }
            BindCustomField();
        }
        protected void rptCustomValueList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                Repeater rptCustomValueList = (Repeater)source; //(Repeater)e.Item.FindControl("rptCustomValueList");
                //HiddenField hfCustomValueListID = (HiddenField)e.Item.FindControl("hfCustomValueListID");
                //HiddenField hfCustomFieldID = (HiddenField)e.Item.FindControl("hfCustomFieldID");
                Label lblCustomValueListValue = (Label)e.Item.FindControl("lblCustomValueListValue");

                List<ImageSolutions.Custom.CustomValueList> CustomValueLists = null;

                try
                {
                    CustomValueLists = new List<ImageSolutions.Custom.CustomValueList>();

                    foreach (RepeaterItem _Item in rptCustomValueList.Items)
                    {
                        HiddenField hfCustomValueListIDExist = (HiddenField)_Item.FindControl("hfCustomValueListID");
                        HiddenField hfCustomFieldIDExist = (HiddenField)_Item.FindControl("hfCustomFieldID");
                        Label lblCustomValueListValueExist = (Label)_Item.FindControl("lblCustomValueListValue");

                        if(Convert.ToString(lblCustomValueListValue.Text) != Convert.ToString(lblCustomValueListValueExist.Text))
                        {
                            ImageSolutions.Custom.CustomValueList ExistCustomValueList = null;
                            if (!string.IsNullOrEmpty(hfCustomValueListIDExist.Value))
                            {
                                ExistCustomValueList = new ImageSolutions.Custom.CustomValueList(Convert.ToString(hfCustomValueListIDExist.Value));
                            }
                            else
                            {
                                ExistCustomValueList = new ImageSolutions.Custom.CustomValueList();
                                ExistCustomValueList.CustomFieldID = Convert.ToString(hfCustomFieldIDExist.Value);
                                ExistCustomValueList.FilterAccountID = mAccountID;
                                ExistCustomValueList.Value = Convert.ToString(lblCustomValueListValueExist.Text);
                            }
                            CustomValueLists.Add(ExistCustomValueList);
                        }
                    }

                    rptCustomValueList.DataSource = CustomValueLists.OrderBy(m => m.Value);
                    rptCustomValueList.DataBind();
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(this, ex.Message);
                }
                finally
                {
                    CustomValueLists = null;
                }

            }
        }
        protected void btnAddCustomValueListValue_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;
            Repeater rptCustomValueList = (Repeater)item.FindControl("rptCustomValueList");
            TextBox txtCustomValueListValue = (TextBox)item.FindControl("txtCustomValueListValue");
            HiddenField hfCustomFieldID = (HiddenField)item.FindControl("hfCustomFieldID");
            List<ImageSolutions.Custom.CustomValueList> CustomValueLists = null;
            try
            {
                if (string.IsNullOrEmpty(txtCustomValueListValue.Text))
                {
                    throw new Exception("Missing Custom Value");
                }
                CustomValueLists = new List<ImageSolutions.Custom.CustomValueList>();
                foreach (RepeaterItem _Item in rptCustomValueList.Items)
                {
                    HiddenField hfCustomValueListID = (HiddenField)_Item.FindControl("hfCustomValueListID");
                    Label lblCustomValueListValue = (Label)_Item.FindControl("lblCustomValueListValue");
                    if (Convert.ToString(txtCustomValueListValue.Text) == Convert.ToString(lblCustomValueListValue.Text))
                    {
                        throw new Exception("Value already exists");
                    }
                    ImageSolutions.Custom.CustomValueList ExistCustomValueList = null;
                    if (!string.IsNullOrEmpty(hfCustomValueListID.Value))
                    {
                        ExistCustomValueList = new ImageSolutions.Custom.CustomValueList(Convert.ToString(hfCustomValueListID.Value));
                    }
                    else
                    {
                        ExistCustomValueList = new ImageSolutions.Custom.CustomValueList();
                        ExistCustomValueList.CustomFieldID = Convert.ToString(hfCustomFieldID.Value);
                        ExistCustomValueList.FilterAccountID = mAccountID;
                        ExistCustomValueList.Value = Convert.ToString(lblCustomValueListValue.Text);
                    }
                    CustomValueLists.Add(ExistCustomValueList);
                }
                ImageSolutions.Custom.CustomValueList CustomValueList = new ImageSolutions.Custom.CustomValueList();
                CustomValueList.CustomFieldID = Convert.ToString(hfCustomFieldID.Value);
                CustomValueList.FilterAccountID = mAccountID;
                CustomValueList.Value = Convert.ToString(txtCustomValueListValue.Text);
                CustomValueLists.Add(CustomValueList);
                rptCustomValueList.DataSource = CustomValueLists.OrderBy(m => m.Value);
                rptCustomValueList.DataBind();
                txtCustomValueListValue.Text = String.Empty;
            }
            catch(Exception ex)
            {
                txtCustomValueListValue.Text = String.Empty;
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
                CustomValueLists = null;
            }            
        }

        protected void chkAddress_CheckedChanged(object sender, EventArgs e)
        {
            pnlAddress.Visible = chkAddress.Checked;
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

        protected void ucUserAccountPager_PostBackPageIndexChanging(object sender, EventArgs e)
        {
            BindUserAccounts();

            if (top_3_tab != null)
            {
                top_3_tab.Attributes.Remove("class");
                top_3_tab.Attributes.Add("class", "nav-link active");
                top_3.Attributes.Remove("class");
                top_3.Attributes.Add("class", "tab-pane fade show active");
            }

            if (top_1_tab != null)
            {
                top_1_tab.Attributes.Remove("class");
                top_1_tab.Attributes.Add("class", "nav-link");
                top_1.Attributes.Remove("class");
                top_1.Attributes.Add("class", "tab-pane fade");
            }

            if (top_2_tab != null)
            {
                top_2_tab.Attributes.Remove("class");
                top_2_tab.Attributes.Add("class", "nav-link");
                top_2.Attributes.Remove("class");
                top_2.Attributes.Add("class", "tab-pane fade");
            }

            if (top_4_tab != null)
            {
                top_4_tab.Attributes.Remove("class");
                top_4_tab.Attributes.Add("class", "nav-link");
                top_4.Attributes.Remove("class");
                top_4.Attributes.Add("class", "tab-pane fade");
            }

            if (top_5_tab != null)
            {
                top_5_tab.Attributes.Remove("class");
                top_5_tab.Attributes.Add("class", "nav-link");
                top_5.Attributes.Remove("class");
                top_5.Attributes.Add("class", "tab-pane fade");
            }

            if (top_6_tab != null)
            {
                top_6_tab.Attributes.Remove("class");
                top_6_tab.Attributes.Add("class", "nav-link");
                top_6.Attributes.Remove("class");
                top_6.Attributes.Add("class", "tab-pane fade");
            }

        }

        protected void btnUserWebsiteSearch2_Click(object sender, EventArgs e)
        {
            ucUserWebsiteSearchModal2.WebsiteID = CurrentWebsite.WebsiteID;
            ucUserWebsiteSearchModal2.Show();
        }

        protected void btnUserWebsiteRemove2_Click(object sender, EventArgs e)
        {
            ddlUserWebsite2.SelectedValue = String.Empty;
            txtUserWebsite2.Text = String.Empty;
            hfUserWebsiteID2.Value = String.Empty;

            btnUserWebsiteRemove2.Visible = !string.IsNullOrEmpty(hfUserWebsiteID2.Value);
        }
    }
}
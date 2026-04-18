using Amazon.S3.Model;
using ImageSolutions.User;
using ImageSolutions.Website;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class UserWebsite : BasePageAdminUserWebSiteAuth
    {
        protected string mAccountID = string.Empty;
        protected string mUserWebsiteID = string.Empty;
        protected ImageSolutions.User.UserWebsite mUserWebsite = null;
        protected ImageSolutions.User.UserWebsite _UserWebSite
        {
            get
            {
                if (mUserWebsite == null)
                {
                    if (string.IsNullOrEmpty(mUserWebsiteID))
                        mUserWebsite = new ImageSolutions.User.UserWebsite();
                    else
                        mUserWebsite = new ImageSolutions.User.UserWebsite(mUserWebsiteID);
                }
                return mUserWebsite;
            }
            set
            {
                mUserWebsite = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                {
                    if (!string.IsNullOrEmpty(mAccountID))
                        return "/Admin/Account.aspx?id=" + mAccountID + "&tab=3";
                    else
                        return "/Admin/UserWebsiteOverview.aspx";
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
            mUserWebsiteID = Request.QueryString.Get("id");
            mAccountID = Request.QueryString.Get("accountid");
            top_3_tab.Visible = CurrentWebsite.EnableEmployeeCredit;
            top_2_tab.Visible = CurrentUser.IsSuperAdmin;
            btnDelete.Visible = CurrentUser.IsSuperAdmin;
            phTaxExempt.Visible = CurrentUser.IsSuperAdmin;
            phRequestTaxExempt.Visible = false; //CurrentUser.IsSuperAdmin;
            phPaymnetTerm.Visible = CurrentUser.IsSuperAdmin;            

            this.btnLogin.Visible = CurrentUser.IsSuperAdmin 
                || (
                    CurrentUser.CurrentUserWebSite.IsAdmin 
                    && _UserWebSite != null && !string.IsNullOrEmpty(_UserWebSite.UserWebsiteID) && _UserWebSite.UserInfo != null && !_UserWebSite.UserInfo.IsSuperAdmin);
            this.phPermission.Visible = CurrentUser.IsSuperAdmin;

            SetAdminControl(CurrentUser.IsSuperAdmin);

            if (!Page.IsPostBack)
            {
                //if (Request.UrlReferrer != null) ReturnURL = Request.UrlReferrer.PathAndQuery;

                InitializePage();
            }

            pnlApplyToBudgetProgram.Visible = CurrentUser.IsSuperAdmin || (CurrentUser.CurrentUserWebSite.BudgetManagement && CurrentUser.CurrentUserWebSite.IsBudgetAdmin);
        }

        public void SetAdminControl(bool enable)
        {
            top_5_tab.Visible = enable;
            top_5.Visible = enable;
            txtInternalID.Enabled = enable;
            txtEmployeeID.Enabled = enable;
            txtHireDate.Enabled = enable;
            chkIsPartTime.Enabled = enable;
            chkAutoAssignGroup.Enabled = enable;
            //chkAutoAssignBudget.Enabled = enable;
            btnSave2.Enabled = enable;
        }

        public void InitializePage()
        {
            try
            {
                InitializeTabs(top_1_tab, top_1, top_2_tab, top_2, top_3_tab, top_3, top_4_tab, top_4);

                BindPaymentTerm();
                BindCustomField();

                ddlAddressPermission.Enabled = !_UserWebSite.IsNew;
                btnSendWelcomeEmail.Visible = !_UserWebSite.IsNew;

                if (!_UserWebSite.IsNew)
                {
                    BindDefaultAddress();

                    this.txtFirstName.Text = _UserWebSite.UserInfo.FirstName;
                    this.txtLastName.Text = _UserWebSite.UserInfo.LastName;
                    this.txtEmailAddress.Text = _UserWebSite.UserInfo.EmailAddress;
                    this.txtNotificationEmailAddress.Text = _UserWebSite.NotificationEmail;

                    chkDisplayNotificationEmailAtCheckout.Checked = _UserWebSite.DisplayNotificaitonEmailAtCheckout;
                    chkDisableNotificationEmail.Checked = _UserWebSite.DisableNotificationEmail;

                    this.txtUsername.Text = _UserWebSite.UserInfo.UserName;
                    this.ddlPaymentTerm.SelectedIndex = this.ddlPaymentTerm.Items.IndexOf(this.ddlPaymentTerm.Items.FindByValue(_UserWebSite.PaymentTermID));

                    this.txtPaymentTermAmount.Text = Convert.ToString(_UserWebSite.PaymentTermAmount);
                    this.txtPaymentTermStartDate.Text = _UserWebSite.PaymentTermStartDate == null ? string.Empty : Convert.ToDateTime(_UserWebSite.PaymentTermStartDate).ToShortDateString();
                    //this.txtPaymentTermEndDate.Text = _UserWebSite.PaymentTermStartDate == null ? string.Empty : Convert.ToDateTime(_UserWebSite.PaymentTermStartDate).ToShortDateString();

                    if (_UserWebSite.PaymentTermBalance > 0)
                    {
                        this.txtPaymentTermBalance.Text = Convert.ToString(_UserWebSite.PaymentTermBalance);
                    }

                    this.chkIsPendingApproval.Checked = _UserWebSite.IsPendingApproval;
                    this.chkIsAdmin.Checked = _UserWebSite.IsAdmin;
                    this.chkWebsiteManagement.Checked = _UserWebSite.WebsiteManagement;
                    this.chkStoreManagement.Checked = _UserWebSite.StoreManagement;
                    this.chkUserManagement.Checked = _UserWebSite.UserManagement;
                    this.chkGroupManagement.Checked = _UserWebSite.GroupManagement;
                    this.chkTabManagement.Checked = _UserWebSite.TabManagement;
                    this.chkItemManagement.Checked = _UserWebSite.ItemManagement;
                    this.chkBudgetManagement.Checked = _UserWebSite.BudgetManagement;
                    this.chkIsBudgetAdmin.Checked = _UserWebSite.IsBudgetAdmin;
                    this.chkIsBudgetViewOnly.Checked = _UserWebSite.IsBudgetViewOnly;
                    this.chkOrderManagement.Checked = _UserWebSite.OrderManagement;
                    this.chkCreditCardManagement.Checked = _UserWebSite.CreditCardManagement;
                    this.chkShippingManagement.Checked = _UserWebSite.ShippingManagement;
                    this.chkMessageManagement.Checked = _UserWebSite.MessageManagement;
                    this.chkOptInForNotification.Checked = _UserWebSite.OptInForNotification;

                    this.chkEnableEmailOptIn.Checked = _UserWebSite.EnableEmailOptIn;
                    this.chkEnableSMSOptIn.Checked = _UserWebSite.EnableSMSOptIn;

                    this.chkTaxExempt.Checked = _UserWebSite.IsTaxExempt;
                    this.chkRequestTaxExempt.Checked = _UserWebSite.RequestTaxExempt;

                    this.txtInternalID.Text = _UserWebSite.CustomerInternalID;
                    this.txtEmployeeID.Text = _UserWebSite.EmployeeID;
                    this.txtHireDate.Text = _UserWebSite.HiredDate == null ? string.Empty : Convert.ToDateTime(_UserWebSite.HiredDate).ToShortDateString();

                    this.chkIsPartTime.Checked = _UserWebSite.IsPartTime;
                    this.chkIsStore.Checked = _UserWebSite.IsStore;
                    this.chkAutoAssignGroup.Checked = _UserWebSite.AutoAssignGroup;
                    //this.chkAutoAssignBudget.Checked = _UserWebSite.AutoAssignBudget;
                    this.chkAllowPasswordUpdate.Checked = _UserWebSite.AllowPasswordUpdate;

                    pnlWebsiteManagement.Visible = CurrentUser.CurrentUserWebSite.WebsiteManagement;
                    pnlStoreManagement.Visible = CurrentUser.CurrentUserWebSite.StoreManagement;
                    pnlUserManagement.Visible = CurrentUser.CurrentUserWebSite.UserManagement;
                    pnlGroupManagement.Visible = CurrentUser.CurrentUserWebSite.GroupManagement;
                    pnlTabManagement.Visible = CurrentUser.CurrentUserWebSite.TabManagement;
                    pnlItemManagement.Visible = CurrentUser.CurrentUserWebSite.ItemManagement;
                    pnlBudgetManagement.Visible = CurrentUser.CurrentUserWebSite.BudgetManagement;
                    pnlOrderManagement.Visible = CurrentUser.CurrentUserWebSite.OrderManagement;
                    pnlCreditCardManagement.Visible = CurrentUser.CurrentUserWebSite.CreditCardManagement;
                    pnlShippingManagement.Visible = CurrentUser.CurrentUserWebSite.ShippingManagement;
                    pnlMessageManagement.Visible = CurrentUser.CurrentUserWebSite.MessageManagement;

                    chkHideOrderApproval.Visible = CurrentUser.CurrentUserWebSite.OrderManagement;

                    pnlAllowPasswordUpdate.Visible = CurrentUser.IsSuperAdmin;

                    this.txtFirstName.Enabled = CurrentUser.IsSuperAdmin;
                    this.txtLastName.Enabled = CurrentUser.IsSuperAdmin;
                    this.txtEmailAddress.Enabled = CurrentUser.IsSuperAdmin;

                    this.phChangePassword.Visible = CurrentUser.IsSuperAdmin || CurrentUser.CurrentUserWebSite.AllowPasswordUpdate;

                    ddlAddressPermission.SelectedValue = Convert.ToString(_UserWebSite.AddressPermission);
                    if(!string.IsNullOrEmpty(ddlAddressPermission.SelectedValue))
                    {
                        ddlDefaultShippingAddress.SelectedValue = _UserWebSite.DefaultShippingAddressID;
                        ddlDefaultBillingAddress.SelectedValue = _UserWebSite.DefaultBillingAddressID;
                    }

                    ddlBudgetSetting.SelectedValue = _UserWebSite.BudgetSetting;

                    chkInactive.Checked = _UserWebSite.InActive;

                    chkApplyToBudgetProgram.Checked = _UserWebSite.ApplyBudgetProgram;

                    chkHideOrderApproval.Checked = _UserWebSite.HideOrderApproval;
                    chkHideInventoryReport.Checked = _UserWebSite.HideInventoryReport;

                    txtPackageAvailableDate.Text = _UserWebSite.PackageAvailableDate == null ? string.Empty : Convert.ToDateTime(_UserWebSite.PackageAvailableDate).ToShortDateString();

                    chkAllowOnlySSO.Checked = _UserWebSite.UserInfo.AllowOnlySSO;

                    BindUserAccounts();
                    BindBudgets();
                    btnSave.Text = "Save";
                }
                else
                {
                    ddlPaymentTerm.SelectedIndex = this.ddlPaymentTerm.Items.IndexOf(this.ddlPaymentTerm.Items.FindByValue(CurrentUser.CurrentUserWebSite.WebSite.DefaultPaymentTermID));

                    this.txtFirstName.Enabled = true;
                    this.txtLastName.Enabled = true;
                    this.txtEmailAddress.Enabled = true;
                    this.phChangePassword.Visible = true;
                    btnChangePassword.Visible = false;
                    phPassword.Visible = true;

                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                    this.top_2_tab.Visible = false;
                }
                aCancel.HRef = ReturnURL;
                chkIsAdmin_CheckedChanged(null, null);

                pnlAddressPermissionDefault.Visible = Convert.ToString(ddlAddressPermission.SelectedValue) == "Default";

                //Disable "Inactive" for EBA / Mavis
                chkInactive.Enabled = CurrentWebsite.WebsiteID != "20" && CurrentWebsite.WebsiteID != "25" && CurrentWebsite.WebsiteID != "26";
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
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
                CustomFieldFilter.Location.SearchString = "user";
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
        protected void BindUserAccounts()
        {
            try
            {
                gvUserAccount.DataSource = _UserWebSite.UserAccounts;
                gvUserAccount.DataBind();

                if (this.gvUserAccount.HeaderRow != null) this.gvUserAccount.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindBudgets()
        {
            try
            {
                this.gvBudgetAssignments.DataSource = _UserWebSite.MyBudgetAssignments;
                this.gvBudgetAssignments.DataBind();

                if (this.gvBudgetAssignments.HeaderRow != null) this.gvBudgetAssignments.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
        protected void BindPaymentTerm()
        {
            try
            {
                this.ddlPaymentTerm.DataSource = ImageSolutions.Payment.PaymentTerm.GetPaymentTerms();
                this.ddlPaymentTerm.DataBind();
                this.ddlPaymentTerm.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
        protected void BindDefaultAddress()
        {
            try
            {
                ddlDefaultShippingAddress.Items.Clear();
                ddlDefaultShippingAddress.Items.Add(new ListItem(string.Empty, string.Empty));

                ddlDefaultBillingAddress.Items.Clear();
                ddlDefaultBillingAddress.Items.Add(new ListItem(string.Empty, string.Empty));

                foreach (ImageSolutions.Address.AddressBook _AddressBook in _UserWebSite.UserInfo.AddressBooks)
                {
                    ddlDefaultShippingAddress.Items.Add(new ListItem(_AddressBook.GetDisplayFormat(false), _AddressBook.AddressBookID));
                    ddlDefaultBillingAddress.Items.Add(new ListItem(_AddressBook.GetDisplayFormat(false), _AddressBook.AddressBookID));
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;
            UserInfo objUserInfo = null;
            bool blnReturn = false;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                _UserWebSite.IsPendingApproval = this.chkIsPendingApproval.Checked;
                _UserWebSite.IsAdmin = this.chkIsAdmin.Checked;
                _UserWebSite.WebsiteManagement = this.chkWebsiteManagement.Checked;
                _UserWebSite.StoreManagement = this.chkStoreManagement.Checked;
                _UserWebSite.UserManagement = this.chkUserManagement.Checked;
                _UserWebSite.GroupManagement = this.chkGroupManagement.Checked;
                _UserWebSite.TabManagement = this.chkTabManagement.Checked;
                _UserWebSite.ItemManagement = this.chkItemManagement.Checked;
                _UserWebSite.BudgetManagement = this.chkBudgetManagement.Checked;
                _UserWebSite.IsBudgetAdmin = this.chkIsBudgetAdmin.Checked;
                _UserWebSite.IsBudgetViewOnly = this.chkIsBudgetViewOnly.Checked;
                _UserWebSite.OrderManagement = this.chkOrderManagement.Checked;
                _UserWebSite.CreditCardManagement = this.chkCreditCardManagement.Checked;
                _UserWebSite.ShippingManagement = this.chkShippingManagement.Checked;
                _UserWebSite.MessageManagement = this.chkMessageManagement.Checked;
                _UserWebSite.OptInForNotification = this.chkOptInForNotification.Checked;

                _UserWebSite.EnableEmailOptIn = this.chkEnableEmailOptIn.Checked;
                _UserWebSite.EnableSMSOptIn = this.chkEnableSMSOptIn.Checked;

                _UserWebSite.HideInventoryReport = chkHideInventoryReport.Checked;
                _UserWebSite.HideOrderApproval = chkHideOrderApproval.Checked;

                _UserWebSite.RequestTaxExempt = this.chkRequestTaxExempt.Checked;
                _UserWebSite.IsTaxExempt = this.chkTaxExempt.Checked;

                _UserWebSite.PaymentTermID = this.ddlPaymentTerm.SelectedValue;
                _UserWebSite.PaymentTermAmount = string.IsNullOrEmpty(txtPaymentTermAmount.Text.Trim()) ? (decimal?)null : Convert.ToDecimal(txtPaymentTermAmount.Text.Trim());                
                _UserWebSite.PaymentTermStartDate = string.IsNullOrEmpty(txtPaymentTermStartDate.Text.Trim()) ? (DateTime?)null : Convert.ToDateTime(txtPaymentTermStartDate.Text.Trim());
                //_UserWebSite.PaymentTermEndDate = string.IsNullOrEmpty(txtPaymentTermEndDate.Text.Trim()) ? (DateTime?)null : Convert.ToDateTime(txtPaymentTermEndDate.Text.Trim());

                _UserWebSite.NotificationEmail = this.txtNotificationEmailAddress.Text;
                _UserWebSite.DisplayNotificaitonEmailAtCheckout = chkDisplayNotificationEmailAtCheckout.Checked;
                _UserWebSite.DisableNotificationEmail = chkDisableNotificationEmail.Checked;

                _UserWebSite.CustomerInternalID = txtInternalID.Text;
                _UserWebSite.EmployeeID = txtEmployeeID.Text;
                _UserWebSite.HiredDate = string.IsNullOrEmpty(txtHireDate.Text) ? (DateTime?) null : Convert.ToDateTime(txtHireDate.Text);
                _UserWebSite.IsPartTime = chkIsPartTime.Checked;
                _UserWebSite.IsStore = chkIsStore.Checked;
                _UserWebSite.AutoAssignGroup = chkAutoAssignGroup.Checked;
                //_UserWebSite.AutoAssignBudget = chkAutoAssignBudget.Checked;
                _UserWebSite.AllowPasswordUpdate = chkAllowPasswordUpdate.Checked;

                _UserWebSite.AddressPermission = ddlAddressPermission.SelectedValue;

                if (Convert.ToString(_UserWebSite.AddressPermission) == "Default")
                {
                    _UserWebSite.DefaultShippingAddressID = ddlDefaultShippingAddress.SelectedValue;
                    _UserWebSite.DefaultBillingAddressID = ddlDefaultBillingAddress.SelectedValue;

                    if(string.IsNullOrEmpty(_UserWebSite.DefaultShippingAddressID) || string.IsNullOrEmpty(_UserWebSite.DefaultBillingAddressID))
                    {
                        throw new Exception("Default Shipping Address and Default Billing Address are required");
                    }
                }

                _UserWebSite.BudgetSetting = ddlBudgetSetting.SelectedValue;
                _UserWebSite.ApplyBudgetProgram = chkApplyToBudgetProgram.Checked;

                _UserWebSite.PackageAvailableDate = string.IsNullOrEmpty(txtPackageAvailableDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtPackageAvailableDate.Text);

                _UserWebSite.InActive = chkInactive.Checked;

                if (_UserWebSite.IsNew)
                {
                    UserInfo ExistUserInfo = new UserInfo();
                    UserInfoFilter ExistUserInfoFilter = new UserInfoFilter();
                    ExistUserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                    ExistUserInfoFilter.EmailAddress.SearchString = txtEmailAddress.Text;
                    ExistUserInfo = UserInfo.GetUserInfo(ExistUserInfoFilter);

                    if (ExistUserInfo != null && !string.IsNullOrEmpty(ExistUserInfo.UserInfoID))
                    {
                        ImageSolutions.User.UserWebsite ExistUserWebsite = new ImageSolutions.User.UserWebsite();
                        UserWebsiteFilter ExistUserWebSiteFilter = new UserWebsiteFilter();
                        ExistUserWebSiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        ExistUserWebSiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                        ExistUserWebSiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                        ExistUserWebSiteFilter.UserInfoID.SearchString = ExistUserInfo.UserInfoID;
                        ExistUserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(ExistUserWebSiteFilter);

                        if (ExistUserWebsite != null && !string.IsNullOrEmpty(ExistUserWebsite.UserWebsiteID))
                        {
                            throw new Exception("User already exists");
                        }
                        else
                        {
                            string strMessage = String.Format("User {0} already exists.  User has been added to {1}.", txtEmailAddress.Text, CurrentWebsite.Name);

                            //Confirmation Message?
                        }

                        _UserWebSite.UserInfoID = ExistUserInfo.UserInfoID;
                    }
                    else
                    {
                        objUserInfo = new UserInfo();
                        objUserInfo.FirstName = this.txtFirstName.Text.Trim();
                        objUserInfo.LastName = this.txtLastName.Text.Trim();
                        objUserInfo.EmailAddress = this.txtEmailAddress.Text.Trim();
                        objUserInfo.UserName = this.txtUsername.Text.Trim();
                        objUserInfo.Password = this.txtPassword.Text.Trim();

                        objUserInfo.AllowOnlySSO = this.chkAllowOnlySSO.Checked;

                        objUserInfo.Create(objConn, objTran);

                        _UserWebSite.UserInfoID = objUserInfo.UserInfoID;
                    }

                    _UserWebSite.WebsiteID = CurrentWebsite.WebsiteID;
                    _UserWebSite.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _UserWebSite.Create(objConn, objTran);

                    ReturnURL = "/admin/userwebsite.aspx?id=" + _UserWebSite.UserWebsiteID + "&tab=2";
                }
                else
                {
                    objUserInfo = new UserInfo(_UserWebSite.UserInfoID);
                    objUserInfo.FirstName = this.txtFirstName.Text.Trim();
                    objUserInfo.LastName = this.txtLastName.Text.Trim();
                    objUserInfo.UserName = this.txtUsername.Text.Trim();
                    objUserInfo.EmailAddress = this.txtEmailAddress.Text.Trim();
                    if (phPassword.Visible)
                    {
                        if(objUserInfo.Password != this.txtPassword.Text.Trim())
                        {
                            objUserInfo.RequirePasswordReset = false;
                        }
                        objUserInfo.Password = this.txtPassword.Text.Trim();
                    }

                    objUserInfo.AllowOnlySSO = this.chkAllowOnlySSO.Checked;

                    objUserInfo.Update(objConn, objTran);

                    blnReturn = _UserWebSite.Update(objConn, objTran);
                }

                if (
                    (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "20")
                    ||
                    (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "53")
                )
                {
                    ImageSolutions.Enterprise.EnterpriseCustomer EnterpriseCustomer = new ImageSolutions.Enterprise.EnterpriseCustomer();
                    ImageSolutions.Enterprise.EnterpriseCustomerFilter EnterpriseCustomerFilter = new ImageSolutions.Enterprise.EnterpriseCustomerFilter();
                    EnterpriseCustomerFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
                    EnterpriseCustomerFilter.EmployeeID.SearchString = _UserWebSite.EmployeeID;
                    EnterpriseCustomer = ImageSolutions.Enterprise.EnterpriseCustomer.GetEnterpriseCustomer(EnterpriseCustomerFilter);

                    if (EnterpriseCustomer != null && !string.IsNullOrEmpty(EnterpriseCustomer.EnterpriseCustomerID))
                    {
                        EnterpriseCustomer.IsUpdated = true;
                        EnterpriseCustomer.Update(objConn, objTran);
                    }
                }

                objTran.Commit();
            }
            catch (Exception ex)
            {
                objTran.Rollback();
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }

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
                blnReturn = _UserWebSite.Delete();
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

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                if(!CurrentWebsite.IsPunchout)
                {
                    blnReturn = CurrentUser.LoginAs(_UserWebSite.UserInfo.EmailAddress);
                }
                else
                {
                    //blnReturn = CurrentUser.LoginAs(_UserWebSite.UserInfo.EmailAddress, _UserWebSite.UserInfo.Password, true, _UserWebSite.GUID);

                    blnReturn = CurrentUser.Login(_UserWebSite.UserInfo.EmailAddress, _UserWebSite.UserInfo.Password, true);
                    blnReturn = CurrentUser.CurrentUserWebSite.Login(_UserWebSite.GUID);
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

            if (blnReturn)
            {
                Response.Redirect("/myaccount/dashboard.aspx");
            }
        }

        protected string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        protected void chkIsAdmin_CheckedChanged(object sender, EventArgs e)
        {
            //this.phPermission.Enabled = this.chkIsAdmin.Checked;
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            phPassword.Visible = !phPassword.Visible;
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
                    RequiredFieldValidator valCustomField = (RequiredFieldValidator)e.Item.FindControl("valCustomField");
                    HiddenField hfCustomValueID = (HiddenField)e.Item.FindControl("hfCustomValueID");

                    ImageSolutions.Custom.CustomValue CustomValue = new ImageSolutions.Custom.CustomValue();
                    ImageSolutions.Custom.CustomValueFilter CustomValueFilter = new ImageSolutions.Custom.CustomValueFilter();
                    CustomValueFilter.CustomFieldID = new Database.Filter.StringSearch.SearchFilter();
                    CustomValueFilter.CustomFieldID.SearchString = CustomField.CustomFieldID;
                    CustomValueFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    CustomValueFilter.UserWebsiteID.SearchString = mUserWebsiteID;
                    CustomValue = ImageSolutions.Custom.CustomValue.GetCustomValue(CustomValueFilter);

                    if (CustomField.Type == "dropdown")
                    {
                        txtCustomValue.Visible = false;
                        ddlCustomValueList.Visible = true;

                        List<ImageSolutions.Custom.CustomValueList> CustomValueLists = new List<ImageSolutions.Custom.CustomValueList>();
                        ImageSolutions.Custom.CustomValueListFilter CustomValueListFilter = new ImageSolutions.Custom.CustomValueListFilter();
                        CustomValueListFilter.CustomFieldID = new Database.Filter.StringSearch.SearchFilter();
                        CustomValueListFilter.CustomFieldID.SearchString = CustomField.CustomFieldID;
                        CustomValueListFilter.Inactive = false;
                        CustomValueLists = ImageSolutions.Custom.CustomValueList.GetCustomValueLists(CustomValueListFilter);

                        ddlCustomValueList.DataSource = CustomValueLists;
                        ddlCustomValueList.DataBind();
                        ddlCustomValueList.Items.Insert(0, new ListItem(String.Empty, string.Empty));

                        if (CustomValue != null)
                        {
                            ddlCustomValueList.SelectedValue = Convert.ToString(CustomValue.Value);
                            hfCustomValueID.Value = CustomValue.CustomValueID;
                        }
                    }
                    else
                    {
                        txtCustomValue.Visible = true;
                        ddlCustomValueList.Visible = false;

                        if (CustomValue != null)
                        {
                            txtCustomValue.Text = Convert.ToString(CustomValue.Value);
                            hfCustomValueID.Value = CustomValue.CustomValueID;
                        }
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
                        CustomValue.UserWebsiteID = _UserWebSite.UserWebsiteID; //CurrentUser.CurrentUserWebSite.UserWebsiteID; //SP
                    }
                    else
                    {
                        CustomValue = new ImageSolutions.Custom.CustomValue(strCustomValueID);
                    }

                    ImageSolutions.Custom.CustomField CustomField = new ImageSolutions.Custom.CustomField(strCustomFieldID);

                    if (CustomField.Type == "dropdown")
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

                    if(CustomValue.IsNew)
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
        }

        protected void btnSendWelcomeEmail_Click(object sender, EventArgs e)
        {
            UserInfo UserInfo = null;

            try
            {
                string strTempPassword = RandomString(8);

                UserInfo = new UserInfo(_UserWebSite.UserInfoID);
                UserInfo.Password = strTempPassword;
                UserInfo.RequirePasswordReset = true;
                UserInfo.Update();

                SendWelcomeEmail(UserInfo, UserInfo.EmailAddress, strTempPassword);
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
                UserInfo = null;
            }
        }

        protected bool SendWelcomeEmail(UserInfo userinfo, string toemailaddress, string password)
        {
            string strHTMLContent = String.Empty;

            try
            {
                strHTMLContent = String.Format(@"
Dear {0},
<br />
<br />
Welcome to the {1} website powered by Image Solutions!
<br />
<br />
Your username is {2} and temporary password is {3} 
<br />
<br />
To access the web site please visit:
<br />
{4}
<br />
<br />
You will be prompted to change your password when you log in.
<br />
<br />
For login support please email loginsupport@imageinc.com
<br />
<br />
Thank you,
Image Solutions Team 
"
                    , string.Format(@"{0} {1}", userinfo.FirstName, userinfo.LastName)
                    , string.Format(@"{0}", CurrentWebsite.Name)
                    , string.Format(@"{0}", string.IsNullOrEmpty(userinfo.UserName) ? userinfo.EmailAddress : userinfo.UserName)
                    , string.Format(@"{0}", password)
                    , string.Format(@"{0}", string.IsNullOrEmpty(CurrentWebsite.Domain) ? Convert.ToString(ConfigurationManager.AppSettings["WebsiteURL"]) : "https://" + CurrentWebsite.Domain)                   
                );

                SendEmail(toemailaddress, string.Format(@"Welcome to the {0} website", CurrentWebsite.Name), strHTMLContent);

                WebUtility.DisplayJavascriptMessage(this, "Welcome email has been sent");
            }
            catch { }
            finally { }
            return true;
        }

        protected void ddlAddressPermission_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlAddressPermissionDefault.Visible = Convert.ToString(ddlAddressPermission.SelectedValue) == "Default";
        }

        protected void gvBudgetAssignments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Cells[5].Visible = CurrentUser.IsSuperAdmin || CurrentUser.CurrentUserWebSite.BudgetManagement;
        }
    }
}   
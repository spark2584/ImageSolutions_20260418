using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Management;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class Budget : BasePageAdminUserWebSiteAuth
    {
        private string mBudgetID = string.Empty;

        private ImageSolutions.Budget.Budget mBudget = null;
        protected ImageSolutions.Budget.Budget _Budget
        {
            get
            {
                if (mBudget == null)
                {
                    if (string.IsNullOrEmpty(mBudgetID))
                        mBudget = new ImageSolutions.Budget.Budget();
                    else
                        mBudget = new ImageSolutions.Budget.Budget(mBudgetID);
                }
                return mBudget;
            }
            set
            {
                mBudget = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/BudgetOverview.aspx";
                else
                    return ViewState["ReturnURL"].ToString();
            }
            set
            {
                ViewState["ReturnURL"] = value;
            }
        }

        private List<ImageSolutions.SalesOrder.SalesOrder> mSalesOrders = null;
        public List<ImageSolutions.SalesOrder.SalesOrder> SalesOrders
        {
            get
            {
                mSalesOrders = _Budget.SalesOrders;

                //if (CurrentUser.CurrentUserWebSite.IsAdmin)
                //    mSalesOrders = _Budget.SalesOrders.FindAll(m => CurrentUser.CurrentUserWebSite.Accounts.Exists(n => n.AccountID == m.AccountID) || CurrentUser.CurrentUserWebSite.Accounts.Exists(n => n.ChildAccounts.Exists(o => o.AccountID == m.AccountID)));
                //else
                //    mSalesOrders = _Budget.SalesOrders.FindAll(m => CurrentUser.CurrentUserWebSite.Accounts.Exists(n => n.AccountID == m.AccountID) || CurrentUser.CurrentUserWebSite.Accounts.Exists(n => n.ChildAccounts.Exists(o => o.AccountID == m.AccountID)));

                    if (!string.IsNullOrEmpty(this.ddlAccount.SelectedValue))
                    mSalesOrders = mSalesOrders.FindAll(m => m.AccountID == this.ddlAccount.SelectedValue);

                if (!string.IsNullOrEmpty(this.txtOrderStartDate.Text.Trim()))
                {
                    DateTime dtStartDate = DateTime.Now;
                    if (!DateTime.TryParse(this.txtOrderStartDate.Text.Trim(), out dtStartDate)) throw new Exception("Invalid Start Date Format");
                    mSalesOrders = mSalesOrders.FindAll(m => m.CreatedOn >= Convert.ToDateTime(this.txtOrderStartDate.Text.Trim()));
                }

                if (!string.IsNullOrEmpty(this.txtOrderEndDate.Text.Trim()))
                {
                    DateTime dtEndDate = DateTime.Now;
                    if (!DateTime.TryParse(this.txtOrderEndDate.Text.Trim(), out dtEndDate)) throw new Exception("Invalid End Date Format");
                    mSalesOrders = mSalesOrders.FindAll(m => m.CreatedOn < Convert.ToDateTime(this.txtOrderEndDate.Text.Trim()).AddDays(1));
                }
                return mSalesOrders;
            }
        }


        private List<ImageSolutions.Budget.BudgetAssignmentAdjustment> mBudgetAssignmentAdjustments = null;
        public List<ImageSolutions.Budget.BudgetAssignmentAdjustment> BudgetAssignmentAdjustments
        {
            get
            {
                mBudgetAssignmentAdjustments = _Budget.BudgetAssignmentAdjustments;

                if (!string.IsNullOrEmpty(this.txtAdjustmentStartDate.Text.Trim()))
                {
                    DateTime dtStartDate = DateTime.Now;
                    if (!DateTime.TryParse(this.txtAdjustmentStartDate.Text.Trim(), out dtStartDate)) throw new Exception("Invalid Start Date Format");
                    mBudgetAssignmentAdjustments = mBudgetAssignmentAdjustments.FindAll(m => m.CreatedOn >= Convert.ToDateTime(this.txtAdjustmentStartDate.Text.Trim()));
                }

                if (!string.IsNullOrEmpty(this.txtAdjustmentEndDate.Text.Trim()))
                {
                    DateTime dtEndDate = DateTime.Now;
                    if (!DateTime.TryParse(this.txtAdjustmentEndDate.Text.Trim(), out dtEndDate)) throw new Exception("Invalid End Date Format");
                    mBudgetAssignmentAdjustments = mBudgetAssignmentAdjustments.FindAll(m => m.CreatedOn < Convert.ToDateTime(this.txtAdjustmentEndDate.Text.Trim()).AddDays(1));
                }

                return mBudgetAssignmentAdjustments;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            mBudgetID = Request.QueryString.Get("id");

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

                    if(!string.IsNullOrEmpty(hfUserWebsiteID.Value))
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
                InitializeTabs(top_1_tab, top_1, top_2_tab, top_2, top_3_tab, top_3, top_4_tab, top_4);

                BindPaymentTerm();
                BindWebsiteShippingService();

                if (CurrentWebsite.UserWebsites.Count <= 100)
                {
                    ddlUserWebsite.Visible = true;
                    BindUserWebsite();
                    txtUserWebsite.Visible = false;
                }
                else
                {
                    ddlUserWebsite.Visible = false;
                    txtUserWebsite.Visible = true;

                    hfUserWebsiteID.Value = Convert.ToString(_Budget.ApproverUserWebsiteID);
                }

                this.ddlAccount.DataSource = CurrentUser.CurrentUserWebSite.Accounts.OrderBy(m => m.AccountNamePath);
                this.ddlAccount.DataBind();
                this.ddlAccount.Items.Insert(0, new ListItem("All", ""));

                if (!_Budget.IsNew)
                {

                    if (_Budget.IsSystemGenerated && !CurrentUser.IsSuperAdmin)
                    {
                        txtName.Enabled = false;
                        txtStartDate.Enabled = false;
                        txtEndDate.Enabled = false;
                        txtDivision.Enabled = false;
                        txtBudgetAmount.Enabled = false;
                        cbIncludeShippingAndTaxes.Enabled = false;

                        cbDisplayShipping.Enabled = false;
                        cbTaxNonBudgetAmount.Enabled = false;

                        ddlWebsiteShippingService.Enabled = false;
                        cbAllowOverBudget.Enabled = false;

                        cbExcludeNoAmountBudget.Enabled = false;

                        ddlPaymentTerm.Enabled = false;
                        ddlUserWebsite.Enabled = false;
                        btnUserWebsiteSearch.Enabled = false;
                        btnUserWebsiteRemove.Enabled = false;
                    }

                    txtName.Text = _Budget.BudgetName;
                    txtStartDate.Text = _Budget.StartDate.ToShortDateString();
                    txtEndDate.Text = _Budget.EndDate.ToShortDateString();
                    txtBudgetAmount.Text = _Budget.BudgetAmount.ToString();
                    cbIncludeShippingAndTaxes.Checked = _Budget.IncludeShippingAndTaxes;

                    cbDisplayShipping.Checked = _Budget.DisplayShippingAndTaxes;
                    cbTaxNonBudgetAmount.Checked = _Budget.TaxNonBudgetAmount;

                    ddlWebsiteShippingService.SelectedValue = _Budget.WebsiteShippingServiceID;
                    cbAllowOverBudget.Checked = _Budget.AllowOverBudget;

                    cbExcludeNoAmountBudget.Checked = _Budget.ExcludeNoAmountBudget;

                    ddlPaymentTerm.SelectedIndex = this.ddlPaymentTerm.Items.IndexOf(this.ddlPaymentTerm.Items.FindByValue(_Budget.PaymentTermID));
                    if (ddlUserWebsite.Visible)
                    {
                        ddlUserWebsite.SelectedValue = Convert.ToString(_Budget.ApproverUserWebsiteID);
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

                    txtDivision.Text = _Budget.Division;

                    btnSave.Text = "Save";

                    BindBudgetAssignment();
                    BindBudgetAssignmentUser();
                    BindUserBalance();
                    BindSalesOrders();
                    BindAdjustments();
                }
                else
                {
                    if (!CurrentUser.IsSuperAdmin && !CurrentUser.CurrentUserWebSite.IsBudgetAdmin)
                    {
                        ddlUserWebsite.SelectedValue = Convert.ToString(CurrentUser.CurrentUserWebSite.UserWebsiteID);
                        hfUserWebsiteID.Value = Convert.ToString(CurrentUser.CurrentUserWebSite.UserWebsiteID);
                        txtUserWebsite.Text = Convert.ToString(CurrentUser.CurrentUserWebSite.Description);
                    }

                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                    top_2_tab.Visible = false;
                    top_3_tab.Visible = false;
                    top_4_tab.Visible = false;
                    top_5_tab.Visible = false;
                }
                aCancel.HRef = ReturnURL;

                btnUserWebsiteRemove.Visible = !string.IsNullOrEmpty(hfUserWebsiteID.Value);

                if (ddlUserWebsite.Visible)
                {
                    btnUserWebsiteSearch.Visible = false;
                    btnUserWebsiteRemove.Visible = false;
                }

                if (!CurrentUser.IsSuperAdmin && !CurrentUser.CurrentUserWebSite.IsBudgetAdmin)
                {
                    btnUserWebsiteSearch.Visible = false;
                    btnUserWebsiteRemove.Visible = false;
                }

                if (!string.IsNullOrEmpty(CurrentWebsite.BudgetAlias))
                {
                    lblAlias.Text = "CurrentWebsite.BudgetAlias";
                }

                lblEmail.Text = CurrentWebsite.HideEmail ? "Employee ID:" : "Email:";
                txtFilterBudgetUserEmail.Attributes.Add("placeholder", CurrentWebsite.HideEmail ? "employee id" : "email");
                lblBalanceEmailFilter.Text = CurrentWebsite.HideEmail ? "Employee ID:" : "Email:";
                txtFilterBalanceEmail.Attributes.Add("placeholder", CurrentWebsite.HideEmail ? "employee id" : "email");

                cbDisplayShipping.Enabled = !cbIncludeShippingAndTaxes.Checked;
                cbTaxNonBudgetAmount.Enabled = !cbIncludeShippingAndTaxes.Checked;

                btnDelete.Enabled = !CurrentUser.CurrentUserWebSite.IsBudgetViewOnly;
                btnSave.Enabled = !CurrentUser.CurrentUserWebSite.IsBudgetViewOnly;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindBudgetAssignment()
        {
            try
            {
                this.gvBudgetGroup.DataSource = _Budget.BudgetAssignments.FindAll(m => !string.IsNullOrEmpty(m.WebsiteGroupID));
                this.gvBudgetGroup.DataBind();
                if (this.gvBudgetGroup.HeaderRow != null) this.gvBudgetGroup.HeaderRow.TableSection = TableRowSection.TableHeader;


                //this.gvBudgetUser.DataSource = _Budget.BudgetAssignments.FindAll(m => !string.IsNullOrEmpty(m.UserWebsiteID));
                //this.gvBudgetUser.DataBind();
                //if (this.gvBudgetUser.HeaderRow != null) this.gvBudgetUser.HeaderRow.TableSection = TableRowSection.TableHeader;

            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindBudgetAssignmentUser()
        {
            List<ImageSolutions.Budget.BudgetAssignment> BudgetAssignments = null;
            ImageSolutions.Budget.BudgetAssignmentFilter BudgetAssignmentFilter = null;
            int intTotalRecord = 0;

            try
            {
                BudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
                BudgetAssignmentFilter.BudgetID = new Database.Filter.StringSearch.SearchFilter();
                BudgetAssignmentFilter.BudgetID.SearchString = _Budget.BudgetID;

                if (string.IsNullOrEmpty(txtFilterBudgetUserEmail.Text))
                {
                    BudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    BudgetAssignmentFilter.UserWebsiteID.Operator = Database.Filter.StringSearch.SearchOperator.notEmpty;
                }
                else
                {
                    ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                    ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                    UserWebsiteFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                    UserWebsiteFilter.EmailAddress.SearchString = txtFilterBudgetUserEmail.Text;
                    UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    UserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                    UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                    if (UserWebsite == null)
                    {
                        UserWebsite = new ImageSolutions.User.UserWebsite();
                        UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                        UserWebsiteFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.EmployeeID.SearchString = txtFilterBudgetUserEmail.Text;
                        UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                        UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                        if (UserWebsite == null)
                        {
                            if (CurrentWebsite.HideEmail)
                                throw new Exception("Invalid Employee ID");
                            else
                                throw new Exception("Invalid Email");
                        }
                    }

                    BudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    BudgetAssignmentFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                    txtFilterBalanceEmail.Text = String.Empty;
                }

                BudgetAssignments = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignments(BudgetAssignmentFilter, ucBudgetUserPager.PageSize, ucBudgetUserPager.CurrentPageNumber, out intTotalRecord);

                gvBudgetUser.DataSource = BudgetAssignments; 
                gvBudgetUser.DataBind();

                ucBudgetUserPager.TotalRecord = intTotalRecord;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }


        protected void BindSalesOrders()
        {
            int intTotalRecord = 0;
            try
            {
                intTotalRecord = SalesOrders.Count();
                gvSalesOrders.DataSource = SalesOrders.OrderByDescending(x => Convert.ToInt32(x.SalesOrderID)).ToList().Skip((ucSalesOrderPager.CurrentPageNumber - 1) * ucSalesOrderPager.PageSize).Take(ucSalesOrderPager.PageSize);
                gvSalesOrders.DataBind();

                ucSalesOrderPager.TotalRecord = intTotalRecord;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindAdjustments()
        {
            int intTotalRecord = 0;
            try
            {
                intTotalRecord = BudgetAssignmentAdjustments.Count();
                gvAdjustments.DataSource = BudgetAssignmentAdjustments.OrderByDescending(x => Convert.ToInt32(x.BudgetAssignmentAdjustmentID)).ToList().Skip((ucAdjustmentPager.CurrentPageNumber - 1) * ucAdjustmentPager.PageSize).Take(ucAdjustmentPager.PageSize);
                gvAdjustments.DataBind();

                ucAdjustmentPager.TotalRecord = intTotalRecord;
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
        protected void BindWebsiteShippingService()
        {
            try
            {
                List<ImageSolutions.Website.WebsiteShippingService> WebsiteShippingServices = new List<ImageSolutions.Website.WebsiteShippingService>();
                ImageSolutions.Website.WebsiteShippingServiceFilter WebsiteShippingServiceFilter = new ImageSolutions.Website.WebsiteShippingServiceFilter();
                WebsiteShippingServiceFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteShippingServiceFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                WebsiteShippingServices = ImageSolutions.Website.WebsiteShippingService.GetWebsiteShippingServices(WebsiteShippingServiceFilter);
                this.ddlWebsiteShippingService.DataSource = WebsiteShippingServices;
                this.ddlWebsiteShippingService.DataBind();
                this.ddlWebsiteShippingService.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
        protected void ddlAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            ucSalesOrderPager.CurrentPageNumber = 1;
            BindSalesOrders();
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            ucSalesOrderPager.CurrentPageNumber = 1;
            BindSalesOrders();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                if (!cbIncludeShippingAndTaxes.Checked && string.IsNullOrEmpty(ddlWebsiteShippingService.SelectedValue))
                {
                    throw new Exception("Default Shipping required when shipping and taxes are not included");
                }

                _Budget.BudgetName = txtName.Text;
                _Budget.StartDate = Convert.ToDateTime(txtStartDate.Text.Trim());
                _Budget.EndDate = Convert.ToDateTime(txtEndDate.Text.Trim());
                _Budget.BudgetAmount = Convert.ToDouble(txtBudgetAmount.Text.Trim());

                _Budget.IncludeShippingAndTaxes = cbIncludeShippingAndTaxes.Checked;

                _Budget.DisplayShippingAndTaxes = cbDisplayShipping.Checked;
                _Budget.TaxNonBudgetAmount = cbTaxNonBudgetAmount.Checked;

                _Budget.WebsiteShippingServiceID = ddlWebsiteShippingService.SelectedValue;
                _Budget.AllowOverBudget = cbAllowOverBudget.Checked;

                _Budget.ExcludeNoAmountBudget = cbExcludeNoAmountBudget.Checked;

                _Budget.PaymentTermID = ddlPaymentTerm.SelectedValue;
                _Budget.ApproverUserWebsiteID = ddlUserWebsite.Visible && !string.IsNullOrEmpty(ddlUserWebsite.SelectedValue) ? Convert.ToString(ddlUserWebsite.SelectedValue) : Convert.ToString(hfUserWebsiteID.Value);

                _Budget.Division = txtDivision.Text;

                if (_Budget.IsNew)
                {
                    _Budget.WebsiteID = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;
                    _Budget.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _Budget.Create();
                    ReturnURL = "/admin/budget.aspx?id=" + _Budget.BudgetID + "&tab=2";
                }
                else
                {
                    blnReturn = _Budget.Update();
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
                blnReturn = _Budget.Delete();
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

        protected void BindUserBalance()
        {
            int intTotalRecord = 0;

            List<ImageSolutions.Budget.BudgetAssignment> BudgetAssignments = new List<ImageSolutions.Budget.BudgetAssignment>();
            ImageSolutions.Budget.BudgetAssignmentFilter BudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
            BudgetAssignmentFilter.BudgetID = new Database.Filter.StringSearch.SearchFilter();
            BudgetAssignmentFilter.BudgetID.SearchString = _Budget.BudgetID;
            BudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
            BudgetAssignmentFilter.UserWebsiteID.Operator = Database.Filter.StringSearch.SearchOperator.notEmpty;
            BudgetAssignments = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignments(BudgetAssignmentFilter);


            if (BudgetAssignments.Count < 10 || !string.IsNullOrEmpty(txtFilterBalanceEmail.Text))
            {
                List<ImageSolutions.Budget.MyBudgetAssignment> MyBudgetAssignments = new List<ImageSolutions.Budget.MyBudgetAssignment>();

                List<ImageSolutions.Budget.BudgetAssignment> UserBudgetAssignments = new List<ImageSolutions.Budget.BudgetAssignment>();
                ImageSolutions.Budget.BudgetAssignmentFilter UserBudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
                UserBudgetAssignmentFilter.BudgetID = new Database.Filter.StringSearch.SearchFilter();
                UserBudgetAssignmentFilter.BudgetID.SearchString = _Budget.BudgetID;
                
                if(string.IsNullOrEmpty(txtFilterBalanceEmail.Text))
                {
                    UserBudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    UserBudgetAssignmentFilter.UserWebsiteID.Operator = Database.Filter.StringSearch.SearchOperator.notEmpty;
                }
                else
                {
                    ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                    ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();

                    if (CurrentWebsite.HideEmail)
                    {
                        UserWebsiteFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.EmployeeID.SearchString = txtFilterBalanceEmail.Text;
                    }
                    else
                    {
                        UserWebsiteFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.EmailAddress.SearchString = txtFilterBalanceEmail.Text;
                    }
                    UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    UserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                    UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                    if(UserWebsite == null)
                    {
                        throw new Exception("Invalid Email");
                    }

                    UserBudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    UserBudgetAssignmentFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                    txtFilterBalanceEmail.Text = String.Empty;
                }

                UserBudgetAssignments = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignments(UserBudgetAssignmentFilter);

                //foreach (ImageSolutions.Budget.BudgetAssignment _BudgetAssignment in _Budget.BudgetAssignments.FindAll(m => !string.IsNullOrEmpty(m.UserWebsiteID)))
                foreach (ImageSolutions.Budget.BudgetAssignment _BudgetAssignment in UserBudgetAssignments)
                {
                    if (!MyBudgetAssignments.Exists(x => x.UserInfoID == _BudgetAssignment.UserWebsite.UserInfoID && x.BudgetAssignmentID == _BudgetAssignment.BudgetAssignmentID))
                    {
                        ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(_BudgetAssignment.UserWebsite.UserInfoID, _BudgetAssignment.BudgetAssignmentID);
                        MyBudgetAssignments.Add(MyBudgetAssignment);
                    }
                }

                List<ImageSolutions.Budget.BudgetAssignment> GroupBudgetAssignments = new List<ImageSolutions.Budget.BudgetAssignment>();
                ImageSolutions.Budget.BudgetAssignmentFilter GroupBudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
                GroupBudgetAssignmentFilter.BudgetID = new Database.Filter.StringSearch.SearchFilter();
                GroupBudgetAssignmentFilter.BudgetID.SearchString = _Budget.BudgetID;
                GroupBudgetAssignmentFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                GroupBudgetAssignmentFilter.WebsiteGroupID.Operator = Database.Filter.StringSearch.SearchOperator.notEmpty;
                GroupBudgetAssignments = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignments(GroupBudgetAssignmentFilter);

                //foreach (ImageSolutions.Budget.BudgetAssignment _BudgetAssignment in _Budget.BudgetAssignments.FindAll(m => !string.IsNullOrEmpty(m.WebsiteGroupID)))
                foreach (ImageSolutions.Budget.BudgetAssignment _BudgetAssignment in GroupBudgetAssignments)
                {
                    List<ImageSolutions.User.UserAccount> UserAccounts = new List<ImageSolutions.User.UserAccount>();
                    ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                    UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                    UserAccountFilter.WebsiteGroupID.SearchString = _BudgetAssignment.WebsiteGroupID;
                    UserAccounts = ImageSolutions.User.UserAccount.GetUserAccounts(UserAccountFilter);

                    foreach (ImageSolutions.User.UserAccount _UserAccount in UserAccounts)
                    {
                        if (!MyBudgetAssignments.Exists(x => x.UserInfoID == _UserAccount.UserWebsite.UserInfoID && x.BudgetAssignmentID == _BudgetAssignment.BudgetAssignmentID))
                        {
                            ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(_UserAccount.UserWebsite.UserInfoID, _BudgetAssignment.BudgetAssignmentID);
                            MyBudgetAssignments.Add(MyBudgetAssignment);
                        }
                    }
                }
                intTotalRecord = MyBudgetAssignments.Count();
                gvUserBalance.DataSource = MyBudgetAssignments.OrderBy(x => Convert.ToString(x.UserInfo.FirstName)).ToList().Skip((ucBudgetBalancePager.CurrentPageNumber - 1) * ucBudgetBalancePager.PageSize).Take(ucBudgetBalancePager.PageSize);
                gvUserBalance.DataBind();

                ucBudgetBalancePager.TotalRecord = intTotalRecord;
            }
        }

        protected void lbnDownload_Click(object sender, EventArgs e)
        {
            //http://www.dotnetspeaks.com/DisplayArticle.aspx?ID=97

            try
            {
                if (SalesOrders == null || SalesOrders.Count == 0) throw new Exception("No results found");

                DataTable dt = new DataTable();

                dt.Columns.Add("OrderNumber");
                dt.Columns.Add("TransactionDate");
                dt.Columns.Add("Store");
                dt.Columns.Add("FirstName");
                dt.Columns.Add("LastName");
                dt.Columns.Add("Email");
                dt.Columns.Add("ShippingAmount");
                dt.Columns.Add("TaxAmount");
                dt.Columns.Add("TotalAmount");
                dt.Columns.Add("ItemName");
                dt.Columns.Add("ItemNumber");
                dt.Columns.Add("ItemDescription");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("UnitPrice");

                foreach (ImageSolutions.SalesOrder.SalesOrder objSalesOrder in SalesOrders)
                {
                    foreach (ImageSolutions.SalesOrder.SalesOrderLine objSalesOrderLine in objSalesOrder.SalesOrderLines)
                    {
                        DataRow objRow = dt.NewRow();
                        objRow["OrderNumber"] = objSalesOrder.SalesOrderID;
                        objRow["TransactionDate"] = objSalesOrder.TransactionDate.ToString();
                        objRow["Store"] = objSalesOrder.Account.AccountName;
                        objRow["FirstName"] = objSalesOrder.UserInfo.FirstName;
                        objRow["LastName"] = objSalesOrder.UserInfo.LastName;
                        objRow["Email"] = objSalesOrder.UserInfo.EmailAddress;
                        objRow["ShippingAmount"] = objSalesOrder.ShippingAmount;
                        objRow["TaxAmount"] = objSalesOrder.TaxAmount;
                        objRow["TotalAmount"] = objSalesOrder.Total;

                        objRow["ItemName"] = objSalesOrderLine.Item.ItemName;
                        objRow["ItemNumber"] = objSalesOrderLine.Item.ItemNumber;
                        objRow["ItemDescription"] = objSalesOrderLine.Item.Description;
                        objRow["Quantity"] = objSalesOrderLine.Quantity;
                        objRow["UnitPrice"] = objSalesOrderLine.UnitPrice;
                        dt.Rows.Add(objRow);
                    }
                }

                //Double dimensional array to keep style name and style
                string[,] styles = { { "text", "\\@;" },
                               { "text", "\\@;" },
                               { "text", "\\@;" },
                               { "text", "\\@;" },
                               { "text", "\\@;" },
                               { "text", "\\@;" },
                               { "number", "0\\.00;" },
                               { "number", "0\\.00;" },
                               { "number", "0\\.00;" },
                               { "text", "\\@;" },
                               { "text", "\\@;" },
                               { "text", "\\@;" },
                               { "number", "0\\.00;" },
                               { "number", "0\\.00;" } };

                //Dummy GridView to hold data to be exported in excel
                System.Web.UI.WebControls.GridView gvExport = new System.Web.UI.WebControls.GridView();
                gvExport.AllowPaging = false;
                gvExport.DataSource = dt;
                gvExport.DataBind();

                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                int cnt = styles.Length / 2;

                for (int i = 0; i < gvExport.Rows.Count; i++)
                {
                    for (int j = 0; j < cnt; j++)
                    {
                        //Apply style to each cell
                        gvExport.Rows[i].Cells[j].Attributes.Add("class", styles[j, 0]);
                    }
                }

                gvExport.RenderControl(hw);
                StringBuilder style = new StringBuilder();
                style.Append("<style>");
                for (int j = 0; j < cnt; j++)
                {
                    style.Append("." + styles[j, 0] + " { mso-number-format:" + styles[j, 1] + " }");
                }

                style.Append("</style>");
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=order_export.xls"); Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Response.Write(style.ToString());
                //string headerTable = @"<Table><tr><td></td><td>Report Header</td></tr><tr><td>second</td></tr></Table>";
                //Response.Output.Write(headerTable);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void ucBudgetUserPager_PostBackPageIndexChanging(object sender, EventArgs e)
        {
            BindBudgetAssignmentUser();
        }

        protected void ucBudgetBalancePager_PostBackPageIndexChanging(object sender, EventArgs e)
        {
            BindUserBalance();
        }

        protected void ucSalesOrderPager_PostBackPageIndexChanging(object sender, EventArgs e)
        {
            BindSalesOrders();
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

        protected void txtFilterBalanceEmail_TextChanged(object sender, EventArgs e)
        {
            try
            {
                BindUserBalance();                
            }
            catch(Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }

            if (top_4_tab != null)
            {
                top_4_tab.Attributes.Remove("class");
                top_4_tab.Attributes.Add("class", "nav-link active");
                top_4.Attributes.Remove("class");
                top_4.Attributes.Add("class", "tab-pane fade show active");
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

            if (top_3_tab != null)
            {
                top_3_tab.Attributes.Remove("class");
                top_3_tab.Attributes.Add("class", "nav-link");
                top_3.Attributes.Remove("class");
                top_3.Attributes.Add("class", "tab-pane fade");
            }

            if (top_5_tab != null)
            {
                top_5_tab.Attributes.Remove("class");
                top_5_tab.Attributes.Add("class", "nav-link");
                top_5.Attributes.Remove("class");
                top_5.Attributes.Add("class", "tab-pane fade");
            }
        }

        protected void txtFilterBudgetUserEmail_TextChanged(object sender, EventArgs e)
        {
            try
            {
                BindBudgetAssignmentUser();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
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
        }

        protected void txtAdjustmentStartDate_TextChanged(object sender, EventArgs e)
        {
            ucAdjustmentPager.CurrentPageNumber = 1;
            BindAdjustments();
        }

        protected void txtAdjustmentEndDate_TextChanged(object sender, EventArgs e)
        {
            ucAdjustmentPager.CurrentPageNumber = 1;
            BindAdjustments();
        }

        protected void lbnAdjustmentDownload_Click(object sender, EventArgs e)
        {
            //http://www.dotnetspeaks.com/DisplayArticle.aspx?ID=97

            try
            {
                if (BudgetAssignmentAdjustments == null || BudgetAssignmentAdjustments.Count == 0) throw new Exception("No results found");

                DataTable dt = new DataTable();

                dt.Columns.Add("AdjustmentDate");
                dt.Columns.Add("FirstName");
                dt.Columns.Add("LastName");
                dt.Columns.Add("Email");
                dt.Columns.Add("Reason");
                dt.Columns.Add("Amount");
                dt.Columns.Add("CreatedBy");

                //Enterprise
                if (CurrentWebsite.WebsiteID == "53" || CurrentWebsite.WebsiteID == "20")
                {
                    dt.Columns.Add("group");
                    dt.Columns.Add("group_branch");
                }

                foreach (ImageSolutions.Budget.BudgetAssignmentAdjustment _BudgetAssignmentAdjustment in BudgetAssignmentAdjustments)
                {
                    DataRow objRow = dt.NewRow();
                    objRow["AdjustmentDate"] = _BudgetAssignmentAdjustment.CreatedOn;
                    objRow["FirstName"] = _BudgetAssignmentAdjustment.BudgetAssignment.UserWebsite.UserInfo.FirstName;
                    objRow["LastName"] = _BudgetAssignmentAdjustment.BudgetAssignment.UserWebsite.UserInfo.LastName;
                    objRow["Email"] = _BudgetAssignmentAdjustment.BudgetAssignment.UserWebsite.UserInfo.EmailAddress;
                    objRow["Reason"] = _BudgetAssignmentAdjustment.Reason;
                    objRow["Amount"] = _BudgetAssignmentAdjustment.Amount;
                    objRow["CreatedBy"] = _BudgetAssignmentAdjustment.CreatedBy == "1" || _BudgetAssignmentAdjustment.CreatedBy == "3" ? "System" : _BudgetAssignmentAdjustment.CreatedByUser.FullName;

                    if (CurrentWebsite.WebsiteID == "53" || CurrentWebsite.WebsiteID == "20")
                    {
                        objRow["group"] = _BudgetAssignmentAdjustment.BudgetAssignment.UserWebsite.UserAccounts[0].Account.StoreNumber.Substring(0, 2);
                        objRow["group_branch"] = _BudgetAssignmentAdjustment.BudgetAssignment.UserWebsite.UserAccounts[0].Account.StoreNumber;
                    }

                    dt.Rows.Add(objRow);
                }

                //Double dimensional array to keep style name and style
                string[,] styles = { { "text", "\\@;" },
                               { "text", "\\@;" },
                               { "text", "\\@;" },
                               { "text", "\\@;" },
                               { "text", "\\@;" },
                               { "number", "0\\.00;" },
                               { "text", "\\@;" },
                };

                //Dummy GridView to hold data to be exported in excel
                System.Web.UI.WebControls.GridView gvExport = new System.Web.UI.WebControls.GridView();
                gvExport.AllowPaging = false;
                gvExport.DataSource = dt;
                gvExport.DataBind();

                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                int cnt = styles.Length / 2;

                for (int i = 0; i < gvExport.Rows.Count; i++)
                {
                    for (int j = 0; j < cnt; j++)
                    {
                        //Apply style to each cell
                        gvExport.Rows[i].Cells[j].Attributes.Add("class", styles[j, 0]);
                    }
                }

                gvExport.RenderControl(hw);
                StringBuilder style = new StringBuilder();
                style.Append("<style>");
                for (int j = 0; j < cnt; j++)
                {
                    style.Append("." + styles[j, 0] + " { mso-number-format:" + styles[j, 1] + " }");
                }

                style.Append("</style>");
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=adjustment_export.xls"); Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                Response.Write(style.ToString());
                //string headerTable = @"<Table><tr><td></td><td>Report Header</td></tr><tr><td>second</td></tr></Table>";
                //Response.Output.Write(headerTable);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void ucAdjustmentPager_PostBackPageIndexChanging(object sender, EventArgs e)
        {
            BindAdjustments();            
        }

        protected void gvBudgetUser_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Hide Email if website is set to hide email
            e.Row.Cells[1].Visible = !CurrentWebsite.HideEmail;
            e.Row.Cells[2].Visible = CurrentWebsite.HideEmail;
        }

        protected void gvUserBalance_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Cells[1].Visible = CurrentWebsite.HideEmail; 
            e.Row.Cells[2].Visible = !CurrentWebsite.HideEmail;
        }

        protected void cbIncludeShippingAndTaxes_CheckedChanged(object sender, EventArgs e)
        {
            cbDisplayShipping.Enabled = !cbIncludeShippingAndTaxes.Checked;
            cbTaxNonBudgetAmount.Enabled = !cbIncludeShippingAndTaxes.Checked;
        }
    }
}
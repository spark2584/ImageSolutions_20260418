using ImageSolutions.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class BudgetAssignmentUser : BasePageAdminUserWebSiteAuth
    {
        protected string mBudgetAssignmentID = string.Empty;
        protected string mBudgetID = string.Empty;

        private ImageSolutions.Budget.BudgetAssignment mBudgetAssignment = null;
        protected ImageSolutions.Budget.BudgetAssignment _BudgetAssignment
        {
            get
            {
                if (mBudgetAssignment == null)
                {
                    if (string.IsNullOrEmpty(mBudgetAssignmentID))
                        mBudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                    else
                        mBudgetAssignment = new ImageSolutions.Budget.BudgetAssignment(mBudgetAssignmentID);
                }
                return mBudgetAssignment;
            }
            set
            {
                mBudgetAssignment = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/Budget.aspx?id=" + (_BudgetAssignment.IsNew ? mBudgetID : _BudgetAssignment.BudgetID) + "&tab=3";
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
            mBudgetAssignmentID = Request.QueryString.Get("id");
            mBudgetID = Request.QueryString.Get("budgetid");

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
                //BindUserWebsites();
                if (CurrentWebsite.UserWebsites.Count <= 100)
                {
                    ddlUserWebsite.Visible = true;
                    BindUserWebsites();
                    txtUserWebsite.Visible = false;
                }
                else
                {
                    ddlUserWebsite.Visible = false;
                    txtUserWebsite.Visible = true;

                    hfUserWebsiteID.Value = Convert.ToString(_BudgetAssignment.UserWebsiteID);
                }


                if (!_BudgetAssignment.IsNew)
                {                   
                    if (ddlUserWebsite.Visible)
                    {
                        this.ddlUserWebsite.SelectedIndex = this.ddlUserWebsite.Items.IndexOf(this.ddlUserWebsite.Items.FindByValue(_BudgetAssignment.UserWebsiteID));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(hfUserWebsiteID.Value))
                        {
                            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite(hfUserWebsiteID.Value);
                            if (UserWebsite != null && !string.IsNullOrEmpty(_BudgetAssignment.UserWebsiteID))
                            {
                                txtUserWebsite.Text = UserWebsite.Description;
                            }

                        }
                        else
                        {
                            txtUserWebsite.Text = String.Empty;
                        }
                    }

                    cbInactive.Checked = _BudgetAssignment.InActive;
                    btnSave.Text = "Save";
                }
                else
                {
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                }
                aCancel.HRef = ReturnURL;

                btnAdjustment.Visible = !string.IsNullOrEmpty(mBudgetAssignmentID);

                btnAdjustment.Enabled = !CurrentUser.CurrentUserWebSite.IsBudgetViewOnly;
                btnDelete.Enabled = !CurrentUser.CurrentUserWebSite.IsBudgetViewOnly;
                btnSave.Enabled = !CurrentUser.CurrentUserWebSite.IsBudgetViewOnly;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindUserWebsites()
        {
            try
            {
                ddlUserWebsite.DataSource = CurrentUser.CurrentUserWebSite.WebSite.UserWebsites;
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
                //_BudgetAssignment.UserWebsiteID = ddlUserWebsite.SelectedValue;

                _BudgetAssignment.UserWebsiteID = ddlUserWebsite.Visible && !string.IsNullOrEmpty(ddlUserWebsite.SelectedValue) ? Convert.ToString(ddlUserWebsite.SelectedValue) : Convert.ToString(hfUserWebsiteID.Value);
                _BudgetAssignment.InActive = cbInactive.Checked;

                if (_BudgetAssignment.IsNew)
                {
                    if (CurrentWebsite.IsOneBudgetPerUser)
                    {
                        ImageSolutions.Budget.BudgetAssignment BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                        ImageSolutions.Budget.BudgetAssignmentFilter BudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
                        BudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        BudgetAssignmentFilter.UserWebsiteID.SearchString = _BudgetAssignment.UserWebsiteID;
                        BudgetAssignment = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignment(BudgetAssignmentFilter);
                        
                        if (BudgetAssignment != null && !string.IsNullOrEmpty(BudgetAssignment.BudgetAssignmentID) && !BudgetAssignment.InActive)
                        {
                            throw new Exception("Cannot have multiple active budgets");
                        }
                    }

                    _BudgetAssignment.BudgetID = mBudgetID;
                    _BudgetAssignment.WebsiteID = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;
                    _BudgetAssignment.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _BudgetAssignment.Create();
                }
                else
                {
                    blnReturn = _BudgetAssignment.Update();
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
                blnReturn = _BudgetAssignment.Delete();
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

        protected void btnAdjustment_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("/Admin/BudgetAssignmentAdjustment.aspx?budgetassignmentid={0}",mBudgetAssignmentID));
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class BudgetAssignmentAdjustment : BasePageAdminUserWebSiteAuth
    {
        protected string mBudgetAssignmentID = string.Empty;
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
                    return "/Admin/Budget.aspx?id=" + (_BudgetAssignment.BudgetID) + "&tab=3";
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
            mBudgetAssignmentID = Request.QueryString.Get("budgetassignmentid");

            if(string.IsNullOrEmpty(mBudgetAssignmentID))
            {
                Response.Redirect("/Admin/BudgetOverview.aspx");
            }

            if (!Page.IsPostBack)
            {
                InitializePage();
            }
        }
        public void InitializePage()
        {
            try
            {
                ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(_BudgetAssignment.UserWebsite.UserInfoID, _BudgetAssignment.BudgetAssignmentID);
                txtCurrentBalance.Text = MyBudgetAssignment.Balance.ToString("c");
                aCancel.HRef = ReturnURL;

                btnSave.Enabled = !CurrentUser.CurrentUserWebSite.IsBudgetViewOnly;


                pnlEnterpriseReason.Visible = false;
                pnlReason.Visible = true;

                pnlSubmitPayroll.Visible = CurrentUser.IsSuperAdmin;

                if (CurrentWebsite.WebsiteID == "20" || CurrentWebsite.WebsiteID == "53")
                {
                    pnlEnterpriseReason.Visible = true;
                    pnlReason.Visible = false;
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
            bool blnReturn = false;
            ImageSolutions.Budget.BudgetAssignmentAdjustment BudgetAssignmentAdjustment = null;

            try
            {

                BudgetAssignmentAdjustment = new ImageSolutions.Budget.BudgetAssignmentAdjustment();
                BudgetAssignmentAdjustment.BudgetAssignmentID = mBudgetAssignmentID;
                BudgetAssignmentAdjustment.Amount = Convert.ToDecimal(txtAmount.Text);

                decimal dbBalance = Convert.ToDecimal(BudgetAssignmentAdjustment.BudgetAssignment.Budget.BudgetAmount) - Convert.ToDecimal(BudgetAssignmentAdjustment.BudgetAssignment.Payments.Sum(x => x.AmountPaid)) + Convert.ToDecimal(BudgetAssignmentAdjustment.BudgetAssignment.BudgetAssignmentAdjustments.Sum(x => x.Amount));
                if (dbBalance + Convert.ToDecimal(txtAmount.Text) < 0)
                {
                    throw new Exception("Insufficient balance for the requested adjustment");
                }

                BudgetAssignmentAdjustment.SubmitPayroll = chkSubmitPayroll.Checked;

                BudgetAssignmentAdjustment.Reason = Convert.ToString(txtReason.Text);

                if (pnlEnterpriseReason.Visible)
                {
                    if (string.IsNullOrEmpty(ddlEnterpriseReason.SelectedValue))
                    {
                        throw new Exception("Reason is required");
                    }

                    if (ddlEnterpriseReason.SelectedValue == "Other")
                    {
                        if(string.IsNullOrEmpty(txtEnterpriseOtherReason.Text))
                        {
                            throw new Exception("Reason is required");
                        }

                        BudgetAssignmentAdjustment.Reason = txtEnterpriseOtherReason.Text;
                    }
                    else
                    {
                        BudgetAssignmentAdjustment.Reason = ddlEnterpriseReason.SelectedValue;
                    }
                }

                BudgetAssignmentAdjustment.CreatedBy = CurrentUser.UserInfoID;

                blnReturn = BudgetAssignmentAdjustment.Create();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally 
            {
                BudgetAssignmentAdjustment = null;
            }

            if (blnReturn)
            {
                Response.Redirect(ReturnURL);
            }
        }

        protected void ddlEnterpriseReason_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtEnterpriseOtherReason.Visible = ddlEnterpriseReason.SelectedValue == "Other";
        }
    }
}
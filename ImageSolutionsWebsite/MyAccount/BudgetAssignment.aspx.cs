using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace ImageSolutionsWebsite.MyAccount
{
    public partial class BudgetAssignment : BasePageUserAccountAuth
    {
        private string mBudgetAssignmentID = string.Empty;

        private ImageSolutions.Budget.MyBudgetAssignment mMyBudgetAssignment = null;
        protected ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment
        {
            get
            {
                if (mMyBudgetAssignment == null)
                {
                    if (!string.IsNullOrEmpty(mBudgetAssignmentID)) mMyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(CurrentUser.UserInfoID, mBudgetAssignmentID);
                    }
                return mMyBudgetAssignment;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/MyAccount/Budget.aspx";
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
                if (MyBudgetAssignment != null)
                {
                    txtName.Text = MyBudgetAssignment.BudgetAssignment.Budget.BudgetName;
                    txtStartDate.Text = MyBudgetAssignment.BudgetAssignment.Budget.StartDate.ToShortDateString();
                    txtEndDate.Text = MyBudgetAssignment.BudgetAssignment.Budget.EndDate.ToShortDateString();
                    txtBudgetAmount.Text = MyBudgetAssignment.BudgetAssignment.Budget.BudgetAmount.ToString("c");
                    txtAvailableAmount.Text = MyBudgetAssignment.Balance.ToString("c");

                    BindSalesOrders();
                    BindBudgetAssignmentAdjustment();
                }
                aCancel.HRef = ReturnURL;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindSalesOrders()
        {
            try
            {
                List<ImageSolutions.SalesOrder.SalesOrder> SalesOrders = new List<ImageSolutions.SalesOrder.SalesOrder>();

                foreach (ImageSolutions.Payment.Payment _Payment in MyBudgetAssignment.Payments)
                {
                    SalesOrders.Add(new ImageSolutions.SalesOrder.SalesOrder(_Payment.SalesOrderID));
                }

                this.gvSalesOrders.DataSource = SalesOrders;//MyBudgetAssignment.SalesOrders;
                this.gvSalesOrders.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                //Response.Redirect(String.Format("/Admin/CreateGroup.aspx?websiteid={0}", ddlWebsite.SelectedValue));
            }
            finally { }
        }

        protected void BindBudgetAssignmentAdjustment()
        {
            try
            {
                List<ImageSolutions.Budget.BudgetAssignmentAdjustment> BudgetAssignmentAdjustments = new List<ImageSolutions.Budget.BudgetAssignmentAdjustment>();
                ImageSolutions.Budget.BudgetAssignmentAdjustmentFilter BudgetAssignmentAdjustmentFilter = new ImageSolutions.Budget.BudgetAssignmentAdjustmentFilter();
                BudgetAssignmentAdjustmentFilter.BudgetAssignmentID = new Database.Filter.StringSearch.SearchFilter();
                BudgetAssignmentAdjustmentFilter.BudgetAssignmentID.SearchString = mBudgetAssignmentID;
                BudgetAssignmentAdjustments = ImageSolutions.Budget.BudgetAssignmentAdjustment.GetBudgetAssignmentAdjustments(BudgetAssignmentAdjustmentFilter);


                this.gvBudgetAssignmentAdjustment.DataSource = BudgetAssignmentAdjustments;
                this.gvBudgetAssignmentAdjustment.DataBind();

            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

    }
}
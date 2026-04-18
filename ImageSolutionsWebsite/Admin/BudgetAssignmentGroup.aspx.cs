using ImageSolutions.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class BudgetAssignmentGroup : BasePageAdminUserWebSiteAuth
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
                    return "/Admin/Budget.aspx?id=" + (_BudgetAssignment.IsNew ? mBudgetID : _BudgetAssignment.BudgetID) + "&tab=2";
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

            if (!Page.IsPostBack)
            {
                //if (Request.UrlReferrer != null) ReturnURL = Request.UrlReferrer.PathAndQuery;

                InitializePage();
            }
        }

        public void InitializePage()
        {
            try
            {
                BindWebsiteGroup();

                if (!_BudgetAssignment.IsNew)
                {
                    this.ddlGroup.SelectedValue = _BudgetAssignment.WebsiteGroupID;
                    btnSave.Text = "Save";
                }
                else
                {
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                }
                aCancel.HRef = ReturnURL;

                btnDelete.Enabled = !CurrentUser.CurrentUserWebSite.IsBudgetViewOnly;
                btnSave.Enabled = !CurrentUser.CurrentUserWebSite.IsBudgetViewOnly;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                //Response.Redirect(string.Format("/Admin/GroupOverview.aspx"));
            }
            finally { }
        }

        protected void BindWebsiteGroup()
        {
            try
            {
                ddlGroup.DataSource = CurrentUser.CurrentUserWebSite.WebSite.WebsiteGroups;
                ddlGroup.DataTextField = "GroupName";
                ddlGroup.DataValueField = "WebsiteGroupID";
                ddlGroup.DataBind();
                ddlGroup.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                //Response.Redirect(string.Format("/Admin/GroupOverview.aspx"));
            }
            finally { }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                _BudgetAssignment.WebsiteGroupID = ddlGroup.SelectedValue;

                if (_BudgetAssignment.IsNew)
                {
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
    }
}
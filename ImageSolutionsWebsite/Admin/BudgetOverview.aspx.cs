using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class BudgetOverview : BasePageAdminUserWebSiteAuth
    {
        protected string mBudgetName = string.Empty;
        protected string mEmail = string.Empty;
        protected string mDivision = string.Empty;
        protected string mActiveOnly = String.Empty;
        protected string mApprover = String.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            mBudgetName = Request.QueryString.Get("budgetname");
            mEmail = Request.QueryString.Get("email");
            mDivision = Request.QueryString.Get("division");
            mActiveOnly = Request.QueryString.Get("activeonly");
            mApprover = Request.QueryString.Get("approver");

            pnlApproverFilter.Visible = CurrentUser.IsSuperAdmin || CurrentUser.CurrentUserWebSite.IsBudgetAdmin;


            UpdateUserWebsite();
            

            if (!Page.IsPostBack)
            {
                InitializePage();
            }
        }

        public void InitializePage()
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

                hfUserWebsiteID.Value = Convert.ToString(mApprover);

                btnUserWebsiteSearch.Visible = true;
                btnUserWebsiteSearch.Enabled = true;
                btnUserWebsiteRemove.Enabled = true;
            }

            if (!string.IsNullOrEmpty(hfUserWebsiteID.Value))
            {
                if (ddlUserWebsite.Visible)
                {
                    this.ddlUserWebsite.SelectedIndex = this.ddlUserWebsite.Items.IndexOf(this.ddlUserWebsite.Items.FindByValue(hfUserWebsiteID.Value));
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
            }

            this.txtBudgetName.Text = mBudgetName;
            this.txtEmail.Text = mEmail;
            this.txtDivision.Text = mDivision;
            this.chkActiveOnly.Checked = mActiveOnly == "True" || string.IsNullOrEmpty(mActiveOnly);

            if (string.IsNullOrEmpty(this.txtBudgetName.Text.Trim()) && string.IsNullOrEmpty(this.txtEmail.Text.Trim()) && string.IsNullOrEmpty(this.txtDivision.Text.Trim()))
            {
                BindBudget();
            }
            else
            {
                BindBudgetWithSearch();
            }

            lblEmailFilter.Text = CurrentWebsite.HideEmail ? "Employee ID:" : "Email:";
            txtEmail.Attributes.Add("placeholder", CurrentWebsite.HideEmail ? "employee id" : "email");

            //For Enterprise EBA - Hide Import Button
            if (
                    (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && CurrentWebsite.WebsiteID == "53")
                    || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "20")
            )
            {
                aBudgetImport.Visible = false;
                aBudgetAdd.Visible = false;
            }
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

        protected void UpdateUserWebsite()
        {
            ucUserWebsiteSearchModal.SendMessageToThePage += delegate (string message)
            {
                try
                {
                    hfUserWebsiteID.Value = message;

                    if (!string.IsNullOrEmpty(hfUserWebsiteID.Value))
                    {
                        Response.Redirect(String.Format("/Admin/BudgetOverview.aspx?budgetname=" + Server.UrlEncode(this.txtBudgetName.Text.Trim())
                            + "&email=" + Server.UrlEncode(this.txtEmail.Text.Trim())
                            + "&activeonly=" + Server.UrlEncode(Convert.ToString(this.chkActiveOnly.Checked))
                            + "&approver=" + Server.UrlEncode(Convert.ToString(hfUserWebsiteID.Value))
                            )
                        );

                        //ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite(hfUserWebsiteID.Value);

                        //if (UserWebsite != null && !string.IsNullOrEmpty(UserWebsite.UserWebsiteID))
                        //    txtUserWebsite.Text = UserWebsite.Description;
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

        protected void BindBudget()
        {
            List<ImageSolutions.Budget.Budget> Budgets = null;
            ImageSolutions.Budget.BudgetFilter BudgetFilter = null;
            int intTotalRecord = 0;

            try
            {
                ucPager.Visible = true;

                BudgetFilter = new ImageSolutions.Budget.BudgetFilter();
                BudgetFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                BudgetFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;

                if(!CurrentUser.IsSuperAdmin && !CurrentUser.CurrentUserWebSite.IsBudgetAdmin)
                {
                    BudgetFilter.ApproverUserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    BudgetFilter.ApproverUserWebsiteID.SearchString = CurrentUser.CurrentUserWebSite.UserWebsiteID;
                }

                if (!string.IsNullOrEmpty(this.txtBudgetName.Text.Trim()))
                {
                    BudgetFilter.BudgetName = new Database.Filter.StringSearch.SearchFilter();
                    BudgetFilter.BudgetName.SearchString = this.txtBudgetName.Text.Trim();
                }

                if (!string.IsNullOrEmpty(this.txtEmail.Text.Trim()))
                {
                    BudgetFilter.Email = new Database.Filter.StringSearch.SearchFilter();
                    BudgetFilter.Email.SearchString = this.txtEmail.Text.Trim();
                }


                if (!string.IsNullOrEmpty(this.txtDivision.Text.Trim()))
                {
                    BudgetFilter.Division = new Database.Filter.StringSearch.SearchFilter();
                    BudgetFilter.Division.SearchString = this.txtDivision.Text.Trim();
                }

                if (chkActiveOnly.Checked)
                {
                    BudgetFilter.StartDate = new Database.Filter.DateTimeSearch.SearchFilter();
                    BudgetFilter.StartDate.Operator = Database.Filter.DateTimeSearch.SearchOperator.onOrBefore;
                    BudgetFilter.StartDate.From = DateTime.UtcNow.Date;
                    BudgetFilter.EndDate = new Database.Filter.DateTimeSearch.SearchFilter();
                    BudgetFilter.EndDate.Operator = Database.Filter.DateTimeSearch.SearchOperator.onOrAfter;
                    BudgetFilter.EndDate.From = DateTime.UtcNow.Date;
                }

                if (!string.IsNullOrEmpty(hfUserWebsiteID.Value))
                {
                    BudgetFilter.ApproverUserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    BudgetFilter.ApproverUserWebsiteID.SearchString = hfUserWebsiteID.Value;
                }

                Budgets = ImageSolutions.Budget.Budget.GetBudgets(BudgetFilter, ucPager.PageSize, ucPager.CurrentPageNumber, out intTotalRecord);

                this.gvBudget.DataSource = Budgets; //CurrentUser.CurrentUserWebSite.WebSite.Budgets;
                this.gvBudget.DataBind();

                ucPager.TotalRecord = intTotalRecord;
            }
            catch(Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindBudgetWithSearch()
        {
            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                ucPager.Visible = false;

                strSQL = string.Format(@"
SELECT DISTINCT b.BudgetID, b.BudgetName, b.BudgetAmount, b.StartDate, b.EndDate, b.CreatedOn
FROM Budget (NOLOCK) b
LEFT OUTER JOIN BudgetAssignment (NOLOCK) ba on ba.BudgetID = b.BudgetID 
LEFT OUTER JOIN UserWebsite (NOLOCK) uw on uw.UserWebsiteID = ba.UserWebsiteID
LEFT OUTER JOIN UserInfo (NOLOCK) u on u.UserInfoID = uw.UserInfoID 
WHERE b.WebsiteID = {0}
--and b.StartDate <= GETUTCDATE() and DATEADD(Day, 1, b.EndDate) >= GETUTCDATE() 
{1}
{2}
{3}
{4}
{5}
{6}
"
                        , Database.HandleQuote(CurrentWebsite.WebsiteID)
                        , !CurrentUser.IsSuperAdmin && !CurrentUser.CurrentUserWebSite.IsBudgetAdmin ? String.Format("and b.ApproverUserWebsiteID = {0}", Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID)) : String.Empty
                        , !string.IsNullOrEmpty(this.txtBudgetName.Text.Trim()) && this.txtBudgetName.Text.Trim().Length > 3 ? String.Format("and b.BudgetName like '%{0}%' ", this.txtBudgetName.Text.Trim()) : String.Empty
                        , !string.IsNullOrEmpty(this.txtEmail.Text.Trim()) && this.txtEmail.Text.Trim().Length > 3 ? String.Format("and u.EmailAddress like '%{0}%' ", this.txtEmail.Text.Trim()) : String.Empty
                        , !string.IsNullOrEmpty(this.txtDivision.Text.Trim()) && this.txtDivision.Text.Trim().Length > 3 ? String.Format("and b.Division like '%{0}%' ", this.txtDivision.Text.Trim()) : String.Empty
                        , chkActiveOnly.Checked ? string.Format("AND b.StartDate <= {0} and b.EndDate >= {0}", Database.HandleQuote(Convert.ToString(DateTime.UtcNow))) : string.Empty
                        , !string.IsNullOrEmpty(hfUserWebsiteID.Value) ? string.Format("AND b.ApproverUserWebsiteID = {0}", Database.HandleQuote(Convert.ToString(hfUserWebsiteID.Value))) : string.Empty
                        );

                objRead = Database.GetDataReader(strSQL);

                this.gvBudget.DataSource = objRead;
                this.gvBudget.DataBind();
            }
            catch
            {

            }
        }

        protected void txtBudgetName_TextChanged(object sender, EventArgs e)
        {
            //Response.Redirect(String.Format("/Admin/BudgetOverview.aspx?budgetname=" + Server.UrlEncode(this.txtBudgetName.Text.Trim()) + "&email=" + Server.UrlEncode(this.txtEmail.Text.Trim()) + "&activeonly=" + Server.UrlEncode(Convert.ToString(this.chkActiveOnly.Checked)) ));
            Response.Redirect(String.Format("/Admin/BudgetOverview.aspx?budgetname=" + Server.UrlEncode(this.txtBudgetName.Text.Trim())
                + "&email=" + Server.UrlEncode(this.txtEmail.Text.Trim())
                + "&division=" + Server.UrlEncode(this.txtDivision.Text.Trim())
                + "&activeonly=" + Server.UrlEncode(Convert.ToString(this.chkActiveOnly.Checked))
                + "&approver=" + Server.UrlEncode(Convert.ToString(hfUserWebsiteID.Value))
                )
            );
        }

        protected void txtEmail_TextChanged(object sender, EventArgs e)
        {
            //Response.Redirect(String.Format("/Admin/BudgetOverview.aspx?budgetname=" + Server.UrlEncode(this.txtBudgetName.Text.Trim()) + "&email=" + Server.UrlEncode(this.txtEmail.Text.Trim()) + "&activeonly=" + Server.UrlEncode(Convert.ToString(this.chkActiveOnly.Checked)) ));
            Response.Redirect(String.Format("/Admin/BudgetOverview.aspx?budgetname=" + Server.UrlEncode(this.txtBudgetName.Text.Trim())
                + "&email=" + Server.UrlEncode(this.txtEmail.Text.Trim())
                + "&division=" + Server.UrlEncode(this.txtDivision.Text.Trim())
                + "&activeonly=" + Server.UrlEncode(Convert.ToString(this.chkActiveOnly.Checked))
                + "&approver=" + Server.UrlEncode(Convert.ToString(hfUserWebsiteID.Value))
                )
            );

        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            string strSQL = string.Empty;
            //Hashtable dicParam = null;

            //List<ImageSolutions.Budget.Budget> Budgets = null;
            //ImageSolutions.Budget.BudgetFilter BudgetFilter = null;
            try
            {
                string strPath = Server.MapPath("\\Export\\Budget\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }

                string strFileExportPath = Server.MapPath(string.Format("\\Export\\Budget\\{0}\\Budget_{1}.csv", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")));
                //CreateExportCSV(strFileExportPath);
                CreateExportCSVBySQL(strFileExportPath);

                Response.ContentType = "text/csv";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + System.IO.Path.GetFileName(strFileExportPath));
                Response.WriteFile(strFileExportPath);

                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
        public void CreateExportCSV(string filepath)
        {
            StringBuilder objReturn = new StringBuilder();

            List<ImageSolutions.Budget.Budget> Budgets = null;
            ImageSolutions.Budget.BudgetFilter BudgetFilter = null;

            try
            {
                objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}"
                    , "id"
                    , "email"
                    , "name"
                    , "start_date"
                    , "end_date"
                    , "amount"
                    , "allow_overbudget"
                    , "payment_term"
                    , "approver"
                    , "division"));
                objReturn.AppendLine();

                BudgetFilter = new ImageSolutions.Budget.BudgetFilter();
                BudgetFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                BudgetFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;

                if (!CurrentUser.IsSuperAdmin && !CurrentUser.CurrentUserWebSite.IsBudgetAdmin)
                {
                    BudgetFilter.ApproverUserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    BudgetFilter.ApproverUserWebsiteID.SearchString = CurrentUser.CurrentUserWebSite.UserWebsiteID;
                }

                if (!string.IsNullOrEmpty(this.txtBudgetName.Text.Trim()))
                {
                    BudgetFilter.BudgetName = new Database.Filter.StringSearch.SearchFilter();
                    BudgetFilter.BudgetName.SearchString = this.txtBudgetName.Text.Trim();
                }

                if (!string.IsNullOrEmpty(this.txtEmail.Text.Trim()))
                {
                    BudgetFilter.Email = new Database.Filter.StringSearch.SearchFilter();
                    BudgetFilter.Email.SearchString = this.txtEmail.Text.Trim();
                }

                Budgets = ImageSolutions.Budget.Budget.GetBudgets(BudgetFilter);

                foreach(ImageSolutions.Budget.Budget _Budget in Budgets)
                {
                    objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}"
                        , Convert.ToString(_Budget.BudgetID)
                        , Convert.ToString(_Budget.BudgetAssignments[0].UserWebsite.UserInfo.EmailAddress)
                        , Convert.ToString(_Budget.BudgetName)
                        , Convert.ToString(_Budget.StartDate.ToString("MM/dd/yyyy"))
                        , Convert.ToString(_Budget.EndDate.ToString("MM/dd/yyyy"))
                        , Convert.ToString(_Budget.BudgetAmount)
                        , _Budget.AllowOverBudget ? "yes" : "no"
                        , _Budget.OverBudgetPaymentTerm == null ? string.Empty : Convert.ToString(_Budget.OverBudgetPaymentTerm.Description)
                        , _Budget.ApproverUserWebsite == null ? string.Empty : Convert.ToString(_Budget.ApproverUserWebsite.UserInfo.EmailAddress)
                        , Convert.ToString(_Budget.Division)
                    ));
                    objReturn.AppendLine();
                }


                if (objReturn != null)
                {
                    using (StreamWriter _streamwriter = new StreamWriter(filepath))
                    {
                        _streamwriter.Write(objReturn.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

        }

        protected void CreateExportCSVBySQL(string filepath)
        {
            StringBuilder objReturn = new StringBuilder();

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                //Enerprise
                if (
                    ( Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "20" ) 
                    || 
                    (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "53")
                )
                {
                    objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}"
                        , "id"
                        , "email"
                        , "name"
                        , "start_date"
                        , "end_date"
                        , "amount"
                        , "allow_overbudget"
                        , "payment_term"
                        , "approver"
                        , "division"
                        , "remaining_balance"
                        , "pending_amount"
                        , "amount_spent"
                        , "adjustment_amount"
                        , "group"
                        , "group_branch"
                        , "regional_group"
                        , "employee_name"
                        , "inactive"
                        , "hiredate"
                        , "jobcode"
                        , "is_airport"
                        , "airport_code"
                    ));
                }
                else
                {
                    objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}"
                        , "id"
                        , "email"
                        , "name"
                        , "start_date"
                        , "end_date"
                        , "amount"
                        , "allow_overbudget"
                        , "payment_term"
                        , "approver"
                        , "division"
                        , "remaining_balance"
                        , "pending_amount"
                        , "inactive" ));
                }
                objReturn.AppendLine();

                strSQL = string.Format(@"
SELECT b.BudgetID as id
	, u.EmailAddress as email
	, b.BudgetName as name
	, convert(varchar, b.StartDate, 101) as start_date
	, convert(varchar, b.EndDate, 101) as end_date
	, b.BudgetAmount as amount
	, CASE WHEN b.AllowOverBudget = 1 THEN 'yes' ELSE 'no' end as allow_overbudget
	, ISNULL(pt.Description,'') as payment_term
	, ISNULL(au.EmailAddress,'') as approver
    , ISNULL(b.Division,'') as division
	, CASE WHEN Balance.Amount < 0 THEN 0 ELSE Balance.Amount END as remaining_balance
	, ISNULL(Pending.Amount,0) as pending_amount
	, ISNULL(Payment.Amount,0) as amount_spent
	, SUBSTRING(account.StoreNumber,1,2) as [group]
	, ISNULL(account.StoreNumber,'') as group_branch
    , ISNULL(enterprisestore.BranchAdminLgcyID,'') as [regional_group]
    , ISNULL(u.FirstName,'') + ' ' + ISNULL(u.LastName,'') as employee_name

    {7}

FROM Budget (NOLOCK) b
Left Outer Join BudgetAssignment (NOLOCK) ba on ba.BudgetID = b.BudgetID
Left Outer Join UserWebsite (NOLOCK) ua on ua.UserWebsiteID = ba.UserWebsiteID
Left Outer Join UserInfo (NOLOCK) u on u.UserInfoID = ua.UserInfoID
Left Outer Join PaymentTerm (NOLOCK) pt on pt.PaymentTermID = b.PaymentTermID
Left Outer Join UserWebsite (NOLOCK) auw on auw .UserWebsiteID = b.ApproverUserWebsiteID
Left Outer Join UserInfo (NOLOCK) au on au.UserInfoID = auw.UserInfoID

{8}

Outer Apply
(
	SELECT SUM(p2.AmountPaid) Amount
	FROM Payment (NOLOCK) p2
	Inner Join SalesOrder (NOLOCK) s2 on s2.SalesOrderID = p2.SalesOrderID
	WHERE p2.BudgetAssignmentID = ba.BudgetAssignmentID
	and s2.Status != 'Rejected'
) Payment
Outer Apply
(
    SELECT SUM(p2.AmountPaid) Amount
	FROM Payment (NOLOCK) p2
	Inner Join SalesOrder (NOLOCK) s2 on s2.SalesOrderID = p2.SalesOrderID
	WHERE p2.BudgetAssignmentID = ba.BudgetAssignmentID
	and s2.Status != 'Rejected'
	and (s2.IsPendingApproval = 1 or s2.IsPendingItemPersonalizationApproval = 1)
) Pending

Outer Apply
(
	SELECT SUM(ba2.Amount) Amount
	FROM BudgetAssignmentAdjustment (NOLOCK) ba2
	WHERE ba2.BudgetAssignmentID = ba.BudgetAssignmentID
) Adjustmnet
Outer Apply
(
    SELECT b.BudgetAmount - ISNULL(Payment.Amount,0) + ISNULL(Adjustmnet.Amount,0) Amount
) Balance

Outer Apply
(
	SELECT Top 1 a2.StoreNumber
	FROM Account (NOLOCK) a2
	Inner Join UserAccount (NOLOCK) ua2 on ua2.AccountID = a2.AccountID
	WHERE ua2.UserWebsiteID = ua.UserWebsiteID

	--SELECT
	--	STUFF((SELECT DISTINCT ',' + CASE WHEN ISNULL(a2.StoreNumber,'') = '' THEN a2.AccountName ELSE a2.StoreNumber END
	--		FROM Account (NOLOCK) a2
	--		Inner Join UserAccount (NOLOCK) ua2 on ua2.AccountID = a2.AccountID
	--		WHERE ua2.UserWebsiteID = ua.UserWebsiteID
	--		FOR XML PATH(''), TYPE).value('text()[1]', 'nvarchar(max)')
	--	, 1, LEN(','), '') AS Value

) account

Outer Apply
(
	SELECT TOP 1 ec2.HireDate, ec2.Job
	FROM EnterpriseCustomer (NOLOCK) ec2
	WHERE ec2.EmployeeID = ua.EmployeeID
    and ISNULL(ec2.EmployeeID,'') != ''
	and ec2.IsIndividual = 1
) enterprise
Outer Apply
(
	SELECT Top 1 ec2.BranchAdminLgcyID, ec2.AirportCode, ec2.IsAirport
	FROM EnterpriseCustomer (NOLOCK) ec2
	WHERE ec2.StoreNumber = account.StoreNumber
	and ec2.IsIndividual = 0
) enterprisestore

WHERE b.WebsiteID = {0}
{1}
{2}
{3}
{4}
{5}
{6}
ORDER BY b.BudgetID
"
                        , Database.HandleQuote(CurrentWebsite.WebsiteID)
                        , CurrentUser.IsSuperAdmin || CurrentUser.CurrentUserWebSite.IsBudgetAdmin ? string.Empty : string.Format("AND b.ApproverUserWebsiteID = {0}", Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID))
                        , string.IsNullOrEmpty(txtBudgetName.Text.Trim()) ? string.Empty : string.Format("AND b.BudgetName like '%{0}%'", txtBudgetName.Text.Trim())
                        , string.IsNullOrEmpty(txtEmail.Text.Trim()) ? string.Empty : string.Format("AND u.EmailAddress like '%{0}%'", txtEmail.Text.Trim())
                        , string.IsNullOrEmpty(txtDivision.Text.Trim()) ? string.Empty : string.Format("AND b.Division like '%{0}%'", txtDivision.Text.Trim())
                        , chkActiveOnly.Checked ? string.Format("AND b.StartDate <= {0} and b.EndDate >= {0}", Database.HandleQuote(Convert.ToString(DateTime.UtcNow))) : string.Empty 
                        , !string.IsNullOrEmpty(hfUserWebsiteID.Value) ? string.Format("AND b.ApproverUserWebsiteID = {0}", Database.HandleQuote(Convert.ToString(hfUserWebsiteID.Value))) : string.Empty

                        , (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "20")
                            || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "53") 
                                ? ", CASE WHEN ISNULL(ec.Inactive,0) = 1 OR ISNULL(ua.Inactive,0) = 1 OR ISNULL(ba.Inactive,0) = 1 THEN 'Yes' ELSE '' END as inactive, ISNULL(CONVERT(varchar, enterprise.HireDate, 101),'') as hiredate, enterprise.Job, CASE WHEN enterprisestore.IsAirport = 1 THEN 'Yes' ELSE '' END as IsAirport, REPLACE(enterprisestore.AirportCode,',',' ') as AirportCode, ISNULL(Adjustmnet.amount,0) as AdjustmentAmount"
                                : String.Format(@", CASE WHEN ISNULL(ba.Inactive,0) = 1 or ISNULL(ua.Inactive,0) = 1 or b.StartDate > {0} or b.EndDate < {0} THEN 'Yes' ELSE '' END as inactive", Database.HandleQuote(Convert.ToString(DateTime.UtcNow)))
                        , (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "20")
                            || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "53") ? "Left Outer Join EnterpriseCustomer (NOLOCK) ec on ec.Email = u.EmailAddress and ec.IsIndividual = 1" : String.Empty
                        
                );

                objRead = Database.GetDataReader(strSQL);


                while (objRead.Read())
                {
                    //Enterprise
                    if (
                        (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "20")
                        ||
                        (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "53")
                    )
                    {
                        objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}"
                            , Convert.ToString(objRead["id"])
                            , Convert.ToString(objRead["email"])
                            , Convert.ToString(objRead["name"])
                            , Convert.ToString(objRead["start_date"])
                            , Convert.ToString(objRead["end_date"])
                            , Convert.ToString(objRead["amount"])
                            , Convert.ToString(objRead["allow_overbudget"])
                            , Convert.ToString(objRead["payment_term"])
                            , Convert.ToString(objRead["approver"])
                            , Convert.ToString(objRead["division"])
                            , Convert.ToString(objRead["remaining_balance"])
                            , Convert.ToString(objRead["pending_amount"])
                            , Convert.ToString(objRead["amount_spent"])
                            , Convert.ToString(objRead["AdjustmentAmount"])
                            , Convert.ToString(objRead["group"])
                            , Convert.ToString(objRead["group_branch"])
                            , Convert.ToString(objRead["regional_group"])
                            , Convert.ToString(objRead["employee_name"])
                            , Convert.ToString(objRead["inactive"])
                            , Convert.ToString(objRead["hiredate"])
                            , Convert.ToString(objRead["Job"])
                            , Convert.ToString(objRead["IsAirport"])
                            , Convert.ToString(objRead["AirportCode"])
                        ));
                    }
                    else
                    {
                        objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}"
                            , Convert.ToString(objRead["id"])
                            , Convert.ToString(objRead["email"])
                            , Convert.ToString(objRead["name"])
                            , Convert.ToString(objRead["start_date"])
                            , Convert.ToString(objRead["end_date"])
                            , Convert.ToString(objRead["amount"])
                            , Convert.ToString(objRead["allow_overbudget"])
                            , Convert.ToString(objRead["payment_term"])
                            , Convert.ToString(objRead["approver"])
                            , Convert.ToString(objRead["division"])
                            , Convert.ToString(objRead["remaining_balance"])
                            , Convert.ToString(objRead["pending_amount"])
                            , Convert.ToString(objRead["inactive"])
                        ));
                    }
                    objReturn.AppendLine();
                }

                if (objReturn != null)
                {
                    using (StreamWriter _streamwriter = new StreamWriter(filepath))
                    {
                        _streamwriter.Write(objReturn.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void chkActiveOnly_CheckedChanged(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/BudgetOverview.aspx?budgetname=" + Server.UrlEncode(this.txtBudgetName.Text.Trim()) 
                + "&email=" + Server.UrlEncode(this.txtEmail.Text.Trim())
                + "&division=" + Server.UrlEncode(this.txtDivision.Text.Trim())
                + "&activeonly=" + Server.UrlEncode(Convert.ToString(this.chkActiveOnly.Checked))
                + "&approver=" + Server.UrlEncode(Convert.ToString(hfUserWebsiteID.Value))
                )
            );
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

            Response.Redirect(String.Format("/Admin/BudgetOverview.aspx?budgetname=" + Server.UrlEncode(this.txtBudgetName.Text.Trim())
                + "&email=" + Server.UrlEncode(this.txtEmail.Text.Trim())
                + "&division=" + Server.UrlEncode(this.txtDivision.Text.Trim())
                + "&activeonly=" + Server.UrlEncode(Convert.ToString(this.chkActiveOnly.Checked))
                + "&approver=" + Server.UrlEncode(Convert.ToString(hfUserWebsiteID.Value))
                )
            );
            //btnUserWebsiteRemove.Visible = !string.IsNullOrEmpty(hfUserWebsiteID.Value);
        }

        protected void txtDivision_TextChanged(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/BudgetOverview.aspx?budgetname=" + Server.UrlEncode(this.txtBudgetName.Text.Trim())
                + "&email=" + Server.UrlEncode(this.txtEmail.Text.Trim())
                + "&division=" + Server.UrlEncode(this.txtDivision.Text.Trim())
                + "&activeonly=" + Server.UrlEncode(Convert.ToString(this.chkActiveOnly.Checked))
                + "&approver=" + Server.UrlEncode(Convert.ToString(hfUserWebsiteID.Value))
                )
            );
        }

        protected void gvBudget_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string strBudgetID = gvBudget.DataKeys[e.Row.RowIndex].Value.ToString();
                    HyperLink hlnkBudget = (HyperLink)e.Row.FindControl("hlnkBudget");

                    hlnkBudget.NavigateUrl = string.Format("/admin/Budget.aspx?id={0}", strBudgetID);

                    if(!string.IsNullOrEmpty(txtEmail.Text) && CurrentWebsite.HideEmail)
                    {
                        ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                        ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                        UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                        UserWebsiteFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.EmployeeID.SearchString = txtEmail.Text;
                        UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                        ImageSolutions.Budget.BudgetAssignment BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                        ImageSolutions.Budget.BudgetAssignmentFilter BudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
                        BudgetAssignmentFilter.BudgetID = new Database.Filter.StringSearch.SearchFilter();
                        BudgetAssignmentFilter.BudgetID.SearchString = strBudgetID;
                        BudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        BudgetAssignmentFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                        BudgetAssignment = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignment(BudgetAssignmentFilter);

                        if (BudgetAssignment != null)
                        {
                            hlnkBudget.NavigateUrl = string.Format("/admin/BudgetAssignmentUser.aspx?id={0}", BudgetAssignment.BudgetAssignmentID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
    }
}
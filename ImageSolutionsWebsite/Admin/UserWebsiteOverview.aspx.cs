using Amazon.Runtime.Internal.Util;
using ImageSolutions.Account;
using ImageSolutions.SalesOrder;
using Stripe;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class UserWebsiteOverview : BasePageAdminUserWebSiteAuth
    {
        protected string mFirstName = string.Empty;
        protected string mLastName = string.Empty;
        protected string mEmail = string.Empty;
        protected string mAccount = string.Empty;
        protected string mPendingApproval = String.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            mFirstName = Request.QueryString.Get("firstname");
            mLastName = Request.QueryString.Get("lastname");
            mEmail = Request.QueryString.Get("email");
            mAccount = Request.QueryString.Get("account");
            mPendingApproval = Request.QueryString.Get("pendingapproval");

            UpdateAccount();

            if (!Page.IsPostBack)
            {
                this.txtFirstName.Text = mFirstName;
                this.txtLastName.Text = mLastName;
                this.txtEmail.Text = mEmail;               

                Initialize();

                lblEmail.Text = CurrentWebsite.HideEmail ? "Employee ID:" : "Email:";
                txtEmail.Attributes.Add("placeholder", CurrentWebsite.HideEmail ? "employee id" : "email");
            }
        }

        protected void UpdateAccount()
        {
            ucMyAccountSearchModal.SendMessageToThePage += delegate (string message)
            {
                try
                {
                    hfAccountID.Value = message;

                    if (!string.IsNullOrEmpty(hfAccountID.Value))
                    {
                        Response.Redirect(String.Format("/Admin/UserWebsiteOverview.aspx?firstname=" + Server.UrlEncode(this.txtFirstName.Text.Trim()) + "&lastname=" + Server.UrlEncode(this.txtLastName.Text.Trim()) + "&email=" + Server.UrlEncode(this.txtEmail.Text.Trim()) + "&account=" + Server.UrlEncode(hfAccountID.Value) + "&pendingapproval=" + Server.UrlEncode(Convert.ToString(this.chkPendingApproval.Checked)) ));
                    }
                    else
                    {
                        txtAccount.Text = string.Empty;
                    }

                    btnAccountRemove.Visible = !string.IsNullOrEmpty(hfAccountID.Value);
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(this, ex.Message);
                }
            };

        }

        protected void Initialize()
        {
            int cntAccount = GetAccountCountBySQL(); //CurrentUser.CurrentUserWebSite.Accounts.Count;

            //if (cntAccount <= 300)
            //{
            //    this.ddlAccount.DataSource = CurrentUser.CurrentUserWebSite.Accounts.OrderBy(m => m.AccountNamePath);
            //    this.ddlAccount.DataBind();
            //    this.ddlAccount.Items.Insert(0, new ListItem("All Stores", ""));

            //    if (!string.IsNullOrEmpty(mAccount))
            //    {
            //        this.ddlAccount.SelectedValue = mAccount;
            //    }
            //}
            //else
            //{
            //    ddlAccount.Visible = false;
            //}

            hfAccountID.Value = Convert.ToString(mAccount);
            this.chkPendingApproval.Checked = mPendingApproval == "True";

            //if (CurrentUser.CurrentUserWebSite.Accounts.Count <= 300)
            if (cntAccount <= 300)
            {
                ddlAccount.Visible = true;
                BindAccount();
                txtAccount.Visible = false;
                btnAccountSearch.Visible = false;
                btnAccountRemove.Visible = false;
                pnlSearchMessage.Visible = false;
            }
            else
            {
                ddlAccount.Visible = false;
                txtAccount.Visible = true;
              
                btnAccountSearch.Visible = true;
                btnAccountSearch.Enabled = true;
                btnAccountRemove.Enabled = true;
                pnlSearchMessage.Visible = true;
            }

            if (!string.IsNullOrEmpty(hfAccountID.Value))
            {
                if (ddlAccount.Visible)
                {
                    this.ddlAccount.SelectedIndex = this.ddlAccount.Items.IndexOf(this.ddlAccount.Items.FindByValue(hfAccountID.Value));
                }
                else
                {
                    if (!string.IsNullOrEmpty(hfAccountID.Value))
                    {
                        ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account(hfAccountID.Value);
                        if (Account != null && !string.IsNullOrEmpty(Account.AccountID))
                        {
                            txtAccount.Text = Account.AccountNamePath;
                        }
                    }
                    else
                    {
                        txtAccount.Text = String.Empty;
                    }

                    btnAccountRemove.Visible = !string.IsNullOrEmpty(hfAccountID.Value);
                }
            }


            if (
                (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && CurrentWebsite.WebsiteID == "53")
                ||
                (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "20")
            )
            {
                BindUserWebsiteBySQLEnterprise();
            }
            else
            {
                BindUserWebsiteBySQL();
            }            

            pnlAdminFunction.Visible = CurrentUser.IsSuperAdmin;

            //If Enterprise EBA
            if (
                (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "53") 
                || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "20")
            )
            {
                btnImportBudgetProgram.Visible = true;
            }
            else
            {
                btnImportBudgetProgram.Visible = false;
            }
        }

        protected void BindAccount()
        {
            this.ddlAccount.DataSource = CurrentUser.CurrentUserWebSite.Accounts.OrderBy(m => m.AccountNamePath);
            this.ddlAccount.DataBind();
            this.ddlAccount.Items.Insert(0, new ListItem("All", ""));
        }

        //private List<ImageSolutions.User.UserWebsite> mUserWebsites = null;
        //public List<ImageSolutions.User.UserWebsite> UserWebsitess
        //{
        //    get
        //    {
        //        if (CurrentUser.IsSuperAdmin)
        //        {
        //            mUserWebsites = CurrentUser.CurrentUserWebSite.WebSite.UserWebsites;
        //        }
        //        else
        //        {
        //            mUserWebsites = CurrentUser.CurrentUserWebSite.WebSite.UserWebsites.FindAll(m => m.UserAccounts.Exists(n => CurrentUser.CurrentUserWebSite.Accounts.Exists(o => o.AccountID == n.AccountID)));
        //        }

        //        if (!string.IsNullOrEmpty(this.ddlAccount.SelectedValue))
        //        {
        //            mUserWebsites = mUserWebsites.FindAll(m => m.Accounts.Exists(n => n.AccountID == this.ddlAccount.SelectedValue));
        //        }

        //        return mUserWebsites;
        //    }
        //}
        protected int GetAccountCountBySQL()
        {
            int intReturn = 0;

            string strSQL = string.Empty;
            SqlDataReader objRead = null;

            strSQL = string.Format("SELECT COUNT(AccountID) Cnt FROM Account a WHERE a.WebsiteID = {0}", Database.HandleQuote(CurrentWebsite.WebsiteID));

            try
            {
                objRead = Database.GetDataReader(strSQL);

                if (objRead.Read())
                {
                    intReturn = Convert.ToInt32(objRead["Cnt"]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (objRead != null) objRead.Dispose();
                objRead = null;
            }

            return intReturn;
        }

    protected void BindUserWebsiteBySQL()
        {
            //List<ImageSolutions.SalesOrder.SalesOrder> SalesOrders = null;
            int intTotalRecord = 0;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                //SalesOrders = new List<ImageSolutions.SalesOrder.SalesOrder>();

                List<string> accounts = new List<string>();
                string strAccountIDs = string.Empty;

                if (!string.IsNullOrEmpty(Convert.ToString(hfAccountID.Value)))
                {
                    accounts.Add(Convert.ToString(hfAccountID.Value));
                    AddSubAccounts(Convert.ToString(hfAccountID.Value), ref accounts);

                    foreach (string _subaccount in accounts)
                    {
                        strAccountIDs = string.IsNullOrEmpty(strAccountIDs) ? string.Format("{0}", _subaccount) : string.Format("{0},{1}", strAccountIDs, _subaccount);
                    }
                }

                strSQL = string.Format(@"

IF OBJECT_ID(N'tempdb..#TempAccountHierarchy') IS NOT NULL
BEGIN
DROP TABLE #TempAccountHierarchy
END

IF OBJECT_ID(N'tempdb..#TempAccounts') IS NOT NULL
BEGIN
DROP TABLE #TempAccounts
END

DECLARE @numRecord int

SET @numRecord = (SELECT COUNT(AccountID) FROM Account a WHERE a.WebsiteID = {0})
;

WITH  account_hierarchy AS (
	SELECT 
		ISNULL(AccountID,0) as AccountID,
		ISNULL(ParentID,0) as ParentID,
		CAST(AccountName as VARCHAR(MAX)) AS AccountNamePath
	FROM Account (NOLOCK) a
	WHERE ISNULL(ParentID,0) = 0
    AND a.WebsiteID = {0}
	
    UNION ALL
   
	SELECT
		ISNULL(a.AccountID,0) as AccountID,
		ISNULL(a.ParentID,0) as ParentID,
		CAST(a2.AccountNamePath as VARCHAR(MAX)) + ' -> ' + CAST(a.AccountName as VARCHAR(400))
	FROM Account (NOLOCK) a, account_hierarchy a2
	WHERE ISNULL(a.ParentID,0) = ISNULL(a2.AccountID,0)
    AND a.WebsiteID = {0}
)


SELECT TOP (@numRecord) AccountID, ParentID, AccountNamePath
INTO #TempAccountHierarchy
FROM account_hierarchy

SELECT acct.AccountID
INTO #TempAccounts
FROM (
	SELECT ua2.AccountID 
	FROM UserAccount (NOLOCK) ua2 
	WHERE ua2.UserWebsiteID = {1}
	UNION
	SELECT a2.AccountID 
	FROM Account (NOLOCK) a2
	WHERE a2.ParentID in (SELECT ua3.AccountID FROM UserAccount (NOLOCK) ua3 WHERE ua3.UserWebsiteID = {1} )
	UNION
	SELECT a2.AccountID 
	FROM Account (NOLOCK) a2
	WHERE a2.ParentID in (
		SELECT a2.AccountID 
		FROM Account (NOLOCK) a2
		WHERE a2.ParentID in (SELECT ua3.AccountID FROM UserAccount (NOLOCK) ua3 WHERE ua3.UserWebsiteID = {1} )
	)
) acct

            SELECT * FROM
            (
                SELECT *, COUNT(*) OVER () AS TotalRecord FROM
                (
		            SELECT *, ROW_NUMBER() OVER (ORDER BY UserWebsiteID DESC) AS RowNumber FROM
		            (


SELECT DISTINCT uw.UserWebsiteID
	, ah.AccountNamePath
    , u.FirstName
    , u.LastName
    , u.EmailAddress
    , uw.EmployeeID
    , uw.IsAdmin
    , uw.IsPendingApproval
    , uw.CreatedOn
FROM UserWebsite (NOLOCK) uw
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = uw.UserInfoID
Left Outer Join UserAccount (NOLOCK) ua on ua.UserWebsiteID = uw.UserWebsiteID
Left Outer Join Account (NOLOCK) a on a.AccountID = ua.AccountID
{2}
Left Outer Join #TempAccountHierarchy (NOLOCK) ah on ah.AccountID = a.AccountID
WHERE uw.WebsiteID = {0}
{3}
{4}
{5}
{6}
{9}

and ISNULL(u.EmailAddress,'') != ''

                    ) t1
                ) t2
            ) t3
            WHERE RowNumber >= {7} AND RowNumber <= {8}
            ORDER BY RowNumber ASC
"
                        , Database.HandleQuote(CurrentWebsite.WebsiteID)
                        , Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID)
                        , CurrentUser.IsSuperAdmin 
                                || (
                                    Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production"
                                    && CurrentWebsite.WebsiteID == "30"
                                    && CurrentUser.CurrentUserWebSite.CurrentUserAccount != null
                                    && string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.ParentID)
                                )
                            ? string.Empty 
                            : string.Format("Inner Join #TempAccounts (NOLOCK) ta on ta.AccountID = ua.AccountID")
                        , string.IsNullOrEmpty(Convert.ToString(hfAccountID.Value)) ? string.Empty : string.Format("AND a.AccountID in ({0})", strAccountIDs)

                        , string.IsNullOrEmpty(Convert.ToString(txtFirstName.Text)) ? string.Empty : string.Format("AND u.FirstName = {0}", Database.HandleQuote(Convert.ToString(txtFirstName.Text)))
                        , string.IsNullOrEmpty(Convert.ToString(txtLastName.Text)) ? string.Empty : string.Format("AND u.LastName = {0}", Database.HandleQuote(Convert.ToString(txtLastName.Text)))
                        , string.IsNullOrEmpty(Convert.ToString(txtEmail.Text)) ? string.Empty : string.Format("AND (u.EmailAddress = {0} or u.UserName = {0})", Database.HandleQuote(Convert.ToString(txtEmail.Text)))

                        , (ucPager.CurrentPageNumber - 1) * ucPager.PageSize + 1
                        , (ucPager.CurrentPageNumber) * ucPager.PageSize

                        , !chkPendingApproval.Checked ? string.Empty : string.Format("AND uw.IsPendingApproval = 1")
                );

                objData = Database.GetDataSet(strSQL);

                if (objData != null)
                {
                    this.gvUserWebsites.DataSource = objData;
                    this.gvUserWebsites.DataBind();

                    if (objData.Tables != null && objData.Tables.Count > 0
                        && objData.Tables[0].Rows != null && objData.Tables[0].Rows.Count > 0
                        && objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) intTotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    ucPager.TotalRecord = intTotalRecord;
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void BindUserWebsiteBySQLEnterprise()
        {
            List<ImageSolutions.SalesOrder.SalesOrder> SalesOrders = null;
            int intTotalRecord = 0;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                SalesOrders = new List<ImageSolutions.SalesOrder.SalesOrder>();

                List<string> accounts = new List<string>();
                string strAccountIDs = string.Empty;

                if (!string.IsNullOrEmpty(Convert.ToString(hfAccountID.Value)))
                {
                    accounts.Add(Convert.ToString(hfAccountID.Value));
                    AddSubAccounts(Convert.ToString(hfAccountID.Value), ref accounts);

                    foreach (string _subaccount in accounts)
                    {
                        strAccountIDs = string.IsNullOrEmpty(strAccountIDs) ? string.Format("{0}", _subaccount) : string.Format("{0},{1}", strAccountIDs, _subaccount);
                    }
                }

                strSQL = string.Format(@"

IF OBJECT_ID(N'tempdb..#TempAccounts') IS NOT NULL
BEGIN
DROP TABLE #TempAccounts
END

SELECT acct.AccountID
INTO #TempAccounts
FROM (
	SELECT ua2.AccountID 
	FROM UserAccount (NOLOCK) ua2 
	WHERE ua2.UserWebsiteID = {1}
	UNION
	SELECT a2.AccountID 
	FROM Account (NOLOCK) a2
	WHERE a2.ParentID in (SELECT ua3.AccountID FROM UserAccount (NOLOCK) ua3 WHERE ua3.UserWebsiteID = {1} )
	UNION
	SELECT a2.AccountID 
	FROM Account (NOLOCK) a2
	WHERE a2.ParentID in (
		SELECT a2.AccountID 
		FROM Account (NOLOCK) a2
		WHERE a2.ParentID in (SELECT ua3.AccountID FROM UserAccount (NOLOCK) ua3 WHERE ua3.UserWebsiteID = {1} )
	)
) acct

            SELECT * FROM
            (
                SELECT *, COUNT(*) OVER () AS TotalRecord FROM
                (
		            SELECT *, ROW_NUMBER() OVER (ORDER BY UserWebsiteID DESC) AS RowNumber FROM
		            (


SELECT DISTINCT uw.UserWebsiteID
	, a.AccountName
    , u.FirstName
    , u.LastName
    , u.EmailAddress
    , uw.EmployeeID
    , uw.IsAdmin
    , uw.IsPendingApproval
    , uw.CreatedOn
FROM UserWebsite (NOLOCK) uw
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = uw.UserInfoID
Left Outer Join UserAccount (NOLOCK) ua on ua.UserWebsiteID = uw.UserWebsiteID
Left Outer Join Account (NOLOCK) a on a.AccountID = ua.AccountID
{2}
WHERE uw.WebsiteID = {0}
{3}
{4}
{5}
{6}
{9}

                    ) t1
                ) t2
            ) t3
            WHERE RowNumber >= {7} AND RowNumber <= {8}
            ORDER BY RowNumber ASC
"
                        , Database.HandleQuote(CurrentWebsite.WebsiteID)
                        , Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID)
                        , CurrentUser.IsSuperAdmin ? string.Empty : string.Format("Inner Join #TempAccounts (NOLOCK) ta on ta.AccountID = ua.AccountID")
                        , string.IsNullOrEmpty(Convert.ToString(hfAccountID.Value)) ? string.Empty : string.Format("AND a.AccountID in ({0})", strAccountIDs)

                        , string.IsNullOrEmpty(Convert.ToString(txtFirstName.Text)) ? string.Empty : string.Format("AND u.FirstName = {0}", Database.HandleQuote(Convert.ToString(txtFirstName.Text)))
                        , string.IsNullOrEmpty(Convert.ToString(txtLastName.Text)) ? string.Empty : string.Format("AND u.LastName = {0}", Database.HandleQuote(Convert.ToString(txtLastName.Text)))
                        , string.IsNullOrEmpty(Convert.ToString(txtEmail.Text)) ? string.Empty : string.Format("AND (u.EmailAddress = {0} or u.UserName = {0})", Database.HandleQuote(Convert.ToString(txtEmail.Text)))

                        , (ucPager.CurrentPageNumber - 1) * ucPager.PageSize + 1
                        , (ucPager.CurrentPageNumber) * ucPager.PageSize

                        , !chkPendingApproval.Checked ? string.Empty : string.Format("AND uw.IsPendingApproval = 1")
                );

                objData = Database.GetDataSet(strSQL);

                if (objData != null)
                {
                    this.gvUserWebsites.DataSource = objData;
                    this.gvUserWebsites.DataBind();

                    if (objData.Tables != null && objData.Tables.Count > 0
                        && objData.Tables[0].Rows != null && objData.Tables[0].Rows.Count > 0
                        && objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) intTotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    ucPager.TotalRecord = intTotalRecord;
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void BindUserWebsites()
        {
            List<ImageSolutions.User.UserWebsite> objUserWebsite = null;
            ImageSolutions.User.UserWebsiteFilter objFilter = null;
            int intTotalRecord = 0;

            try
            {
                objFilter = new ImageSolutions.User.UserWebsiteFilter();

                if (CurrentUser.IsSuperAdmin)
                {
                    objFilter = new ImageSolutions.User.UserWebsiteFilter();
                    objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    objFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;
                }
                else
                {
                    if (!string.IsNullOrEmpty(hfAccountID.Value))
                    {
                        objFilter.AccountIDs = new List<string>();
                        objFilter.AccountIDs.Add(hfAccountID.Value);

                        //ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account(this.ddlAccount.SelectedValue);
                        //foreach (ImageSolutions.Account.Account _Account in Account.SubAccounts)
                        //{
                        //    objFilter.AccountIDs.Add(_Account.AccountID);
                        //}

                        List<string> subaccounts = new List<string>();

                        if (!subaccounts.Exists(m => m == Convert.ToString(hfAccountID.Value)))
                        {
                            AddSubAccounts(Convert.ToString(hfAccountID.Value), ref subaccounts);
                        }                        

                        foreach (string _subaccount in subaccounts)
                        {
                            objFilter.AccountIDs.Add(_subaccount);
                        }
                    }
                    else
                    {
                        objFilter.AccountIDs = new List<string>();

                        //foreach (ImageSolutions.Account.Account objAccount in CurrentUser.CurrentUserWebSite.Accounts)
                        //{
                        //    objFilter.AccountIDs.Add(objAccount.AccountID);
                        //    foreach (ImageSolutions.Account.Account _Account in objAccount.SubAccounts)
                        //    {
                        //        objFilter.AccountIDs.Add(_Account.AccountID);
                        //    }
                        //}

                        List<string> subaccounts = new List<string>();                        
                        foreach (ImageSolutions.User.UserAccount _UserAccount in CurrentUser.CurrentUserWebSite.UserAccounts)
                        {
                            if (!subaccounts.Exists(m => m == _UserAccount.AccountID))
                            {
                                AddSubAccounts(_UserAccount.AccountID, ref subaccounts);
                            }
                        }

                        foreach (string _subaccount in subaccounts)
                        {
                            objFilter.AccountIDs.Add(_subaccount);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(this.txtFirstName.Text.Trim()))
                {
                    objFilter.FirstName = new Database.Filter.StringSearch.SearchFilter();
                    objFilter.FirstName.SearchString = this.txtFirstName.Text.Trim();
                }

                if (!string.IsNullOrEmpty(this.txtLastName.Text.Trim()))
                {
                    objFilter.LastName = new Database.Filter.StringSearch.SearchFilter();
                    objFilter.LastName.SearchString = this.txtLastName.Text.Trim();
                }

                if (!string.IsNullOrEmpty(this.txtEmail.Text.Trim()))
                {
                    if(CurrentWebsite.HideEmail)
                    {
                        objFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.EmployeeID.SearchString = this.txtEmail.Text.Trim();
                    }
                    else
                    {
                        objFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.EmailAddress.SearchString = this.txtEmail.Text.Trim();
                    }
                }

                objUserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsites(objFilter, ucPager.PageSize, ucPager.CurrentPageNumber, out intTotalRecord);
            
                gvUserWebsites.DataSource = objUserWebsite;
                gvUserWebsites.DataBind();
                ucPager.TotalRecord = intTotalRecord;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        public class SubAccounts
        {
            string Parentid { get; set; }
            string AccoutnID { get; set; }
        }

        public void AddSubAccounts(string accountid, ref List<string> accountids)
        {
            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                ucPager.Visible = false;

                strSQL = string.Format(@"

WITH SubAccount
AS
(
	SELECT a2.AccountID, a2.ParentID
	FROM Account (NOLOCK) a 
	Inner Join Account (NOLOCK) a2 on a2.ParentID = a.AccountID
	WHERE a.AccountID = {0}

	UNION ALL

	SELECT a3.AccountID, a3.ParentID
	FROM SubAccount, Account (NOLOCK) a3
	WHERE SubAccount.AccountID = a3.ParentID
)

SELECT AccountID FROM SubAccount
"
                        , Database.HandleQuote(accountid)
                        );

                objRead = Database.GetDataReader(strSQL);

                while (objRead.Read())
                {
                    string strAccountID = Convert.ToString(objRead["AccountID"]);

                    if (!accountids.Exists(m => m == strAccountID))
                    {
                        accountids.Add(strAccountID);

                        //string strChild = Convert.ToString(objRead["Child"]);

                        //if (!string.IsNullOrEmpty(strChild) && !accountids.Exists(m => m == strChild))
                        //{
                        //    AddSubAccounts(strAccountID, ref accountids);
                        //}
                    }
                }
            }
            catch
            {

            }
        }

        protected void ddlAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            hfAccountID.Value = Convert.ToString(ddlAccount.SelectedValue);

            //if (string.IsNullOrEmpty(ddlAccount.SelectedValue))
            //{
            //    Response.Redirect(String.Format("/Admin/UserWebsiteOverview.aspx?firstname=" + Server.UrlEncode(this.txtFirstName.Text.Trim()) + "&lastname=" + Server.UrlEncode(this.txtLastName.Text.Trim()) + "&email=" + Server.UrlEncode(this.txtEmail.Text.Trim())));
            //}
            //else
            //{
            Response.Redirect(String.Format("/Admin/UserWebsiteOverview.aspx?firstname=" + Server.UrlEncode(this.txtFirstName.Text.Trim()) + "&lastname=" + Server.UrlEncode(this.txtLastName.Text.Trim()) + "&email=" + Server.UrlEncode(this.txtEmail.Text.Trim()) + "&account=" + Server.UrlEncode(Convert.ToString(hfAccountID.Value)) + "&pendingapproval=" + Server.UrlEncode(Convert.ToString(this.chkPendingApproval.Checked))));
            //}
            //BindUserWebsites();
        }

        protected void txtFilter_TextChanged(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(ddlAccount.SelectedValue))
            //{
            //    Response.Redirect(String.Format("/Admin/UserWebsiteOverview.aspx?firstname=" + Server.UrlEncode(this.txtFirstName.Text.Trim()) + "&lastname=" + Server.UrlEncode(this.txtLastName.Text.Trim()) + "&email=" + Server.UrlEncode(this.txtEmail.Text.Trim())));
            //}
            //else
            //{
                Response.Redirect(String.Format("/Admin/UserWebsiteOverview.aspx?firstname=" + Server.UrlEncode(this.txtFirstName.Text.Trim()) + "&lastname=" + Server.UrlEncode(this.txtLastName.Text.Trim()) + "&email=" + Server.UrlEncode(this.txtEmail.Text.Trim()) + "&account=" + Server.UrlEncode(Convert.ToString(hfAccountID.Value)) + "&pendingapproval=" + Server.UrlEncode(Convert.ToString(this.chkPendingApproval.Checked))));
            //}
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            //System.Data.OleDb.OleDbConnection OleDbConnection = null;
            //System.Data.OleDb.OleDbCommand OleDbCommand = null;
            string strSQL = string.Empty;
            Hashtable dicParam = null;

            List<ImageSolutions.User.UserWebsite> objUserWebsite = null;
            ImageSolutions.User.UserWebsiteFilter objFilter = null;
            try
            {
                string strPath = Server.MapPath("\\Export\\UserWebsite\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }

                //string strTemplateFileName = "UserWebsiteTemplate.xlsx";
                //string strTemplateFilePath = Server.MapPath(string.Format("\\Import\\Template\\{0}", strTemplateFileName));
                //string strFileExportPath = Server.MapPath(string.Format("\\Export\\UserWebsite\\{0}\\UserWebsite_{1}.xlsx", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")));
                //System.IO.File.Copy(strTemplateFilePath, strFileExportPath, true);

                string strFileExportPath = Server.MapPath(string.Format("\\Export\\UserWebsite\\{0}\\UserWebsite_{1}.csv", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")));
                //CreateExportCSV(strFileExportPath);
                CreateExportCSVBySQL(strFileExportPath);

                //OleDbConnection = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + strFileExportPath + "';Extended Properties=Excel 12.0;");
                //OleDbConnection.Open();
                //OleDbCommand = new System.Data.OleDb.OleDbCommand();
                //OleDbCommand.Connection = OleDbConnection;

                //objFilter = new ImageSolutions.User.UserWebsiteFilter();

                //if (CurrentUser.IsSuperAdmin)
                //{
                //    objFilter = new ImageSolutions.User.UserWebsiteFilter();
                //    objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                //    objFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;
                //}
                //else
                //{
                //    objFilter.AccountIDs = new List<string>();
                //    foreach (ImageSolutions.Account.Account objAccount in CurrentUser.CurrentUserWebSite.Accounts)
                //    {
                //        objFilter.AccountIDs.Add(objAccount.AccountID);
                //    }
                //}

                //if (!string.IsNullOrEmpty(this.ddlAccount.SelectedValue))
                //{
                //    objFilter.AccountIDs = new List<string>();
                //    objFilter.AccountIDs.Add(this.ddlAccount.SelectedValue);
                //}

                //if (!string.IsNullOrEmpty(this.txtFirstName.Text.Trim()))
                //{
                //    objFilter.FirstName = new Database.Filter.StringSearch.SearchFilter();
                //    objFilter.FirstName.SearchString = this.txtFirstName.Text.Trim();
                //}

                //if (!string.IsNullOrEmpty(this.txtLastName.Text.Trim()))
                //{
                //    objFilter.LastName = new Database.Filter.StringSearch.SearchFilter();
                //    objFilter.LastName.SearchString = this.txtLastName.Text.Trim();
                //}

                //if (!string.IsNullOrEmpty(this.txtEmail.Text.Trim()))
                //{
                //    objFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                //    objFilter.EmailAddress.SearchString = this.txtEmail.Text.Trim();
                //}

                //objUserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsites(objFilter);

                //foreach (ImageSolutions.User.UserWebsite _UserWebsite in objUserWebsite)
                //{
                //    dicParam = new Hashtable();
                //    dicParam["BusinessUnitCodes"] = _UserWebsite.Accounts[0].ParentAccount != null ? _UserWebsite.Accounts[0].ParentAccount.AccountName : String.Empty;
                //    dicParam["CustomerCode"] = _UserWebsite.Accounts[0].AccountName;
                //    dicParam["FirstName"] = _UserWebsite.UserInfo.FirstName;
                //    dicParam["LastName"] = _UserWebsite.UserInfo.LastName;
                //    dicParam["AddressLine1"] = _UserWebsite.Accounts[0].DefaultShippingAddressBook != null ? _UserWebsite.Accounts[0].DefaultShippingAddressBook.AddressLine1 : String.Empty;
                //    dicParam["AddressLine2"] = _UserWebsite.Accounts[0].DefaultShippingAddressBook != null ? _UserWebsite.Accounts[0].DefaultShippingAddressBook.AddressLine2 : String.Empty;
                //    dicParam["AddressLine3"] = _UserWebsite.Accounts[0].DefaultShippingAddressBook != null ? _UserWebsite.Accounts[0].DefaultShippingAddressBook.AddressLine3 : String.Empty;
                //    dicParam["Country"] = _UserWebsite.Accounts[0].DefaultShippingAddressBook != null ? _UserWebsite.Accounts[0].DefaultShippingAddressBook.CountryCode : String.Empty;
                //    dicParam["State"] = _UserWebsite.Accounts[0].DefaultShippingAddressBook != null ? _UserWebsite.Accounts[0].DefaultShippingAddressBook.State : String.Empty;
                //    dicParam["City"] = _UserWebsite.Accounts[0].DefaultShippingAddressBook != null ? _UserWebsite.Accounts[0].DefaultShippingAddressBook.City : String.Empty;
                //    dicParam["PostalCode"] = _UserWebsite.Accounts[0].DefaultShippingAddressBook != null ? _UserWebsite.Accounts[0].DefaultShippingAddressBook.PostalCode : String.Empty;
                //    //dicParam["WorkPhone"] = "";
                //    //dicParam["Type"] = "";
                //    dicParam["UserName"] = _UserWebsite.UserInfo.EmailAddress;
                //    //dicParam["ContactEmail"] = "";

                //    strSQL = Database.GetInsertSQL(dicParam, "[users$]", false);
                //    OleDbCommand.CommandText = strSQL;
                //    OleDbCommand.ExecuteNonQuery();
                //}


                //                //SQL
                //                SqlDataReader objRead = null;
                //                string strDBSQL = string.Empty;
                //                int intCount = 0;

                //                try
                //                {
                //                    string strAccountIDs = "0";
                //                    foreach (ImageSolutions.Account.Account objAccount in CurrentUser.CurrentUserWebSite.Accounts)
                //                    {
                //                        strAccountIDs += string.Format(",{0}",objAccount.AccountID);
                //                    }

                //                    strDBSQL = string.Format(@"
                //SELECT p.AccountName as ParentAccountName
                //	, a.AccountName
                //	, u.FirstName
                //	, u.LastName
                //	, ab.AddressLine1
                //	, ab.AddressLine2
                //	, ab.AddressLine3
                //	, ab.CountryCode
                //	, ab.State
                //	, ab.City
                //	, ab.PostalCode
                //	, u.EmailAddress
                //FROM UserWebsite (NOLOCK) uw
                //inner Join UserInfo (NOLOCK) u on u.UserInfoID = uw.UserInfoID
                //Left Outer JOin UserAccount (NOLOCK) ua on ua.UserWebsiteID = uw.UserWebsiteID
                //Left Outer Join Account (NOLOCK) a  on a.AccountID = ua.AccountID
                //Left Outer Join Account (NOLOCK) p  on p.AccountID = a.ParentID
                //Left Outer Join AddressBook (NOLOCK) ab on ab.AddressBookID = a.DefaultShippingAddressBookID
                //WHERE {0} "
                //                        , CurrentUser.IsSuperAdmin ? string.Format("uw.WebsiteID = {0}", CurrentUser.CurrentUserWebSite.WebSite.WebsiteID) : string.Format("ua.AccountID in ({0})", strAccountIDs));


                //                    objRead = Database.GetDataReader(strDBSQL);

                //                    while (objRead.Read())
                //                    {
                //                        dicParam = new Hashtable();
                //                        dicParam["BusinessUnitCodes"] = Convert.ToString(objRead["ParentAccountName"]);
                //                        dicParam["CustomerCode"] = Convert.ToString(objRead["AccountName"]);
                //                        dicParam["FirstName"] = Convert.ToString(objRead["FirstName"]);
                //                        dicParam["LastName"] = Convert.ToString(objRead["LastName"]);
                //                        dicParam["AddressLine1"] = Convert.ToString(objRead["AddressLine1"]);
                //                        dicParam["AddressLine2"] = Convert.ToString(objRead["AddressLine2"]);
                //                        dicParam["AddressLine3"] = Convert.ToString(objRead["AddressLine3"]);
                //                        dicParam["Country"] = Convert.ToString(objRead["CountryCode"]);
                //                        dicParam["State"] = Convert.ToString(objRead["State"]);
                //                        dicParam["City"] = Convert.ToString(objRead["City"]);
                //                        dicParam["PostalCode"] = Convert.ToString(objRead["PostalCode"]);
                //                        //dicParam["WorkPhone"] = "";
                //                        //dicParam["Type"] = "";
                //                        dicParam["UserName"] = Convert.ToString(objRead["EmailAddress"]);
                //                        //dicParam["ContactEmail"] = "";

                //                        strSQL = Database.GetInsertSQL(dicParam, "[users$]", false);
                //                        OleDbCommand.CommandText = strSQL;
                //                        OleDbCommand.ExecuteNonQuery();
                //                        intCount++;

                //                        //if (intCount > 500) break;
                //                    }
                //                }
                //                catch (Exception ex)
                //                {
                //                    throw ex;
                //                }
                //                finally
                //                {
                //                    if (objRead != null) objRead.Dispose();
                //                    objRead = null;
                //                }


                //                OleDbConnection.Close();

                //                Thread.Sleep(4000);

                Response.ContentType = "text/csv";
                //Response.ContentType = "application/vnd.ms-excel";
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

        public void CreateExportCSVBySQL(string filepath)
        {
            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                StringBuilder objCSV = null;

                objCSV = new StringBuilder();

                if (CurrentWebsite.EnablePackagePayment || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "26"))
                {
                    objCSV.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}"
, "Account"
, "First Name"
, "Last Name"
, "Address Line 1"
, "Address Line 2"
, "Address Line 3"
, "Country"
, "State"
, "City"
, "Postal Code"
, "User Name"
, "Is Admin"
, "Inactive"
, "Created On"
, "Employee ID"
, "Store Number"
, "Require Reset Password"
, "Last Order Date"
, "Package Refresh Date"
, "RVP"
, "RD"
, "RTM"
));
                }
                else if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "2"
                    || Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && CurrentWebsite.WebsiteID == "26"
                )
                {
                    objCSV.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}"
, "Account"
, "First Name"
, "Last Name"
, "Address Line 1"
, "Address Line 2"
, "Address Line 3"
, "Country"
, "State"
, "City"
, "Postal Code"
, "User Name"
, "Is Admin"
, "Inactive"
, "Created On"
, "Notification Email"
));
                }
                else
                {
                    objCSV.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}"
, "Account"
, "First Name"
, "Last Name"
, "Address Line 1"
, "Address Line 2"
, "Address Line 3"
, "Country"
, "State"
, "City"
, "Postal Code"
, "User Name"
, "Is Admin"
, "Inactive"
, "Created On"));
                }

                objCSV.AppendLine();

                List<string> accounts = new List<string>();
                string strAccountIDs = string.Empty;

                if (!string.IsNullOrEmpty(Convert.ToString(hfAccountID.Value)))
                {
                    accounts.Add(Convert.ToString(hfAccountID.Value));
                    AddSubAccounts(Convert.ToString(hfAccountID.Value), ref accounts);

                    foreach (string _subaccount in accounts)
                    {
                        strAccountIDs = string.IsNullOrEmpty(strAccountIDs) ? string.Format("{0}", _subaccount) : string.Format("{0},{1}", strAccountIDs, _subaccount);
                    }
                }                              

                strSQL = string.Format(@"

IF OBJECT_ID(N'tempdb..#TempAccountHierarchy') IS NOT NULL
BEGIN
DROP TABLE #TempAccountHierarchy
END

IF OBJECT_ID(N'tempdb..#TempAccounts') IS NOT NULL
BEGIN
DROP TABLE #TempAccounts
END

DECLARE @numRecord int

SET @numRecord = (SELECT COUNT(AccountID) FROM Account a WHERE a.WebsiteID = {0})
;

WITH  account_hierarchy AS (
	SELECT 
		ISNULL(AccountID,0) as AccountID,
		ISNULL(ParentID,0) as ParentID,
		CAST(AccountName as VARCHAR(MAX)) AS AccountNamePath
	FROM Account a
	WHERE ISNULL(ParentID,0) = 0
    AND a.WebsiteID = {0}
	
    UNION ALL
   
	SELECT
		ISNULL(a.AccountID,0) as AccountID,
		ISNULL(a.ParentID,0) as ParentID,
		CAST(a2.AccountNamePath as VARCHAR(MAX)) + ' -> ' + CAST(a.AccountName as VARCHAR(400))
	FROM Account a, account_hierarchy a2
	WHERE ISNULL(a.ParentID,0) = ISNULL(a2.AccountID,0)
    AND a.WebsiteID = {0}
)

SELECT TOP (@numRecord) AccountID, ParentID, AccountNamePath
INTO #TempAccountHierarchy
FROM account_hierarchy

SELECT acct.AccountID
INTO #TempAccounts
FROM (
	SELECT ua2.AccountID 
	FROM UserAccount (NOLOCK) ua2 
	WHERE ua2.UserWebsiteID = {1}
	UNION
	SELECT a2.AccountID 
	FROM Account (NOLOCK) a2
	WHERE a2.ParentID in (SELECT ua3.AccountID FROM UserAccount (NOLOCK) ua3 WHERE ua3.UserWebsiteID = {1} )
	UNION
	SELECT a2.AccountID 
	FROM Account (NOLOCK) a2
	WHERE a2.ParentID in (
		SELECT a2.AccountID 
		FROM Account (NOLOCK) a2
		WHERE a2.ParentID in (SELECT ua3.AccountID FROM UserAccount (NOLOCK) ua3 WHERE ua3.UserWebsiteID = {1} )
	)
) acct

SELECT DISTINCT uw.UserWebsiteID
	, ah.AccountNamePath
    , u.FirstName
    , u.LastName
	, ab.AddressLine1
	, ab.AddressLine2
	, ab.AddressLine3
	, ab.CountryCode
	, ab.State
	, ab.City
	, ab.PostalCode
	, CASE WHEN ISNULL(u.UserName,'') = '' THEN u.EmailAddress ELSE u.UserName END as UserName
    , CASE WHEN ISNULL(uw.IsAdmin,0) = 1 THEN 'Yes' ELSE '' END as IsAdmin

    , CASE WHEN ISNULL(uw.Inactive,0) = 1 THEN 'Yes' ELSE '' END as Inactive
    , CONVERT(VARCHAR(10),uw.CreatedOn,101) as CreatedOn

    {6}

    {5}

FROM UserWebsite (NOLOCK) uw
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = uw.UserInfoID
Left Outer Join UserAccount (NOLOCK) ua on ua.UserWebsiteID = uw.UserWebsiteID
Left Outer Join Account (NOLOCK) a on a.AccountID = ua.AccountID
Left Outer Join AddressBook (NOLOCK) ab on ab.AddressBookID = a.DefaultShippingAddressBookID
{2}
Inner Join #TempAccountHierarchy (NOLOCK) ah on ah.AccountID = a.AccountID

{4}

WHERE uw.WebsiteID = {0}
{3}
ORDER BY uw.UserWebsiteID

"
                        , Database.HandleQuote(CurrentWebsite.WebsiteID)
                        , Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID)
                        , CurrentUser.IsSuperAdmin
                                || (
                                    Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" 
                                    && CurrentWebsite.WebsiteID == "30"
                                    && CurrentUser.CurrentUserWebSite.CurrentUserAccount != null
                                    && string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.ParentID)
                                )
                            ? string.Empty : string.Format("Inner Join #TempAccounts (NOLOCK) ta on ta.AccountID = ua.AccountID")
                        , string.IsNullOrEmpty(Convert.ToString(hfAccountID.Value)) ? string.Empty : string.Format("AND a.AccountID in ({0})", strAccountIDs)

                        , Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" 
                            && (CurrentWebsite.WebsiteID == "25" || CurrentWebsite.WebsiteID == "26") ? string.Format(@"
Outer Apply
(
    SELECT MAX(TransactionDate) TransactionDate FROM SalesOrder (NOLOCK) s2 WHERE s2.UserWebsiteID = uw.UserWebsiteID 
) LastOrder
Outer Apply
(
    SELECT TOP 1 c2.StoreNumber, c2.RVP, c2.RD, c2.RTM
    FROM MavisTireCustomer (NOLOCK) c2 
    WHERE c2.EmployeeNumber = uw.EmployeeID
) mavis
") 
                            : string.Empty
                        , Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production"
                            && (CurrentWebsite.WebsiteID == "25" || CurrentWebsite.WebsiteID == "26") ? string.Format(@"
, ISNULL(uw.EmployeeID,'') as EmployeeID
, CASE WHEN u.RequirePasswordReset = 1 THEN 'Yes' ELSE '' END as RequirePasswordReset
, ISNULL(CONVERT(VARCHAR(10),LastOrder.TransactionDate,101),'') as LastOrderDate
, ISNULL(CONVERT(VARCHAR(10),uw.PackageAvailableDate,101),'') as PackageAvailableDate
, mavis.StoreNumber, mavis.RVP, mavis.RD, mavis.RTM
") 
                            : string.Empty

                        , Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "2" 
                            || Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && CurrentWebsite.WebsiteID == "26"
                                ? ", uw.NotificationEmail" : string.Empty

                );

                objRead = Database.GetDataReader(strSQL);

                while (objRead.Read())
                {
                    objCSV.Append(Convert.ToString(objRead["AccountNamePath"]).Replace(",", "")).Append(",");
                    objCSV.Append(Convert.ToString(objRead["FirstName"]).Replace(",", "")).Append(",");
                    objCSV.Append(Convert.ToString(objRead["LastName"]).Replace(",", "")).Append(",");
     
                    objCSV.Append(Convert.ToString(objRead["AddressLine1"]).Replace(",", "")).Append(",");
                    objCSV.Append(Convert.ToString(objRead["AddressLine2"]).Replace(",", "")).Append(",");
                    objCSV.Append(Convert.ToString(objRead["AddressLine3"]).Replace(",", "")).Append(",");
                    objCSV.Append(Convert.ToString(objRead["CountryCode"]).Replace(",", "")).Append(",");
                    objCSV.Append(Convert.ToString(objRead["State"]).Replace(",", "")).Append(",");
                    objCSV.Append(Convert.ToString(objRead["City"]).Replace(",", "")).Append(",");
                    objCSV.Append(Convert.ToString(objRead["PostalCode"]).Replace(",", "")).Append(",");

                    objCSV.Append(Convert.ToString(objRead["UserName"]).Replace(",", "")).Append(",");
                    objCSV.Append(Convert.ToString(objRead["IsAdmin"]).Replace(",", "")).Append(",");

                    objCSV.Append(Convert.ToString(objRead["Inactive"]).Replace(",", "")).Append(",");
                    objCSV.Append(Convert.ToString(objRead["CreatedOn"]).Replace(",", "")).Append(",");

                    if (CurrentWebsite.EnablePackagePayment || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "26"))
                        {
                        objCSV.Append(Convert.ToString(objRead["EmployeeID"]).Replace(",", "")).Append(",");
                        objCSV.Append(Convert.ToString(objRead["StoreNumber"]).Replace(",", "")).Append(",");
                        objCSV.Append(Convert.ToString(objRead["RequirePasswordReset"]).Replace(",", "")).Append(",");
                        objCSV.Append(Convert.ToString(objRead["LastOrderDate"]).Replace(",", "")).Append(",");
                        objCSV.Append(Convert.ToString(objRead["PackageAvailableDate"]).Replace(",", "")).Append(",");
                        objCSV.Append(Convert.ToString(objRead["RVP"]).Replace(",", "")).Append(",");
                        objCSV.Append(Convert.ToString(objRead["RD"]).Replace(",", "")).Append(",");
                        objCSV.Append(Convert.ToString(objRead["RTM"]).Replace(",", "")).Append(",");
                    }

                    if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "2"
                        || Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && CurrentWebsite.WebsiteID == "26"
                    )
                    {
                        objCSV.Append(Convert.ToString(objRead["NotificationEmail"]).Replace(",", "")).Append(",");
                    }

                    objCSV.AppendLine();
                }

                if (objCSV != null)
                {
                    using (StreamWriter objWriter = new StreamWriter(filepath))
                    {
                        objWriter.Write(objCSV.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (objRead != null) objRead.Dispose();
                objRead = null;
            }
        }

        public void CreateExportCSV(string filepath)
        {
            SqlDataReader objRead = null;
            string strDBSQL = string.Empty;

            StringBuilder objReturn = new StringBuilder();

            int intCount = 0;

            try
            {
                StringBuilder objCSV = null;

                objCSV = new StringBuilder();

                objCSV.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}"
    , "Account"
    , "First Name"
    , "Last Name"
    , "Address Line 1"
    , "Address Line 2"
    , "Address Line 3"
    , "Country"
    , "State"
    , "City"
    , "Postal Code"
    , "user Name"));
                objReturn.AppendLine();

                List<ImageSolutions.Account.Account> Accounts = new List<ImageSolutions.Account.Account>();


                foreach (ImageSolutions.Account.Account _Account in CurrentUser.CurrentUserWebSite.Accounts)
                {
                    if (!Accounts.Exists(x => x.AccountID == _Account.AccountID))
                    {
                        Accounts.Add(_Account);
                    }

                    foreach (ImageSolutions.Account.Account _SubAccount in _Account.SubAccounts)
                    {
                        if(!Accounts.Exists(x => x.AccountID == _SubAccount.AccountID))
                        {
                            Accounts.Add(_SubAccount);
                        }
                    }
                }

                foreach (ImageSolutions.Account.Account _Account in Accounts)
                {
                    foreach (ImageSolutions.User.UserInfo _UserInfo in _Account.UserInfos)
                    {
                        objCSV.Append(_Account.AccountNamePath.Replace(",", "")).Append(",");
                        objCSV.Append(_UserInfo.FirstName.Replace(",", "")).Append(",");
                        objCSV.Append(_UserInfo.LastName.Replace(",", "")).Append(",");
                        if (_Account.DefaultShippingAddressBook != null)
                        {
                            objCSV.Append(_Account.DefaultShippingAddressBook.AddressLine1.Replace(",", "")).Append(",");
                            objCSV.Append(_Account.DefaultShippingAddressBook.AddressLine2.Replace(",", "")).Append(",");
                            objCSV.Append(_Account.DefaultShippingAddressBook.AddressLine3.Replace(",", "")).Append(",");
                            objCSV.Append(_Account.DefaultShippingAddressBook.CountryCode.Replace(",", "")).Append(",");
                            objCSV.Append(_Account.DefaultShippingAddressBook.State.Replace(",", "")).Append(",");
                            objCSV.Append(_Account.DefaultShippingAddressBook.City.Replace(",", "")).Append(",");
                            objCSV.Append(_Account.DefaultShippingAddressBook.PostalCode.Replace(",", "")).Append(",");
                        }
                        else
                        {
                            objCSV.Append(",");
                            objCSV.Append(",");
                            objCSV.Append(",");
                            objCSV.Append(",");
                            objCSV.Append(",");
                            objCSV.Append(",");
                            objCSV.Append(",");
                        }

                        objCSV.Append(_UserInfo.EmailAddress.Replace(",", "")).Append(",");
                        objCSV.AppendLine();

                    }
                }
               
                if (objCSV != null)
                {
                    using (StreamWriter objWriter = new StreamWriter(filepath))
                    {
                        objWriter.Write(objCSV.ToString());
                    }
                }

                //                //Header
                //                objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}"
                //                    , "Business Unit Codes"
                //                    , "Customer Code"
                //                    , "First Name"
                //                    , "Last Name"
                //                    , "Address Line 1"
                //                    , "Address Line 2"
                //                    , "Address Line 3"
                //                    , "Country"
                //                    , "State"
                //                    , "City"
                //                    , "Postal Code"
                //                    , "user Name"));
                //                objReturn.AppendLine();

                //                string strAccountIDs = "0";
                //                foreach (ImageSolutions.Account.Account objAccount in CurrentUser.CurrentUserWebSite.Accounts)
                //                {
                //                    strAccountIDs += string.Format(",{0}", objAccount.AccountID);

                //                    foreach (ImageSolutions.Account.Account _Account in objAccount.SubAccounts)
                //                    {
                //                        strAccountIDs += string.Format(",{0}", _Account.AccountID);
                //                    }
                //                }

                //                strDBSQL = string.Format(@"
                //SELECT p.AccountName as ParentAccountName
                //	, a.AccountName
                //	, u.FirstName
                //	, u.LastName
                //	, ab.AddressLine1
                //	, ab.AddressLine2
                //	, ab.AddressLine3
                //	, ab.CountryCode
                //	, ab.State
                //	, ab.City
                //	, ab.PostalCode
                //	, u.EmailAddress
                //FROM UserWebsite (NOLOCK) uw
                //inner Join UserInfo (NOLOCK) u on u.UserInfoID = uw.UserInfoID
                //Left Outer JOin UserAccount (NOLOCK) ua on ua.UserWebsiteID = uw.UserWebsiteID
                //Left Outer Join Account (NOLOCK) a  on a.AccountID = ua.AccountID
                //Left Outer Join Account (NOLOCK) p  on p.AccountID = a.ParentID
                //Left Outer Join AddressBook (NOLOCK) ab on ab.AddressBookID = a.DefaultShippingAddressBookID
                //WHERE {0} "
                //                    , CurrentUser.IsSuperAdmin ? string.Format("uw.WebsiteID = {0}", CurrentUser.CurrentUserWebSite.WebSite.WebsiteID) : string.Format("ua.AccountID in ({0})", strAccountIDs));


                //                objRead = Database.GetDataReader(strDBSQL);

                //                while (objRead.Read())
                //                {
                //                    objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}"
                //                        , Convert.ToString(objRead["ParentAccountName"])
                //                        , Convert.ToString(objRead["AccountName"])
                //                        , Convert.ToString(objRead["FirstName"])
                //                        , Convert.ToString(objRead["LastName"])
                //                        , Convert.ToString(objRead["AddressLine1"])
                //                        , Convert.ToString(objRead["AddressLine2"])
                //                        , Convert.ToString(objRead["AddressLine3"])
                //                        , Convert.ToString(objRead["CountryCode"])
                //                        , Convert.ToString(objRead["State"])
                //                        , Convert.ToString(objRead["City"])
                //                        , Convert.ToString(objRead["PostalCode"])
                //                        , Convert.ToString(objRead["EmailAddress"]))
                //                    );
                //                    objReturn.AppendLine();
                //                }

                //                if (objReturn != null)
                //                {
                //                    using (StreamWriter _streamwriter = new StreamWriter(filepath))
                //                    {
                //                        _streamwriter.Write(objReturn.ToString());
                //                    }
                //                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (objRead != null) objRead.Dispose();
                objRead = null;
            }
        }

        protected void gvUserWebsites_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (CurrentWebsite.HideEmail)
            {
                //Hide Email if website is set to hide email
                e.Row.Cells[2].Visible = false;
            }
        }

        protected void btnAccountSearch_Click(object sender, EventArgs e)
        {
            ucMyAccountSearchModal.WebsiteID = CurrentWebsite.WebsiteID;
            ucMyAccountSearchModal.UserWebsiteID = CurrentUser.CurrentUserWebSite.UserWebsiteID;
            ucMyAccountSearchModal.Show();
        }

        protected void btnAccountRemove_Click(object sender, EventArgs e)
        {
            ddlAccount.SelectedValue = String.Empty;
            txtAccount.Text = String.Empty;
            hfAccountID.Value = String.Empty;

            Response.Redirect(String.Format("/Admin/UserWebsiteOverview.aspx?firstname=" + Server.UrlEncode(this.txtFirstName.Text.Trim()) + "&lastname=" + Server.UrlEncode(this.txtLastName.Text.Trim()) + "&email=" + Server.UrlEncode(this.txtEmail.Text.Trim()) + "&account=" + Server.UrlEncode(Convert.ToString(hfAccountID.Value)) + "&pendingapproval=" + Server.UrlEncode(Convert.ToString(this.chkPendingApproval.Checked))));
        }

        protected void btnImportBudgetProgram_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/UserWebsiteBudgetProgramImport.aspx"));
        }
    }
}
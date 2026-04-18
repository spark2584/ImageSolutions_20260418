using ImageSolutions.Item;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class AccountOverview : BasePageAdminUserWebSiteAuth
    {
        protected string mStoreName = string.Empty;
        protected string mEmail = string.Empty;
        protected string mStreet = string.Empty;
        protected string mCity = string.Empty;
        protected string mState = string.Empty;
        protected string mZip = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            mStoreName = Request.QueryString.Get("storename");
            mEmail = Request.QueryString.Get("email");
            mStreet = Request.QueryString.Get("street");
            mCity = Request.QueryString.Get("city");
            mState = Request.QueryString.Get("state");
            mZip = Request.QueryString.Get("zip");
            aAddNew.Visible = CurrentUser.IsSuperAdmin;

            if (!Page.IsPostBack)
            {
                this.txtStoreName.Text = mStoreName;
                this.txtEmail.Text = mEmail;
                this.txtStreet.Text = mStreet;
                this.txtCity.Text = mCity;
                this.txtState.Text = mState;
                this.txtZip.Text = mZip;

                BindAccounts();
            }
        }

        protected void BindAccounts()
        {
            List<ImageSolutions.Account.Account> Accounts = null;
            ImageSolutions.Account.AccountFilter AccountFilter = null;
            int intTotalRecord = 0;

            try
            {
                AccountFilter = new ImageSolutions.Account.AccountFilter();
                AccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                AccountFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;

                if (CurrentUser.IsSuperAdmin)
                {

                    if (!string.IsNullOrEmpty(this.txtStoreName.Text.Trim()))
                    {
                        AccountFilter.AccountName = new Database.Filter.StringSearch.SearchFilter();
                        AccountFilter.AccountName.SearchString = txtStoreName.Text.Trim();
                    }
                    if (!string.IsNullOrEmpty(this.txtEmail.Text.Trim()))
                    {
                        AccountFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                        AccountFilter.EmailAddress.SearchString = txtEmail.Text.Trim();
                    }

                    Accounts = ImageSolutions.Account.Account.GetAccounts(AccountFilter, ucPager.PageSize, ucPager.CurrentPageNumber, out intTotalRecord);

                    gvAccounts.DataSource = Accounts;
                    gvAccounts.DataBind();
                    ucPager.TotalRecord = intTotalRecord;
                }
                else
                {
                    AccountFilter.AccountIDs = new List<string>();

                    foreach (ImageSolutions.Account.Account objAccount in CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.SubAccounts)
                    {
                        AccountFilter.AccountIDs.Add(objAccount.AccountID);

                        if (!string.IsNullOrEmpty(this.txtStoreName.Text.Trim()))
                        {
                            AccountFilter.AccountName = new Database.Filter.StringSearch.SearchFilter();
                            AccountFilter.AccountName.SearchString = txtStoreName.Text.Trim();
                        }
                        if (!string.IsNullOrEmpty(this.txtEmail.Text.Trim()))
                        {
                            AccountFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                            AccountFilter.EmailAddress.SearchString = txtEmail.Text.Trim();
                        }
                    }

                    if (AccountFilter.AccountIDs != null && AccountFilter.AccountIDs.Count > 0)
                    {
                        Accounts = ImageSolutions.Account.Account.GetAccounts(AccountFilter, ucPager.PageSize, ucPager.CurrentPageNumber, out intTotalRecord);

                        if (!string.IsNullOrEmpty(this.txtStreet.Text.Trim()))
                        {
                            Accounts = Accounts.FindAll(x => x.DefaultShippingAddressBook.AddressLine1.Contains(txtStreet.Text.Trim()));
                        }
                        if (!string.IsNullOrEmpty(this.txtCity.Text.Trim()))
                        {
                            Accounts = Accounts.FindAll(x => x.DefaultShippingAddressBook.City.Contains(txtCity.Text.Trim()));
                        }
                        if (!string.IsNullOrEmpty(this.txtState.Text.Trim()))
                        {
                            Accounts = Accounts.FindAll(x => x.DefaultShippingAddressBook.State.Contains(txtState.Text.Trim()));
                        }
                        if (!string.IsNullOrEmpty(this.txtZip.Text.Trim()))
                        {
                            Accounts = Accounts.FindAll(x => x.DefaultShippingAddressBook.PostalCode.Contains(txtZip.Text.Trim()));
                        }

                        gvAccounts.DataSource = Accounts;
                        gvAccounts.DataBind();
                        ucPager.TotalRecord = intTotalRecord;
                    }
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
                Accounts = null;
                AccountFilter = null;
            }
        }

        protected void txtFilter_TextChanged(object sender, EventArgs e)
        {
            string strURL = "/Admin/AccountOverview.aspx";

            if(!string.IsNullOrEmpty(this.txtStoreName.Text.Trim()))
            {
                strURL = string.Format("{0}{1}storename={2}", strURL, strURL == "/Admin/AccountOverview.aspx" ? "?" : "&", Server.UrlEncode(this.txtStoreName.Text.Trim()));
            }
            if (!string.IsNullOrEmpty(this.txtEmail.Text.Trim()))
            {
                strURL = string.Format("{0}{1}email={2}", strURL, strURL == "/Admin/AccountOverview.aspx" ? "?" : "&", Server.UrlEncode(this.txtEmail.Text.Trim()));
            }
            if (!string.IsNullOrEmpty(this.txtStreet.Text.Trim()))
            {
                strURL = string.Format("{0}{1}street={2}", strURL, strURL == "/Admin/AccountOverview.aspx" ? "?" : "&", Server.UrlEncode(this.txtStreet.Text.Trim()));
            }
            if (!string.IsNullOrEmpty(this.txtCity.Text.Trim()))
            {
                strURL = string.Format("{0}{1}city={2}", strURL, strURL == "/Admin/AccountOverview.aspx" ? "?" : "&", Server.UrlEncode(this.txtCity.Text.Trim()));
            }
            if (!string.IsNullOrEmpty(this.txtState.Text.Trim()))
            {
                strURL = string.Format("{0}{1}state={2}", strURL, strURL == "/Admin/AccountOverview.aspx" ? "?" : "&", Server.UrlEncode(this.txtState.Text.Trim()));
            }
            if (!string.IsNullOrEmpty(this.txtZip.Text.Trim()))
            {
                strURL = string.Format("{0}{1}zip={2}", strURL, strURL == "/Admin/AccountOverview.aspx" ? "?" : "&", Server.UrlEncode(this.txtZip.Text.Trim()));
            }

            Response.Redirect(strURL);
            //Response.Redirect(String.Format("/Admin/AccountOverview.aspx?storename=" + Server.UrlEncode(this.txtStoreName.Text.Trim())));
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            string strSQL = string.Empty;

            try
            {
                string strPath = Server.MapPath("\\Export\\Account\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }

                string strFileExportPath = Server.MapPath(string.Format("\\Export\\Account\\{0}\\Account_{1}.csv", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")));
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

        protected void CreateExportCSVBySQL(string filepath)
        {
            StringBuilder objReturn = new StringBuilder();

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {

                objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}"
                    , "account"
                    , "name"
                    , "addressline1"
                    , "addressline2"
                    , "countrycode"
                    , "state"
                    , "city"
                    , "postalcode")
                );
                
                objReturn.AppendLine();

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

SELECT DISTINCT 
	ah.AccountNamePath
	, a.AccountName
	, ab.AddressLine1
	, ab.AddressLine2
	, ab.CountryCode
	, ab.State
	, ab.City
	, ab.PostalCode
FROM Account (NOLOCK) a
Left Outer Join AddressBook (NOLOCK) ab on ab.AddressBookID = a.DefaultShippingAddressBookID
{2}
Inner Join #TempAccountHierarchy (NOLOCK) ah on ah.AccountID = a.AccountID
WHERE a.WebsiteID = {0}
ORDER BY ah.AccountNamePath

"
                        , Database.HandleQuote(CurrentWebsite.WebsiteID)
                        , Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID)
                        , CurrentUser.IsSuperAdmin ? string.Empty : string.Format("Inner Join #TempAccounts (NOLOCK) ta on ta.AccountID = a.AccountID")
                );

                objRead = Database.GetDataReader(strSQL);


                while (objRead.Read())
                {
                    objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}"
                        , Convert.ToString(objRead["AccountNamePath"])
                        , Convert.ToString(objRead["AccountName"])
                        , Convert.ToString(objRead["AddressLine1"])
                        , Convert.ToString(objRead["AddressLine2"])
                        , Convert.ToString(objRead["CountryCode"])
                        , Convert.ToString(objRead["State"])
                        , Convert.ToString(objRead["City"])
                        , Convert.ToString(objRead["PostalCode"])
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
        }
    }
}
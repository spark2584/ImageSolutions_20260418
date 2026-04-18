using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Control
{
    public partial class MyAccountSearchModal : BaseControl
    {
        public string WebsiteID
        {
            get { return hfWebsiteID.Value; }
            set { hfWebsiteID.Value = value; }
        }

        public string UserWebsiteID
        {
            get { return hfUserWebsiteID.Value; }
            set { hfUserWebsiteID.Value = value; }
        }

        public event SendAccountMessageToThePageHandler SendMessageToThePage;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Visible = false;
            }
        }
        public bool Show()
        {
            try
            {
                WebUtility.ClearForm(this);

                this.Visible = true;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this.Page, ex.Message);
            }

            return true;
        }
        protected void BindAccount()
        {
            if (
                (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && ThisPage.CurrentWebsite.WebsiteID == "53")
                ||
                (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && ThisPage.CurrentWebsite.WebsiteID == "20")
            )
            {
                BindAccountEnterprise();
            }
            else
            {
                List<ImageSolutions.Account.Account> Accounts = null;

                SqlDataReader objRead = null;
                string strSQL = string.Empty;

                try
                {
                    Accounts = new List<ImageSolutions.Account.Account>();
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

SET @numRecord = (SELECT COUNT(AccountID) FROM Account (NOLOCK) a WHERE a.WebsiteID = {0})
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



SELECT DISTINCT a.AccountID, ah.AccountNamePath as AccountName
FROM Account (NOLOCK) a
Inner Join #TempAccountHierarchy (NOLOCK) ah on ah.AccountID = a.AccountID
Left Outer Join UserAccount (NOLOCK) ua on ua.AccountID = a.AccountID
Left Outer Join UserWebsite (NOLOCK) uw on uw.UserWebsiteID = ua.UserWebsiteID
Left Outer Join UserInfo (NOLOCK) u on u.UserInfoID = uw.UserInfoID
WHERE a.WebsiteID = {0}
{2}
"
                        , Convert.ToString(hfWebsiteID.Value)
                        , Convert.ToString(hfUserWebsiteID.Value)
                        , string.IsNullOrEmpty(this.txtAccountName.Text.Trim()) && string.IsNullOrEmpty(this.txtEmail.Text.Trim())
                            ? string.Empty
                            : string.Format(@"
and (
    1 = 1
    {0}
    {1}
)"
                                , string.IsNullOrEmpty(this.txtAccountName.Text.Trim()) ? string.Empty : string.Format("and (a.AccountName like '%{0}%') ", this.txtAccountName.Text.Trim())
                                , string.IsNullOrEmpty(this.txtEmail.Text.Trim()) ? string.Empty : string.Format("and (u.EmailAddress like '%{0}%') ", this.txtEmail.Text.Trim())
                        )
                    );

                    objRead = Database.GetDataReader(strSQL);

                    while (objRead.Read())
                    {
                        Accounts.Add(new ImageSolutions.Account.Account(Convert.ToString(objRead["AccountID"])));
                    }

                    gvAccounts.DataSource = Accounts.OrderBy(x => x.AccountName);
                    gvAccounts.DataBind();
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(this.Page, ex.Message);
                }
                finally { }
            }            
        }

        protected void BindAccountEnterprise()
        {
            List<ImageSolutions.Account.Account> Accounts = null;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                Accounts = new List<ImageSolutions.Account.Account>();
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



SELECT DISTINCT a.AccountID, a.AccountName as AccountName
FROM Account (NOLOCK) a
{3}
Left Outer Join UserAccount (NOLOCK) ua on ua.AccountID = a.AccountID
Left Outer Join UserWebsite (NOLOCK) uw on uw.UserWebsiteID = ua.UserWebsiteID
Left Outer Join UserInfo (NOLOCK) u on u.UserInfoID = uw.UserInfoID
WHERE a.WebsiteID = {0}
{2}
"
                    , Convert.ToString(hfWebsiteID.Value)
                    , Convert.ToString(hfUserWebsiteID.Value)
                    , string.IsNullOrEmpty(this.txtAccountName.Text.Trim()) && string.IsNullOrEmpty(this.txtEmail.Text.Trim())
                        ? string.Empty
                        : string.Format(@"
and (
    1 = 1
    {0}
    {1}
)"
                            , string.IsNullOrEmpty(this.txtAccountName.Text.Trim()) ? string.Empty : string.Format("and (a.AccountName like '%{0}%') ", this.txtAccountName.Text.Trim())
                            , string.IsNullOrEmpty(this.txtEmail.Text.Trim()) ? string.Empty : string.Format("and (u.EmailAddress like '%{0}%') ", this.txtEmail.Text.Trim())
                    )
                    , ThisPage.CurrentUser.IsSuperAdmin ? string.Empty : string.Format("Inner Join #TempAccounts (NOLOCK) ta on ta.AccountID = a.AccountID")
                );

                objRead = Database.GetDataReader(strSQL);

                while (objRead.Read())
                {
                    Accounts.Add(new ImageSolutions.Account.Account(Convert.ToString(objRead["AccountID"])));
                }

                gvAccounts.DataSource = Accounts.OrderBy(x => x.AccountName);
                gvAccounts.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this.Page, ex.Message);
            }
            finally { }
        }

        protected void txtAccountName_TextChanged(object sender, EventArgs e)
        {
            if (txtAccountName.Text.Length > 3)
            {
                BindAccount();
            }
            else
            {
                txtAccountName.Text = string.Empty;
            }
        }

        protected void txtEmail_TextChanged(object sender, EventArgs e)
        {
            BindAccount();
        }

        protected void gvAccounts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectLine")
            {
                string strAccountID = Convert.ToString(e.CommandArgument);
                SendMessageToThePage(strAccountID);

                gvAccounts.DataSource = null;
                gvAccounts.DataBind();

                this.Visible = false;
            }
        }

        public void Hide()
        {
            this.Visible = false;
        }

    }
}
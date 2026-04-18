using ImageSolutionsWebsite.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Management;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class OrderOverview : BasePageAdminUserWebSiteAuth
    {
        protected string mAccount = string.Empty;
        protected string mStartDate = string.Empty;
        protected string mEndDate = string.Empty;
        protected string mOrderNumber = string.Empty;
        protected string mPendingApproval = String.Empty;

        class SalesByItem
        {
            public string ItemName { get; set; }
            public string ItemNumber { get; set; }
            public string ItemDescription { get; set; }
            public int Quantity { get; set; }
            public double Total { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            mAccount = Request.QueryString.Get("account");
            mStartDate = Request.QueryString.Get("startdate");
            mEndDate = Request.QueryString.Get("enddate");
            mOrderNumber = Request.QueryString.Get("ordernumber");
            mPendingApproval = Request.QueryString.Get("pendingapproval");
            aOrderCreate.Visible = CurrentUser.CurrentUserWebSite.WebSite.Name == "ABS Companies Uniforms";

            UpdateAccount();

            if (!Page.IsPostBack)
            {
                Initialize();
            }
        }

        protected void Initialize()
        {
            pnlOrderNumberSearch.Visible = CurrentUser.IsSuperAdmin;

            hfAccountID.Value = Convert.ToString(mAccount);

            if (CurrentUser.CurrentUserWebSite.Accounts.Count <= 300)
            {
                ddlAccount.Visible = true;
                BindAccount();
                txtAccount.Visible = false;
                btnAccountSearch.Visible = false;
                btnAccountRemove.Visible = false;
            }
            else
            {
                ddlAccount.Visible = false;
                txtAccount.Visible = true;

                btnAccountSearch.Visible = true;
                btnAccountSearch.Enabled = true;
                btnAccountRemove.Enabled = true;
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


            if (string.IsNullOrEmpty(mStartDate))
            {
                DateTime StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
                mStartDate = StartDate.ToString("MM/dd/yyyy");
                txtOrderStartDate.Text = mStartDate;
            }

            //BindAccount();

            this.ddlAccount.SelectedValue = mAccount;
            this.txtOrderStartDate.Text = mStartDate;
            this.txtOrderEndDate.Text = mEndDate;
            this.txtOrderNumber.Text = mOrderNumber;
            this.chkPendingApproval.Checked = mPendingApproval == "True";

            //if (
            //    CurrentUser.CurrentUserWebSite.WebSite.WebsiteID == "7" || CurrentUser.CurrentUserWebSite.WebSite.WebsiteID == "8"  //Albertsons
            //    //|| CurrentUser.CurrentUserWebSite.WebSite.WebsiteID == "53" || CurrentUser.CurrentUserWebSite.WebSite.WebsiteID == "20" //Enterprise (staging and prod)
            //)
            //{
            //    lbnDownloadReconciliation.Visible = false;
            //}

            //If Enterprise
            if ( 
                (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) =="staging" && CurrentWebsite.WebsiteID == "53")
                || 
                (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "20")
            )
            {
                BindSalesOrderBySQLEnterprise();
            }
            else
            {
                BindSalesOrderBySQL();
            }

            //Temp - only for Mavis
            this.gvSalesOrders.Columns[7].Visible = CurrentWebsite.WebsiteID == "25";
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
                        Response.Redirect(String.Format("/Admin/OrderOverview.aspx?account=" + Server.UrlEncode(hfAccountID.Value) + "&startdate=" + Server.UrlEncode(this.txtOrderStartDate.Text.Trim()) + "&enddate=" + Server.UrlEncode(this.txtOrderEndDate.Text.Trim()) + "&ordernumber=" + Server.UrlEncode(this.txtOrderNumber.Text.Trim()) + "&pendingapproval=" + Server.UrlEncode(Convert.ToString(this.chkPendingApproval.Checked))));
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

        protected void BindAccount()
        {
            this.ddlAccount.DataSource = CurrentUser.CurrentUserWebSite.Accounts.OrderBy(m => m.AccountNamePath);
            this.ddlAccount.DataBind();
            this.ddlAccount.Items.Insert(0, new ListItem("All", ""));
        }

        private List<ImageSolutions.SalesOrder.SalesOrder> mSalesOrders = null;
        public List<ImageSolutions.SalesOrder.SalesOrder> SalesOrders
        {
            get
            {
                if (CurrentUser.IsSuperAdmin)
                    mSalesOrders = CurrentUser.CurrentUserWebSite.WebSite.SalesOrders;
                else
                {
                    List<ImageSolutions.Account.Account> Accounts = CurrentUser.CurrentUserWebSite.Accounts;
                    //mSalesOrders = CurrentUser.CurrentUserWebSite.WebSite.SalesOrders.FindAll(m => Accounts.Exists(n => n.AccountID == m.AccountID) || Accounts.Exists(n => n.ChildAccounts.Exists(o => o.AccountID == m.AccountID)));
                    mSalesOrders = CurrentUser.CurrentUserWebSite.WebSite.SalesOrders.Where(m => Accounts.Exists(n => n.AccountID == m.AccountID) || Accounts.Exists(n => n.ChildAccounts.Exists(o => o.AccountID == m.AccountID))).ToList();
                }

                if (!string.IsNullOrEmpty(hfAccountID.Value))
                    mSalesOrders = mSalesOrders.FindAll(m => m.AccountID == hfAccountID.Value);

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

                if (!string.IsNullOrEmpty(this.txtOrderNumber.Text.Trim()))
                {
                    mSalesOrders = mSalesOrders.FindAll(m => m.SalesOrderID == txtOrderNumber.Text.Trim());
                }

                return mSalesOrders;
            }
        }

        protected void BindSalesOrders()
        {
            int intTotalRecord = 0;
            try
            {
                intTotalRecord = SalesOrders.Count();
                this.gvSalesOrders.DataSource = SalesOrders.OrderByDescending(x => Convert.ToInt32(x.SalesOrderID)).ToList().Skip((ucPager.CurrentPageNumber - 1) * ucPager.PageSize).Take(ucPager.PageSize);
                this.gvSalesOrders.DataBind();

                ucPager.TotalRecord = intTotalRecord;

            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindSalesOrderBySQL()
        {
            List<ImageSolutions.SalesOrder.SalesOrder> SalesOrders = null;
            int intTotalRecord = 0;
            DataSet objData = null;
            string strSQL = string.Empty;
            
            try
            {
                SalesOrders = new List<ImageSolutions.SalesOrder.SalesOrder>();

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

            SELECT * FROM
            (
                SELECT *, COUNT(*) OVER () AS TotalRecord FROM
                (
		            SELECT *, ROW_NUMBER() OVER (ORDER BY SalesOrderID DESC) AS RowNumber FROM
		            (

SELECT s.TransactionDate, s.SalesOrderID, a.AccountName, ISNULL(ISNULL(FullName.Value,'') + ' | ' + ISNULL(u.UserName, u.EmailAddress), Shipping.Name) as UserDescription
	, s.IsClosed, s.IsPendingApproval, ISNULL(LineTotal.Value,0) + ISNULL(s.ShippingAmount,0) + ISNULL(s.TaxAmount,0) as Total
    , s.InvoiceFilePath, CASE WHEN s.Status = 'Rejected' THEN CAST(1 as bit) ELSE CAST(0 as bit) END as IsRejected
FROM SalesOrder (NOLOCK) s
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = s.UserInfoID
Inner JOin UserWebsite (NOLOCK) uw on uw.UserWebsiteID = s.UserWebsiteID
Inner Join Account (NOLOCK) a on a.AccountID = s.AccountID
{2}
Inner Join #TempAccountHierarchy (NOLOCK) ah on ah.AccountID = a.AccountID
Outer Apply
(
	SELECT SUM(sl2.Quantity * sl2.UnitPrice) Value
	FROM SalesOrderLine (NOLOCK) sl2
	WHERE sl2.SalesOrderID = s.SalesOrderID
) LineTotal
Outer Apply
(
	SELECT u.FirstName + ' ' + u.LastName as Value
) FullName
Outer Apply
(
	SELECT at2.FirstName + ' ' + at2.LastName as Name
	FROM AddressTrans (NOLOCK) at2
	WHERE at2.AddressTransID = s.DeliveryAddressTransID
) Shipping
WHERE s.WebsiteID = {0}
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
                        , CurrentUser.IsSuperAdmin ? string.Empty : string.Format("Inner Join #TempAccounts (NOLOCK) ta on ta.AccountID = a.AccountID")
                        , string.IsNullOrEmpty(hfAccountID.Value) ? string.Empty : string.Format("AND a.AccountID = {0}", Database.HandleQuote(Convert.ToString(hfAccountID.Value)))
                        , string.IsNullOrEmpty(txtOrderStartDate.Text.Trim()) ? string.Empty : string.Format("AND s.CreatedOn >= {0}", Database.HandleQuote(txtOrderStartDate.Text.Trim()))
                        , string.IsNullOrEmpty(txtOrderEndDate.Text.Trim()) ? string.Empty : string.Format("AND s.CreatedOn < {0}", Database.HandleQuote(txtOrderEndDate.Text.Trim()))
                        , string.IsNullOrEmpty(txtOrderNumber.Text.Trim()) ? string.Empty : string.Format("AND CAST(s.SalesOrderID as varchar) = {0}", Database.HandleQuote(txtOrderNumber.Text.Trim()))

                        , (ucPager.CurrentPageNumber - 1) * ucPager.PageSize + 1
                        , (ucPager.CurrentPageNumber) * ucPager.PageSize

                        , !chkPendingApproval.Checked ? string.Empty : string.Format("AND s.IsPendingApproval = 1")
                );

                objData = Database.GetDataSet(strSQL);

                if (objData != null)
                {
                    this.gvSalesOrders.DataSource = objData;
                    this.gvSalesOrders.DataBind();

                    if (objData.Tables != null && objData.Tables.Count > 0
                        && objData.Tables[0].Rows != null && objData.Tables[0].Rows.Count > 0
                        && objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) intTotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    ucPager.TotalRecord = intTotalRecord;
                }

                //while (objRead.Read())
                //{
                //    ImageSolutions.SalesOrder.SalesOrder SalesOrder = new ImageSolutions.SalesOrder.SalesOrder(Convert.ToString(objRead["SalesOrderID"]));
                //    SalesOrders.Add(SalesOrder);
                //}

                //intTotalRecord = SalesOrders.Count();
                //this.gvSalesOrders.DataSource = SalesOrders.OrderByDescending(x => Convert.ToInt32(x.SalesOrderID)).ToList().Skip((ucPager.CurrentPageNumber - 1) * ucPager.PageSize).Take(ucPager.PageSize);
                //this.gvSalesOrders.DataBind();

                //ucPager.TotalRecord = intTotalRecord;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void BindSalesOrderBySQLEnterprise()
        {
            List<ImageSolutions.SalesOrder.SalesOrder> SalesOrders = null;
            int intTotalRecord = 0;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                SalesOrders = new List<ImageSolutions.SalesOrder.SalesOrder>();

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
		            SELECT *, ROW_NUMBER() OVER (ORDER BY SalesOrderID DESC) AS RowNumber FROM
		            (

SELECT s.TransactionDate, s.SalesOrderID, a.AccountName, ISNULL(FullName.Value,'') + ' | ' + ISNULL(u.UserName, u.EmailAddress) as UserDescription
	, s.IsClosed, s.IsPendingApproval, ISNULL(LineTotal.Value,0) + ISNULL(s.ShippingAmount,0) + ISNULL(s.TaxAmount,0) + ISNULL(s.IPDDutiesAndTaxesAmount,0) as Total
    , s.InvoiceFilePath, CASE WHEN s.Status = 'Rejected' THEN CAST(1 as bit) ELSE CAST(0 as bit) END as IsRejected
FROM SalesOrder (NOLOCK) s
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = s.UserInfoID
Inner JOin UserWebsite (NOLOCK) uw on uw.UserWebsiteID = s.UserWebsiteID
Inner Join Account (NOLOCK) a on a.AccountID = s.AccountID
{2}

Outer Apply
(
	SELECT SUM(sl2.Quantity * sl2.UnitPrice) Value
	FROM SalesOrderLine (NOLOCK) sl2
	WHERE sl2.SalesOrderID = s.SalesOrderID
) LineTotal
Outer Apply
(
	SELECT u.FirstName + ' ' + u.LastName as Value
) FullName
WHERE s.WebsiteID = {0}
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
                        , CurrentUser.IsSuperAdmin ? string.Empty : string.Format("Inner Join #TempAccounts (NOLOCK) ta on ta.AccountID = a.AccountID")
                        , string.IsNullOrEmpty(hfAccountID.Value) ? string.Empty : string.Format("AND a.AccountID = {0}", Database.HandleQuote(Convert.ToString(hfAccountID.Value)))
                        , string.IsNullOrEmpty(txtOrderStartDate.Text.Trim()) ? string.Empty : string.Format("AND s.CreatedOn >= {0}", Database.HandleQuote(txtOrderStartDate.Text.Trim()))
                        , string.IsNullOrEmpty(txtOrderEndDate.Text.Trim()) ? string.Empty : string.Format("AND s.CreatedOn < {0}", Database.HandleQuote(txtOrderEndDate.Text.Trim()))
                        , string.IsNullOrEmpty(txtOrderNumber.Text.Trim()) ? string.Empty : string.Format("AND CAST(s.SalesOrderID as varchar) = {0}", Database.HandleQuote(txtOrderNumber.Text.Trim()))

                        , (ucPager.CurrentPageNumber - 1) * ucPager.PageSize + 1
                        , (ucPager.CurrentPageNumber) * ucPager.PageSize

                        , !chkPendingApproval.Checked ? string.Empty : string.Format("AND s.IsPendingApproval = 1")
                );

                objData = Database.GetDataSet(strSQL);

                if (objData != null)
                {
                    this.gvSalesOrders.DataSource = objData;
                    this.gvSalesOrders.DataBind();

                    if (objData.Tables != null && objData.Tables.Count > 0
                        && objData.Tables[0].Rows != null && objData.Tables[0].Rows.Count > 0
                        && objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) intTotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    ucPager.TotalRecord = intTotalRecord;
                }

                //while (objRead.Read())
                //{
                //    ImageSolutions.SalesOrder.SalesOrder SalesOrder = new ImageSolutions.SalesOrder.SalesOrder(Convert.ToString(objRead["SalesOrderID"]));
                //    SalesOrders.Add(SalesOrder);
                //}

                //intTotalRecord = SalesOrders.Count();
                //this.gvSalesOrders.DataSource = SalesOrders.OrderByDescending(x => Convert.ToInt32(x.SalesOrderID)).ToList().Skip((ucPager.CurrentPageNumber - 1) * ucPager.PageSize).Take(ucPager.PageSize);
                //this.gvSalesOrders.DataBind();

                //ucPager.TotalRecord = intTotalRecord;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void ddlAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            hfAccountID.Value = Convert.ToString(ddlAccount.SelectedValue);

            Response.Redirect(String.Format("/Admin/OrderOverview.aspx?account=" + Server.UrlEncode(Convert.ToString(hfAccountID.Value)) + "&startdate=" + Server.UrlEncode(this.txtOrderStartDate.Text.Trim()) + "&enddate=" + Server.UrlEncode(this.txtOrderEndDate.Text.Trim()) + "&ordernumber=" + Server.UrlEncode(this.txtOrderEndDate.Text.Trim()) + "&pendingapproval=" + Server.UrlEncode(Convert.ToString(this.chkPendingApproval.Checked)) ));
            //BindSalesOrders();
        }

        protected void lbnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (CurrentWebsite.DisplayTariffCharge)
                {
                    DataSet DataSet = GetOrderReportBySQLWithTariff();
                    string strFileName = string.Format("order_export{0}", DateTime.UtcNow.ToString("yyyyMMddHHmmssffff"));
                    ExportToExcel(DataSet.Tables[0], strFileName);
                }
                else
                {
                    DataSet DataSet = GetOrderReportBySQL();
                    string strFileName = string.Format("order_export{0}", DateTime.UtcNow.ToString("yyyyMMddHHmmssffff"));
                    ExportToExcel(DataSet.Tables[0], strFileName);
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }

            ////http://www.dotnetspeaks.com/DisplayArticle.aspx?ID=97
            //try
            //{
            //    if (SalesOrders == null || SalesOrders.Count == 0) throw new Exception("No results found");

            //    StringBuilder objCSV = null;

            //    string strPath = Server.MapPath("\\Export\\OrderReport\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");
            //    if (!Directory.Exists(strPath))
            //    {
            //        Directory.CreateDirectory(strPath);
            //    }
            //    string strFileExportPath = Server.MapPath(string.Format("\\Export\\OrderReport\\{0}\\order_export{1}.csv", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")));

            //    try
            //    {
            //        objCSV = new StringBuilder();
            //        objCSV.Append("OrderNumber,TransactionDate,Account,FirstName,LastName,Email,Address1,Address2,City,State,Zip,Country,ItemName,ItemNumber,ItemDescription,Quantity,UnitPrice,LineSubtotal,TotalShippingAmount,TotalTaxAmount,TotalOrderAmount");
            //        objCSV.AppendLine();

            //        foreach (ImageSolutions.SalesOrder.SalesOrder objSalesOrder in SalesOrders.OrderBy(x => Convert.ToInt32(x.SalesOrderID)).ToList())
            //        {
            //            foreach (ImageSolutions.SalesOrder.SalesOrderLine objSalesOrderLine in objSalesOrder.SalesOrderLines)
            //            {
            //                objCSV.Append(objSalesOrder.SalesOrderID.Replace(",", "")).Append(",");
            //                objCSV.Append(objSalesOrder.TransactionDate.Value.ToShortDateString()).Append(",");
            //                objCSV.Append(objSalesOrder.Account.AccountNamePath.Replace(",", "")).Append(",");
            //                objCSV.Append(objSalesOrder.UserInfo.FirstName.Replace(",", "")).Append(",");
            //                objCSV.Append(objSalesOrder.UserInfo.LastName.Replace(",", "")).Append(",");
            //                objCSV.Append(objSalesOrder.UserInfo.EmailAddress.Replace(",", "")).Append(",");
            //                if(objSalesOrder.DeliveryAddress != null)
            //                {
            //                    objCSV.Append(objSalesOrder.DeliveryAddress.AddressLine1.Replace(",", "")).Append(",");
            //                    objCSV.Append(objSalesOrder.DeliveryAddress.AddressLine2.Replace(",", "")).Append(",");
            //                    objCSV.Append(objSalesOrder.DeliveryAddress.City.Replace(",", "")).Append(",");
            //                    objCSV.Append(objSalesOrder.DeliveryAddress.State.Replace(",", "")).Append(",");
            //                    objCSV.Append(objSalesOrder.DeliveryAddress.PostalCode.Replace(",", "")).Append(",");
            //                    objCSV.Append(objSalesOrder.DeliveryAddress.CountryCode.Replace(",", "")).Append(",");
            //                }
            //                else
            //                {
            //                    objCSV.Append(",");
            //                    objCSV.Append(",");
            //                    objCSV.Append(",");
            //                    objCSV.Append(",");
            //                    objCSV.Append(",");
            //                    objCSV.Append(",");
            //                }
            //                objCSV.Append(objSalesOrderLine.Item.ItemName.Replace(",", "")).Append(",");
            //                objCSV.Append(objSalesOrderLine.Item.ItemNumber.Replace(",", "")).Append(",");
            //                objCSV.Append(objSalesOrderLine.Item.Description.Replace(",", "")).Append(",");
            //                objCSV.Append(objSalesOrderLine.Quantity).Append(",");
            //                objCSV.Append(objSalesOrderLine.UnitPrice.ToString("0.00")).Append(",");
            //                objCSV.Append(objSalesOrderLine.LineSubTotal.Value.ToString("0.00")).Append(",");
            //                objCSV.Append(objSalesOrder.ShippingAmount.ToString("0.00")).Append(",");
            //                objCSV.Append(objSalesOrder.TaxAmount.ToString("0.00")).Append(",");
            //                objCSV.Append(objSalesOrder.Total.ToString("0.00")).Append(",");
            //                objCSV.AppendLine();
            //            }
            //        }

            //        if (objCSV != null)
            //        {
            //            using (StreamWriter objWriter = new StreamWriter(strFileExportPath))
            //            {
            //                objWriter.Write(objCSV.ToString());
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        throw ex;
            //    }
            //    finally
            //    {
            //        objCSV = null;
            //    }

            //    Response.ContentType = "text/csv";
            //    //Response.ContentType = "application/vnd.ms-excel";
            //    Response.AppendHeader("Content-Disposition", "attachment; filename=" + System.IO.Path.GetFileName(strFileExportPath));
            //    Response.WriteFile(strFileExportPath);

            //    HttpContext.Current.Response.Flush();
            //    HttpContext.Current.Response.SuppressContent = true;
            //    HttpContext.Current.ApplicationInstance.CompleteRequest();

                
            //    ////Excel
            //    //DataTable dt = new DataTable();

            //    //dt.Columns.Add("OrderNumber");
            //    //dt.Columns.Add("TransactionDate");
            //    //dt.Columns.Add("Store");
            //    //dt.Columns.Add("FirstName");
            //    //dt.Columns.Add("LastName");
            //    //dt.Columns.Add("Email");
            //    //dt.Columns.Add("ItemName");
            //    //dt.Columns.Add("ItemNumber");
            //    //dt.Columns.Add("ItemDescription");
            //    //dt.Columns.Add("Quantity");
            //    //dt.Columns.Add("UnitPrice");
            //    //dt.Columns.Add("LineSubtotal");
            //    //dt.Columns.Add("TotalShippingAmount");
            //    //dt.Columns.Add("TotalTaxAmount");
            //    //dt.Columns.Add("TotalOrderAmount");

            //    //foreach (ImageSolutions.SalesOrder.SalesOrder objSalesOrder in SalesOrders)
            //    //{
            //    //    foreach (ImageSolutions.SalesOrder.SalesOrderLine objSalesOrderLine in objSalesOrder.SalesOrderLines)
            //    //    {
            //    //        DataRow objRow = dt.NewRow();
            //    //        objRow["OrderNumber"] = objSalesOrder.SalesOrderID;
            //    //        objRow["TransactionDate"] = objSalesOrder.TransactionDate.ToString();
            //    //        objRow["Store"] = objSalesOrder.Account.AccountName;
            //    //        objRow["FirstName"] = objSalesOrder.UserInfo.FirstName;
            //    //        objRow["LastName"] = objSalesOrder.UserInfo.LastName;
            //    //        objRow["Email"] = objSalesOrder.UserInfo.EmailAddress;

            //    //        objRow["ItemName"] = objSalesOrderLine.Item.ItemName;
            //    //        objRow["ItemNumber"] = objSalesOrderLine.Item.ItemNumber;
            //    //        objRow["ItemDescription"] = objSalesOrderLine.Item.Description;
            //    //        objRow["Quantity"] = objSalesOrderLine.Quantity;
            //    //        objRow["UnitPrice"] = objSalesOrderLine.UnitPrice;
            //    //        objRow["LineSubtotal"] = objSalesOrderLine.LineSubTotal;

            //    //        objRow["TotalShippingAmount"] = objSalesOrder.ShippingAmount;
            //    //        objRow["TotalTaxAmount"] = objSalesOrder.TaxAmount;
            //    //        objRow["TotalOrderAmount"] = objSalesOrder.Total;
            //    //        dt.Rows.Add(objRow);
            //    //    }
            //    //}

            //    ////Double dimensional array to keep style name and style
            //    //string[,] styles = { { "text", "\\@;" },
            //    //               { "text", "\\@;" },
            //    //               { "text", "\\@;" },
            //    //               { "text", "\\@;" },
            //    //               { "text", "\\@;" },
            //    //               { "text", "\\@;" },
            //    //               { "number", "0\\.00;" },
            //    //               { "number", "0\\.00;" },
            //    //               { "number", "0\\.00;" },
            //    //               { "text", "\\@;" },
            //    //               { "text", "\\@;" },
            //    //               { "text", "\\@;" },
            //    //               { "number", "0\\.00;" },
            //    //               { "number", "0\\.00;" } };

            //    ////Dummy GridView to hold data to be exported in excel
            //    //System.Web.UI.WebControls.GridView gvExport = new System.Web.UI.WebControls.GridView();
            //    //gvExport.AllowPaging = false;
            //    //gvExport.DataSource = dt;
            //    //gvExport.DataBind();

            //    //StringWriter sw = new StringWriter();
            //    //HtmlTextWriter hw = new HtmlTextWriter(sw);
            //    //int cnt = styles.Length / 2;

            //    //for (int i = 0; i < gvExport.Rows.Count; i++)
            //    //{
            //    //    for (int j = 0; j < cnt; j++)
            //    //    {
            //    //        //Apply style to each cell
            //    //        gvExport.Rows[i].Cells[j].Attributes.Add("class", styles[j, 0]);
            //    //    }
            //    //}

            //    //gvExport.RenderControl(hw);
            //    //StringBuilder style = new StringBuilder();
            //    //style.Append("<style>");
            //    //for (int j = 0; j < cnt; j++)
            //    //{
            //    //    style.Append("." + styles[j, 0] + " { mso-number-format:" + styles[j, 1] + " }");
            //    //}

            //    //style.Append("</style>");
            //    //Response.Clear();
            //    //Response.Buffer = true;
            //    //Response.AddHeader("content-disposition", "attachment;filename=order_export.xls"); Response.Charset = "";
            //    //Response.ContentType = "application/vnd.ms-excel";
            //    //Response.Write(style.ToString());
            //    ////string headerTable = @"<Table><tr><td></td><td>Report Header</td></tr><tr><td>second</td></tr></Table>";
            //    ////Response.Output.Write(headerTable);
            //    //Response.Output.Write(sw.ToString());
            //    //Response.Flush();
            //    //Response.End();
            //}
            //catch (Exception ex)
            //{
            //    WebUtility.DisplayJavascriptMessage(this, ex.Message);
            //}
        }

        protected DataSet GetOrderReportBySQL()
        {
            DataSet DataSet = new DataSet();
            string strSQL = string.Empty;

            try
            {
                ImageSolutions.Custom.CustomField CustomField = new ImageSolutions.Custom.CustomField();
                ImageSolutions.Custom.CustomFieldFilter CustomFieldFilter = new ImageSolutions.Custom.CustomFieldFilter();
                CustomFieldFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                CustomFieldFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                CustomFieldFilter.Location = new Database.Filter.StringSearch.SearchFilter();
                CustomFieldFilter.Location.SearchString = "checkout";
                CustomFieldFilter.Name = new Database.Filter.StringSearch.SearchFilter();
                CustomFieldFilter.Name.SearchString = "Department";
                CustomField = ImageSolutions.Custom.CustomField.GetCustomField(CustomFieldFilter); 

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


SELECT s.SalesOrderID as OrderNumber
	, CONVERT(VARCHAR(10), s.TransactionDate , 101) as TransactionDate
	, ah.AccountNamePath
    , ap.AccountName as ParentAccount
    , a.AccountName as Account

    {11}

	, u.FirstName
	, u.LastName
	, u.EmailAddress
	, at.AddressLine1 as Address1
	, at.AddressLine2 as Address2
	, at.City
	, at.State
	, at.PostalCode as Zip
	, at.CountryCode as Country
	, i.ItemName
    , i.ItemNumber
    --, i.ItemNumber + ' (' + i.StoreDisplayName + ')' as ItemDescription
    , REPLACE(i.SalesDescription, char(160), ' ') as ItemDescription
    , sl.Quantity
    , sl.UnitPrice
    , sl.Quantity * sl.UnitPrice as LineSubtotal
	, s.ShippingAmount as TotalShippingAmount
	, s.TaxAmount as TotalTaxAmount
	, line.LineTotal + s.ShippingAmount + s.TaxAmount as Total
    , CASE WHEN (s.IsPendingApproval =1 or s.IsPendingItemPersonalizationApproval = 1) THEN 'Yes' Else '' END as PendingApproval

    {7}

    {10}

    , backorder.Quantity as BackOrdered

    {12}

FROM SalesOrder (NOLOCK) s
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = s.UserInfoID
Inner Join UserWebsite (NOLOCK) uw on uw.UserWebsiteID = s.UserWebsiteID
Inner Join SalesOrderLine (NOLOCK) sl on sl.SalesOrderID = s.SalesOrderID
Inner Join Item (NOLOCK) i on i.ItemID = sl.ItemID
Inner Join Account (NOLOCK) a on a.AccountID = s.AccountID
Left Outer Join Account (NOLOCK) ap on ap.AccountID = a.ParentID
{2}
Inner Join #TempAccountHierarchy (NOLOCK) ah on ah.AccountID = a.AccountID
Inner Join AddressTrans (NOLOCK) at on at.AddressTransID = s.DeliveryAddressTransID

{8}

Outer Apply
(
	SELECT p2.PaymentID 
	FROM Payment (NOLOCK) p2 
	WHERE p2.SalesOrderID = s.SalesOrderID
	and isnull(p2.CreditCardTransactionLogID,0) != 0
) paym
Outer Apply
(
	SELECT SUM(sl2.Quantity * sl2.UnitPrice) LineTotal
	FROM SalesOrderLine (NOLOCK) sl2
	WHERE sl2.SalesOrderID = s.SalesOrderID
) line
Outer Apply
(
	SELECT STUFF((
		SELECT ', ' + CAST(f2.TrackingNumber as VARCHAR(MAX))
		FROM Fulfillment (NOLOCK) f2
		WHERE f2.SalesOrderID = s.SalesOrderID
		FOR XML PATH('')
    ), 1, 2, '') as TrackingNumber
) fulfill


Outer Apply
(
	SELECT TOP 1 cv2.Value as Department
	FROM CustomValue (NOLOCK) cv2
	Inner Join CustomField (NOLOCK) cf2 on cf2.CustomFieldID = cv2.CustomFieldID
	WHERE cv2.SalesOrderID = s.SalesOrderID
	and cf2.Name = 'Department'
) Dept

Outer Apply
(
	SELECT b2.QuantityBackOrdered as Quantity 
	FROM SalesOrderLineBackOrder b2
	WHERE b2.SalesOrderInternalID = s.NetSuiteInternalID 
	and b2.ItemInternalID = i.InternalID
) backorder

{13}

WHERE s.WebsiteID = {0}
{3}
{4}
{5}
{6}

{9}
ORDER BY s.SalesOrderID
"
                        , Database.HandleQuote(CurrentWebsite.WebsiteID)
                        , Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID)
                        //, CurrentUser.IsSuperAdmin ? string.Empty : string.Format("AND a.AccountID in (SELECT AccountID FROM UserAccount (NOLOCK) ua WHERE ua.UserWebsiteID = {0})", Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID))
                        , CurrentUser.IsSuperAdmin ? string.Empty : string.Format("Inner Join #TempAccounts (NOLOCK) ta on ta.AccountID = a.AccountID")
                        , string.IsNullOrEmpty(hfAccountID.Value) ? string.Empty : string.Format("AND a.AccountID = {0}", Database.HandleQuote(Convert.ToString(hfAccountID.Value)))
                        , string.IsNullOrEmpty(txtOrderStartDate.Text.Trim()) ? string.Empty : string.Format("AND s.CreatedOn >= {0}", Database.HandleQuote(txtOrderStartDate.Text.Trim()))
                        , string.IsNullOrEmpty(txtOrderEndDate.Text.Trim()) ? string.Empty : string.Format("AND s.CreatedOn < {0}", Database.HandleQuote(txtOrderEndDate.Text.Trim()))
                        , string.IsNullOrEmpty(txtOrderNumber.Text.Trim()) ? string.Empty : string.Format("AND CAST(s.SalesOrderID as varchar) = {0}", Database.HandleQuote(txtOrderNumber.Text.Trim()))
                        // Enterprise
                        , (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "53") 
                            || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "20") 
                                ? ", SUBSTRING(a.StoreNumber,1,2) as [group], a.StoreNumber as group_branch, enterprisestore.BranchAdminLgcyID as [regional_group]" 
                                : String.Empty

                        , (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "53")
                            || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "20")
                                ? @"
Outer Apply
(
	SELECT Top 1 BranchAdminLgcyID
	FROM EnterpriseCustomer (NOLOCK) ec
	WHERE ec.StoreNumber = a.StoreNumber
	and ec.IsIndividual = 0
) enterprisestore
"
                                : String.Empty

                        , !chkPendingApproval.Checked ? string.Empty : string.Format("AND s.IsPendingApproval = 1")
                        , CustomField != null && !string.IsNullOrEmpty(CustomField.CustomFieldID) ? ", ISNULL(Dept.Department,'') as Department" : string.Empty

                        //BK
                        , (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "26")
                            || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "2")
                            ? ", CASE WHEN ap.ParentID is null THEN a.AccountName ELSE ap.AccountName END as [FOA]"
                                : String.Empty

                        //Mavis
                        , Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && (CurrentWebsite.WebsiteID == "25" || CurrentWebsite.WebsiteID == "26")
                            ? ", ISNULL(uw.EmployeeID, '') as EmployeeID, mavis.StoreNumber"
                                : String.Empty
                        , Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && (CurrentWebsite.WebsiteID == "25" || CurrentWebsite.WebsiteID == "26")
                            ? @"
Outer Apply
(
    SELECT TOP 1 c2.StoreNumber
    FROM MavisTireCustomer (NOLOCK) c2 
    WHERE c2.EmployeeNumber = uw.EmployeeID
) mavis
"
                                : String.Empty
                    
                );

                DataSet = Database.GetDataSet(strSQL);

            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }

            return DataSet;
        }

        protected DataSet GetOrderReportBySQLWithTariff()
        {
            DataSet DataSet = new DataSet();
            string strSQL = string.Empty;

            try
            {
                ImageSolutions.Custom.CustomField CustomField = new ImageSolutions.Custom.CustomField();
                ImageSolutions.Custom.CustomFieldFilter CustomFieldFilter = new ImageSolutions.Custom.CustomFieldFilter();
                CustomFieldFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                CustomFieldFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                CustomFieldFilter.Location = new Database.Filter.StringSearch.SearchFilter();
                CustomFieldFilter.Location.SearchString = "checkout";
                CustomFieldFilter.Name = new Database.Filter.StringSearch.SearchFilter();
                CustomFieldFilter.Name.SearchString = "Department";
                CustomField = ImageSolutions.Custom.CustomField.GetCustomField(CustomFieldFilter);

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


SELECT s.SalesOrderID as OrderNumber
	, CONVERT(VARCHAR(10), s.TransactionDate , 101) as TransactionDate
	, ah.AccountNamePath
    , ap.AccountName as ParentAccount
    , a.AccountName as Account

    {11}

	, u.FirstName
	, u.LastName
	, u.EmailAddress
	, at.AddressLine1 as Address1
	, at.AddressLine2 as Address2
	, at.City
	, at.State
	, at.PostalCode as Zip
	, at.CountryCode as Country
	, i.ItemName
    , i.ItemNumber
    --, i.ItemNumber + ' (' + i.StoreDisplayName + ')' as ItemDescription
    , REPLACE(i.SalesDescription, char(160), ' ') as ItemDescription
    , sl.Quantity
    , CASE WHEN sl.TariffCharge IS NOT NULL THEN sl.OnlinePrice ELSE sl.UnitPrice END AS UnitPrice
    , sl.TariffCharge
    , sl.Quantity * sl.UnitPrice as LineSubtotal
	, s.ShippingAmount as TotalShippingAmount
	, s.TaxAmount as TotalTaxAmount
	, line.LineTotal + s.ShippingAmount + s.TaxAmount as Total
    , CASE WHEN (s.IsPendingApproval =1 or s.IsPendingItemPersonalizationApproval = 1) THEN 'Yes' Else '' END as PendingApproval

    {7}

    {10}

    , backorder.Quantity as BackOrdered

    {12}

FROM SalesOrder (NOLOCK) s
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = s.UserInfoID
Inner Join UserWebsite (NOLOCK) uw on uw.UserWebsiteID = s.UserWebsiteID
Inner Join SalesOrderLine (NOLOCK) sl on sl.SalesOrderID = s.SalesOrderID
Inner Join Item (NOLOCK) i on i.ItemID = sl.ItemID
Inner Join Account (NOLOCK) a on a.AccountID = s.AccountID
Left Outer Join Account (NOLOCK) ap on ap.AccountID = a.ParentID
{2}
Inner Join #TempAccountHierarchy (NOLOCK) ah on ah.AccountID = a.AccountID
Inner Join AddressTrans (NOLOCK) at on at.AddressTransID = s.DeliveryAddressTransID

{8}

Outer Apply
(
	SELECT p2.PaymentID 
	FROM Payment (NOLOCK) p2 
	WHERE p2.SalesOrderID = s.SalesOrderID
	and isnull(p2.CreditCardTransactionLogID,0) != 0
) paym
Outer Apply
(
	SELECT SUM(sl2.Quantity * sl2.UnitPrice) LineTotal
	FROM SalesOrderLine (NOLOCK) sl2
	WHERE sl2.SalesOrderID = s.SalesOrderID
) line
Outer Apply
(
	SELECT STUFF((
		SELECT ', ' + CAST(f2.TrackingNumber as VARCHAR(MAX))
		FROM Fulfillment (NOLOCK) f2
		WHERE f2.SalesOrderID = s.SalesOrderID
		FOR XML PATH('')
    ), 1, 2, '') as TrackingNumber
) fulfill


Outer Apply
(
	SELECT TOP 1 cv2.Value as Department
	FROM CustomValue (NOLOCK) cv2
	Inner Join CustomField (NOLOCK) cf2 on cf2.CustomFieldID = cv2.CustomFieldID
	WHERE cv2.SalesOrderID = s.SalesOrderID
	and cf2.Name = 'Department'
) Dept

Outer Apply
(
	SELECT b2.QuantityBackOrdered as Quantity 
	FROM SalesOrderLineBackOrder b2
	WHERE b2.SalesOrderInternalID = s.NetSuiteInternalID 
	and b2.ItemInternalID = i.InternalID
) backorder

{13}

WHERE s.WebsiteID = {0}
{3}
{4}
{5}
{6}

{9}
ORDER BY s.SalesOrderID
"
                        , Database.HandleQuote(CurrentWebsite.WebsiteID)
                        , Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID)
                        //, CurrentUser.IsSuperAdmin ? string.Empty : string.Format("AND a.AccountID in (SELECT AccountID FROM UserAccount (NOLOCK) ua WHERE ua.UserWebsiteID = {0})", Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID))
                        , CurrentUser.IsSuperAdmin ? string.Empty : string.Format("Inner Join #TempAccounts (NOLOCK) ta on ta.AccountID = a.AccountID")
                        , string.IsNullOrEmpty(hfAccountID.Value) ? string.Empty : string.Format("AND a.AccountID = {0}", Database.HandleQuote(Convert.ToString(hfAccountID.Value)))
                        , string.IsNullOrEmpty(txtOrderStartDate.Text.Trim()) ? string.Empty : string.Format("AND s.CreatedOn >= {0}", Database.HandleQuote(txtOrderStartDate.Text.Trim()))
                        , string.IsNullOrEmpty(txtOrderEndDate.Text.Trim()) ? string.Empty : string.Format("AND s.CreatedOn < {0}", Database.HandleQuote(txtOrderEndDate.Text.Trim()))
                        , string.IsNullOrEmpty(txtOrderNumber.Text.Trim()) ? string.Empty : string.Format("AND CAST(s.SalesOrderID as varchar) = {0}", Database.HandleQuote(txtOrderNumber.Text.Trim()))
                        // Enterprise
                        , (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "53")
                            || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "20")
                                ? ", SUBSTRING(a.StoreNumber,1,2) as [group], a.StoreNumber as group_branch, enterprisestore.BranchAdminLgcyID as [regional_group]"
                                : String.Empty

                        , (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "53")
                            || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "20")
                                ? @"
Outer Apply
(
	SELECT Top 1 BranchAdminLgcyID
	FROM EnterpriseCustomer (NOLOCK) ec
	WHERE ec.StoreNumber = a.StoreNumber
	and ec.IsIndividual = 0
) enterprisestore
"
                                : String.Empty

                        , !chkPendingApproval.Checked ? string.Empty : string.Format("AND s.IsPendingApproval = 1")
                        , CustomField != null && !string.IsNullOrEmpty(CustomField.CustomFieldID) ? ", ISNULL(Dept.Department,'') as Department" : string.Empty

                        //BK
                        , (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "26")
                            || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "2")
                            ? ", CASE WHEN ap.ParentID is null THEN a.AccountName ELSE ap.AccountName END as [FOA]"
                                : String.Empty

                        //Mavis
                        , Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && (CurrentWebsite.WebsiteID == "25" || CurrentWebsite.WebsiteID == "26")
                            ? ", ISNULL(uw.EmployeeID, '') as EmployeeID, mavis.StoreNumber"
                                : String.Empty
                        , Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && (CurrentWebsite.WebsiteID == "25" || CurrentWebsite.WebsiteID == "26")
                            ? @"
Outer Apply
(
    SELECT TOP 1 c2.StoreNumber
    FROM MavisTireCustomer (NOLOCK) c2 
    WHERE c2.EmployeeNumber = uw.EmployeeID
) mavis
"
                                : String.Empty

                );

                DataSet = Database.GetDataSet(strSQL);

            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }

            return DataSet;
        }


        protected void ExportToExcel(DataTable dt, string FileName)
        {
            if (dt.Rows.Count > 0)
            {
                string filename = FileName + ".xls";
                System.IO.StringWriter tw = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dt;
                dgGrid.DataBind();

                //Get the HTML for the control.
                dgGrid.RenderControl(hw);
                //Write the HTML back to the browser.
                //Response.ContentType = application/vnd.ms-excel;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AppendHeader("Content-Disposition",
                                      "attachment; filename=" + filename + "");
                this.EnableViewState = false;
                Response.Write(tw.ToString());
                Response.End();
            }
        }

        protected DataSet GetReconcilationReportBySQL()
        {
            DataSet DataSet = new DataSet();
            string strSQL = string.Empty;

            try
            {
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

SELECT s.SalesOrderID as OrderNumber
	, CONVERT(VARCHAR(10), s.TransactionDate , 101) as TransactionDate
	, CASE WHEN ISNULL(paym.PaymentID,0) != 0 THEN 'IMAGE SOLU* SO' + CAST(s.SalesOrderID as varchar) ELSE '' END as TransactionID
	, creditcard.NickName as CreditCardNickName
	, ah.AccountNamePath
	, ap.AccountName as ParentAccount
	, a.AccountName as Account

    {10}

	, u.FirstName as AccountFirstName
	, u.LastName as AccountLastName
	, u.EmailAddress
	, at.FirstName as ShippingFirstName
	, at.LastName as ShippingLastName
	, at.AddressLine1 as Address1
	, at.AddressLine2 as Address2
	, at.City
	, at.State
	, at.PostalCode as Zip
	, at.CountryCode as Country
	, line.LineTotal as SubTotalAmount
	, s.ShippingAmount as TotalShippingAmount
	, s.TaxAmount + ISNULL(s.IPDDutiesAndTaxesAmount,0) as TotalTaxAmount

	, CCpaym.Amount as CreditCardPaymentAmount
	, Budgetpaym.Amount as BudgetPaymentAmount 

	, fulfill.TrackingNumber as TrackingNumber
    , '' as Memo

    {7}
    , PONum.PONumber
    , Promopaym.PromotionCode
    , Promopaym.Amount as PromoAmount

FROM SalesOrder (NOLOCK) s
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = s.UserInfoID
Inner Join Account (NOLOCK) a on a.AccountID = s.AccountID
Left Outer Join Account (NOLOCK) ap on ap.AccountID = a.ParentID

{2}
Inner Join #TempAccountHierarchy (NOLOCK) ah on ah.AccountID = a.AccountID
Inner Join AddressTrans (NOLOCK) at on at.AddressTransID = s.DeliveryAddressTransID
Outer Apply
(
	SELECT p2.PaymentID 
	FROM Payment (NOLOCK) p2 
	WHERE p2.SalesOrderID = s.SalesOrderID
	and isnull(p2.CreditCardTransactionLogID,0) != 0
) paym
Outer Apply
(
	SELECT SUM(sl2.Quantity * sl2.UnitPrice) LineTotal
	FROM SalesOrderLine (NOLOCK) sl2
	WHERE sl2.SalesOrderID = s.SalesOrderID
) line
Outer Apply
(
	SELECT STUFF((
        SELECT '| ' + CONVERT(VARCHAR(10), f2.ShipDate , 101) + ' - ' + CAST(REPLACE(f2.TrackingNumber, '<BR>',',') as VARCHAR(MAX))
		--SELECT ', ' + CAST(f2.TrackingNumber as VARCHAR(MAX))
		FROM Fulfillment (NOLOCK) f2
		WHERE f2.SalesOrderID = s.SalesOrderID
		FOR XML PATH('')
    ), 1, 2, '') as TrackingNumber
) fulfill
Outer Apply
(
	SELECT STUFF((
		SELECT ', ' + CAST(c2.Nickname as VARCHAR(MAX))
		FROM Payment (NOLOCK) p2
		Inner Join CreditCard (NOLOCK) c2 on c2.CreditCardID = p2.CreditCardID
		WHERE p2.SalesOrderID = s.SalesOrderID
		FOR XML PATH('')
    ), 1, 2, '') as NickName
) creditcard

Outer Apply
(

	SELECT SUM(p2.AmountPaid) Amount
	FROM Payment (NOLOCK) p2 
	WHERE p2.SalesOrderID = s.SalesOrderID
	and isnull(p2.CreditCardTransactionLogID,0) != 0
) CCpaym
Outer Apply
(

	SELECT SUM(p2.AmountPaid) Amount
	FROM Payment (NOLOCK) p2 
	WHERE p2.SalesOrderID = s.SalesOrderID
	and isnull(p2.BudgetAssignmentID,0) != 0
) Budgetpaym
Outer Apply
(

	SELECT TOP 1 p2.AmountPaid Amount, pr2.PromotionCode
	FROM Payment (NOLOCK) p2 
    Inner Join Promotion (NOLOCK) pr2 on pr2.PromotionID = p2.PromotionID
	WHERE p2.SalesOrderID = s.SalesOrderID
	and p2.PaymentSource = 3
) Promopaym

Outer Apply
(
	SELECT TOP 1 cv2.Value as PONumber
	FROM CustomValue (NOLOCK) cv2
	Inner Join CustomField (NOLOCK) cf2 on cf2.CustomFieldID = cv2.CustomFieldID
	WHERE cv2.SalesOrderID = s.SalesOrderID
    and cf2.Inactive = 0
	and cf2.Name = 'PO Number'
) PONum

{8}

WHERE s.WebsiteID = {0}
{3}
{4}
{5}
{6}

{9}

UNION 
 
SELECT cm.SalesOrderID 
	, CONVERT(VARCHAR(10), cm.TransactionDate , 101) as TransactionDate 
	, 'CREDIT MEMO ' + cm.InternalID as TransactionID
	, '' as CreditCardNickName 
	, ah.AccountNamePath
	, ap.AccountName as ParentAccount
	, a.AccountName as Account

    {10}

	, u.FirstName as AccountFirstName
	, u.LastName as AccountLastName
	, u.EmailAddress 
	, '' as ShippingFirstName
	, '' as ShippingLastName 
	, '' as Address1
	, '' as Address2
	, '' as City
	, '' as State
	, '' as Zip
	, '' as Country
	, cm.SubTotal *-1.0 as SubTotalAmount
	, cm.ShippingCost *-1.0 as TotalShippingAmount
	, cm.TotalTax *-1.0 as TotalTaxAmount

	, 0 as CreditCardPaymentAmount
	, 0 as BudgetPaymentAmount 

	, '' as TrackingNumber 
	, ISNULL(cm.Memo,'') as Memo 

    {7}

    ,'' as PONumber
    , '' as PromotionCode
    , 0 as PromoAmount
FROM CreditMemo (NOLOCK) cm 
Inner JOin SalesOrder (NOLOCK) s on s.SalesOrderID = cm.SalesOrderID 
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = s.UserInfoID
Inner Join Account (NOLOCK) a on a.AccountID = s.AccountID 
Left Outer Join Account (NOLOCK) ap on ap.AccountID = a.ParentID
{2} 
Inner Join #TempAccountHierarchy (NOLOCK) ah on ah.AccountID = a.AccountID 

{8}

WHERE s.WebsiteID = {0} 
{3} 
{4} 
{5} 
{6}

{9}

ORDER BY s.SalesOrderID
"
                        , Database.HandleQuote(CurrentWebsite.WebsiteID)
                        , Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID)
                        //, CurrentUser.IsSuperAdmin ? string.Empty : string.Format("AND a.AccountID in (SELECT AccountID FROM UserAccount (NOLOCK) ua WHERE ua.UserWebsiteID = {0})", Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID))
                        , CurrentUser.IsSuperAdmin ? string.Empty : string.Format("Inner Join #TempAccounts (NOLOCK) ta on ta.AccountID = a.AccountID")
                        , string.IsNullOrEmpty(hfAccountID.Value) ? string.Empty : string.Format("AND a.AccountID = {0}", Database.HandleQuote(Convert.ToString(hfAccountID.Value)))
                        , string.IsNullOrEmpty(txtOrderStartDate.Text.Trim()) ? string.Empty : string.Format("AND s.CreatedOn >= {0}", Database.HandleQuote(txtOrderStartDate.Text.Trim()))
                        , string.IsNullOrEmpty(txtOrderEndDate.Text.Trim()) ? string.Empty : string.Format("AND s.CreatedOn < {0}", Database.HandleQuote(txtOrderEndDate.Text.Trim()))
                        , string.IsNullOrEmpty(txtOrderNumber.Text.Trim()) ? string.Empty : string.Format("AND CAST(s.SalesOrderID as varchar) = {0}", Database.HandleQuote(txtOrderNumber.Text.Trim()))

                        , String.Empty
                        , String.Empty

                        , !chkPendingApproval.Checked ? string.Empty : string.Format("AND s.IsPendingApproval = 1")

                        //BK
                        , (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "26")
                            || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "2")
                            ? ", CASE WHEN ap.ParentID is null THEN a.AccountName ELSE ap.AccountName END as [FOA]"
                                : String.Empty
                ); 

                DataSet = Database.GetDataSet(strSQL);

            }
            catch (Exception ex)
            {
                throw ex;
                //WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }            

            return DataSet;
        }

        protected DataSet GetReconcilationReportBySQLMavis()
        {
            DataSet DataSet = new DataSet();
            string strSQL = string.Empty;

            try
            {
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

SELECT s.SalesOrderID as OrderNumber
	, CONVERT(VARCHAR(10), s.TransactionDate , 101) as TransactionDate
	, CASE WHEN ISNULL(paym.PaymentID,0) != 0 THEN 'IMAGE SOLU* SO' + CAST(s.SalesOrderID as varchar) ELSE '' END as TransactionID
	, creditcard.NickName as CreditCardNickName
	, ah.AccountNamePath
	, ap.AccountName as ParentAccount
	, a.AccountName as Account

    {10}

    , ISNULL(uw.EmployeeID,'') as EmployeeID

	, u.FirstName as AccountFirstName
	, u.LastName as AccountLastName
	, u.EmailAddress

    , at.AddressLabel as CompanyName
	, at.FirstName as ShippingFirstName
	, at.LastName as ShippingLastName
	, at.AddressLine1 as Address1
	, at.AddressLine2 as Address2
	, at.City
	, at.State
	, at.PostalCode as Zip
	, at.CountryCode as Country
	, line.LineTotal as SubTotalAmount
	, s.ShippingAmount as TotalShippingAmount
	, s.TaxAmount + ISNULL(s.IPDDutiesAndTaxesAmount,0) as TotalTaxAmount

	, CCpaym.Amount as CreditCardPaymentAmount
	, Budgetpaym.Amount as BudgetPaymentAmount 

	, fulfill.TrackingNumber as TrackingNumber
    , '' as Memo

    {7}
    , PONum.PONumber
    , Promopaym.PromotionCode
    , Promopaym.Amount as PromoAmount

    , CASE WHEN s.Status = 'Rejected' THEN 'Yes' ELSE 'No' END as IsRejected
    , CASE WHEN s.ExcludeOptional = 1 THEN 'Yes' ELSE 'No' END as ExcludePants

FROM SalesOrder (NOLOCK) s
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = s.UserInfoID
Inner Join UserWebsite (NOLOCK) uw on uw.UserwebsiteID = s.UserwebsiteID
Inner Join Account (NOLOCK) a on a.AccountID = s.AccountID
Left Outer Join Account (NOLOCK) ap on ap.AccountID = a.ParentID

{2}

Inner Join #TempAccountHierarchy (NOLOCK) ah on ah.AccountID = a.AccountID
Inner Join AddressTrans (NOLOCK) at on at.AddressTransID = s.DeliveryAddressTransID
Outer Apply
(
	SELECT p2.PaymentID 
	FROM Payment (NOLOCK) p2 
	WHERE p2.SalesOrderID = s.SalesOrderID
	and isnull(p2.CreditCardTransactionLogID,0) != 0
) paym
Outer Apply
(
	SELECT SUM(sl2.Quantity * sl2.UnitPrice) LineTotal
	FROM SalesOrderLine (NOLOCK) sl2
	WHERE sl2.SalesOrderID = s.SalesOrderID
) line
Outer Apply
(
	SELECT STUFF((
        SELECT '| ' + CONVERT(VARCHAR(10), f2.ShipDate , 101) + ' - ' + CAST(REPLACE(f2.TrackingNumber, '<BR>',',') as VARCHAR(MAX))
		--SELECT ', ' + CAST(f2.TrackingNumber as VARCHAR(MAX))
		FROM Fulfillment (NOLOCK) f2
		WHERE f2.SalesOrderID = s.SalesOrderID
		FOR XML PATH('')
    ), 1, 2, '') as TrackingNumber
) fulfill
Outer Apply
(
	SELECT STUFF((
		SELECT ', ' + CAST(c2.Nickname as VARCHAR(MAX))
		FROM Payment (NOLOCK) p2
		Inner Join CreditCard (NOLOCK) c2 on c2.CreditCardID = p2.CreditCardID
		WHERE p2.SalesOrderID = s.SalesOrderID
		FOR XML PATH('')
    ), 1, 2, '') as NickName
) creditcard

Outer Apply
(

	SELECT SUM(p2.AmountPaid) Amount
	FROM Payment (NOLOCK) p2 
	WHERE p2.SalesOrderID = s.SalesOrderID
	and isnull(p2.CreditCardTransactionLogID,0) != 0
) CCpaym
Outer Apply
(

	SELECT SUM(p2.AmountPaid) Amount
	FROM Payment (NOLOCK) p2 
	WHERE p2.SalesOrderID = s.SalesOrderID
	and isnull(p2.BudgetAssignmentID,0) != 0
) Budgetpaym
Outer Apply
(

	SELECT TOP 1 p2.AmountPaid Amount, pr2.PromotionCode
	FROM Payment (NOLOCK) p2 
    Inner Join Promotion (NOLOCK) pr2 on pr2.PromotionID = p2.PromotionID
	WHERE p2.SalesOrderID = s.SalesOrderID
	and p2.PaymentSource = 3
) Promopaym

Outer Apply
(
	SELECT TOP 1 cv2.Value as PONumber
	FROM CustomValue (NOLOCK) cv2
	Inner Join CustomField (NOLOCK) cf2 on cf2.CustomFieldID = cv2.CustomFieldID
	WHERE cv2.SalesOrderID = s.SalesOrderID
    and cf2.Inactive = 0
	and cf2.Name = 'PO Number'
) PONum

{8}

WHERE s.WebsiteID = {0}
{3}
{4}
{5}
{6}

{9}

UNION 
 
SELECT cm.SalesOrderID 
	, CONVERT(VARCHAR(10), cm.TransactionDate , 101) as TransactionDate 
	, 'CREDIT MEMO ' + cm.InternalID as TransactionID
	, '' as CreditCardNickName 
	, ah.AccountNamePath
	, ap.AccountName as ParentAccount
	, a.AccountName as Account

    {10}

    , ISNULL(uw.EmployeeID,'') as EmployeeID

	, u.FirstName as AccountFirstName
	, u.LastName as AccountLastName
	, u.EmailAddress 

    , '' as CompanyName
	, '' as ShippingFirstName
	, '' as ShippingLastName 
	, '' as Address1
	, '' as Address2
	, '' as City
	, '' as State
	, '' as Zip
	, '' as Country
	, cm.SubTotal *-1.0 as SubTotalAmount
	, cm.ShippingCost *-1.0 as TotalShippingAmount
	, cm.TotalTax *-1.0 as TotalTaxAmount

	, 0 as CreditCardPaymentAmount
	, 0 as BudgetPaymentAmount 

	, '' as TrackingNumber 
	, ISNULL(cm.Memo,'') as Memo 

    {7}

    ,'' as PONumber
    , '' as PromotionCode
    , 0 as PromoAmount
    , '' as IsRejected
    , '' as ExcludePants
FROM CreditMemo (NOLOCK) cm 
Inner JOin SalesOrder (NOLOCK) s on s.SalesOrderID = cm.SalesOrderID 
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = s.UserInfoID
Inner Join UserWebsite (NOLOCK) uw on uw.UserWebsiteID = s.UserWebsiteID
Inner Join Account (NOLOCK) a on a.AccountID = s.AccountID 
Left Outer Join Account (NOLOCK) ap on ap.AccountID = a.ParentID
{2} 
Inner Join #TempAccountHierarchy (NOLOCK) ah on ah.AccountID = a.AccountID 

{8}

WHERE s.WebsiteID = {0} 
{3} 
{4} 
{5} 
{6}

{9}

ORDER BY s.SalesOrderID
"
                        , Database.HandleQuote(CurrentWebsite.WebsiteID)
                        , Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID)
                        //, CurrentUser.IsSuperAdmin ? string.Empty : string.Format("AND a.AccountID in (SELECT AccountID FROM UserAccount (NOLOCK) ua WHERE ua.UserWebsiteID = {0})", Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID))
                        , CurrentUser.IsSuperAdmin ? string.Empty : string.Format("Inner Join #TempAccounts (NOLOCK) ta on ta.AccountID = a.AccountID")
                        , string.IsNullOrEmpty(hfAccountID.Value) ? string.Empty : string.Format("AND a.AccountID = {0}", Database.HandleQuote(Convert.ToString(hfAccountID.Value)))
                        , string.IsNullOrEmpty(txtOrderStartDate.Text.Trim()) ? string.Empty : string.Format("AND s.CreatedOn >= {0}", Database.HandleQuote(txtOrderStartDate.Text.Trim()))
                        , string.IsNullOrEmpty(txtOrderEndDate.Text.Trim()) ? string.Empty : string.Format("AND s.CreatedOn < {0}", Database.HandleQuote(txtOrderEndDate.Text.Trim()))
                        , string.IsNullOrEmpty(txtOrderNumber.Text.Trim()) ? string.Empty : string.Format("AND CAST(s.SalesOrderID as varchar) = {0}", Database.HandleQuote(txtOrderNumber.Text.Trim()))

                        , string.Format(@", mavis.StoreNumber")
                        , string.Format(@"
Outer Apply
(
    SELECT TOP 1 c2.StoreNumber
    FROM MavisTireCustomer (NOLOCK) c2 
    WHERE c2.EmployeeNumber = uw.EmployeeID
) mavis
")

                        , !chkPendingApproval.Checked ? string.Empty : string.Format("AND s.IsPendingApproval = 1")

                        //BK
                        , (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "26")
                            || (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "2")
                            ? ", CASE WHEN ap.ParentID is null THEN a.AccountName ELSE ap.AccountName END as [FOA]"
                                : String.Empty
                );

                DataSet = Database.GetDataSet(strSQL);

            }
            catch (Exception ex)
            {
                throw ex;
                //WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }

            return DataSet;
        }

        protected DataSet GetReconcilationReportBySQLEnterprise()
        {
            DataSet DataSet = new DataSet();
            string strSQL = string.Empty;

            try
            {
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

SELECT s.SalesOrderID as OrderNumber
	, CONVERT(VARCHAR(10), s.TransactionDate , 101) as TransactionDate
	, CASE WHEN ISNULL(paym.PaymentID,0) != 0 THEN 'IMAGE SOLU* SO' + CAST(s.SalesOrderID as varchar) ELSE '' END as TransactionID
	, creditcard.NickName as CreditCardNickName
	, a.AccountName
	, u.FirstName as AccountFirstName
	, u.LastName as AccountLastName
	, u.EmailAddress
	, at.FirstName as ShippingFirstName
	, at.LastName as ShippingLastName
	, at.AddressLine1 as Address1
	, at.AddressLine2 as Address2
	, at.City
	, at.State
	, at.PostalCode as Zip
	, at.CountryCode as Country
	, line.LineTotal as SubTotalAmount
	, s.ShippingAmount as TotalShippingAmount
	, s.TaxAmount + ISNULL(s.IPDDutiesAndTaxesAmount,0) as TotalTaxAmount

	, CCpaym.Amount as CreditCardPaymentAmount
	, Budgetpaym.Amount as BudgetPaymentAmount 

	, fulfill.TrackingNumber as TrackingNumber
    , '' as Memo

    {7}
    , enterpriseinvoice.Amount as [InvoicedAmount]

FROM SalesOrder (NOLOCK) s
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = s.UserInfoID
Inner Join Account (NOLOCK) a on a.AccountID = s.AccountID
{2}
Inner Join AddressTrans (NOLOCK) at on at.AddressTransID = s.DeliveryAddressTransID
Outer Apply
(
	SELECT p2.PaymentID 
	FROM Payment (NOLOCK) p2 
	WHERE p2.SalesOrderID = s.SalesOrderID
	and isnull(p2.CreditCardTransactionLogID,0) != 0
) paym
Outer Apply
(
	SELECT SUM(sl2.Quantity * sl2.UnitPrice) LineTotal
	FROM SalesOrderLine (NOLOCK) sl2
	WHERE sl2.SalesOrderID = s.SalesOrderID
) line
Outer Apply
(
	SELECT STUFF((
        SELECT '| ' + CONVERT(VARCHAR(10), f2.ShipDate , 101) + ' - ' + CAST(REPLACE(f2.TrackingNumber, '<BR>',',') as VARCHAR(MAX))
		--SELECT ', ' + CAST(f2.TrackingNumber as VARCHAR(MAX))
		FROM Fulfillment (NOLOCK) f2
		WHERE f2.SalesOrderID = s.SalesOrderID
		FOR XML PATH('')
    ), 1, 2, '') as TrackingNumber
) fulfill
Outer Apply
(
	SELECT STUFF((
		SELECT ', ' + CAST(c2.Nickname as VARCHAR(MAX))
		FROM Payment (NOLOCK) p2
		Inner Join CreditCard (NOLOCK) c2 on c2.CreditCardID = p2.CreditCardID
		WHERE p2.SalesOrderID = s.SalesOrderID
		FOR XML PATH('')
    ), 1, 2, '') as NickName
) creditcard

Outer Apply
(

	SELECT SUM(p2.AmountPaid) Amount
	FROM Payment (NOLOCK) p2 
	WHERE p2.SalesOrderID = s.SalesOrderID
	and isnull(p2.CreditCardTransactionLogID,0) != 0
) CCpaym
Outer Apply
(

	SELECT SUM(p2.AmountPaid) Amount
	FROM Payment (NOLOCK) p2 
	WHERE p2.SalesOrderID = s.SalesOrderID
	and isnull(p2.BudgetAssignmentID,0) != 0
) Budgetpaym

{8}

WHERE s.WebsiteID = {0}
{3}
{4}
{5}
{6}

{9}

UNION 
 
SELECT cm.SalesOrderID 
	, CONVERT(VARCHAR(10), cm.TransactionDate , 101) as TransactionDate 
	, 'CREDIT MEMO ' + cm.InternalID as TransactionID
	, '' as CreditCardNickName 
	, a.AccountName
	, u.FirstName as AccountFirstName
	, u.LastName as AccountLastName
	, u.EmailAddress 
	, '' as ShippingFirstName
	, '' as ShippingLastName 
	, '' as Address1
	, '' as Address2
	, '' as City
	, '' as State
	, '' as Zip
	, '' as Country
	, cm.SubTotal *-1.0 as SubTotalAmount
	, cm.ShippingCost *-1.0 as TotalShippingAmount
	, cm.TotalTax *-1.0 as TotalTaxAmount

	, 0 as CreditCardPaymentAmount
	, 0 as BudgetPaymentAmount 

	, '' as TrackingNumber 
	, ISNULL(cm.Memo,'') as Memo 

    {7}
    , 0 as [InvoicedAmount]
FROM CreditMemo (NOLOCK) cm 
Inner JOin SalesOrder (NOLOCK) s on s.SalesOrderID = cm.SalesOrderID 
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = s.UserInfoID
Inner Join Account (NOLOCK) a on a.AccountID = s.AccountID 
{2} 

{8}

WHERE s.WebsiteID = {0} 
{3} 
{4} 
{5} 
{6}

{9}

ORDER BY s.SalesOrderID
"
                        , Database.HandleQuote(CurrentWebsite.WebsiteID)
                        , Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID)
                        //, CurrentUser.IsSuperAdmin ? string.Empty : string.Format("AND a.AccountID in (SELECT AccountID FROM UserAccount (NOLOCK) ua WHERE ua.UserWebsiteID = {0})", Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID))
                        , CurrentUser.IsSuperAdmin ? string.Empty : string.Format("Inner Join #TempAccounts (NOLOCK) ta on ta.AccountID = a.AccountID")
                        , string.IsNullOrEmpty(hfAccountID.Value) ? string.Empty : string.Format("AND a.AccountID = {0}", Database.HandleQuote(Convert.ToString(hfAccountID.Value)))
                        , string.IsNullOrEmpty(txtOrderStartDate.Text.Trim()) ? string.Empty : string.Format("AND s.CreatedOn >= {0}", Database.HandleQuote(txtOrderStartDate.Text.Trim()))
                        , string.IsNullOrEmpty(txtOrderEndDate.Text.Trim()) ? string.Empty : string.Format("AND s.CreatedOn < {0}", Database.HandleQuote(txtOrderEndDate.Text.Trim()))
                        , string.IsNullOrEmpty(txtOrderNumber.Text.Trim()) ? string.Empty : string.Format("AND CAST(s.SalesOrderID as varchar) = {0}", Database.HandleQuote(txtOrderNumber.Text.Trim()))

                        , "	, SUBSTRING(a.StoreNumber,1,2) as [group], a.StoreNumber as group_branch, enterprisestore.BranchAdminLgcyID as [regional_group], enterprisestore.AirportCode as [Airport Code], enterprise.JobProfile as [Job]"

                        , @"
Outer Apply
(
	SELECT Top 1 j2.JobProfile
	FROM EnterpriseCustomer (NOLOCK) ec2
	Inner Join EnterpriseEBAJob (NOLOCK) j2 on j2.JobCode = ec2.Job
    Inner Join UserWebsite (NOLOCK) uw2 on uw2.EmployeeID = ec2.EmployeeID and uw2.WebsiteID = s.WebsiteID
	WHERE ec2.IsIndividual = 1
    and uw2.UserInfoID = u.UserInfoID
) enterprise
Outer Apply
(
	SELECT Top 1 BranchAdminLgcyID, AirportCode
	FROM EnterpriseCustomer (NOLOCK) ec
	WHERE ec.StoreNumber = a.StoreNumber
	and ec.IsIndividual = 0
) enterprisestore
Outer Apply
(
	SELECT SUM(ei.TotalAmount) Amount
	FROM EnterpriseInvoice (NOLOCK) ei
	WHERE ei.SalesOrderInternalID = s.NetSuiteInternalID
) enterpriseinvoice
"

                        , !chkPendingApproval.Checked ? string.Empty : string.Format("AND s.IsPendingApproval = 1")


                );

                DataSet = Database.GetDataSet(strSQL);

            }
            catch (Exception ex)
            {
                throw ex;
                //WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }

            return DataSet;
        }

        protected void lbnDownloadReconciliation_Click(object sender, EventArgs e)
        {
            try
            {
                if (
                    (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && CurrentWebsite.WebsiteID == "53")
                    ||
                    (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "20")
                )
                {
                    DataSet DataSet = GetReconcilationReportBySQLEnterprise();
                    string strFileName = string.Format("reconciliation_export{0}", DateTime.UtcNow.ToString("yyyyMMddHHmmssffff"));
                    ExportToExcel(DataSet.Tables[0], strFileName);
                }
                else if (
                    (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "staging" && CurrentWebsite.WebsiteID == "58")
                    ||
                    (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "25")
                    ||
                    (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production" && CurrentWebsite.WebsiteID == "26")
                )
                {
                    DataSet DataSet = GetReconcilationReportBySQLMavis();
                    string strFileName = string.Format("reconciliation_export{0}", DateTime.UtcNow.ToString("yyyyMMddHHmmssffff"));
                    ExportToExcel(DataSet.Tables[0], strFileName);
                }
                else
                {
                    DataSet DataSet = GetReconcilationReportBySQL();
                    string strFileName = string.Format("reconciliation_export{0}", DateTime.UtcNow.ToString("yyyyMMddHHmmssffff"));
                    ExportToExcel(DataSet.Tables[0], strFileName);
                }
            }
            catch(Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }

            //System.Data.OleDb.OleDbConnection OleDbConnection = null;
            //System.Data.OleDb.OleDbCommand OleDbCommand = null;
            //string strSQL = string.Empty;
            //Hashtable dicParam = null;
            //try
            //{
            //    if (SalesOrders == null || SalesOrders.Count == 0) throw new Exception("No results found");

            //    string strPath = Server.MapPath("\\Export\\ReconciliationReport\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");
            //    if (!Directory.Exists(strPath))
            //    {
            //        Directory.CreateDirectory(strPath);
            //    }

            //    string strTemplateFileName = "ReconciliationTemplate.xlsx";
            //    string strTemplateFilePath = Server.MapPath(string.Format("\\Export\\Template\\{0}", strTemplateFileName));
            //    string strFileExportPath = Server.MapPath(string.Format("\\Export\\ReconciliationReport\\{0}\\reconciliation_export{1}.xlsx", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")));
            //    File.Copy(strTemplateFilePath, strFileExportPath, true);

            //    OleDbConnection = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + strFileExportPath + "';Extended Properties=Excel 12.0;");
            //    OleDbConnection.Open();
            //    OleDbCommand = new System.Data.OleDb.OleDbCommand();
            //    OleDbCommand.Connection = OleDbConnection;

            //    foreach (ImageSolutions.SalesOrder.SalesOrder objSalesOrder in SalesOrders.OrderBy(x => Convert.ToInt32(x.SalesOrderID)).ToList())
            //    {
            //        dicParam = new Hashtable();
            //        dicParam["OrderNumber"] = objSalesOrder.SalesOrderID;
            //        dicParam["TransactionDate"] = objSalesOrder.TransactionDate.Value.ToShortDateString();

            //        if (objSalesOrder.Payments != null && objSalesOrder.Payments.FindAll(m => m.CreditCardTransactionLog != null).Count > 0)
            //        {
            //            dicParam["TransactionID"] = string.Format(@"IMAGE SOLU* SO{0}", objSalesOrder.SalesOrderID);
            //        }
            //        else
            //        {
            //            dicParam["TransactionID"] = string.Empty;
            //        }

            //        dicParam["Account"] = objSalesOrder.Account.AccountNamePath;
            //        dicParam["FirstName"] = objSalesOrder.UserInfo.FirstName;
            //        dicParam["LastName"] = objSalesOrder.UserInfo.LastName;
            //        dicParam["EmailAddress"] = objSalesOrder.UserInfo.EmailAddress;

            //        if (objSalesOrder.DeliveryAddress != null)
            //        {
            //            dicParam["Address1"] = objSalesOrder.DeliveryAddress.AddressLine1;
            //            dicParam["Address2"] = objSalesOrder.DeliveryAddress.AddressLine2;
            //            dicParam["City"] = objSalesOrder.DeliveryAddress.City;
            //            dicParam["State"] = objSalesOrder.DeliveryAddress.State;
            //            dicParam["Zip"] = objSalesOrder.DeliveryAddress.PostalCode;
            //            dicParam["Country"] = objSalesOrder.DeliveryAddress.CountryCode;
            //        }
            //        else
            //        {
            //            dicParam["Address1"] = string.Empty;
            //            dicParam["Address2"] = string.Empty;
            //            dicParam["City"] = string.Empty;
            //            dicParam["State"] = string.Empty;
            //            dicParam["Zip"] = string.Empty;
            //            dicParam["Country"] = string.Empty;
            //        }

            //        dicParam["SubTotalAmount"] = objSalesOrder.LineTotal.ToString("0.00");
            //        dicParam["TotalShippingAmount"] = objSalesOrder.ShippingAmount.ToString("0.00");
            //        dicParam["TotalTaxAmount"] = objSalesOrder.TaxAmount.ToString("0.00");
            //        dicParam["TotalOrderAmount"] = objSalesOrder.Total.ToString("0.00");

            //        string strTrackingNumber = string.Empty;
            //        foreach (ImageSolutions.Fulfillment.Fulfillment _Fulfillment in objSalesOrder.Fulfillments)
            //        {
            //            strTrackingNumber += string.IsNullOrEmpty(strTrackingNumber) ? _Fulfillment.TrackingNumber : string.Format(",{0}", _Fulfillment.TrackingNumber);
            //        }
            //        dicParam["TrackingNumber"] = strTrackingNumber;

            //        strSQL = Database.GetInsertSQL(dicParam, "[reconciliation$]", false);

            //        OleDbCommand.CommandText = strSQL;
            //        OleDbCommand.ExecuteNonQuery();
            //    }

            //    OleDbConnection.Close();


            //    Response.ContentType = "application/vnd.ms-excel";
            //    Response.AppendHeader("Content-Disposition", "attachment; filename=" + System.IO.Path.GetFileName(strFileExportPath));
            //    Response.WriteFile(strFileExportPath);
            //    Response.Flush();
            //    Response.End();
            //}
            //catch (Exception ex)
            //{
            //    WebUtility.DisplayJavascriptMessage(this, ex.Message);
            //}


            ////http://www.dotnetspeaks.com/DisplayArticle.aspx?ID=97
            //try
            //{
            //    if (SalesOrders == null || SalesOrders.Count == 0) throw new Exception("No results found");

            //    StringBuilder objCSV = null;

            //    string strPath = Server.MapPath("\\Export\\ReconciliationReport\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");
            //    if (!Directory.Exists(strPath))
            //    {
            //        Directory.CreateDirectory(strPath);
            //    }
            //    string strFileExportPath = Server.MapPath(string.Format("\\Export\\ReconciliationReport\\{0}\\reconciliation_export{1}.csv", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")));

            //    try
            //    {
            //        objCSV = new StringBuilder();
            //        objCSV.Append("OrderNumber,TransactionDate,TransactionID,Account,FirstName,LastName,Email,Address1,Address2,City,State,Zip,Country,SubTotalAmount,TotalShippingAmount,TotalTaxAmount,TotalOrderAmount,TrackingNumber");
            //        objCSV.AppendLine();

            //        foreach (ImageSolutions.SalesOrder.SalesOrder objSalesOrder in SalesOrders.OrderBy(x => Convert.ToInt32(x.SalesOrderID)).ToList())
            //        {
            //            objCSV.Append(objSalesOrder.SalesOrderID.Replace(",", "")).Append(",");
            //            objCSV.Append(objSalesOrder.TransactionDate.Value.ToShortDateString()).Append(",");

            //            if (objSalesOrder.Payments != null && objSalesOrder.Payments.FindAll(m => m.CreditCardTransactionLog != null).Count > 0)
            //            {
            //                //objCSV.Append(objSalesOrder.Payments.Find(m => m.CreditCardTransactionLog != null).CreditCardTransactionLog.TransactionID.Replace(",", "")).Append(",");
            //                objCSV.Append("IMAGE SOLU* SO").Append(objSalesOrder.SalesOrderID.Replace(",", "")).Append(",");
            //                //objCSV.Append(objSalesOrder.Payments.Find(m => m.CreditCardTransactionLog != null).CreditCardTransactionLog.TransactionID.Replace(",", "")).Append(",");
            //            }
            //            else
            //            {
            //                objCSV.Append(",");
            //            }
            //            objCSV.Append(objSalesOrder.Account.AccountNamePath.Replace(",", "")).Append(",");
            //            objCSV.Append(objSalesOrder.UserInfo.FirstName.Replace(",", "")).Append(",");
            //            objCSV.Append(objSalesOrder.UserInfo.LastName.Replace(",", "")).Append(",");
            //            objCSV.Append(objSalesOrder.UserInfo.EmailAddress.Replace(",", "")).Append(",");
            //            if(objSalesOrder.DeliveryAddress != null)
            //            {
            //                objCSV.Append(objSalesOrder.DeliveryAddress.AddressLine1.Replace(",", "")).Append(",");
            //                objCSV.Append(objSalesOrder.DeliveryAddress.AddressLine2.Replace(",", "")).Append(",");
            //                objCSV.Append(objSalesOrder.DeliveryAddress.City.Replace(",", "")).Append(",");
            //                objCSV.Append(objSalesOrder.DeliveryAddress.State.Replace(",", "")).Append(",");
            //                objCSV.Append(objSalesOrder.DeliveryAddress.PostalCode.Replace(",", "")).Append(",");
            //                objCSV.Append(objSalesOrder.DeliveryAddress.CountryCode.Replace(",", "")).Append(",");
            //            }
            //            else
            //            {
            //                objCSV.Append(",");
            //                objCSV.Append(",");
            //                objCSV.Append(",");
            //                objCSV.Append(",");
            //                objCSV.Append(",");
            //                objCSV.Append(",");
            //            }

            //            objCSV.Append(objSalesOrder.LineTotal.ToString("0.00")).Append(",");
            //            objCSV.Append(objSalesOrder.ShippingAmount.ToString("0.00")).Append(",");
            //            objCSV.Append(objSalesOrder.TaxAmount.ToString("0.00")).Append(",");
            //            objCSV.Append(objSalesOrder.Total.ToString("0.00")).Append(",");

            //            string strTrackingNumber = string.Empty;
            //            foreach(ImageSolutions.Fulfillment.Fulfillment _Fulfillment in objSalesOrder.Fulfillments)
            //            {
            //                strTrackingNumber += string.IsNullOrEmpty(strTrackingNumber) ? _Fulfillment.TrackingNumber : string.Format(",{0}", _Fulfillment.TrackingNumber);
            //            }
            //            objCSV.Append(strTrackingNumber).Append(",");

            //            objCSV.AppendLine();
            //        }

            //        if (objCSV != null)
            //        {
            //            using (StreamWriter objWriter = new StreamWriter(strFileExportPath))
            //            {
            //                objWriter.Write(objCSV.ToString());
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        throw ex;
            //    }
            //    finally
            //    {
            //        objCSV = null;
            //    }

            //    Response.ContentType = "text/csv";
            //    //Response.ContentType = "application/vnd.ms-excel";
            //    Response.AppendHeader("Content-Disposition", "attachment; filename=" + System.IO.Path.GetFileName(strFileExportPath));
            //    Response.WriteFile(strFileExportPath);

            //    HttpContext.Current.Response.Flush();
            //    HttpContext.Current.Response.SuppressContent = true;
            //    HttpContext.Current.ApplicationInstance.CompleteRequest();
            //}
            //catch (Exception ex)
            //{
            //    WebUtility.DisplayJavascriptMessage(this, ex.Message);
            //}
        }

        protected DataSet GetItemReportBySQL()
        {
            DataSet DataSet = new DataSet();
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"

IF OBJECT_ID(N'tempdb..#TempAccounts') IS NOT NULL
BEGIN
DROP TABLE #TempAccounts
END
;

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


SELECT i.ItemName
	, i.ItemNumber
	--, i.ItemNumber + ' (' + i.StoreDisplayName + ')' as ItemDescription
	, REPLACE(i.SalesDescription, char(160), ' ') as ItemDescription
	, SUM(sl.Quantity) as Quantity
	, SUM(sl.Quantity * sl.UnitPrice) as Total
FROM SalesOrder (NOLOCK) s
Inner Join SalesOrderLine (NOLOCK) sl on sl.SalesOrderID = s.SalesOrderID
Inner Join Item (NOLOCK) i on i.ItemID = sl.ItemID
Inner Join Account (NOLOCK) a on a.AccountID = s.AccountID
{2}
WHERE s.WebsiteID = {0}
{3}
{4}
{5}
{6}

{7}

GROUP BY i.ItemName
	, i.ItemNumber
	--, i.ItemNumber + ' (' + i.StoreDisplayName + ')'
    , REPLACE(i.SalesDescription, char(160), ' ') 
"
                        , Database.HandleQuote(CurrentWebsite.WebsiteID)
                        , Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID)
                        //, CurrentUser.IsSuperAdmin ? string.Empty : string.Format("AND a.AccountID in (SELECT AccountID FROM UserAccount (NOLOCK) ua WHERE ua.UserWebsiteID = {0})", Database.HandleQuote(CurrentUser.CurrentUserWebSite.UserWebsiteID))
                        , CurrentUser.IsSuperAdmin ? string.Empty : string.Format("Inner Join #TempAccounts (NOLOCK) ta on ta.AccountID = a.AccountID")
                        , string.IsNullOrEmpty(hfAccountID.Value) ? string.Empty : string.Format("AND a.AccountID = {0}", Database.HandleQuote(Convert.ToString(hfAccountID.Value)))
                        , string.IsNullOrEmpty(txtOrderStartDate.Text.Trim()) ? string.Empty : string.Format("AND s.CreatedOn >= {0}", Database.HandleQuote(txtOrderStartDate.Text.Trim()))
                        , string.IsNullOrEmpty(txtOrderEndDate.Text.Trim()) ? string.Empty : string.Format("AND s.CreatedOn < {0}", Database.HandleQuote(txtOrderEndDate.Text.Trim()))

                        , string.IsNullOrEmpty(txtOrderNumber.Text.Trim()) ? string.Empty : string.Format("AND CAST(s.SalesOrderID as varchar) = {0}", Database.HandleQuote(txtOrderNumber.Text.Trim()))

                        , !chkPendingApproval.Checked ? string.Empty : string.Format("AND s.IsPendingApproval = 1")
                );

                DataSet = Database.GetDataSet(strSQL);

            }
            catch (Exception ex)
            {
                throw ex;
                //WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }

            return DataSet;
        }




        protected void lbnDownloadItem_Click(object sender, EventArgs e)
        {

            try
            {
                DataSet DataSet = GetItemReportBySQL();
                string strFileName = string.Format("sales_by_item_export{0}", DateTime.UtcNow.ToString("yyyyMMddHHmmssffff"));
                ExportToExcel(DataSet.Tables[0], strFileName);
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }


            ////http://www.dotnetspeaks.com/DisplayArticle.aspx?ID=97
            //List<SalesByItem> objSalesByItems = new List<SalesByItem>();

            //try
            //{
            //    if (SalesOrders == null || SalesOrders.Count == 0) throw new Exception("No results found");

            //    foreach (ImageSolutions.SalesOrder.SalesOrder objSalesOrder in SalesOrders)
            //    {
            //        foreach (ImageSolutions.SalesOrder.SalesOrderLine objSalesOrderLine in objSalesOrder.SalesOrderLines)
            //        {
            //            SalesByItem objSalesByItem = objSalesByItems.Find(m => m.ItemNumber == objSalesOrderLine.Item.ItemNumber);
            //            if (objSalesByItem == null)
            //            {
            //                objSalesByItem = new SalesByItem();
            //                objSalesByItem.ItemNumber = objSalesOrderLine.Item.ItemNumber;
            //                objSalesByItem.ItemName = objSalesOrderLine.Item.ItemName;
            //                objSalesByItem.ItemDescription = objSalesOrderLine.Item.Description;
            //                objSalesByItems.Add(objSalesByItem);
            //            }
            //            objSalesByItem.Quantity += objSalesOrderLine.Quantity;
            //            objSalesByItem.Total += objSalesOrderLine.LineSubTotal.Value;
            //        }
            //    }

            //    StringBuilder objCSV = null;

            //    string strPath = Server.MapPath("\\Export\\OrderReport\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");
            //    if (!Directory.Exists(strPath))
            //    {
            //        Directory.CreateDirectory(strPath);
            //    }
            //    string strFileExportPath = Server.MapPath(string.Format("\\Export\\OrderReport\\{0}\\sales_by_item_export{1}.csv", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")));

            //    try
            //    {
            //        objCSV = new StringBuilder();
            //        objCSV.Append("ItemName,ItemNumber,ItemDescription,Quantity,Total");
            //        objCSV.AppendLine();

            //        foreach (SalesByItem objSalesByItem in objSalesByItems)
            //        {
            //            objCSV.Append(objSalesByItem.ItemName.Replace(",", "")).Append(",");
            //            objCSV.Append(objSalesByItem.ItemNumber.Replace(",", "")).Append(",");
            //            objCSV.Append(objSalesByItem.ItemDescription.Replace(",", "")).Append(",");
            //            objCSV.Append(objSalesByItem.Quantity).Append(",");
            //            objCSV.Append(objSalesByItem.Total.ToString("0.00")).Append(",");
            //            objCSV.AppendLine();
            //        }

            //        if (objCSV != null)
            //        {
            //            using (StreamWriter objWriter = new StreamWriter(strFileExportPath))
            //            {
            //                objWriter.Write(objCSV.ToString());
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        throw ex;
            //    }
            //    finally
            //    {
            //        objCSV = null;
            //    }

            //    Response.ContentType = "text/csv";
            //    //Response.ContentType = "application/vnd.ms-excel";
            //    Response.AppendHeader("Content-Disposition", "attachment; filename=" + System.IO.Path.GetFileName(strFileExportPath));
            //    Response.WriteFile(strFileExportPath);

            //    HttpContext.Current.Response.Flush();
            //    HttpContext.Current.Response.SuppressContent = true;
            //    HttpContext.Current.ApplicationInstance.CompleteRequest();


            //    ////Excel
            //    //DataTable dt = new DataTable();

            //    //dt.Columns.Add("ItemName");
            //    //dt.Columns.Add("ItemNumber");
            //    //dt.Columns.Add("ItemDescription");
            //    //dt.Columns.Add("Quantity");
            //    //dt.Columns.Add("Total");


            //    //foreach (SalesByItem objSalesByItem in objSalesByItems)
            //    //{
            //    //    DataRow objRow = dt.NewRow();
            //    //    objRow["ItemName"] = objSalesByItem.ItemName;
            //    //    objRow["ItemNumber"] = objSalesByItem.ItemNumber;
            //    //    objRow["ItemDescription"] = objSalesByItem.ItemDescription;
            //    //    objRow["Quantity"] = objSalesByItem.Quantity;
            //    //    objRow["Total"] = objSalesByItem.Total;
            //    //    dt.Rows.Add(objRow);
            //    //}

            //    ////Double dimensional array to keep style name and style
            //    //string[,] styles = { 
            //    //               { "text", "\\@;" },
            //    //               { "text", "\\@;" },
            //    //               { "text", "\\@;" },
            //    //               { "number", "0\\.00;" },
            //    //               { "number", "0\\.00;" } };

            //    ////Dummy GridView to hold data to be exported in excel
            //    //System.Web.UI.WebControls.GridView gvExport = new System.Web.UI.WebControls.GridView();
            //    //gvExport.AllowPaging = false;
            //    //gvExport.DataSource = dt;
            //    //gvExport.DataBind();

            //    //StringWriter sw = new StringWriter();
            //    //HtmlTextWriter hw = new HtmlTextWriter(sw);
            //    //int cnt = styles.Length / 2;

            //    //for (int i = 0; i < gvExport.Rows.Count; i++)
            //    //{
            //    //    for (int j = 0; j < cnt; j++)
            //    //    {
            //    //        //Apply style to each cell
            //    //        gvExport.Rows[i].Cells[j].Attributes.Add("class", styles[j, 0]);
            //    //    }
            //    //}

            //    //gvExport.RenderControl(hw);
            //    //StringBuilder style = new StringBuilder();
            //    //style.Append("<style>");
            //    //for (int j = 0; j < cnt; j++)
            //    //{
            //    //    style.Append("." + styles[j, 0] + " { mso-number-format:" + styles[j, 1] + " }");
            //    //}

            //    //style.Append("</style>");
            //    //Response.Clear();
            //    //Response.Buffer = true;
            //    //Response.AddHeader("content-disposition", "attachment;filename=sales_by_item_export.xls"); Response.Charset = "";
            //    //Response.ContentType = "application/vnd.ms-excel";
            //    //Response.Write(style.ToString());
            //    ////string headerTable = @"<Table><tr><td></td><td>Report Header</td></tr><tr><td>second</td></tr></Table>";
            //    ////Response.Output.Write(headerTable);
            //    //Response.Output.Write(sw.ToString());
            //    //Response.Flush();
            //    //Response.End();
            //}
            //catch (Exception ex)
            //{
            //    WebUtility.DisplayJavascriptMessage(this, ex.Message);
            //}
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            if (btnFilter.Text == "Show Filters")
            {
                filter.Visible = true;
                btnFilter.Text = "Hide Filters";
            }
            else
            {
                filter.Visible = false;
                btnFilter.Text = "Show Filters";
            }
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/OrderOverview.aspx?account=" + Server.UrlEncode(hfAccountID.Value) + "&startdate=" + Server.UrlEncode(this.txtOrderStartDate.Text.Trim()) + "&enddate=" + Server.UrlEncode(this.txtOrderEndDate.Text.Trim()) + "&ordernumber=" + Server.UrlEncode(this.txtOrderNumber.Text.Trim()) + "&pendingapproval=" + Server.UrlEncode(Convert.ToString(this.chkPendingApproval.Checked)) ));
            //BindSalesOrders();
        }

        protected void txtOrderNumber_TextChanged(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/OrderOverview.aspx?account=" + Server.UrlEncode(hfAccountID.Value) + "&startdate=" + Server.UrlEncode(this.txtOrderStartDate.Text.Trim()) + "&enddate=" + Server.UrlEncode(this.txtOrderEndDate.Text.Trim()) + "&ordernumber=" + Server.UrlEncode(this.txtOrderNumber.Text.Trim()) + "&pendingapproval=" + Server.UrlEncode(Convert.ToString(this.chkPendingApproval.Checked)) ));
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

            Response.Redirect(String.Format("/Admin/OrderOverview.aspx?account=" + Server.UrlEncode(hfAccountID.Value) + "&startdate=" + Server.UrlEncode(this.txtOrderStartDate.Text.Trim()) + "&enddate=" + Server.UrlEncode(this.txtOrderEndDate.Text.Trim()) + "&ordernumber=" + Server.UrlEncode(this.txtOrderNumber.Text.Trim()) + "&pendingapproval=" + Server.UrlEncode(Convert.ToString(this.chkPendingApproval.Checked)) ));

        }

        protected void gvSalesOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    HyperLink lnkInvoiceFile = (HyperLink)e.Row.FindControl("lnkInvoiceFile");
                    HiddenField hfSalesOrderID = (HiddenField)e.Row.FindControl("hfSalesOrderID");

                    ImageSolutions.SalesOrder.SalesOrder SalesOrder = new ImageSolutions.SalesOrder.SalesOrder(Convert.ToString(hfSalesOrderID.Value));
                    lnkInvoiceFile.Visible = !string.IsNullOrEmpty(SalesOrder.InvoiceFilePath);
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
    }
}
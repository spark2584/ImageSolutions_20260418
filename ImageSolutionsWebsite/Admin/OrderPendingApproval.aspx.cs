using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class OrderPendingApproval : BasePageAdminUserWebSiteAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Initialize();
            }
        }

        protected void Initialize()
        {
            UpdateHeaderText();

            BindSalesOrders();
        }

        protected void BindSalesOrders()
        {
            List<ImageSolutions.SalesOrder.SalesOrder> SalesOrders = null;
            //ImageSolutions.SalesOrder.SalesOrderFilter SalesOrderFilter = null;
            List<ImageSolutions.SalesOrder.SalesOrder> MyApprovalSalesOrders = null;
            try
            {
                //SalesOrders = new List<ImageSolutions.SalesOrder.SalesOrder>();
                //SalesOrderFilter = new ImageSolutions.SalesOrder.SalesOrderFilter();
                //SalesOrderFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                //SalesOrderFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;
                //SalesOrderFilter.IsPendingApproval = true;
                //SalesOrderFilter.IsClosed = false;
                //SalesOrders = ImageSolutions.SalesOrder.SalesOrder.GetSalesOrders(SalesOrderFilter);

                //SalesOrders = GetPendingApprovalOrders();

                if (!CurrentUser.CurrentUserWebSite.WebSite.OrderApprovalRequired)
                {
                    SalesOrders = GetUserPendingApprovalOrders();

                    MyApprovalSalesOrders = new List<ImageSolutions.SalesOrder.SalesOrder>();

                    //foreach (ImageSolutions.SalesOrder.SalesOrder _SalesOrder in SalesOrders)
                    //{
                    //    if (_SalesOrder.IsPendingApproval && CanApprove(_SalesOrder))
                    //    {
                    //        MyApprovalSalesOrders.Add(_SalesOrder);
                    //    }
                    //    else if(_SalesOrder.IsPendingItemPersonalizationApproval && CanApprovePersonalization(_SalesOrder))
                    //    {
                    //        MyApprovalSalesOrders.Add(_SalesOrder);
                    //    }                        
                    //}

                    foreach (ImageSolutions.SalesOrder.SalesOrder _SalesOrder in SalesOrders)
                    {
                        MyApprovalSalesOrders.Add(_SalesOrder);
                    }

                    this.gvSalesOrders.DataSource = MyApprovalSalesOrders.OrderBy(x => Convert.ToInt32(x.SalesOrderID)).ToList();
                    this.gvSalesOrders.DataBind();
                }
                else if (CurrentUser.CurrentUserWebSite.OrderManagement)
                {
                    SalesOrders = GetPendingApprovalOrders();

                    MyApprovalSalesOrders = new List<ImageSolutions.SalesOrder.SalesOrder>();

                    foreach(ImageSolutions.SalesOrder.SalesOrder _SalesOrder in SalesOrders)
                    {
                        if(_SalesOrder.IsPendingApproval)
                        {
                            MyApprovalSalesOrders.Add(_SalesOrder);
                        }
                        else if (_SalesOrder.IsPendingItemPersonalizationApproval && CanApprovePersonalization(_SalesOrder))
                        {
                            MyApprovalSalesOrders.Add(_SalesOrder);
                        }
                    }

                    //this.gvSalesOrders.DataSource = SalesOrders.OrderBy(x => Convert.ToInt32(x.SalesOrderID)).ToList();
                    //this.gvSalesOrders.DataBind();
                }

                this.gvSalesOrders.DataSource = MyApprovalSalesOrders.OrderBy(x => Convert.ToInt32(x.SalesOrderID)).ToList();
                this.gvSalesOrders.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected bool CanApprove(ImageSolutions.SalesOrder.SalesOrder salesorder)
        {
            bool _return = false;

            if (CurrentWebsite.OrderApprovalRequired)
            {
                _return = true;
            }

            if(!_return)
            {
                //List<ImageSolutions.Account.AccountOrderApproval> AccountOrderApprovals = new List<ImageSolutions.Account.AccountOrderApproval>();
                //ImageSolutions.Account.AccountOrderApprovalFilter AccountOrderApprovalFilter = new ImageSolutions.Account.AccountOrderApprovalFilter();

                //AccountOrderApprovalFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                //AccountOrderApprovalFilter.AccountID.SearchString = salesorder.AccountID;
                //AccountOrderApprovals = ImageSolutions.Account.AccountOrderApproval.GetAccountOrderApprovals(AccountOrderApprovalFilter);

                //if (AccountOrderApprovals != null && AccountOrderApprovals.Count > 0)
                //{
                //    AccountOrderApprovals = AccountOrderApprovals.Where(m => m.Amount <= Convert.ToDecimal(salesorder.Total)).ToList();

                //    if (AccountOrderApprovals != null && AccountOrderApprovals.Count > 0)
                //    {
                //        ImageSolutions.Account.AccountOrderApproval AccountOrderApproval = new ImageSolutions.Account.AccountOrderApproval();
                //        AccountOrderApproval = AccountOrderApprovals.OrderByDescending(m => m.Amount).First();

                //        if (CurrentUser.UserInfoID == AccountOrderApproval.UserWebsite.UserInfoID)
                //        {
                //            _return = true;
                //        }
                //    }
                //}

                string strUserInfoID = GetAccountOrderApprovalUserInfoID(salesorder.AccountID, Convert.ToDecimal(salesorder.Total));

                if(CurrentUser.UserInfoID == strUserInfoID)
                {
                    _return = true;
                }
            }

            if (!_return)
            {
                foreach (ImageSolutions.Payment.Payment _Payment in salesorder.Payments)
                {
                    if (!string.IsNullOrEmpty(_Payment.BudgetAssignmentID)
                        && _Payment.AmountPaid != salesorder.PaymentTotal)                      
                    {
                        if (CurrentUser.UserInfoID == _Payment.BudgetAssignment.Budget.ApproverUserWebsite.UserInfoID)
                        {
                            _return = true;
                        }
                    }
                }
            }

            return _return;
        }

        protected string GetAccountOrderApprovalUserInfoID(string accountid, decimal amount)
        {
            string strUserInfoID = string.Empty;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"
SELECT TOP 1 uw.UserInfoID
FROM AccountOrderApproval (NOLOCK) a
Inner Join UserWebsite (NOLOCK) uw on uw.UserWebsiteID = a.UserWebsiteID
WHERE a.AccountID = {0}
and a.Amount <= {1}
ORDER BY a.Amount DESC
"
                    , Database.HandleQuote(accountid)
                    , Database.HandleQuote(Convert.ToString(amount))
                    );

                objRead = Database.GetDataReader(strSQL);
                if (objRead.Read())
                {
                    strUserInfoID = Convert.ToString(objRead["UserInfoID"]);
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

            return strUserInfoID;
        }


        protected ImageSolutions.User.UserWebsite GetPersonalizationAppover(string accountid)
        {
            ImageSolutions.User.UserWebsite UserWebsite = null;
            ImageSolutions.Account.Account Account = null;

            try
            {
                Account = new ImageSolutions.Account.Account(accountid);

                if (!string.IsNullOrEmpty(Account.PersonalizationApproverUserWebsiteID))
                {
                    UserWebsite = new ImageSolutions.User.UserWebsite(Account.PersonalizationApproverUserWebsiteID);
                }
                else if (!string.IsNullOrEmpty(Account.ParentID))
                {
                    UserWebsite = GetPersonalizationAppover(Account.ParentID);
                }
            }
            catch (Exception ex)
            {

            }

            return UserWebsite;
        }

        protected ImageSolutions.User.UserWebsite GetPersonalizationAppover2(string accountid)
        {
            ImageSolutions.User.UserWebsite UserWebsite = null;
            ImageSolutions.Account.Account Account = null;

            try
            {
                Account = new ImageSolutions.Account.Account(accountid);

                if (!string.IsNullOrEmpty(Account.PersonalizationApprover2UserWebsiteID))
                {
                    UserWebsite = new ImageSolutions.User.UserWebsite(Account.PersonalizationApprover2UserWebsiteID);
                }
                else if (string.IsNullOrEmpty(Account.PersonalizationApproverUserWebsiteID) && !string.IsNullOrEmpty(Account.ParentID))
                {
                    UserWebsite = GetPersonalizationAppover2(Account.ParentID);
                }
            }
            catch (Exception ex)
            {

            }

            return UserWebsite;
        }

        protected bool CanApprovePersonalization(ImageSolutions.SalesOrder.SalesOrder salesorder)
        {
            bool blnReturn = false;
            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
            List<ImageSolutions.User.UserWebsite> UserWebsites = null;

            try
            {
                UserWebsites = new List<ImageSolutions.User.UserWebsite>();
                //if (CurrentWebsite.CombineWebsiteGroup)
                //{
                //    foreach (ImageSolutions.User.UserAccount _UserAccount in CurrentUser.CurrentUserWebSite.UserAccounts)
                //    {
                //        UserWebsite = GetPersonalizationAppover(CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.AccountID);

                //        if (UserWebsite != null && !string.IsNullOrEmpty(UserWebsite.UserWebsiteID))
                //        {
                //            if (UserWebsite.UserInfoID == CurrentUser.UserInfoID)
                //            {
                //                blnReturn = true;
                //            }
                //        }
                //    }
                //}
                //else
                //{

                UserWebsite = GetPersonalizationAppover(salesorder.AccountID);

                if (UserWebsite != null && UserWebsite.UserInfoID == CurrentUser.UserInfoID)
                {
                    blnReturn = true;
                }

                if (!blnReturn)
                {
                    UserWebsite = GetPersonalizationAppover2(salesorder.AccountID);

                    if (UserWebsite != null && UserWebsite.UserInfoID == CurrentUser.UserInfoID)
                    {
                        blnReturn = true;
                    }
                }

                //}
            }
            catch (Exception ex)
            {

            }
            finally
            {
                UserWebsite = null;
            }
            return blnReturn;
        }


        public List<ImageSolutions.SalesOrder.SalesOrder> GetUserPendingApprovalOrders()
        {
            List<ImageSolutions.SalesOrder.SalesOrder> SalesOrders = null;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            int counter = 0;

            try
            {
                SalesOrders = new List<ImageSolutions.SalesOrder.SalesOrder>();
                strSQL = string.Format(@"
SELECT DISTINCT s.SalesOrderID, line.Amount, ordertotal.Amount, approve.UserInfoID, s.IsPendingApproval, s.IsPendingItemPersonalizationApproval
FROM SalesOrder (NOLOCK) s
Outer Apply
(
	SELECT SUM(sl.Quantity * sl.UnitPrice) Amount FROM SalesOrderLine (NOLOCK) sl WHERE sl.SalesOrderID = s.SalesOrderID
) line
Outer Apply
(
	SELECT line.Amount + s.ShippingAmount + s.TaxAmount as Amount
) ordertotal
Outer Apply
(
	SELECT MAX(a.Amount) Amount
	FROM AccountOrderApproval (NOLOCK) a
	Inner Join UserWebsite (NOLOCK) uw on uw.UserWebsiteID = a.UserWebsiteID
	WHERE a.AccountID = s.AccountID
	and a.Amount <= ordertotal.Amount
) maxApprovalAmount
Outer Apply
(
	SELECT uw.UserInfoID
	FROM AccountOrderApproval (NOLOCK) a
	Inner Join UserWebsite (NOLOCK) uw on uw.UserWebsiteID = a.UserWebsiteID
	WHERE a.AccountID = s.AccountID
	and a.Amount = maxApprovalAmount.Amount
) approve
WHERE s.WebsiteID = {0}
and s.IsClosed = 0 
and ( s.IsPendingApproval = 1 or s.IsPendingItemPersonalizationApproval = 1 )
"
                        , Database.HandleQuote(CurrentWebsite.WebsiteID) );
                objRead = Database.GetDataReader(strSQL);
                while (objRead.Read())
                {
                    if (!chkTop100.Checked || counter < 100)
                    {
                        bool IsUpdated = false;

                        ImageSolutions.SalesOrder.SalesOrder SalesOrder = new ImageSolutions.SalesOrder.SalesOrder(Convert.ToString(objRead["SalesOrderID"]));

                        if (Convert.ToBoolean(objRead["IsPendingApproval"]))
                        {
                            if (CurrentUser.UserInfoID == Convert.ToString(objRead["UserInfoID"]))
                            {
                                counter++;
                                SalesOrders.Add(SalesOrder);
                                IsUpdated = true;
                            }
                        }
                        else if (Convert.ToBoolean(objRead["IsPendingItemPersonalizationApproval"]))
                        {
                            if (CanApprovePersonalization(SalesOrder))
                            {
                                counter++;
                                SalesOrders.Add(SalesOrder);
                                IsUpdated = true;
                            }
                        }

                        if (!IsUpdated)
                        {
                            foreach (ImageSolutions.Payment.Payment _Payment in SalesOrder.Payments)
                            {
                                if (!string.IsNullOrEmpty(_Payment.BudgetAssignmentID)
                                    && !string.IsNullOrEmpty(_Payment.BudgetAssignment.Budget.ApproverUserWebsiteID)
                                    && _Payment.AmountPaid != SalesOrder.PaymentTotal)
                                {
                                    if (CurrentUser.UserInfoID == _Payment.BudgetAssignment.Budget.ApproverUserWebsite.UserInfoID)
                                    {
                                        counter++;
                                        SalesOrders.Add(SalesOrder);
                                        IsUpdated = true;
                                    }
                                }
                            }
                        }
                    }                   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return SalesOrders;
        }


        public List<ImageSolutions.SalesOrder.SalesOrder> GetPendingApprovalOrders()
        {
            List<ImageSolutions.SalesOrder.SalesOrder> SalesOrders = null;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            int counter = 0;

            try
            {
                SalesOrders = new List<ImageSolutions.SalesOrder.SalesOrder>();
                strSQL = string.Format(@"
SELECT s.SalesOrderID 
FROM SalesOrder (NOLOCK) s
WHERE s.WebsiteID = {0}
and s.IsClosed = 0 
and ( s.IsPendingApproval = 1 or s.IsPendingItemPersonalizationApproval = 1 )
"
                        , Database.HandleQuote(CurrentWebsite.WebsiteID));
                objRead = Database.GetDataReader(strSQL);
                while (objRead.Read())
                {
                    if (!chkTop100.Checked || counter < 100)
                    {
                        counter++;
                        ImageSolutions.SalesOrder.SalesOrder SalesOrder = new ImageSolutions.SalesOrder.SalesOrder(Convert.ToString(objRead["SalesOrderID"]));
                        SalesOrders.Add(SalesOrder);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return SalesOrders;
        }


        protected void gvSalesOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string strSalesOrderID = gvSalesOrders.DataKeys[e.Row.RowIndex].Value.ToString();
                    ImageSolutions.SalesOrder.SalesOrder SalesOrder = new ImageSolutions.SalesOrder.SalesOrder(strSalesOrderID);
                    CheckBox cbPendingOrderApproval = (CheckBox)e.Row.FindControl("cbPendingOrderApproval");
                    CheckBox cbPendingPersonalizationApproval = (CheckBox)e.Row.FindControl("cbPendingPersonalizationApproval");

                    cbPendingOrderApproval.Checked = SalesOrder.IsPendingApproval;
                    cbPendingPersonalizationApproval.Checked = SalesOrder.IsPendingItemPersonalizationApproval;
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void chkTop100_CheckedChanged(object sender, EventArgs e)
        {
            BindSalesOrders();
        }

        private void UpdateHeaderText()
        {
            if (!string.IsNullOrEmpty(CurrentWebsite.BudgetAlias))
            {
                TemplateField objTemplateFieldCreditApplied = gvSalesOrders.Columns[7] as TemplateField;
                if (objTemplateFieldCreditApplied != null)
                {
                    objTemplateFieldCreditApplied.HeaderText = string.Format("{0} Applied", CurrentWebsite.BudgetAlias);
                }
            }
        }
    }
}
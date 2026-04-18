using ImageSolutions.Account;
using ImageSolutions.Budget;
using ImageSolutions.ShoppingCart;
using ImageSolutions.Website;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.User
{
    public class UserWebsite : ISBase.BaseClass
    {
        public string UserWebsiteID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(UserWebsiteID); } }
        public string GUID { get; set; }
        public string UserInfoID { get; set; }
        public string WebsiteID { get; set; }
        public string CustomerInternalID { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsPendingApproval { get; set; }
        public bool WebsiteManagement { get; set; }
        public bool StoreManagement { get; set; }
        public bool UserManagement { get; set; }
        public bool GroupManagement { get; set; }
        public bool TabManagement { get; set; }
        public bool ItemManagement { get; set; }
        public bool BudgetManagement { get; set; }
        public bool IsBudgetAdmin { get; set; }
        public bool IsBudgetViewOnly { get; set; }        
        public bool OrderManagement { get; set; }
        public bool CreditCardManagement { get; set; }
        public bool ShippingManagement { get; set; }
        public bool MessageManagement { get; set; }
        public bool OptInForNotification { get; set; }
        public bool HideInventoryReport { get; set; }
        public bool HideOrderApproval { get; set; }
        //public string PaymentTerm { get; set; }
        public string PaymentTermID { get; set; }
        public decimal? PaymentTermAmount { get; set; }
        public DateTime? PaymentTermStartDate { get; set; }
        //public DateTime? PaymentTermEndDate { get; set; }
        public string Division { get; set; }
        public string Military { get; set; }
        public string EmployeeID { get; set; }
        public DateTime? HiredDate { get; set; }
        public bool IsPartTime { get; set; }
        public bool AutoAssignGroup { get; set; }
        public bool AutoAssignBudget { get; set; }
        public bool DisablePlaceOrder { get; set; }
        public bool IsTaxExempt { get; set; }
        public bool RequestTaxExempt { get; set; }
        public string NotificationEmail { get; set; }
        public bool SendWelcomeEmail { get; set; }
        public string AddressPermission { get; set; }
        public bool EnableShipToAccount { get; set; }
        public string DefaultShippingAddressID { get; set; }
        public string DefaultBillingAddressID { get; set; }
        public bool IsStore { get; set; }
        public string PunchoutSessionID { get; set; }
        public string PunchoutGUID { get; set; }
        public string PunchoutReturnURL { get; set; }
        public string BudgetSetting { get; set; }
        public bool AllowPasswordUpdate { get; set; }
        public bool DisplayNotificaitonEmailAtCheckout { get; set; }
        public bool DisableNotificationEmail { get; set; }
        public bool ApplyBudgetProgram { get; set; }

        public DateTime? PackageAvailableDate { get; set; }
        public string LocationCode { get; set; }
        public bool IsUpdated { get; set; }
        public string SMSMobileNumber { get; set; }

        //public bool SMSOptIn { get; set; }
        public bool SMSOptIn { get { return !string.IsNullOrEmpty(SMSMobileNumber); } }
        public bool EmailOptIn { get { return !string.IsNullOrEmpty(NotificationEmail); } }

        public bool InActive { get; set; }
        //public bool MarketingWelcomeSent { get; set; }

        public bool EnableSMSOptIn { get; set; }
        public bool EnableEmailOptIn { get; set; }
        public bool MarketingWelcome { get; set; }
        public bool MarketingOutreach { get; set; }
        public bool MarketingCartAbandonment { get; set; }
        public bool MarketingBudgetExpiration { get; set; }
        public bool MarketingBudgetRenewal { get; set; }
        public DateTime? MarketingWelcomeSentOn { get; set; }
        public DateTime? MarketingOutreachSentOn { get; set; }
        public DateTime? MarketingCartAbandonmentSentOn { get; set; }
        public DateTime? MarketingBudgetExpirationSentOn { get; set; }
        public DateTime? MarketingBudgetRenewalSentOn { get; set; }

        public double TotalActiveBudgetAmount
        {
            get
            {
                double dbReturn = 0;

                if (MyBudgetAssignments != null)
                {

                    dbReturn = MyBudgetAssignments.FindAll(x =>
                                !x.BudgetAssignment.InActive
                                && x.BudgetAssignment.Budget.StartDate <= DateTime.UtcNow
                                && x.BudgetAssignment.Budget.EndDate >= DateTime.UtcNow.AddDays(-1)
                                && (x.Balance > 0)).Sum(n => n.Balance);
                }

                return dbReturn;
            }
        }

        public DateTime? ActiveBudgetExpirationDate
        {
            get
            {
                DateTime? dbReturn = null;

                if (MyBudgetAssignments != null)
                {
                    List<MyBudgetAssignment> ActiveMyBudgetAssignments = MyBudgetAssignments.FindAll(x =>
                                !x.BudgetAssignment.InActive
                                && x.BudgetAssignment.Budget.StartDate <= DateTime.UtcNow
                                && x.BudgetAssignment.Budget.EndDate >= DateTime.UtcNow.AddDays(-1)
                                && (x.Balance > 0));
                    if (ActiveMyBudgetAssignments != null && ActiveMyBudgetAssignments.Count > 0)
                    {
                        dbReturn = ActiveMyBudgetAssignments.Min(n => n.BudgetAssignment.Budget.EndDate);
                    }
                }

                return dbReturn;
            }
        }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        private List<Payment.Payment> mPaymentTermPayments = null;
        public List<Payment.Payment> PaymentTermPayments
        {
            get
            {
                if (mPaymentTermPayments == null && !string.IsNullOrEmpty(UserWebsiteID) && !string.IsNullOrEmpty(PaymentTermID))
                {
                    ImageSolutions.Payment.PaymentFilter objFilter = null;
                    try
                    {
                        List<Payment.Payment> Payments = new List<Payment.Payment>();
                        objFilter = new Payment.PaymentFilter();
                        objFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.UserWebsiteID.SearchString = UserWebsiteID;
                        objFilter.PaymentTermID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.PaymentTermID.SearchString = PaymentTermID;
                        if(PaymentTermStartDate != null)
                        {
                            objFilter.CreatedOn = new Database.Filter.DateTimeSearch.SearchFilter();
                            objFilter.CreatedOn.Operator = Database.Filter.DateTimeSearch.SearchOperator.onOrAfter;
                            objFilter.CreatedOn.From = PaymentTermStartDate;
                        }
                        mPaymentTermPayments = Payment.Payment.GetPayments(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mPaymentTermPayments;
            }
        }

        private List<Payment.PaymentTermAdjustment> mPaymentTermAdjustments = null;
        public List<Payment.PaymentTermAdjustment> PaymentTermAdjustments
        {
            get
            {
                if (mPaymentTermAdjustments == null && !string.IsNullOrEmpty(UserWebsiteID) && !string.IsNullOrEmpty(PaymentTermID))
                {
                    ImageSolutions.Payment.PaymentTermAdjustmentFilter objFilter = null;
                    try
                    {
                        List<Payment.PaymentTermAdjustmentFilter> Payments = new List<Payment.PaymentTermAdjustmentFilter>();
                        objFilter = new Payment.PaymentTermAdjustmentFilter();
                        objFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.UserWebsiteID.SearchString = UserWebsiteID;
                        if (PaymentTermStartDate != null)
                        {
                            objFilter.TransactionDate = new Database.Filter.DateTimeSearch.SearchFilter();
                            objFilter.TransactionDate.Operator = Database.Filter.DateTimeSearch.SearchOperator.onOrAfter;
                            objFilter.TransactionDate.From = PaymentTermStartDate;
                        }
                        mPaymentTermAdjustments = Payment.PaymentTermAdjustment.GetPaymentTermAdjustments(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mPaymentTermAdjustments;
            }
        }

        public decimal PaymentTermBalance
        {
            get
            {
                decimal decReturn = 0;

                if (PaymentTermAmount != null)
                {
                    decReturn = Convert.ToDecimal(PaymentTermAmount);

                    if (PaymentTermPayments != null)
                    {
                        decReturn = decReturn - Convert.ToDecimal(PaymentTermPayments.Sum(m => m.AmountPaid));                        
                    }

                    if(PaymentTermAdjustments != null)
                    {
                        decReturn = decReturn + Convert.ToDecimal(PaymentTermAdjustments.Sum(m => m.Amount));
                    }
                }
                else
                {
                    decReturn = 0;
                }

                return decReturn < 0 ? 0 : Decimal.Round(decReturn, 2);
            }
        }

        public string Description
        {
            get
            {
                string strDescription = UserInfo == null ? string.Empty : UserInfo.FullName + " | " + (string.IsNullOrEmpty(UserInfo.UserName) ? UserInfo.EmailAddress : UserInfo.UserName);

                if (!string.IsNullOrEmpty(EmployeeID))
                {
                    strDescription = string.Format("{0} | {1}", strDescription, EmployeeID);
                }

                return strDescription;
            }
        }
        private List<UserWebsiteTab> mUserWebsiteTab = null;
        public List<UserWebsiteTab> UserWebsiteTabs
        {
            get
            {
                UserWebsiteTabFilter objFilter = null;

                try
                {
                    if (mUserWebsiteTab == null && !string.IsNullOrEmpty(UserWebsiteID))
                    {
                        objFilter = new UserWebsiteTabFilter();
                        objFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.UserWebsiteID.SearchString = UserWebsiteID;
                        mUserWebsiteTab = UserWebsiteTab.GetUserWebsiteTabs(objFilter);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    objFilter = null;
                }
                return mUserWebsiteTab;
            }
        }

        private Website.Website mWebSite = null;
        public Website.Website WebSite
        {
            get
            {
                if (mWebSite == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    mWebSite = new Website.Website(WebsiteID);
                }
                return mWebSite;
            }
        }

        private UserInfo mCreatedByUser = null;
        public UserInfo CreatedByUser
        {
            get
            {
                if (mCreatedByUser == null && !string.IsNullOrEmpty(CreatedBy))
                {
                    mCreatedByUser = new UserInfo(CreatedBy);
                }
                return mCreatedByUser;
            }
        }

        private User.UserInfo mUserInfo = null;
        public User.UserInfo UserInfo
        {
            get
            {
                if (mUserInfo == null && !string.IsNullOrEmpty(UserInfoID))
                {
                    mUserInfo = new ImageSolutions.User.UserInfo(UserInfoID);
                }
                return mUserInfo;
            }
        }

        private List<UserAccount> mUserAccounts = null;
        public List<UserAccount> UserAccounts
        {
            get
            {
                if (mUserAccounts == null && !string.IsNullOrEmpty(UserWebsiteID))
                {
                    UserAccountFilter objFilter = null;

                    try
                    {
                        objFilter = new UserAccountFilter();
                        objFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.UserWebsiteID.SearchString = UserWebsiteID;
                        mUserAccounts = UserAccount.GetUserAccounts(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mUserAccounts;
            }
        }

        private List<Account.Account> mAccounts = null;
        public List<Account.Account> Accounts
        {
            get
            {
                if (UserAccounts != null)
                {
                    mAccounts = new List<Account.Account>();
                    foreach (UserAccount objUserAccount in UserAccounts)
                    {
                        if (!mAccounts.Exists(m => m.AccountID == objUserAccount.AccountID))
                        {
                            mAccounts.Add(objUserAccount.Account);
                        }

                        if (objUserAccount.Account.ChildAccounts != null)
                        {
                            foreach (Account.Account objAccount in objUserAccount.Account.ChildAccounts)
                            {
                                if (!mAccounts.Exists(m => m.AccountID == objAccount.AccountID))
                                {
                                    mAccounts.Add(objAccount);
                                }
                            }
                        }
                    }
                }
                return mAccounts;
            }
        }

        private List<Account.Account> mSubAccounts = null;
        public List<Account.Account> SubAccounts
        {
            get
            {
                if (UserAccounts != null)
                {
                    mSubAccounts = new List<Account.Account>();
                    foreach (UserAccount objUserAccount in UserAccounts)
                    {
                        //AddChildAccount(objUserAccount.Account.ChildAccounts, ref mSubAccounts);

                        if (objUserAccount.Account.ChildAccounts != null)
                        {
                            foreach (Account.Account objAccount in objUserAccount.Account.ChildAccounts)
                            {
                                if (!mSubAccounts.Exists(m => m.AccountID == objAccount.AccountID))
                                {
                                    mSubAccounts.Add(objAccount);
                                }
                            }
                        }
                    }
                }
                return mSubAccounts;
            }
        }

        //public void AddChildAccount(List<Account.Account> childaccounts, ref List<Account.Account> subaccounts)
        //{
        //    foreach(Account.Account _Account in childaccounts)
        //    {
        //        if (!mSubAccounts.Exists(m => m.AccountID == _Account.AccountID))
        //        {
        //            subaccounts.Add(_Account);
        //        }
        //        AddChildAccount(_Account.ChildAccounts, ref subaccounts);
                
        //    }
        //}


        private List<SalesOrder.SalesOrder> mSalesOrders = null;
        public List<SalesOrder.SalesOrder> SalesOrders
        {
            get
            {
                if (mSalesOrders == null && !string.IsNullOrEmpty(UserInfoID) && !string.IsNullOrEmpty(WebsiteID))
                {
                    SalesOrder.SalesOrderFilter objFilter = null;

                    try
                    {
                        objFilter = new SalesOrder.SalesOrderFilter();
                        objFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.UserInfoID.SearchString = UserInfoID;
                        objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteID.SearchString = WebsiteID;
                        mSalesOrders = SalesOrder.SalesOrder.GetSalesOrders(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mSalesOrders;
            }
        }

        private List<Budget.BudgetAssignment> mBudgetAssignments = null;
        public List<Budget.BudgetAssignment> BudgetAssignments
        {
            get
            {
                if (mBudgetAssignments == null && !string.IsNullOrEmpty(UserWebsiteID))
                {
                    Budget.BudgetAssignmentFilter objFilter = null;
                    List<Budget.BudgetAssignment> objUserAssignments = null;
                    List<Budget.BudgetAssignment> objGroupAssignments = null;

                    try
                    {
                        objFilter = new Budget.BudgetAssignmentFilter();
                        objFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.UserWebsiteID.SearchString = UserWebsiteID;
                        objUserAssignments = Budget.BudgetAssignment.GetBudgetAssignments(objFilter);

                        if (UserAccounts != null && UserAccounts.Count > 0)
                        {
                            objFilter = new BudgetAssignmentFilter();
                            objFilter.WebsiteGroupIDs = new List<string>();
                            foreach (UserAccount objUserAccount in UserAccounts)
                            {
                                objFilter.WebsiteGroupIDs.Add(objUserAccount.WebsiteGroupID);
                            }
                            objGroupAssignments = Budget.BudgetAssignment.GetBudgetAssignments(objFilter);
                        }

                        mBudgetAssignments = objUserAssignments;
                        if (objGroupAssignments != null && objGroupAssignments.Count > 0)
                        {
                            foreach (Budget.BudgetAssignment objBudgetAssignment in objGroupAssignments)
                            {
                                if (!mBudgetAssignments.Exists(m => m.Budget.BudgetID == objBudgetAssignment.Budget.BudgetID))
                                {
                                    mBudgetAssignments.Add(objBudgetAssignment);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mBudgetAssignments;
            }
        }

        private List<Budget.MyBudgetAssignment> mMyBudgetAssignments = null;
        public List<Budget.MyBudgetAssignment> MyBudgetAssignments
        {
            get
            {
                if (mMyBudgetAssignments == null && BudgetAssignments != null)
                {
                    mMyBudgetAssignments = new List<Budget.MyBudgetAssignment>();

                    foreach (Budget.BudgetAssignment objBudgetAssignment in BudgetAssignments)
                    {
                        mMyBudgetAssignments.Add(new Budget.MyBudgetAssignment(UserInfoID, objBudgetAssignment.BudgetAssignmentID));
                    }
                }
                return mMyBudgetAssignments;
            }
        }

        private ShoppingCart.ShoppingCart mShoppingCart = null;
        public ShoppingCart.ShoppingCart ShoppingCart
        {
            get
            {
                if (mShoppingCart == null && !string.IsNullOrEmpty(UserWebsiteID))
                {
                    ShoppingCart.ShoppingCartFilter objFilter = null;

                    try
                    {
                        objFilter = new ShoppingCart.ShoppingCartFilter();
                        objFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.UserWebsiteID.SearchString = UserWebsiteID;
                        objFilter.SalesOrderID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.SalesOrderID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                        objFilter.PunchoutSessionID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.PunchoutSessionID.Operator = Database.Filter.StringSearch.SearchOperator.empty;

                        mShoppingCart = ImageSolutions.ShoppingCart.ShoppingCart.GetShoppingCart(objFilter);

                        if (mShoppingCart == null)
                        {
                            mShoppingCart = new ShoppingCart.ShoppingCart();
                            mShoppingCart.UserWebsiteID = UserWebsiteID;
                            mShoppingCart.Create();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mShoppingCart;
            }
        }
        private Payment.PaymentTerm mPaymentTerm = null;
        public Payment.PaymentTerm PaymentTerm
        {
            get
            {
                if (mPaymentTerm == null && !string.IsNullOrEmpty(PaymentTermID))
                {
                    mPaymentTerm = new ImageSolutions.Payment.PaymentTerm(PaymentTermID);
                }

                return mPaymentTerm;
            }
        }
        public UserWebsite()
        {
        }

        public UserWebsite(string UserWebsiteID)
        {
            this.UserWebsiteID = UserWebsiteID;
            Load();
        }

        public UserWebsite(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            base.Load();

            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM UserWebsite (NOLOCK) " +
                         "WHERE UserWebsiteID=" + Database.HandleQuote(UserWebsiteID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("UserWebsiteID=" + UserWebsiteID + " is not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
            }
        }

        protected void Load(string GUID)
        {
            base.Load();

            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM UserWebsite (NOLOCK) " +
                         "WHERE GUID=" + Database.HandleQuote(GUID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("GUID=" + GUID + " is not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
            }
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("UserWebsiteID")) UserWebsiteID = Convert.ToString(objRow["UserWebsiteID"]);
                if (objColumns.Contains("GUID")) GUID = Convert.ToString(objRow["GUID"]);
                if (objColumns.Contains("UserInfoID")) UserInfoID = Convert.ToString(objRow["UserInfoID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("CustomerInternalID")) CustomerInternalID = Convert.ToString(objRow["CustomerInternalID"]);
                if (objColumns.Contains("IsAdmin")) IsAdmin = Convert.ToBoolean(objRow["IsAdmin"]);
                if (objColumns.Contains("IsPendingApproval")) IsPendingApproval = Convert.ToBoolean(objRow["IsPendingApproval"]);
                if (objColumns.Contains("WebsiteManagement")) WebsiteManagement = Convert.ToBoolean(objRow["WebsiteManagement"]);
                if (objColumns.Contains("StoreManagement")) StoreManagement = Convert.ToBoolean(objRow["StoreManagement"]);
                if (objColumns.Contains("UserManagement")) UserManagement = Convert.ToBoolean(objRow["UserManagement"]);
                if (objColumns.Contains("GroupManagement")) GroupManagement = Convert.ToBoolean(objRow["GroupManagement"]);
                if (objColumns.Contains("TabManagement")) TabManagement = Convert.ToBoolean(objRow["TabManagement"]);
                if (objColumns.Contains("ItemManagement")) ItemManagement = Convert.ToBoolean(objRow["ItemManagement"]);
                if (objColumns.Contains("BudgetManagement")) BudgetManagement = Convert.ToBoolean(objRow["BudgetManagement"]);
                if (objColumns.Contains("IsBudgetAdmin")) IsBudgetAdmin = Convert.ToBoolean(objRow["IsBudgetAdmin"]);
                if (objColumns.Contains("IsBudgetViewOnly")) IsBudgetViewOnly = Convert.ToBoolean(objRow["IsBudgetViewOnly"]);
                if (objColumns.Contains("OrderManagement")) OrderManagement = Convert.ToBoolean(objRow["OrderManagement"]);
                if (objColumns.Contains("CreditCardManagement")) CreditCardManagement = Convert.ToBoolean(objRow["CreditCardManagement"]);
                if (objColumns.Contains("ShippingManagement")) ShippingManagement = Convert.ToBoolean(objRow["ShippingManagement"]);
                if (objColumns.Contains("MessageManagement")) MessageManagement = Convert.ToBoolean(objRow["MessageManagement"]);
                if (objColumns.Contains("OptInForNotification")) OptInForNotification = Convert.ToBoolean(objRow["OptInForNotification"]);
                if (objColumns.Contains("HideInventoryReport")) HideInventoryReport = Convert.ToBoolean(objRow["HideInventoryReport"]);
                if (objColumns.Contains("HideOrderApproval")) HideOrderApproval = Convert.ToBoolean(objRow["HideOrderApproval"]);
                //if (objColumns.Contains("PaymentTerm")) PaymentTerm = Convert.ToString(objRow["PaymentTerm"]);
                if (objColumns.Contains("PaymentTermID")) PaymentTermID = Convert.ToString(objRow["PaymentTermID"]);
                if (objColumns.Contains("PaymentTermAmount") && objRow["PaymentTermAmount"] != DBNull.Value) PaymentTermAmount = Convert.ToDecimal(objRow["PaymentTermAmount"]);
                if (objColumns.Contains("PaymentTermStartDate") && objRow["PaymentTermStartDate"] != DBNull.Value) PaymentTermStartDate = Convert.ToDateTime(objRow["PaymentTermStartDate"]);
                //if (objColumns.Contains("PaymentTermEndDate") && objRow["PaymentTermEndDate"] != DBNull.Value) PaymentTermEndDate = Convert.ToDateTime(objRow["PaymentTermEndDate"]);
                if (objColumns.Contains("Division")) Division = Convert.ToString(objRow["Division"]);
                if (objColumns.Contains("Military")) Military = Convert.ToString(objRow["Military"]);
                if (objColumns.Contains("EmployeeID")) EmployeeID = Convert.ToString(objRow["EmployeeID"]);
                if (objColumns.Contains("HiredDate") && objRow["HiredDate"] != DBNull.Value) HiredDate = Convert.ToDateTime(objRow["HiredDate"]);
                if (objColumns.Contains("IsPartTime")) IsPartTime = Convert.ToBoolean(objRow["IsPartTime"]);
                if (objColumns.Contains("AutoAssignGroup")) AutoAssignGroup = Convert.ToBoolean(objRow["AutoAssignGroup"]);
                if (objColumns.Contains("AutoAssignBudget")) AutoAssignBudget = Convert.ToBoolean(objRow["AutoAssignBudget"]);
                if (objColumns.Contains("DisablePlaceOrder")) DisablePlaceOrder = Convert.ToBoolean(objRow["DisablePlaceOrder"]);
                if (objColumns.Contains("IsTaxExempt")) IsTaxExempt = Convert.ToBoolean(objRow["IsTaxExempt"]);
                if (objColumns.Contains("RequestTaxExempt")) RequestTaxExempt = Convert.ToBoolean(objRow["RequestTaxExempt"]);
                if (objColumns.Contains("NotificationEmail")) NotificationEmail = Convert.ToString(objRow["NotificationEmail"]);
                if (objColumns.Contains("SendWelcomeEmail")) SendWelcomeEmail = Convert.ToBoolean(objRow["SendWelcomeEmail"]);
                if (objColumns.Contains("AddressPermission")) AddressPermission = Convert.ToString(objRow["AddressPermission"]);

                if (objColumns.Contains("EnableShipToAccount")) EnableShipToAccount = Convert.ToBoolean(objRow["EnableShipToAccount"]);

                if (objColumns.Contains("DefaultShippingAddressID")) DefaultShippingAddressID = Convert.ToString(objRow["DefaultShippingAddressID"]);
                if (objColumns.Contains("DefaultBillingAddressID")) DefaultBillingAddressID = Convert.ToString(objRow["DefaultBillingAddressID"]);
                if (objColumns.Contains("IsStore")) IsStore = Convert.ToBoolean(objRow["IsStore"]);
                if (objColumns.Contains("PunchoutSessionID")) PunchoutSessionID = Convert.ToString(objRow["PunchoutSessionID"]);
                if (objColumns.Contains("PunchoutGUID")) PunchoutGUID = Convert.ToString(objRow["PunchoutGUID"]);
                if (objColumns.Contains("PunchoutReturnURL")) PunchoutReturnURL = Convert.ToString(objRow["PunchoutReturnURL"]);
                if (objColumns.Contains("BudgetSetting")) BudgetSetting = Convert.ToString(objRow["BudgetSetting"]);
                if (objColumns.Contains("AllowPasswordUpdate")) AllowPasswordUpdate = Convert.ToBoolean(objRow["AllowPasswordUpdate"]);
                if (objColumns.Contains("DisplayNotificaitonEmailAtCheckout")) DisplayNotificaitonEmailAtCheckout = Convert.ToBoolean(objRow["DisplayNotificaitonEmailAtCheckout"]);
                if (objColumns.Contains("DisableNotificationEmail")) DisableNotificationEmail = Convert.ToBoolean(objRow["DisableNotificationEmail"]);
                if (objColumns.Contains("ApplyBudgetProgram")) ApplyBudgetProgram = Convert.ToBoolean(objRow["ApplyBudgetProgram"]);

                if (objColumns.Contains("PackageAvailableDate") && objRow["PackageAvailableDate"] != DBNull.Value) PackageAvailableDate = Convert.ToDateTime(objRow["PackageAvailableDate"]);

                if (objColumns.Contains("LocationCode")) LocationCode = Convert.ToString(objRow["LocationCode"]);
                if (objColumns.Contains("IsUpdated")) IsUpdated = Convert.ToBoolean(objRow["IsUpdated"]);
                if (objColumns.Contains("SMSMobileNumber")) SMSMobileNumber = Convert.ToString(objRow["SMSMobileNumber"]);

                //if (objColumns.Contains("MarketingWelcomeSent")) MarketingWelcomeSent = Convert.ToBoolean(objRow["MarketingWelcomeSent"]);
                if (objColumns.Contains("EnableSMSOptIn")) EnableSMSOptIn = Convert.ToBoolean(objRow["EnableSMSOptIn"]);
                if (objColumns.Contains("EnableEmailOptIn")) EnableEmailOptIn = Convert.ToBoolean(objRow["EnableEmailOptIn"]);
                if (objColumns.Contains("MarketingWelcome")) MarketingWelcome = Convert.ToBoolean(objRow["MarketingWelcome"]);
                if (objColumns.Contains("MarketingOutreach")) MarketingOutreach = Convert.ToBoolean(objRow["MarketingOutreach"]);
                if (objColumns.Contains("MarketingCartAbandonment")) MarketingCartAbandonment = Convert.ToBoolean(objRow["MarketingCartAbandonment"]);
                if (objColumns.Contains("MarketingBudgetExpiration")) MarketingBudgetExpiration = Convert.ToBoolean(objRow["MarketingBudgetExpiration"]);
                if (objColumns.Contains("MarketingBudgetRenewal")) MarketingBudgetRenewal = Convert.ToBoolean(objRow["MarketingBudgetRenewal"]);
                if (objColumns.Contains("MarketingWelcomeSentOn") && objRow["MarketingWelcomeSentOn"] != DBNull.Value) MarketingWelcomeSentOn = Convert.ToDateTime(objRow["MarketingWelcomeSentOn"]);
                if (objColumns.Contains("MarketingOutreachSentOn") && objRow["MarketingOutreachSentOn"] != DBNull.Value) MarketingOutreachSentOn = Convert.ToDateTime(objRow["MarketingOutreachSentOn"]);
                if (objColumns.Contains("MarketingCartAbandonmentSentOn") && objRow["MarketingCartAbandonmentSentOn"] != DBNull.Value) MarketingCartAbandonmentSentOn = Convert.ToDateTime(objRow["MarketingCartAbandonmentSentOn"]);
                if (objColumns.Contains("MarketingBudgetExpirationSentOn") && objRow["MarketingBudgetExpirationSentOn"] != DBNull.Value) MarketingBudgetExpirationSentOn = Convert.ToDateTime(objRow["MarketingBudgetExpirationSentOn"]);
                if (objColumns.Contains("MarketingBudgetRenewalSentOn") && objRow["MarketingBudgetRenewalSentOn"] != DBNull.Value) MarketingBudgetRenewalSentOn = Convert.ToDateTime(objRow["MarketingBudgetRenewalSentOn"]);

                if (objColumns.Contains("InActive")) InActive = Convert.ToBoolean(objRow["InActive"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(UserWebsiteID)) throw new Exception("Missing UserWebsiteID in the datarow");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objColumns = null;
            }
        }

        public override bool Create()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Create(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public override bool Create(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Create();

            if (!IsActive) return true;

            Hashtable dicParam = new Hashtable();
            try
            {
                if (UserInfoID == null) throw new Exception("UserInfoID is required");
                if (WebsiteID == null) throw new Exception("WebsiteID is required");
                //if (ExistsUserInfoIDWebsiteID()) throw new Exception("User is already assigned to this website");
                if (!IsNew) throw new Exception("Create cannot be performed, UserWebsiteID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["UserInfoID"] = UserInfoID;
                dicParam["WebsiteID"] = WebsiteID;
                dicParam["CustomerInternalID"] = CustomerInternalID;
                dicParam["IsAdmin"] = IsAdmin;
                dicParam["IsPendingApproval"] = IsPendingApproval;
                dicParam["WebsiteManagement"] = WebsiteManagement;
                dicParam["StoreManagement"] = StoreManagement;
                dicParam["UserManagement"] = UserManagement;
                dicParam["GroupManagement"] = GroupManagement;
                dicParam["TabManagement"] = TabManagement;
                dicParam["ItemManagement"] = ItemManagement;
                dicParam["BudgetManagement"] = BudgetManagement;
                dicParam["IsBudgetAdmin"] = IsBudgetAdmin;
                dicParam["IsBudgetViewOnly"] = IsBudgetViewOnly;
                dicParam["OrderManagement"] = OrderManagement;
                dicParam["CreditCardManagement"] = CreditCardManagement;
                dicParam["ShippingManagement"] = ShippingManagement;
                dicParam["MessageManagement"] = MessageManagement;
                dicParam["OptInForNotification"] = OptInForNotification;
                dicParam["HideInventoryReport"] = HideInventoryReport;
                dicParam["HideOrderApproval"] = HideOrderApproval;
                //dicParam["PaymentTerm"] = PaymentTerm;
                dicParam["PaymentTermID"] = PaymentTermID;
                dicParam["PaymentTermAmount"] = PaymentTermAmount;
                dicParam["PaymentTermStartDate"] = PaymentTermStartDate;
                //dicParam["PaymentTermEndDate"] = PaymentTermEndDate;
                dicParam["Division"] = Division;
                dicParam["Military"] = Military;
                dicParam["EmployeeID"] = EmployeeID;
                dicParam["HiredDate"] = HiredDate;
                dicParam["IsPartTime"] = IsPartTime;
                dicParam["AutoAssignGroup"] = AutoAssignGroup;
                dicParam["AutoAssignBudget"] = AutoAssignBudget;
                dicParam["DisablePlaceOrder"] = DisablePlaceOrder;
                dicParam["IsTaxExempt"] = IsTaxExempt;
                dicParam["RequestTaxExempt"] = RequestTaxExempt;
                dicParam["NotificationEmail"] = NotificationEmail;
                dicParam["SendWelcomeEmail"] = SendWelcomeEmail;
                dicParam["AddressPermission"] = AddressPermission;

                dicParam["EnableShipToAccount"] = EnableShipToAccount;

                dicParam["DefaultShippingAddressID"] = DefaultShippingAddressID;
                dicParam["DefaultBillingAddressID"] = DefaultBillingAddressID;
                dicParam["IsStore"] = IsStore;
                dicParam["PunchoutSessionID"] = PunchoutSessionID;
                dicParam["PunchoutGUID"] = PunchoutGUID;
                dicParam["PunchoutReturnURL"] = PunchoutReturnURL;
                dicParam["BudgetSetting"] = BudgetSetting;
                dicParam["AllowPasswordUpdate"] = AllowPasswordUpdate;
                dicParam["DisplayNotificaitonEmailAtCheckout"] = DisplayNotificaitonEmailAtCheckout;
                dicParam["DisableNotificationEmail"] = DisableNotificationEmail;
                dicParam["ApplyBudgetProgram"] = ApplyBudgetProgram;

                dicParam["PackageAvailableDate"] = PackageAvailableDate;

                dicParam["LocationCode"] = LocationCode;
                dicParam["IsUpdated"] = IsUpdated;

                dicParam["SMSMobileNumber"] = SMSMobileNumber;
                //dicParam["SMSOptIn"] = SMSOptIn;
                //dicParam["MarketingWelcomeSent"] = MarketingWelcomeSent;

                dicParam["EnableSMSOptIn"] = EnableSMSOptIn;
                dicParam["EnableEmailOptIn"] = EnableEmailOptIn;

                dicParam["MarketingWelcome"] = MarketingWelcome;
                dicParam["MarketingOutreach"] = MarketingOutreach;
                dicParam["MarketingCartAbandonment"] = MarketingCartAbandonment;
                dicParam["MarketingBudgetExpiration"] = MarketingBudgetExpiration;
                dicParam["MarketingBudgetRenewal"] = MarketingBudgetRenewal;

                dicParam["MarketingWelcomeSentOn"] = MarketingWelcomeSentOn;
                dicParam["MarketingOutreachSentOn"] = MarketingOutreachSentOn;
                dicParam["MarketingCartAbandonmentSentOn"] = MarketingCartAbandonmentSentOn;
                dicParam["MarketingBudgetExpirationSentOn"] = MarketingBudgetExpirationSentOn;
                dicParam["MarketingBudgetRenewalSentOn"] = MarketingBudgetRenewalSentOn;

                dicParam["InActive"] = InActive;
                dicParam["CreatedBy"] = CreatedBy;

                UserWebsiteID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "UserWebsite"), objConn, objTran).ToString();

                Load(objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicParam = null;
            }
            return true;
        }

        public override bool Update()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Update(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public override bool Update(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Update();

            if (!IsActive) return Delete(objConn, objTran);

            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (UserInfoID == null) throw new Exception("UserInfoID is required");
                if (WebsiteID == null) throw new Exception("WebsiteID is required");
                //if (ExistsUserInfoIDWebsiteID()) throw new Exception("User is already assigned to this website");
                if (IsNew) throw new Exception("Update cannot be performed, UserWebsiteID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["UserInfoID"] = UserInfoID;
                dicParam["WebsiteID"] = WebsiteID;
                dicParam["CustomerInternalID"] = CustomerInternalID;
                dicParam["IsAdmin"] = IsAdmin;
                dicParam["IsPendingApproval"] = IsPendingApproval;
                dicParam["WebsiteManagement"] = WebsiteManagement;
                dicParam["StoreManagement"] = StoreManagement;
                dicParam["UserManagement"] = UserManagement;
                dicParam["GroupManagement"] = GroupManagement;
                dicParam["TabManagement"] = TabManagement;
                dicParam["ItemManagement"] = ItemManagement;
                dicParam["BudgetManagement"] = BudgetManagement;
                dicParam["IsBudgetAdmin"] = IsBudgetAdmin;
                dicParam["IsBudgetViewOnly"] = IsBudgetViewOnly;
                dicParam["OrderManagement"] = OrderManagement;
                dicParam["CreditCardManagement"] = CreditCardManagement;
                dicParam["ShippingManagement"] = ShippingManagement;
                dicParam["MessageManagement"] = MessageManagement;
                dicParam["OptInForNotification"] = OptInForNotification;
                dicParam["HideInventoryReport"] = HideInventoryReport;
                dicParam["HideOrderApproval"] = HideOrderApproval;
                //dicParam["PaymentTerm"] = PaymentTerm;
                dicParam["PaymentTermID"] = PaymentTermID;
                dicParam["PaymentTermAmount"] = PaymentTermAmount;
                dicParam["PaymentTermStartDate"] = PaymentTermStartDate;
                //dicParam["PaymentTermEndDate"] = PaymentTermEndDate;
                dicParam["Division"] = Division;
                dicParam["Military"] = Military;
                dicParam["EmployeeID"] = EmployeeID;
                dicParam["HiredDate"] = HiredDate;
                dicParam["DisablePlaceOrder"] = DisablePlaceOrder;
                dicParam["IsPartTime"] = IsPartTime;
                dicParam["AutoAssignGroup"] = AutoAssignGroup;
                dicParam["AutoAssignBudget"] = AutoAssignBudget;
                dicParam["IsTaxExempt"] = IsTaxExempt;
                dicParam["RequestTaxExempt"] = RequestTaxExempt;
                dicParam["NotificationEmail"] = NotificationEmail;
                dicParam["SendWelcomeEmail"] = SendWelcomeEmail;
                dicParam["AddressPermission"] = AddressPermission;

                dicParam["EnableShipToAccount"] = EnableShipToAccount;

                dicParam["DefaultShippingAddressID"] = DefaultShippingAddressID;
                dicParam["DefaultBillingAddressID"] = DefaultBillingAddressID;
                dicParam["IsStore"] = IsStore;
                dicParam["PunchoutSessionID"] = PunchoutSessionID;
                dicParam["PunchoutGUID"] = PunchoutGUID;
                dicParam["PunchoutReturnURL"] = PunchoutReturnURL;
                dicParam["BudgetSetting"] = BudgetSetting;
                dicParam["AllowPasswordUpdate"] = AllowPasswordUpdate;
                dicParam["DisplayNotificaitonEmailAtCheckout"] = DisplayNotificaitonEmailAtCheckout;
                dicParam["DisableNotificationEmail"] = DisableNotificationEmail;
                dicParam["ApplyBudgetProgram"] = ApplyBudgetProgram;
                
                dicParam["PackageAvailableDate"] = PackageAvailableDate;

                dicParam["LocationCode"] = LocationCode;
                dicParam["IsUpdated"] = IsUpdated;

                dicParam["SMSMobileNumber"] = SMSMobileNumber;
                //dicParam["SMSOptIn"] = SMSOptIn;
                //dicParam["MarketingWelcomeSent"] = MarketingWelcomeSent;

                dicParam["EnableSMSOptIn"] = EnableSMSOptIn;
                dicParam["EnableEmailOptIn"] = EnableEmailOptIn;

                dicParam["MarketingWelcome"] = MarketingWelcome;
                dicParam["MarketingOutreach"] = MarketingOutreach;
                dicParam["MarketingCartAbandonment"] = MarketingCartAbandonment;
                dicParam["MarketingBudgetExpiration"] = MarketingBudgetExpiration;
                dicParam["MarketingBudgetRenewal"] = MarketingBudgetRenewal;

                dicParam["MarketingWelcomeSentOn"] = MarketingWelcomeSentOn;
                dicParam["MarketingOutreachSentOn"] = MarketingOutreachSentOn;
                dicParam["MarketingCartAbandonmentSentOn"] = MarketingCartAbandonmentSentOn;
                dicParam["MarketingBudgetExpirationSentOn"] = MarketingBudgetExpirationSentOn;
                dicParam["MarketingBudgetRenewalSentOn"] = MarketingBudgetRenewalSentOn;

                dicParam["InActive"] = InActive;
                dicWParam["UserWebsiteID"] = UserWebsiteID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "UserWebsite"), objConn, objTran);

                Load(objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicParam = null;
                dicWParam = null;
            }
            return true;
        }

        public override bool Delete()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Delete(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public override bool Delete(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Delete();

            Hashtable dicDParam = new Hashtable();

            try
            {
                if (IsNew) throw new Exception("Delete cannot be performed, UserWebsiteID is missing");

                if (UserInfo.LastVisitedUserWebsiteID == UserWebsiteID)
                {
                    UserInfo.LastVisitedUserWebsiteID = string.Empty;
                    UserInfo.Update(objConn, objTran);
                }

                if (ShoppingCart != null) ShoppingCart.Delete(objConn, objTran);

                foreach (UserAccount _UserAccount in UserAccounts)
                {
                    _UserAccount.Delete(objConn, objTran);
                }

                dicDParam["UserWebsiteID"] = UserWebsiteID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "UserWebsite"), objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicDParam = null;
            }
            return true;
        }

        public bool ObjectAlreadyExists()
        {

            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                        "FROM UserWebsite (NOLOCK) p " +
                        "WHERE " +
                        "(" +
                        "  (p.UserInfoID=" + Database.HandleQuote(UserInfoID) + " AND p.WebsiteID=" + Database.HandleQuote(WebsiteID) + ")" +
                        ") ";

            if (!string.IsNullOrEmpty(UserWebsiteID)) strSQL += "AND p.UserWebsiteID<>" + Database.HandleQuote(UserWebsiteID);
            return Database.HasRows(strSQL);
        }

        public static UserWebsite GetUserWebsite(UserWebsiteFilter Filter)
        {
            List<UserWebsite> objUserWebsites = null;
            UserWebsite objReturn = null;

            try
            {
                objUserWebsites = GetUserWebsites(Filter);
                if (objUserWebsites != null && objUserWebsites.Count >= 1) objReturn = objUserWebsites[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objUserWebsites = null;
            }
            return objReturn;
        }

        public static List<UserWebsite> GetUserWebsites()
        {
            int intTotalCount = 0;
            return GetUserWebsites(null, null, null, out intTotalCount);
        }

        public static List<UserWebsite> GetUserWebsites(UserWebsiteFilter Filter)
        {
            int intTotalCount = 0;
            return GetUserWebsites(Filter, null, null, out intTotalCount);
        }

        public static List<UserWebsite> GetUserWebsites(UserWebsiteFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetUserWebsites(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<UserWebsite> GetUserWebsites(UserWebsiteFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<UserWebsite> objReturn = null;
            UserWebsite objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<UserWebsite>();

                strSQL = "SELECT uw.* " +
                         "FROM UserWebsite (NOLOCK) uw " +
                         "INNER JOIN UserInfo (NOLOCK) u ON uw.UserInfoID=u.UserInfoID " +
                         "WHERE 1=1  ";

                if (Filter != null)
                {
                    if (Filter.UserWebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserWebsiteID, "uw.UserWebsiteID");
                    if (Filter.GUID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.GUID, "uw.GUID");
                    if (Filter.UserInfoID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserInfoID, "uw.UserInfoID");
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "uw.WebsiteID");
                    if (Filter.CustomerInternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomerInternalID, "uw.CustomerInternalID");
                    if (Filter.IsAdmin != null) strSQL += "AND uw.IsAdmin=" + Database.HandleQuote(Convert.ToInt32(Filter.IsAdmin.Value).ToString());
                    if (Filter.IsPendingApproval != null) strSQL += "AND uw.IsPendingApproval=" + Database.HandleQuote(Convert.ToInt32(Filter.IsPendingApproval.Value).ToString());
                    if (Filter.WebsiteManagement != null) strSQL += "AND uw.WebsiteManagement=" + Database.HandleQuote(Convert.ToInt32(Filter.WebsiteManagement.Value).ToString());
                    if (Filter.StoreManagement != null) strSQL += "AND uw.StoreManagement=" + Database.HandleQuote(Convert.ToInt32(Filter.StoreManagement.Value).ToString());
                    if (Filter.UserManagement != null) strSQL += "AND uw.UserManagement=" + Database.HandleQuote(Convert.ToInt32(Filter.UserManagement.Value).ToString());
                    if (Filter.GroupManagement != null) strSQL += "AND uw.GroupManagement=" + Database.HandleQuote(Convert.ToInt32(Filter.GroupManagement.Value).ToString());
                    if (Filter.TabManagement != null) strSQL += "AND uw.TabManagement=" + Database.HandleQuote(Convert.ToInt32(Filter.TabManagement.Value).ToString());
                    if (Filter.ItemManagement != null) strSQL += "AND uw.ItemManagement=" + Database.HandleQuote(Convert.ToInt32(Filter.ItemManagement.Value).ToString());
                    if (Filter.BudgetManagement != null) strSQL += "AND uw.BudgetManagement=" + Database.HandleQuote(Convert.ToInt32(Filter.BudgetManagement.Value).ToString());
                    if (Filter.IsBudgetAdmin != null) strSQL += "AND uw.IsBudgetAdmin=" + Database.HandleQuote(Convert.ToInt32(Filter.IsBudgetAdmin.Value).ToString());
                    if (Filter.OrderManagement != null) strSQL += "AND uw.OrderManagement=" + Database.HandleQuote(Convert.ToInt32(Filter.OrderManagement.Value).ToString());
                    if (Filter.CreditCardManagement != null) strSQL += "AND uw.CreditCardManagement=" + Database.HandleQuote(Convert.ToInt32(Filter.CreditCardManagement.Value).ToString());
                    if (Filter.ShippingManagement != null) strSQL += "AND uw.ShippingManagement=" + Database.HandleQuote(Convert.ToInt32(Filter.ShippingManagement.Value).ToString());
                    if (Filter.MessageManagement != null) strSQL += "AND uw.MessageManagement=" + Database.HandleQuote(Convert.ToInt32(Filter.MessageManagement.Value).ToString());
                    if (Filter.OptInForNotification != null) strSQL += "AND uw.OptInForNotification=" + Database.HandleQuote(Convert.ToInt32(Filter.OptInForNotification.Value).ToString());
                    if (Filter.AccountIDs != null && Filter.AccountIDs.Count > 0)
                    {
                        strSQL += "AND uw.UserWebsiteID IN (SELECT UserWebsiteID FROM UserAccount (NOLOCK) WHERE AccountID IN (" + String.Join(",", Filter.AccountIDs) + ") )";
                    }
                    if (Filter.FirstName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.FirstName, "u.FirstName");
                    if (Filter.LastName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.LastName, "u.LastName");
                    if (Filter.EmailAddress != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EmailAddress, "u.EmailAddress");
                    if (Filter.EmployeeID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EmployeeID, "uw.EmployeeID");
                    if (Filter.HireDate != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.HireDate, "HireDate");
                    if (Filter.SendWelcomeEmail != null) strSQL += "AND uw.SendWelcomeEmail=" + Database.HandleQuote(Convert.ToInt32(Filter.SendWelcomeEmail.Value).ToString());
                    if (Filter.Inactive != null) strSQL += "AND uw.Inactive=" + Database.HandleQuote(Convert.ToInt32(Filter.Inactive.Value).ToString());

                    if (Filter.PunchoutSessionID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PunchoutSessionID, "uw.PunchoutSessionID");
                    if (Filter.PunchoutGUID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PunchoutGUID, "uw.PunchoutGUID");
                    if (Filter.SMSMobileNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SMSMobileNumber, "SMSMobileNumber");
                    //if (Filter.SMSOptIn != null) strSQL += "AND SMSOptIn=" + Database.HandleQuote(Convert.ToInt32(Filter.SMSOptIn.Value).ToString());

                    if (Filter.EnableSMSOptIn != null) strSQL += "AND EnableSMSOptIn=" + Database.HandleQuote(Convert.ToInt32(Filter.EnableSMSOptIn.Value).ToString());
                    if (Filter.EnableEmailOptIn != null) strSQL += "AND EnableEmailOptIn=" + Database.HandleQuote(Convert.ToInt32(Filter.EnableEmailOptIn.Value).ToString());

                    if (Filter.MarketingWelcome != null) strSQL += "AND MarketingWelcome=" + Database.HandleQuote(Convert.ToInt32(Filter.MarketingWelcome.Value).ToString());
                    if (Filter.MarketingOutreach != null) strSQL += "AND MarketingOutreach=" + Database.HandleQuote(Convert.ToInt32(Filter.MarketingOutreach.Value).ToString());
                    if (Filter.MarketingCartAbandonment != null) strSQL += "AND MarketingCartAbandonment=" + Database.HandleQuote(Convert.ToInt32(Filter.MarketingCartAbandonment.Value).ToString());
                    if (Filter.MarketingBudgetExpiration != null) strSQL += "AND MarketingBudgetExpiration=" + Database.HandleQuote(Convert.ToInt32(Filter.MarketingBudgetExpiration.Value).ToString());
                    if (Filter.MarketingBudgetRenewal != null) strSQL += "AND MarketingBudgetRenewal=" + Database.HandleQuote(Convert.ToInt32(Filter.MarketingBudgetRenewal.Value).ToString());

                    if (Filter.MarketingWelcomeSentOn != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.MarketingWelcomeSentOn, "MarketingWelcomeSentOn");
                    if (Filter.MarketingOutreachSentOn != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.MarketingOutreachSentOn, "MarketingOutreachSentOn");
                    if (Filter.MarketingCartAbandonmentSentOn != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.MarketingCartAbandonmentSentOn, "MarketingCartAbandonmentSentOn");
                    if (Filter.MarketingBudgetExpirationSentOn != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.MarketingBudgetExpirationSentOn, "MarketingBudgetExpirationSentOn");
                    if (Filter.MarketingBudgetRenewalSentOn != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.MarketingBudgetRenewalSentOn, "MarketingBudgetRenewalSentOn");

                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "UserWebsiteID" : Utility.CustomSorting.GetSortExpression(typeof(UserWebsite), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new UserWebsite(objData.Tables[0].Rows[i]);
                        objNew.IsLoaded = true;
                        objReturn.Add(objNew);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
            }
            return objReturn;
        }
    }
}
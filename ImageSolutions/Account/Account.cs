using ImageSolutions.Account;
using ImageSolutions.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageSolutions.Budget;

namespace ImageSolutions.Account
{
    public class Account : ISBase.BaseClass
    {
        public string AccountID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(AccountID); } }
        public string WebsiteID { get; set; }
        public string CustomerInternalID { get; set; }
        public string ParentID { get; set; }
        public string AccountName { get; set; }
        public string RegistrationKey { get; set; }
        public string SiteNumber { get; set; }
        public string StoreNumber { get; set; }
        public string DefaultShippingAddressBookID { get; set; }
        public string DefaultWebsiteGroupID { get; set; }
        public bool IsPendingApproval { get; set; }
        //public bool IsTaxExempt { get; set; }
        public bool GetSubAccountNotification { get; set; }
        public string PersonalizationApproverUserWebsiteID { get; set; }
        public string PersonalizationApprover2UserWebsiteID { get; set; }
        public bool AutoAssignBudget { get; set; }
        public string BudgetSetting { get; set; }
        public bool DoNotAllowCreditCard { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

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
        private UserWebsite mPersonalizationApproverUserWebsite = null;
        public UserWebsite PersonalizationApproverUserWebsite
        {
            get
            {
                if (mPersonalizationApproverUserWebsite == null && !string.IsNullOrEmpty(PersonalizationApproverUserWebsiteID))
                {
                    mPersonalizationApproverUserWebsite = new UserWebsite(PersonalizationApproverUserWebsiteID);
                }
                return mPersonalizationApproverUserWebsite;
            }
        }

        private Address.AddressBook mDefaultShippingAddressBook = null;
        public Address.AddressBook DefaultShippingAddressBook
        {
            get
            {
                if (mDefaultShippingAddressBook == null && !string.IsNullOrEmpty(DefaultShippingAddressBookID))
                {
                    mDefaultShippingAddressBook = new Address.AddressBook(DefaultShippingAddressBookID);
                }
                return mDefaultShippingAddressBook;
            }
            set
            {
                mDefaultShippingAddressBook = value;
            }
        }

        private string mAccountNamePath = string.Empty;
        public string AccountNamePath
        {
            get
            {
                if (string.IsNullOrEmpty(mAccountNamePath) && !string.IsNullOrEmpty(AccountName))
                {
                    if (ParentAccount != null)
                        mAccountNamePath = ParentAccount.AccountNamePath + " -> " + AccountName;
                    else
                        mAccountNamePath = AccountName;
                }
                return mAccountNamePath;
            }
        }

        private Website.Website mWebsite = null;
        public Website.Website Website
        {
            get
            {
                if (mWebsite == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    mWebsite = new Website.Website(WebsiteID);
                }
                return mWebsite;
            }
        }

        private Account mParentAccount = null;
        public Account ParentAccount
        {
            get
            {
                if (mParentAccount == null && !string.IsNullOrEmpty(ParentID))
                {
                    mParentAccount = new Account(ParentID);
                }
                return mParentAccount;
            }
        }

        private List<Account> mChildAccounts = null;
        public List<Account> ChildAccounts
        {
            get
            {
                if (mChildAccounts == null && !string.IsNullOrEmpty(AccountID))
                {
                    AccountFilter objFilter = null;

                    try
                    {
                        objFilter = new AccountFilter();
                        objFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ParentID.SearchString = AccountID;
                        mChildAccounts = Account.GetAccounts(objFilter);
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
                return mChildAccounts;
            }
        }

        private List<User.UserAccount> mUserAccounts = null;
        public List<User.UserAccount> UserAccounts
        {
            get
            {
                if (mUserAccounts == null && !string.IsNullOrEmpty(AccountID))
                {
                    User.UserAccountFilter objFilter = null;

                    try
                    {
                        objFilter = new UserAccountFilter();
                        objFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.AccountID.SearchString = AccountID;
                        mUserAccounts = User.UserAccount.GetUserAccounts(objFilter);
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

        public string EmailAddress
        {
            get
            {
                if (UserAccounts != null && UserAccounts.Count > 0)
                    return UserAccounts[0].UserWebsite.UserInfo.EmailAddress;
                else
                    return string.Empty;
            }
        }

        private List<User.UserInfo> mUserInfos = null;
        public List<User.UserInfo> UserInfos
        {
            get
            {
                if (mUserInfos == null && UserAccounts != null)
                {
                    mUserInfos = new List<UserInfo>();

                    foreach (UserAccount objUserAccount in UserAccounts)
                    {
                        if (!mUserInfos.Exists(m => m.UserInfoID == objUserAccount.UserWebsite.UserInfoID))
                        {
                            mUserInfos.Add(objUserAccount.UserWebsite.UserInfo);
                        }
                    }
                }
                return mUserInfos;
            }
        }

        private List<Account> mSubAccounts = null;
        public List<Account> SubAccounts
        {
            get
            {
                if (ChildAccounts != null)
                {
                    mSubAccounts = new List<Account>();
                    foreach (Account _Account in ChildAccounts)
                    {
                        if (!mSubAccounts.Exists(m => m.AccountID == _Account.AccountID))
                        {
                            mSubAccounts.Add(_Account);
                        }
                        AddChildAccount(_Account.ChildAccounts, ref mSubAccounts);
                    }
                }
                return mSubAccounts;
            }
        }

        public void AddChildAccount(List<Account> childaccounts, ref List<Account> subaccounts)
        {
            foreach (Account _Account in childaccounts)
            {
                if (!mSubAccounts.Exists(m => m.AccountID == _Account.AccountID))
                {
                    subaccounts.Add(_Account);
                }
                AddChildAccount(_Account.ChildAccounts, ref subaccounts);

            }
        }

        public Account()
        {
        }
        public Account(string AccountID)
        {
            this.AccountID = AccountID;
            Load();
        }
        public Account(DataRow objRow)
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
                         "FROM Account (NOLOCK) " +
                         "WHERE AccountID=" + Database.HandleQuote(AccountID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("AccountID=" + AccountID + " is not found");
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
                if (objColumns.Contains("AccountID")) AccountID = Convert.ToString(objRow["AccountID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("CustomerInternalID")) CustomerInternalID = Convert.ToString(objRow["CustomerInternalID"]);
                if (objColumns.Contains("ParentID")) ParentID = Convert.ToString(objRow["ParentID"]);
                if (objColumns.Contains("AccountName")) AccountName = Convert.ToString(objRow["AccountName"]);
                if (objColumns.Contains("RegistrationKey")) RegistrationKey = Convert.ToString(objRow["RegistrationKey"]);
                if (objColumns.Contains("SiteNumber")) SiteNumber = Convert.ToString(objRow["SiteNumber"]);
                if (objColumns.Contains("StoreNumber")) StoreNumber = Convert.ToString(objRow["StoreNumber"]);
                if (objColumns.Contains("DefaultShippingAddressBookID")) DefaultShippingAddressBookID = Convert.ToString(objRow["DefaultShippingAddressBookID"]);
                if (objColumns.Contains("DefaultWebsiteGroupID")) DefaultWebsiteGroupID = Convert.ToString(objRow["DefaultWebsiteGroupID"]);
                if (objColumns.Contains("IsPendingApproval")) IsPendingApproval = Convert.ToBoolean(objRow["IsPendingApproval"]);
                //if (objColumns.Contains("IsTaxExempt")) IsTaxExempt = Convert.ToBoolean(objRow["IsTaxExempt"]);
                if (objColumns.Contains("GetSubAccountNotification")) GetSubAccountNotification = Convert.ToBoolean(objRow["GetSubAccountNotification"]);
                if (objColumns.Contains("PersonalizationApproverUserWebsiteID")) PersonalizationApproverUserWebsiteID = Convert.ToString(objRow["PersonalizationApproverUserWebsiteID"]);
                if (objColumns.Contains("PersonalizationApprover2UserWebsiteID")) PersonalizationApprover2UserWebsiteID = Convert.ToString(objRow["PersonalizationApprover2UserWebsiteID"]);
                if (objColumns.Contains("AutoAssignBudget")) AutoAssignBudget = Convert.ToBoolean(objRow["AutoAssignBudget"]);
                if (objColumns.Contains("BudgetSetting")) BudgetSetting = Convert.ToString(objRow["BudgetSetting"]);
                if (objColumns.Contains("DoNotAllowCreditCard")) DoNotAllowCreditCard = Convert.ToBoolean(objRow["DoNotAllowCreditCard"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(AccountID)) throw new Exception("Missing AccountID in the datarow");
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
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (string.IsNullOrEmpty(WebsiteID)) throw new Exception("WebsiteID is required");
                if (string.IsNullOrEmpty(AccountName)) throw new Exception("AccountName is required");
                if (!IsNew) throw new Exception("Create cannot be performed, AccountID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["CustomerInternalID"] = CustomerInternalID;
                dicParam["ParentID"] = ParentID;
                dicParam["AccountName"] = AccountName;
                dicParam["RegistrationKey"] = RegistrationKey;
                dicParam["SiteNumber"] = SiteNumber;
                dicParam["StoreNumber"] = StoreNumber;
                dicParam["DefaultShippingAddressBookID"] = DefaultShippingAddressBookID;
                dicParam["DefaultWebsiteGroupID"] = DefaultWebsiteGroupID;
                dicParam["IsPendingApproval"] = IsPendingApproval;
                //dicParam["IsTaxExempt"] = IsTaxExempt;
                dicParam["GetSubAccountNotification"] = GetSubAccountNotification;
                dicParam["PersonalizationApproverUserWebsiteID"] = PersonalizationApproverUserWebsiteID;
                dicParam["PersonalizationApprover2UserWebsiteID"] = PersonalizationApprover2UserWebsiteID;                
                dicParam["AutoAssignBudget"] = AutoAssignBudget;
                dicParam["BudgetSetting"] = BudgetSetting;
                dicParam["DoNotAllowCreditCard"] = DoNotAllowCreditCard;
                dicParam["CreatedBy"] = CreatedBy;
                AccountID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Account"), objConn, objTran).ToString();

                if (DefaultShippingAddressBook != null)
                {
                    if (DefaultShippingAddressBook.IsNew)
                    {
                        DefaultShippingAddressBook.AccountID = AccountID;
                        DefaultShippingAddressBook.CreatedBy = CreatedBy;
                        DefaultShippingAddressBook.Create(objConn, objTran);

                        dicParam = new Hashtable();
                        dicWParam = new Hashtable();
                        dicParam["DefaultShippingAddressBookID"] = DefaultShippingAddressBook.AddressBookID;
                        dicWParam["AccountID"] = AccountID;
                        Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Account"), objConn, objTran);
                    }
                    else
                    {
                        DefaultShippingAddressBook.Update(objConn, objTran);
                    }
                }
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
                if (string.IsNullOrEmpty(AccountID)) throw new Exception("AccountID is required");
                if (string.IsNullOrEmpty(WebsiteID)) throw new Exception("WebsiteID is required");
                if (string.IsNullOrEmpty(AccountName)) throw new Exception("AccountName is required");
                if (IsNew) throw new Exception("Update cannot be performed, AccountID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CustomerInternalID"] = CustomerInternalID;
                dicParam["ParentID"] = ParentID;
                dicParam["AccountName"] = AccountName;
                dicParam["WebsiteID"] = WebsiteID;
                dicParam["RegistrationKey"] = RegistrationKey;
                dicParam["SiteNumber"] = SiteNumber;
                dicParam["StoreNumber"] = StoreNumber;
                dicParam["DefaultShippingAddressBookID"] = DefaultShippingAddressBookID;
                dicParam["DefaultWebsiteGroupID"] = DefaultWebsiteGroupID;
                dicParam["IsPendingApproval"] = IsPendingApproval;
                //dicParam["IsTaxExempt"] = IsTaxExempt;
                dicParam["GetSubAccountNotification"] = GetSubAccountNotification;
                dicParam["PersonalizationApproverUserWebsiteID"] = PersonalizationApproverUserWebsiteID;
                dicParam["PersonalizationApprover2UserWebsiteID"] = PersonalizationApprover2UserWebsiteID;
                dicParam["AutoAssignBudget"] = AutoAssignBudget;
                dicParam["BudgetSetting"] = BudgetSetting;
                dicParam["DoNotAllowCreditCard"] = DoNotAllowCreditCard;
                dicWParam["AccountID"] = AccountID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Account"), objConn, objTran);

                if (DefaultShippingAddressBook != null)
                {
                    if (DefaultShippingAddressBook.IsNew)
                    {
                        DefaultShippingAddressBook.AccountID = AccountID;
                        DefaultShippingAddressBook.Create(objConn, objTran);

                        dicParam = new Hashtable();
                        dicWParam = new Hashtable();
                        dicParam["DefaultShippingAddressBookID"] = DefaultShippingAddressBook.AddressBookID;
                        dicWParam["AccountID"] = AccountID;
                        Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Account"), objConn, objTran);
                    }
                    else
                    {
                        DefaultShippingAddressBook.Update(objConn, objTran);
                    }
                }
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
                if (IsNew) throw new Exception("Delete cannot be performed, AccountID is missing");
                if (UserAccounts != null && UserAccounts.Count > 0) throw new Exception("Delete cannot be performed, user profile has been assigned to this account");

                foreach (Account _Account in ChildAccounts)
                {
                    _Account.Delete(objConn, objTran);
                }

                dicDParam["AccountID"] = AccountID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Account"), objConn, objTran);
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
                        "FROM Account (NOLOCK) p " +
                        "WHERE " +
                        "(" +
                        "  (p.AccountName=" + Database.HandleQuote(AccountName) + " AND p.ParentID=" + Database.HandleQuote(ParentID) + " AND p.WebsiteID=" + Database.HandleQuote(WebsiteID) + ")" +
                        "  OR " +
                        "  (ISNULL(" + Database.HandleQuote(RegistrationKey) + ",'') != '' AND " + "p.RegistrationKey=" + Database.HandleQuote(RegistrationKey) + " AND p.WebsiteID=" + Database.HandleQuote(WebsiteID) + ")" +
                        ") ";

            if (!string.IsNullOrEmpty(AccountID)) strSQL += "AND p.AccountID<>" + Database.HandleQuote(AccountID);
            return Database.HasRows(strSQL);
        }

        public static Account GetAccount(AccountFilter Filter)
        {
            List<Account> objAccounts = null;
            Account objReturn = null;

            try
            {
                objAccounts = GetAccounts(Filter);
                if (objAccounts != null && objAccounts.Count >= 1) objReturn = objAccounts[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAccounts = null;
            }
            return objReturn;
        }

        public static List<Account> GetAccounts()
        {
            int intTotalCount = 0;
            return GetAccounts(null, null, null, out intTotalCount);
        }

        public static List<Account> GetAccounts(AccountFilter Filter)
        {
            int intTotalCount = 0;
            return GetAccounts(Filter, null, null, out intTotalCount);
        }

        public static List<Account> GetAccounts(AccountFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAccounts(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Account> GetAccounts(AccountFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Account> objReturn = null;
            Account objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Account>();

                strSQL = "SELECT * " +
                         "FROM Account (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.ParentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ParentID, "ParentID");
                    if (Filter.AccountName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.AccountName, "AccountName");
                    if (Filter.AccountNameExact != null) strSQL += " AND AccountName=" + Database.HandleQuote(Convert.ToString(Filter.AccountNameExact));
                    if (Filter.RegistrationKey != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.RegistrationKey, "RegistrationKey");
                    if (Filter.StoreNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.StoreNumber, "StoreNumber");
                    if (Filter.IsPendingApproval != null) strSQL += "AND IsPendingApproval=" + Database.HandleQuote(Convert.ToInt32(Filter.IsPendingApproval.Value).ToString());
                    if (Filter.AccountIDs != null && Filter.AccountIDs.Count > 0)
                    {
                        strSQL += "AND AccountID IN (" + String.Join(",", Filter.AccountIDs) + ") ";
                    }
                    if (Filter.EmailAddress != null)
                    {
                        strSQL += "AND AccountID IN (SELECT AccountID FROM UserAccount WHERE UserWebsiteID IN (SELECT UserWebsiteID FROM UserWebsite WHERE UserInfoID IN (SELECT UserInfoID FROM UserInfo WHERE EmailAddress=" + Database.HandleQuote(Filter.EmailAddress.SearchString) + "))) ";
                    }
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "AccountID" : Utility.CustomSorting.GetSortExpression(typeof(Account), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Account(objData.Tables[0].Rows[i]);
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

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
    public class UserAccount : ISBase.BaseClass
    {
        public string UserAccountID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(UserAccountID); } }
        public string GUID { get; set; }
        public string UserWebsiteID { get; set; }
        public string AccountID { get; set; }
        public string WebsiteGroupID { get; set; }
        public bool IsPrimary { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        private User.UserWebsite mUserWebsite = null;
        public User.UserWebsite UserWebsite
        {
            get
            {
                if (mUserWebsite == null && !string.IsNullOrEmpty(UserWebsiteID))
                {
                    mUserWebsite = new UserWebsite(UserWebsiteID);
                }
                return mUserWebsite;
            }
        }

        private Account.Account mAccount = null;
        public Account.Account Account
        {
            get
            {
                if (mAccount == null && !string.IsNullOrEmpty(AccountID))
                {
                    mAccount = new Account.Account(AccountID);
                }
                return mAccount;
            }
        }

        private Website.WebsiteGroup mWebsiteGroup = null;
        public Website.WebsiteGroup WebsiteGroup
        {
            get
            {
                if (mWebsiteGroup == null && !string.IsNullOrEmpty(WebsiteGroupID))
                {
                    mWebsiteGroup = new Website.WebsiteGroup(WebsiteGroupID);
                }
                return mWebsiteGroup;
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
        private List<Item.ItemWebsite> mItemWebsites = null;
        public List<Item.ItemWebsite> ItemWebsites
        {
            get
            {
                if (mItemWebsites == null && UserWebsite != null && !string.IsNullOrEmpty(UserWebsite.WebsiteID))
                {
                    Item.ItemWebsiteFilter objFilter = null;
                    try
                    {
                        objFilter = new Item.ItemWebsiteFilter();
                        objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteID.SearchString = UserWebsite.WebsiteID;
                        mItemWebsites = Item.ItemWebsite.GetItemWebsites(objFilter);
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
                return mItemWebsites;
            }
        }
        private List<Item.MyGroupItem> mMyGroupItems = null;
        public List<Item.MyGroupItem> MyGroupItems
        {
            get
            {
                if (mMyGroupItems == null && ItemWebsites != null)
                {
                    mMyGroupItems = new List<Item.MyGroupItem>();

                    foreach (Item.ItemWebsite _ItemWebsite in ItemWebsites)
                    {
                        mMyGroupItems.Add(new Item.MyGroupItem(WebsiteGroupID, _ItemWebsite.ItemID));
                    }
                }
                return mMyGroupItems;
            }
        }
        public UserAccount()
        {
        }
        public UserAccount(string UserAccountID)
        {
            this.UserAccountID = UserAccountID;
            Load();
        }
        public UserAccount(DataRow objRow)
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
                         "FROM UserAccount (NOLOCK) " +
                         "WHERE UserAccountID=" + Database.HandleQuote(UserAccountID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("UserAccountID=" + UserAccountID + " is not found");
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
                         "FROM UserAccount (NOLOCK) " +
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

                if (objColumns.Contains("UserAccountID")) UserAccountID = Convert.ToString(objRow["UserAccountID"]);
                if (objColumns.Contains("GUID")) GUID = Convert.ToString(objRow["GUID"]);
                if (objColumns.Contains("UserWebsiteID")) UserWebsiteID = Convert.ToString(objRow["UserWebsiteID"]);
                if (objColumns.Contains("AccountID")) AccountID = Convert.ToString(objRow["AccountID"]);
                if (objColumns.Contains("WebsiteGroupID")) WebsiteGroupID = Convert.ToString(objRow["WebsiteGroupID"]);
                if (objColumns.Contains("IsPrimary")) IsPrimary = Convert.ToBoolean(objRow["IsPrimary"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(UserAccountID)) throw new Exception("Missing UserAccountID in the datarow");
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
                if (UserWebsiteID == null) throw new Exception("UserWebsiteID is required");
                if (AccountID == null) throw new Exception("AccountID is required");
                if (WebsiteGroupID == null) throw new Exception("WebsiteGroupID is required");
                //if (ExistsUserInfoIDWebsiteGroupID()) throw new Exception("User is already assigned to this group");
                if (!IsNew) throw new Exception("Create cannot be performed, UserAccountID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["AccountID"] = AccountID;
                dicParam["WebsiteGroupID"] = WebsiteGroupID;
                dicParam["IsPrimary"] = IsPrimary;
                dicParam["CreatedBy"] = CreatedBy;

                UserAccountID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "UserAccount"), objConn, objTran).ToString();

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
                if (UserWebsiteID == null) throw new Exception("UserWebsiteID is required");
                if (AccountID == null) throw new Exception("AccountID is required");
                if (WebsiteGroupID == null) throw new Exception("WebsiteGroupID is required");
                //if (ExistsUserInfoIDWebsiteGroupID()) throw new Exception("User is already assigned to this group");
                if (IsNew) throw new Exception("Update cannot be performed, UserAccountID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["AccountID"] = AccountID;
                dicParam["WebsiteGroupID"] = WebsiteGroupID;
                dicParam["IsPrimary"] = IsPrimary;
                dicWParam["UserAccountID"] = UserAccountID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "UserAccount"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, UserAccountID is missing");

                dicDParam["UserAccountID"] = UserAccountID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "UserAccount"), objConn, objTran);
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

        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                     "FROM UserAccount (NOLOCK) p " +
                     "WHERE " +
                     "(" +
                     "  (p.UserWebsiteID=" + Database.HandleQuote(UserWebsiteID) + " AND p.AccountID=" + Database.HandleQuote(AccountID) + " AND p.WebsiteGroupID=" + Database.HandleQuote(WebsiteGroupID) + ")" +
                     ") ";

            if (!string.IsNullOrEmpty(UserAccountID)) strSQL += "AND p.UserAccountID<>" + Database.HandleQuote(UserAccountID);
            return Database.HasRows(strSQL);
        }

        public static UserAccount GetUserAccount(UserAccountFilter Filter)
        {
            List<UserAccount> objUserAccounts = null;
            UserAccount objReturn = null;

            try
            {
                objUserAccounts = GetUserAccounts(Filter);
                if (objUserAccounts != null && objUserAccounts.Count >= 1) objReturn = objUserAccounts[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objUserAccounts = null;
            }
            return objReturn;
        }

        public static List<UserAccount> GetUserAccounts()
        {
            int intTotalCount = 0;
            return GetUserAccounts(null, null, null, out intTotalCount);
        }

        public static List<UserAccount> GetUserAccounts(UserAccountFilter Filter)
        {
            int intTotalCount = 0;
            return GetUserAccounts(Filter, null, null, out intTotalCount);
        }

        public static List<UserAccount> GetUserAccounts(UserAccountFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetUserAccounts(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<UserAccount> GetUserAccounts(UserAccountFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<UserAccount> objReturn = null;
            UserAccount objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<UserAccount>();

                strSQL = "SELECT * " +
                         "FROM UserAccount (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.UserAccountID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserAccountID, "UserAccountID");
                    if (Filter.UserInfoID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserInfoID, "UserInfoID");
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.AccountID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.AccountID, "AccountID");
                    if (Filter.UserWebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserWebsiteID, "UserWebsiteID");
                    if (Filter.WebsiteGroupID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteGroupID, "WebsiteGroupID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "UserAccountID" : Utility.CustomSorting.GetSortExpression(typeof(UserAccount), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else strSQL += "ORDER BY IsPrimary DESC";

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new UserAccount(objData.Tables[0].Rows[i]);
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

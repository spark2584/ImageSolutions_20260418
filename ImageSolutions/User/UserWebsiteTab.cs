using ImageSolutions.User;
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
    public class UserWebsiteTab : ISBase.BaseClass
    {
        public string UserWebsiteTabID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(UserWebsiteTabID); } }
        public string UserWebsiteID { get; set; }
        public string WebsiteTabID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        private UserWebsite mUserWebsite = null;
        public UserWebsite UserWebsite
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
        private Website.WebsiteTab mWebsiteTab = null;
        public Website.WebsiteTab WebsiteTab
        {
            get
            {
                if (mWebsiteTab == null && !string.IsNullOrEmpty(WebsiteTabID))
                {
                    mWebsiteTab = new Website.WebsiteTab(WebsiteTabID);
                }
                return mWebsiteTab;
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

        public UserWebsiteTab()
        {
        }
        public UserWebsiteTab(string UserWebsiteTabID)
        {
            this.UserWebsiteTabID = UserWebsiteTabID;
            Load();
        }
        public UserWebsiteTab(DataRow objRow)
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
                         "FROM UserWebsiteTab (NOLOCK) " +
                         "WHERE UserWebsiteTabID=" + Database.HandleQuote(UserWebsiteTabID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("UserWebsiteTabID=" + UserWebsiteTabID + " is not found");
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

                if (objColumns.Contains("UserWebsiteTabID")) UserWebsiteTabID = Convert.ToString(objRow["UserWebsiteTabID"]);
                if (objColumns.Contains("UserWebsiteID")) UserWebsiteID = Convert.ToString(objRow["UserWebsiteID"]);
                if (objColumns.Contains("WebsiteTabID")) WebsiteTabID = Convert.ToString(objRow["WebsiteTabID"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(UserWebsiteTabID)) throw new Exception("Missing UserWebsiteTabID in the datarow");
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
        protected bool ExistsUserWebsiteIDWebsiteTabID()
        {
            bool _ret = false;

            try
            {
                UserWebsiteTab UserWebsiteTab = new UserWebsiteTab();
                UserWebsiteTabFilter UserWebsiteTabFilter = new UserWebsiteTabFilter();
                UserWebsiteTabFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                UserWebsiteTabFilter.UserWebsiteID.SearchString = UserWebsiteID;
                UserWebsiteTabFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                UserWebsiteTabFilter.WebsiteTabID.SearchString = WebsiteTabID;
                UserWebsiteTab = UserWebsiteTab.GetUserWebsiteTab(UserWebsiteTabFilter);
                if (UserWebsiteTab != null && !string.IsNullOrEmpty(UserWebsiteTab.UserWebsiteTabID) && UserWebsiteTab.UserWebsiteTabID != UserWebsiteTabID)
                {
                    _ret = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _ret;
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
                if (WebsiteTabID == null) throw new Exception("WebsiteTabID is required");

                if (ExistsUserWebsiteIDWebsiteTabID()) throw new Exception("Tab is already assigned to this user");

                if (!IsNew) throw new Exception("Create cannot be performed, UserWebsiteTabID already exists");

                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["WebsiteTabID"] = WebsiteTabID;
                dicParam["CreatedBy"] = CreatedBy;

                UserWebsiteTabID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "UserWebsiteTab"), objConn, objTran).ToString();

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
                if (WebsiteTabID == null) throw new Exception("WebsiteID is required");

                if (ExistsUserWebsiteIDWebsiteTabID()) throw new Exception("Tab is already assigned to this user");

                if (IsNew) throw new Exception("Update cannot be performed, UserWebsiteTabID is missing");

                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["WebsiteTabID"] = WebsiteTabID;
                dicWParam["UserWebsiteTabID"] = UserWebsiteTabID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "UserWebsiteTab"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, UserWebsiteTabID is missing");

                dicDParam["UserWebsiteTabID"] = UserWebsiteTabID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "UserWebsiteTab"), objConn, objTran);
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
            return false;
        }

        public static UserWebsiteTab GetUserWebsiteTab(UserWebsiteTabFilter Filter)
        {
            List<UserWebsiteTab> objUserWebsiteTabs = null;
            UserWebsiteTab objReturn = null;

            try
            {
                objUserWebsiteTabs = GetUserWebsiteTabs(Filter);
                if (objUserWebsiteTabs != null && objUserWebsiteTabs.Count >= 1) objReturn = objUserWebsiteTabs[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objUserWebsiteTabs = null;
            }
            return objReturn;
        }

        public static List<UserWebsiteTab> GetUserWebsiteTabs()
        {
            int intTotalCount = 0;
            return GetUserWebsiteTabs(null, null, null, out intTotalCount);
        }

        public static List<UserWebsiteTab> GetUserWebsiteTabs(UserWebsiteTabFilter Filter)
        {
            int intTotalCount = 0;
            return GetUserWebsiteTabs(Filter, null, null, out intTotalCount);
        }

        public static List<UserWebsiteTab> GetUserWebsiteTabs(UserWebsiteTabFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetUserWebsiteTabs(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<UserWebsiteTab> GetUserWebsiteTabs(UserWebsiteTabFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<UserWebsiteTab> objReturn = null;
            UserWebsiteTab objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<UserWebsiteTab>();

                strSQL = "SELECT * " +
                         "FROM UserWebsiteTab (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteTabID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteTabID, "WebsiteTabID");
                    if (Filter.UserWebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserWebsiteID, "UserWebsiteID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "UserWebsiteTabID" : Utility.CustomSorting.GetSortExpression(typeof(UserWebsiteTab), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new UserWebsiteTab(objData.Tables[0].Rows[i]);
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

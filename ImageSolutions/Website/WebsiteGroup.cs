using ImageSolutions.Customer;
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

namespace ImageSolutions.Website
{
    public class WebsiteGroup : ISBase.BaseClass
    {
        public string WebsiteGroupID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(WebsiteGroupID); } }
        public string WebsiteID { get; set; }
        public string GroupName { get; set; }
        public string LogoPath { get; set; }
        public string HomePageImagePath { get; set; }
        public string HomePageMobileImagePath { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        private List<WebsiteGroupTab> mWebsiteGroupTabs = null;
        public List<WebsiteGroupTab> WebsiteGroupTabs
        {
            get
            {
                WebsiteGroupTabFilter objFilter = null;

                try
                {
                    if (mWebsiteGroupTabs == null && !string.IsNullOrEmpty(WebsiteGroupID))
                    {
                        objFilter = new WebsiteGroupTabFilter();
                        objFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteGroupID.SearchString = WebsiteGroupID;
                        mWebsiteGroupTabs =  WebsiteGroupTab.GetWebsiteGroupTabs(objFilter);
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
                return mWebsiteGroupTabs;
            }
        }

        private List<UserAccount> mUserAccounts = null;
        public List<UserAccount> UserAccounts
        {
            get
            {
                UserAccountFilter objFilter = null;

                try
                {
                    if (mWebsiteGroupTabs == null && !string.IsNullOrEmpty(WebsiteGroupID))
                    {
                        objFilter = new UserAccountFilter();
                        objFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteGroupID.SearchString = WebsiteGroupID;
                        mUserAccounts = UserAccount.GetUserAccounts(objFilter);
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
                return mUserAccounts;
            }
        }

        private Website mWebsite = null;
        public Website Website
        {
            get
            {
                if (mWebsite == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    mWebsite = new Website(WebsiteID);
                }
                return mWebsite;
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

        public WebsiteGroup()
        {
        }
        public WebsiteGroup(string WebsiteGroupID)
        {
            this.WebsiteGroupID = WebsiteGroupID;
            Load();
        }
        public WebsiteGroup(DataRow objRow)
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
                         "FROM WebsiteGroup (NOLOCK) " +
                         "WHERE WebsiteGroupID=" + Database.HandleQuote(WebsiteGroupID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("WebsiteGroupID=" + WebsiteGroupID + " is not found");
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

                if (objColumns.Contains("WebsiteGroupID")) WebsiteGroupID = Convert.ToString(objRow["WebsiteGroupID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("GroupName")) GroupName = Convert.ToString(objRow["GroupName"]);
                if (objColumns.Contains("LogoPath")) LogoPath = Convert.ToString(objRow["LogoPath"]);
                if (objColumns.Contains("HomePageImagePath")) HomePageImagePath = Convert.ToString(objRow["HomePageImagePath"]);
                if (objColumns.Contains("HomePageMobileImagePath")) HomePageMobileImagePath = Convert.ToString(objRow["HomePageMobileImagePath"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(WebsiteGroupID)) throw new Exception("Missing WebsiteGroupID in the datarow");
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
        //protected bool ExistsWebsiteIDGroupName()
        //{
        //    bool _ret = false;

        //    try
        //    {
        //        WebsiteGroup WebsiteGroup = new WebsiteGroup();
        //        WebsiteGroupFilter WebsiteGroupFilter = new WebsiteGroupFilter();
        //        WebsiteGroupFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
        //        WebsiteGroupFilter.WebsiteID.SearchString = WebsiteID;
        //        WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
        //        WebsiteGroupFilter.GroupName.SearchString = GroupName;
        //        WebsiteGroup = WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);
        //        if (WebsiteGroup != null && !string.IsNullOrEmpty(WebsiteGroup.WebsiteGroupID) && WebsiteGroup.WebsiteGroupID != WebsiteGroupID)
        //        {
        //            _ret = true;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return _ret;
        //}
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
                if (WebsiteID == null) throw new Exception("WebsiteID is required");
                //if (ExistsWebsiteIDGroupName()) throw new Exception("Group already exists");
                if (!IsNew) throw new Exception("Create cannot be performed, WebsiteGroupID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["GroupName"] = GroupName;
                dicParam["LogoPath"] = LogoPath;
                dicParam["HomePageImagePath"] = HomePageImagePath;
                dicParam["HomePageMobileImagePath"] = HomePageMobileImagePath;
                dicParam["CreatedBy"] = CreatedBy;

                WebsiteGroupID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "WebsiteGroup"), objConn, objTran).ToString();

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
                if (WebsiteID == null) throw new Exception("WebsiteID is required");
                //if (ExistsWebsiteIDGroupName()) throw new Exception("Group already exists");
                if (IsNew) throw new Exception("Update cannot be performed, WebsiteGroupID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["GroupName"] = GroupName;
                dicParam["LogoPath"] = LogoPath;
                dicParam["HomePageImagePath"] = HomePageImagePath;
                dicParam["HomePageMobileImagePath"] = HomePageMobileImagePath;
                dicWParam["WebsiteGroupID"] = WebsiteGroupID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "WebsiteGroup"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, WebsiteGroupID is missing");

                foreach (UserAccount _UserAccount in UserAccounts)
                {
                    _UserAccount.Delete(objConn, objTran);
                }

                dicDParam["WebsiteGroupID"] = WebsiteGroupID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "WebsiteGroup"), objConn, objTran);
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
                     "FROM WebsiteGroup (NOLOCK) p " +
                     "WHERE " +
                     "(" +
                     "  (p.WebsiteID=" + Database.HandleQuote(WebsiteID) + " AND p.GroupName=" + Database.HandleQuote(GroupName) + ")" +
                     ") ";

            if (!string.IsNullOrEmpty(WebsiteGroupID)) strSQL += "AND p.WebsiteGroupID<>" + Database.HandleQuote(WebsiteGroupID);
            return Database.HasRows(strSQL);
        }

        public static WebsiteGroup GetWebsiteGroup(WebsiteGroupFilter Filter)
        {
            List<WebsiteGroup> objWebsiteGroups = null;
            WebsiteGroup objReturn = null;

            try
            {
                objWebsiteGroups = GetWebsiteGroups(Filter);
                if (objWebsiteGroups != null && objWebsiteGroups.Count >= 1) objReturn = objWebsiteGroups[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebsiteGroups = null;
            }
            return objReturn;
        }

        public static List<WebsiteGroup> GetWebsiteGroups()
        {
            int intTotalCount = 0;
            return GetWebsiteGroups(null, null, null, out intTotalCount);
        }

        public static List<WebsiteGroup> GetWebsiteGroups(WebsiteGroupFilter Filter)
        {
            int intTotalCount = 0;
            return GetWebsiteGroups(Filter, null, null, out intTotalCount);
        }

        public static List<WebsiteGroup> GetWebsiteGroups(WebsiteGroupFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetWebsiteGroups(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<WebsiteGroup> GetWebsiteGroups(WebsiteGroupFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<WebsiteGroup> objReturn = null;
            WebsiteGroup objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<WebsiteGroup>();

                strSQL = "SELECT * " +
                         "FROM WebsiteGroup (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteGroupID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteGroupID, "WebsiteGroupID");
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.GroupName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.GroupName, "GroupName");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "WebsiteGroupID" : Utility.CustomSorting.GetSortExpression(typeof(WebsiteGroup), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new WebsiteGroup(objData.Tables[0].Rows[i]);
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

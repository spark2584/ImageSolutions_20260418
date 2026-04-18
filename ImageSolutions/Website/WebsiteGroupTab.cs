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
    public class WebsiteGroupTab: ISBase.BaseClass
    {
        public string WebsiteGroupTabID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(WebsiteGroupTabID); } }
        public string WebsiteGroupID { get; set; }
        public string WebsiteTabID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        private WebsiteGroup mWebsiteGroup = null;
        public WebsiteGroup WebsiteGroup
        {
            get
            {
                if (mWebsiteGroup == null && !string.IsNullOrEmpty(WebsiteGroupID))
                {
                    mWebsiteGroup = new WebsiteGroup(WebsiteGroupID);
                }
                return mWebsiteGroup;
            }
        }
        private WebsiteTab mWebsiteTab = null;
        public WebsiteTab WebsiteTab
        {
            get
            {
                if (mWebsiteTab == null && !string.IsNullOrEmpty(WebsiteTabID))
                {
                    mWebsiteTab = new WebsiteTab(WebsiteTabID);
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

        public WebsiteGroupTab()
        {
        }
        public WebsiteGroupTab(string WebsiteGroupTabID)
        {
            this.WebsiteGroupTabID = WebsiteGroupTabID;
            Load();
        }
        public WebsiteGroupTab(DataRow objRow)
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
                         "FROM WebsiteGroupTab (NOLOCK) " +
                         "WHERE WebsiteGroupTabID=" + Database.HandleQuote(WebsiteGroupTabID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("WebsiteGroupTabID=" + WebsiteGroupTabID + " is not found");
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

                if (objColumns.Contains("WebsiteGroupTabID")) WebsiteGroupTabID = Convert.ToString(objRow["WebsiteGroupTabID"]);
                if (objColumns.Contains("WebsiteGroupID")) WebsiteGroupID = Convert.ToString(objRow["WebsiteGroupID"]);
                if (objColumns.Contains("WebsiteTabID")) WebsiteTabID = Convert.ToString(objRow["WebsiteTabID"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(WebsiteGroupTabID)) throw new Exception("Missing WebsiteGroupTabID in the datarow");
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
        protected bool ExistsWebsiteGroupIDWebsiteTabID()
        {
            bool _ret = false;

            try
            {
                WebsiteGroupTab WebsiteGroupTab = new WebsiteGroupTab();
                WebsiteGroupTabFilter WebsiteGroupTabFilter = new WebsiteGroupTabFilter();
                WebsiteGroupTabFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupTabFilter.WebsiteGroupID.SearchString = WebsiteGroupID;
                WebsiteGroupTabFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupTabFilter.WebsiteTabID.SearchString = WebsiteTabID;
                WebsiteGroupTab = WebsiteGroupTab.GetWebsiteGroupTab(WebsiteGroupTabFilter);
                if (WebsiteGroupTab != null && !string.IsNullOrEmpty(WebsiteGroupTab.WebsiteGroupTabID) && WebsiteGroupTab.WebsiteGroupTabID != WebsiteGroupTabID)
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

                if (ExistsWebsiteGroupIDWebsiteTabID()) throw new Exception("Tab is already assigned to this group");

                if (!IsNew) throw new Exception("Create cannot be performed, WebsiteGroupTabID already exists");

                dicParam["WebsiteGroupID"] = WebsiteGroupID;
                dicParam["WebsiteTabID"] = WebsiteTabID;
                dicParam["CreatedBy"] = CreatedBy;

                WebsiteGroupTabID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "WebsiteGroupTab"), objConn, objTran).ToString();

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

                if (ExistsWebsiteGroupIDWebsiteTabID()) throw new Exception("Tab is already assigned to this group");

                if (IsNew) throw new Exception("Update cannot be performed, WebsiteGroupTabID is missing");

                dicParam["WebsiteGroupID"] = WebsiteGroupID;
                dicParam["WebsiteTabID"] = WebsiteTabID;
                dicWParam["WebsiteGroupTabID"] = WebsiteGroupTabID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "WebsiteGroupTab"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, WebsiteGroupTabID is missing");

                dicDParam["WebsiteGroupTabID"] = WebsiteGroupTabID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "WebsiteGroupTab"), objConn, objTran);
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

        public static WebsiteGroupTab GetWebsiteGroupTab(WebsiteGroupTabFilter Filter)
        {
            List<WebsiteGroupTab> objWebsiteGroupTabs = null;
            WebsiteGroupTab objReturn = null;

            try
            {
                objWebsiteGroupTabs = GetWebsiteGroupTabs(Filter);
                if (objWebsiteGroupTabs != null && objWebsiteGroupTabs.Count >= 1) objReturn = objWebsiteGroupTabs[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebsiteGroupTabs = null;
            }
            return objReturn;
        }

        public static List<WebsiteGroupTab> GetWebsiteGroupTabs()
        {
            int intTotalCount = 0;
            return GetWebsiteGroupTabs(null, null, null, out intTotalCount);
        }

        public static List<WebsiteGroupTab> GetWebsiteGroupTabs(WebsiteGroupTabFilter Filter)
        {
            int intTotalCount = 0;
            return GetWebsiteGroupTabs(Filter, null, null, out intTotalCount);
        }

        public static List<WebsiteGroupTab> GetWebsiteGroupTabs(WebsiteGroupTabFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetWebsiteGroupTabs(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<WebsiteGroupTab> GetWebsiteGroupTabs(WebsiteGroupTabFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<WebsiteGroupTab> objReturn = null;
            WebsiteGroupTab objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<WebsiteGroupTab>();

                strSQL = "SELECT * " +
                         "FROM WebsiteGroupTab (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteTabID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteTabID, "WebsiteTabID");
                    if (Filter.WebsiteGroupID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteGroupID, "WebsiteGroupID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "WebsiteGroupTabID" : Utility.CustomSorting.GetSortExpression(typeof(WebsiteGroupTab), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new WebsiteGroupTab(objData.Tables[0].Rows[i]);
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Website
{
    public class WebsiteShippingServiceGroup : ISBase.BaseClass
    {
        public string WebsiteShippingServiceGroupID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(WebsiteShippingServiceGroupID); } }
        public string WebsiteShippingServiceID { get; set; }
        public string WebsiteGroupID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        private WebsiteShippingService mWebsiteShippingService = null;
        public WebsiteShippingService WebsiteShippingService
        {
            get
            {
                if (mWebsiteShippingService == null && !string.IsNullOrEmpty(WebsiteShippingServiceID))
                {
                    mWebsiteShippingService = new WebsiteShippingService(WebsiteShippingServiceID);
                }
                return mWebsiteShippingService;
            }
        }

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

        private User.UserInfo mCreatedByUser = null;
        public User.UserInfo CreatedByUser
        {
            get
            {
                if (mCreatedByUser == null && !string.IsNullOrEmpty(CreatedBy))
                {
                    mCreatedByUser = new User.UserInfo(CreatedBy);
                }
                return mCreatedByUser;
            }
        }

        public WebsiteShippingServiceGroup()
        {
        }
        public WebsiteShippingServiceGroup(string WebsiteShippingServiceGroupID)
        {
            this.WebsiteShippingServiceGroupID = WebsiteShippingServiceGroupID;
            Load();
        }
        public WebsiteShippingServiceGroup(DataRow objRow)
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
                         "FROM WebsiteShippingServiceGroup (NOLOCK) " +
                         "WHERE WebsiteShippingServiceGroupID=" + Database.HandleQuote(WebsiteShippingServiceGroupID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("WebsiteShippingServiceGroupID=" + WebsiteShippingServiceGroupID + " is not found");
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

                if (objColumns.Contains("WebsiteShippingServiceGroupID")) WebsiteShippingServiceGroupID = Convert.ToString(objRow["WebsiteShippingServiceGroupID"]);
                if (objColumns.Contains("WebsiteShippingServiceID")) WebsiteShippingServiceID = Convert.ToString(objRow["WebsiteShippingServiceID"]);
                if (objColumns.Contains("WebsiteGroupID")) WebsiteGroupID = Convert.ToString(objRow["WebsiteGroupID"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(WebsiteShippingServiceGroupID)) throw new Exception("Missing WebsiteShippingServiceGroupID in the datarow");
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
                if (WebsiteShippingServiceID == null) throw new Exception("WebsiteShippingServiceID is required");
                if (WebsiteGroupID == null) throw new Exception("WebsiteGroupID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, WebsiteShippingServiceGroupID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteShippingServiceID"] = WebsiteShippingServiceID;
                dicParam["WebsiteGroupID"] = WebsiteGroupID;
                dicParam["CreatedBy"] = CreatedBy;
                WebsiteShippingServiceGroupID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "WebsiteShippingServiceGroup"), objConn, objTran).ToString();

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
                if (WebsiteShippingServiceID == null) throw new Exception("WebsiteShippingServiceID is required");
                if (WebsiteGroupID == null) throw new Exception("WebsiteGroupID is required");
                if (IsNew) throw new Exception("Update cannot be performed, WebsiteShippingServiceGroupID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteShippingServiceID"] = WebsiteShippingServiceID;
                dicParam["WebsiteGroupID"] = WebsiteGroupID;
                dicWParam["WebsiteShippingServiceGroupID"] = WebsiteShippingServiceGroupID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "WebsiteShippingServiceGroup"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, WebsiteShippingServiceGroupID is missing");

                dicDParam["WebsiteShippingServiceGroupID"] = WebsiteShippingServiceGroupID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "WebsiteShippingServiceGroup"), objConn, objTran);
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
                    "FROM WebsiteShippingServiceGroup (NOLOCK) p " +
                    "WHERE " +
                    "(" +
                    "  (p.WebsiteShippingServiceID=" + Database.HandleQuote(WebsiteShippingServiceID) + " AND p.WebsiteGroupID=" + Database.HandleQuote(WebsiteGroupID) + ")" +
                    ") ";


            if (!string.IsNullOrEmpty(WebsiteShippingServiceGroupID)) strSQL += "AND p.WebsiteShippingServiceGroupID<>" + Database.HandleQuote(WebsiteShippingServiceGroupID);
            return Database.HasRows(strSQL);
        }

        public static WebsiteShippingServiceGroup GetWebsiteShippingServiceGroup(WebsiteShippingServiceGroupFilter Filter)
        {
            List<WebsiteShippingServiceGroup> objWebsiteShippingServiceGroups = null;
            WebsiteShippingServiceGroup objReturn = null;

            try
            {
                objWebsiteShippingServiceGroups = GetWebsiteShippingServiceGroups(Filter);
                if (objWebsiteShippingServiceGroups != null && objWebsiteShippingServiceGroups.Count >= 1) objReturn = objWebsiteShippingServiceGroups[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebsiteShippingServiceGroups = null;
            }
            return objReturn;
        }

        public static List<WebsiteShippingServiceGroup> GetWebsiteShippingServiceGroups()
        {
            int intTotalCount = 0;
            return GetWebsiteShippingServiceGroups(null, null, null, out intTotalCount);
        }

        public static List<WebsiteShippingServiceGroup> GetWebsiteShippingServiceGroups(WebsiteShippingServiceGroupFilter Filter)
        {
            int intTotalCount = 0;
            return GetWebsiteShippingServiceGroups(Filter, null, null, out intTotalCount);
        }

        public static List<WebsiteShippingServiceGroup> GetWebsiteShippingServiceGroups(WebsiteShippingServiceGroupFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetWebsiteShippingServiceGroups(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<WebsiteShippingServiceGroup> GetWebsiteShippingServiceGroups(WebsiteShippingServiceGroupFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<WebsiteShippingServiceGroup> objReturn = null;
            WebsiteShippingServiceGroup objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<WebsiteShippingServiceGroup>();

                strSQL = "SELECT * " +
                         "FROM WebsiteShippingServiceGroup (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteShippingServiceID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteShippingServiceID, "WebsiteShippingServiceID");
                    if (Filter.WebsiteGroupID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteGroupID, "WebsiteGroupID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "WebsiteShippingServiceGroupID" : Utility.CustomSorting.GetSortExpression(typeof(WebsiteShippingServiceGroup), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new WebsiteShippingServiceGroup(objData.Tables[0].Rows[i]);
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

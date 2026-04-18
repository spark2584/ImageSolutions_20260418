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
    public class WebsiteGroupItemExclude : ISBase.BaseClass
    {
        public string WebsiteGroupItemExcludeID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(WebsiteGroupItemExcludeID); } }
        public string WebsiteID { get; set; }
        public string WebsiteGroupID { get; set; }
        public string ItemID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public string GroupName
        {
            get
            {
                return WebsiteGroup.GroupName;
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
        private ImageSolutions.Item.Item mItem = null;
        public ImageSolutions.Item.Item Item
        {
            get
            {
                if (mItem == null && !string.IsNullOrEmpty(ItemID))
                {
                    mItem = new ImageSolutions.Item.Item(ItemID);
                }
                return mItem;
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

        public WebsiteGroupItemExclude()
        {
        }
        public WebsiteGroupItemExclude(string WebsiteGroupItemExcludeID)
        {
            this.WebsiteGroupItemExcludeID = WebsiteGroupItemExcludeID;
            Load();
        }
        public WebsiteGroupItemExclude(DataRow objRow)
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
                         "FROM WebsiteGroupItemExclude (NOLOCK) " +
                         "WHERE WebsiteGroupItemExcludeID=" + Database.HandleQuote(WebsiteGroupItemExcludeID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("WebsiteGroupItemExcludeID=" + WebsiteGroupItemExcludeID + " is not found");
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

                if (objColumns.Contains("WebsiteGroupItemExcludeID")) WebsiteGroupItemExcludeID = Convert.ToString(objRow["WebsiteGroupItemExcludeID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("WebsiteGroupID")) WebsiteGroupID = Convert.ToString(objRow["WebsiteGroupID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(WebsiteGroupItemExcludeID)) throw new Exception("Missing WebsiteGroupItemExcludeID in the datarow");
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
                if (ItemID == null) throw new Exception("WebsiteTabID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, WebsiteGroupItemExcludeID already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["WebsiteGroupID"] = WebsiteGroupID;
                dicParam["ItemID"] = ItemID;
                dicParam["CreatedBy"] = CreatedBy;

                WebsiteGroupItemExcludeID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "WebsiteGroupItemExclude"), objConn, objTran).ToString();

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
                if (ItemID == null) throw new Exception("WebsiteID is required");
                if (IsNew) throw new Exception("Update cannot be performed, WebsiteGroupItemExcludeID is missing");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["WebsiteGroupID"] = WebsiteGroupID;
                dicParam["ItemID"] = ItemID;
                dicWParam["WebsiteGroupItemExcludeID"] = WebsiteGroupItemExcludeID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "WebsiteGroupItemExclude"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, WebsiteGroupItemExcludeID is missing");

                dicDParam["WebsiteGroupItemExcludeID"] = WebsiteGroupItemExcludeID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "WebsiteGroupItemExclude"), objConn, objTran);
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

        public static WebsiteGroupItemExclude GetWebsiteGroupItemExclude(WebsiteGroupItemExcludeFilter Filter)
        {
            List<WebsiteGroupItemExclude> objWebsiteGroupItemExcludes = null;
            WebsiteGroupItemExclude objReturn = null;

            try
            {
                objWebsiteGroupItemExcludes = GetWebsiteGroupItemExcludes(Filter);
                if (objWebsiteGroupItemExcludes != null && objWebsiteGroupItemExcludes.Count >= 1) objReturn = objWebsiteGroupItemExcludes[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebsiteGroupItemExcludes = null;
            }
            return objReturn;
        }

        public static List<WebsiteGroupItemExclude> GetWebsiteGroupItemExcludes()
        {
            int intTotalCount = 0;
            return GetWebsiteGroupItemExcludes(null, null, null, out intTotalCount);
        }

        public static List<WebsiteGroupItemExclude> GetWebsiteGroupItemExcludes(WebsiteGroupItemExcludeFilter Filter)
        {
            int intTotalCount = 0;
            return GetWebsiteGroupItemExcludes(Filter, null, null, out intTotalCount);
        }

        public static List<WebsiteGroupItemExclude> GetWebsiteGroupItemExcludes(WebsiteGroupItemExcludeFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetWebsiteGroupItemExcludes(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<WebsiteGroupItemExclude> GetWebsiteGroupItemExcludes(WebsiteGroupItemExcludeFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<WebsiteGroupItemExclude> objReturn = null;
            WebsiteGroupItemExclude objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<WebsiteGroupItemExclude>();

                strSQL = "SELECT * " +
                         "FROM WebsiteGroupItemExclude (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                    if (Filter.WebsiteGroupID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteGroupID, "WebsiteGroupID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "WebsiteGroupItemExcludeID" : Utility.CustomSorting.GetSortExpression(typeof(WebsiteGroupItemExclude), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new WebsiteGroupItemExclude(objData.Tables[0].Rows[i]);
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

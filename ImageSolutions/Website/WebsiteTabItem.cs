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
    public class WebsiteTabItem : ISBase.BaseClass
    {
        public string WebsiteTabItemID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(WebsiteTabItemID); } }
        public string WebsiteTabID { get; set; }
        public string ItemID { get; set; }
        public int? Sort { get; set; }
        public bool Inactive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public string ItemDescription
        {
            get
            {
                return string.Format("{0} ({1})", Item.ItemNumber, Item.StoreDisplayName);
            }
        }

        private Item.Item mItem = null;
        public Item.Item Item
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

        public WebsiteTabItem()
        {
        }
        public WebsiteTabItem(string WebsiteTabItemID)
        {
            this.WebsiteTabItemID = WebsiteTabItemID;
            Load();
        }
        public WebsiteTabItem(DataRow objRow)
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
                         "FROM WebsiteTabItem (NOLOCK) " +
                         "WHERE WebsiteTabItemID=" + Database.HandleQuote(WebsiteTabItemID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("WebsiteTabItemID=" + WebsiteTabItemID + " is not found");
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

                if (objColumns.Contains("WebsiteTabItemID")) WebsiteTabItemID = Convert.ToString(objRow["WebsiteTabItemID"]);
                if (objColumns.Contains("WebsiteTabID")) WebsiteTabID = Convert.ToString(objRow["WebsiteTabID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("Sort") && objRow["Sort"] != DBNull.Value) Sort = Convert.ToInt32(objRow["Sort"]);
                if (objColumns.Contains("Inactive")) Inactive = Convert.ToBoolean(objRow["Inactive"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(WebsiteTabItemID)) throw new Exception("Missing WebsiteTabItemID in the datarow");
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
                if (WebsiteTabID == null) throw new Exception("WebsiteTabID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, WebsiteTabItemID already exists");

                dicParam["WebsiteTabID"] = WebsiteTabID;
                dicParam["ItemID"] = ItemID;
                dicParam["Sort"] = Sort;
                dicParam["Inactive"] = Inactive;
                dicParam["CreatedBy"] = CreatedBy;

                WebsiteTabItemID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "WebsiteTabItem"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, WebsiteTabItemID is missing");

                dicParam["WebsiteTabID"] = WebsiteTabID;
                dicParam["ItemID"] = ItemID;
                dicParam["Sort"] = Sort;
                dicParam["Inactive"] = Inactive;
                dicWParam["WebsiteTabItemID"] = WebsiteTabItemID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "WebsiteTabItem"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, WebsiteTabItemID is missing");

                dicDParam["WebsiteTabItemID"] = WebsiteTabItemID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "WebsiteTabItem"), objConn, objTran);
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

        public static WebsiteTabItem GetWebsiteTabItem(WebsiteTabItemFilter Filter)
        {
            List<WebsiteTabItem> objWebsiteTabItems = null;
            WebsiteTabItem objReturn = null;

            try
            {
                objWebsiteTabItems = GetWebsiteTabItems(Filter);
                if (objWebsiteTabItems != null && objWebsiteTabItems.Count >= 1) objReturn = objWebsiteTabItems[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebsiteTabItems = null;
            }
            return objReturn;
        }

        public static List<WebsiteTabItem> GetWebsiteTabItems()
        {
            int intTotalCount = 0;
            return GetWebsiteTabItems(null, null, null, out intTotalCount);
        }

        public static List<WebsiteTabItem> GetWebsiteTabItems(WebsiteTabItemFilter Filter)
        {
            int intTotalCount = 0;
            return GetWebsiteTabItems(Filter, null, null, out intTotalCount);
        }

        public static List<WebsiteTabItem> GetWebsiteTabItems(WebsiteTabItemFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetWebsiteTabItems(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<WebsiteTabItem> GetWebsiteTabItems(WebsiteTabItemFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<WebsiteTabItem> objReturn = null;
            WebsiteTabItem objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<WebsiteTabItem>();

                strSQL = "SELECT wti.* " +
                         "FROM WebsiteTabItem (NOLOCK) wti " +
                         "INNER JOIN ITEM (NOLOCK) i ON i.ItemID = wti.ItemID " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteTabID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteTabID, "wti.WebsiteTabID");
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "wti.ItemID");
                    if (Filter.Sort != null) strSQL += "AND wti.Sort=" + Database.HandleQuote(Convert.ToInt32(Filter.Sort).ToString());
                    if (Filter.IsOnline != null) strSQL += "AND i.IsOnline=" + Database.HandleQuote(Convert.ToInt32(Filter.IsOnline).ToString());
                    if (Filter.Inactive != null) strSQL += "AND i.Inactive=" + Database.HandleQuote(Convert.ToInt32(Filter.Inactive).ToString());
                }

                if (Filter.mAttributeValues != null && Filter.mAttributeValues.Count > 0)
                {

                    strSQL += "AND wti.ItemID IN (SELECT ItemID FROM Attribute a (NOLOCK) INNER JOIN AttributeValue av (NOLOCK) ON a.AttributeID=av.AttributeID WHERE av.Value IN (";

                    for (int i = 0; i < Filter.mAttributeValues.Count; i++)
                    {
                        if (i > 0) strSQL += ", ";
                        strSQL += Database.HandleQuote(Filter.mAttributeValues[i]);
                    }

                    strSQL += ")) ";
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Sort" : Utility.CustomSorting.GetSortExpression(typeof(WebsiteTabItem), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else strSQL += " ORDER BY Sort";

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new WebsiteTabItem(objData.Tables[0].Rows[i]);
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

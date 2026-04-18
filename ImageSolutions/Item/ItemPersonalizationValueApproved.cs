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

namespace ImageSolutions.Item
{
    public class ItemPersonalizationValueApproved : ISBase.BaseClass
    {
        public string ItemPersonalizationValueApprovedID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemPersonalizationValueApprovedID); } }
        public string WebsiteID { get; set; }
        public string ItemPersonalizationID { get; set; }
        public string ItemPersonalizationName { get; set; }
        public string ItemPersonalizationApprovedValue { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        private ItemPersonalization mItemPersonalization = null;
        public ItemPersonalization ItemPersonalization
        {
            get
            {
                if (mItemPersonalization == null && !string.IsNullOrEmpty(ItemPersonalizationID))
                {
                    mItemPersonalization = new ItemPersonalization(ItemPersonalizationID);
                }
                return mItemPersonalization;
            }
        }

        public ItemPersonalizationValueApproved()
        {
        }
        public ItemPersonalizationValueApproved(string ItemPersonalizationValueApprovedID)
        {
            this.ItemPersonalizationValueApprovedID = ItemPersonalizationValueApprovedID;
            Load();
        }
        public ItemPersonalizationValueApproved(DataRow objRow)
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
                         "FROM ItemPersonalizationValueApproved (NOLOCK) " +
                         "WHERE ItemPersonalizationValueApprovedID=" + Database.HandleQuote(ItemPersonalizationValueApprovedID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemPersonalizationValueApprovedID=" + ItemPersonalizationValueApprovedID + " is not found");
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

                if (objColumns.Contains("ItemPersonalizationValueApprovedID")) ItemPersonalizationValueApprovedID = Convert.ToString(objRow["ItemPersonalizationValueApprovedID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("ItemPersonalizationID")) ItemPersonalizationID = Convert.ToString(objRow["ItemPersonalizationID"]);
                if (objColumns.Contains("ItemPersonalizationName")) ItemPersonalizationName = Convert.ToString(objRow["ItemPersonalizationName"]);
                if (objColumns.Contains("ItemPersonalizationApprovedValue")) ItemPersonalizationApprovedValue = Convert.ToString(objRow["ItemPersonalizationApprovedValue"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemPersonalizationValueApprovedID)) throw new Exception("Missing ItemPersonalizationValueApprovedID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, ItemPersonalizationValueApprovedID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["ItemPersonalizationID"] = ItemPersonalizationID;
                dicParam["ItemPersonalizationName"] = ItemPersonalizationName;
                dicParam["ItemPersonalizationApprovedValue"] = ItemPersonalizationApprovedValue;
                dicParam["CreatedBy"] = CreatedBy;

                ItemPersonalizationValueApprovedID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemPersonalizationValueApproved"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, ItemPersonalizationValueApprovedID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["ItemPersonalizationID"] = ItemPersonalizationID;
                dicParam["ItemPersonalizationName"] = ItemPersonalizationName;
                dicParam["ItemPersonalizationApprovedValue"] = ItemPersonalizationApprovedValue;
                dicWParam["ItemPersonalizationValueApprovedID"] = ItemPersonalizationValueApprovedID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemPersonalizationValueApproved"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemPersonalizationValueApprovedID is missing");
                dicDParam["ItemPersonalizationValueApprovedID"] = ItemPersonalizationValueApprovedID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemPersonalizationValueApproved"), objConn, objTran);
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

            strSQL = "SELECT TOP 1 i.* " +
                     "FROM ItemPersonalizationValueApproved (NOLOCK) i " +
                     "WHERE " +
                     "(" +
                     "  (i.WebsiteID=" + Database.HandleQuote(WebsiteID) 
                        + " AND ISNULL(i.ItemPersonalizationID,'')=" + Database.HandleQuote(Convert.ToString(ItemPersonalizationID)) 
                        + " AND i.ItemPersonalizationName=" + Database.HandleQuote(ItemPersonalizationName)
                        + " AND i.ItemPersonalizationApprovedValue=" + Database.HandleQuote(ItemPersonalizationApprovedValue) + ")" +
                     ") ";

            if (!string.IsNullOrEmpty(ItemPersonalizationValueApprovedID)) strSQL += "AND i.ItemPersonalizationValueApprovedID<>" + Database.HandleQuote(ItemPersonalizationValueApprovedID);
            return Database.HasRows(strSQL);
        }

        public static ItemPersonalizationValueApproved GetItemPersonalizationValueApproved(ItemPersonalizationValueApprovedFilter Filter)
        {
            List<ItemPersonalizationValueApproved> objItemPersonalizationValueApproveds = null;
            ItemPersonalizationValueApproved objReturn = null;

            try
            {
                objItemPersonalizationValueApproveds = GetItemPersonalizationValueApproveds(Filter);
                if (objItemPersonalizationValueApproveds != null && objItemPersonalizationValueApproveds.Count >= 1) objReturn = objItemPersonalizationValueApproveds[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemPersonalizationValueApproveds = null;
            }
            return objReturn;
        }

        public static List<ItemPersonalizationValueApproved> GetItemPersonalizationValueApproveds()
        {
            int intTotalCount = 0;
            return GetItemPersonalizationValueApproveds(null, null, null, out intTotalCount);
        }

        public static List<ItemPersonalizationValueApproved> GetItemPersonalizationValueApproveds(ItemPersonalizationValueApprovedFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemPersonalizationValueApproveds(Filter, null, null, out intTotalCount);
        }

        public static List<ItemPersonalizationValueApproved> GetItemPersonalizationValueApproveds(ItemPersonalizationValueApprovedFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemPersonalizationValueApproveds(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemPersonalizationValueApproved> GetItemPersonalizationValueApproveds(ItemPersonalizationValueApprovedFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemPersonalizationValueApproved> objReturn = null;
            ItemPersonalizationValueApproved objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemPersonalizationValueApproved>();

                strSQL = "SELECT * " +
                         "FROM ItemPersonalizationValueApproved (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.ItemPersonalizationID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemPersonalizationID, "ItemPersonalizationID");
                    if (Filter.ItemPersonalizationName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemPersonalizationName, "ItemPersonalizationName");
                    if (Filter.ItemPersonalizationApprovedValue != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemPersonalizationApprovedValue, "ItemPersonalizationApprovedValue");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemPersonalizationValueApprovedID" : Utility.CustomSorting.GetSortExpression(typeof(ItemPersonalizationValueApproved), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemPersonalizationValueApproved(objData.Tables[0].Rows[i]);
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

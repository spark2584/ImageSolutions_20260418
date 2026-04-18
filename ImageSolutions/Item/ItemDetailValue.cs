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
    public class ItemDetailValue : ISBase.BaseClass
    {
        public string ItemDetailValueID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemDetailValueID); } }
        public string ItemDetailID { get; set; }
        public string Value { get; set; }
        public int? Sort { get; set; }
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

        private ItemDetail mItemDetail = null;
        public ItemDetail ItemDetail
        {
            get
            {
                if (mItemDetail == null && !string.IsNullOrEmpty(ItemDetailID))
                {
                    try
                    {
                        mItemDetail = new ItemDetail(ItemDetailID);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                    }
                }
                return mItemDetail;
            }
        }
        public ItemDetailValue()
        {
        }
        public ItemDetailValue(string ItemDetailValueID)
        {
            this.ItemDetailValueID = ItemDetailValueID;
            Load();
        }
        public ItemDetailValue(DataRow objRow)
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
                         "FROM ItemDetailValue (NOLOCK) " +
                         "WHERE ItemDetailValueID=" + Database.HandleQuote(ItemDetailValueID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemDetailValueID=" + ItemDetailValueID + " is not found");
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


        protected void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("ItemDetailValueID")) ItemDetailValueID = Convert.ToString(objRow["ItemDetailValueID"]);
                if (objColumns.Contains("ItemDetailID")) ItemDetailID = Convert.ToString(objRow["ItemDetailID"]);
                if (objColumns.Contains("Value")) Value = Convert.ToString(objRow["Value"]);
                if (objColumns.Contains("Sort") && objRow["Sort"] != DBNull.Value) Sort = Convert.ToInt32(objRow["Sort"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemDetailValueID)) throw new Exception("Missing ItemDetailValueID in the datarow");
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
                if (ItemDetailID == null) throw new Exception("ItemDetailID is required");
                if (Value == null) throw new Exception("Value is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemDetailValueID already exists");

                dicParam["ItemDetailID"] = ItemDetailID;
                dicParam["Value"] = Value;
                dicParam["Sort"] = Sort;
                dicParam["CreatedBy"] = CreatedBy;
                ItemDetailValueID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemDetailValue"), objConn, objTran).ToString();

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
                if (ItemDetailID == null) throw new Exception("ItemDetailID is required");
                if (Value == null) throw new Exception("Value is required");
                if (IsNew) throw new Exception("Update cannot be performed, ItemDetailValueID is missing");

                dicParam["ItemDetailID"] = ItemDetailID;
                dicParam["Value"] = Value;
                dicParam["Sort"] = Sort;
                dicWParam["ItemDetailValueID"] = ItemDetailValueID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemDetailValue"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemDetailValueID is missing");

                dicDParam["ItemDetailValueID"] = ItemDetailValueID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemDetailValue"), objConn, objTran);
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

        public static ItemDetailValue GetItemDetailValue(ItemDetailValueFilter Filter)
        {
            List<ItemDetailValue> objItemDetailValues = null;
            ItemDetailValue objReturn = null;

            try
            {
                objItemDetailValues = GetItemDetailValues(Filter);
                if (objItemDetailValues != null && objItemDetailValues.Count >= 1) objReturn = objItemDetailValues[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemDetailValues = null;
            }
            return objReturn;
        }

        public static List<ItemDetailValue> GetItemDetailValues()
        {
            int intTotalCount = 0;
            return GetItemDetailValues(null, null, null, out intTotalCount);
        }

        public static List<ItemDetailValue> GetItemDetailValues(ItemDetailValueFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemDetailValues(Filter, null, null, out intTotalCount);
        }

        public static List<ItemDetailValue> GetItemDetailValues(ItemDetailValueFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemDetailValues(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemDetailValue> GetItemDetailValues(ItemDetailValueFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemDetailValue> objReturn = null;
            ItemDetailValue objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemDetailValue>();

                strSQL = "SELECT * " +
                         "FROM ItemDetailValue (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ItemDetailID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemDetailID, "ItemDetailID");
                    if (Filter.Value != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Value, "Value");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemDetailValueID" : Utility.CustomSorting.GetSortExpression(typeof(ItemDetailValue), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else strSQL += " ORDER BY ISNULL(Sort,9999) ASC"; 
                
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemDetailValue(objData.Tables[0].Rows[i]);
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

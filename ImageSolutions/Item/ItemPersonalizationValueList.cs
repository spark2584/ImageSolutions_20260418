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
    public class ItemPersonalizationValueList : ISBase.BaseClass
    {
        public string ItemPersonalizationValueListID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemPersonalizationValueListID); } }
        public string ItemPersonalizationID { get; set; }
        public string Value { get; set; }
        public int? Sort { get; set; }
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

        public ItemPersonalizationValueList()
        {
        }
        public ItemPersonalizationValueList(string ItemPersonalizationValueListID)
        {
            this.ItemPersonalizationValueListID = ItemPersonalizationValueListID;
            Load();
        }
        public ItemPersonalizationValueList(DataRow objRow)
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
                         "FROM ItemPersonalizationValueList (NOLOCK) " +
                         "WHERE ItemPersonalizationValueListID=" + Database.HandleQuote(ItemPersonalizationValueListID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemPersonalizationValueListID=" + ItemPersonalizationValueListID + " is not found");
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

                if (objColumns.Contains("ItemPersonalizationValueListID")) ItemPersonalizationValueListID = Convert.ToString(objRow["ItemPersonalizationValueListID"]);
                if (objColumns.Contains("ItemPersonalizationID")) ItemPersonalizationID = Convert.ToString(objRow["ItemPersonalizationID"]);
                if (objColumns.Contains("Value")) Value = Convert.ToString(objRow["Value"]);
                if (objColumns.Contains("Sort") && objRow["Sort"] != DBNull.Value) Sort = Convert.ToInt32(objRow["Sort"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemPersonalizationValueListID)) throw new Exception("Missing ItemPersonalizationValueListID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, ItemPersonalizationValueListID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["ItemPersonalizationID"] = ItemPersonalizationID;
                dicParam["Value"] = Value;
                dicParam["Sort"] = Sort;
                dicParam["CreatedBy"] = CreatedBy;

                ItemPersonalizationValueListID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemPersonalizationValueList"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, ItemPersonalizationValueListID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["ItemPersonalizationID"] = ItemPersonalizationID;
                dicParam["Value"] = Value;
                dicParam["Sort"] = Sort;
                dicWParam["ItemPersonalizationValueListID"] = ItemPersonalizationValueListID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemPersonalizationValueList"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemPersonalizationValueListID is missing");
                dicDParam["ItemPersonalizationValueListID"] = ItemPersonalizationValueListID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemPersonalizationValueList"), objConn, objTran);
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
            return false;
        }

        public static ItemPersonalizationValueList GetItemPersonalizationValueList(ItemPersonalizationValueListFilter Filter)
        {
            List<ItemPersonalizationValueList> objItemPersonalizationValueLists = null;
            ItemPersonalizationValueList objReturn = null;

            try
            {
                objItemPersonalizationValueLists = GetItemPersonalizationValueLists(Filter);
                if (objItemPersonalizationValueLists != null && objItemPersonalizationValueLists.Count >= 1) objReturn = objItemPersonalizationValueLists[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemPersonalizationValueLists = null;
            }
            return objReturn;
        }

        public static List<ItemPersonalizationValueList> GetItemPersonalizationValueLists()
        {
            int intTotalCount = 0;
            return GetItemPersonalizationValueLists(null, null, null, out intTotalCount);
        }

        public static List<ItemPersonalizationValueList> GetItemPersonalizationValueLists(ItemPersonalizationValueListFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemPersonalizationValueLists(Filter, null, null, out intTotalCount);
        }

        public static List<ItemPersonalizationValueList> GetItemPersonalizationValueLists(ItemPersonalizationValueListFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemPersonalizationValueLists(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemPersonalizationValueList> GetItemPersonalizationValueLists(ItemPersonalizationValueListFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemPersonalizationValueList> objReturn = null;
            ItemPersonalizationValueList objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemPersonalizationValueList>();

                strSQL = "SELECT * " +
                         "FROM ItemPersonalizationValueList (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ItemPersonalizationID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemPersonalizationID, "ItemPersonalizationID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Sort" : Utility.CustomSorting.GetSortExpression(typeof(ItemPersonalizationValueList), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else strSQL += " ORDER BY ISNULL(Sort,9999) ASC";

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemPersonalizationValueList(objData.Tables[0].Rows[i]);
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

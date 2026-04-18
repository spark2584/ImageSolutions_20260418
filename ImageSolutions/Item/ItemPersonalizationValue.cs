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
    public class ItemPersonalizationValue : ISBase.BaseClass
    {
        public string ItemPersonalizationValueID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemPersonalizationValueID); } }
        public string ItemPersonalizationID { get; set; }
        public string ShoppingCartLineID { get; set; }
        public string SalesOrderLineID { get; set; }
        public string Value { get; set; }
        public string TextOption { get; set; }
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

        public ItemPersonalizationValue()
        {
        }
        public ItemPersonalizationValue(string ItemPersonalizationValueID)
        {
            this.ItemPersonalizationValueID = ItemPersonalizationValueID;
            Load();
        }
        public ItemPersonalizationValue(DataRow objRow)
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
                         "FROM ItemPersonalizationValue (NOLOCK) " +
                         "WHERE ItemPersonalizationValueID=" + Database.HandleQuote(ItemPersonalizationValueID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemPersonalizationValueID=" + ItemPersonalizationValueID + " is not found");
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

                if (objColumns.Contains("ItemPersonalizationValueID")) ItemPersonalizationValueID = Convert.ToString(objRow["ItemPersonalizationValueID"]);
                if (objColumns.Contains("ItemPersonalizationID")) ItemPersonalizationID = Convert.ToString(objRow["ItemPersonalizationID"]);
                if (objColumns.Contains("ShoppingCartLineID")) ShoppingCartLineID = Convert.ToString(objRow["ShoppingCartLineID"]);
                if (objColumns.Contains("SalesOrderLineID")) SalesOrderLineID = Convert.ToString(objRow["SalesOrderLineID"]);
                if (objColumns.Contains("Value")) Value = Convert.ToString(objRow["Value"]);
                if (objColumns.Contains("TextOption")) TextOption = Convert.ToString(objRow["TextOption"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemPersonalizationValueID)) throw new Exception("Missing ItemPersonalizationValueID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, ItemPersonalizationValueID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["ItemPersonalizationID"] = ItemPersonalizationID;
                dicParam["ShoppingCartLineID"] = ShoppingCartLineID;
                dicParam["SalesOrderLineID"] = SalesOrderLineID;
                dicParam["Value"] = Value;
                dicParam["TextOption"] = TextOption;

                ItemPersonalizationValueID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemPersonalizationValue"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, ItemPersonalizationValueID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["ItemPersonalizationID"] = ItemPersonalizationID;
                dicParam["ShoppingCartLineID"] = ShoppingCartLineID;
                dicParam["SalesOrderLineID"] = SalesOrderLineID;
                dicParam["Value"] = Value;
                dicParam["TextOption"] = TextOption;
                dicWParam["ItemPersonalizationValueID"] = ItemPersonalizationValueID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemPersonalizationValue"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemPersonalizationValueID is missing");
                dicDParam["ItemPersonalizationValueID"] = ItemPersonalizationValueID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemPersonalizationValue"), objConn, objTran);
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

        public static ItemPersonalizationValue GetItemPersonalizationValue(ItemPersonalizationValueFilter Filter)
        {
            List<ItemPersonalizationValue> objItemPersonalizationValues = null;
            ItemPersonalizationValue objReturn = null;

            try
            {
                objItemPersonalizationValues = GetItemPersonalizationValues(Filter);
                if (objItemPersonalizationValues != null && objItemPersonalizationValues.Count >= 1) objReturn = objItemPersonalizationValues[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemPersonalizationValues = null;
            }
            return objReturn;
        }

        public static List<ItemPersonalizationValue> GetItemPersonalizationValues()
        {
            int intTotalCount = 0;
            return GetItemPersonalizationValues(null, null, null, out intTotalCount);
        }

        public static List<ItemPersonalizationValue> GetItemPersonalizationValues(ItemPersonalizationValueFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemPersonalizationValues(Filter, null, null, out intTotalCount);
        }

        public static List<ItemPersonalizationValue> GetItemPersonalizationValues(ItemPersonalizationValueFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemPersonalizationValues(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemPersonalizationValue> GetItemPersonalizationValues(ItemPersonalizationValueFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemPersonalizationValue> objReturn = null;
            ItemPersonalizationValue objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemPersonalizationValue>();

                strSQL = "SELECT * " +
                         "FROM ItemPersonalizationValue (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ItemPersonalizationID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemPersonalizationID, "ItemPersonalizationID");
                    if (Filter.ShoppingCartLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ShoppingCartLineID, "ShoppingCartLineID");
                    if (Filter.SalesOrderLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderLineID, "SalesOrderLineID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Sort" : Utility.CustomSorting.GetSortExpression(typeof(ItemPersonalizationValue), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemPersonalizationValue(objData.Tables[0].Rows[i]);
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

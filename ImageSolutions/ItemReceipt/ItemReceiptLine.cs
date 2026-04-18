using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.ItemReceipt
{
    public class ItemReceiptLine : ISBase.BaseClass
    {
        public string ItemReceiptLineID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemReceiptLineID); } }
        public string ItemReceiptID { get; set; }
        public string RMALineID { get; set; }
        public long? NetSuiteLineID { get; set; }
        public string ItemID { get; set; }
        public int Quantity { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public DateTime CreatedOn { get; private set; }

        private ItemReceipt mItemReceipt = null;
        public ItemReceipt ItemReceipt
        {
            get
            {
                if (mItemReceipt == null && !string.IsNullOrEmpty(ItemReceiptID))
                {
                    mItemReceipt = new ItemReceipt(ItemReceiptID);
                }
                return mItemReceipt;
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

        public ItemReceiptLine()
        {
        }

        public ItemReceiptLine(string ItemReceiptLineID)
        {
            this.ItemReceiptLineID = ItemReceiptLineID;
            Load();
        }

        public ItemReceiptLine(DataRow objRow)
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
                         "FROM ItemReceiptLine (NOLOCK) " +
                         "WHERE ItemReceiptLineID=" + Database.HandleQuote(ItemReceiptLineID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemReceiptLineID=" + ItemReceiptLineID + " is not found");
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

                if (objColumns.Contains("ItemReceiptLineID")) ItemReceiptLineID = Convert.ToString(objRow["ItemReceiptLineID"]);
                if (objColumns.Contains("ItemReceiptID")) ItemReceiptID = Convert.ToString(objRow["ItemReceiptID"]);
                if (objColumns.Contains("RMALineID")) RMALineID = Convert.ToString(objRow["RMALineID"]);
                if (objColumns.Contains("NetSuiteLineID") && objRow["NetSuiteLineID"] != DBNull.Value) NetSuiteLineID = Convert.ToInt64(objRow["NetSuiteLineID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToInt32(objRow["Quantity"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
                
                if (string.IsNullOrEmpty(ItemReceiptLineID)) throw new Exception("Missing ItemReceiptLineID in the datarow");
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
                if (string.IsNullOrEmpty(ItemReceiptID)) throw new Exception("ItemReceiptID is required");
                if (string.IsNullOrEmpty(RMALineID)) throw new Exception("RMALineID is required");
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (Quantity <= 0) throw new Exception("Quantity cannot be less than or equal to 0");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemReceiptLineID already exists");

                dicParam["ItemReceiptID"] = ItemReceiptID;
                dicParam["RMALineID"] = RMALineID;
                dicParam["NetSuiteLineID"] = NetSuiteLineID;
                dicParam["ItemID"] = ItemID;
                dicParam["Quantity"] = Quantity;
                ItemReceiptLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemReceiptLine"), objConn, objTran).ToString();
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
                if(string.IsNullOrEmpty(ItemReceiptID)) throw new Exception("ItemReceiptID is required");
                if (string.IsNullOrEmpty(RMALineID)) throw new Exception("RMALineID is required");
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (Quantity <= 0) throw new Exception("Quantity cannot be less than or equal to 0");
                if (IsNew) throw new Exception("Update cannot be performed, RMALineID is missing");

                dicParam["ItemReceiptID"] = ItemReceiptID;
                dicParam["RMALineID"] = RMALineID;
                dicParam["NetSuiteLineID"] = NetSuiteLineID;
                dicParam["ItemID"] = ItemID;
                dicParam["Quantity"] = Quantity;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["ItemReceiptLineID"] = ItemReceiptLineID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemReceiptLine"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemReceiptLineID is missing");

                dicDParam["ItemReceiptLineID"] = ItemReceiptLineID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemReceiptLine"), objConn, objTran);
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

        public static List<ItemReceiptLine> GetItemReceiptLine()
        {
            int intTotalCount = 0;
            return GetItemReceiptLines(null, null, null, out intTotalCount);
        }

        public static List<ItemReceiptLine> GetItemReceiptLines(ItemReceiptLineFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemReceiptLines(Filter, null, null, out intTotalCount);
        }

        public static List<ItemReceiptLine> GetItemReceiptLines(ItemReceiptLineFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemReceiptLines(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemReceiptLine> GetItemReceiptLines(ItemReceiptLineFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemReceiptLine> objReturn = null;
            ItemReceiptLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemReceiptLine>();

                strSQL = "SELECT * " +
                         "FROM ItemReceiptLine (NOLOCK) rl " +
                         "INNER JOIN ItemReceipt (NOLOCK) r ON rl.ItemReceiptID=r.ItemReceiptID " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ItemReceiptID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemReceiptID, "ItemReceiptID");
                    if (Filter.RMALineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.RMALineID, "RMALineID");
                    if (Filter.NetSuiteLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.NetSuiteLineID, "NetSuiteLineID");
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemReceiptLineID" : Utility.CustomSorting.GetSortExpression(typeof(ItemReceiptLine), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemReceiptLine(objData.Tables[0].Rows[i]);
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

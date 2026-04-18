using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.ItemReceipt
{
    public class ItemReceipt : ISBase.BaseClass
    {
        public string ItemReceiptID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemReceiptID); } }
        public string InternalID { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public string RMAID { get; private set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        private RMA.RMA mRMA = null;
        public RMA.RMA RMA
        {
            get
            {
                if (mRMA == null && !string.IsNullOrEmpty(RMAID))
                {
                    mRMA = new RMA.RMA(RMAID);
                }
                return mRMA;
            }
        }

        private List<ItemReceiptLine> mItemReceiptLines = null;
        public List<ItemReceiptLine> ItemReceiptLines
        {
            get
            {
                if (mItemReceiptLines == null && !string.IsNullOrEmpty(ItemReceiptID))
                {
                    ItemReceiptLineFilter objFilter = null;
                    try
                    {
                        objFilter = new ItemReceiptLineFilter();
                        objFilter.ItemReceiptID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemReceiptID.SearchString = ItemReceiptID;
                        mItemReceiptLines = ItemReceiptLine.GetItemReceiptLines(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mItemReceiptLines;
            }
            set
            {
                mItemReceiptLines = value;
            }
        }

        public ItemReceipt()
        {
        }

        public ItemReceipt(string ItemReceiptID)
        {
            this.ItemReceiptID = ItemReceiptID;
            Load();
        }

        public ItemReceipt(DataRow objRow)
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
                         "FROM ItemReceipt (NOLOCK) " +
                         "WHERE ItemReceiptID=" + Database.HandleQuote(ItemReceiptID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemReceiptID=" + ItemReceiptID + " is not found");
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
                if (objColumns.Contains("ItemReceiptID")) ItemReceiptID = Convert.ToString(objRow["ItemReceiptID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("DocumentNumber")) DocumentNumber = Convert.ToString(objRow["DocumentNumber"]);
                if (objColumns.Contains("TransactionDate")) TransactionDate = Convert.ToDateTime(objRow["TransactionDate"]);
                if (objColumns.Contains("RMAID")) RMAID = Convert.ToString(objRow["RMAID"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);

                if (string.IsNullOrEmpty(RMAID)) throw new Exception("Missing RMAID in the datarow");
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
                if (string.IsNullOrEmpty(InternalID)) throw new Exception("InternalID is required");
                if (string.IsNullOrEmpty(DocumentNumber)) throw new Exception("DocumentNumber is required");
                if (TransactionDate == null) throw new Exception("TransactionDate is required");
                if (string.IsNullOrEmpty(RMAID)) throw new Exception("RMAID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemReceiptID already exists");

                dicParam["InternalID"] = InternalID;
                dicParam["DocumentNumber"] = DocumentNumber;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["RMAID"] = RMAID;
                dicParam["ErrorMessage"] = ErrorMessage;
                ItemReceiptID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemReceipt"), objConn, objTran).ToString();

                foreach (ItemReceiptLine objItemReceiptLine in ItemReceiptLines)
                {
                    objItemReceiptLine.ItemReceiptID = ItemReceiptID;
                    objItemReceiptLine.Create(objConn, objTran);
                }

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
                if (string.IsNullOrEmpty(InternalID)) throw new Exception("InternalID is required");
                if (string.IsNullOrEmpty(DocumentNumber)) throw new Exception("DocumentNumber is required");
                if (TransactionDate == null) throw new Exception("TransactionDate is required");
                if (string.IsNullOrEmpty(RMAID)) throw new Exception("RMAID is required");
                if (IsNew) throw new Exception("Update cannot be performed, ItemReceiptID is missing");

                dicParam["InternalID"] = InternalID;
                dicParam["DocumentNumber"] = DocumentNumber;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["RMAID"] = RMAID;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicWParam["ItemReceiptID"] = ItemReceiptID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemReceipt"), objConn, objTran);

                foreach (ItemReceiptLine objItemReceiptLine in ItemReceiptLines)
                {
                    objItemReceiptLine.Update(objConn, objTran);
                }

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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemReceiptID is missing");

                dicDParam["ItemReceiptID"] = ItemReceiptID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemReceipt"), objConn, objTran);
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

        public static ItemReceipt GetItemReceipt(ItemReceiptFilter Filter)
        {
            List<ItemReceipt> objItemReceipts = null;
            ItemReceipt objReturn = null;

            try
            {
                objItemReceipts = GetItemReceipts(Filter);
                if (objItemReceipts != null && objItemReceipts.Count >= 1) objReturn = objItemReceipts[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemReceipts = null;
            }
            return objReturn;
        }

        public static List<ItemReceipt> GetItemReceipts()
        {
            int intTotalCount = 0;
            return GetItemReceipts(null, null, null, out intTotalCount);
        }

        public static List<ItemReceipt> GetItemReceipts(ItemReceiptFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemReceipts(Filter, null, null, out intTotalCount);
        }

        public static List<ItemReceipt> GetItemReceipts(ItemReceiptFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemReceipts(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemReceipt> GetItemReceipts(ItemReceiptFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemReceipt> objReturn = null;
            ItemReceipt objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemReceipt>();

                strSQL = "SELECT * " +
                         "FROM ItemReceipt (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ItemReceiptID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemReceiptID, "ItemReceiptID");
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.DocumentNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.DocumentNumber, "DocumentNumber");
                    if (Filter.RMAID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.RMAID, "RMAID");
                    if (Filter.UpdatedOn != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.UpdatedOn, "UpdatedOn");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemReceiptID" : Utility.CustomSorting.GetSortExpression(typeof(ItemReceipt), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemReceipt(objData.Tables[0].Rows[i]);
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

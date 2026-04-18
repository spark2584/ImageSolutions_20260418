using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Collections;


namespace ImageSolutions.TransferOrder
{
    public class TransferOrderLine : ISBase.BaseClass
    {
        public string TransferOrderLineID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(TransferOrderLineID); } }
        public string BusinessID { get; set; }
        public string TransferOrderID { get; set; }
        public long? NetSuiteLineID { get; set; }
        public string ItemID { get; set; }
        public string RetailerSKU { get; set; }
        public string FulfillmentNetworkSKU { get; set; }
        public int Quantity { get; set; }
        public int QuantityReceived { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public DateTime CreatedOn { get; private set; }

        private TransferOrder mTransferOrder = null;
        public TransferOrder TransferOrder
        {
            get
            {
                if (mTransferOrder == null && !string.IsNullOrEmpty(TransferOrderID) && !string.IsNullOrEmpty(BusinessID))
                {
                    mTransferOrder = new TransferOrder(BusinessID, TransferOrderID);
                }
                return mTransferOrder;
            }
        }

        private Item.Item mItem = null;
        public Item.Item Item
        {
            get
            {
                if (mItem == null && !string.IsNullOrEmpty(ItemID) && !string.IsNullOrEmpty(BusinessID))
                {
                    mItem = new ImageSolutions.Item.Item(ItemID);
                }
                return mItem;
            }
        }

      
        //private List<ItemReceipt.ItemReceiptLine> mItemReceiptLines = null;
        //public List<ItemReceipt.ItemReceiptLine> ItemReceiptLines
        //{
        //    get
        //    {
        //        if (mItemReceiptLines == null && !string.IsNullOrEmpty(BusinessID) && !string.IsNullOrEmpty(TransferOrderLineID))
        //        {
        //            ItemReceipt.ItemReceiptLineFilter objFilter = null;

        //            try
        //            {
        //                objFilter = new ItemReceipt.ItemReceiptLineFilter();
        //                objFilter.TransferOrderLineID = new Database.Filter.StringSearch.SearchFilter();
        //                objFilter.TransferOrderLineID.SearchString = TransferOrderLineID;
        //                mItemReceiptLines = ItemReceipt.ItemReceiptLine.GetItemReceiptLines(objFilter);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //            finally
        //            {
        //                objFilter = null;
        //            }
        //        }
        //        return mItemReceiptLines;
        //    }
        //}

        public TransferOrderLine(string BusinessID)
        {
            this.BusinessID = BusinessID;
        }

        public TransferOrderLine(string BusinessID, string TransferOrderLineID)
        {
            this.BusinessID = BusinessID;
            this.TransferOrderLineID = TransferOrderLineID;
            Load();
        }

        public TransferOrderLine(DataRow objRow)
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
                         "FROM TransferOrderLine (NOLOCK) " +
                         "WHERE TransferOrderLineID=" + Database.HandleQuote(TransferOrderLineID) +
                         "AND BusinessID=" + Database.HandleQuote(BusinessID);

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("TransferOrderLineID=" + TransferOrderLineID + " is not found");
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

                if (objColumns.Contains("TransferOrderLineID")) TransferOrderLineID = Convert.ToString(objRow["TransferOrderLineID"]);
                if (objColumns.Contains("BusinessID")) BusinessID = Convert.ToString(objRow["BusinessID"]);
                if (objColumns.Contains("TransferOrderID")) TransferOrderID = Convert.ToString(objRow["TransferOrderID"]);
                if (objColumns.Contains("NetSuiteLineID") && objRow["NetSuiteLineID"] != DBNull.Value) NetSuiteLineID = Convert.ToInt32(objRow["NetSuiteLineID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("RetailerSKU")) RetailerSKU = Convert.ToString(objRow["RetailerSKU"]);
                if (objColumns.Contains("FulfillmentNetworkSKU")) FulfillmentNetworkSKU = Convert.ToString(objRow["FulfillmentNetworkSKU"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToInt32(objRow["Quantity"]);
                if (objColumns.Contains("QuantityReceived")) QuantityReceived = Convert.ToInt32(objRow["QuantityReceived"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(TransferOrderLineID)) throw new Exception("Missing TransferOrderLineID in the datarow");
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
                if (string.IsNullOrEmpty(BusinessID)) throw new Exception("BusinessID is required");
                if (string.IsNullOrEmpty(TransferOrderID)) throw new Exception("TransferOrderID is required");
                if (string.IsNullOrEmpty(RetailerSKU)) throw new Exception("RetailerSKU is required");
                if (Quantity <= 0) throw new Exception("Quantity cannot be less than or equal to 0");
                if (!IsNew) throw new Exception("Create cannot be performed, TransferOrderLineID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["BusinessID"] = BusinessID;
                dicParam["TransferOrderID"] = TransferOrderID;
                dicParam["NetSuiteLineID"] = NetSuiteLineID;
                dicParam["ItemID"] = ItemID;
                dicParam["RetailerSKU"] = RetailerSKU;
                dicParam["FulfillmentNetworkSKU"] = FulfillmentNetworkSKU;
                dicParam["Quantity"] = Quantity;
                dicParam["QuantityReceived"] = QuantityReceived;
                dicParam["ErrorMessage"] = ErrorMessage;
                TransferOrderLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "TransferOrderLine"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(BusinessID)) throw new Exception("BusinessID is required");
                if (string.IsNullOrEmpty(TransferOrderID)) throw new Exception("TransferOrderID is required");
                if (string.IsNullOrEmpty(RetailerSKU)) throw new Exception("RetailerSKU is required");
                if (Quantity <= 0) throw new Exception("Quantity cannot be less than or equal to 0");
                if (IsNew) throw new Exception("Update cannot be performed, TransferOrderLineID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["BusinessID"] = BusinessID;
                dicParam["TransferOrderID"] = TransferOrderID;
                dicParam["NetSuiteLineID"] = NetSuiteLineID;
                dicParam["ItemID"] = ItemID;
                dicParam["RetailerSKU"] = RetailerSKU;
                dicParam["FulfillmentNetworkSKU"] = FulfillmentNetworkSKU;
                dicParam["Quantity"] = Quantity;
                dicParam["QuantityReceived"] = QuantityReceived;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["TransferOrderLineID"] = TransferOrderLineID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "TransferOrderLine"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, OrderLinesID is missing");

                //if (ItemReceiptLines != null)
                //{
                //    foreach (ItemReceipt.ItemReceiptLine objItemReceiptLine in ItemReceiptLines)
                //    {
                //        objItemReceiptLine.Delete(objConn, objTran);
                //    }
                //}

                dicDParam["TransferOrderLineID"] = TransferOrderLineID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "TransferOrderLine"), objConn, objTran);
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

        public static List<TransferOrderLine> GetTransferOrderLines()
        {
            int intTotalCount = 0;
            return GetTransferOrderLines(null, null, null, out intTotalCount);
        }

        public static List<TransferOrderLine> GetTransferOrderLines(TransferOrderLineFilter Filter)
        {
            int intTotalCount = 0;
            return GetTransferOrderLines(Filter, null, null, out intTotalCount);
        }

        public static List<TransferOrderLine> GetTransferOrderLines(TransferOrderLineFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetTransferOrderLines(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<TransferOrderLine> GetTransferOrderLines(TransferOrderLineFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<TransferOrderLine> objReturn = null;
            TransferOrderLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<TransferOrderLine>();

                strSQL = "SELECT * " +
                         "FROM TransferOrderLine (NOLOCK) sl " +
                         "INNER JOIN TransferOrder (NOLOCK) s ON sl.TransferOrderID=s.TransferOrderID " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (!string.IsNullOrEmpty(Filter.TransferOrderID)) strSQL += "AND sl.TransferOrderID=" + Database.HandleQuote(Filter.TransferOrderID);
                    if (!string.IsNullOrEmpty(Filter.RetailerSKU)) strSQL += "AND sl.RetailerSKU=" + Database.HandleQuote(Filter.RetailerSKU);
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "sl.ItemID");
                    if (Filter.NetSuiteLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.NetSuiteLineID, "sl.NetSuiteLineID");
                    if (Filter.TransferOrder_NetSuiteInternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.TransferOrder_NetSuiteInternalID, "s.NetSuiteInternalID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "TransferOrderLineID" : Utility.CustomSorting.GetSortExpression(typeof(TransferOrderLine), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new TransferOrderLine(objData.Tables[0].Rows[i]);
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

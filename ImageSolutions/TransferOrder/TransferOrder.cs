using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;

namespace ImageSolutions.TransferOrder
{
    public class TransferOrder : ISBase.BaseClass
    {
        public string TransferOrderID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(TransferOrderID); } }
        public string BusinessID { get; private set; }
        public string NetSuiteInternalID { get; set; }
        public string NetSuiteDocumentNumber { get; set; }
        public bool IsPurchaseOrder { get; set; }
        public string PINumber { get; set; }
        public string AmazonMWSAccountID { get; set; }
        public string ShipmentID { get; set; }
        public string ShipmentName { get; set; }
        public string FromLocationID { get; set; }
        public string ToLocationID { get; set; }
        public bool IsNSUpdated { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public DateTime CreatedOn { get; private set; }

     

        private List<TransferOrderLine> mTransferOrderLines = null;
        public List<TransferOrderLine> TransferOrderLines
        {
            get
            {
                if (mTransferOrderLines == null && !string.IsNullOrEmpty(TransferOrderID))
                {
                    TransferOrderLineFilter objFilter = null;

                    try
                    {
                        objFilter = new TransferOrderLineFilter();
                        objFilter.TransferOrderID = TransferOrderID;
                        mTransferOrderLines = TransferOrderLine.GetTransferOrderLines(objFilter);
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
                return mTransferOrderLines;
            }
            set
            {
                mTransferOrderLines = value;
            }
        }

        //private List<ItemReceipt.ItemReceipt> mItemReceipts = null;
        //public List<ItemReceipt.ItemReceipt> ItemReceipts
        //{
        //    get
        //    {
        //        if (mItemReceipts == null && !string.IsNullOrEmpty(TransferOrderID))
        //        {
        //            ItemReceipt.ItemReceiptFilter objFilter = null;

        //            try
        //            {
        //                objFilter = new ItemReceipt.ItemReceiptFilter();
        //                objFilter.TransferOrderID = new Database.Filter.StringSearch.SearchFilter();
        //                objFilter.TransferOrderID.SearchString = TransferOrderID;
        //                mItemReceipts = ItemReceipt.ItemReceipt.GetItemReceipts(objFilter);
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
        //        return mItemReceipts;
        //    }
        //    set
        //    {
        //        mItemReceipts = value;
        //    }
        //}

        //private Location.Location mFromLocation = null;
        //public Location.Location FromLocation
        //{
        //    get
        //    {
        //        if (mFromLocation == null && !string.IsNullOrEmpty(FromLocationID) && !string.IsNullOrEmpty(BusinessID))
        //        {
        //            mFromLocation = new Location.Location(BusinessID, FromLocationID);
        //        }
        //        return mFromLocation;
        //    }
        //}

        //private Location.Location mToLocation = null;
        //public Location.Location ToLocation
        //{
        //    get
        //    {
        //        if (mToLocation == null && !string.IsNullOrEmpty(ToLocationID) && !string.IsNullOrEmpty(BusinessID))
        //        {
        //            mToLocation = new Location.Location(BusinessID, ToLocationID);
        //        }
        //        return mToLocation;
        //    }
        //}

        public TransferOrder(string BusinessID)
        {
            this.BusinessID = BusinessID;
        }

        public TransferOrder(string BusinessID, string TransferOrderID)
        {
            this.BusinessID = BusinessID;
            this.TransferOrderID = TransferOrderID;
            Load();
        }

        public TransferOrder(DataRow objRow)
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
                         "FROM TransferOrder (NOLOCK) " +
                         "WHERE TransferOrderID=" + Database.HandleQuote(TransferOrderID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("TransferOrderID=" + TransferOrderID + " is not found");
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

                if (objColumns.Contains("TransferOrderID")) TransferOrderID = Convert.ToString(objRow["TransferOrderID"]);
                if (objColumns.Contains("BusinessID")) BusinessID = Convert.ToString(objRow["BusinessID"]);
                if (objColumns.Contains("NetSuiteInternalID")) NetSuiteInternalID = Convert.ToString(objRow["NetSuiteInternalID"]);
                if (objColumns.Contains("NetSuiteDocumentNumber")) NetSuiteDocumentNumber = Convert.ToString(objRow["NetSuiteDocumentNumber"]);
                if (objColumns.Contains("IsPurchaseOrder")) IsPurchaseOrder = Convert.ToBoolean(objRow["IsPurchaseOrder"]);
                if (objColumns.Contains("PINumber")) PINumber = Convert.ToString(objRow["PINumber"]);
                if (objColumns.Contains("AmazonMWSAccountID")) AmazonMWSAccountID = Convert.ToString(objRow["AmazonMWSAccountID"]);
                if (objColumns.Contains("ShipmentID")) ShipmentID = Convert.ToString(objRow["ShipmentID"]);
                if (objColumns.Contains("ShipmentName")) ShipmentName = Convert.ToString(objRow["ShipmentName"]);
                if (objColumns.Contains("FromLocationID")) FromLocationID = Convert.ToString(objRow["FromLocationID"]);
                if (objColumns.Contains("ToLocationID")) ToLocationID = Convert.ToString(objRow["ToLocationID"]);
                if (objColumns.Contains("IsNSUpdated")) IsNSUpdated = Convert.ToBoolean(objRow["IsNSUpdated"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(TransferOrderID)) throw new Exception("Missing TransferOrderID in the datarow");
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
            ItemReceipt.ItemReceipt objItemReceipt = null;
            ItemReceipt.ItemReceiptLine objItemReceiptLine = null;

            try
            {
                //if (string.IsNullOrEmpty(ShipmentID)) throw new Exception("ShipmentID is required");
                //if (string.IsNullOrEmpty(ShipmentName)) throw new Exception("ShipmentName is required");
                if (TransferOrderLines == null || TransferOrderLines.Count == 0) throw new Exception("TransferOrderLines is required");
                if (!IsNew) throw new Exception("Create cannot be performed, TransferOrderID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["BusinessID"] = BusinessID;
                dicParam["NetSuiteInternalID"] = NetSuiteInternalID;
                dicParam["NetSuiteDocumentNumber"] = NetSuiteDocumentNumber;
                dicParam["IsPurchaseOrder"] = IsPurchaseOrder;
                dicParam["PINumber"] = PINumber;
                dicParam["AmazonMWSAccountID"] = AmazonMWSAccountID;
                dicParam["ShipmentID"] = ShipmentID;
                dicParam["ShipmentName"] = ShipmentName;
                dicParam["FromLocationID"] = FromLocationID;
                dicParam["ToLocationID"] = ToLocationID;
                dicParam["IsNSUpdated"] = IsNSUpdated;
                dicParam["ErrorMessage"] = ErrorMessage;
                TransferOrderID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "TransferOrder"), objConn, objTran).ToString();

                foreach (TransferOrderLine objTransferOrderLine in TransferOrderLines)
                {
                    objTransferOrderLine.TransferOrderID = TransferOrderID;
                    objTransferOrderLine.Create(objConn, objTran);
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
                //if (string.IsNullOrEmpty(ShipmentID)) throw new Exception("ShipmentID is required");
                //if (string.IsNullOrEmpty(ShipmentName)) throw new Exception("ShipmentName is required");
                if (TransferOrderLines == null || TransferOrderLines.Count == 0) throw new Exception("TransferOrderLines is required");
                if (IsNew) throw new Exception("Update cannot be performed, TransferOrderID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["BusinessID"] = BusinessID;
                dicParam["NetSuiteInternalID"] = NetSuiteInternalID;
                dicParam["NetSuiteDocumentNumber"] = NetSuiteDocumentNumber;
                dicParam["IsPurchaseOrder"] = IsPurchaseOrder;
                dicParam["PINumber"] = PINumber;
                dicParam["AmazonMWSAccountID"] = AmazonMWSAccountID;
                dicParam["ShipmentID"] = ShipmentID;
                dicParam["ShipmentName"] = ShipmentName;
                dicParam["FromLocationID"] = FromLocationID;
                dicParam["ToLocationID"] = ToLocationID;
                dicParam["IsNSUpdated"] = IsNSUpdated;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["TransferOrderID"] = TransferOrderID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "TransferOrder"), objConn, objTran);

                foreach (TransferOrderLine objTransferOrderLine in TransferOrderLines)
                {
                    objTransferOrderLine.Update(objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, TransferOrderID is missing");

                dicDParam["TransferOrderID"] = TransferOrderID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "TransferOrder"), objConn, objTran);
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

        public static TransferOrder GetTransferOrder(TransferOrderFilter Filter)
        {
            List<TransferOrder> objTransferOrders = null;
            TransferOrder objReturn = null;

            try
            {
                objTransferOrders = GetTransferOrders(Filter);
                if (objTransferOrders != null && objTransferOrders.Count >= 1) objReturn = objTransferOrders[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objTransferOrders = null;
            }
            return objReturn;
        }

        public static List<TransferOrder> GetTransferOrders()
        {
            int intTotalCount = 0;
            return GetTransferOrders(null, null, null, out intTotalCount);
        }

        public static List<TransferOrder> GetTransferOrders(TransferOrderFilter Filter)
        {
            int intTotalCount = 0;
            return GetTransferOrders(Filter, null, null, out intTotalCount);
        }

        public static List<TransferOrder> GetTransferOrders(TransferOrderFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetTransferOrders(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<TransferOrder> GetTransferOrders(TransferOrderFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<TransferOrder> objReturn = null;
            TransferOrder objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<TransferOrder>();

                strSQL = "SELECT s.* " +
                         "FROM TransferOrder (NOLOCK) s " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.TransferOrderID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.TransferOrderID, "s.TransferOrderID");
                    if (Filter.NetSuiteInternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.NetSuiteInternalID, "s.NetSuiteInternalID");
                    if (Filter.NetSuiteDocumentNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.NetSuiteDocumentNumber, "s.NetSuiteDocumentNumber");
                    if (Filter.IsPurchaseOrder != null) strSQL += " AND s.IsPurchaseOrder=" + Database.HandleQuote(Convert.ToInt32(Filter.IsPurchaseOrder).ToString());
                    if (Filter.PINumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PINumber, "s.PINumber");
                    if (Filter.AmazonMWSAccountID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.AmazonMWSAccountID, "s.AmazonMWSAccountID");
                    if (Filter.ShipmentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ShipmentID, "s.ShipmentID");
                    if (Filter.ShipmentName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ShipmentName, "s.ShipmentName");
                    if (Filter.FromLocationID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.FromLocationID, "s.FromLocationID");
                    if (Filter.ToLocationID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ToLocationID, "s.ToLocationID");
                    if (Filter.ErrorMessage != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ErrorMessage, "s.ErrorMessage");
                    if (Filter.IsNSUpdated != null) strSQL += " AND s.IsNSUpdated=" + Database.HandleQuote(Convert.ToInt32(Filter.IsNSUpdated).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "TransferOrderID" : Utility.CustomSorting.GetSortExpression(typeof(TransferOrder), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new TransferOrder(objData.Tables[0].Rows[i]);
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

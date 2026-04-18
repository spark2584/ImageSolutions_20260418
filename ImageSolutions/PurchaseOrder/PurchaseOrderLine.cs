using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ImageSolutions.PurchaseOrder
{
    public class PurchaseOrderLine : ISBase.BaseClass
    {
        public string PurchaseOrderLineID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(PurchaseOrderLineID); } }
        public string PurchaseOrderID { get; set; }
        public long? LineID { get; set; }
        public string ItemID { get; set; }
        public int Quantity { get; set; }
        public int? ReceivedQty { get; set; }
        public decimal? FOBPrice { get; set; }
        public string SalesOrderLineID { get; set; }
        public bool IsClosed { get; set; }
        public bool IsClosedByMerchant { get; set; }

        public string CancellationReason { get; set; }

        public DateTime CreatedOn { get; private set; }
        public DateTime? UpdatedOn { get; private set; }
        public int DockReceivedQty { get; set; }
        public int CartonQuantity { get; set; }

        private PurchaseOrder mPurchaseOrder = null;
        public PurchaseOrder PurchaseOrder
        {
            get
            {
                if (mPurchaseOrder == null && !string.IsNullOrEmpty(PurchaseOrderID))
                {
                    mPurchaseOrder = new PurchaseOrder(PurchaseOrderID);
                }
                return mPurchaseOrder;
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

        //private List<ItemReceipt.ItemReceiptLine> mItemReceiptLines = null;
        //public List<ItemReceipt.ItemReceiptLine> ItemReceiptLines
        //{
        //    get
        //    {
        //        if (mItemReceiptLines == null && !string.IsNullOrEmpty(PurchaseOrderLineID))
        //        {
        //            ItemReceipt.ItemReceiptLineFilter objFilter = null;

        //            try
        //            {
        //                objFilter = new ItemReceipt.ItemReceiptLineFilter();
        //                objFilter.PurchaseOrderLineID = PurchaseOrderLineID;
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

        private List<Fulfillment.FulfillmentLine> mFulfillmentLines = null;
        public List<Fulfillment.FulfillmentLine> FulfillmentLines
        {
            get
            {
                if (mFulfillmentLines == null && !string.IsNullOrEmpty(PurchaseOrderLineID))
                {
                    Fulfillment.FulfillmentLineFilter objFilter = null;

                    try
                    {
                        objFilter = new Fulfillment.FulfillmentLineFilter();
                        objFilter.PurchaseOrderLineID = PurchaseOrderLineID;
                        mFulfillmentLines = Fulfillment.FulfillmentLine.GetFulfillmentLines(objFilter);
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
                return mFulfillmentLines;
            }
        }

        //public int QuantityReceived
        //{
        //    get
        //    {
        //        int intReturn = 0;

        //        if (ItemReceiptLines != null)
        //        {
        //            foreach (ItemReceipt.ItemReceiptLine objItemReceiptLine in ItemReceiptLines)
        //            {
        //                if (!string.IsNullOrEmpty(objItemReceiptLine.ItemReceipt.NetSuiteInternalID))
        //                {
        //                    intReturn += objItemReceiptLine.Quantity;
        //                }
        //            }
        //        }
        //        return intReturn;
        //    }
        //}

        public PurchaseOrderLine()
        {
        }

        public PurchaseOrderLine(string PurchaseOrderLineID)
        {
            this.PurchaseOrderLineID = PurchaseOrderLineID;
            Load();
        }

        public PurchaseOrderLine(DataRow objRow)
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
                //strSQL = "SELECT * " +
                //         "FROM PurchaseOrderLine (NOLOCK) " +
                //         "WHERE PurchaseOrderLineID=" + Database.HandleQuote(PurchaseOrderLineID);

                strSQL = string.Format(@"
SELECT isnull(Carton.CartonQuantity,0) as CartonQuantity, pl.*
FROM PurchaseOrderLine (NOLOCK) pl 
Outer Apply
(
	SELECT sum(cl.Quantity) as CartonQuantity
	FROM CartonLine (nolock) cl
	WHERE cl.PurchaseOrderLineID = pl.PurchaseOrderLineID
) Carton
WHERE PurchaseOrderLineID= {0}
 "
                    , Database.HandleQuote(PurchaseOrderLineID));

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("PurchaseOrderLineID=" + PurchaseOrderLineID + " is not found");
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

                if (objColumns.Contains("PurchaseOrderLineID")) PurchaseOrderLineID = Convert.ToString(objRow["PurchaseOrderLineID"]);
                if (objColumns.Contains("PurchaseOrderID")) PurchaseOrderID = Convert.ToString(objRow["PurchaseOrderID"]);
                if (objColumns.Contains("LineID") && objRow["LineID"] != DBNull.Value) LineID = Convert.ToInt64(objRow["LineID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToInt32(objRow["Quantity"]);
                if (objColumns.Contains("ReceivedQty") && objRow["ReceivedQty"] != DBNull.Value) ReceivedQty = Convert.ToInt32(objRow["ReceivedQty"]);
                if (objColumns.Contains("FOBPrice") && objRow["FOBPrice"] != DBNull.Value) FOBPrice = Convert.ToDecimal(objRow["FOBPrice"]);

                if (objColumns.Contains("IsClosed")) IsClosed = Convert.ToBoolean(objRow["IsClosed"]);
                if (objColumns.Contains("IsClosedByMerchant")) IsClosedByMerchant = Convert.ToBoolean(objRow["IsClosedByMerchant"]);
                if (objColumns.Contains("CancellationReason")) CancellationReason = Convert.ToString(objRow["CancellationReason"]);
                if (objColumns.Contains("DockReceivedQty") && objRow["DockReceivedQty"] != DBNull.Value) DockReceivedQty = Convert.ToInt32(objRow["DockReceivedQty"]);

                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);

                if (objColumns.Contains("CartonQuantity")) CartonQuantity = Convert.ToInt32(objRow["CartonQuantity"]);
                if (objColumns.Contains("SalesOrderLineID")) SalesOrderLineID = Convert.ToString(objRow["SalesOrderLineID"]);

                if (string.IsNullOrEmpty(PurchaseOrderLineID)) throw new Exception("Missing PurchaseOrderLineID in the datarow");
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
                if (string.IsNullOrEmpty(PurchaseOrderID)) throw new Exception("PurchaseOrderID is required");
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, PurchaseOrderLineID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["PurchaseOrderID"] = PurchaseOrderID;
                dicParam["LineID"] = LineID;
                dicParam["ItemID"] = ItemID;
                dicParam["Quantity"] = Quantity;
                dicParam["FOBPrice"] = FOBPrice;
                dicParam["IsClosed"] = IsClosed;
                dicParam["DockReceivedQty"] = DockReceivedQty;
                dicParam["IsClosedByMerchant"] = IsClosedByMerchant;
                dicParam["CancellationReason"] = CancellationReason;
                dicParam["SalesOrderLineID"] = SalesOrderLineID;
                PurchaseOrderLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "PurchaseOrderLine"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(PurchaseOrderID)) throw new Exception("PurchaseOrderID is required");
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (IsNew) throw new Exception("Update cannot be performed, PurchaseOrderLineID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["PurchaseOrderID"] = PurchaseOrderID;
                //dicParam["NetSuiteLineID"] = NetSuiteLineID;
                dicParam["ItemID"] = ItemID;
                dicParam["Quantity"] = Quantity;
                dicParam["ReceivedQty"] = ReceivedQty;
                dicParam["FOBPrice"] = FOBPrice;
                dicParam["IsClosed"] = IsClosed;
                dicParam["IsClosedByMerchant"] = IsClosedByMerchant;
                dicParam["CancellationReason"] = CancellationReason;
                dicParam["SalesOrderLineID"] = SalesOrderLineID;
                dicParam["DockReceivedQty"] = DockReceivedQty;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["PurchaseOrderLineID"] = PurchaseOrderLineID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "PurchaseOrderLine"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, PurchaseOrderLinesID is missing");

                dicDParam["PurchaseOrderLineID"] = PurchaseOrderLineID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "PurchaseOrderLine"), objConn, objTran);
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

        public static PurchaseOrderLine GetPurchaseOrderLine(PurchaseOrderLineFilter Filter)
        {
            List<PurchaseOrderLine> objPurchaseOrderLines = null;
            PurchaseOrderLine objReturn = null;

            try
            {
                objPurchaseOrderLines = GetPurchaseOrderLines(Filter);
                if (objPurchaseOrderLines != null && objPurchaseOrderLines.Count >= 1) objReturn = objPurchaseOrderLines[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objPurchaseOrderLines = null;
            }
            return objReturn;

        }
        public static List<PurchaseOrderLine> GetPurchaseOrderLines()
        {
            int intTotalCount = 0;
            return GetPurchaseOrderLines(null, null, null, out intTotalCount);
        }

        public static List<PurchaseOrderLine> GetPurchaseOrderLines(PurchaseOrderLineFilter Filter)
        {
            int intTotalCount = 0;
            return GetPurchaseOrderLines(Filter, null, null, out intTotalCount);
        }

        public static List<PurchaseOrderLine> GetPurchaseOrderLines(PurchaseOrderLineFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetPurchaseOrderLines(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<PurchaseOrderLine> GetPurchaseOrderLines(PurchaseOrderLineFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<PurchaseOrderLine> objReturn = null;
            PurchaseOrderLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<PurchaseOrderLine>();

                //strSQL = "SELECT * " +
                //         "FROM PurchaseOrderLine (NOLOCK) pl " +
                //         "INNER JOIN PurchaseOrder (NOLOCK) p ON pl.PurchaseOrderID=p.PurchaseOrderID " +
                //         "WHERE 1=1 ";

                strSQL = string.Format(@"
SELECT isnull(Carton.CartonQuantity,0) as CartonQuantity, pl.*
FROM PurchaseOrderLine (NOLOCK) pl 
Outer Apply
(
	SELECT sum(cl.Quantity) as CartonQuantity
	FROM CartonLine (nolock) cl
	WHERE cl.PurchaseOrderLineID = pl.PurchaseOrderLineID
) Carton
WHERE 1=1

 ");

                if (Filter != null)
                {
                    //if (!string.IsNullOrEmpty(Filter.PurchaseOrderID)) strSQL += "AND pl.PurchaseOrderID=" + Database.HandleQuote(Filter.PurchaseOrderID);
                    //if (!string.IsNullOrEmpty(Filter.ItemID)) strSQL += "AND pl.ItemID=" + Database.HandleQuote(Filter.ItemID);

                    if (Filter.PurchaseOrderLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PurchaseOrderLineID, "pl.PurchaseOrderLineID");
                    if (Filter.PurchaseOrderID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PurchaseOrderID, "pl.PurchaseOrderID");
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "pl.ItemID");
                    if (Filter.SalesOrderLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderLineID, "pl.SalesOrderLineID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "PurchaseOrderLineID" : Utility.CustomSorting.GetSortExpression(typeof(PurchaseOrderLine), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new PurchaseOrderLine(objData.Tables[0].Rows[i]);
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

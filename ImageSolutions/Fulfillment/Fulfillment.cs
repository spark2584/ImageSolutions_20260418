using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Fulfillment
{
    public class Fulfillment : ISBase.BaseClass
    {
        public string FulfillmentID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(FulfillmentID); } }
        public string InternalID { get; set; }
        public string PurchaseOrderID { get; set; }
        public string SalesOrderID { get; set; }
        public DateTime? ShipDate { get; set; }
        public string FulfillmentNumber { get; set; }
        public string TrackingNumber { get; set; }
        public string ShippingCarrier { get; set; }
        public decimal? ShippingCost { get; set; }
        public string ShippingMethod { get; set; }
        public bool ShipConfirmationSent { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        //private Customer.Customer mCustomer = null;
        //public Customer.Customer Customer
        //{
        //    get
        //    {
        //        if (mCustomer == null && !string.IsNullOrEmpty(CustomerID))
        //        {
        //            mCustomer = new ImageSolutions.Customer.Customer(CustomerID);
        //        }
        //        return mCustomer;
        //    }
        //}

        private SalesOrder.SalesOrder mSalesOrder = null;
        public SalesOrder.SalesOrder SalesOrder
        {
            get
            {
                if (mSalesOrder == null && !string.IsNullOrEmpty(SalesOrderID))
                {
                    mSalesOrder = new ImageSolutions.SalesOrder.SalesOrder(SalesOrderID);
                }
                return mSalesOrder;
            }
        }

        private PurchaseOrder.PurchaseOrder mPurchaseOrder = null;
        public PurchaseOrder.PurchaseOrder PurchaseOrder
        {
            get
            {
                if (mPurchaseOrder == null && !string.IsNullOrEmpty(PurchaseOrderID))
                {
                    mPurchaseOrder = new ImageSolutions.PurchaseOrder.PurchaseOrder(PurchaseOrderID);
                }
                return mPurchaseOrder;
            }
        }

        private List<FulfillmentLine> mFulfillmentLines = null;
        public List<FulfillmentLine> FulfillmentLines
        {
            get
            {
                if (mFulfillmentLines == null && !string.IsNullOrEmpty(FulfillmentID))
                {
                    FulfillmentLineFilter objFilter = null;
                    try
                    {
                        objFilter = new FulfillmentLineFilter();
                        objFilter.FulfillmentID = FulfillmentID;
                        mFulfillmentLines = FulfillmentLine.GetFulfillmentLines(objFilter);
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
            set
            {
                mFulfillmentLines = value;
            }
        }

        public Fulfillment()
        {
        }
        public Fulfillment(string FulfillmentID)
        {
            this.FulfillmentID = FulfillmentID;
            Load();
        }
        public Fulfillment(DataRow objRow)
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
                         "FROM Fulfillment (NOLOCK) " +
                         "WHERE FulfillmentID=" + Database.HandleQuote(FulfillmentID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("FulfillmentID=" + FulfillmentID + " is not found");
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
                if (objColumns.Contains("FulfillmentID")) FulfillmentID = Convert.ToString(objRow["FulfillmentID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("PurchaseOrderID")) PurchaseOrderID = Convert.ToString(objRow["PurchaseOrderID"]);
                if (objColumns.Contains("SalesOrderID")) SalesOrderID = Convert.ToString(objRow["SalesOrderID"]);
                if (objColumns.Contains("ShipDate") && objRow["ShipDate"] != DBNull.Value) ShipDate = Convert.ToDateTime(objRow["ShipDate"]);
                if (objColumns.Contains("FulfillmentNumber")) FulfillmentNumber = Convert.ToString(objRow["FulfillmentNumber"]);
                if (objColumns.Contains("TrackingNumber")) TrackingNumber = Convert.ToString(objRow["TrackingNumber"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("ShipConfirmationSent")) ShipConfirmationSent = Convert.ToBoolean(objRow["ShipConfirmationSent"]);
                if (objColumns.Contains("ShippingCarrier")) ShippingCarrier = Convert.ToString(objRow["ShippingCarrier"]);
                if (objColumns.Contains("ShippingCost") && objRow["ShippingCost"] != DBNull.Value) ShippingCost = Convert.ToDecimal(objRow["ShippingCost"]);
                if (objColumns.Contains("ShippingMethod")) ShippingMethod = Convert.ToString(objRow["ShippingMethod"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(SalesOrderID)) throw new Exception("Missing SalesOrderID in the datarow");
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
                if (string.IsNullOrEmpty(SalesOrderID) && string.IsNullOrEmpty(PurchaseOrderID)) throw new Exception("SalesOrderID or PurchaseOrderID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, FulfillmentID already exists");

                dicParam["InternalID"] = InternalID;
                dicParam["PurchaseOrderID"] = PurchaseOrderID;
                dicParam["SalesOrderID"] = SalesOrderID;
                dicParam["ShipDate"] = ShipDate;
                dicParam["FulfillmentNumber"] = FulfillmentNumber;
                dicParam["TrackingNumber"] = TrackingNumber;
                dicParam["ShippingCarrier"] = ShippingCarrier;
                dicParam["ShippingCost"] = ShippingCost;
                dicParam["ShippingMethod"] = ShippingMethod;
                dicParam["ShipConfirmationSent"] = ShipConfirmationSent;
                dicParam["ErrorMessage"] = ErrorMessage;

                FulfillmentID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Fulfillment"), objConn, objTran).ToString();

                foreach (FulfillmentLine objFulfillmentLine in FulfillmentLines)
                {
                    objFulfillmentLine.FulfillmentID = FulfillmentID;
                    objFulfillmentLine.Create(objConn, objTran);
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
                if (string.IsNullOrEmpty(SalesOrderID)) throw new Exception("SalesOrderID is required");
                if (IsNew) throw new Exception("Update cannot be performed, FulfillmentID is missing");

                dicParam["InternalID"] = InternalID;
                dicParam["PurchaseOrderID"] = PurchaseOrderID;
                dicParam["SalesOrderID"] = SalesOrderID;
                dicParam["ShipDate"] = ShipDate;
                dicParam["FulfillmentNumber"] = FulfillmentNumber;
                dicParam["TrackingNumber"] = TrackingNumber;
                dicParam["ShippingCarrier"] = ShippingCarrier;
                dicParam["ShippingCost"] = ShippingCost;
                dicParam["ShippingMethod"] = ShippingMethod;
                dicParam["ShipConfirmationSent"] = ShipConfirmationSent;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = "_#GETUTCDATE()";

                dicWParam["FulfillmentID"] = FulfillmentID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Fulfillment"), objConn, objTran);

                foreach (FulfillmentLine objFulfillmentLine in FulfillmentLines)
                {
                    objFulfillmentLine.Update(objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, FulfillmentID is missing");

                //Delete Lines
                List<FulfillmentLine> lstFulfillmentLine;
                lstFulfillmentLine = FulfillmentLines;
                foreach (FulfillmentLine _FulfillmentLine in lstFulfillmentLine)
                {
                    _FulfillmentLine.Delete(objConn, objTran);
                }

                dicDParam["FulfillmentID"] = FulfillmentID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Fulfillment"), objConn, objTran);
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

        public static Fulfillment GetFulfillment(FulfillmentFilter Filter)
        {
            List<Fulfillment> objFulfillments = null;
            Fulfillment objReturn = null;

            try
            {
                objFulfillments = GetFulfillments(Filter);
                if (objFulfillments != null && objFulfillments.Count >= 1) objReturn = objFulfillments[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objFulfillments = null;
            }
            return objReturn;
        }

        public static List<Fulfillment> GetFulfillments()
        {
            int intTotalCount = 0;
            return GetFulfillments(null, null, null, out intTotalCount);
        }

        public static List<Fulfillment> GetFulfillments(FulfillmentFilter Filter)
        {
            int intTotalCount = 0;
            return GetFulfillments(Filter, null, null, out intTotalCount);
        }

        public static List<Fulfillment> GetFulfillments(FulfillmentFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetFulfillments(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Fulfillment> GetFulfillments(FulfillmentFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Fulfillment> objReturn = null;
            Fulfillment objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Fulfillment>();

                strSQL = "SELECT * " +
                         "FROM Fulfillment (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.FulfillmentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.FulfillmentID, "FulfillmentID");
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    //if (Filter.NetSuiteDocumentNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.NetSuiteDocumentNumber, "NetSuiteDocumentNumber");
                    if (Filter.TrackingNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.TrackingNumber, "TrackingNumber");
                    if (Filter.SalesOrderID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderID, "SalesOrderID");
                    if (Filter.PurchaseOrderID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PurchaseOrderID, "PurchaseOrderID");
                    if (Filter.UpdatedOn != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.UpdatedOn, "UpdatedOn");
                    if (Filter.ShipConfirmationSent != null) strSQL += "AND ShipConfirmationSent=" + Database.HandleQuote(Convert.ToInt32(Filter.ShipConfirmationSent.Value).ToString());
                    if (Filter.IncrementID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.IncrementID, "IncrementID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "FulfillmentID" : Utility.CustomSorting.GetSortExpression(typeof(Fulfillment), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Fulfillment(objData.Tables[0].Rows[i]);
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

        //public static Fulfillment IfExternalIDExists(string ExternalID)
        //{
        //    FulfillmentFilter objFilter = null;
        //    Fulfillment objFulfillment = null;
        //    try
        //    {
        //        objFilter = new ImageSolutions.Fulfillment.FulfillmentFilter();
        //        objFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
        //        objFilter.ExternalID.SearchString = ExternalID;
        //        objFulfillment = ImageSolutions.Fulfillment.Fulfillment.GetFulfillment(objFilter);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objFilter = null;
        //    }
        //    return objFulfillment;
        //}

        //public bool UpdateByMagento()
        //{
        //    Magento.SalesOrder objMagentoSO = null;
        //    Toolots.SalesOrder.SalesOrder objSalesOrder = null;
        //    Magento.Product objMagentoProduct = null;
        //    Toolots.Item.Item objItem = null;
        //    try
        //    {
        //        if (ObjectAlreadyExists())
        //        {
        //            //update
        //        }
        //        else
        //        {
        //            //create 
        //            objMagentoSO = new Magento.SalesOrder(MagentoShipment.ShipmentInfo.order_id);
        //            objSalesOrder = new Toolots.SalesOrder.SalesOrder(objMagentoSO);
        //            if (!objSalesOrder.ObjectAlreadyExists()) throw new Exception("Sales Order ExternalID=" + MagentoShipment.ShipmentInfo.order_id + " not found in toolots");

        //            this.SalesOrderID = objSalesOrder.SalesOrderID;
        //            this.CustomerID = objSalesOrder.CustomerID;
        //            this.ExternalID = MagentoShipment.ShipmentInfo.shipment_id;
        //            this.IncrementID = MagentoShipment.ShipmentInfo.increment_id;
        //            //this.ShipDate = MagentoShipment.ShipmentInfo.tracks[1].

        //            if (MagentoShipment.ShipmentInfo.items != null && MagentoShipment.ShipmentInfo.items.Count() > 0)
        //            {
        //                FulfillmentLines = new List<FulfillmentLine>();
        //                foreach (Magento.MagentoService.salesOrderShipmentItemEntity objShipItem in MagentoShipment.ShipmentInfo.items)
        //                {
        //                    if (objSalesOrder.SalesOrderLines.Any(s => s.ExternalID == objShipItem.order_item_id))
        //                    {
        //                        objMagentoProduct = new Magento.Product(objShipItem.product_id);
        //                        objItem = new Item.Item(objMagentoProduct);
        //                        if (!objItem.ObjectAlreadyExists()) throw new Exception("Item externalID " + objShipItem.product_id + " not found in Toolots");
        //                        FulfillmentLine objFulfillmentLine = new FulfillmentLine();
        //                        objFulfillmentLine.ExternalID = objShipItem.item_id;
        //                        objFulfillmentLine.SalesOrderLineID = objSalesOrder.SalesOrderLines.First(s => s.ExternalID == objShipItem.order_item_id).SalesOrderLineID;
        //                        objFulfillmentLine.UnitPrice = Convert.ToDecimal(objShipItem.price);
        //                        objFulfillmentLine.Quantity = Convert.ToInt32(Convert.ToDecimal(objShipItem.qty.Trim()));
        //                        objFulfillmentLine.ItemID = objItem.ItemID;
        //                        FulfillmentLines.Add(objFulfillmentLine);
        //                    }
        //                    else
        //                    {
        //                        throw new Exception("ShipmentLine SalesOrderline not found in Toolots ");
        //                    }
        //                }
        //            }
        //            Create();

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return true;
        //}
    }
}

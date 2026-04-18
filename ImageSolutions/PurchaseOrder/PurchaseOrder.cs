using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.PurchaseOrder
{
    public class PurchaseOrder : ISBase.BaseClass
    {
        public string PurchaseOrderID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(PurchaseOrderID); } }
        public string PurchaseOrderNumber { get; set; }
        public string VendorID { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string InternalID { get; set; }
        public string ExternalID { get; set; }
        public string ShipToAddressID { get; set; }
        public string ShippingMethod { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        private Address.AddressTrans mShipToAddress = null;
        public Address.AddressTrans ShipToAddress
        {
            get
            {
                if (mShipToAddress == null && !string.IsNullOrEmpty(ShipToAddressID))
                {
                    mShipToAddress = new ImageSolutions.Address.AddressTrans(ShipToAddressID);
                }
                return mShipToAddress;
            }
        }
        private Vendor.Vendor mVendor = null;
        public Vendor.Vendor Vendor
        {
            get
            {
                if (mVendor == null && !string.IsNullOrEmpty(VendorID))
                {
                    mVendor = new ImageSolutions.Vendor.Vendor(VendorID);
                }
                return mVendor;
            }
        }

        private List<PurchaseOrderLine> mPurchaseOrderLines = null;
        public List<PurchaseOrderLine> PurchaseOrderLines
        {
            get
            {
                if (mPurchaseOrderLines == null && !string.IsNullOrEmpty(PurchaseOrderID))
                {
                    PurchaseOrderLineFilter objFilter = null;

                    try
                    {
                        //objFilter = new PurchaseOrderLineFilter();
                        //objFilter.PurchaseOrderID = PurchaseOrderID;

                        objFilter = new PurchaseOrderLineFilter();
                        objFilter.PurchaseOrderID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.PurchaseOrderID.SearchString = PurchaseOrderID;

                        mPurchaseOrderLines = PurchaseOrderLine.GetPurchaseOrderLines(objFilter);
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
                return mPurchaseOrderLines;
            }
            set
            {
                mPurchaseOrderLines = value;
            }
        }

        public PurchaseOrder()
        {
        }
        public PurchaseOrder(string PurchaseOrderID)
        {
            this.PurchaseOrderID = PurchaseOrderID;
            Load();
        }
        public PurchaseOrder(DataRow objRow)
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
                //         "FROM PurchaseOrder (NOLOCK) " +
                //         "WHERE PurchaseOrderID=" + Database.HandleQuote(PurchaseOrderID);

                strSQL = string.Format(@"
SELECT p.*
FROM PurchaseOrder (NOLOCK) p
WHERE PurchaseOrderID={0}", Database.HandleQuote(PurchaseOrderID));

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("PurchaseOrderID=" + PurchaseOrderID + " is not found");
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

                if (objColumns.Contains("PurchaseOrderID")) PurchaseOrderID = Convert.ToString(objRow["PurchaseOrderID"]);
                if (objColumns.Contains("PurchaseOrderNumber")) PurchaseOrderNumber = Convert.ToString(objRow["PurchaseOrderNumber"]);
                if (objColumns.Contains("VendorID")) VendorID = Convert.ToString(objRow["VendorID"]);
                if (objColumns.Contains("TransactionDate") && objRow["TransactionDate"] != DBNull.Value) TransactionDate = Convert.ToDateTime(objRow["TransactionDate"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
                if (objColumns.Contains("ShipToAddressID")) ShipToAddressID = Convert.ToString(objRow["ShipToAddressID"]);
                if (objColumns.Contains("ShippingMethod")) ShippingMethod = Convert.ToString(objRow["ShippingMethod"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(PurchaseOrderID)) throw new Exception("Missing PurchaseOrderID in the datarow");
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
                if (TransactionDate == null) throw new Exception("TransactionDate is required");
                if (string.IsNullOrEmpty(VendorID)) throw new Exception("VendorID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, PurchaseOrderID already exists");

                dicParam["PurchaseOrderNumber"] = PurchaseOrderNumber;
                dicParam["VendorID"] = VendorID;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["InternalID"] = InternalID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["ShipToAddressID"] = ShipToAddressID;
                dicParam["ShippingMethod"] = ShippingMethod;
                dicParam["ErrorMessage"] = ErrorMessage;

                PurchaseOrderID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "PurchaseOrder"), objConn, objTran).ToString();

                foreach (PurchaseOrderLine objPurchaseOrderLine in PurchaseOrderLines)
                {
                    objPurchaseOrderLine.PurchaseOrderID = PurchaseOrderID;
                    objPurchaseOrderLine.Create(objConn, objTran);
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
                if (TransactionDate == null) throw new Exception("TransactionDate is required");
                if (string.IsNullOrEmpty(VendorID)) throw new Exception("VendorID is required");
                if (IsNew) throw new Exception("Update cannot be performed, PurchaseOrderID is missing");

                dicParam["PurchaseOrderNumber"] = PurchaseOrderNumber;
                dicParam["VendorID"] = VendorID;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["InternalID"] = InternalID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["ShipToAddressID"] = ShipToAddressID;
                dicParam["ShippingMethod"] = ShippingMethod;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = DateTime.UtcNow;

                dicWParam["PurchaseOrderID"] = PurchaseOrderID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "PurchaseOrder"), objConn, objTran);

                foreach (PurchaseOrderLine objPurchaseOrderLine in PurchaseOrderLines)
                {
                    objPurchaseOrderLine.Update(objConn, objTran);
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

        //public bool Update(SqlConnection objConn, SqlTransaction objTran, bool isNested = true)
        //{
        //    base.Update();

        //    if (!IsActive) return Delete(objConn, objTran);

        //    Hashtable dicParam = new Hashtable();
        //    Hashtable dicWParam = new Hashtable();

        //    try
        //    {
        //        if (TransactionDate == null) throw new Exception("TransactionDate is required");
        //        if (string.IsNullOrEmpty(VendorID)) throw new Exception("VendorID is required");
        //        if (IsNew) throw new Exception("Update cannot be performed, PurchaseOrderID is missing");

        //        //dicParam["NetSuiteInternalID"] = NetSuiteInternalID;
        //        dicParam["TransactionDate"] = TransactionDate;
        //        //dicParam["NetSuiteInternalID"] = NetSuiteInternalID;
        //        //dicParam["NetSuiteDocumentNumber"] = NetSuiteDocumentNumber;
        //        dicParam["VendorID"] = VendorID;
        //        dicParam["PackingListNumber"] = PackingListNumber;
        //        //dicParam["MagentoPartnerID"] = MagentoPartnerID;
        //        //dicParam["MagentoVendorID"] = MagentoVendorID;
        //        dicParam["ErrorMessage"] = ErrorMessage;
        //        dicParam["UpdatedOn"] = DateTime.UtcNow;

        //        dicParam["Status"] = Status;
        //        dicParam["IsInboundShipment"] = IsInboundShipment;
        //        //dicParam["PackingSlipNumber"] = PackingSlipNumber;
        //        dicParam["ShipmentDate"] = ShipmentDate;

        //        dicWParam["PurchaseOrderID"] = PurchaseOrderID;

        //        Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "PurchaseOrder"), objConn, objTran);

        //        if (isNested)
        //        {
        //            foreach (PurchaseOrderLine objPurchaseOrderLine in PurchaseOrderLines)
        //            {
        //                objPurchaseOrderLine.Update(objConn, objTran);
        //            }
        //        }

        //        Load(objConn, objTran);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        dicParam = null;
        //        dicWParam = null;
        //    }
        //    return true;
        //}

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
                if (IsNew) throw new Exception("Delete cannot be performed, PurchaseOrderID is missing");

                //Delete Purchase order lines
                List<PurchaseOrderLine> lstPurchaseOrderLine;
                lstPurchaseOrderLine = this.PurchaseOrderLines;
                foreach (PurchaseOrderLine _purchaseOrderLine in lstPurchaseOrderLine)
                {
                    _purchaseOrderLine.Delete(objConn, objTran);
                }

                dicDParam["PurchaseOrderID"] = PurchaseOrderID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "PurchaseOrder"), objConn, objTran);
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

        public static PurchaseOrder GetPurchaseOrder(PurchaseOrderFilter Filter)
        {
            List<PurchaseOrder> objPurchaseOrders = null;
            PurchaseOrder objReturn = null;

            try
            {
                objPurchaseOrders = GetPurchaseOrders(Filter);
                if (objPurchaseOrders != null && objPurchaseOrders.Count >= 1) objReturn = objPurchaseOrders[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objPurchaseOrders = null;
            }
            return objReturn;
        }

        public static List<PurchaseOrder> GetPurchaseOrders()
        {
            int intTotalCount = 0;
            return GetPurchaseOrders(null, null, null, out intTotalCount);
        }

        public static List<PurchaseOrder> GetPurchaseOrders(PurchaseOrderFilter Filter)
        {
            int intTotalCount = 0;
            return GetPurchaseOrders(Filter, null, null, out intTotalCount);
        }

        public static List<PurchaseOrder> GetPurchaseOrders(PurchaseOrderFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetPurchaseOrders(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<PurchaseOrder> GetPurchaseOrders(PurchaseOrderFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<PurchaseOrder> objReturn = null;
            PurchaseOrder objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<PurchaseOrder>();

                //strSQL = "SELECT * " +
                //         "FROM PurchaseOrder (NOLOCK) " +
                //         "WHERE MagentoVendorID NOT IN (12) ";

                strSQL = string.Format(@"
SELECT p.*
FROM PurchaseOrder (NOLOCK) p
WHERE 1=1
");


                if (Filter != null)
                {
                    if (Filter.PurchaseOrderID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PurchaseOrderID, "PurchaseOrderID");
                    if (Filter.PurchaseOrderNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PurchaseOrderNumber, "PurchaseOrderNumber");
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.ExternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ExternalID, "ExternalID");
                    if (Filter.VendorID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.VendorID, "VendorID");
                    if (Filter.ShipToAddressID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ShipToAddressID, "ShipToAddressID");
                    if (Filter.PurchaseOrderID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PurchaseOrderID, "PurchaseOrderID");
                    if (Filter.UpdatedOn != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.UpdatedOn, "UpdatedOn");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "PurchaseOrderID" : Utility.CustomSorting.GetSortExpression(typeof(PurchaseOrder), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new PurchaseOrder(objData.Tables[0].Rows[i]);
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

        public static List<PurchaseOrder> GetPurchaseOrdersWithLines(string VendorID)
        {
            List<PurchaseOrder> objReturn = null;
            PurchaseOrder objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                objReturn = new List<PurchaseOrder>();

                strSQL = string.Format(@"
SELECT p.*
FROM PurchaseOrder (NOLOCK) p
Outer Apply
(
	SELECT Count(pl.PurchaseOrderLIneID) LineCount
	FROM PurchaseOrderLine (NOLOCK) pl
	WHERE pl.PurchaseOrderID = p.PurchaseOrderID
)lines
WHERE p.VendorID = {0}
and p.IsInboundShipment = 1
and lines.LineCount > 0
ORDER BY PurchaseOrderID DESC
", Database.HandleQuote(VendorID));

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new PurchaseOrder(objData.Tables[0].Rows[i]);
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
        public static List<PurchaseOrder> GetPurchaseOrdersAllVendorWithLines(string VendorID)
        {
            List<PurchaseOrder> objReturn = null;
            PurchaseOrder objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                objReturn = new List<PurchaseOrder>();
                if (string.IsNullOrEmpty(VendorID)) return objReturn;
                strSQL = string.Format(@"
SELECT p.*,Vendor.MerchantID
FROM PurchaseOrder (NOLOCK) p
Outer Apply
(
	SELECT Count(pl.PurchaseOrderLIneID) LineCount
	FROM PurchaseOrderLine (NOLOCK) pl
	WHERE pl.PurchaseOrderID = p.PurchaseOrderID
)lines
inner join Vendor on p.VendorID = Vendor.VendorID
WHERE p.VendorID in ({0})
and p.IsInboundShipment = 1
and lines.LineCount > 0
ORDER BY PurchaseOrderID DESC
", VendorID);

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new PurchaseOrder(objData.Tables[0].Rows[i]);
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
        public bool isValidToShip()
        {
            bool _return = true;

            if (!string.IsNullOrEmpty(PurchaseOrderID))
            {
                DataSet objData;
                string strSQL = string.Empty;

                try
                {
                    strSQL = "EXEC spGetCartonTotal " + Database.HandleQuote(PurchaseOrderID);
                    objData = Database.GetDataSet(strSQL);

                    if (objData != null && objData.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                        {
                            if (Convert.ToInt32(objData.Tables[0].Rows[i]["OrderLineQuantity"]) != Convert.ToInt32(objData.Tables[0].Rows[i]["CartonQuantity"]))
                                _return = false;
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
            }

            return _return;
        }
    }
}

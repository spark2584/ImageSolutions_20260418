using ImageSolutions.User;
using ImageSolutions.Website;
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


namespace ImageSolutions.RMA
{
    public class RMA : ISBase.BaseClass
    {
        public string RMAID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(RMAID); } }
        public string WebsiteID { get; set; }
        public string SalesOrderID { get; set; }
        public string InternalID { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string WebsiteShippingServiceID { get; set; }
        public string TrackingNumber { get; set; }
        public double TotalAmount { get; set; }
        public double ShippingAmount { get; set; }
        public double TaxAmount { get; set; }
        public string Memo { get; set; }
        public bool RequestReturnLabel { get; set; }
        public string ReferenceNumber { get; set; }
        public string Reason { get; set; }
        public string ShippingLabelPath { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        public double LineTotal
        {
            get
            {
                double dcmReturn = 0;
                if (RMALines != null)
                {
                    dcmReturn = RMALines.Sum(m => m.LineTotal);
                }
                return Math.Round(dcmReturn, 2);
            }
        }

        public int TotalQuantity
        {
            get
            {
                int intReturn = 0;
                if (RMALines != null)
                {
                    intReturn = RMALines.Sum(m => m.Quantity);
                }
                return intReturn;
            }
        }

        private List<RMALine> mRMALines = null;
        public List<RMALine> RMALines
        {
            get
            {
                if (mRMALines == null && !string.IsNullOrEmpty(RMAID))
                {
                    RMALineFilter objFilter = null;

                    try
                    {
                        objFilter = new RMALineFilter();
                        objFilter.RMAID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.RMAID.SearchString = RMAID;
                        mRMALines = RMALine.GetRMALines(objFilter);
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
                return mRMALines;
            }
            set
            {
                mRMALines = value;
            }
        }

        private List<ItemReceipt.ItemReceipt> mItemReceipts = null;
        public List<ItemReceipt.ItemReceipt> ItemReceipts
        {
            get
            {
                if (mItemReceipts == null && !string.IsNullOrEmpty(RMAID))
                {
                    ItemReceipt.ItemReceiptFilter objFilter = null;
                    try
                    {
                        objFilter = new ItemReceipt.ItemReceiptFilter();
                        objFilter.RMAID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.RMAID.SearchString = RMAID;
                        mItemReceipts = ItemReceipt.ItemReceipt.GetItemReceipts(objFilter);
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
                return mItemReceipts;
            }
            set
            {
                mItemReceipts = value;
            }
        }

        private Website.Website mWebsite = null;
        public Website.Website Website
        {
            get
            {
                if (mWebsite == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    mWebsite = new Website.Website(WebsiteID);
                }
                return mWebsite;
            }
            set
            {
                mWebsite = value;
            }
        }

        private SalesOrder.SalesOrder mSalesOrder = null;
        public SalesOrder.SalesOrder SalesOrder
        {
            get
            {
                if (mSalesOrder == null && !string.IsNullOrEmpty(SalesOrderID))
                {
                    mSalesOrder = new SalesOrder.SalesOrder(SalesOrderID);
                }
                return mSalesOrder;
            }
            set
            {
                mSalesOrder = value;
            }
        }


        private WebsiteShippingService mWebsiteShippingService = null;
        public WebsiteShippingService WebsiteShippingService
        {
            get
            {
                if (mWebsiteShippingService == null && !string.IsNullOrEmpty(WebsiteShippingServiceID))
                {
                    mWebsiteShippingService = new WebsiteShippingService(WebsiteShippingServiceID);
                }
                return mWebsiteShippingService;
            }
            set
            {
                mWebsiteShippingService = value;
            }
        }

        public RMA()
        {
        }

        public RMA(string RMAID)
        {
            this.RMAID = RMAID;
            Load();
        }

        public RMA(DataRow objRow)
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
                         "FROM RMA (NOLOCK) " +
                         "WHERE RMAID=" + Database.HandleQuote(RMAID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("RMAID=" + RMAID + " is not found");
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

                if (objColumns.Contains("RMAID")) RMAID = Convert.ToString(objRow["RMAID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("SalesOrderID")) SalesOrderID = Convert.ToString(objRow["SalesOrderID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("DocumentNumber")) DocumentNumber = Convert.ToString(objRow["DocumentNumber"]);
                if (objColumns.Contains("TransactionDate") && objRow["TransactionDate"] != DBNull.Value) TransactionDate = Convert.ToDateTime(objRow["TransactionDate"]);
                if (objColumns.Contains("WebsiteShippingServiceID")) WebsiteShippingServiceID = Convert.ToString(objRow["WebsiteShippingServiceID"]);
                if (objColumns.Contains("TrackingNumber")) TrackingNumber = Convert.ToString(objRow["TrackingNumber"]);
                if (objColumns.Contains("ShippingAmount")) ShippingAmount = Convert.ToDouble(objRow["ShippingAmount"]);
                if (objColumns.Contains("TaxAmount") && objRow["TaxAmount"] != DBNull.Value) TaxAmount = Convert.ToDouble(objRow["TaxAmount"]);
                if (objColumns.Contains("Memo")) Memo = Convert.ToString(objRow["Memo"]);
                if (objColumns.Contains("RequestReturnLabel") && objRow["RequestReturnLabel"] != DBNull.Value) RequestReturnLabel = Convert.ToBoolean(objRow["RequestReturnLabel"]);
                if (objColumns.Contains("ReferenceNumber")) ReferenceNumber = Convert.ToString(objRow["ReferenceNumber"]);
                if (objColumns.Contains("Reason")) Reason = Convert.ToString(objRow["Reason"]);
                if (objColumns.Contains("ShippingLabelPath")) ShippingLabelPath = Convert.ToString(objRow["ShippingLabelPath"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
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
                if (string.IsNullOrEmpty(WebsiteID)) throw new Exception("WebsiteID is required");
                if (string.IsNullOrEmpty(SalesOrderID)) throw new Exception("SalesOrderID is required");
                if (TransactionDate == null) throw new Exception("TransactionDate is required");
                if (!IsNew) throw new Exception("Create cannot be performed, RMAID already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["SalesOrderID"] = SalesOrderID;
                dicParam["InternalID"] = InternalID;
                dicParam["DocumentNumber"] = DocumentNumber;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["WebsiteShippingServiceID"] = WebsiteShippingServiceID;
                dicParam["TrackingNumber"] = TrackingNumber;
                dicParam["ShippingAmount"] = ShippingAmount;
                dicParam["TaxAmount"] = TaxAmount;
                dicParam["Memo"] = Memo;
                dicParam["RequestReturnLabel"] = RequestReturnLabel;
                dicParam["ReferenceNumber"] = ReferenceNumber;
                dicParam["Reason"] = Reason;
                dicParam["ShippingLabelPath"] = ShippingLabelPath;
                dicParam["ErrorMessage"] = ErrorMessage;

                RMAID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "RMA"), objConn, objTran).ToString();

                foreach (RMALine objRMALine in RMALines)
                {
                    objRMALine.RMAID = RMAID;
                    objRMALine.Create(objConn, objTran);
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
                if (string.IsNullOrEmpty(WebsiteID)) throw new Exception("WebsiteID is required");
                if (string.IsNullOrEmpty(SalesOrderID)) throw new Exception("SalesOrderID is required");
                if (TransactionDate == null) throw new Exception("TransactionDate is required");
                if (IsNew) throw new Exception("Update cannot be performed, RMAID is missing");


                dicParam["WebsiteID"] = WebsiteID;
                dicParam["SalesOrderID"] = SalesOrderID;
                dicParam["InternalID"] = InternalID;
                dicParam["DocumentNumber"] = DocumentNumber;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["WebsiteShippingServiceID"] = WebsiteShippingServiceID;
                dicParam["TrackingNumber"] = TrackingNumber;
                dicParam["ShippingAmount"] = ShippingAmount;
                dicParam["TaxAmount"] = TaxAmount;
                dicParam["Memo"] = Memo;
                dicParam["RequestReturnLabel"] = RequestReturnLabel;
                dicParam["ReferenceNumber"] = ReferenceNumber;
                dicParam["Reason"] = Reason;
                dicParam["ShippingLabelPath"] = ShippingLabelPath;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["RMAID"] = RMAID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "RMA"), objConn, objTran);

                foreach (RMALine objRMALine in RMALines)
                {
                    objRMALine.Update(objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, RMAID is missing");

                dicDParam["RMAID"] = RMAID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "RMA"), objConn, objTran);
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

        public static RMA GetRMA(RMAFilter Filter)
        {
            List<RMA> objRMAs = null;
            RMA objReturn = null;

            try
            {
                objRMAs = GetRMAs(Filter);
                if (objRMAs != null && objRMAs.Count >= 1) objReturn = objRMAs[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objRMAs = null;
            }
            return objReturn;
        }

        public static List<RMA> GetRMAs()
        {
            int intTotalCount = 0;
            return GetRMAs(null, null, null, out intTotalCount);
        }

        public static List<RMA> GetRMAs(RMAFilter Filter)
        {
            int intTotalCount = 0;
            return GetRMAs(Filter, null, null, out intTotalCount);
        }

        public static List<RMA> GetRMAs(RMAFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetRMAs(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<RMA> GetRMAs(RMAFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<RMA> objReturn = null;
            RMA objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<RMA>();

                strSQL = "SELECT * " +
                         "FROM RMA (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.SalesOrderID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderID, "SalesOrderID");
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.DocumentNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.DocumentNumber, "DocumentNumber");
                    if (Filter.UpdatedOn != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.UpdatedOn, "UpdatedOn");
                    if (Filter.ErrorMessage != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ErrorMessage, "ErrorMessage");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "RMAID" : Utility.CustomSorting.GetSortExpression(typeof(RMA), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new RMA(objData.Tables[0].Rows[i]);
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

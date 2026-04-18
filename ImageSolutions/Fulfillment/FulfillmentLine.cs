using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ImageSolutions.Fulfillment
{
    public class FulfillmentLine : ISBase.BaseClass
    {
        public string FulfillmentLineID { get; private set; }
        public string FulfillmentID { get; set; }
        public string SalesOrderLineID { get; set; }
        public string PurchaseOrderLineID { get; set; }
        public long? LineID { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(FulfillmentLineID); } }
        public string ItemID { get; set; }
        public double Quantity { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedOn { get; private set; }
        public DateTime? UpdatedOn { get; private set; }

        private SalesOrder.SalesOrderLine mSalesOrderLine = null;
        public SalesOrder.SalesOrderLine SalesOrderLine
        {
            get
            {
                if (mSalesOrderLine == null && !string.IsNullOrEmpty(SalesOrderLineID))
                {
                    mSalesOrderLine = new SalesOrder.SalesOrderLine(SalesOrderLineID);
                }
                return mSalesOrderLine;
            }
        }
        private PurchaseOrder.PurchaseOrderLine mPurchaseOrderLine = null;
        public PurchaseOrder.PurchaseOrderLine PurchaseOrderLine
        {
            get
            {
                if (mPurchaseOrderLine == null && !string.IsNullOrEmpty(PurchaseOrderLineID))
                {
                    mPurchaseOrderLine = new PurchaseOrder.PurchaseOrderLine(PurchaseOrderLineID);
                }
                return mPurchaseOrderLine;
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

        private Fulfillment mFulfillment = null;
        public Fulfillment Fulfillment
        {
            get
            {
                if (mFulfillment == null && !string.IsNullOrEmpty(FulfillmentID))
                {
                    mFulfillment = new Fulfillment(FulfillmentID);
                }
                return mFulfillment;
            }
        }

        public FulfillmentLine()
        {
        }

        public FulfillmentLine(string FulfillmentLineID)
        {
            this.FulfillmentLineID = FulfillmentLineID;
            Load();
        }

        public FulfillmentLine(DataRow objRow)
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
                         "FROM FulfillmentLine (NOLOCK) " +
                         "WHERE FulfillmentLineID=" + Database.HandleQuote(FulfillmentLineID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("FulfillmentLineID=" + FulfillmentLineID + " is not found");
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

                if (objColumns.Contains("FulfillmentLineID")) FulfillmentLineID = Convert.ToString(objRow["FulfillmentLineID"]);
                if (objColumns.Contains("FulfillmentID")) FulfillmentID = Convert.ToString(objRow["FulfillmentID"]);
                if (objColumns.Contains("PurchaseOrderLineID")) PurchaseOrderLineID = Convert.ToString(objRow["PurchaseOrderLineID"]);
                if (objColumns.Contains("SalesOrderLineID")) SalesOrderLineID = Convert.ToString(objRow["SalesOrderLineID"]);
                if (objColumns.Contains("LineID") && objRow["LineID"] != DBNull.Value) LineID = Convert.ToInt64(objRow["LineID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToDouble(objRow["Quantity"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);

                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);

                if (string.IsNullOrEmpty(FulfillmentLineID)) throw new Exception("Missing FulfillmentLineID in the datarow");
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
                if (string.IsNullOrEmpty(SalesOrderLineID) && string.IsNullOrEmpty(PurchaseOrderLineID)) throw new Exception("SalesOrderLineID or PurchaseOrderLineID is required");
                if (string.IsNullOrEmpty(FulfillmentID)) throw new Exception("FulfillmentID is required");
                //if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (Quantity <= 0) throw new Exception("Quantity cannot be less than or equal to 0");
                if (!IsNew) throw new Exception("Create cannot be performed, FulfillmentLineID already exists");

                dicParam["FulfillmentID"] = FulfillmentID;
                dicParam["PurchaseOrderLineID"] = PurchaseOrderLineID;
                dicParam["PurchaseOrderLineID"] = PurchaseOrderLineID;
                dicParam["SalesOrderLineID"] = SalesOrderLineID;
                dicParam["LineID"] = LineID;
                dicParam["ItemID"] = ItemID;
                dicParam["Quantity"] = Quantity;

                dicParam["ErrorMessage"] = ErrorMessage;
             
                FulfillmentLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "FulfillmentLine"), objConn, objTran).ToString();

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
                //if (string.IsNullOrEmpty(SalesOrderLineID)) throw new Exception("SalesOrderLineID is required");
                if (string.IsNullOrEmpty(FulfillmentID)) throw new Exception("FulfillmentID is required");
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (Quantity <= 0) throw new Exception("Quantity cannot be less than or equal to 0");
                if (IsNew) throw new Exception("Update cannot be performed, SalesOrderLineID is missing");

                dicParam["FulfillmentID"] = FulfillmentID;
                dicParam["PurchaseOrderLineID"] = PurchaseOrderLineID;
                dicParam["SalesOrderLineID"] = SalesOrderLineID;
                dicParam["LineID"] = LineID;
                dicParam["ItemID"] = ItemID;
                dicParam["Quantity"] = Quantity;
                dicParam["ErrorMessage"] = ErrorMessage;

                dicParam["UpdatedOn"] = "_#GETUTCDATE()";
                dicWParam["FulfillmentLineID"] = FulfillmentLineID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "FulfillmentLine"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, FulfillmentLineID is missing");

                dicDParam["FulfillmentLineID"] = FulfillmentLineID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "FulfillmentLine"), objConn, objTran);
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

        public static List<FulfillmentLine> GetFulfillmentLine()
        {
            int intTotalCount = 0;
            return GetFulfillmentLines(null, null, null, out intTotalCount);
        }

        public static List<FulfillmentLine> GetFulfillmentLines(FulfillmentLineFilter Filter)
        {
            int intTotalCount = 0;
            return GetFulfillmentLines(Filter, null, null, out intTotalCount);
        }

        public static List<FulfillmentLine> GetFulfillmentLines(FulfillmentLineFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetFulfillmentLines(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<FulfillmentLine> GetFulfillmentLines(FulfillmentLineFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<FulfillmentLine> objReturn = null;
            FulfillmentLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<FulfillmentLine>();

                strSQL = "SELECT * " +
                         "FROM FulfillmentLine (NOLOCK) fl " +
                         "INNER JOIN Fulfillment (NOLOCK) f ON fl.FulfillmentID=f.FulfillmentID " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (!string.IsNullOrEmpty(Filter.FulfillmentID)) strSQL += "AND fl.FulfillmentID=" + Database.HandleQuote(Filter.FulfillmentID);
                    if (!string.IsNullOrEmpty(Filter.SalesOrderLineID)) strSQL += " AND fl.SalesOrderLineID=" + Database.HandleQuote(Filter.SalesOrderLineID);
                    if (!string.IsNullOrEmpty(Filter.PurchaseOrderLineID)) strSQL += " AND fl.PurchaseOrderLineID=" + Database.HandleQuote(Filter.PurchaseOrderLineID);

                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "FulfillmentLineID" : Utility.CustomSorting.GetSortExpression(typeof(FulfillmentLine), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new FulfillmentLine(objData.Tables[0].Rows[i]);
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

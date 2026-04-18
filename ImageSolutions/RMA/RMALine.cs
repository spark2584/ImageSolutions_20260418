using ImageSolutions.SalesOrder;
using ImageSolutions.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.RMA
{
    public class RMALine : ISBase.BaseClass
    {
        public string RMALineID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(RMALineID); } }
        public string RMAID { get; set; }
        public string SalesOrderLineID { get; set; }
        public long? NetSuiteLineID { get; set; }
        public string ItemID { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public double LineTotal { get { return Quantity * UnitPrice; } }

        private RMA mRMA = null;
        public RMA RMA
        {
            get
            {
                if (mRMA == null && !string.IsNullOrEmpty(RMAID))
                {
                    mRMA = new RMA(RMAID);
                }
                return mRMA;
            }
        }

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
            set
            {
                mSalesOrderLine = value;
            }
        }

        private Item.Item mItem = null;
        public Item.Item Item
        {
            get
            {
                if (mItem == null && !string.IsNullOrEmpty(ItemID))
                {
                    mItem = new Item.Item(ItemID);
                }
                return mItem;
            }
            set
            {
                mItem = value;
            }
        }

        //private List<Fulfillment.FulfillmentLine> mFulfillmentLines = null;
        //public List<Fulfillment.FulfillmentLine> FulfillmentLines
        //{
        //    get
        //    {
        //        if (mFulfillmentLines == null && !string.IsNullOrEmpty(RMALineID))
        //        {
        //            Fulfillment.FulfillmentLineFilter objFilter = null;

        //            try
        //            {
        //                objFilter = new Fulfillment.FulfillmentLineFilter();
        //                objFilter.RMALineID = RMALineID;
        //                mFulfillmentLines = Fulfillment.FulfillmentLine.GetFulfillmentLines(objFilter);
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
        //        return mFulfillmentLines;
        //    }
        //}

        public RMALine()
        {
        }

        public RMALine(string RMALineID)
        {
            this.RMALineID = RMALineID;
            Load();
        }

        public RMALine(DataRow objRow)
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
                strSQL = string.Format(@"
                                SELECT * 
                                FROM RMALine (NOLOCK)
                                WHERE RMALineID = {0} "
                                                    , Database.HandleQuote(RMALineID)
                                );

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("RMALineID=" + RMALineID + " is not found");
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

                if (objColumns.Contains("RMALineID")) RMALineID = Convert.ToString(objRow["RMALineID"]);
                if (objColumns.Contains("RMAID")) RMAID = Convert.ToString(objRow["RMAID"]);
                if (objColumns.Contains("SalesOrderLineID")) SalesOrderLineID = Convert.ToString(objRow["SalesOrderLineID"]);
                if (objColumns.Contains("NetSuiteLineID") && objRow["NetSuiteLineID"] != DBNull.Value) NetSuiteLineID = Convert.ToInt64(objRow["NetSuiteLineID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("Quantity") && objRow["Quantity"] != DBNull.Value) Quantity = Convert.ToInt32(objRow["Quantity"]);
                if (objColumns.Contains("UnitPrice") && objRow["UnitPrice"] != DBNull.Value) UnitPrice = Convert.ToDouble(objRow["UnitPrice"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
                
                if (string.IsNullOrEmpty(RMALineID)) throw new Exception("Missing RMALineID in the datarow");
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
                if (string.IsNullOrEmpty(RMAID)) throw new Exception("RMAID is required");
                if (string.IsNullOrEmpty(SalesOrderLineID)) throw new Exception("SalesOrderLineID is required");
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (Quantity <= 0) throw new Exception("Quantity cannot be less than or equal to 0");
                if (!IsNew) throw new Exception("Create cannot be performed, RMALineID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["RMAID"] = RMAID;
                dicParam["SalesOrderLineID"] = SalesOrderLineID;
                dicParam["NetSuiteLineID"] = NetSuiteLineID;
                dicParam["ItemID"] = ItemID;
                dicParam["Quantity"] = Quantity;
                dicParam["UnitPrice"] = UnitPrice;
                RMALineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "RMALine"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(RMAID)) throw new Exception("RMAID is required");
                if (string.IsNullOrEmpty(SalesOrderLineID)) throw new Exception("SalesOrderLineID is required");
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (Quantity <= 0) throw new Exception("Quantity cannot be less than or equal to 0");
                if (IsNew) throw new Exception("Update cannot be performed, RMALineID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["RMAID"] = RMAID;
                dicParam["SalesOrderLineID"] = SalesOrderLineID;
                dicParam["NetSuiteLineID"] = NetSuiteLineID;
                dicParam["ItemID"] = ItemID;
                dicParam["Quantity"] = Quantity;
                dicParam["UnitPrice"] = UnitPrice;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["RMALineID"] = RMALineID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "RMALine"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, RMALinesID is missing");

                dicDParam["RMALineID"] = RMALineID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "RMALine"), objConn, objTran);
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

        public static RMALine GetRMALine(RMALineFilter Filter)
        {
            List<RMALine> objRMALines = null;
            RMALine objReturn = null;

            try
            {
                objRMALines = GetRMALines(Filter);
                if (objRMALines != null && objRMALines.Count >= 1) objReturn = objRMALines[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objRMALines = null;
            }
            return objReturn;

        }

        public static List<RMALine> GetRMALines()
        {
            int intTotalCount = 0;
            return GetRMALines(null, null, null, out intTotalCount);
        }

        public static List<RMALine> GetRMALines(RMALineFilter Filter)
        {
            int intTotalCount = 0;
            return GetRMALines(Filter, null, null, out intTotalCount);
        }

        public static List<RMALine> GetRMALines(RMALineFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetRMALines(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<RMALine> GetRMALines(RMALineFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<RMALine> objReturn = null;
            RMALine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<RMALine>();

                strSQL = string.Format(@"
                                    SELECT * 
                                    FROM RMALine (NOLOCK) 
                                    WHERE 1=1 ");

                if (Filter != null)
                {
                    if (Filter.RMAID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.RMAID, "RMAID");
                    if (Filter.SalesOrderLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderLineID, "SalesOrderLineID");
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "RMALineID" : Utility.CustomSorting.GetSortExpression(typeof(RMALine), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new RMALine(objData.Tables[0].Rows[i]);
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

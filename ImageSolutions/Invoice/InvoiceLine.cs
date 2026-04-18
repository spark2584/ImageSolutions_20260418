using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Invoice
{
    public class InvoiceLine : ISBase.BaseClass
    {
        public string InvoiceLineID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(InvoiceLineID); } }
        public string InvoiceID { get; set; }
        public string PurchaseOrderLineID { get; set; }
        public string ItemID { get; set; }
        public string LineID { get; set; }
        public decimal UnitPrice { get; set; }
        public string ItemInternalID { get; set; }
        public decimal Quantity { get; set; }

        private Invoice mInvoice = null;
        public Invoice Invoice
        {
            get
            {
                if (mInvoice == null && !string.IsNullOrEmpty(InvoiceID))
                {
                    mInvoice = new Invoice(InvoiceID);
                }
                return mInvoice;
            }
        }

        public InvoiceLine()
        {
        }

        public InvoiceLine(string InvoiceLineID)
        {
            this.InvoiceLineID = InvoiceLineID;
            Load();
        }

        public InvoiceLine(DataRow objRow)
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
                         "FROM InvoiceLine (NOLOCK) " +
                         "WHERE InvoiceLineID=" + Database.HandleQuote(InvoiceLineID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("InvoiceLineID=" + InvoiceLineID + " is not found");
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

                if (objColumns.Contains("InvoiceLineID")) InvoiceLineID = Convert.ToString(objRow["InvoiceLineID"]);
                if (objColumns.Contains("InvoiceID")) InvoiceID = Convert.ToString(objRow["InvoiceID"]);
                if (objColumns.Contains("LineID")) LineID = Convert.ToString(objRow["LineID"]);
                if (objColumns.Contains("ItemInternalID")) ItemInternalID = Convert.ToString(objRow["ItemInternalID"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToInt32(objRow["Quantity"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("UnitPrice") && objRow["UnitPrice"] != DBNull.Value) UnitPrice = Convert.ToDecimal(objRow["UnitPrice"]);
                if (objColumns.Contains("PurchaseOrderLineID")) PurchaseOrderLineID = Convert.ToString(objRow["PurchaseOrderLineID"]);

                if (string.IsNullOrEmpty(InvoiceLineID)) throw new Exception("Missing InvoiceLineID in the datarow");
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
                if (string.IsNullOrEmpty(InvoiceID)) throw new Exception("InvoiceID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, FulfillmentLineID already exists");

                dicParam["InvoiceID"] = InvoiceID;
                dicParam["LineID"] = LineID;
                dicParam["ItemInternalID"] = ItemInternalID;
                dicParam["Quantity"] = Quantity;
                dicParam["ItemID"] = ItemID;
                dicParam["UnitPrice"] = UnitPrice;
                dicParam["PurchaseOrderLineID"] = PurchaseOrderLineID;

                InvoiceLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "InvoiceLine"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(InvoiceID)) throw new Exception("InvoiceID is required");
                if (IsNew) throw new Exception("Update cannot be performed, SalesOrderLineID is missing");

                dicParam["InvoiceID"] = InvoiceID;
                dicParam["LineID"] = LineID;
                dicParam["ItemInternalID"] = ItemInternalID;
                dicParam["Quantity"] = Quantity;
                dicParam["ItemID"] = ItemID;
                dicParam["UnitPrice"] = UnitPrice;
                dicParam["PurchaseOrderLineID"] = PurchaseOrderLineID;

                dicWParam["InvoiceLineID"] = InvoiceLineID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "InvoiceLine"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, InvoiceLineID is missing");

                dicDParam["InvoiceLineID"] = InvoiceLineID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "InvoiceLine"), objConn, objTran);
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

        public static List<InvoiceLine> GetInvoiceLine()
        {
            int intTotalCount = 0;
            return GetInvoiceLines(null, null, null, out intTotalCount);
        }

        public static List<InvoiceLine> GetInvoiceLines(InvoiceLineFilter Filter)
        {
            int intTotalCount = 0;
            return GetInvoiceLines(Filter, null, null, out intTotalCount);
        }

        public static List<InvoiceLine> GetInvoiceLines(InvoiceLineFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetInvoiceLines(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<InvoiceLine> GetInvoiceLines(InvoiceLineFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<InvoiceLine> objReturn = null;
            InvoiceLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<InvoiceLine>();

                strSQL = "SELECT * " +
                         "FROM InvoiceLine (NOLOCK) il " +
                         "INNER JOIN Invoice (NOLOCK) i ON il.InvoiceID=i.InvoiceID " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.InvoiceID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InvoiceID, "i.InvoiceID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "InvoiceLineID" : Utility.CustomSorting.GetSortExpression(typeof(InvoiceLine), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new InvoiceLine(objData.Tables[0].Rows[i]);
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
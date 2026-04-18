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
    public class Invoice : ISBase.BaseClass
    {
        public string InvoiceID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(InvoiceID); } }
        public string InternalID { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal? AmountRemaining { get; set; }
        public decimal? Total { get; set; }
        public string ExternalID { get; set; }
        public string PurchaseOrderID { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string PaymentLinkID { get; set; }
        public string PaymentLinkURL { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        private List<InvoiceLine> mInvoiceLines = null;
        public List<InvoiceLine> InvoiceLines
        {
            get
            {
                if (mInvoiceLines == null && !string.IsNullOrEmpty(InvoiceID))
                {
                    InvoiceLineFilter objFilter = null;
                    int intTotal = 0;
                    try
                    {
                        objFilter = new InvoiceLineFilter();
                        objFilter.InvoiceID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.InvoiceID.SearchString = InvoiceID;
                        mInvoiceLines = InvoiceLine.GetInvoiceLines(objFilter, "InvoiceLineID", true, null, null, out intTotal);
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
                return mInvoiceLines;
            }
            set
            {
                mInvoiceLines = value;
            }
        }

        public Invoice()
        {
        }
        public Invoice(string InvoiceID)
        {
            this.InvoiceID = InvoiceID;
            Load();
        }
        public Invoice(DataRow objRow)
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
                         "FROM Invoice (NOLOCK) " +
                         "WHERE InvoiceID=" + Database.HandleQuote(InvoiceID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("InvoiceID=" + InvoiceID + " is not found");
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

                if (objColumns.Contains("InvoiceID")) InvoiceID = Convert.ToString(objRow["InvoiceID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("TransactionDate") && objRow["TransactionDate"] != DBNull.Value) TransactionDate = Convert.ToDateTime(objRow["TransactionDate"]);
                if (objColumns.Contains("InvoiceNumber")) InvoiceNumber = Convert.ToString(objRow["InvoiceNumber"]);
                if (objColumns.Contains("AmountRemaining") && objRow["AmountRemaining"] != DBNull.Value) AmountRemaining = Convert.ToDecimal(objRow["AmountRemaining"]);
                if (objColumns.Contains("Total") && objRow["Total"] != DBNull.Value) Total = Convert.ToDecimal(objRow["Total"]);
                if (objColumns.Contains("PaymentLinkID")) PaymentLinkID = Convert.ToString(objRow["PaymentLinkID"]);
                if (objColumns.Contains("PaymentLinkURL")) PaymentLinkURL = Convert.ToString(objRow["PaymentLinkURL"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
                if (objColumns.Contains("PurchaseOrderID")) PurchaseOrderID = Convert.ToString(objRow["PurchaseOrderID"]);
                if (objColumns.Contains("OrderNumber")) OrderNumber = Convert.ToString(objRow["OrderNumber"]);
                if (objColumns.Contains("InvoiceDate") && objRow["InvoiceDate"] != DBNull.Value) InvoiceDate = Convert.ToDateTime(objRow["InvoiceDate"]);

                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);

                if (string.IsNullOrEmpty(InvoiceID)) throw new Exception("Missing InvoiceID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, InvoiceID already exists");

                dicParam["InternalID"] = InternalID;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["InvoiceNumber"] = InvoiceNumber;
                dicParam["AmountRemaining"] = AmountRemaining;
                dicParam["Total"] = Total;
                dicParam["PaymentLinkID"] = PaymentLinkID;
                dicParam["PaymentLinkURL"] = PaymentLinkURL;
                dicParam["ExternalID"] = ExternalID;
                dicParam["PurchaseOrderID"] = PurchaseOrderID;
                dicParam["OrderNumber"] = OrderNumber;
                dicParam["InvoiceDate"] = InvoiceDate;
                dicParam["ErrorMessage"] = ErrorMessage;

                InvoiceID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Invoice"), objConn, objTran).ToString();

                foreach (InvoiceLine objInvoiceLIne in InvoiceLines)
                {
                    objInvoiceLIne.InvoiceID = InvoiceID;
                    objInvoiceLIne.Create(objConn, objTran);
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
                if (IsNew) throw new Exception("Update cannot be performed, InvoiceID is missing");


                dicParam["InternalID"] = InternalID;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["InvoiceNumber"] = InvoiceNumber;
                dicParam["AmountRemaining"] = AmountRemaining;
                dicParam["Total"] = Total;
                dicParam["PaymentLinkID"] = PaymentLinkID;
                dicParam["PaymentLinkURL"] = PaymentLinkURL;
                dicParam["ExternalID"] = ExternalID;
                dicParam["PurchaseOrderID"] = PurchaseOrderID;
                dicParam["OrderNumber"] = OrderNumber;
                dicParam["InvoiceDate"] = InvoiceDate;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = DateTime.UtcNow;

                dicWParam["InvoiceID"] = InvoiceID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Invoice"), objConn, objTran);

                foreach (InvoiceLine objInvoiceLine in InvoiceLines)
                {
                    objInvoiceLine.Update(objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, InvoiceID is missing");

                dicDParam["InvoiceID"] = InvoiceID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Invoice"), objConn, objTran);
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

        public static Invoice GetInvoice(InvoiceFilter Filter)
        {
            List<Invoice> objInvoices = null;
            Invoice objReturn = null;

            try
            {
                objInvoices = GetInvoices(Filter);
                if (objInvoices != null && objInvoices.Count >= 1) objReturn = objInvoices[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objInvoices = null;
            }
            return objReturn;
        }

        public static List<Invoice> GetInvoices()
        {
            int intTotalCount = 0;
            return GetInvoices(null, null, null, out intTotalCount);
        }

        public static List<Invoice> GetInvoices(InvoiceFilter Filter)
        {
            int intTotalCount = 0;
            return GetInvoices(Filter, null, null, out intTotalCount);
        }

        public static List<Invoice> GetInvoices(InvoiceFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetInvoices(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Invoice> GetInvoices(InvoiceFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Invoice> objReturn = null;
            Invoice objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Invoice>();

                strSQL = "SELECT i.* " +
                         "FROM Invoice i (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "i.InternalID");
                    if (Filter.InvoiceNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InvoiceNumber, "i.InvoiceNumber");
                    if (Filter.PaymentLinkID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PaymentLinkID, "i.PaymentLinkID");
                    if (Filter.PaymentLinkURL != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PaymentLinkURL, "i.PaymentLinkURL");
                    if (Filter.OrderNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.OrderNumber, "i.OrderNumber");
                    if (Filter.ExternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ExternalID, "i.ExternalID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "InvoiceID" : Utility.CustomSorting.GetSortExpression(typeof(Invoice), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Invoice(objData.Tables[0].Rows[i]);
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

    public enum enumStatus
    {
        Open = 1,
        Paid = 2,
        Cancel = 3
    }
}

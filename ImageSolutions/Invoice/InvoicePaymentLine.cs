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
    public class InvoicePaymentLine : ISBase.BaseClass
    {
        public string InvoicePaymentLineID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(InvoicePaymentLineID); } }

        public string InvoicePaymentID { get; set; }
        public string InvoiceInternalID { get; set; }
        public string CustomerInternalID { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal? AmountTotal { get; set; }
        public decimal? AmountRemaining { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedOn { get; set; }

        private InvoicePayment mInvoicePayment = null;
        public InvoicePayment InvoicePayment
        {
            get
            {
                if (mInvoicePayment == null && !string.IsNullOrEmpty(InvoicePaymentID))
                {
                    mInvoicePayment = new InvoicePayment(InvoicePaymentID);
                }
                return mInvoicePayment;
            }
        }

        public InvoicePaymentLine()
        {
        }
        public InvoicePaymentLine(string InvoicePaymentLineID)
        {
            this.InvoicePaymentLineID = InvoicePaymentLineID;
            Load();
        }
        public InvoicePaymentLine(DataRow objRow)
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
                         "FROM InvoicePaymentLine (NOLOCK) " +
                         "WHERE InvoicePaymentLineID=" + Database.HandleQuote(InvoicePaymentLineID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("InvoicePaymentLineID=" + InvoicePaymentLineID + " is not found");
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

                if (objColumns.Contains("InvoicePaymentLineID")) InvoicePaymentLineID = Convert.ToString(objRow["InvoicePaymentLineID"]);
                if (objColumns.Contains("InvoicePaymentID")) InvoicePaymentID = Convert.ToString(objRow["InvoicePaymentID"]);
                if (objColumns.Contains("InvoiceInternalID")) InvoiceInternalID = Convert.ToString(objRow["InvoiceInternalID"]);
                if (objColumns.Contains("CustomerInternalID")) CustomerInternalID = Convert.ToString(objRow["CustomerInternalID"]);
                if (objColumns.Contains("TransactionDate") && objRow["TransactionDate"] != DBNull.Value) TransactionDate = Convert.ToDateTime(objRow["TransactionDate"]);
                if (objColumns.Contains("InvoiceNumber")) InvoiceNumber = Convert.ToString(objRow["InvoiceNumber"]);
                if (objColumns.Contains("AmountTotal") && objRow["AmountTotal"] != DBNull.Value) AmountTotal = Convert.ToDecimal(objRow["AmountTotal"]);
                if (objColumns.Contains("AmountRemaining") && objRow["AmountRemaining"] != DBNull.Value) AmountRemaining = Convert.ToDecimal(objRow["AmountRemaining"]);
                if (objColumns.Contains("Status")) Status = Convert.ToString(objRow["Status"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(InvoicePaymentLineID)) throw new Exception("Missing InvoicePaymentLineID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, InvoicePaymentLineID already exists");

                dicParam["InvoicePaymentID"] = InvoicePaymentID;
                dicParam["InvoiceInternalID"] = InvoiceInternalID;
                dicParam["CustomerInternalID"] = CustomerInternalID;                
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["InvoiceNumber"] = InvoiceNumber;
                dicParam["AmountTotal"] = AmountTotal;
                dicParam["AmountRemaining"] = AmountRemaining;
                dicParam["Status"] = Status;
                dicParam["ErrorMessage"] = ErrorMessage;

                InvoicePaymentLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "InvoicePaymentLine"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, InvoicePaymentLineID is missing");

                dicParam["InvoicePaymentID"] = InvoicePaymentID;
                dicParam["InvoiceInternalID"] = InvoiceInternalID;
                dicParam["CustomerInternalID"] = CustomerInternalID;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["InvoiceNumber"] = InvoiceNumber;
                dicParam["AmountTotal"] = AmountTotal;
                dicParam["AmountRemaining"] = AmountRemaining;
                dicParam["Status"] = Status;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicWParam["InvoicePaymentLineID"] = InvoicePaymentLineID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "InvoicePaymentLine"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, InvoicePaymentLineID is missing");

                dicDParam["InvoicePaymentLineID"] = InvoicePaymentLineID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "InvoicePaymentLine"), objConn, objTran);
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

        public static InvoicePaymentLine GetInvoicePaymentLine(InvoicePaymentLineFilter Filter)
        {
            List<InvoicePaymentLine> objInvoicePaymentLines = null;
            InvoicePaymentLine objReturn = null;

            try
            {
                objInvoicePaymentLines = GetInvoicePaymentLines(Filter);
                if (objInvoicePaymentLines != null && objInvoicePaymentLines.Count >= 1) objReturn = objInvoicePaymentLines[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objInvoicePaymentLines = null;
            }
            return objReturn;
        }

        public static List<InvoicePaymentLine> GetInvoicePaymentLines()
        {
            int intTotalCount = 0;
            return GetInvoicePaymentLines(null, null, null, out intTotalCount);
        }

        public static List<InvoicePaymentLine> GetInvoicePaymentLines(InvoicePaymentLineFilter Filter)
        {
            int intTotalCount = 0;
            return GetInvoicePaymentLines(Filter, null, null, out intTotalCount);
        }

        public static List<InvoicePaymentLine> GetInvoicePaymentLines(InvoicePaymentLineFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetInvoicePaymentLines(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<InvoicePaymentLine> GetInvoicePaymentLines(InvoicePaymentLineFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<InvoicePaymentLine> objReturn = null;
            InvoicePaymentLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<InvoicePaymentLine>();

                strSQL = "SELECT i.* " +
                         "FROM InvoicePaymentLine i (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.InvoicePaymentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InvoicePaymentID, "i.InvoicePaymentID");
                    if (Filter.InvoiceInternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InvoiceInternalID, "i.InvoiceInternalID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "InvoicePaymentLineID" : Utility.CustomSorting.GetSortExpression(typeof(InvoicePaymentLine), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new InvoicePaymentLine(objData.Tables[0].Rows[i]);
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

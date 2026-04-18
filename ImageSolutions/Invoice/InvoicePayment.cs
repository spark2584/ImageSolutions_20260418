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
    public class InvoicePayment : ISBase.BaseClass
    {
        public string InvoicePaymentID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(InvoicePaymentID); } }

        public string CustomerInternalID { get; set; }
        public string CustomerPaymentInternalID { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentLinkID { get; set; }
        public string PaymentLinkURL { get; set; }
        public string PaymentIntentID { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedOn { get; set; }

        private List<InvoicePaymentLine> mInvoicePaymentLines = null;
        public List<InvoicePaymentLine> InvoicePaymentLines
        {
            get
            {
                if (mInvoicePaymentLines == null && !string.IsNullOrEmpty(InvoicePaymentID))
                {
                    InvoicePaymentLineFilter objFilter = null;
                    int intTotal = 0;
                    try
                    {
                        objFilter = new InvoicePaymentLineFilter();
                        objFilter.InvoicePaymentID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.InvoicePaymentID.SearchString = InvoicePaymentID;
                        mInvoicePaymentLines = InvoicePaymentLine.GetInvoicePaymentLines(objFilter, "InvoicePaymentLineID", true, null, null, out intTotal);
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
                return mInvoicePaymentLines;
            }
            set
            {
                mInvoicePaymentLines = value;
            }
        }

        public InvoicePayment()
        {
        }
        public InvoicePayment(string InvoicePaymentID)
        {
            this.InvoicePaymentID = InvoicePaymentID;
            Load();
        }
        public InvoicePayment(DataRow objRow)
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
                         "FROM InvoicePayment (NOLOCK) " +
                         "WHERE InvoicePaymentID=" + Database.HandleQuote(InvoicePaymentID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("InvoicePaymentID=" + InvoicePaymentID + " is not found");
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

                if (objColumns.Contains("InvoicePaymentID")) InvoicePaymentID = Convert.ToString(objRow["InvoicePaymentID"]);
                if (objColumns.Contains("CustomerInternalID")) CustomerInternalID = Convert.ToString(objRow["CustomerInternalID"]);
                if (objColumns.Contains("CustomerPaymentInternalID")) CustomerPaymentInternalID = Convert.ToString(objRow["CustomerPaymentInternalID"]);
                if (objColumns.Contains("Amount") && objRow["Amount"] != DBNull.Value) Amount = Convert.ToDecimal(objRow["Amount"]);
                if (objColumns.Contains("PaymentLinkID")) PaymentLinkID = Convert.ToString(objRow["PaymentLinkID"]);
                if (objColumns.Contains("PaymentLinkURL")) PaymentLinkURL = Convert.ToString(objRow["PaymentLinkURL"]);
                if (objColumns.Contains("PaymentIntentID")) PaymentIntentID = Convert.ToString(objRow["PaymentIntentID"]);
                if (objColumns.Contains("Status")) Status = Convert.ToString(objRow["Status"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(InvoicePaymentID)) throw new Exception("Missing InvoicePaymentID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, InvoicePaymentID already exists");

                dicParam["CustomerInternalID"] = CustomerInternalID;
                dicParam["CustomerPaymentInternalID"] = CustomerPaymentInternalID;
                dicParam["Amount"] = Amount;
                dicParam["PaymentLinkID"] = PaymentLinkID;
                dicParam["PaymentLinkURL"] = PaymentLinkURL;
                dicParam["PaymentIntentID"] = PaymentIntentID;
                dicParam["Status"] = Status;
                dicParam["ErrorMessage"] = ErrorMessage;

                InvoicePaymentID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "InvoicePayment"), objConn, objTran).ToString();

                foreach (InvoicePaymentLine objInvoicePaymentLIne in InvoicePaymentLines)
                {
                    objInvoicePaymentLIne.InvoicePaymentID = InvoicePaymentID;
                    objInvoicePaymentLIne.Create(objConn, objTran);
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
                if (IsNew) throw new Exception("Update cannot be performed, InvoicePaymentID is missing");


                dicParam["CustomerInternalID"] = CustomerInternalID;
                dicParam["CustomerPaymentInternalID"] = CustomerPaymentInternalID;
                dicParam["Amount"] = Amount;
                dicParam["PaymentLinkID"] = PaymentLinkID;
                dicParam["PaymentLinkURL"] = PaymentLinkURL;
                dicParam["PaymentIntentID"] = PaymentIntentID;
                dicParam["Status"] = Status;
                dicParam["ErrorMessage"] = ErrorMessage;

                dicWParam["InvoicePaymentID"] = InvoicePaymentID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "InvoicePayment"), objConn, objTran);

                foreach (InvoicePaymentLine objInvoicePaymentLine in InvoicePaymentLines)
                {
                    objInvoicePaymentLine.Update(objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, InvoicePaymentID is missing");

                dicDParam["InvoicePaymentID"] = InvoicePaymentID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "InvoicePayment"), objConn, objTran);
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

        public static InvoicePayment GetInvoicePayment(InvoicePaymentFilter Filter)
        {
            List<InvoicePayment> objInvoicePayments = null;
            InvoicePayment objReturn = null;

            try
            {
                objInvoicePayments = GetInvoicePayments(Filter);
                if (objInvoicePayments != null && objInvoicePayments.Count >= 1) objReturn = objInvoicePayments[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objInvoicePayments = null;
            }
            return objReturn;
        }

        public static List<InvoicePayment> GetInvoicePayments()
        {
            int intTotalCount = 0;
            return GetInvoicePayments(null, null, null, out intTotalCount);
        }

        public static List<InvoicePayment> GetInvoicePayments(InvoicePaymentFilter Filter)
        {
            int intTotalCount = 0;
            return GetInvoicePayments(Filter, null, null, out intTotalCount);
        }

        public static List<InvoicePayment> GetInvoicePayments(InvoicePaymentFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetInvoicePayments(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<InvoicePayment> GetInvoicePayments(InvoicePaymentFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<InvoicePayment> objReturn = null;
            InvoicePayment objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<InvoicePayment>();

                strSQL = "SELECT i.* " +
                         "FROM InvoicePayment i (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.CustomerInternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomerInternalID, "i.CustomerInternalID");
                    if (Filter.PaymentLinkID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PaymentLinkID, "i.PaymentLinkID");
                    if (Filter.PaymentLinkURL != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PaymentLinkURL, "i.PaymentLinkURL");
                    if (Filter.Status != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Status, "i.Status");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "InvoicePaymentID" : Utility.CustomSorting.GetSortExpression(typeof(InvoicePayment), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new InvoicePayment(objData.Tables[0].Rows[i]);
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

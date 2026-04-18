using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageSolutions.Item;

namespace ImageSolutions.Payment
{
    public class Payment : ISBase.BaseClass
    {
        public enum enumPaymentSource
        {
            [Description("Credit Card")]
            CreditCard = 0,
            [Description("Budget")]
            Budget = 1,
            [Description("Invoice")]
            Invoice = 2,
            [Description("Promotion")]
            Promotion = 3
        }

        public bool IsNew { get { return string.IsNullOrEmpty(PaymentID); } }
        public string PaymentID { get; protected set; }
        public string UserInfoID { get; set; }
        public string SalesOrderID { get; set; }
        public string NetSuiteInternalID { get; set; }
        public enumPaymentSource? PaymentSource { get; set; }
        public double AmountPaid { get; set; }
        public string CreditCardTransactionLogID { get; set; }
        public string CreditCardID { get; set; }
        public string BudgetAssignmentID { get; set; }
        public string PromotionID { get; set; }
        //spublic string PaymentTerm { get; set; }
        public string PaymentTermID { get; set; }
        public string ErrorMessage { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        private CreditCard.CreditCardTransactionLog mCreditCardTransactionLog = null;
        public CreditCard.CreditCardTransactionLog CreditCardTransactionLog
        {
            get
            {
                if (mCreditCardTransactionLog == null && !string.IsNullOrEmpty(CreditCardTransactionLogID))
                {
                    mCreditCardTransactionLog = new CreditCard.CreditCardTransactionLog(CreditCardTransactionLogID);
                }
                return mCreditCardTransactionLog;
            }
        }

        private Budget.BudgetAssignment mBudgetAssignment = null;
        public Budget.BudgetAssignment BudgetAssignment
        {
            get
            {
                if (mBudgetAssignment == null && !string.IsNullOrEmpty(BudgetAssignmentID))
                {
                    mBudgetAssignment = new Budget.BudgetAssignment(BudgetAssignmentID);
                }
                return mBudgetAssignment;
            }
        }
        private PaymentTerm mPaymentTerm = null;
        public PaymentTerm PaymentTerm
        {
            get
            {
                if (mPaymentTerm == null && !string.IsNullOrEmpty(PaymentTermID))
                {
                    mPaymentTerm = new ImageSolutions.Payment.PaymentTerm(PaymentTermID);
                }

                return mPaymentTerm;
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
        }
        private Promotion.Promotion mPromotion = null;
        public Promotion.Promotion Promotion
        {
            get
            {
                if (mPromotion == null && !string.IsNullOrEmpty(PromotionID))
                {
                    mPromotion = new Promotion.Promotion(SalesOrder.WebsiteID, PromotionID);
                }
                return mPromotion;
            }
        }
        public Payment()
        {

        }

        public Payment(string PaymentID)
        {
            this.PaymentID = PaymentID;
            Load();
        }

        public Payment(DataRow objRow)
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
                strSQL = "SELECT p.* " +
                         "FROM Payment p (NOLOCK) " +
                         "WHERE p.PaymentID=" + Database.HandleQuote(PaymentID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("PaymentID=" + PaymentID + " is not found");
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

                if (objColumns.Contains("PaymentID")) PaymentID = Convert.ToString(objRow["PaymentID"]);
                if (objColumns.Contains("UserInfoID")) UserInfoID = Convert.ToString(objRow["UserInfoID"]);
                if (objColumns.Contains("SalesOrderID")) SalesOrderID = Convert.ToString(objRow["SalesOrderID"]);
                if (objColumns.Contains("NetSuiteInternalID")) NetSuiteInternalID = Convert.ToString(objRow["NetSuiteInternalID"]);
                if (objColumns.Contains("PaymentSource")) PaymentSource = (enumPaymentSource)Enum.ToObject(typeof(enumPaymentSource), Convert.ToInt32(objRow["PaymentSource"]));
                if (objColumns.Contains("AmountPaid")) AmountPaid = Convert.ToDouble(objRow["AmountPaid"]);
                if (objColumns.Contains("CreditCardTransactionLogID")) CreditCardTransactionLogID = Convert.ToString(objRow["CreditCardTransactionLogID"]);
                if (objColumns.Contains("CreditCardID")) CreditCardID = Convert.ToString(objRow["CreditCardID"]);
                if (objColumns.Contains("BudgetAssignmentID")) BudgetAssignmentID = Convert.ToString(objRow["BudgetAssignmentID"]);
                if (objColumns.Contains("PromotionID")) PromotionID = Convert.ToString(objRow["PromotionID"]);
                //if (objColumns.Contains("PaymentTerm")) PaymentTerm = Convert.ToString(objRow["PaymentTerm"]);
                if (objColumns.Contains("PaymentTermID")) PaymentTermID = Convert.ToString(objRow["PaymentTermID"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(PaymentID)) throw new Exception("Missing PaymentID in the datarow");
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

            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (string.IsNullOrEmpty(UserInfoID)) throw new Exception("UserInfoID is required");
                if (string.IsNullOrEmpty(SalesOrderID)) throw new Exception("SalesOrderID is required");
                //if (AmountPaid == 0) throw new Exception("Amount charged cannot be 0");
                if (PaymentSource == null) throw new Exception("Payment source is required");
                if (!IsNew) throw new Exception("Create cannot be performed, PaymentID already exists");

                dicParam["UserInfoID"] = UserInfoID;
                dicParam["SalesOrderID"] = SalesOrderID;
                dicParam["NetSuiteInternalID"] = NetSuiteInternalID;
                dicParam["AmountPaid"] = AmountPaid;
                dicParam["PaymentSource"] = (int)PaymentSource.Value;
                dicParam["CreditCardTransactionLogID"] = CreditCardTransactionLogID;
                dicParam["CreditCardID"] = CreditCardID;
                dicParam["BudgetAssignmentID"] = BudgetAssignmentID;
                dicParam["PromotionID"] = PromotionID;
                //dicParam["PaymentTerm"] = PaymentTerm;
                dicParam["PaymentTermID"] = PaymentTermID;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["CreatedBy"] = CreatedBy;
                PaymentID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Payment"), objConn, objTran).ToString();

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

            Hashtable dicWParam = new Hashtable();

            try
            {
                dicWParam["PaymentID"] = PaymentID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicWParam, "Payment"), objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicWParam = null;
            }
            return true;
        }

        public override bool Update(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Update();

            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (string.IsNullOrEmpty(UserInfoID)) throw new Exception("UserInfoID is required");
                //if (AmountPaid == 0) throw new Exception("Amount charged cannot be 0");
                if (PaymentSource == null) throw new Exception("Payment source is required");
                if (IsNew) throw new Exception("Update cannot be performed, PaymentID is missing");

                dicParam["NetSuiteInternalID"] = NetSuiteInternalID;
                dicParam["AmountPaid"] = AmountPaid;
                dicParam["PaymentSource"] = (int)PaymentSource.Value;
                dicParam["CreditCardTransactionLogID"] = CreditCardTransactionLogID;
                dicParam["CreditCardID"] = CreditCardID;
                dicParam["BudgetAssignmentID"] = BudgetAssignmentID;
                dicParam["PromotionID"] = PromotionID;
                //dicParam["PaymentTerm"] = PaymentTerm;
                dicParam["PaymentTermID"] = PaymentTermID;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["PaymentID"] = PaymentID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Payment"), objConn, objTran);

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

        public static Payment GetPayment(PaymentFilter Filter)
        {
            List<Payment> objItems = null;
            Payment objReturn = null;

            try
            {
                objItems = GetPayments(Filter);
                if (objItems != null && objItems.Count >= 1) objReturn = objItems[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItems = null;
            }
            return objReturn;
        }

        public static List<Payment> GetPayments()
        {
            int intTotalCount = 0;
            return GetPayments(null, null, null, out intTotalCount);
        }

        public static List<Payment> GetPayments(PaymentFilter Filter)
        {
            int intTotalCount = 0;
            return GetPayments(Filter, null, null, out intTotalCount);
        }

        public static List<Payment> GetPayments(PaymentFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetPayments(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Payment> GetPayments(PaymentFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Payment> objReturn = null;
            Payment objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Payment>();

                strSQL = "SELECT s.UserWebsiteID, p.* " +
                         "FROM Payment (NOLOCK) p " +
                         "Inner Join SalesOrder (NOLOCK) s on s.SalesOrderID = p.SalesOrderID " +
                         "WHERE 1=1  ";

                if (Filter != null)
                {
                    if (Filter.SalesOrderID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderID, "p.SalesOrderID");
                    if (Filter.BudgetAssignmentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.BudgetAssignmentID, "p.BudgetAssignmentID");
                    if (Filter.PromotionID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PromotionID, "p.PromotionID");
                    if (Filter.UserInfoID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserInfoID, "p.UserInfoID");
                    if (Filter.NetSuiteInternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.NetSuiteInternalID, "p.NetSuiteInternalID");
                    if (Filter.PaymentSource != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PaymentSource, "p.PaymentSource");
                    if (Filter.SalesOrderNetSuiteInternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderNetSuiteInternalID, "s.NetSuiteInternalID");
                    if (Filter.CreditCardTransactionLogID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CreditCardTransactionLogID, "p.CreditCardTransactionLogID");
                    if (Filter.PaymentTermID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PaymentTermID, "p.PaymentTermID");
                    if (Filter.UserWebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserWebsiteID, "s.UserWebsiteID");
                    if (Filter.CreatedOn != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.CreatedOn, "p.CreatedOn");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "PaymentID" : Utility.CustomSorting.GetSortExpression(typeof(Payment), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else strSQL += "ORDER BY p.PaymentID DESC";

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Payment(objData.Tables[0].Rows[i]);
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

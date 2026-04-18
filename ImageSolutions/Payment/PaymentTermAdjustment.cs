using ImageSolutions.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Payment
{
    public class PaymentTermAdjustment : ISBase.BaseClass
    {
        public string PaymentTermAdjustmentID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(PaymentTermAdjustmentID); } }
        public string UserWebsiteID { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        private UserWebsite mUserWebsite = null;
        public UserWebsite UserWebsite
        {
            get
            {
                if (mUserWebsite == null && !string.IsNullOrEmpty(UserWebsiteID))
                {
                    mUserWebsite = new UserWebsite(UserWebsiteID);
                }
                return mUserWebsite;
            }
        }

        private UserInfo mCreatedByUser = null;
        public UserInfo CreatedByUser
        {
            get
            {
                if (mCreatedByUser == null && !string.IsNullOrEmpty(CreatedBy))
                {
                    mCreatedByUser = new UserInfo(CreatedBy);
                }
                return mCreatedByUser;
            }
        }
        public PaymentTermAdjustment()
        {
        }
        public PaymentTermAdjustment(string PaymentTermAdjustmentID)
        {
            this.PaymentTermAdjustmentID = PaymentTermAdjustmentID;
            Load();
        }
        public PaymentTermAdjustment(DataRow objRow)
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
                         "FROM PaymentTermAdjustment (NOLOCK) " +
                         "WHERE PaymentTermAdjustmentID=" + Database.HandleQuote(PaymentTermAdjustmentID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("PaymentTermAdjustmentID=" + PaymentTermAdjustmentID + " is not found");
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
                if (objColumns.Contains("PaymentTermAdjustmentID")) PaymentTermAdjustmentID = Convert.ToString(objRow["PaymentTermAdjustmentID"]);
                if (objColumns.Contains("UserWebsiteID")) UserWebsiteID = Convert.ToString(objRow["UserWebsiteID"]);
                if (objColumns.Contains("TransactionDate")) TransactionDate = Convert.ToDateTime(objRow["TransactionDate"]);
                if (objColumns.Contains("Amount")) Amount = Convert.ToDecimal(objRow["Amount"]);
                if (objColumns.Contains("Reason")) Reason = Convert.ToString(objRow["Reason"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(PaymentTermAdjustmentID)) throw new Exception("Missing PaymentTermAdjustmentID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, PaymentTermAdjustmentID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["Amount"] = Amount;
                dicParam["Reason"] = Reason;
                dicParam["CreatedBy"] = CreatedBy;
                PaymentTermAdjustmentID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "PaymentTermAdjustment"), objConn, objTran).ToString();

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
                if (PaymentTermAdjustmentID == null) throw new Exception("PaymentTermAdjustmentID is required");
                if (IsNew) throw new Exception("Update cannot be performed, PaymentTermAdjustmentID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["Amount"] = Amount;
                dicParam["Reason"] = Reason;
                dicWParam["PaymentTermAdjustmentID"] = PaymentTermAdjustmentID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "PaymentTermAdjustment"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, PaymentTermAdjustmentID is missing");

                dicDParam["PaymentTermAdjustmentID"] = PaymentTermAdjustmentID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "PaymentTermAdjustment"), objConn, objTran);
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
            //string strSQL = string.Empty;

            //strSQL = "SELECT TOP 1 p.* " +
            //         "FROM PaymentTermAdjustment (NOLOCK) p " +
            //         "WHERE " +
            //         "(" +
            //         "  (p.WebsiteID=" + Database.HandleQuote(WebsiteID) + " AND p.UserWebsiteID=" + Database.HandleQuote(UserWebsiteID) + " AND p.BudgetID=" + Database.HandleQuote(BudgetID) + ")" +
            //         "  OR " +
            //         "  (p.WebsiteID=" + Database.HandleQuote(WebsiteID) + " AND p.WebsiteGroupID=" + Database.HandleQuote(WebsiteGroupID) + " AND p.BudgetID=" + Database.HandleQuote(BudgetID) + " )" +
            //         ") ";

            //if (!string.IsNullOrEmpty(PaymentTermAdjustmentID)) strSQL += "AND p.PaymentTermAdjustmentID<>" + Database.HandleQuote(PaymentTermAdjustmentID);
            //return Database.HasRows(strSQL);
        }

        public static PaymentTermAdjustment GetPaymentTermAdjustment(PaymentTermAdjustmentFilter Filter)
        {
            List<PaymentTermAdjustment> objPaymentTermAdjustments = null;
            PaymentTermAdjustment objReturn = null;

            try
            {
                objPaymentTermAdjustments = GetPaymentTermAdjustments(Filter);
                if (objPaymentTermAdjustments != null && objPaymentTermAdjustments.Count >= 1) objReturn = objPaymentTermAdjustments[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objPaymentTermAdjustments = null;
            }
            return objReturn;
        }

        public static List<PaymentTermAdjustment> GetPaymentTermAdjustments()
        {
            int intTotalCount = 0;
            return GetPaymentTermAdjustments(null, null, null, out intTotalCount);
        }

        public static List<PaymentTermAdjustment> GetPaymentTermAdjustments(PaymentTermAdjustmentFilter Filter)
        {
            int intTotalCount = 0;
            return GetPaymentTermAdjustments(Filter, null, null, out intTotalCount);
        }

        public static List<PaymentTermAdjustment> GetPaymentTermAdjustments(PaymentTermAdjustmentFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetPaymentTermAdjustments(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<PaymentTermAdjustment> GetPaymentTermAdjustments(PaymentTermAdjustmentFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<PaymentTermAdjustment> objReturn = null;
            PaymentTermAdjustment objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<PaymentTermAdjustment>();

                strSQL = "SELECT * " +
                         "FROM PaymentTermAdjustment (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.UserWebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserWebsiteID, "UserWebsiteID");
                    if (Filter.TransactionDate != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.TransactionDate, "TransactionDate");

                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "PaymentTermAdjustmentID" : Utility.CustomSorting.GetSortExpression(typeof(PaymentTermAdjustment), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new PaymentTermAdjustment(objData.Tables[0].Rows[i]);
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

using ImageSolutions.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Budget
{
    public class BudgetAssignmentAdjustment : ISBase.BaseClass
    {
        public string BudgetAssignmentAdjustmentID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(BudgetAssignmentAdjustmentID); } }
        public string BudgetAssignmentID { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public bool SubmitPayroll { get; set; }
        public bool IsPayrollSubmitted { get; set; }
        public string PayrollReference { get; set; }
        public DateTime? PayrollSubmittedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }       

        private BudgetAssignment mBudgetAssignment = null;
        public BudgetAssignment BudgetAssignment
        {
            get
            {
                if (mBudgetAssignment == null && !string.IsNullOrEmpty(BudgetAssignmentID))
                {
                    mBudgetAssignment = new BudgetAssignment(BudgetAssignmentID);
                }
                return mBudgetAssignment;
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
        public BudgetAssignmentAdjustment()
        {
        }
        public BudgetAssignmentAdjustment(string BudgetAssignmentAdjustmentID)
        {
            this.BudgetAssignmentAdjustmentID = BudgetAssignmentAdjustmentID;
            Load();
        }
        public BudgetAssignmentAdjustment(DataRow objRow)
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
                         "FROM BudgetAssignmentAdjustment (NOLOCK) " +
                         "WHERE BudgetAssignmentAdjustmentID=" + Database.HandleQuote(BudgetAssignmentAdjustmentID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("BudgetAssignmentAdjustmentID=" + BudgetAssignmentAdjustmentID + " is not found");
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
                if (objColumns.Contains("BudgetAssignmentAdjustmentID")) BudgetAssignmentAdjustmentID = Convert.ToString(objRow["BudgetAssignmentAdjustmentID"]);
                if (objColumns.Contains("BudgetAssignmentID")) BudgetAssignmentID = Convert.ToString(objRow["BudgetAssignmentID"]);
                if (objColumns.Contains("Amount")) Amount = Convert.ToDecimal(objRow["Amount"]);
                if (objColumns.Contains("Reason")) Reason = Convert.ToString(objRow["Reason"]);
                if (objColumns.Contains("SubmitPayroll")) SubmitPayroll = Convert.ToBoolean(objRow["SubmitPayroll"]);
                if (objColumns.Contains("PayrollReference")) PayrollReference = Convert.ToString(objRow["PayrollReference"]);
                if (objColumns.Contains("IsPayrollSubmitted")) IsPayrollSubmitted = Convert.ToBoolean(objRow["IsPayrollSubmitted"]);
                if (objColumns.Contains("PayrollSubmittedOn") && objRow["PayrollSubmittedOn"] != DBNull.Value) PayrollSubmittedOn = Convert.ToDateTime(objRow["PayrollSubmittedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(BudgetAssignmentAdjustmentID)) throw new Exception("Missing BudgetAssignmentAdjustmentID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, BudgetAssignmentAdjustmentID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["BudgetAssignmentID"] = BudgetAssignmentID;
                dicParam["Amount"] = Amount;
                dicParam["Reason"] = Reason;
                dicParam["SubmitPayroll"] = SubmitPayroll;
                dicParam["IsPayrollSubmitted"] = IsPayrollSubmitted;
                dicParam["PayrollReference"] = PayrollReference;
                dicParam["PayrollSubmittedOn"] = PayrollSubmittedOn;
                dicParam["CreatedBy"] = CreatedBy;
                BudgetAssignmentAdjustmentID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "BudgetAssignmentAdjustment"), objConn, objTran).ToString();

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
                if (BudgetAssignmentAdjustmentID == null) throw new Exception("BudgetAssignmentAdjustmentID is required");
                if (IsNew) throw new Exception("Update cannot be performed, BudgetAssignmentAdjustmentID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["BudgetAssignmentID"] = BudgetAssignmentID;
                dicParam["Amount"] = Amount;
                dicParam["Reason"] = Reason;
                dicParam["SubmitPayroll"] = SubmitPayroll;
                dicParam["IsPayrollSubmitted"] = IsPayrollSubmitted;
                dicParam["PayrollReference"] = PayrollReference;
                dicParam["PayrollSubmittedOn"] = PayrollSubmittedOn;
                dicWParam["BudgetAssignmentAdjustmentID"] = BudgetAssignmentAdjustmentID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "BudgetAssignmentAdjustment"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, BudgetAssignmentAdjustmentID is missing");

                dicDParam["BudgetAssignmentAdjustmentID"] = BudgetAssignmentAdjustmentID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "BudgetAssignmentAdjustment"), objConn, objTran);
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
            //         "FROM BudgetAssignmentAdjustment (NOLOCK) p " +
            //         "WHERE " +
            //         "(" +
            //         "  (p.WebsiteID=" + Database.HandleQuote(WebsiteID) + " AND p.UserWebsiteID=" + Database.HandleQuote(UserWebsiteID) + " AND p.BudgetID=" + Database.HandleQuote(BudgetID) + ")" +
            //         "  OR " +
            //         "  (p.WebsiteID=" + Database.HandleQuote(WebsiteID) + " AND p.WebsiteGroupID=" + Database.HandleQuote(WebsiteGroupID) + " AND p.BudgetID=" + Database.HandleQuote(BudgetID) + " )" +
            //         ") ";

            //if (!string.IsNullOrEmpty(BudgetAssignmentAdjustmentID)) strSQL += "AND p.BudgetAssignmentAdjustmentID<>" + Database.HandleQuote(BudgetAssignmentAdjustmentID);
            //return Database.HasRows(strSQL);
        }

        public static BudgetAssignmentAdjustment GetBudgetAssignmentAdjustment(BudgetAssignmentAdjustmentFilter Filter)
        {
            List<BudgetAssignmentAdjustment> objBudgetAssignmentAdjustments = null;
            BudgetAssignmentAdjustment objReturn = null;

            try
            {
                objBudgetAssignmentAdjustments = GetBudgetAssignmentAdjustments(Filter);
                if (objBudgetAssignmentAdjustments != null && objBudgetAssignmentAdjustments.Count >= 1) objReturn = objBudgetAssignmentAdjustments[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objBudgetAssignmentAdjustments = null;
            }
            return objReturn;
        }

        public static List<BudgetAssignmentAdjustment> GetBudgetAssignmentAdjustments()
        {
            int intTotalCount = 0;
            return GetBudgetAssignmentAdjustments(null, null, null, out intTotalCount);
        }

        public static List<BudgetAssignmentAdjustment> GetBudgetAssignmentAdjustments(BudgetAssignmentAdjustmentFilter Filter)
        {
            int intTotalCount = 0;
            return GetBudgetAssignmentAdjustments(Filter, null, null, out intTotalCount);
        }

        public static List<BudgetAssignmentAdjustment> GetBudgetAssignmentAdjustments(BudgetAssignmentAdjustmentFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetBudgetAssignmentAdjustments(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<BudgetAssignmentAdjustment> GetBudgetAssignmentAdjustments(BudgetAssignmentAdjustmentFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<BudgetAssignmentAdjustment> objReturn = null;
            BudgetAssignmentAdjustment objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<BudgetAssignmentAdjustment>();

                strSQL = "SELECT ba.BudgetID, baa.* " +
                         "FROM BudgetAssignmentAdjustment (NOLOCK) baa " +
                         "LEFT OUTER JOIN BudgetAssignment (NOLOCK) ba on ba.BudgetAssignmentID = baa.BudgetAssignmentID " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.BudgetAssignmentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.BudgetAssignmentID, "baa.BudgetAssignmentID");
                    if (Filter.BudgetID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.BudgetID, "ba.BudgetID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "BudgetAssignmentAdjustmentID" : Utility.CustomSorting.GetSortExpression(typeof(BudgetAssignmentAdjustment), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new BudgetAssignmentAdjustment(objData.Tables[0].Rows[i]);
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

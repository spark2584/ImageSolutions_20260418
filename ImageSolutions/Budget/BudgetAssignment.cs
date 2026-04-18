using ImageSolutions.Item;
using ImageSolutions.Payment;
using ImageSolutions.User;
using ImageSolutions.Website;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Budget
{
    public class BudgetAssignment : ISBase.BaseClass
    {
        public string BudgetAssignmentID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(BudgetAssignmentID); } }
        public string UserWebsiteID { get; set; }
        public string WebsiteGroupID { get; set; }
        public string BudgetID { get; set; }
        public string WebsiteID { get; set; }
        public bool InActive { get; set; }
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

        private Budget mBudget = null;
        public Budget Budget
        {
            get
            {
                if (mBudget == null && !string.IsNullOrEmpty(BudgetID))
                {
                    mBudget = new Budget(BudgetID);
                }
                return mBudget;
            }
        }

        private WebsiteGroup mWebsiteGroup = null;
        public WebsiteGroup WebsiteGroup
        {
            get
            {
                if (mWebsiteGroup == null && !string.IsNullOrEmpty(WebsiteGroupID))
                {
                    mWebsiteGroup = new WebsiteGroup(WebsiteGroupID);
                }
                return mWebsiteGroup;
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

        private List<Payment.Payment> mPayments = null;
        public List<Payment.Payment> Payments
        {
            get
            {
                if (mPayments == null && !string.IsNullOrEmpty(BudgetAssignmentID))
                {
                    PaymentFilter objFilter = null;

                    try
                    {
                        objFilter = new PaymentFilter();
                        objFilter.BudgetAssignmentID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.BudgetAssignmentID.SearchString = BudgetAssignmentID;
                        mPayments = Payment.Payment.GetPayments(objFilter);
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
                return mPayments;
            }
        }

        private List<BudgetAssignmentAdjustment> mBudgetAssignmentAdjustments = null;
        public List<BudgetAssignmentAdjustment> BudgetAssignmentAdjustments
        {
            get
            {
                if (mBudgetAssignmentAdjustments == null && !string.IsNullOrEmpty(BudgetAssignmentID))
                {
                    BudgetAssignmentAdjustmentFilter objFilter = null;

                    try
                    {
                        objFilter = new BudgetAssignmentAdjustmentFilter();
                        objFilter.BudgetAssignmentID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.BudgetAssignmentID.SearchString = BudgetAssignmentID;
                        mBudgetAssignmentAdjustments = BudgetAssignmentAdjustment.GetBudgetAssignmentAdjustments(objFilter);
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
                return mBudgetAssignmentAdjustments;
            }
        }


        public BudgetAssignment()
        {
        }
        public BudgetAssignment(string BudgetAssignmentID)
        {
            this.BudgetAssignmentID = BudgetAssignmentID;
            Load();
        }
        public BudgetAssignment(DataRow objRow)
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
                         "FROM BudgetAssignment (NOLOCK) " +
                         "WHERE BudgetAssignmentID=" + Database.HandleQuote(BudgetAssignmentID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("BudgetAssignmentID=" + BudgetAssignmentID + " is not found");
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
                if (objColumns.Contains("BudgetAssignmentID")) BudgetAssignmentID = Convert.ToString(objRow["BudgetAssignmentID"]);
                //if (objColumns.Contains("UserInfoID")) UserInfoID = Convert.ToString(objRow["UserInfoID"]);
                if (objColumns.Contains("UserWebsiteID")) UserWebsiteID = Convert.ToString(objRow["UserWebsiteID"]);
                if (objColumns.Contains("WebsiteGroupID")) WebsiteGroupID = Convert.ToString(objRow["WebsiteGroupID"]);
                if (objColumns.Contains("BudgetID")) BudgetID = Convert.ToString(objRow["BudgetID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("InActive")) InActive = Convert.ToBoolean(objRow["InActive"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(BudgetAssignmentID)) throw new Exception("Missing BudgetAssignmentID in the datarow");
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
                if (string.IsNullOrEmpty(UserWebsiteID) && string.IsNullOrEmpty(WebsiteGroupID)) throw new Exception("UserWebsiteID or WebsiteGroupID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, BudgetAssignmentID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                //dicParam["UserInfoID"] = UserInfoID;
                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["WebsiteGroupID"] = WebsiteGroupID;
                dicParam["BudgetID"] = BudgetID;
                dicParam["WebsiteID"] = WebsiteID;
                dicParam["InActive"] = InActive;
                dicParam["CreatedBy"] = CreatedBy;
                BudgetAssignmentID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "BudgetAssignment"), objConn, objTran).ToString();

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
                if (BudgetAssignmentID == null) throw new Exception("BudgetAssignmentID is required");
                if (string.IsNullOrEmpty(UserWebsiteID) && string.IsNullOrEmpty(WebsiteGroupID)) throw new Exception("UserWebsiteID or WebsiteGroupID is required");
                if (IsNew) throw new Exception("Update cannot be performed, BudgetAssignmentID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                //dicParam["UserInfoID"] = UserInfoID;
                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["WebsiteGroupID"] = WebsiteGroupID;
                dicParam["BudgetID"] = BudgetID;
                dicParam["WebsiteID"] = WebsiteID;
                dicParam["InActive"] = InActive;
                dicWParam["BudgetAssignmentID"] = BudgetAssignmentID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "BudgetAssignment"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, BudgetAssignmentID is missing");
                if (Payments != null && Payments.Count > 0) throw new Exception("Delete cannot be performed, budget has already been applied");

                dicDParam["BudgetAssignmentID"] = BudgetAssignmentID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "BudgetAssignment"), objConn, objTran);
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
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                     "FROM BudgetAssignment (NOLOCK) p " +
                     "WHERE " +
                     "(" +
                     "  (p.WebsiteID=" + Database.HandleQuote(WebsiteID) + " AND p.UserWebsiteID=" + Database.HandleQuote(UserWebsiteID) + " AND p.BudgetID=" + Database.HandleQuote(BudgetID) + ")" +
                     "  OR " +
                     "  (p.WebsiteID=" + Database.HandleQuote(WebsiteID) + " AND p.WebsiteGroupID=" + Database.HandleQuote(WebsiteGroupID) + " AND p.BudgetID=" + Database.HandleQuote(BudgetID) + " )" +
                     ") ";

            if (!string.IsNullOrEmpty(BudgetAssignmentID)) strSQL += "AND p.BudgetAssignmentID<>" + Database.HandleQuote(BudgetAssignmentID);
            return Database.HasRows(strSQL);
        }

        public static BudgetAssignment GetBudgetAssignment(BudgetAssignmentFilter Filter)
        {
            List<BudgetAssignment> objBudgetAssignments = null;
            BudgetAssignment objReturn = null;

            try
            {
                objBudgetAssignments = GetBudgetAssignments(Filter);
                if (objBudgetAssignments != null && objBudgetAssignments.Count >= 1) objReturn = objBudgetAssignments[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objBudgetAssignments = null;
            }
            return objReturn;
        }

        public static List<BudgetAssignment> GetBudgetAssignments()
        {
            int intTotalCount = 0;
            return GetBudgetAssignments(null, null, null, out intTotalCount);
        }

        public static List<BudgetAssignment> GetBudgetAssignments(BudgetAssignmentFilter Filter)
        {
            int intTotalCount = 0;
            return GetBudgetAssignments(Filter, null, null, out intTotalCount);
        }

        public static List<BudgetAssignment> GetBudgetAssignments(BudgetAssignmentFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetBudgetAssignments(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<BudgetAssignment> GetBudgetAssignments(BudgetAssignmentFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<BudgetAssignment> objReturn = null;
            BudgetAssignment objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<BudgetAssignment>();

                strSQL = "SELECT * " +
                         "FROM BudgetAssignment (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.BudgetAssignmentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.BudgetAssignmentID, "BudgetAssignmentID");
                    //if (Filter.UserInfoID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserInfoID, "UserInfoID");
                    if (Filter.UserWebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserWebsiteID, "UserWebsiteID"); 
                    if (Filter.WebsiteGroupID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteGroupID, "WebsiteGroupID");
                    if (Filter.WebsiteGroupIDs != null && Filter.WebsiteGroupIDs.Count > 0)
                    {
                        strSQL += "AND (";
                        for (int i = 0; i < Filter.WebsiteGroupIDs.Count; i++)
                        {
                            if (i > 0) strSQL += "OR ";
                            strSQL += "WebsiteGroupID = " + Filter.WebsiteGroupIDs[i];
                        }
                        strSQL += ") ";
                    }
                    if (Filter.BudgetID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.BudgetID, "BudgetID");
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.InActive != null) strSQL += "AND InActive=" + Database.HandleQuote(Convert.ToInt32(Filter.InActive.Value).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "BudgetAssignmentID" : Utility.CustomSorting.GetSortExpression(typeof(BudgetAssignment), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new BudgetAssignment(objData.Tables[0].Rows[i]);
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

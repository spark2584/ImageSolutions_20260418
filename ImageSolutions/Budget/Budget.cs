using ImageSolutions.Item;
using ImageSolutions.SalesOrder;
using ImageSolutions.User;
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
    public class Budget : ISBase.BaseClass
    {
        public string BudgetID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(BudgetID); } }
        public string BudgetName { get; set; }
        public string WebsiteID { get; set; }
        public double BudgetAmount { get; set; }
        public string Division { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IncludeShippingAndTaxes { get; set; }
        public bool DisplayShippingAndTaxes { get; set; }
        public bool TaxNonBudgetAmount { get; set; }
        public string WebsiteShippingServiceID { get; set; }
        public bool AllowOverBudget { get; set; }
        public bool ExcludeNoAmountBudget { get; set; }
        public string PaymentTermID { get; set; }
        public string ApproverUserWebsiteID { get; set; }

        public bool IsSystemGenerated { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

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


        private Website.Website mWebsite = null;
        public Website.Website Website
        {
            get
            {
                if (mWebsite == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    mWebsite = new Website.Website(WebsiteID);
                }
                return mWebsite;
            }
        }

        private List<BudgetAssignment> mBudgetAssignments = null;
        public List<BudgetAssignment> BudgetAssignments
        {
            get
            {
                if (mBudgetAssignments == null && !string.IsNullOrEmpty(BudgetID))
                {
                    BudgetAssignmentFilter objFilter = null;

                    try
                    {
                        objFilter = new BudgetAssignmentFilter();
                        objFilter.BudgetID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.BudgetID.SearchString = BudgetID;
                        mBudgetAssignments = BudgetAssignment.GetBudgetAssignments(objFilter);
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
                return mBudgetAssignments;
            }
            set
            {
                mBudgetAssignments = value;
            }
        }


        private List<BudgetAssignmentAdjustment> mBudgetAssignmentAdjustments = null;
        public List<BudgetAssignmentAdjustment> BudgetAssignmentAdjustments
        {
            get
            {
                if (mBudgetAssignmentAdjustments == null && !string.IsNullOrEmpty(BudgetID))
                {
                    BudgetAssignmentAdjustmentFilter objFilter = null;

                    try
                    {
                        objFilter = new BudgetAssignmentAdjustmentFilter();
                        objFilter.BudgetID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.BudgetID.SearchString = BudgetID;
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
            set
            {
                mBudgetAssignmentAdjustments = value;
            }
        }

        private List<SalesOrder.SalesOrder> mSalesOrders = null;
        public List<SalesOrder.SalesOrder> SalesOrders
        {
            get
            {
                if (mSalesOrders == null && !string.IsNullOrEmpty(BudgetID))
                {
                    SalesOrderFilter objFilter = null;

                    try
                    {
                        objFilter = new SalesOrderFilter();
                        objFilter.BudgetID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.BudgetID.SearchString = BudgetID;
                        mSalesOrders = SalesOrder.SalesOrder.GetSalesOrders(objFilter);
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
                return mSalesOrders;
            }
        }
        private UserWebsite mApproverUserWebsite = null;
        public UserWebsite ApproverUserWebsite
        {
            get
            {
                if (mApproverUserWebsite == null && !string.IsNullOrEmpty(ApproverUserWebsiteID))
                {
                    mApproverUserWebsite = new UserWebsite(ApproverUserWebsiteID);
                }
                return mApproverUserWebsite;
            }
        }
        private Payment.PaymentTerm mOverBudgetPaymentTerm = null;
        public Payment.PaymentTerm OverBudgetPaymentTerm
        {
            get
            {
                if (mOverBudgetPaymentTerm == null && !string.IsNullOrEmpty(PaymentTermID))
                {
                    mOverBudgetPaymentTerm = new ImageSolutions.Payment.PaymentTerm(PaymentTermID);
                }

                return mOverBudgetPaymentTerm;
            }
        }
        public Budget()
        {
        }
        public Budget(string BudgetID)
        {
            this.BudgetID = BudgetID;
            Load();
        }
        public Budget(DataRow objRow)
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
                         "FROM Budget (NOLOCK) " +
                         "WHERE BudgetID=" + Database.HandleQuote(BudgetID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("BudgetID=" + BudgetID + " is not found");
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
                if (objColumns.Contains("BudgetID")) BudgetID = Convert.ToString(objRow["BudgetID"]);
                if (objColumns.Contains("BudgetName")) BudgetName = Convert.ToString(objRow["BudgetName"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("StartDate")) StartDate = Convert.ToDateTime(objRow["StartDate"]);
                if (objColumns.Contains("EndDate")) EndDate = Convert.ToDateTime(objRow["EndDate"]);
                if (objColumns.Contains("BudgetAmount")) BudgetAmount = Convert.ToDouble(objRow["BudgetAmount"]);
                if (objColumns.Contains("Division")) Division = Convert.ToString(objRow["Division"]);
                if (objColumns.Contains("IncludeShippingAndTaxes")) IncludeShippingAndTaxes = Convert.ToBoolean(objRow["IncludeShippingAndTaxes"]);
                if (objColumns.Contains("DisplayShippingAndTaxes")) DisplayShippingAndTaxes = Convert.ToBoolean(objRow["DisplayShippingAndTaxes"]);
                if (objColumns.Contains("TaxNonBudgetAmount")) TaxNonBudgetAmount = Convert.ToBoolean(objRow["TaxNonBudgetAmount"]);
                if (objColumns.Contains("WebsiteShippingServiceID")) WebsiteShippingServiceID = Convert.ToString(objRow["WebsiteShippingServiceID"]);
                if (objColumns.Contains("AllowOverBudget")) AllowOverBudget = Convert.ToBoolean(objRow["AllowOverBudget"]);
                if (objColumns.Contains("ExcludeNoAmountBudget")) ExcludeNoAmountBudget = Convert.ToBoolean(objRow["ExcludeNoAmountBudget"]);
                if (objColumns.Contains("PaymentTermID")) PaymentTermID = Convert.ToString(objRow["PaymentTermID"]);
                if (objColumns.Contains("ApproverUserWebsiteID")) ApproverUserWebsiteID = Convert.ToString(objRow["ApproverUserWebsiteID"]);

                if (objColumns.Contains("IsSystemGenerated")) IsSystemGenerated = Convert.ToBoolean(objRow["IsSystemGenerated"]);

                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(BudgetID)) throw new Exception("Missing BudgetID in the datarow");
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
                if (WebsiteID == null) throw new Exception("WebsiteID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, BudgetID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["BudgetName"] = BudgetName;
                dicParam["WebsiteID"] = WebsiteID;
                dicParam["StartDate"] = StartDate;
                dicParam["EndDate"] = EndDate;
                dicParam["BudgetAmount"] = BudgetAmount;
                dicParam["Division"] = Division;
                dicParam["IncludeShippingAndTaxes"] = IncludeShippingAndTaxes;
                dicParam["DisplayShippingAndTaxes"] = DisplayShippingAndTaxes;
                dicParam["TaxNonBudgetAmount"] = TaxNonBudgetAmount;
                dicParam["WebsiteShippingServiceID"] = WebsiteShippingServiceID;
                dicParam["AllowOverBudget"] = AllowOverBudget;
                dicParam["ExcludeNoAmountBudget"] = ExcludeNoAmountBudget;
                dicParam["PaymentTermID"] = PaymentTermID;
                dicParam["ApproverUserWebsiteID"] = ApproverUserWebsiteID;

                dicParam["IsSystemGenerated"] = IsSystemGenerated;

                dicParam["CreatedBy"] = CreatedBy;
                BudgetID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Budget"), objConn, objTran).ToString();

                foreach (BudgetAssignment objBudgetAssignment in BudgetAssignments)
                {
                    objBudgetAssignment.BudgetID = BudgetID;
                    objBudgetAssignment.Create(objConn, objTran);
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
                if (BudgetID == null) throw new Exception("BudgetID is required");
                //if (string.IsNullOrEmpty(ExternalID)) throw new Exception("ExternalID is required");
                //if (string.IsNullOrEmpty(IncrementID)) throw new Exception("IncrementID is required");
                if (string.IsNullOrEmpty(WebsiteID)) throw new Exception("WebsiteID is required");
                if (IsNew) throw new Exception("Update cannot be performed, BudgetID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["BudgetName"] = BudgetName;
                dicParam["WebsiteID"] = WebsiteID;
                dicParam["StartDate"] = StartDate;
                dicParam["EndDate"] = EndDate;
                dicParam["BudgetAmount"] = BudgetAmount;
                dicParam["Division"] = Division;
                dicParam["IncludeShippingAndTaxes"] = IncludeShippingAndTaxes;
                dicParam["DisplayShippingAndTaxes"] = DisplayShippingAndTaxes;
                dicParam["TaxNonBudgetAmount"] = TaxNonBudgetAmount;
                dicParam["WebsiteShippingServiceID"] = WebsiteShippingServiceID;
                dicParam["AllowOverBudget"] = AllowOverBudget;
                dicParam["ExcludeNoAmountBudget"] = ExcludeNoAmountBudget;
                dicParam["PaymentTermID"] = PaymentTermID;
                dicParam["ApproverUserWebsiteID"] = ApproverUserWebsiteID;
                
                dicParam["IsSystemGenerated"] = IsSystemGenerated;

                dicWParam["BudgetID"] = BudgetID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Budget"), objConn, objTran);

                //foreach (BudgetAssignment objBudgetAssignment in BudgetAssignments)
                //{
                //    objBudgetAssignment.Update(objConn, objTran);
                //}

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
                if (IsNew) throw new Exception("Delete cannot be performed, BudgetID is missing");

                foreach (BudgetAssignment objBudgetAssignment in BudgetAssignments)
                {
                    objBudgetAssignment.Delete(objConn, objTran);
                }

                dicDParam["BudgetID"] = BudgetID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Budget"), objConn, objTran);
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

        public bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                    "FROM Budget (NOLOCK) p " + 
                    "WHERE " +
                    "(" +
                    "  (p.BudgetName=" + Database.HandleQuote(BudgetName) + " AND p.WebsiteID=" + Database.HandleQuote(WebsiteID) + ")" +
                    ") ";


            if (!string.IsNullOrEmpty(BudgetID)) strSQL += "AND p.BudgetID<>" + Database.HandleQuote(BudgetID);
            return Database.HasRows(strSQL);
        }

        public static Budget GetBudget(BudgetFilter Filter)
        {
            List<Budget> objBudgets = null;
            Budget objReturn = null;

            try
            {
                objBudgets = GetBudgets(Filter);
                if (objBudgets != null && objBudgets.Count >= 1) objReturn = objBudgets[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objBudgets = null;
            }
            return objReturn;
        }

        public static List<Budget> GetBudgets()
        {
            int intTotalCount = 0;
            return GetBudgets(null, null, null, out intTotalCount);
        }

        public static List<Budget> GetBudgets(BudgetFilter Filter)
        {
            int intTotalCount = 0;
            return GetBudgets(Filter, null, null, out intTotalCount);
        }

        public static List<Budget> GetBudgets(BudgetFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetBudgets(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Budget> GetBudgets(BudgetFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Budget> objReturn = null;
            Budget objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Budget>();

                strSQL = "SELECT DISTINCT b.* " +
                         "FROM Budget (NOLOCK) b " +
                         "LEFT OUTER JOIN BudgetAssignment (NOLOCK) ba on ba.BudgetID = b.BudgetID " +
                         "LEFT OUTER JOIN UserWebsite (NOLOCK) uw on uw.UserWebsiteID = ba.UserWebsiteID " +
                         "LEFT OUTER JOIN UserInfo (NOLOCK) u on u.UserInfoID = uw.UserInfoID " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.BudgetID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.BudgetID, "b.BudgetID");
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "b.WebsiteID");
                    if (Filter.BudgetName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.BudgetName, "b.BudgetName");
                    if (Filter.Division != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Division, "b.Division");
                    if (Filter.Email != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Email, "u.EmailAddress");
                    if (Filter.ApproverUserWebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ApproverUserWebsiteID, "b.ApproverUserWebsiteID");

                    if (Filter.StartDate != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.StartDate, "b.StartDate");
                    if (Filter.EndDate != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.EndDate, "b.EndDate");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "BudgetID" : Utility.CustomSorting.GetSortExpression(typeof(Budget), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else
                    strSQL += "ORDER BY CreatedOn ";

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Budget(objData.Tables[0].Rows[i]);
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

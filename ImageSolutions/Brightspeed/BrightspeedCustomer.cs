using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Brightspeed
{
    public class BrightspeedCustomer : ISBase.BaseClass
    {
        public string BrightspeedCustomerID { get; private set; }
        public string InternalID { get; set; }
        public string EmployeeID { get; set; }
        public string DisplayName{ get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Manager { get; set; }
        public string BusinessUnit { get; set; }
        public string DivisionName { get; set; }
        public string DepartmentName { get; set; }
        public string CostCenterCode { get; set; }
        public string Location { get; set; }
        public string EmployeeStatus { get; set; }
        public string PersonnelSubArea { get; set; }
        public string JobCode { get; set; }
        public DateTime? HireDate { get; set; }
        public bool InActive { get; set; }
        public bool IsUpdated { get; set; }
        public string SyncID { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(BrightspeedCustomerID); } }        

        public BrightspeedCustomer()
        {
        }
        public BrightspeedCustomer(string BrightspeedCustomerID)
        {
            this.BrightspeedCustomerID = BrightspeedCustomerID;
            Load();
        }
        public BrightspeedCustomer(DataRow objRow)
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
                         "FROM BrightspeedCustomer (NOLOCK) " +
                         "WHERE BrightspeedCustomerID=" + Database.HandleQuote(BrightspeedCustomerID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("BrightspeedCustomerID=" + BrightspeedCustomerID + " is not found");
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

                if (objColumns.Contains("BrightspeedCustomerID")) BrightspeedCustomerID = Convert.ToString(objRow["BrightspeedCustomerID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("EmployeeID")) EmployeeID = Convert.ToString(objRow["EmployeeID"]);
                if (objColumns.Contains("DisplayName")) DisplayName = Convert.ToString(objRow["DisplayName"]);
                if (objColumns.Contains("Email")) Email = Convert.ToString(objRow["Email"]);
                if (objColumns.Contains("Title")) Title = Convert.ToString(objRow["Title"]);
                if (objColumns.Contains("Manager")) Manager = Convert.ToString(objRow["Manager"]);
                if (objColumns.Contains("BusinessUnit")) BusinessUnit = Convert.ToString(objRow["BusinessUnit"]);
                if (objColumns.Contains("DivisionName")) DivisionName = Convert.ToString(objRow["DivisionName"]);
                if (objColumns.Contains("DepartmentName")) DepartmentName = Convert.ToString(objRow["DepartmentName"]);
                if (objColumns.Contains("CostCenterCode")) CostCenterCode = Convert.ToString(objRow["CostCenterCode"]);
                if (objColumns.Contains("Location")) Location = Convert.ToString(objRow["Location"]);
                if (objColumns.Contains("EmployeeStatus")) EmployeeStatus = Convert.ToString(objRow["EmployeeStatus"]);
                if (objColumns.Contains("PersonnelSubArea")) PersonnelSubArea = Convert.ToString(objRow["PersonnelSubArea"]);
                if (objColumns.Contains("JobCode")) JobCode = Convert.ToString(objRow["JobCode"]);
                if (objColumns.Contains("HireDate") && objRow["HireDate"] != DBNull.Value) HireDate = Convert.ToDateTime(objRow["HireDate"]);
                if (objColumns.Contains("InActive") && objRow["InActive"] != DBNull.Value) InActive = Convert.ToBoolean(objRow["InActive"]);                
                if (objColumns.Contains("IsUpdated") && objRow["IsUpdated"] != DBNull.Value) IsUpdated = Convert.ToBoolean(objRow["IsUpdated"]);
                if (objColumns.Contains("SyncID")) SyncID = Convert.ToString(objRow["SyncID"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(BrightspeedCustomerID)) throw new Exception("Missing BrightspeedCustomerID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, BrightspeedCustomerID already exists");

                dicParam["InternalID"] = InternalID;
                dicParam["EmployeeID"] = EmployeeID;
                dicParam["DisplayName"] = DisplayName;
                dicParam["Email"] = Email;
                dicParam["Title"] = Title;
                dicParam["Manager"] = Manager;
                dicParam["BusinessUnit"] = BusinessUnit;
                dicParam["DivisionName"] = DivisionName;
                dicParam["DepartmentName"] = DepartmentName;
                dicParam["CostCenterCode"] = CostCenterCode;
                dicParam["Location"] = Location;
                dicParam["EmployeeStatus"] = EmployeeStatus;
                dicParam["PersonnelSubArea"] = PersonnelSubArea;
                dicParam["JobCode"] = JobCode;
                dicParam["HireDate"] = HireDate;
                dicParam["InActive"] = InActive;
                dicParam["IsUpdated"] = IsUpdated;
                dicParam["SyncID"] = SyncID;
                dicParam["ErrorMessage"] = ErrorMessage;

                BrightspeedCustomerID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "BrightspeedCustomer"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, BrightspeedCustomerID is missing");

                dicParam["InternalID"] = InternalID;
                dicParam["EmployeeID"] = EmployeeID;
                dicParam["DisplayName"] = DisplayName;
                dicParam["Email"] = Email;
                dicParam["Title"] = Title;
                dicParam["Manager"] = Manager;
                dicParam["BusinessUnit"] = BusinessUnit;
                dicParam["DivisionName"] = DivisionName;
                dicParam["DepartmentName"] = DepartmentName;
                dicParam["CostCenterCode"] = CostCenterCode;
                dicParam["Location"] = Location;
                dicParam["EmployeeStatus"] = EmployeeStatus;
                dicParam["PersonnelSubArea"] = PersonnelSubArea;
                dicParam["JobCode"] = JobCode;
                dicParam["HireDate"] = HireDate;
                dicParam["InActive"] = InActive;
                dicParam["IsUpdated"] = IsUpdated;
                dicParam["SyncID"] = SyncID;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = DateTime.UtcNow;

                dicWParam["BrightspeedCustomerID"] = BrightspeedCustomerID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "BrightspeedCustomer"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, BrightspeedCustomerID is missing");

                dicDParam["BrightspeedCustomerID"] = BrightspeedCustomerID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "BrightspeedCustomer"), objConn, objTran);
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
            return false;
        }

        public static BrightspeedCustomer GetBrightspeedCustomer(BrightspeedCustomerFilter Filter)
        {
            List<BrightspeedCustomer> objBrightspeedCustomers = null;
            BrightspeedCustomer objReturn = null;

            try
            {
                objBrightspeedCustomers = GetBrightspeedCustomers(Filter);
                if (objBrightspeedCustomers != null && objBrightspeedCustomers.Count >= 1) objReturn = objBrightspeedCustomers[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objBrightspeedCustomers = null;
            }
            return objReturn;
        }

        public static List<BrightspeedCustomer> GetBrightspeedCustomers()
        {
            int intTotalCount = 0;
            return GetBrightspeedCustomers(null, null, null, out intTotalCount);
        }

        public static List<BrightspeedCustomer> GetBrightspeedCustomers(BrightspeedCustomerFilter Filter)
        {
            int intTotalCount = 0;
            return GetBrightspeedCustomers(Filter, null, null, out intTotalCount);
        }

        public static List<BrightspeedCustomer> GetBrightspeedCustomers(BrightspeedCustomerFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetBrightspeedCustomers(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<BrightspeedCustomer> GetBrightspeedCustomers(BrightspeedCustomerFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<BrightspeedCustomer> objReturn = null;
            BrightspeedCustomer objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<BrightspeedCustomer>();

                strSQL = "SELECT * " +
                         "FROM BrightspeedCustomer (NOLOCK) " +
                         "where 1=1 ";

                if (Filter != null)
                {
                    if (Filter.EmployeeID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EmployeeID, "EmployeeID");
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.Email != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Email, "Email");
                    if (Filter.SyncID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SyncID, "SyncID");
                    if (Filter.IsUpdated != null) strSQL += "AND IsUpdated=" + Database.HandleQuote(Convert.ToInt32(Filter.IsUpdated.Value).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "BrightspeedCustomerID" : Utility.CustomSorting.GetSortExpression(typeof(BrightspeedCustomer), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else
                {
                    strSQL += " ORDER BY BrightspeedCustomerID DESC";
                }

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new BrightspeedCustomer(objData.Tables[0].Rows[i]);
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Employee
{
    public class EmployeeFileImport : ISBase.BaseClass
    {
        public string EmployeeFileImportID { get; private set; }
        public string WebsiteID { get; set; }
        public string UserWebsiteID { get; set; }
        public string Region { get; set; }
        public string CostCenter { get; set; }
        public string EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string TimeType { get; set; }
        public DateTime? ServiceDate { get; set; }
        public DateTime? DateOfHire { get; set; }
        public string NewHire { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        public EmployeeFileImport()
        {
        }
        public EmployeeFileImport(string EmployeeFileImportID)
        {
            this.EmployeeFileImportID = EmployeeFileImportID;
            Load();
        }
        public EmployeeFileImport(DataRow objRow)
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
                         "FROM EmployeeFileImport (NOLOCK) " +
                         "WHERE EmployeeFileImportID=" + Database.HandleQuote(EmployeeFileImportID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("EmployeeFileImportID=" + EmployeeFileImportID + " is not found");
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

                if (objColumns.Contains("EmployeeFileImportID")) EmployeeFileImportID = Convert.ToString(objRow["EmployeeFileImportID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("UserWebsiteID")) UserWebsiteID = Convert.ToString(objRow["UserWebsiteID"]);
                if (objColumns.Contains("Region")) Region = Convert.ToString(objRow["Region"]);
                if (objColumns.Contains("CostCenter")) CostCenter = Convert.ToString(objRow["CostCenter"]);
                if (objColumns.Contains("EmployeeID")) EmployeeID = Convert.ToString(objRow["EmployeeID"]);
                if (objColumns.Contains("LastName")) LastName = Convert.ToString(objRow["LastName"]);
                if (objColumns.Contains("FirstName")) FirstName = Convert.ToString(objRow["FirstName"]);
                if (objColumns.Contains("MiddleName")) MiddleName = Convert.ToString(objRow["MiddleName"]);
                if (objColumns.Contains("TimeType")) TimeType = Convert.ToString(objRow["TimeType"]);
                if (objColumns.Contains("ServiceDate") && objRow["ServiceDate"] != DBNull.Value) ServiceDate = Convert.ToDateTime(objRow["ServiceDate"]);
                if (objColumns.Contains("DateOfHire") && objRow["DateOfHire"] != DBNull.Value) DateOfHire = Convert.ToDateTime(objRow["DateOfHire"]);
                if (objColumns.Contains("NewHire")) NewHire = Convert.ToString(objRow["NewHire"]);
                if (objColumns.Contains("Password")) Password = Convert.ToString(objRow["Password"]);
                if (objColumns.Contains("Status")) Status = Convert.ToString(objRow["Status"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) CreatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(EmployeeFileImportID)) throw new Exception("Missing EmployeeFileImportID in the datarow");
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
                dicParam["WebsiteID"] = WebsiteID;
                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["Region"] = Region;
                dicParam["CostCenter"] = CostCenter;
                dicParam["EmployeeID"] = EmployeeID;
                dicParam["LastName"] = LastName;
                dicParam["FirstName"] = FirstName;
                dicParam["MiddleName"] = MiddleName;
                dicParam["TimeType"] = TimeType;
                dicParam["ServiceDate"] = ServiceDate;
                dicParam["DateOfHire"] = DateOfHire;
                dicParam["NewHire"] = NewHire;
                dicParam["Password"] = Password;
                dicParam["Status"] = Status;
                dicParam["ErrorMessage"] = ErrorMessage;

                EmployeeFileImportID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "EmployeeFileImport"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(EmployeeFileImportID)) throw new Exception("EmployeeFileImportID is required");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["Region"] = Region;
                dicParam["CostCenter"] = CostCenter;
                dicParam["EmployeeID"] = EmployeeID;
                dicParam["LastName"] = LastName;
                dicParam["FirstName"] = FirstName;
                dicParam["MiddleName"] = MiddleName;
                dicParam["TimeType"] = TimeType;
                dicParam["ServiceDate"] = ServiceDate;
                dicParam["DateOfHire"] = DateOfHire;
                dicParam["NewHire"] = NewHire;
                dicParam["Password"] = Password;
                dicParam["Status"] = Status;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["EmployeeFileImportID"] = EmployeeFileImportID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "EmployeeFileImport"), objConn, objTran);

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
                dicDParam["EmployeeFileImportID"] = EmployeeFileImportID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "EmployeeFileImport"), objConn, objTran);
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

        public static EmployeeFileImport GetEmployeeFileImport(EmployeeFileImportFilter Filter)
        {
            List<EmployeeFileImport> objTaskEntries = null;
            EmployeeFileImport objReturn = null;

            try
            {
                objTaskEntries = GetEmployeeFileImports(Filter);
                if (objTaskEntries != null && objTaskEntries.Count >= 1) objReturn = objTaskEntries[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objTaskEntries = null;
            }
            return objReturn;
        }

        public static List<EmployeeFileImport> GetEmployeeFileImports()
        {
            int intTotalCount = 0;
            return GetEmployeeFileImports(null, null, null, out intTotalCount);
        }

        public static List<EmployeeFileImport> GetEmployeeFileImports(EmployeeFileImportFilter Filter)
        {
            int intTotalCount = 0;
            return GetEmployeeFileImports(Filter, null, null, out intTotalCount);
        }

        public static List<EmployeeFileImport> GetEmployeeFileImports(EmployeeFileImportFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetEmployeeFileImports(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<EmployeeFileImport> GetEmployeeFileImports(EmployeeFileImportFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<EmployeeFileImport> objReturn = null;
            EmployeeFileImport objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<EmployeeFileImport>();

                strSQL = "SELECT * " +
                         "FROM EmployeeFileImport (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.EmployeeID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EmployeeID, "EmployeeID");
                    if (Filter.Status != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Status, "Status");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "EmployeeFileImportID" : Utility.CustomSorting.GetSortExpression(typeof(EmployeeFileImport), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new EmployeeFileImport(objData.Tables[0].Rows[i]);
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

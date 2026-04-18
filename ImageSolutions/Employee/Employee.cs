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
    public class Employee : ISBase.BaseClass
    {
        public string EmployeeID { get; private set; }
        public string ExternalID { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public bool IsSalesRep { get; set; }
        public bool IsPM { get; set; }
        public bool IsWarehouse { get; set; }
        public bool IsWarehouseManager { get; set; }
        public string NetSuiteEntityID { get; set; }
        public string NetSuiteInternalID { get; set; }
        public string ScanNumber { get; set; }
        public string WarehouseID { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(EmployeeID); } }
        public bool IsSalesOrder { get; set; }
        public bool IsInvoiceFulfillment { get; set; }
        public bool IsRemainingTransaction { get; set; }

        //private Member.Member mMember = null;
        //public Member.Member Member
        //{
        //    get
        //    {
        //        if (mMember == null && !string.IsNullOrEmpty(Email))
        //        {
        //            IPower.Member.MemberFilter objFilter = null;

        //            try
        //            {
        //                objFilter = new IPower.Member.MemberFilter();
        //                objFilter.Email = new Database.Filter.StringSearch.SearchFilter();
        //                objFilter.Email.SearchString = Email;
        //                mMember = IPower.Member.Member.GetMember(objFilter);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //            finally
        //            {
        //                objFilter = null;
        //            }
        //        }
        //        return mMember;
        //    }
        //}

        public Employee()
        {
        }
        public Employee(string EmployeeID)
        {
            this.EmployeeID = EmployeeID;
            Load();
        }
        public Employee(DataRow objRow)
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
                //strSQL = "SELECT * " +
                //         "FROM Employee (NOLOCK) " +
                //         "WHERE EmployeeID=" + Database.HandleQuote(EmployeeID);
                //objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("EmployeeID=" + EmployeeID + " is not found");
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

                if (objColumns.Contains("EmployeeID")) EmployeeID = Convert.ToString(objRow["EmployeeID"]);
                if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);

                if (objColumns.Contains("Email")) Email = Convert.ToString(objRow["Email"]);
                if (objColumns.Contains("NetSuiteEntityID")) NetSuiteEntityID = Convert.ToString(objRow["NetSuiteEntityID"]);
                if (objColumns.Contains("NetSuiteInternalID")) NetSuiteInternalID = Convert.ToString(objRow["NetSuiteInternalID"]);
                if (objColumns.Contains("ScanNumber")) ScanNumber = Convert.ToString(objRow["ScanNumber"]);

                if (objColumns.Contains("FirstName")) FirstName = Convert.ToString(objRow["FirstName"]);
                if (objColumns.Contains("LastName")) LastName = Convert.ToString(objRow["LastName"]);
                if (objColumns.Contains("UserName")) UserName = Convert.ToString(objRow["UserName"]);
                if (objColumns.Contains("IsSalesRep")) IsSalesRep = Convert.ToBoolean(objRow["IsSalesRep"]);
                if (objColumns.Contains("IsPM")) IsPM = Convert.ToBoolean(objRow["IsPM"]);
                if (objColumns.Contains("IsWarehouse")) IsWarehouse = Convert.ToBoolean(objRow["IsWarehouse"]);
                if (objColumns.Contains("IsWarehouseManager")) IsWarehouseManager = Convert.ToBoolean(objRow["IsWarehouseManager"]);

                if (objColumns.Contains("WarehouseID")) WarehouseID = Convert.ToString(objRow["WarehouseID"]);

                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) CreatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
                if (string.IsNullOrEmpty(EmployeeID)) throw new Exception("Missing EmployeeID in the datarow");
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
                //if (string.IsNullOrEmpty(ExternalID)) throw new Exception("ExternalID is required");
                //if (!IsPM && !IsSalesRep) throw new Exception("Employee must be a sales rep or PM");
                if (!IsNew) throw new Exception("Create cannot be performed, EmployeeID already exists");

                dicParam["NetSuiteInternalID"] = NetSuiteInternalID;
                dicParam["NetSuiteEntityID"] = NetSuiteEntityID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["ScanNumber"] = ScanNumber;
                dicParam["Email"] = Email;
                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["UserName"] = UserName;
                dicParam["IsSalesRep"] = IsSalesRep;
                dicParam["IsPM"] = IsPM;
                dicParam["IsWarehouse"] = IsWarehouse;
                dicParam["IsWarehouseManager"] = IsWarehouseManager;
                dicParam["WarehouseID"] = WarehouseID;
                EmployeeID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Employee"), objConn, objTran).ToString();
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
                //if (!IsPM && !IsSalesRep) throw new Exception("Employee must be a sales rep or PM");
                if (IsNew) throw new Exception("Update cannot be performed, EmployeeID is missing");

                dicParam["NetSuiteInternalID"] = NetSuiteInternalID;
                dicParam["NetSuiteEntityID"] = NetSuiteEntityID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["ScanNumber"] = ScanNumber;
                dicParam["Email"] = Email;
                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["UserName"] = UserName;
                dicParam["IsSalesRep"] = IsSalesRep;
                dicParam["IsPM"] = IsPM;
                dicParam["IsWarehouse"] = IsWarehouse;
                dicParam["IsWarehouseManager"] = IsWarehouseManager;
                dicParam["WarehouseID"] = WarehouseID;
                dicWParam["EmployeeID"] = EmployeeID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Employee"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, EmployeeID is missing");

                dicDParam["EmployeeID"] = EmployeeID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Employee"), objConn, objTran);
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

        public static Employee GetEmployee(EmployeeFilter Filter)
        {
            List<Employee> objEmployees = null;
            Employee objReturn = null;

            try
            {
                objEmployees = GetEmployees(Filter);
                if (objEmployees != null && objEmployees.Count >= 1) objReturn = objEmployees[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEmployees = null;
            }
            return objReturn;
        }

        public static List<Employee> GetEmployees()
        {
            int intTotalCount = 0;
            return GetEmployees(null, null, null, out intTotalCount);
        }

        public static List<Employee> GetEmployees(EmployeeFilter Filter)
        {
            int intTotalCount = 0;
            return GetEmployees(Filter, null, null, out intTotalCount);
        }

        public static List<Employee> GetEmployees(EmployeeFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetEmployees(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Employee> GetEmployees(EmployeeFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Employee> objReturn = null;
            Employee objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Employee>();

                strSQL = "SELECT * " +
                         "FROM Employee  (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.IsPM != null) strSQL += "AND IsPM=" + Database.HandleQuote(Convert.ToInt32(Filter.IsPM.Value).ToString());
                    if (Filter.IsSalesRep != null) strSQL += "AND IsSalesRep=" + Database.HandleQuote(Convert.ToInt32(Filter.IsSalesRep.Value).ToString());
                    if (Filter.NetSuiteInternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.NetSuiteInternalID, "NetSuiteInternalID");
                    if (Filter.ExternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ExternalID, "ExternalID");
                    if (Filter.Email != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Email, "Email");
                    if (Filter.ScanNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ScanNumber, "ScanNumber");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "EmployeeID" : Utility.CustomSorting.GetSortExpression(typeof(Employee), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Employee(objData.Tables[0].Rows[i]);
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

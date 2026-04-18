using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Customer
{
    public class Customer : ISBase.BaseClass
    {
        public string CustomerID { get; private set; }
        public string ParentID { get; set; }
        public string InternalID { get; set; }
        public string ExternalID { get; set; }
        public string Email { get; set; }
        public string StoreNumber { get; set; }
        public string EmployeeID { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhysicalAddressID { get; set; }
        public string ShippingAddressID { get; set; }
        public string PhoneNumber { get; set; }
        public string Area { get; set; }
        public string Region { get; set; }
        public string BrandName { get; set; }
        public string BranchType { get; set; }
        public string CountryCode { get; set; }
        public string Title { get; set; }
        public string Job { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? ActionDate { get; set; }
        public string Password { get; set; }
        public bool IsIndividual { get; set; }
        public bool IsUpdated { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(CustomerID); } }

        private Address.Address mShipToAddress = null;
        public Address.Address ShipToAddress
        {
            get
            {
                if (mShipToAddress == null && !string.IsNullOrEmpty(ShippingAddressID))
                {
                    mShipToAddress = new Address.Address(ShippingAddressID);                   
                }
                return mShipToAddress;
            }
            set
            {
                mShipToAddress = value;
            }
        }

        private Customer mParent = null;
        public Customer Parent
        {
            get
            {
                if (mParent == null && !string.IsNullOrEmpty(ParentID))
                {
                    mParent = new Customer(ParentID);
                }
                return mParent;
            }
            set
            {
                mParent = value;
            }
        }


        private List<SalesOrder.SalesOrder> mSalesOrders = null;
        public List<SalesOrder.SalesOrder> SalesOrders
        {
            get
            {
                if (mSalesOrders == null && !string.IsNullOrEmpty(CustomerID))
                {
                    SalesOrder.SalesOrderFilter objFilter = null;

                    try
                    {
                        objFilter = new SalesOrder.SalesOrderFilter();
                        objFilter.CustomerID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.CustomerID.SearchString = CustomerID;
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
            set
            {
                mSalesOrders = value;
            }
        }

        public Customer()
        {
        }
        public Customer(string CustomerID)
        {
            this.CustomerID = CustomerID;
            Load();
        }
        public Customer(DataRow objRow)
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
                         "FROM Customer (NOLOCK) " +
                         "WHERE CustomerID=" + Database.HandleQuote(CustomerID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("CustomerID=" + CustomerID + " is not found");
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

                if (objColumns.Contains("CustomerID")) CustomerID = Convert.ToString(objRow["CustomerID"]);
                if (objColumns.Contains("ParentID")) ParentID = Convert.ToString(objRow["ParentID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
                if (objColumns.Contains("Email")) Email = Convert.ToString(objRow["Email"]);
                if (objColumns.Contains("StoreNumber")) StoreNumber = Convert.ToString(objRow["StoreNumber"]);
                if (objColumns.Contains("EmployeeID")) EmployeeID = Convert.ToString(objRow["EmployeeID"]);
                if (objColumns.Contains("CompanyName")) CompanyName = Convert.ToString(objRow["CompanyName"]);
                if (objColumns.Contains("FirstName")) FirstName = Convert.ToString(objRow["FirstName"]);
                if (objColumns.Contains("LastName")) LastName = Convert.ToString(objRow["LastName"]);
                if (objColumns.Contains("PhysicalAddressID")) PhysicalAddressID = Convert.ToString(objRow["PhysicalAddressID"]);
                if (objColumns.Contains("ShippingAddressID")) ShippingAddressID = Convert.ToString(objRow["ShippingAddressID"]);
                if (objColumns.Contains("PhoneNumber")) PhoneNumber = Convert.ToString(objRow["PhoneNumber"]);
                if (objColumns.Contains("Area")) Area = Convert.ToString(objRow["Area"]);
                if (objColumns.Contains("Region")) Region = Convert.ToString(objRow["Region"]);
                if (objColumns.Contains("BrandName")) BrandName = Convert.ToString(objRow["BrandName"]);
                if (objColumns.Contains("BranchType")) BranchType = Convert.ToString(objRow["BranchType"]);
                if (objColumns.Contains("CountryCode")) CountryCode = Convert.ToString(objRow["CountryCode"]);
                if (objColumns.Contains("Title")) Title = Convert.ToString(objRow["Title"]);
                if (objColumns.Contains("Job")) Job = Convert.ToString(objRow["Job"]);
                if (objColumns.Contains("HireDate") && objRow["HireDate"] != DBNull.Value) HireDate = Convert.ToDateTime(objRow["HireDate"]);
                if (objColumns.Contains("ActionDate") && objRow["ActionDate"] != DBNull.Value) ActionDate = Convert.ToDateTime(objRow["ActionDate"]);
                if (objColumns.Contains("Password")) Password = Convert.ToString(objRow["Password"]);
                if (objColumns.Contains("IsIndividual") && objRow["IsIndividual"] != DBNull.Value) IsIndividual = Convert.ToBoolean(objRow["IsIndividual"]);
                if (objColumns.Contains("IsUpdated") && objRow["IsUpdated"] != DBNull.Value) IsUpdated = Convert.ToBoolean(objRow["IsUpdated"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(CustomerID)) throw new Exception("Missing CustomerID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, CustomerID already exists");

                dicParam["InternalID"] = InternalID;
                dicParam["ParentID"] = ParentID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["Email"] = Email;
                dicParam["StoreNumber"] = StoreNumber;
                dicParam["EmployeeID"] = EmployeeID;
                dicParam["CompanyName"] = CompanyName;
                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["PhysicalAddressID"] = PhysicalAddressID;
                dicParam["ShippingAddressID"] = ShippingAddressID;
                dicParam["PhoneNumber"] = PhoneNumber;
                dicParam["Area"] = Area;
                dicParam["Region"] = Region;
                dicParam["BrandName"] = BrandName;
                dicParam["BranchType"] = BranchType;
                dicParam["CountryCode"] = CountryCode;
                dicParam["Password"] = Password;
                dicParam["Job"] = Job;
                dicParam["Title"] = Title;
                dicParam["HireDate"] = HireDate;
                dicParam["ActionDate"] = ActionDate;
                dicParam["IsIndividual"] = IsIndividual;
                dicParam["IsUpdated"] = IsUpdated;
                dicParam["ErrorMessage"] = ErrorMessage;

                CustomerID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Customer"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, CustomerID is missing");

                dicParam["InternalID"] = InternalID;
                dicParam["ParentID"] = ParentID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["Email"] = Email;
                dicParam["StoreNumber"] = StoreNumber;
                dicParam["EmployeeID"] = EmployeeID;
                dicParam["CompanyName"] = CompanyName;
                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["PhysicalAddressID"] = PhysicalAddressID;
                dicParam["ShippingAddressID"] = ShippingAddressID;
                dicParam["PhoneNumber"] = PhoneNumber;
                dicParam["Area"] = Area;
                dicParam["Region"] = Region;
                dicParam["BrandName"] = BrandName;
                dicParam["BranchType"] = BranchType;
                dicParam["CountryCode"] = CountryCode;
                dicParam["Password"] = Password;
                dicParam["Title"] = Title;
                dicParam["Job"] = Job;
                dicParam["HireDate"] = HireDate;
                dicParam["ActionDate"] = ActionDate;
                dicParam["IsIndividual"] = IsIndividual;
                dicParam["IsUpdated"] = IsUpdated;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = DateTime.UtcNow;

                dicWParam["CustomerID"] = CustomerID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Customer"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, CustomerID is missing");

                dicDParam["CustomerID"] = CustomerID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Customer"), objConn, objTran);
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

        public static Customer GetCustomer(CustomerFilter Filter)
        {
            List<Customer> objCustomers = null;
            Customer objReturn = null;

            try
            {
                objCustomers = GetCustomers(Filter);
                if (objCustomers != null && objCustomers.Count >= 1) objReturn = objCustomers[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomers = null;
            }
            return objReturn;
        }

        public static List<Customer> GetCustomers()
        {
            int intTotalCount = 0;
            return GetCustomers(null, null, null, out intTotalCount);
        }

        public static List<Customer> GetCustomers(CustomerFilter Filter)
        {
            int intTotalCount = 0;
            return GetCustomers(Filter, null, null, out intTotalCount);
        }

        public static List<Customer> GetCustomers(CustomerFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetCustomers(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Customer> GetCustomers(CustomerFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Customer> objReturn = null;
            Customer objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Customer>();

                strSQL = "SELECT * " +
                         "FROM Customer (NOLOCK) " +
                         "where 1=1 ";

                if (Filter != null)
                {
                    if (Filter.EmployeeID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EmployeeID, "EmployeeID");
                    if (Filter.StoreNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.StoreNumber, "StoreNumber");
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.ExternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ExternalID, "ExternalID");
                    if (Filter.ParentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ParentID, "ParentID");
                    if (Filter.Email != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Email, "Email");
                    if (Filter.IsIndividual != null) strSQL += "AND IsIndividual=" + Database.HandleQuote(Convert.ToInt32(Filter.IsIndividual.Value).ToString());
                    if (Filter.IsUpdated != null) strSQL += "AND IsUpdated=" + Database.HandleQuote(Convert.ToInt32(Filter.IsUpdated.Value).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "CustomerID" : Utility.CustomSorting.GetSortExpression(typeof(Customer), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else
                {
                    strSQL += " ORDER BY CustomerID DESC";
                }

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Customer(objData.Tables[0].Rows[i]);
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

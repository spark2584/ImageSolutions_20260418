using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Enterprise
{
    public class EnterpriseCustomer : ISBase.BaseClass
    {
        public string EnterpriseCustomerID { get; private set; }
        public string ParentID { get; set; }
        public string InternalID { get; set; }
        public string EBAInternalID { get; set; }
        public string ExternalID { get; set; }
        public string Email { get; set; }
        public string StoreNumber { get; set; }
        public string EmployeeID { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhysicalEnterpriseAddressID { get; set; }
        public string ShippingEnterpriseAddressID { get; set; }
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
        public DateTime? TermDate { get; set; }
        public string CustomerStatus { get; set; }
        public string CustomerRegTemp { get; set; }
        public string CustomerBrand { get; set; }
        public string CustomerGPBR { get; set; }
        public string NotificationEmail { get; set; }
        public string Password { get; set; }
        public bool IsIndividual { get; set; }
        public bool IsAirport { get; set; }
        public string BranchAdminLgcyID { get; set; }
        public string BranchAdminPSID { get; set; }
        public string PSGroup { get; set; }
        public string WorkdayID { get; set; }
        public bool IsPreEmployee { get; set; }
        public bool IsRegionalized { get; set; }
        public string AirportCode { get; set; }
        public DateTime? BudgetEndDate { get; set; }
        public DateTime? BudgetRefreshedOn { get; set; }
        public bool IsUpdated { get; set; }
        public string SyncID { get; set; }
        public bool InActivePreEmployee { get; set; }
        public bool InActive { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(EnterpriseCustomerID); } }
        private EnterpriseAddress mPhysicalAddress = null;
        public EnterpriseAddress PhysicalAddress
        {
            get
            {
                if (mPhysicalAddress == null && !string.IsNullOrEmpty(PhysicalEnterpriseAddressID))
                {
                    mPhysicalAddress = new EnterpriseAddress(PhysicalEnterpriseAddressID);
                }
                return mPhysicalAddress;
            }
            set
            {
                mPhysicalAddress = value;
            }
        }

        private EnterpriseAddress mShipToAddress = null;
        public EnterpriseAddress ShipToAddress
        {
            get
            {
                if (mShipToAddress == null && !string.IsNullOrEmpty(ShippingEnterpriseAddressID))
                {
                    mShipToAddress = new EnterpriseAddress(ShippingEnterpriseAddressID);                   
                }
                return mShipToAddress;
            }
            set
            {
                mShipToAddress = value;
            }
        }

        private EnterpriseCustomer mParent = null;
        public EnterpriseCustomer Parent
        {
            get
            {
                if (mParent == null && !string.IsNullOrEmpty(ParentID))
                {
                    mParent = new EnterpriseCustomer(ParentID);
                }
                return mParent;
            }
            set
            {
                mParent = value;
            }
        }

        public EnterpriseCustomer()
        {
        }
        public EnterpriseCustomer(string EnterpriseCustomerID)
        {
            this.EnterpriseCustomerID = EnterpriseCustomerID;
            Load();
        }
        public EnterpriseCustomer(DataRow objRow)
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
                         "FROM EnterpriseCustomer (NOLOCK) " +
                         "WHERE EnterpriseCustomerID=" + Database.HandleQuote(EnterpriseCustomerID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("EnterpriseCustomerID=" + EnterpriseCustomerID + " is not found");
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

                if (objColumns.Contains("EnterpriseCustomerID")) EnterpriseCustomerID = Convert.ToString(objRow["EnterpriseCustomerID"]);
                if (objColumns.Contains("ParentID")) ParentID = Convert.ToString(objRow["ParentID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("EBAInternalID")) EBAInternalID = Convert.ToString(objRow["EBAInternalID"]);
                if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
                if (objColumns.Contains("Email")) Email = Convert.ToString(objRow["Email"]);
                if (objColumns.Contains("StoreNumber")) StoreNumber = Convert.ToString(objRow["StoreNumber"]);
                if (objColumns.Contains("EmployeeID")) EmployeeID = Convert.ToString(objRow["EmployeeID"]);
                if (objColumns.Contains("CompanyName")) CompanyName = Convert.ToString(objRow["CompanyName"]);
                if (objColumns.Contains("FirstName")) FirstName = Convert.ToString(objRow["FirstName"]);
                if (objColumns.Contains("LastName")) LastName = Convert.ToString(objRow["LastName"]);
                if (objColumns.Contains("PhysicalEnterpriseAddressID")) PhysicalEnterpriseAddressID = Convert.ToString(objRow["PhysicalEnterpriseAddressID"]);
                if (objColumns.Contains("ShippingEnterpriseAddressID")) ShippingEnterpriseAddressID = Convert.ToString(objRow["ShippingEnterpriseAddressID"]);
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
                if (objColumns.Contains("TermDate") && objRow["TermDate"] != DBNull.Value) TermDate = Convert.ToDateTime(objRow["TermDate"]);
                if (objColumns.Contains("CustomerStatus")) CustomerStatus = Convert.ToString(objRow["CustomerStatus"]);
                if (objColumns.Contains("CustomerRegTemp")) CustomerRegTemp = Convert.ToString(objRow["CustomerRegTemp"]);
                if (objColumns.Contains("CustomerBrand")) CustomerBrand = Convert.ToString(objRow["CustomerBrand"]);
                if (objColumns.Contains("CustomerGPBR")) CustomerGPBR = Convert.ToString(objRow["CustomerGPBR"]);
                if (objColumns.Contains("NotificationEmail")) NotificationEmail = Convert.ToString(objRow["NotificationEmail"]);
                if (objColumns.Contains("Password")) Password = Convert.ToString(objRow["Password"]);
                if (objColumns.Contains("IsIndividual") && objRow["IsIndividual"] != DBNull.Value) IsIndividual = Convert.ToBoolean(objRow["IsIndividual"]);
                if (objColumns.Contains("IsAirport") && objRow["IsAirport"] != DBNull.Value) IsAirport = Convert.ToBoolean(objRow["IsAirport"]);
                if (objColumns.Contains("WorkdayID")) WorkdayID = Convert.ToString(objRow["WorkdayID"]);
                if (objColumns.Contains("BranchAdminLgcyID")) BranchAdminLgcyID = Convert.ToString(objRow["BranchAdminLgcyID"]);
                if (objColumns.Contains("BranchAdminPSID")) BranchAdminPSID = Convert.ToString(objRow["BranchAdminPSID"]);
                if (objColumns.Contains("PSGroup")) PSGroup = Convert.ToString(objRow["PSGroup"]);

                if (objColumns.Contains("IsRegionalized") && objRow["IsRegionalized"] != DBNull.Value) IsRegionalized = Convert.ToBoolean(objRow["IsRegionalized"]);
                if (objColumns.Contains("AirportCode") && objRow["AirportCode"] != DBNull.Value) AirportCode = Convert.ToString(objRow["AirportCode"]);


                if (objColumns.Contains("IsPreEmployee") && objRow["IsPreEmployee"] != DBNull.Value) IsPreEmployee = Convert.ToBoolean(objRow["IsPreEmployee"]);
                if (objColumns.Contains("BudgetEndDate") && objRow["BudgetEndDate"] != DBNull.Value) BudgetEndDate = Convert.ToDateTime(objRow["BudgetEndDate"]);
                if (objColumns.Contains("BudgetRefreshedOn") && objRow["BudgetRefreshedOn"] != DBNull.Value) BudgetRefreshedOn = Convert.ToDateTime(objRow["BudgetRefreshedOn"]);
                if (objColumns.Contains("IsUpdated") && objRow["IsUpdated"] != DBNull.Value) IsUpdated = Convert.ToBoolean(objRow["IsUpdated"]);
                if (objColumns.Contains("InActivePreEmployee") && objRow["InActivePreEmployee"] != DBNull.Value) InActivePreEmployee = Convert.ToBoolean(objRow["InActivePreEmployee"]);
                if (objColumns.Contains("InActive") && objRow["InActive"] != DBNull.Value) InActive = Convert.ToBoolean(objRow["InActive"]);
                if (objColumns.Contains("SyncID")) SyncID = Convert.ToString(objRow["SyncID"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(EnterpriseCustomerID)) throw new Exception("Missing EnterpriseCustomerID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, EnterpriseCustomerID already exists");

                dicParam["InternalID"] = InternalID; 
                dicParam["EBAInternalID"] = EBAInternalID;
                dicParam["ParentID"] = ParentID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["Email"] = Email;
                dicParam["StoreNumber"] = StoreNumber;
                dicParam["EmployeeID"] = EmployeeID;
                dicParam["CompanyName"] = CompanyName;
                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["PhysicalEnterpriseAddressID"] = PhysicalEnterpriseAddressID;
                dicParam["ShippingEnterpriseAddressID"] = ShippingEnterpriseAddressID;
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
                dicParam["TermDate"] = TermDate;
                dicParam["CustomerStatus"] = CustomerStatus;
                dicParam["CustomerRegTemp"] = CustomerRegTemp;
                dicParam["CustomerBrand"] = CustomerBrand;
                dicParam["CustomerGPBR"] = CustomerGPBR;
                dicParam["NotificationEmail"] = NotificationEmail;
                dicParam["IsIndividual"] = IsIndividual;
                dicParam["IsAirport"] = IsAirport;
                dicParam["WorkdayID"] = WorkdayID;
                dicParam["BranchAdminLgcyID"] = BranchAdminLgcyID;
                dicParam["BranchAdminPSID"] = BranchAdminPSID;
                dicParam["PSGroup"] = PSGroup;
                dicParam["IsRegionalized"] = IsRegionalized;
                dicParam["AirportCode"] = AirportCode;
                dicParam["IsPreEmployee"] = IsPreEmployee;
                dicParam["BudgetEndDate"] = BudgetEndDate;
                dicParam["BudgetRefreshedOn"] = BudgetRefreshedOn;
                dicParam["IsUpdated"] = IsUpdated;
                dicParam["InActivePreEmployee"] = InActivePreEmployee;
                dicParam["InActive"] = InActive;
                dicParam["SyncID"] = SyncID;
                dicParam["ErrorMessage"] = ErrorMessage;

                EnterpriseCustomerID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "EnterpriseCustomer"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, EnterpriseCustomerID is missing");

                dicParam["InternalID"] = InternalID;
                dicParam["EBAInternalID"] = EBAInternalID;
                dicParam["ParentID"] = ParentID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["Email"] = Email;
                dicParam["StoreNumber"] = StoreNumber;
                dicParam["EmployeeID"] = EmployeeID;
                dicParam["CompanyName"] = CompanyName;
                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["PhysicalEnterpriseAddressID"] = PhysicalEnterpriseAddressID;
                dicParam["ShippingEnterpriseAddressID"] = ShippingEnterpriseAddressID;
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
                dicParam["TermDate"] = TermDate;
                dicParam["CustomerStatus"] = CustomerStatus;
                dicParam["CustomerRegTemp"] = CustomerRegTemp;
                dicParam["CustomerBrand"] = CustomerBrand;
                dicParam["CustomerGPBR"] = CustomerGPBR;
                dicParam["NotificationEmail"] = NotificationEmail;
                dicParam["IsIndividual"] = IsIndividual;
                dicParam["IsAirport"] = IsAirport;
                dicParam["WorkdayID"] = WorkdayID;
                dicParam["BranchAdminLgcyID"] = BranchAdminLgcyID;
                dicParam["BranchAdminPSID"] = BranchAdminPSID;
                dicParam["PSGroup"] = PSGroup;
                dicParam["IsRegionalized"] = IsRegionalized;
                dicParam["AirportCode"] = AirportCode;
                dicParam["IsPreEmployee"] = IsPreEmployee;
                dicParam["BudgetEndDate"] = BudgetEndDate;
                dicParam["BudgetRefreshedOn"] = BudgetRefreshedOn;
                dicParam["IsUpdated"] = IsUpdated;
                dicParam["InActivePreEmployee"] = InActivePreEmployee;
                dicParam["InActive"] = InActive;
                dicParam["SyncID"] = SyncID;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = DateTime.UtcNow;

                dicWParam["EnterpriseCustomerID"] = EnterpriseCustomerID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "EnterpriseCustomer"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, EnterpriseCustomerID is missing");

                dicDParam["EnterpriseCustomerID"] = EnterpriseCustomerID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "EnterpriseCustomer"), objConn, objTran);
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

        public static EnterpriseCustomer GetEnterpriseCustomer(EnterpriseCustomerFilter Filter)
        {
            List<EnterpriseCustomer> objEnterpriseCustomers = null;
            EnterpriseCustomer objReturn = null;

            try
            {
                objEnterpriseCustomers = GetEnterpriseCustomers(Filter);
                if (objEnterpriseCustomers != null && objEnterpriseCustomers.Count >= 1) objReturn = objEnterpriseCustomers[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEnterpriseCustomers = null;
            }
            return objReturn;
        }

        public static List<EnterpriseCustomer> GetEnterpriseCustomers()
        {
            int intTotalCount = 0;
            return GetEnterpriseCustomers(null, null, null, out intTotalCount);
        }

        public static List<EnterpriseCustomer> GetEnterpriseCustomers(EnterpriseCustomerFilter Filter)
        {
            int intTotalCount = 0;
            return GetEnterpriseCustomers(Filter, null, null, out intTotalCount);
        }

        public static List<EnterpriseCustomer> GetEnterpriseCustomers(EnterpriseCustomerFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetEnterpriseCustomers(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<EnterpriseCustomer> GetEnterpriseCustomers(EnterpriseCustomerFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<EnterpriseCustomer> objReturn = null;
            EnterpriseCustomer objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<EnterpriseCustomer>();

                strSQL = "SELECT * " +
                         "FROM EnterpriseCustomer (NOLOCK) " +
                         "where 1=1 ";

                if (Filter != null)
                {
                    if (Filter.EmployeeID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EmployeeID, "EmployeeID");
                    if (Filter.StoreNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.StoreNumber, "StoreNumber");
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.ExternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ExternalID, "ExternalID");
                    if (Filter.ParentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ParentID, "ParentID");
                    if (Filter.Email != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Email, "Email");
                    if (Filter.SyncID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SyncID, "SyncID");
                    if (Filter.IsAirport != null) strSQL += "AND IsAirport=" + Database.HandleQuote(Convert.ToInt32(Filter.IsAirport.Value).ToString());
                    if (Filter.IsPreEmployee != null) strSQL += "AND IsPreEmployee=" + Database.HandleQuote(Convert.ToInt32(Filter.IsPreEmployee.Value).ToString());
                    if (Filter.IsIndividual != null) strSQL += "AND IsIndividual=" + Database.HandleQuote(Convert.ToInt32(Filter.IsIndividual.Value).ToString());
                    if (Filter.IsUpdated != null) strSQL += "AND IsUpdated=" + Database.HandleQuote(Convert.ToInt32(Filter.IsUpdated.Value).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "EnterpriseCustomerID" : Utility.CustomSorting.GetSortExpression(typeof(EnterpriseCustomer), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else
                {
                    strSQL += " ORDER BY EnterpriseCustomerID DESC";
                }

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new EnterpriseCustomer(objData.Tables[0].Rows[i]);
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

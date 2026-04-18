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
    public class CustomerEntityGroup : ISBase.BaseClass
    {
        public string CustomerEntityGroupID { get; private set; }
        public string CustomerID { get; set; }
        public string EntityGroupID { get; set; }
        public bool Inactive { get; set; }
        public bool IsUpdated { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(CustomerEntityGroupID); } }
        private Entity.EntityGroup mEntityGroup = null;
        public Entity.EntityGroup EntityGroup
        {
            get
            {
                if (mEntityGroup == null && !string.IsNullOrEmpty(EntityGroupID))
                {
                    mEntityGroup = new Entity.EntityGroup(EntityGroupID);
                }
                return mEntityGroup;
            }
            set
            {
                mEntityGroup = value;
            }
        }
        private Customer mCustomer = null;
        public Customer Customer
        {
            get
            {
                if (mCustomer == null && !string.IsNullOrEmpty(EntityGroupID))
                {
                    mCustomer = new Customer(CustomerID);
                }
                return mCustomer;
            }
            set
            {
                mCustomer = value;
            }
        }
        public CustomerEntityGroup()
        {
        }
        public CustomerEntityGroup(string CustomerEntityGroupID)
        {
            this.CustomerEntityGroupID = CustomerEntityGroupID;
            Load();
        }
        public CustomerEntityGroup(DataRow objRow)
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
                         "FROM CustomerEntityGroup (NOLOCK) " +
                         "WHERE CustomerEntityGroupID=" + Database.HandleQuote(CustomerEntityGroupID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("CustomerEntityGroupID=" + CustomerEntityGroupID + " is not found");
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

                if (objColumns.Contains("CustomerEntityGroupID")) CustomerEntityGroupID = Convert.ToString(objRow["CustomerEntityGroupID"]);
                if (objColumns.Contains("CustomerID")) CustomerID = Convert.ToString(objRow["CustomerID"]);
                if (objColumns.Contains("EntityGroupID")) EntityGroupID = Convert.ToString(objRow["EntityGroupID"]);
                if (objColumns.Contains("Inactive") && objRow["Inactive"] != DBNull.Value) Inactive = Convert.ToBoolean(objRow["Inactive"]);
                if (objColumns.Contains("IsUpdated") && objRow["IsUpdated"] != DBNull.Value) IsUpdated = Convert.ToBoolean(objRow["IsUpdated"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(CustomerEntityGroupID)) throw new Exception("Missing CustomerEntityGroupID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, CustomerEntityGroupID already exists");

                dicParam["CustomerID"] = CustomerID;
                dicParam["EntityGroupID"] = EntityGroupID;
                dicParam["Inactive"] = Inactive;
                dicParam["IsUpdated"] = IsUpdated;
                dicParam["ErrorMessage"] = ErrorMessage;

                CustomerEntityGroupID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "CustomerEntityGroup"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, CustomerEntityGroupID is missing");

                dicParam["CustomerID"] = CustomerID;
                dicParam["EntityGroupID"] = EntityGroupID;
                dicParam["Inactive"] = Inactive;
                dicParam["IsUpdated"] = IsUpdated;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = DateTime.UtcNow;

                dicWParam["CustomerEntityGroupID"] = CustomerEntityGroupID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "CustomerEntityGroup"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, CustomerEntityGroupID is missing");

                dicDParam["CustomerEntityGroupID"] = CustomerEntityGroupID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "CustomerEntityGroup"), objConn, objTran);
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

        public static CustomerEntityGroup GetCustomerEntityGroup(CustomerEntityGroupFilter Filter)
        {
            List<CustomerEntityGroup> objCustomerEntityGroups = null;
            CustomerEntityGroup objReturn = null;

            try
            {
                objCustomerEntityGroups = GetCustomerEntityGroups(Filter);
                if (objCustomerEntityGroups != null && objCustomerEntityGroups.Count >= 1) objReturn = objCustomerEntityGroups[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomerEntityGroups = null;
            }
            return objReturn;
        }

        public static List<CustomerEntityGroup> GetCustomerEntityGroups()
        {
            int intTotalCount = 0;
            return GetCustomerEntityGroups(null, null, null, out intTotalCount);
        }

        public static List<CustomerEntityGroup> GetCustomerEntityGroups(CustomerEntityGroupFilter Filter)
        {
            int intTotalCount = 0;
            return GetCustomerEntityGroups(Filter, null, null, out intTotalCount);
        }

        public static List<CustomerEntityGroup> GetCustomerEntityGroups(CustomerEntityGroupFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetCustomerEntityGroups(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<CustomerEntityGroup> GetCustomerEntityGroups(CustomerEntityGroupFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<CustomerEntityGroup> objReturn = null;
            CustomerEntityGroup objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<CustomerEntityGroup>();

                strSQL = "SELECT * " +
                         "FROM CustomerEntityGroup (NOLOCK) " +
                         "where 1=1 ";

                if (Filter != null)
                {
                    if (Filter.CustomerID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomerID, "CustomerID");
                    if (Filter.EntityGroupID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EntityGroupID, "EntityGroupID");
                    if (Filter.IsUpdated != null) strSQL += "AND IsUpdated=" + Database.HandleQuote(Convert.ToInt32(Filter.IsUpdated.Value).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "CustomerEntityGroupID" : Utility.CustomSorting.GetSortExpression(typeof(CustomerEntityGroup), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new CustomerEntityGroup(objData.Tables[0].Rows[i]);
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

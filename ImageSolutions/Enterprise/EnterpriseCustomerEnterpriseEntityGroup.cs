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
    public class EnterpriseCustomerEnterpriseEntityGroup : ISBase.BaseClass
    {
        public string EnterpriseCustomerEnterpriseEntityGroupID { get; private set; }
        public string EnterpriseCustomerID { get; set; }
        public string EnterpriseEntityGroupID { get; set; }
        public bool Inactive { get; set; }
        public bool IsUpdated { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(EnterpriseCustomerEnterpriseEntityGroupID); } }
        private EnterpriseEntityGroup mEnterpriseEntityGroup = null;
        public EnterpriseEntityGroup EnterpriseEntityGroup
        {
            get
            {
                if (mEnterpriseEntityGroup == null && !string.IsNullOrEmpty(EnterpriseEntityGroupID))
                {
                    mEnterpriseEntityGroup = new EnterpriseEntityGroup(EnterpriseEntityGroupID);
                }
                return mEnterpriseEntityGroup;
            }
            set
            {
                mEnterpriseEntityGroup = value;
            }
        }
        private EnterpriseCustomer mEnterpriseCustomer = null;
        public EnterpriseCustomer EnterpriseCustomer
        {
            get
            {
                if (mEnterpriseCustomer == null && !string.IsNullOrEmpty(EnterpriseEntityGroupID))
                {
                    mEnterpriseCustomer = new EnterpriseCustomer(EnterpriseCustomerID);
                }
                return mEnterpriseCustomer;
            }
            set
            {
                mEnterpriseCustomer = value;
            }
        }
        public EnterpriseCustomerEnterpriseEntityGroup()
        {
        }
        public EnterpriseCustomerEnterpriseEntityGroup(string EnterpriseCustomerEnterpriseEntityGroupID)
        {
            this.EnterpriseCustomerEnterpriseEntityGroupID = EnterpriseCustomerEnterpriseEntityGroupID;
            Load();
        }
        public EnterpriseCustomerEnterpriseEntityGroup(DataRow objRow)
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
                         "FROM EnterpriseCustomerEnterpriseEntityGroup (NOLOCK) " +
                         "WHERE EnterpriseCustomerEnterpriseEntityGroupID=" + Database.HandleQuote(EnterpriseCustomerEnterpriseEntityGroupID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("EnterpriseCustomerEnterpriseEntityGroupID=" + EnterpriseCustomerEnterpriseEntityGroupID + " is not found");
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

                if (objColumns.Contains("EnterpriseCustomerEnterpriseEntityGroupID")) EnterpriseCustomerEnterpriseEntityGroupID = Convert.ToString(objRow["EnterpriseCustomerEnterpriseEntityGroupID"]);
                if (objColumns.Contains("EnterpriseCustomerID")) EnterpriseCustomerID = Convert.ToString(objRow["EnterpriseCustomerID"]);
                if (objColumns.Contains("EnterpriseEntityGroupID")) EnterpriseEntityGroupID = Convert.ToString(objRow["EnterpriseEntityGroupID"]);
                if (objColumns.Contains("Inactive") && objRow["Inactive"] != DBNull.Value) Inactive = Convert.ToBoolean(objRow["Inactive"]);
                if (objColumns.Contains("IsUpdated") && objRow["IsUpdated"] != DBNull.Value) IsUpdated = Convert.ToBoolean(objRow["IsUpdated"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(EnterpriseCustomerEnterpriseEntityGroupID)) throw new Exception("Missing EnterpriseCustomerEnterpriseEntityGroupID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, EnterpriseCustomerEnterpriseEntityGroupID already exists");

                dicParam["EnterpriseCustomerID"] = EnterpriseCustomerID;
                dicParam["EnterpriseEntityGroupID"] = EnterpriseEntityGroupID;
                dicParam["Inactive"] = Inactive;
                dicParam["IsUpdated"] = IsUpdated;
                dicParam["ErrorMessage"] = ErrorMessage;

                EnterpriseCustomerEnterpriseEntityGroupID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "EnterpriseCustomerEnterpriseEntityGroup"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, EnterpriseCustomerEnterpriseEntityGroupID is missing");

                dicParam["EnterpriseCustomerID"] = EnterpriseCustomerID;
                dicParam["EnterpriseEntityGroupID"] = EnterpriseEntityGroupID;
                dicParam["Inactive"] = Inactive;
                dicParam["IsUpdated"] = IsUpdated;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = DateTime.UtcNow;

                dicWParam["EnterpriseCustomerEnterpriseEntityGroupID"] = EnterpriseCustomerEnterpriseEntityGroupID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "EnterpriseCustomerEnterpriseEntityGroup"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, EnterpriseCustomerEnterpriseEntityGroupID is missing");

                dicDParam["EnterpriseCustomerEnterpriseEntityGroupID"] = EnterpriseCustomerEnterpriseEntityGroupID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "EnterpriseCustomerEnterpriseEntityGroup"), objConn, objTran);
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

        public static EnterpriseCustomerEnterpriseEntityGroup GetEnterpriseCustomerEnterpriseEntityGroup(EnterpriseCustomerEnterpriseEntityGroupFilter Filter)
        {
            List<EnterpriseCustomerEnterpriseEntityGroup> objEnterpriseCustomerEnterpriseEntityGroups = null;
            EnterpriseCustomerEnterpriseEntityGroup objReturn = null;

            try
            {
                objEnterpriseCustomerEnterpriseEntityGroups = GetEnterpriseCustomerEnterpriseEntityGroups(Filter);
                if (objEnterpriseCustomerEnterpriseEntityGroups != null && objEnterpriseCustomerEnterpriseEntityGroups.Count >= 1) objReturn = objEnterpriseCustomerEnterpriseEntityGroups[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEnterpriseCustomerEnterpriseEntityGroups = null;
            }
            return objReturn;
        }

        public static List<EnterpriseCustomerEnterpriseEntityGroup> GetEnterpriseCustomerEnterpriseEntityGroups()
        {
            int intTotalCount = 0;
            return GetEnterpriseCustomerEnterpriseEntityGroups(null, null, null, out intTotalCount);
        }

        public static List<EnterpriseCustomerEnterpriseEntityGroup> GetEnterpriseCustomerEnterpriseEntityGroups(EnterpriseCustomerEnterpriseEntityGroupFilter Filter)
        {
            int intTotalCount = 0;
            return GetEnterpriseCustomerEnterpriseEntityGroups(Filter, null, null, out intTotalCount);
        }

        public static List<EnterpriseCustomerEnterpriseEntityGroup> GetEnterpriseCustomerEnterpriseEntityGroups(EnterpriseCustomerEnterpriseEntityGroupFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetEnterpriseCustomerEnterpriseEntityGroups(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<EnterpriseCustomerEnterpriseEntityGroup> GetEnterpriseCustomerEnterpriseEntityGroups(EnterpriseCustomerEnterpriseEntityGroupFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<EnterpriseCustomerEnterpriseEntityGroup> objReturn = null;
            EnterpriseCustomerEnterpriseEntityGroup objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<EnterpriseCustomerEnterpriseEntityGroup>();

                strSQL = "SELECT * " +
                         "FROM EnterpriseCustomerEnterpriseEntityGroup (NOLOCK) " +
                         "where 1=1 ";

                if (Filter != null)
                {
                    if (Filter.EnterpriseCustomerID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EnterpriseCustomerID, "EnterpriseCustomerID");
                    if (Filter.EnterpriseEntityGroupID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EnterpriseEntityGroupID, "EnterpriseEntityGroupID");
                    if (Filter.IsUpdated != null) strSQL += "AND IsUpdated=" + Database.HandleQuote(Convert.ToInt32(Filter.IsUpdated.Value).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "EnterpriseCustomerEnterpriseEntityGroupID" : Utility.CustomSorting.GetSortExpression(typeof(EnterpriseCustomerEnterpriseEntityGroup), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new EnterpriseCustomerEnterpriseEntityGroup(objData.Tables[0].Rows[i]);
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

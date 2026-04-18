using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Sprouts
{
    public class SproutsCustomer : ISBase.BaseClass
    {
        public string SproutsCustomerID { get; private set; }
        public string InternalID { get; set; }
        public string TeamMemberID { get; set; }
        public string CostCenterID { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string Status { get; set; }
        public bool EnableBudget { get; set; }
        public bool InActive { get; set; }
        public bool IsUpdated { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(SproutsCustomerID); } }

        public SproutsCustomer()
        {
        }
        public SproutsCustomer(string SproutsCustomerID)
        {
            this.SproutsCustomerID = SproutsCustomerID;
            Load();
        }
        public SproutsCustomer(DataRow objRow)
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
                         "FROM SproutsCustomer (NOLOCK) " +
                         "WHERE SproutsCustomerID=" + Database.HandleQuote(SproutsCustomerID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("SproutsCustomerID=" + SproutsCustomerID + " is not found");
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

                if (objColumns.Contains("SproutsCustomerID")) SproutsCustomerID = Convert.ToString(objRow["SproutsCustomerID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("TeamMemberID")) TeamMemberID = Convert.ToString(objRow["TeamMemberID"]);
                if (objColumns.Contains("CostCenterID")) CostCenterID = Convert.ToString(objRow["CostCenterID"]);
                if (objColumns.Contains("Status")) Status = Convert.ToString(objRow["Status"]);
                if (objColumns.Contains("HireDate") && objRow["HireDate"] != DBNull.Value) HireDate = Convert.ToDateTime(objRow["HireDate"]);
                if (objColumns.Contains("TerminationDate") && objRow["TerminationDate"] != DBNull.Value) TerminationDate = Convert.ToDateTime(objRow["TerminationDate"]);
                if (objColumns.Contains("Email")) Email = Convert.ToString(objRow["Email"]);
                if (objColumns.Contains("MobilePhone")) MobilePhone = Convert.ToString(objRow["MobilePhone"]);
                if (objColumns.Contains("EnableBudget") && objRow["EnableBudget"] != DBNull.Value) EnableBudget = Convert.ToBoolean(objRow["EnableBudget"]);
                if (objColumns.Contains("InActive") && objRow["InActive"] != DBNull.Value) InActive = Convert.ToBoolean(objRow["InActive"]);
                if (objColumns.Contains("IsUpdated") && objRow["IsUpdated"] != DBNull.Value) IsUpdated = Convert.ToBoolean(objRow["IsUpdated"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(SproutsCustomerID)) throw new Exception("Missing SproutsCustomerID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, SproutsCustomerID already exists");

                dicParam["InternalID"] = InternalID;
                dicParam["TeamMemberID"] = TeamMemberID;
                dicParam["CostCenterID"] = CostCenterID;
                dicParam["Status"] = Status;
                dicParam["HireDate"] = HireDate;
                dicParam["TerminationDate"] = TerminationDate;
                dicParam["Email"] = Email;
                dicParam["MobilePhone"] = MobilePhone;
                dicParam["EnableBudget"] = EnableBudget;
                dicParam["InActive"] = InActive;
                dicParam["IsUpdated"] = IsUpdated;
                dicParam["ErrorMessage"] = ErrorMessage;

                SproutsCustomerID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "SproutsCustomer"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, SproutsCustomerID is missing");

                dicParam["InternalID"] = InternalID;
                dicParam["TeamMemberID"] = TeamMemberID;
                dicParam["CostCenterID"] = CostCenterID;
                dicParam["Status"] = Status;
                dicParam["HireDate"] = HireDate;
                dicParam["TerminationDate"] = TerminationDate;
                dicParam["Email"] = Email;
                dicParam["MobilePhone"] = MobilePhone;
                dicParam["Status"] = Status;
                dicParam["EnableBudget"] = EnableBudget;
                dicParam["InActive"] = InActive;
                dicParam["IsUpdated"] = IsUpdated;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = DateTime.UtcNow;

                dicWParam["SproutsCustomerID"] = SproutsCustomerID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "SproutsCustomer"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, SproutsCustomerID is missing");

                dicDParam["SproutsCustomerID"] = SproutsCustomerID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "SproutsCustomer"), objConn, objTran);
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

        public static SproutsCustomer GetSproutsCustomer(SproutsCustomerFilter Filter)
        {
            List<SproutsCustomer> objSproutsCustomers = null;
            SproutsCustomer objReturn = null;

            try
            {
                objSproutsCustomers = GetSproutsCustomers(Filter);
                if (objSproutsCustomers != null && objSproutsCustomers.Count >= 1) objReturn = objSproutsCustomers[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSproutsCustomers = null;
            }
            return objReturn;
        }

        public static List<SproutsCustomer> GetSproutsCustomers()
        {
            int intTotalCount = 0;
            return GetSproutsCustomers(null, null, null, out intTotalCount);
        }

        public static List<SproutsCustomer> GetSproutsCustomers(SproutsCustomerFilter Filter)
        {
            int intTotalCount = 0;
            return GetSproutsCustomers(Filter, null, null, out intTotalCount);
        }

        public static List<SproutsCustomer> GetSproutsCustomers(SproutsCustomerFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetSproutsCustomers(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<SproutsCustomer> GetSproutsCustomers(SproutsCustomerFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<SproutsCustomer> objReturn = null;
            SproutsCustomer objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<SproutsCustomer>();

                strSQL = "SELECT * " +
                         "FROM SproutsCustomer (NOLOCK) " +
                         "where 1=1 ";

                if (Filter != null)
                {
                    if (Filter.TeamMemberID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.TeamMemberID, "TeamMemberID");
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.Email != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Email, "Email");
                    if (Filter.IsUpdated != null) strSQL += "AND IsUpdated=" + Database.HandleQuote(Convert.ToInt32(Filter.IsUpdated.Value).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "SproutsCustomerID" : Utility.CustomSorting.GetSortExpression(typeof(SproutsCustomer), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else
                {
                    strSQL += " ORDER BY SproutsCustomerID DESC";
                }

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new SproutsCustomer(objData.Tables[0].Rows[i]);
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

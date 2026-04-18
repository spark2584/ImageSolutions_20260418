using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.SSO
{
    public class SSOLog : ISBase.BaseClass
    {
        public string SSOLogID { get; private set; }
        public string AuthenticateResult { get; set; }
        public string Email { get; set; }
        public string NameIdentifier { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedOn { get; set; }

        public SSOLog()
        {
        }
        public SSOLog(string SSOLogID)
        {
            this.SSOLogID = SSOLogID;
            Load();
        }
        public SSOLog(DataRow objRow)
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
                         "FROM SSOLog (NOLOCK) " +
                         "WHERE SSOLogID=" + Database.HandleQuote(SSOLogID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("SSOLogID=" + SSOLogID + " is not found");
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

                if (objColumns.Contains("SSOLogID")) SSOLogID = Convert.ToString(objRow["SSOLogID"]);
                if (objColumns.Contains("AuthenticateResult")) AuthenticateResult = Convert.ToString(objRow["AuthenticateResult"]);
                if (objColumns.Contains("Email")) Email = Convert.ToString(objRow["Email"]);
                if (objColumns.Contains("NameIdentifier")) NameIdentifier = Convert.ToString(objRow["NameIdentifier"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);

                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(SSOLogID)) throw new Exception("Missing SSOLogID in the datarow");
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

                dicParam["AuthenticateResult"] = AuthenticateResult;
                dicParam["Email"] = Email;
                dicParam["NameIdentifier"] = NameIdentifier;
                dicParam["ErrorMessage"] = ErrorMessage;

                SSOLogID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "SSOLog"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(SSOLogID)) throw new Exception("SSOLogID is required");

                dicParam["AuthenticateResult"] = AuthenticateResult;
                dicParam["Email"] = Email;
                dicParam["NameIdentifier"] = NameIdentifier;
                dicParam["ErrorMessage"] = ErrorMessage;

                dicWParam["SSOLogID"] = SSOLogID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "SSOLog"), objConn, objTran);

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
                dicDParam["SSOLogID"] = SSOLogID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "SSOLog"), objConn, objTran);
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

        public static SSOLog GetSSOLog(SSOLogFilter Filter)
        {
            List<SSOLog> objTaskEntries = null;
            SSOLog objReturn = null;

            try
            {
                objTaskEntries = GetSSOLogs(Filter);
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

        public static List<SSOLog> GetSSOLogs()
        {
            int intTotalCount = 0;
            return GetSSOLogs(null, null, null, out intTotalCount);
        }

        public static List<SSOLog> GetSSOLogs(SSOLogFilter Filter)
        {
            int intTotalCount = 0;
            return GetSSOLogs(Filter, null, null, out intTotalCount);
        }

        public static List<SSOLog> GetSSOLogs(SSOLogFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetSSOLogs(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<SSOLog> GetSSOLogs(SSOLogFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<SSOLog> objReturn = null;
            SSOLog objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<SSOLog>();

                strSQL = "SELECT * " +
                         "FROM SSOLog (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.Email != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Email, "Email");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "SSOLogID" : Utility.CustomSorting.GetSortExpression(typeof(SSOLog), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new SSOLog(objData.Tables[0].Rows[i]);
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

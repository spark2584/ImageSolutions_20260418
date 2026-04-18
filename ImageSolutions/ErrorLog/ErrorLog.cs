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
namespace ImageSolutions.ErrorLog
{
    public class ErrorLog : ISBase.BaseClass
    {
        public string ErrorLogID { get; private set; }
        public string UserInfoID { get; set; }
        public string UserWebsiteID { get; set; }
        public string Path { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedOn { get; set; }

        public ErrorLog()
        {
        }
        public ErrorLog(string ErrorLogID)
        {
            this.ErrorLogID = ErrorLogID;
            Load();
        }
        public ErrorLog(DataRow objRow)
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
                         "FROM ErrorLog (NOLOCK) " +
                         "WHERE ErrorLogID=" + Database.HandleQuote(ErrorLogID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ErrorLogID=" + ErrorLogID + " is not found");
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

                if (objColumns.Contains("ErrorLogID")) ErrorLogID = Convert.ToString(objRow["ErrorLogID"]);
                if (objColumns.Contains("UserInfoID")) UserInfoID = Convert.ToString(objRow["UserInfoID"]);
                if (objColumns.Contains("UserWebsiteID")) UserWebsiteID = Convert.ToString(objRow["UserWebsiteID"]);
                if (objColumns.Contains("Path")) Path = Convert.ToString(objRow["Path"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ErrorLogID)) throw new Exception("Missing ErrorLogID in the datarow");
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
                dicParam["UserInfoID"] = UserInfoID;
                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["Path"] = Path;
                dicParam["ErrorMessage"] = ErrorMessage;

                ErrorLogID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ErrorLog"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(ErrorLogID)) throw new Exception("ErrorLogID is required");

                dicParam["UserInfoID"] = UserInfoID;
                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["Path"] = Path;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicWParam["ErrorLogID"] = ErrorLogID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ErrorLog"), objConn, objTran);

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
                dicDParam["ErrorLogID"] = ErrorLogID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ErrorLog"), objConn, objTran);
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

        public static ErrorLog GetErrorLog(ErrorLogFilter Filter)
        {
            List<ErrorLog> objTaskEntries = null;
            ErrorLog objReturn = null;

            try
            {
                objTaskEntries = GetErrorLogs(Filter);
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

        public static List<ErrorLog> GetErrorLogs()
        {
            int intTotalCount = 0;
            return GetErrorLogs(null, null, null, out intTotalCount);
        }

        public static List<ErrorLog> GetErrorLogs(ErrorLogFilter Filter)
        {
            int intTotalCount = 0;
            return GetErrorLogs(Filter, null, null, out intTotalCount);
        }

        public static List<ErrorLog> GetErrorLogs(ErrorLogFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetErrorLogs(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ErrorLog> GetErrorLogs(ErrorLogFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ErrorLog> objReturn = null;
            ErrorLog objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ErrorLog>();

                strSQL = "SELECT * " +
                         "FROM ErrorLog (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.UserInfoID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserInfoID, "UserInfoID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ErrorLogID" : Utility.CustomSorting.GetSortExpression(typeof(ErrorLog), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ErrorLog(objData.Tables[0].Rows[i]);
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

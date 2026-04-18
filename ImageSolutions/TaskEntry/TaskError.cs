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

namespace ImageSolutions.TaskEntry
{
    public class TaskError : ISBase.BaseClass
    {
        public string TaskErrorID { get; private set; }
        public string TaskEntryID { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedOn { get; set; }

        public TaskError()
        {
        }
        public TaskError(string TaskErrorID)
        {
            this.TaskErrorID = TaskErrorID;
            Load();
        }
        public TaskError(DataRow objRow)
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
                         "FROM TaskError (NOLOCK) " +
                         "WHERE TaskErrorID=" + Database.HandleQuote(TaskErrorID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("TaskErrorID=" + TaskErrorID + " is not found");
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

                if (objColumns.Contains("TaskErrorID")) TaskErrorID = Convert.ToString(objRow["TaskErrorID"]);
                if (objColumns.Contains("TaskEntryID")) TaskEntryID = Convert.ToString(objRow["TaskEntryID"]);
             
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(TaskErrorID)) throw new Exception("Missing TaskErrorID in the datarow");
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
                if (string.IsNullOrEmpty(TaskEntryID)) throw new Exception("TaskEntryID is required");
                if (string.IsNullOrEmpty(ErrorMessage)) throw new Exception("ErrorMessage is required");

                dicParam["TaskEntryID"] = TaskEntryID;
                dicParam["ErrorMessage"] = ErrorMessage;

                TaskErrorID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "TaskError"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(TaskErrorID)) throw new Exception("TaskErrorID is required");
                if (string.IsNullOrEmpty(TaskEntryID)) throw new Exception("TaskEntryID is required");

                dicParam["TaskEntryID"] = TaskEntryID;
                dicParam["ErrorMessage"] = ErrorMessage;

                dicWParam["TaskErrorID"] = TaskErrorID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "TaskError"), objConn, objTran);

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
                dicDParam["TaskErrorID"] = TaskErrorID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "TaskError"), objConn, objTran);
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

        public static TaskError GetTaskError(TaskErrorFilter Filter)
        {
            List<TaskError> objTaskErrors = null;
            TaskError objReturn = null;

            try
            {
                objTaskErrors = GetTaskErrors(Filter);
                if (objTaskErrors != null && objTaskErrors.Count >= 1) objReturn = objTaskErrors[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objTaskErrors = null;
            }
            return objReturn;
        }

        public static List<TaskError> GetTaskErrors()
        {
            int intTotalCount = 0;
            return GetTaskErrors(null, null, null, out intTotalCount);
        }

        public static List<TaskError> GetTaskErrors(TaskErrorFilter Filter)
        {
            int intTotalCount = 0;
            return GetTaskErrors(Filter, null, null, out intTotalCount);
        }

        public static List<TaskError> GetTaskErrors(TaskErrorFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetTaskErrors(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<TaskError> GetTaskErrors(TaskErrorFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<TaskError> objReturn = null;
            TaskError objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<TaskError>();

                strSQL = "SELECT * " +
                         "FROM TaskError (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.TaskErrorID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.TaskErrorID, "TaskErrorID");
                    if (Filter.TaskID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.TaskID, "TaskID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "TaskErrorID" : Utility.CustomSorting.GetSortExpression(typeof(TaskError), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new TaskError(objData.Tables[0].Rows[i]);
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

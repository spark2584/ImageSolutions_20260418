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

namespace ImageSolutions.Task
{
    public class Task : ISBase.BaseClass
    {
        public string TaskID { get; private set; }
        public string TaskName{ get; set; }
        public DateTime? LastExecutedOn { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedOn { get; set; }
               
        public Task()
        {
        }
        public Task(string TaskID)
        {
            this.TaskID = TaskID;
            Load();
        }
        public Task(DataRow objRow)
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
                         "FROM Task (NOLOCK) " +
                         "WHERE TaskID=" + Database.HandleQuote(TaskID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("TaskID=" + TaskID + " is not found");
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

                if (objColumns.Contains("TaskID")) TaskID = Convert.ToString(objRow["TaskID"]);
                if (objColumns.Contains("TaskName")) TaskName = Convert.ToString(objRow["TaskName"]);
                if (objColumns.Contains("LastExecutedOn") && objRow["LastExecutedOn"] != DBNull.Value) LastExecutedOn = Convert.ToDateTime(objRow["LastExecutedOn"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(TaskID)) throw new Exception("Missing TaskID in the datarow");
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
                //if (string.IsNullOrEmpty(IncrementID)) throw new Exception("IncrementID is required");

                dicParam["TaskName"] = TaskName;
                dicParam["LastExecutedOn"] = LastExecutedOn;
                dicParam["ErrorMessage"] = ErrorMessage;

                TaskID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Task"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(TaskID)) throw new Exception("TaskID is required");
                //if (string.IsNullOrEmpty(IncrementID)) throw new Exception("IncrementID is required");

                dicParam["TaskName"] = TaskName;
                dicParam["LastExecutedOn"] = LastExecutedOn;
                dicParam["ErrorMessage"] = ErrorMessage;

                dicWParam["TaskID"] = TaskID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Task"), objConn, objTran);

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
                dicDParam["TaskID"] = TaskID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Task"), objConn, objTran);
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

        public static Task GetTask(TaskFilter Filter)
        {
            List<Task> objTaskEntries = null;
            Task objReturn = null;

            try
            {
                objTaskEntries = GetTaskEntries(Filter);
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

        public static List<Task> GetTaskEntries()
        {
            int intTotalCount = 0;
            return GetTaskEntries(null, null, null, out intTotalCount);
        }

        public static List<Task> GetTaskEntries(TaskFilter Filter)
        {
            int intTotalCount = 0;
            return GetTaskEntries(Filter, null, null, out intTotalCount);
        }

        public static List<Task> GetTaskEntries(TaskFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetTaskEntries(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Task> GetTaskEntries(TaskFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Task> objReturn = null;
            Task objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Task>();

                strSQL = "SELECT * " +
                         "FROM Task (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.TaskID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.TaskID, "TaskID");
                    if (Filter.TaskName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.TaskName, "TaskName");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "TaskID" : Utility.CustomSorting.GetSortExpression(typeof(Task), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Task(objData.Tables[0].Rows[i]);
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

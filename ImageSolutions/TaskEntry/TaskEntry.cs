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
    public class TaskEntry : ISBase.BaseClass
    {
        public string TaskEntryID { get; private set; }
        public string TaskID { get; set; }
        public string PrimaryKey { get; set; }
        public string Parameter { get; set; }
        public string InternalID { get; set; }
        public string ExternalID { get; set; }
        public string Status { get; set; }
        public DateTime? ScheduledRunDate { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        private List<TaskError> mTaskErrors = null;
        public List<TaskError> TaskErrors
        {
            get
            {
                if (mTaskErrors == null && !string.IsNullOrEmpty(TaskEntryID))
                {
                    TaskErrorFilter objFilter = null;

                    try
                    {
                        //objFilter = new TaskErrorFilter();
                        //objFilter.TaskEntryID = TaskEntryID;

                        objFilter = new TaskErrorFilter();
                        objFilter.TaskEntryID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.TaskEntryID.SearchString = TaskEntryID;

                        mTaskErrors = TaskError.GetTaskErrors(objFilter);
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
                return mTaskErrors;
            }
            set
            {
                mTaskErrors = value;
            }
        }

        public TaskEntry()
        {
        }
        public TaskEntry(string TaskEntryID)
        {
            this.TaskEntryID = TaskEntryID;
            Load();
        }
        public TaskEntry(DataRow objRow)
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
                         "FROM TaskEntry (NOLOCK) " +
                         "WHERE TaskEntryID=" + Database.HandleQuote(TaskEntryID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("TaskEntryID=" + TaskEntryID + " is not found");
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

                if (objColumns.Contains("TaskEntryID")) TaskEntryID = Convert.ToString(objRow["TaskEntryID"]);
                if (objColumns.Contains("TaskID")) TaskID = Convert.ToString(objRow["TaskID"]);
                if (objColumns.Contains("PrimaryKey")) PrimaryKey = Convert.ToString(objRow["PrimaryKey"]);
                if (objColumns.Contains("Parameter")) Parameter = Convert.ToString(objRow["Parameter"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
                if (objColumns.Contains("Status")) Status = Convert.ToString(objRow["Status"]);
                if (objColumns.Contains("ScheduledRunDate") && objRow["ScheduledRunDate"] != DBNull.Value) ScheduledRunDate = Convert.ToDateTime(objRow["ScheduledRunDate"]);
                if (objColumns.Contains("ProcessedOn") && objRow["ProcessedOn"] != DBNull.Value) ProcessedOn = Convert.ToDateTime(objRow["ProcessedOn"]);

                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(TaskEntryID)) throw new Exception("Missing TaskEntryID in the datarow");
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

                dicParam["TaskID"] = TaskID;
                dicParam["PrimaryKey"] = PrimaryKey;
                dicParam["Parameter"] = Parameter;
                dicParam["InternalID"] = InternalID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["Status"] = Status;
                dicParam["ScheduledRunDate"] = ScheduledRunDate;
                dicParam["ProcessedOn"] = ProcessedOn;

                TaskEntryID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "TaskEntry"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(TaskEntryID)) throw new Exception("TaskEntryID is required");
                //if (string.IsNullOrEmpty(IncrementID)) throw new Exception("IncrementID is required");

                dicParam["TaskID"] = TaskID;
                dicParam["PrimaryKey"] = PrimaryKey;
                dicParam["Parameter"] = Parameter;
                dicParam["InternalID"] = InternalID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["Status"] = Status;
                dicParam["ScheduledRunDate"] = ScheduledRunDate;
                dicParam["ProcessedOn"] = ProcessedOn;

                dicWParam["TaskEntryID"] = TaskEntryID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "TaskEntry"), objConn, objTran);

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
                dicDParam["TaskEntryID"] = TaskEntryID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "TaskEntry"), objConn, objTran);
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

        public static TaskEntry GetTaskEntry(TaskEntryFilter Filter)
        {
            List<TaskEntry> objTaskEntries = null;
            TaskEntry objReturn = null;

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

        public static List<TaskEntry> GetTaskEntries()
        {
            int intTotalCount = 0;
            return GetTaskEntries(null, null, null, out intTotalCount);
        }

        public static List<TaskEntry> GetTaskEntries(TaskEntryFilter Filter)
        {
            int intTotalCount = 0;
            return GetTaskEntries(Filter, null, null, out intTotalCount);
        }

        public static List<TaskEntry> GetTaskEntries(TaskEntryFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetTaskEntries(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<TaskEntry> GetTaskEntries(TaskEntryFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<TaskEntry> objReturn = null;
            TaskEntry objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<TaskEntry>();

                strSQL = "SELECT * " +
                         "FROM TaskEntry (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.TaskEntryID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.TaskEntryID, "TaskEntryID");
                    if (Filter.TaskID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.TaskID, "TaskID");
                    if (Filter.ExternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ExternalID, "ExternalID");
                    if (Filter.Status != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Status, "Status");
                    if (Filter.ScheduleRunDateDelay) strSQL += " and (ScheduledRunDate is null or ScheduledRunDate <= GETUTCDATE())";
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "TaskEntryID" : Utility.CustomSorting.GetSortExpression(typeof(TaskEntry), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new TaskEntry(objData.Tables[0].Rows[i]);
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

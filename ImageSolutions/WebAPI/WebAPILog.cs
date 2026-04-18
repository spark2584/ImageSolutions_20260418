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


namespace ImageSolutions.WebAPI
{
    public class WebAPILog : ISBase.BaseClass
    {
        public string WebAPILogID { get; private set; }
        public string RequestUri { get; set; }
        public string Method { get; set; }
        public string Route { get; set; }
        public string Body { get; set; }
        public string Response { get; set; }
        public string OrderID { get; set; }
        public string OrderNumber { get; set; }
        public string InternalID { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedOn { get; set; }

        public WebAPILog()
        {
        }
        public WebAPILog(string WebAPILogID)
        {
            this.WebAPILogID = WebAPILogID;
            Load();
        }
        public WebAPILog(DataRow objRow)
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
                         "FROM WebAPILog (NOLOCK) " +
                         "WHERE WebAPILogID=" + Database.HandleQuote(WebAPILogID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("WebAPILogID=" + WebAPILogID + " is not found");
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

                if (objColumns.Contains("WebAPILogID")) WebAPILogID = Convert.ToString(objRow["WebAPILogID"]);
                if (objColumns.Contains("RequestUri")) RequestUri = Convert.ToString(objRow["RequestUri"]);
                if (objColumns.Contains("Method")) Method = Convert.ToString(objRow["Method"]);
                if (objColumns.Contains("Route")) Route = Convert.ToString(objRow["Route"]);
                if (objColumns.Contains("Body")) Body = Convert.ToString(objRow["Body"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("Response")) Response = Convert.ToString(objRow["Response"]);

                if (objColumns.Contains("OrderID")) OrderID = Convert.ToString(objRow["OrderID"]);
                if (objColumns.Contains("OrderNumber")) OrderNumber = Convert.ToString(objRow["OrderNumber"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);

                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(WebAPILogID)) throw new Exception("Missing WebAPILogID in the datarow");
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
                dicParam["RequestUri"] = RequestUri;
                dicParam["Method"] = Method;
                dicParam["Route"] = Route;
                dicParam["Body"] = Body;
                dicParam["Response"] = Response;
                dicParam["OrderID"] = OrderID;
                dicParam["OrderNumber"] = OrderNumber;
                dicParam["InternalID"] = InternalID;

                dicParam["ErrorMessage"] = ErrorMessage;

                WebAPILogID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "WebAPILog"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(WebAPILogID)) throw new Exception("WebAPILogID is required");

                dicParam["RequestUri"] = RequestUri;
                dicParam["Method"] = Method;
                dicParam["Route"] = Route;
                dicParam["Body"] = Body;
                dicParam["Response"] = Response;
                dicParam["OrderID"] = OrderID;
                dicParam["OrderNumber"] = OrderNumber;
                dicParam["InternalID"] = InternalID;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicWParam["WebAPILogID"] = WebAPILogID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "WebAPILog"), objConn, objTran);

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
                dicDParam["WebAPILogID"] = WebAPILogID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "WebAPILog"), objConn, objTran);
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

        public static WebAPILog GetWebAPILog(WebAPILogFilter Filter)
        {
            List<WebAPILog> objTaskEntries = null;
            WebAPILog objReturn = null;

            try
            {
                objTaskEntries = GetWebAPILogs(Filter);
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

        public static List<WebAPILog> GetWebAPILogs()
        {
            int intTotalCount = 0;
            return GetWebAPILogs(null, null, null, out intTotalCount);
        }

        public static List<WebAPILog> GetWebAPILogs(WebAPILogFilter Filter)
        {
            int intTotalCount = 0;
            return GetWebAPILogs(Filter, null, null, out intTotalCount);
        }

        public static List<WebAPILog> GetWebAPILogs(WebAPILogFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetWebAPILogs(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<WebAPILog> GetWebAPILogs(WebAPILogFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<WebAPILog> objReturn = null;
            WebAPILog objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<WebAPILog>();

                strSQL = "SELECT * " +
                         "FROM WebAPILog (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.Method != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Method, "Method");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "WebAPILogID" : Utility.CustomSorting.GetSortExpression(typeof(WebAPILog), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new WebAPILog(objData.Tables[0].Rows[i]);
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

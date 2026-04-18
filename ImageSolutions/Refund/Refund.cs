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

namespace ImageSolutions.Refund
{
    public class Refund : ISBase.BaseClass
    {
        public string RefundID { get; private set; }
        public string InternalID { get; set; }
        public string ExternalID { get; set; }
        public string SalesOrderID { get; set; }
        public string TransactionNumber { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public string Response { get; set; }
        public DateTime CreatedOn { get; set; }

        public Refund()
        {
        }
        public Refund(string RefundID)
        {
            this.RefundID = RefundID;
            Load();
        }
        public Refund(DataRow objRow)
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
                         "FROM Refund (NOLOCK) " +
                         "WHERE RefundID=" + Database.HandleQuote(RefundID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("RefundID=" + RefundID + " is not found");
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

                if (objColumns.Contains("RefundID")) RefundID = Convert.ToString(objRow["RefundID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
                if (objColumns.Contains("SalesOrderID")) SalesOrderID = Convert.ToString(objRow["SalesOrderID"]);
                if (objColumns.Contains("TransactionNumber")) TransactionNumber = Convert.ToString(objRow["TransactionNumber"]);
                if (objColumns.Contains("Amount")) Amount = Convert.ToDecimal(objRow["Amount"]);
                if (objColumns.Contains("Status")) Status = Convert.ToString(objRow["Status"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("Response")) Response = Convert.ToString(objRow["Response"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(RefundID)) throw new Exception("Missing RefundID in the datarow");
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
                dicParam["InternalID"] = InternalID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["SalesOrderID"] = SalesOrderID;
                dicParam["TransactionNumber"] = TransactionNumber;
                dicParam["Amount"] = Amount;
                dicParam["Status"] = Status;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["Response"] = Response;

                RefundID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Refund"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(RefundID)) throw new Exception("RefundID is required");

                dicParam["InternalID"] = InternalID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["SalesOrderID"] = SalesOrderID;
                dicParam["TransactionNumber"] = TransactionNumber;
                dicParam["Amount"] = Amount;
                dicParam["Status"] = Status;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["Response"] = Response;
                dicWParam["RefundID"] = RefundID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Refund"), objConn, objTran);

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
                dicDParam["RefundID"] = RefundID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Refund"), objConn, objTran);
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

        public static Refund GetRefund(RefundFilter Filter)
        {
            List<Refund> objTaskEntries = null;
            Refund objReturn = null;

            try
            {
                objTaskEntries = GetRefunds(Filter);
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

        public static List<Refund> GetRefunds()
        {
            int intTotalCount = 0;
            return GetRefunds(null, null, null, out intTotalCount);
        }

        public static List<Refund> GetRefunds(RefundFilter Filter)
        {
            int intTotalCount = 0;
            return GetRefunds(Filter, null, null, out intTotalCount);
        }

        public static List<Refund> GetRefunds(RefundFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetRefunds(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Refund> GetRefunds(RefundFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Refund> objReturn = null;
            Refund objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Refund>();

                strSQL = "SELECT * " +
                         "FROM Refund (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.SalesOrderID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderID, "SalesOrderID");
                    if (Filter.Status != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Status, "Status");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "RefundID" : Utility.CustomSorting.GetSortExpression(typeof(Refund), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Refund(objData.Tables[0].Rows[i]);
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

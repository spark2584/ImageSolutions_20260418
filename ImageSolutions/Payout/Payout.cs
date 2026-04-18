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

namespace ImageSolutions.Payout
{
    public class Payout : ISBase.BaseClass
    {
        public string PayoutID { get; private set; }
        public string InternalID { get; set; }
        public string ExternalID { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime ArrivalDate { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedOn { get; set; }

        public Payout()
        {
        }
        public Payout(string PayoutID)
        {
            this.PayoutID = PayoutID;
            Load();
        }
        public Payout(DataRow objRow)
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
                         "FROM Payout (NOLOCK) " +
                         "WHERE PayoutID=" + Database.HandleQuote(PayoutID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("PayoutID=" + PayoutID + " is not found");
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

                if (objColumns.Contains("PayoutID")) PayoutID = Convert.ToString(objRow["PayoutID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
                if (objColumns.Contains("Amount")) Amount = Convert.ToDecimal(objRow["Amount"]);
                if (objColumns.Contains("Description")) Description = Convert.ToString(objRow["Description"]);
                if (objColumns.Contains("Status")) Status = Convert.ToString(objRow["Status"]);
                if (objColumns.Contains("ArrivalDate") && objRow["ArrivalDate"] != DBNull.Value) ArrivalDate = Convert.ToDateTime(objRow["ArrivalDate"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(PayoutID)) throw new Exception("Missing PayoutID in the datarow");
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
                dicParam["Amount"] = Amount;
                dicParam["Description"] = Description;
                dicParam["Status"] = Status;
                dicParam["ArrivalDate"] = ArrivalDate;
                dicParam["ErrorMessage"] = ErrorMessage;

                PayoutID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Payout"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(PayoutID)) throw new Exception("PayoutID is required");

                dicParam["InternalID"] = InternalID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["Amount"] = Amount;
                dicParam["Description"] = Description;
                dicParam["Status"] = Status;
                dicParam["ArrivalDate"] = ArrivalDate;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicWParam["PayoutID"] = PayoutID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Payout"), objConn, objTran);

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
                dicDParam["PayoutID"] = PayoutID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Payout"), objConn, objTran);
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

        public static Payout GetPayout(PayoutFilter Filter)
        {
            List<Payout> objTaskEntries = null;
            Payout objReturn = null;

            try
            {
                objTaskEntries = GetPayouts(Filter);
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

        public static List<Payout> GetPayouts()
        {
            int intTotalCount = 0;
            return GetPayouts(null, null, null, out intTotalCount);
        }

        public static List<Payout> GetPayouts(PayoutFilter Filter)
        {
            int intTotalCount = 0;
            return GetPayouts(Filter, null, null, out intTotalCount);
        }

        public static List<Payout> GetPayouts(PayoutFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetPayouts(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Payout> GetPayouts(PayoutFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Payout> objReturn = null;
            Payout objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Payout>();

                strSQL = "SELECT * " +
                         "FROM Payout (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.ExternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ExternalID, "ExternalID");
                    if (Filter.Status != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Status, "Status");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "PayoutID" : Utility.CustomSorting.GetSortExpression(typeof(Payout), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Payout(objData.Tables[0].Rows[i]);
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

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
    public class PayoutLine : ISBase.BaseClass
    {
        public string PayoutLineID { get; private set; }
        public string PayoutID { get; set; }
        public string InternalID { get; set; }
        public string ExternalID { get; set; }
        public string SourceID { get; set; }
        public string IntentID { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime? TransactionDate { get; set; }
        public bool Inactive { get; set; }
        public DateTime CreatedOn { get; set; }

        public PayoutLine()
        {
        }
        public PayoutLine(string PayoutLineID)
        {
            this.PayoutLineID = PayoutLineID;
            Load();
        }
        public PayoutLine(DataRow objRow)
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
                         "FROM PayoutLine (NOLOCK) " +
                         "WHERE PayoutLineID=" + Database.HandleQuote(PayoutLineID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("PayoutLineID=" + PayoutLineID + " is not found");
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

                if (objColumns.Contains("PayoutLineID")) PayoutLineID = Convert.ToString(objRow["PayoutLineID"]);
                if (objColumns.Contains("PayoutID")) PayoutID = Convert.ToString(objRow["PayoutID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
                if (objColumns.Contains("SourceID")) SourceID = Convert.ToString(objRow["SourceID"]);
                if (objColumns.Contains("IntentID")) IntentID = Convert.ToString(objRow["IntentID"]);
                if (objColumns.Contains("Amount")) Amount = Convert.ToDecimal(objRow["Amount"]);
                if (objColumns.Contains("Type")) Type = Convert.ToString(objRow["Type"]);
                if (objColumns.Contains("Description")) Description = Convert.ToString(objRow["Description"]);
                if (objColumns.Contains("TransactionDate") && objRow["TransactionDate"] != DBNull.Value) TransactionDate = Convert.ToDateTime(objRow["TransactionDate"]);
                if (objColumns.Contains("Inactive")) Inactive = Convert.ToBoolean(objRow["Inactive"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(PayoutLineID)) throw new Exception("Missing PayoutLineID in the datarow");
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
                dicParam["PayoutID"] = PayoutID;
                dicParam["InternalID"] = InternalID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["SourceID"] = SourceID;
                dicParam["IntentID"] = IntentID;
                dicParam["Amount"] = Amount;
                dicParam["Type"] = Type;
                dicParam["Description"] = Description;
                dicParam["TransactionDate"] = TransactionDate;

                PayoutLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "PayoutLine"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(PayoutLineID)) throw new Exception("PayoutLineID is required");

                dicParam["PayoutID"] = PayoutID;
                dicParam["InternalID"] = InternalID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["SourceID"] = SourceID;
                dicParam["IntentID"] = IntentID;
                dicParam["Amount"] = Amount;
                dicParam["Type"] = Type;
                dicParam["Description"] = Description;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["Inactive"] = Inactive;
                dicWParam["PayoutLineID"] = PayoutLineID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "PayoutLine"), objConn, objTran);

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
                dicDParam["PayoutLineID"] = PayoutLineID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "PayoutLine"), objConn, objTran);
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

        public static PayoutLine GetPayoutLine(PayoutLineFilter Filter)
        {
            List<PayoutLine> objTaskEntries = null;
            PayoutLine objReturn = null;

            try
            {
                objTaskEntries = GetPayoutLines(Filter);
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

        public static List<PayoutLine> GetPayoutLines()
        {
            int intTotalCount = 0;
            return GetPayoutLines(null, null, null, out intTotalCount);
        }

        public static List<PayoutLine> GetPayoutLines(PayoutLineFilter Filter)
        {
            int intTotalCount = 0;
            return GetPayoutLines(Filter, null, null, out intTotalCount);
        }

        public static List<PayoutLine> GetPayoutLines(PayoutLineFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetPayoutLines(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<PayoutLine> GetPayoutLines(PayoutLineFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<PayoutLine> objReturn = null;
            PayoutLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<PayoutLine>();

                strSQL = "SELECT * " +
                         "FROM PayoutLine (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.PayoutID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PayoutID, "PayoutID");
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "PayoutLineID" : Utility.CustomSorting.GetSortExpression(typeof(PayoutLine), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new PayoutLine(objData.Tables[0].Rows[i]);
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

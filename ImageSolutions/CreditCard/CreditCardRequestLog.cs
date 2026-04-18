using ImageSolutions.Account;
using ImageSolutions.Item;
using ImageSolutions.User;
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

namespace ImageSolutions.CreditCard
{
    public class CreditCardRequestLog : ISBase.BaseClass
    {
        public string CreditCardRequestLogID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(CreditCardRequestLogID); } }
        public string LastFourDigit { get; set; }
        public string PayerExternalID { get; set; }
        public string CardExternalID { get; set; }
        public string FullName { get; set; }
        public decimal Amount { get; set; }
        public string IPAddress { get; set; }
        public DateTime CreatedOn { get; set; }
        
        public CreditCardRequestLog()
        {
        }
        public CreditCardRequestLog(string CreditCardRequestLogID)
        {
            this.CreditCardRequestLogID = CreditCardRequestLogID;
            Load();
        }
        public CreditCardRequestLog(DataRow objRow)
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
                         "FROM CreditCardRequestLog (NOLOCK) " +
                         "WHERE CreditCardRequestLogID=" + Database.HandleQuote(CreditCardRequestLogID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("CreditCardRequestLogID=" + CreditCardRequestLogID + " is not found");
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
                if (objColumns.Contains("CreditCardRequestLogID")) CreditCardRequestLogID = Convert.ToString(objRow["CreditCardRequestLogID"]);
                if (objColumns.Contains("LastFourDigit")) LastFourDigit = Convert.ToString(objRow["LastFourDigit"]);
                if (objColumns.Contains("PayerExternalID")) PayerExternalID = Convert.ToString(objRow["PayerExternalID"]);
                if (objColumns.Contains("CardExternalID")) CardExternalID = Convert.ToString(objRow["CardExternalID"]);
                if (objColumns.Contains("Amount") && objRow["Amount"] != DBNull.Value) Amount = Convert.ToDecimal(objRow["Amount"]);
                if (objColumns.Contains("IPAddress")) IPAddress = Convert.ToString(objRow["IPAddress"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(CreditCardRequestLogID)) throw new Exception("Missing CreditCardRequestLogID in the datarow");
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
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (!IsNew) throw new Exception("Create cannot be performed, CreditCardRequestLogID already exists");

                dicParam["LastFourDigit"] = LastFourDigit;
                dicParam["PayerExternalID"] = PayerExternalID;
                dicParam["CardExternalID"] = CardExternalID;
                dicParam["FullName"] = FullName;
                dicParam["Amount"] = Amount;
                dicParam["IPAddress"] = IPAddress;
                CreditCardRequestLogID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "CreditCardRequestLog"), objConn, objTran).ToString();

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
                if (CreditCardRequestLogID == null) throw new Exception("CreditCardRequestLogID is required");
                if (IsNew) throw new Exception("Update cannot be performed, CreditCardRequestLogID is missing");

                dicParam["LastFourDigit"] = LastFourDigit;
                dicParam["PayerExternalID"] = PayerExternalID;
                dicParam["CardExternalID"] = CardExternalID;
                dicParam["FullName"] = FullName;
                dicParam["Amount"] = Amount;
                dicParam["IPAddress"] = IPAddress;
                dicWParam["CreditCardRequestLogID"] = CreditCardRequestLogID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "CreditCardRequestLog"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, CreditCardRequestLogID is missing");

                dicDParam["CreditCardRequestLogID"] = CreditCardRequestLogID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "CreditCardRequestLog"), objConn, objTran);
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

        public static CreditCardRequestLog GetCreditCardRequestLog(CreditCardRequestLogFilter Filter)
        {
            List<CreditCardRequestLog> objCreditCardRequestLogs = null;
            CreditCardRequestLog objReturn = null;

            try
            {
                objCreditCardRequestLogs = GetCreditCardRequestLogs(Filter);
                if (objCreditCardRequestLogs != null && objCreditCardRequestLogs.Count >= 1) objReturn = objCreditCardRequestLogs[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCreditCardRequestLogs = null;
            }
            return objReturn;
        }

        public static List<CreditCardRequestLog> GetCreditCardRequestLogs()
        {
            int intTotalCount = 0;
            return GetCreditCardRequestLogs(null, null, null, out intTotalCount);
        }

        public static List<CreditCardRequestLog> GetCreditCardRequestLogs(CreditCardRequestLogFilter Filter)
        {
            int intTotalCount = 0;
            return GetCreditCardRequestLogs(Filter, null, null, out intTotalCount);
        }

        public static List<CreditCardRequestLog> GetCreditCardRequestLogs(CreditCardRequestLogFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetCreditCardRequestLogs(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<CreditCardRequestLog> GetCreditCardRequestLogs(CreditCardRequestLogFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<CreditCardRequestLog> objReturn = null;
            CreditCardRequestLog objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<CreditCardRequestLog>();

                strSQL = "SELECT * " +
                         "FROM CreditCardRequestLog (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.LastFourDigit != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.LastFourDigit, "LastFourDigit");
                    if (Filter.PayerExternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PayerExternalID, "PayerExternalID");
                    if (Filter.CardExternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CardExternalID, "CardExternalID");
                    if (Filter.FullName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.FullName, "FullName");
                    if (Filter.Amount != null) strSQL += "AND Amount=" + Database.HandleQuote(Convert.ToDecimal(Filter.Amount.Value).ToString());
                    if (Filter.IPAddress != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.IPAddress, "IPAddress");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "CreditCardRequestLogID" : Utility.CustomSorting.GetSortExpression(typeof(CreditCardRequestLog), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new CreditCardRequestLog(objData.Tables[0].Rows[i]);
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

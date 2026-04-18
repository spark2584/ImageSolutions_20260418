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

namespace ImageSolutions.Payment
{
    public class PaymentTerm : ISBase.BaseClass
    {
        public string PaymentTermID { get; private set; }
        public string InternalID { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }

        public PaymentTerm()
        {
        }
        public PaymentTerm(string PaymentTermID)
        {
            this.PaymentTermID = PaymentTermID;
            Load();
        }
        public PaymentTerm(DataRow objRow)
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
                         "FROM PaymentTerm (NOLOCK) " +
                         "WHERE PaymentTermID=" + Database.HandleQuote(PaymentTermID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("PaymentTermID=" + PaymentTermID + " is not found");
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

                if (objColumns.Contains("PaymentTermID")) PaymentTermID = Convert.ToString(objRow["PaymentTermID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("Description")) Description = Convert.ToString(objRow["Description"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(PaymentTermID)) throw new Exception("Missing PaymentTermID in the datarow");
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
                dicParam["Description"] = Description;

                PaymentTermID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "PaymentTerm"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(PaymentTermID)) throw new Exception("PaymentTermID is required");

                dicParam["InternalID"] = InternalID;
                dicParam["Description"] = Description;
                dicWParam["PaymentTermID"] = PaymentTermID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "PaymentTerm"), objConn, objTran);

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
                dicDParam["PaymentTermID"] = PaymentTermID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "PaymentTerm"), objConn, objTran);
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

        public static PaymentTerm GetPaymentTerm(PaymentTermFilter Filter)
        {
            List<PaymentTerm> objTaskEntries = null;
            PaymentTerm objReturn = null;

            try
            {
                objTaskEntries = GetPaymentTerms(Filter);
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

        public static List<PaymentTerm> GetPaymentTerms()
        {
            int intTotalCount = 0;
            return GetPaymentTerms(null, null, null, out intTotalCount);
        }

        public static List<PaymentTerm> GetPaymentTerms(PaymentTermFilter Filter)
        {
            int intTotalCount = 0;
            return GetPaymentTerms(Filter, null, null, out intTotalCount);
        }

        public static List<PaymentTerm> GetPaymentTerms(PaymentTermFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetPaymentTerms(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<PaymentTerm> GetPaymentTerms(PaymentTermFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<PaymentTerm> objReturn = null;
            PaymentTerm objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<PaymentTerm>();

                strSQL = "SELECT * " +
                         "FROM PaymentTerm (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.Description != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Description, "Description");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "PaymentTermID" : Utility.CustomSorting.GetSortExpression(typeof(PaymentTerm), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new PaymentTerm(objData.Tables[0].Rows[i]);
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

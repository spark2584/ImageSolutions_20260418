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

namespace ImageSolutions.Account
{
    public class AccountOrderApproval : ISBase.BaseClass
    {
        public string AccountOrderApprovalID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(AccountOrderApprovalID); } }
        public string AccountID { get; set; }
        public string UserWebsiteID { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedOn { get; set; }
        private UserWebsite mUserWebsite = null;
        public UserWebsite UserWebsite
        {
            get
            {
                if (mUserWebsite == null && !string.IsNullOrEmpty(UserWebsiteID))
                {
                    mUserWebsite = new UserWebsite(UserWebsiteID);
                }
                return mUserWebsite;
            }
        }
        public AccountOrderApproval()
        {
        }
        public AccountOrderApproval(string AccountOrderApprovalID)
        {
            this.AccountOrderApprovalID = AccountOrderApprovalID;
            Load();
        }
        public AccountOrderApproval(DataRow objRow)
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
                         "FROM AccountOrderApproval (NOLOCK) " +
                         "WHERE AccountOrderApprovalID=" + Database.HandleQuote(AccountOrderApprovalID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("AccountOrderApprovalID=" + AccountOrderApprovalID + " is not found");
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

                if (objColumns.Contains("AccountOrderApprovalID")) AccountOrderApprovalID = Convert.ToString(objRow["AccountOrderApprovalID"]);
                if (objColumns.Contains("AccountID")) AccountID = Convert.ToString(objRow["AccountID"]);
                if (objColumns.Contains("UserWebsiteID")) UserWebsiteID = Convert.ToString(objRow["UserWebsiteID"]);
                if (objColumns.Contains("Amount")) Amount = Convert.ToDecimal(objRow["Amount"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(AccountOrderApprovalID)) throw new Exception("Missing AccountOrderApprovalID in the datarow");
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
                dicParam["AccountID"] = AccountID;
                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["Amount"] = Amount;

                AccountOrderApprovalID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AccountOrderApproval"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(AccountOrderApprovalID)) throw new Exception("AccountOrderApprovalID is required");

                dicParam["AccountID"] = AccountID;
                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["Amount"] = Amount;
                dicWParam["AccountOrderApprovalID"] = AccountOrderApprovalID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AccountOrderApproval"), objConn, objTran);

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
                dicDParam["AccountOrderApprovalID"] = AccountOrderApprovalID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AccountOrderApproval"), objConn, objTran);
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

        public static AccountOrderApproval GetAccountOrderApproval(AccountOrderApprovalFilter Filter)
        {
            List<AccountOrderApproval> objTaskEntries = null;
            AccountOrderApproval objReturn = null;

            try
            {
                objTaskEntries = GetAccountOrderApprovals(Filter);
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

        public static List<AccountOrderApproval> GetAccountOrderApprovals()
        {
            int intTotalCount = 0;
            return GetAccountOrderApprovals(null, null, null, out intTotalCount);
        }

        public static List<AccountOrderApproval> GetAccountOrderApprovals(AccountOrderApprovalFilter Filter)
        {
            int intTotalCount = 0;
            return GetAccountOrderApprovals(Filter, null, null, out intTotalCount);
        }

        public static List<AccountOrderApproval> GetAccountOrderApprovals(AccountOrderApprovalFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAccountOrderApprovals(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AccountOrderApproval> GetAccountOrderApprovals(AccountOrderApprovalFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AccountOrderApproval> objReturn = null;
            AccountOrderApproval objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AccountOrderApproval>();

                strSQL = "SELECT * " +
                         "FROM AccountOrderApproval (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.UserInfoID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserInfoID, "UserInfoID");
                    if (Filter.AccountID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.AccountID, "AccountID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "AccountOrderApprovalID" : Utility.CustomSorting.GetSortExpression(typeof(AccountOrderApproval), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AccountOrderApproval(objData.Tables[0].Rows[i]);
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

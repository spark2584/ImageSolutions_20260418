using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.CreditMemo
{
    public class CreditMemo : ISBase.BaseClass
    {
        public string CreditMemoID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(CreditMemoID); } }
        public string SalesOrderID { get; set; }
        public string UserInfoID { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string InternalID { get; set; }
        public string CustomerInternalID { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TransactionDiscount { get; set; }
        public decimal? TotalTax { get; set; }
        public decimal? ShippingCost { get; set; }
        public decimal? Total { get; set; }
        public string Memo { get; set; }
        public bool IsExported { get; set; }
        public string EnterpriseFileSubmitID { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedOn { get; set; }

        public CreditMemo()
        {
        }

        public CreditMemo(string CreditMemoID)
        {
            this.CreditMemoID = CreditMemoID;
            Load();
        }
        public CreditMemo(DataRow objRow)
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
                         "FROM CreditMemo (NOLOCK) " +
                         "WHERE CreditMemoID=" + Database.HandleQuote(CreditMemoID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("CreditMemoID=" + CreditMemoID + " is not found");
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

                if (objColumns.Contains("CreditMemoID")) CreditMemoID = Convert.ToString(objRow["CreditMemoID"]);
                if (objColumns.Contains("SalesOrderID")) SalesOrderID = Convert.ToString(objRow["SalesOrderID"]);
                if (objColumns.Contains("UserInfoID")) UserInfoID = Convert.ToString(objRow["UserInfoID"]);
                if (objColumns.Contains("TransactionDate") && objRow["TransactionDate"] != DBNull.Value) TransactionDate = Convert.ToDateTime(objRow["TransactionDate"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("CustomerInternalID")) CustomerInternalID = Convert.ToString(objRow["CustomerInternalID"]);
                if (objColumns.Contains("SubTotal") && objRow["SubTotal"] != DBNull.Value) SubTotal = Convert.ToDecimal(objRow["SubTotal"]);
                if (objColumns.Contains("TransactionDiscount") && objRow["TransactionDiscount"] != DBNull.Value) TransactionDiscount = Convert.ToDecimal(objRow["TransactionDiscount"]);
                if (objColumns.Contains("TotalTax") && objRow["TotalTax"] != DBNull.Value) TotalTax = Convert.ToDecimal(objRow["TotalTax"]);
                if (objColumns.Contains("ShippingCost") && objRow["ShippingCost"] != DBNull.Value) ShippingCost = Convert.ToDecimal(objRow["ShippingCost"]);
                if (objColumns.Contains("Total") && objRow["Total"] != DBNull.Value) Total = Convert.ToDecimal(objRow["Total"]);
                if (objColumns.Contains("Memo")) Memo = Convert.ToString(objRow["Memo"]);
                if (objColumns.Contains("IsExported")) IsExported = Convert.ToBoolean(objRow["IsExported"]);
                if (objColumns.Contains("EnterpriseFileSubmitID")) EnterpriseFileSubmitID = Convert.ToString(objRow["EnterpriseFileSubmitID"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);

                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(CreditMemoID)) throw new Exception("Missing CreditMemoID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, CreditMemoID already exists");

                dicParam["SalesOrderID"] = SalesOrderID;
                dicParam["UserInfoID"] = UserInfoID;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["InternalID"] = InternalID;
                dicParam["CustomerInternalID"] = CustomerInternalID;
                dicParam["SubTotal"] = SubTotal;
                dicParam["TransactionDiscount"] = TransactionDiscount;
                dicParam["TotalTax"] = TotalTax;
                dicParam["ShippingCost"] = ShippingCost;
                dicParam["Total"] = Total;
                dicParam["Memo"] = Memo;
                dicParam["IsExported"] = IsExported;
                dicParam["EnterpriseFileSubmitID"] = EnterpriseFileSubmitID;
                dicParam["ErrorMessage"] = ErrorMessage;

                CreditMemoID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "CreditMemo"), objConn, objTran).ToString();

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
                if (CreditMemoID == null) throw new Exception("CreditMemoID is required");
                if (IsNew) throw new Exception("Update cannot be performed, CreditMemoID is missing");

                dicParam["SalesOrderID"] = SalesOrderID;
                dicParam["UserInfoID"] = UserInfoID;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["InternalID"] = InternalID;
                dicParam["CustomerInternalID"] = CustomerInternalID;
                dicParam["SubTotal"] = SubTotal;
                dicParam["TransactionDiscount"] = TransactionDiscount;
                dicParam["TotalTax"] = TotalTax;
                dicParam["ShippingCost"] = ShippingCost;
                dicParam["Total"] = Total;
                dicParam["Memo"] = Memo;
                dicParam["IsExported"] = IsExported;
                dicParam["EnterpriseFileSubmitID"] = EnterpriseFileSubmitID;
                dicParam["ErrorMessage"] = ErrorMessage;


                dicWParam["CreditMemoID"] = CreditMemoID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "CreditMemo"), objConn, objTran);

                //foreach (CreditMemoLine objCreditMemoLine in CreditMemoLines)
                //{
                //    objCreditMemoLine.Update(objConn, objTran);
                //}

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
                if (IsNew) throw new Exception("Delete cannot be performed, CreditMemoID is missing");

                //Delete Lines
                //List<CreditMemoLine> lstCreditMemoLine;
                //lstCreditMemoLine = CreditMemoLines;
                //foreach (CreditMemoLine _CreditMemoLine in lstCreditMemoLine)
                //{
                //    _CreditMemoLine.Delete(objConn, objTran);
                //}

                dicDParam["CreditMemoID"] = CreditMemoID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "CreditMemo"), objConn, objTran);
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

        public static CreditMemo GetCreditMemo(CreditMemoFilter Filter)
        {
            List<CreditMemo> objCreditMemos = null;
            CreditMemo objReturn = null;

            try
            {
                objCreditMemos = GetCreditMemos(Filter);
                if (objCreditMemos != null && objCreditMemos.Count >= 1) objReturn = objCreditMemos[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCreditMemos = null;
            }
            return objReturn;
        }

        public static List<CreditMemo> GetCreditMemos()
        {
            int intTotalCount = 0;
            return GetCreditMemos(null, null, null, out intTotalCount);
        }

        public static List<CreditMemo> GetCreditMemos(CreditMemoFilter Filter)
        {
            int intTotalCount = 0;
            return GetCreditMemos(Filter, null, null, out intTotalCount);
        }

        public static List<CreditMemo> GetCreditMemos(CreditMemoFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetCreditMemos(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<CreditMemo> GetCreditMemos(CreditMemoFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<CreditMemo> objReturn = null;
            CreditMemo objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<CreditMemo>();

                strSQL = "SELECT * " +
                         "FROM CreditMemo (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.SalesOrderID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderID, "SalesOrderID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "CreditMemoID" : Utility.CustomSorting.GetSortExpression(typeof(CreditMemo), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new CreditMemo(objData.Tables[0].Rows[i]);
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

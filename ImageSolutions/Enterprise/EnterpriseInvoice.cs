using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Enterprise
{
    public class EnterpriseInvoice : ISBase.BaseClass
    {
        public string EnterpriseInvoiceID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(EnterpriseInvoiceID); } }
        public string InvoiceInternalID { get; set; }
        public string SalesOrderInternalID { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime InvoiceCreatedOn { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal? BudgetShippingAmount { get; set; }
        public decimal? BudgetTaxAmount { get; set; }
        public bool IsExported { get; set; }
        public string EnterpriseFileSubmitID { get; set; }
        public bool IsNSUpdated { get; set; }
        public DateTime CreatedOn { get; set; }

        public EnterpriseInvoice()
        {
        }

        public EnterpriseInvoice(string EnterpriseInvoiceID)
        {
            this.EnterpriseInvoiceID = EnterpriseInvoiceID;
            Load();
        }
        public EnterpriseInvoice(DataRow objRow)
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
                         "FROM EnterpriseInvoice (NOLOCK) " +
                         "WHERE EnterpriseInvoiceID=" + Database.HandleQuote(EnterpriseInvoiceID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("EnterpriseInvoiceID=" + EnterpriseInvoiceID + " is not found");
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

                if (objColumns.Contains("EnterpriseInvoiceID")) EnterpriseInvoiceID = Convert.ToString(objRow["EnterpriseInvoiceID"]);
                if (objColumns.Contains("InvoiceInternalID")) InvoiceInternalID = Convert.ToString(objRow["InvoiceInternalID"]);
                if (objColumns.Contains("SalesOrderInternalID")) SalesOrderInternalID = Convert.ToString(objRow["SalesOrderInternalID"]);
                if (objColumns.Contains("TransactionDate")) TransactionDate = Convert.ToDateTime(objRow["TransactionDate"]);
                if (objColumns.Contains("InvoiceCreatedOn")) InvoiceCreatedOn = Convert.ToDateTime(objRow["InvoiceCreatedOn"]);
                if (objColumns.Contains("TotalAmount")) TotalAmount = Convert.ToDecimal(objRow["TotalAmount"]);
                if (objColumns.Contains("DiscountAmount")) DiscountAmount = Convert.ToDecimal(objRow["DiscountAmount"]);
                if (objColumns.Contains("ShippingAmount")) ShippingAmount = Convert.ToDecimal(objRow["ShippingAmount"]);
                if (objColumns.Contains("TaxAmount")) TaxAmount = Convert.ToDecimal(objRow["TaxAmount"]);
                if (objColumns.Contains("DiscountAmount")) DiscountAmount = Convert.ToDecimal(objRow["DiscountAmount"]);
                if (objColumns.Contains("SubTotal")) SubTotal = Convert.ToDecimal(objRow["SubTotal"]);
                if (objColumns.Contains("BudgetShippingAmount") && objRow["BudgetShippingAmount"] != DBNull.Value) BudgetShippingAmount = Convert.ToDecimal(objRow["BudgetShippingAmount"]);
                if (objColumns.Contains("BudgetTaxAmount") && objRow["BudgetTaxAmount"] != DBNull.Value) BudgetTaxAmount = Convert.ToDecimal(objRow["BudgetTaxAmount"]);
                if (objColumns.Contains("IsExported")) IsExported = Convert.ToBoolean(objRow["IsExported"]);
                if (objColumns.Contains("EnterpriseFileSubmitID")) EnterpriseFileSubmitID = Convert.ToString(objRow["EnterpriseFileSubmitID"]);
                if (objColumns.Contains("IsNSUpdated")) IsNSUpdated = Convert.ToBoolean(objRow["IsNSUpdated"]);

                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(EnterpriseInvoiceID)) throw new Exception("Missing EnterpriseInvoiceID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, EnterpriseInvoiceID already exists");

                dicParam["InvoiceInternalID"] = InvoiceInternalID;
                dicParam["SalesOrderInternalID"] = SalesOrderInternalID;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["InvoiceCreatedOn"] = InvoiceCreatedOn;
                dicParam["TotalAmount"] = TotalAmount;
                dicParam["DiscountAmount"] = DiscountAmount;
                dicParam["ShippingAmount"] = ShippingAmount;
                dicParam["TaxAmount"] = TaxAmount;
                dicParam["SubTotal"] = SubTotal;
                dicParam["BudgetShippingAmount"] = BudgetShippingAmount;
                dicParam["BudgetTaxAmount"] = BudgetTaxAmount;
                dicParam["IsExported"] = IsExported;
                dicParam["EnterpriseFileSubmitID"] = EnterpriseFileSubmitID;
                dicParam["IsNSUpdated"] = IsNSUpdated;

                EnterpriseInvoiceID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "EnterpriseInvoice"), objConn, objTran).ToString();

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
                if (EnterpriseInvoiceID == null) throw new Exception("EnterpriseInvoiceID is required");
                if (IsNew) throw new Exception("Update cannot be performed, EnterpriseInvoiceID is missing");

                dicParam["InvoiceInternalID"] = InvoiceInternalID;
                dicParam["SalesOrderInternalID"] = SalesOrderInternalID;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["InvoiceCreatedOn"] = InvoiceCreatedOn;
                dicParam["TotalAmount"] = TotalAmount;
                dicParam["DiscountAmount"] = DiscountAmount;
                dicParam["ShippingAmount"] = ShippingAmount;
                dicParam["TaxAmount"] = TaxAmount;
                dicParam["SubTotal"] = SubTotal;
                dicParam["BudgetShippingAmount"] = BudgetShippingAmount;
                dicParam["BudgetTaxAmount"] = BudgetTaxAmount;
                dicParam["IsExported"] = IsExported;
                dicParam["EnterpriseFileSubmitID"] = EnterpriseFileSubmitID;
                dicParam["IsNSUpdated"] = IsNSUpdated;

                dicWParam["EnterpriseInvoiceID"] = EnterpriseInvoiceID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "EnterpriseInvoice"), objConn, objTran);

                //foreach (EnterpriseInvoiceLine objEnterpriseInvoiceLine in EnterpriseInvoiceLines)
                //{
                //    objEnterpriseInvoiceLine.Update(objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, EnterpriseInvoiceID is missing");

                //Delete Lines
                //List<EnterpriseInvoiceLine> lstEnterpriseInvoiceLine;
                //lstEnterpriseInvoiceLine = EnterpriseInvoiceLines;
                //foreach (EnterpriseInvoiceLine _EnterpriseInvoiceLine in lstEnterpriseInvoiceLine)
                //{
                //    _EnterpriseInvoiceLine.Delete(objConn, objTran);
                //}

                dicDParam["EnterpriseInvoiceID"] = EnterpriseInvoiceID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "EnterpriseInvoice"), objConn, objTran);
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

        public static EnterpriseInvoice GetEnterpriseInvoice(EnterpriseInvoiceFilter Filter)
        {
            List<EnterpriseInvoice> objEnterpriseInvoices = null;
            EnterpriseInvoice objReturn = null;

            try
            {
                objEnterpriseInvoices = GetEnterpriseInvoices(Filter);
                if (objEnterpriseInvoices != null && objEnterpriseInvoices.Count >= 1) objReturn = objEnterpriseInvoices[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEnterpriseInvoices = null;
            }
            return objReturn;
        }

        public static List<EnterpriseInvoice> GetEnterpriseInvoices()
        {
            int intTotalCount = 0;
            return GetEnterpriseInvoices(null, null, null, out intTotalCount);
        }

        public static List<EnterpriseInvoice> GetEnterpriseInvoices(EnterpriseInvoiceFilter Filter)
        {
            int intTotalCount = 0;
            return GetEnterpriseInvoices(Filter, null, null, out intTotalCount);
        }

        public static List<EnterpriseInvoice> GetEnterpriseInvoices(EnterpriseInvoiceFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetEnterpriseInvoices(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<EnterpriseInvoice> GetEnterpriseInvoices(EnterpriseInvoiceFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<EnterpriseInvoice> objReturn = null;
            EnterpriseInvoice objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<EnterpriseInvoice>();

                strSQL = "SELECT * " +
                         "FROM EnterpriseInvoice (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.SalesOrderInternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderInternalID, "SalesOrderInternalID");
                    if (Filter.InvoiceInternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InvoiceInternalID, "InvoiceInternalID");
                    if (Filter.IsExported != null) strSQL += "AND IsExported=" + Database.HandleQuote(Convert.ToInt32(Filter.IsExported.Value).ToString());
                    if (Filter.IsNSUpdated != null) strSQL += "AND IsNSUpdated=" + Database.HandleQuote(Convert.ToInt32(Filter.IsNSUpdated.Value).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "EnterpriseInvoiceID" : Utility.CustomSorting.GetSortExpression(typeof(EnterpriseInvoice), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new EnterpriseInvoice(objData.Tables[0].Rows[i]);
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

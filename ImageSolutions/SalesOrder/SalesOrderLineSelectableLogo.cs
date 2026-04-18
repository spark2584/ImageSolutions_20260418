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

namespace ImageSolutions.SalesOrder
{
    public class SalesOrderLineSelectableLogo : ISBase.BaseClass
    {
        public string SalesOrderLineSelectableLogoID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(SalesOrderLineSelectableLogoID); } }
        public string SalesOrderLineID { get; set; }
        public string SelectableLogoID { get; set; }
        public bool HasNoLogo { get; set; }
        public string SelectYear { get; set; }
        public decimal? BasePrice { get; set; }
        public DateTime CreatedOn { get; set; }

        private ImageSolutions.SelectableLogo.SelectableLogo mSelectableLogo = null;
        public ImageSolutions.SelectableLogo.SelectableLogo SelectableLogo
        {
            get
            {
                if (mSelectableLogo == null && !string.IsNullOrEmpty(SelectableLogoID))
                {
                    mSelectableLogo = new ImageSolutions.SelectableLogo.SelectableLogo(SelectableLogoID);
                }
                return mSelectableLogo;
            }
        }

        public SalesOrderLineSelectableLogo()
        {
        }
        public SalesOrderLineSelectableLogo(string SalesOrderLineSelectableLogoID)
        {
            this.SalesOrderLineSelectableLogoID = SalesOrderLineSelectableLogoID;
            Load();
        }
        public SalesOrderLineSelectableLogo(DataRow objRow)
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
                         "FROM SalesOrderLineSelectableLogo (NOLOCK) " +
                         "WHERE SalesOrderLineSelectableLogoID=" + Database.HandleQuote(SalesOrderLineSelectableLogoID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("SalesOrderLineSelectableLogoID=" + SalesOrderLineSelectableLogoID + " is not found");
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

                if (objColumns.Contains("SalesOrderLineSelectableLogoID")) SalesOrderLineSelectableLogoID = Convert.ToString(objRow["SalesOrderLineSelectableLogoID"]);
                if (objColumns.Contains("SalesOrderLineID")) SalesOrderLineID = Convert.ToString(objRow["SalesOrderLineID"]);
                if (objColumns.Contains("SelectableLogoID")) SelectableLogoID = Convert.ToString(objRow["SelectableLogoID"]);
                if (objColumns.Contains("HasNoLogo")) HasNoLogo = Convert.ToBoolean(objRow["HasNoLogo"]);
                if (objColumns.Contains("SelectYear")) SelectYear = Convert.ToString(objRow["SelectYear"]);
                if (objColumns.Contains("BasePrice") && objRow["BasePrice"] != DBNull.Value) BasePrice = Convert.ToDecimal(objRow["BasePrice"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(SalesOrderLineSelectableLogoID)) throw new Exception("Missing SalesOrderLineSelectableLogoID in the datarow");
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
                dicParam["SalesOrderLineID"] = SalesOrderLineID;
                dicParam["SelectableLogoID"] = SelectableLogoID;
                dicParam["HasNoLogo"] = HasNoLogo;
                dicParam["SelectYear"] = SelectYear;
                dicParam["BasePrice"] = BasePrice;

                SalesOrderLineSelectableLogoID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "SalesOrderLineSelectableLogo"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(SalesOrderLineSelectableLogoID)) throw new Exception("SalesOrderLineSelectableLogoID is required");
                dicParam["SalesOrderLineID"] = SalesOrderLineID;
                dicParam["SelectableLogoID"] = SelectableLogoID;
                dicParam["HasNoLogo"] = HasNoLogo;
                dicParam["SelectYear"] = SelectYear;
                dicParam["BasePrice"] = BasePrice;
                dicWParam["SalesOrderLineSelectableLogoID"] = SalesOrderLineSelectableLogoID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "SalesOrderLineSelectableLogo"), objConn, objTran);

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
                dicDParam["SalesOrderLineSelectableLogoID"] = SalesOrderLineSelectableLogoID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "SalesOrderLineSelectableLogo"), objConn, objTran);
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

        public static SalesOrderLineSelectableLogo GetSalesOrderLineSelectableLogo(SalesOrderLineSelectableLogoFilter Filter)
        {
            List<SalesOrderLineSelectableLogo> objTaskEntries = null;
            SalesOrderLineSelectableLogo objReturn = null;

            try
            {
                objTaskEntries = GetSalesOrderLineSelectableLogos(Filter);
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

        public static List<SalesOrderLineSelectableLogo> GetSalesOrderLineSelectableLogos()
        {
            int intTotalCount = 0;
            return GetSalesOrderLineSelectableLogos(null, null, null, out intTotalCount);
        }

        public static List<SalesOrderLineSelectableLogo> GetSalesOrderLineSelectableLogos(SalesOrderLineSelectableLogoFilter Filter)
        {
            int intTotalCount = 0;
            return GetSalesOrderLineSelectableLogos(Filter, null, null, out intTotalCount);
        }

        public static List<SalesOrderLineSelectableLogo> GetSalesOrderLineSelectableLogos(SalesOrderLineSelectableLogoFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetSalesOrderLineSelectableLogos(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<SalesOrderLineSelectableLogo> GetSalesOrderLineSelectableLogos(SalesOrderLineSelectableLogoFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<SalesOrderLineSelectableLogo> objReturn = null;
            SalesOrderLineSelectableLogo objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<SalesOrderLineSelectableLogo>();

                strSQL = "SELECT * " +
                         "FROM SalesOrderLineSelectableLogo (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.SalesOrderLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderLineID, "SalesOrderLineID");
                    if (Filter.SelectableLogoID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SelectableLogoID, "SelectableLogoID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "SalesOrderLineSelectableLogoID" : Utility.CustomSorting.GetSortExpression(typeof(SalesOrderLineSelectableLogo), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new SalesOrderLineSelectableLogo(objData.Tables[0].Rows[i]);
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

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

namespace ImageSolutions.ShoppingCart
{
    public class ShoppingCartLineSelectableLogo : ISBase.BaseClass
    {
        public string ShoppingCartLineSelectableLogoID { get; private set; }
        public string ShoppingCartLineID { get; set; }
        public string SelectableLogoID { get; set; }
        public bool HasNoLogo { get; set; }
        public string SelectYear { get; set; }
        public decimal? BasePrice { get; set; }
        public DateTime CreatedOn { get; set; }

        private SelectableLogo.SelectableLogo mSelectableLogo = null;
        public SelectableLogo.SelectableLogo SelectableLogo
        {
            get
            {
                if (mSelectableLogo == null && !string.IsNullOrEmpty(SelectableLogoID))
                {
                    mSelectableLogo = new SelectableLogo.SelectableLogo(SelectableLogoID);
                }
                return mSelectableLogo;
            }
        }

        public ShoppingCartLineSelectableLogo()
        {
        }
        public ShoppingCartLineSelectableLogo(string ShoppingCartLineSelectableLogoID)
        {
            this.ShoppingCartLineSelectableLogoID = ShoppingCartLineSelectableLogoID;
            Load();
        }
        public ShoppingCartLineSelectableLogo(DataRow objRow)
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
                         "FROM ShoppingCartLineSelectableLogo (NOLOCK) " +
                         "WHERE ShoppingCartLineSelectableLogoID=" + Database.HandleQuote(ShoppingCartLineSelectableLogoID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ShoppingCartLineSelectableLogoID=" + ShoppingCartLineSelectableLogoID + " is not found");
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

                if (objColumns.Contains("ShoppingCartLineSelectableLogoID")) ShoppingCartLineSelectableLogoID = Convert.ToString(objRow["ShoppingCartLineSelectableLogoID"]);
                if (objColumns.Contains("ShoppingCartLineID")) ShoppingCartLineID = Convert.ToString(objRow["ShoppingCartLineID"]);
                if (objColumns.Contains("SelectableLogoID")) SelectableLogoID = Convert.ToString(objRow["SelectableLogoID"]);
                if (objColumns.Contains("HasNoLogo")) HasNoLogo = Convert.ToBoolean(objRow["HasNoLogo"]);
                if (objColumns.Contains("SelectYear")) SelectYear = Convert.ToString(objRow["SelectYear"]);
                if (objColumns.Contains("BasePrice") && objRow["BasePrice"] != DBNull.Value) BasePrice = Convert.ToDecimal(objRow["BasePrice"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ShoppingCartLineSelectableLogoID)) throw new Exception("Missing ShoppingCartLineSelectableLogoID in the datarow");
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
                dicParam["ShoppingCartLineID"] = ShoppingCartLineID;
                dicParam["SelectableLogoID"] = SelectableLogoID;
                dicParam["HasNoLogo"] = HasNoLogo;
                dicParam["SelectYear"] = SelectYear;
                dicParam["BasePrice"] = BasePrice;

                ShoppingCartLineSelectableLogoID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ShoppingCartLineSelectableLogo"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(ShoppingCartLineSelectableLogoID)) throw new Exception("ShoppingCartLineSelectableLogoID is required");
                dicParam["ShoppingCartLineID"] = ShoppingCartLineID;
                dicParam["SelectableLogoID"] = SelectableLogoID;
                dicParam["HasNoLogo"] = HasNoLogo;
                dicParam["SelectYear"] = SelectYear;
                dicParam["BasePrice"] = BasePrice;

                dicWParam["ShoppingCartLineSelectableLogoID"] = ShoppingCartLineSelectableLogoID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ShoppingCartLineSelectableLogo"), objConn, objTran);

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
                dicDParam["ShoppingCartLineSelectableLogoID"] = ShoppingCartLineSelectableLogoID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ShoppingCartLineSelectableLogo"), objConn, objTran);
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

        public static ShoppingCartLineSelectableLogo GetShoppingCartLineSelectableLogo(ShoppingCartLineSelectableLogoFilter Filter)
        {
            List<ShoppingCartLineSelectableLogo> objTaskEntries = null;
            ShoppingCartLineSelectableLogo objReturn = null;

            try
            {
                objTaskEntries = GetShoppingCartLineSelectableLogos(Filter);
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

        public static List<ShoppingCartLineSelectableLogo> GetShoppingCartLineSelectableLogos()
        {
            int intTotalCount = 0;
            return GetShoppingCartLineSelectableLogos(null, null, null, out intTotalCount);
        }

        public static List<ShoppingCartLineSelectableLogo> GetShoppingCartLineSelectableLogos(ShoppingCartLineSelectableLogoFilter Filter)
        {
            int intTotalCount = 0;
            return GetShoppingCartLineSelectableLogos(Filter, null, null, out intTotalCount);
        }

        public static List<ShoppingCartLineSelectableLogo> GetShoppingCartLineSelectableLogos(ShoppingCartLineSelectableLogoFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetShoppingCartLineSelectableLogos(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ShoppingCartLineSelectableLogo> GetShoppingCartLineSelectableLogos(ShoppingCartLineSelectableLogoFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ShoppingCartLineSelectableLogo> objReturn = null;
            ShoppingCartLineSelectableLogo objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ShoppingCartLineSelectableLogo>();

                strSQL = "SELECT * " +
                         "FROM ShoppingCartLineSelectableLogo (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ShoppingCartLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ShoppingCartLineID, "ShoppingCartLineID");
                    if (Filter.SelectableLogoID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SelectableLogoID, "SelectableLogoID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ShoppingCartLineSelectableLogoID" : Utility.CustomSorting.GetSortExpression(typeof(ShoppingCartLineSelectableLogo), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ShoppingCartLineSelectableLogo(objData.Tables[0].Rows[i]);
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

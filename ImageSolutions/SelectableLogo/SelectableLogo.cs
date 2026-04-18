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

namespace ImageSolutions.SelectableLogo
{
    public class SelectableLogo : ISBase.BaseClass
    {
        public string SelectableLogoID { get; private set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string LogoPosition { get; set; }
        public string NetsuiteInternalID { get; set; }
        public string SelectYears { get; set; }
        public string SelectYearsLabel { get; set; }
        public string Placement { get; set; }
        public decimal? BasePrice { get; set; }
        public bool IsPersonalization { get; set; }
        public bool Inactive { get; set; }
        public DateTime CreatedOn { get; set; }

        public SelectableLogo()
        {
        }
        public SelectableLogo(string SelectableLogoID)
        {
            this.SelectableLogoID = SelectableLogoID;
            Load();
        }
        public SelectableLogo(DataRow objRow)
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
                         "FROM SelectableLogo (NOLOCK) " +
                         "WHERE SelectableLogoID=" + Database.HandleQuote(SelectableLogoID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("SelectableLogoID=" + SelectableLogoID + " is not found");
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

                if (objColumns.Contains("SelectableLogoID")) SelectableLogoID = Convert.ToString(objRow["SelectableLogoID"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("ImagePath")) ImagePath = Convert.ToString(objRow["ImagePath"]);
                if (objColumns.Contains("LogoPosition")) LogoPosition = Convert.ToString(objRow["LogoPosition"]);
                if (objColumns.Contains("NetsuiteInternalID")) NetsuiteInternalID = Convert.ToString(objRow["NetsuiteInternalID"]);
                if (objColumns.Contains("SelectYears")) SelectYears = Convert.ToString(objRow["SelectYears"]);
                if (objColumns.Contains("SelectYearsLabel")) SelectYearsLabel = Convert.ToString(objRow["SelectYearsLabel"]);
                if (objColumns.Contains("Placement")) Placement = Convert.ToString(objRow["Placement"]);
                if (objColumns.Contains("BasePrice") && objRow["BasePrice"] != DBNull.Value) BasePrice = Convert.ToDecimal(objRow["BasePrice"]);
                if (objColumns.Contains("IsPersonalization") && objRow["IsPersonalization"] != DBNull.Value) IsPersonalization = Convert.ToBoolean(objRow["IsPersonalization"]);
                if (objColumns.Contains("Inactive") && objRow["Inactive"] != DBNull.Value) Inactive = Convert.ToBoolean(objRow["Inactive"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(SelectableLogoID)) throw new Exception("Missing SelectableLogoID in the datarow");
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
                dicParam["Name"] = Name;
                dicParam["ImagePath"] = ImagePath;
                dicParam["LogoPosition"] = LogoPosition;
                dicParam["NetsuiteInternalID"] = NetsuiteInternalID;
                dicParam["SelectYears"] = SelectYears;
                dicParam["SelectYearsLabel"] = SelectYearsLabel;
                dicParam["Placement"] = Placement;
                dicParam["BasePrice"] = BasePrice;
                dicParam["IsPersonalization"] = IsPersonalization;
                dicParam["Inactive"] = Inactive;

                SelectableLogoID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "SelectableLogo"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(SelectableLogoID)) throw new Exception("SelectableLogoID is required");

                dicParam["Name"] = Name;
                dicParam["ImagePath"] = ImagePath;
                dicParam["LogoPosition"] = LogoPosition;
                dicParam["NetsuiteInternalID"] = NetsuiteInternalID;
                dicParam["SelectYears"] = SelectYears;
                dicParam["SelectYearsLabel"] = SelectYearsLabel;
                dicParam["Placement"] = Placement;
                dicParam["BasePrice"] = BasePrice;
                dicParam["IsPersonalization"] = IsPersonalization;
                dicParam["Inactive"] = Inactive;
                dicWParam["SelectableLogoID"] = SelectableLogoID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "SelectableLogo"), objConn, objTran);

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
                dicDParam["SelectableLogoID"] = SelectableLogoID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "SelectableLogo"), objConn, objTran);
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

        public static SelectableLogo GetSelectableLogo(SelectableLogoFilter Filter)
        {
            List<SelectableLogo> objTaskEntries = null;
            SelectableLogo objReturn = null;

            try
            {
                objTaskEntries = GetSelectableLogos(Filter);
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

        public static List<SelectableLogo> GetSelectableLogos()
        {
            int intTotalCount = 0;
            return GetSelectableLogos(null, null, null, out intTotalCount);
        }

        public static List<SelectableLogo> GetSelectableLogos(SelectableLogoFilter Filter)
        {
            int intTotalCount = 0;
            return GetSelectableLogos(Filter, null, null, out intTotalCount);
        }

        public static List<SelectableLogo> GetSelectableLogos(SelectableLogoFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetSelectableLogos(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<SelectableLogo> GetSelectableLogos(SelectableLogoFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<SelectableLogo> objReturn = null;
            SelectableLogo objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<SelectableLogo>();

                strSQL = "SELECT * " +
                         "FROM SelectableLogo (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.NetsuiteInternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.NetsuiteInternalID, "NetsuiteInternalID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "SelectableLogoID" : Utility.CustomSorting.GetSortExpression(typeof(SelectableLogo), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new SelectableLogo(objData.Tables[0].Rows[i]);
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

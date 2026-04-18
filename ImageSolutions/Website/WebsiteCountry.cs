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

namespace ImageSolutions.Website
{
    public class WebsiteCountry : ISBase.BaseClass
    {
        public string WebsiteCountryID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(WebsiteCountryID); } }
        public string WebsiteID { get; set; }
        public string CountryCode { get; set; }
        public bool? Exclude { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        private UserInfo mCreatedByUser = null;
        public UserInfo CreatedByUser
        {
            get
            {
                if (mCreatedByUser == null && !string.IsNullOrEmpty(CreatedBy))
                {
                    mCreatedByUser = new UserInfo(CreatedBy);
                }
                return mCreatedByUser;
            }
        }

        public WebsiteCountry()
        {
        }
        public WebsiteCountry(string WebsiteCountryID)
        {
            this.WebsiteCountryID = WebsiteCountryID;
            Load();
        }
        public WebsiteCountry(DataRow objRow)
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
                         "FROM WebsiteCountry (NOLOCK) " +
                         "WHERE WebsiteCountryID=" + Database.HandleQuote(WebsiteCountryID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("WebsiteCountryID=" + WebsiteCountryID + " is not found");
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

                if (objColumns.Contains("WebsiteCountryID")) WebsiteCountryID = Convert.ToString(objRow["WebsiteCountryID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("CountryCode")) CountryCode = Convert.ToString(objRow["CountryCode"]);
                if (objColumns.Contains("Exclude") && objRow["Exclude"] != DBNull.Value) Exclude = Convert.ToBoolean(objRow["Exclude"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(WebsiteCountryID)) throw new Exception("Missing WebsiteCountryID in the datarow");
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
                if (WebsiteID == null) throw new Exception("WebsiteID is required");
                //if (ExistsParentIDTabName()) throw new Exception("Name already exists");
                if (!IsNew) throw new Exception("Create cannot be performed, WebsiteCountryID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["CountryCode"] = CountryCode;
                dicParam["Exclude"] = Exclude;
                dicParam["CreatedBy"] = CreatedBy;

                WebsiteCountryID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "WebsiteCountry"), objConn, objTran).ToString();

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
                if (WebsiteID == null) throw new Exception("WebsiteID is required");
                //if (ExistsParentIDTabName()) throw new Exception("Name already exists");
                if (IsNew) throw new Exception("Update cannot be performed, WebsiteCountryID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["CountryCode"] = CountryCode;
                dicParam["Exclude"] = Exclude;
                dicWParam["WebsiteCountryID"] = WebsiteCountryID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "WebsiteCountry"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, WebsiteCountryID is missing");

                dicDParam["WebsiteCountryID"] = WebsiteCountryID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "WebsiteCountry"), objConn, objTran);
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

        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;


            strSQL = "SELECT TOP 1 p.* " +
                     "FROM WebsiteCountry (NOLOCK) p " +
                     "WHERE " +
                     "(" +
                     "  (p.WebsiteID=" + Database.HandleQuote(WebsiteID) + " AND p.CountryCode =" + Database.HandleQuote(CountryCode) + ")" +
                     ") ";

            if (!string.IsNullOrEmpty(WebsiteCountryID)) strSQL += "AND p.WebsiteCountryID<>" + Database.HandleQuote(WebsiteCountryID);
            return Database.HasRows(strSQL);
        }

        public static WebsiteCountry GetWebsiteCountry(WebsiteCountryFilter Filter)
        {
            List<WebsiteCountry> objWebsiteCountrys = null;
            WebsiteCountry objReturn = null;

            try
            {
                objWebsiteCountrys = GetWebsiteCountries(Filter);
                if (objWebsiteCountrys != null && objWebsiteCountrys.Count >= 1) objReturn = objWebsiteCountrys[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebsiteCountrys = null;
            }
            return objReturn;
        }

        public static List<WebsiteCountry> GetWebsiteCountries()
        {
            int intTotalCount = 0;
            return GetWebsiteCountries(null, null, null, out intTotalCount);
        }

        public static List<WebsiteCountry> GetWebsiteCountries(WebsiteCountryFilter Filter)
        {
            int intTotalCount = 0;
            return GetWebsiteCountries(Filter, null, null, out intTotalCount);
        }

        public static List<WebsiteCountry> GetWebsiteCountries(WebsiteCountryFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetWebsiteCountries(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<WebsiteCountry> GetWebsiteCountries(WebsiteCountryFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<WebsiteCountry> objReturn = null;
            WebsiteCountry objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<WebsiteCountry>();

                strSQL = "SELECT * " +
                         "FROM WebsiteCountry (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.Exclude != null) strSQL += "Exclude=" + Database.HandleQuote(Convert.ToInt32(Filter.Exclude).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "WebsiteCountryID" : Utility.CustomSorting.GetSortExpression(typeof(WebsiteCountry), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new WebsiteCountry(objData.Tables[0].Rows[i]);
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

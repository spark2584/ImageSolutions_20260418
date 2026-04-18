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
    public class WebsiteUsefulLink : ISBase.BaseClass
    {
        public string WebsiteUsefulLinkID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(WebsiteUsefulLinkID); } }
        public string WebsiteID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public int? Sort { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }        

        private Website mWebsite = null;
        public Website Website
        {
            get
            {
                if (mWebsite == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    mWebsite = new Website(WebsiteID);
                }
                return mWebsite;
            }
        }

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

        public WebsiteUsefulLink()
        {
        }
        public WebsiteUsefulLink(string WebsiteUsefulLinkID)
        {
            this.WebsiteUsefulLinkID = WebsiteUsefulLinkID;
            Load();
        }
        public WebsiteUsefulLink(DataRow objRow)
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
                         "FROM WebsiteUsefulLink (NOLOCK) " +
                         "WHERE WebsiteUsefulLinkID=" + Database.HandleQuote(WebsiteUsefulLinkID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("WebsiteUsefulLinkID=" + WebsiteUsefulLinkID + " is not found");
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

                if (objColumns.Contains("WebsiteUsefulLinkID")) WebsiteUsefulLinkID = Convert.ToString(objRow["WebsiteUsefulLinkID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("URL")) URL = Convert.ToString(objRow["URL"]);
                if (objColumns.Contains("Sort") && objRow["Sort"] != DBNull.Value) Sort = Convert.ToInt32(objRow["Sort"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(WebsiteUsefulLinkID)) throw new Exception("Missing WebsiteUsefulLinkID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, WebsiteUsefulLinkID already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["Name"] = Name;
                dicParam["URL"] = URL;
                dicParam["Sort"] = Sort;
                dicParam["CreatedBy"] = CreatedBy;

                WebsiteUsefulLinkID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "WebsiteUsefulLink"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, WebsiteUsefulLinkID is missing");


                dicParam["WebsiteID"] = WebsiteID;
                dicParam["Name"] = Name;
                dicParam["URL"] = URL;
                dicParam["Sort"] = Sort;
                dicWParam["WebsiteUsefulLinkID"] = WebsiteUsefulLinkID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "WebsiteUsefulLink"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, WebsiteUsefulLinkID is missing");

                dicDParam["WebsiteUsefulLinkID"] = WebsiteUsefulLinkID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "WebsiteUsefulLink"), objConn, objTran);
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

        public static WebsiteUsefulLink GetWebsiteUsefulLink(WebsiteUsefulLinkFilter Filter)
        {
            List<WebsiteUsefulLink> objWebsiteUsefulLinks = null;
            WebsiteUsefulLink objReturn = null;

            try
            {
                objWebsiteUsefulLinks = GetWebsiteUsefulLinks(Filter);
                if (objWebsiteUsefulLinks != null && objWebsiteUsefulLinks.Count >= 1) objReturn = objWebsiteUsefulLinks[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebsiteUsefulLinks = null;
            }
            return objReturn;
        }

        public static List<WebsiteUsefulLink> GetWebsiteUsefulLinks()
        {
            int intTotalCount = 0;
            return GetWebsiteUsefulLinks(null, null, null, out intTotalCount);
        }

        public static List<WebsiteUsefulLink> GetWebsiteUsefulLinks(WebsiteUsefulLinkFilter Filter)
        {
            int intTotalCount = 0;
            return GetWebsiteUsefulLinks(Filter, null, null, out intTotalCount);
        }

        public static List<WebsiteUsefulLink> GetWebsiteUsefulLinks(WebsiteUsefulLinkFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetWebsiteUsefulLinks(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<WebsiteUsefulLink> GetWebsiteUsefulLinks(WebsiteUsefulLinkFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<WebsiteUsefulLink> objReturn = null;
            WebsiteUsefulLink objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<WebsiteUsefulLink>();

                strSQL = "SELECT * " +
                         "FROM WebsiteUsefulLink (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Sort" : Utility.CustomSorting.GetSortExpression(typeof(WebsiteUsefulLink), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else strSQL += " ORDER BY Sort";

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new WebsiteUsefulLink(objData.Tables[0].Rows[i]);
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

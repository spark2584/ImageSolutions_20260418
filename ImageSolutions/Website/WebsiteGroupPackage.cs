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
    public class WebsiteGroupPackage : ISBase.BaseClass
    {
        public string WebsiteGroupPackageID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(WebsiteGroupPackageID); } }
        public string WebsiteID { get; set; }
        public string WebsiteGroupID { get; set; }
        public string PackageID { get; set; }
        public DateTime CreatedOn { get; set; }

        public string GroupName
        {
            get
            {
                return WebsiteGroup.GroupName;
            }
        }

        private WebsiteGroup mWebsiteGroup = null;
        public WebsiteGroup WebsiteGroup
        {
            get
            {
                if (mWebsiteGroup == null && !string.IsNullOrEmpty(WebsiteGroupID))
                {
                    mWebsiteGroup = new WebsiteGroup(WebsiteGroupID);
                }
                return mWebsiteGroup;
            }
        }
        private ImageSolutions.Package.Package mPackage = null;
        public ImageSolutions.Package.Package Package
        {
            get
            {
                if (mPackage == null && !string.IsNullOrEmpty(PackageID))
                {
                    mPackage = new ImageSolutions.Package.Package(PackageID);
                }
                return mPackage;
            }
        }

        public WebsiteGroupPackage()
        {
        }
        public WebsiteGroupPackage(string WebsiteGroupPackageID)
        {
            this.WebsiteGroupPackageID = WebsiteGroupPackageID;
            Load();
        }
        public WebsiteGroupPackage(DataRow objRow)
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
                         "FROM WebsiteGroupPackage (NOLOCK) " +
                         "WHERE WebsiteGroupPackageID=" + Database.HandleQuote(WebsiteGroupPackageID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("WebsiteGroupPackageID=" + WebsiteGroupPackageID + " is not found");
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

                if (objColumns.Contains("WebsiteGroupPackageID")) WebsiteGroupPackageID = Convert.ToString(objRow["WebsiteGroupPackageID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("WebsiteGroupID")) WebsiteGroupID = Convert.ToString(objRow["WebsiteGroupID"]);
                if (objColumns.Contains("PackageID")) PackageID = Convert.ToString(objRow["PackageID"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(WebsiteGroupPackageID)) throw new Exception("Missing WebsiteGroupPackageID in the datarow");
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
                if (PackageID == null) throw new Exception("PackageID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, WebsiteGroupPackageID already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["WebsiteGroupID"] = WebsiteGroupID;
                dicParam["PackageID"] = PackageID;

                WebsiteGroupPackageID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "WebsiteGroupPackage"), objConn, objTran).ToString();

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
                if (PackageID == null) throw new Exception("PackageID is required");
                if (IsNew) throw new Exception("Update cannot be performed, WebsiteGroupPackageID is missing");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["WebsiteGroupID"] = WebsiteGroupID;
                dicParam["PackageID"] = PackageID;
                dicWParam["WebsiteGroupPackageID"] = WebsiteGroupPackageID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "WebsiteGroupPackage"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, WebsiteGroupPackageID is missing");

                dicDParam["WebsiteGroupPackageID"] = WebsiteGroupPackageID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "WebsiteGroupPackage"), objConn, objTran);
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

        public static WebsiteGroupPackage GetWebsiteGroupPackage(WebsiteGroupPackageFilter Filter)
        {
            List<WebsiteGroupPackage> objWebsiteGroupPackages = null;
            WebsiteGroupPackage objReturn = null;

            try
            {
                objWebsiteGroupPackages = GetWebsiteGroupPackages(Filter);
                if (objWebsiteGroupPackages != null && objWebsiteGroupPackages.Count >= 1) objReturn = objWebsiteGroupPackages[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebsiteGroupPackages = null;
            }
            return objReturn;
        }

        public static List<WebsiteGroupPackage> GetWebsiteGroupPackages()
        {
            int intTotalCount = 0;
            return GetWebsiteGroupPackages(null, null, null, out intTotalCount);
        }

        public static List<WebsiteGroupPackage> GetWebsiteGroupPackages(WebsiteGroupPackageFilter Filter)
        {
            int intTotalCount = 0;
            return GetWebsiteGroupPackages(Filter, null, null, out intTotalCount);
        }

        public static List<WebsiteGroupPackage> GetWebsiteGroupPackages(WebsiteGroupPackageFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetWebsiteGroupPackages(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<WebsiteGroupPackage> GetWebsiteGroupPackages(WebsiteGroupPackageFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<WebsiteGroupPackage> objReturn = null;
            WebsiteGroupPackage objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<WebsiteGroupPackage>();

                strSQL = "SELECT * " +
                         "FROM WebsiteGroupPackage (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.PackageID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PackageID, "PackageID");
                    if (Filter.WebsiteGroupID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteGroupID, "WebsiteGroupID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "WebsiteGroupPackageID" : Utility.CustomSorting.GetSortExpression(typeof(WebsiteGroupPackage), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new WebsiteGroupPackage(objData.Tables[0].Rows[i]);
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

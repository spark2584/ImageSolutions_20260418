using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageSolutions.User;
using ImageSolutions.Shipping;

namespace ImageSolutions.Website
{
    public class WebsiteShippingService : ISBase.BaseClass
    {
        public string WebsiteShippingServiceID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(WebsiteShippingServiceID); } }
        public string WebsiteID { get; set; }
        public string ShippingServiceID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public string Description
        {
            get
            {
                return ShippingService == null ? string.Empty : ShippingService.Description;
            }
        }
        private List<ImageSolutions.Website.WebsiteShippingServiceGroup> mWebsiteShippingServiceGroups = null;
        public List<ImageSolutions.Website.WebsiteShippingServiceGroup> WebsiteShippingServiceGroups
        {
            get
            {
                if (mWebsiteShippingServiceGroups == null && !string.IsNullOrEmpty(WebsiteShippingServiceID))
                {
                    ImageSolutions.Website.WebsiteShippingServiceGroupFilter objFilter = null;

                    try
                    {
                        objFilter = new ImageSolutions.Website.WebsiteShippingServiceGroupFilter();
                        objFilter.WebsiteShippingServiceID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteShippingServiceID.SearchString = WebsiteShippingServiceID;
                        mWebsiteShippingServiceGroups = ImageSolutions.Website.WebsiteShippingServiceGroup.GetWebsiteShippingServiceGroups(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mWebsiteShippingServiceGroups;                
            }
        }

        private Shipping.ShippingService mShippingService = null;
        public Shipping.ShippingService ShippingService
        {
            get
            {
                if (mShippingService == null && !string.IsNullOrEmpty(ShippingServiceID))
                {
                    mShippingService = new Shipping.ShippingService(ShippingServiceID);
                }
                return mShippingService;
            }
        }

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

        public WebsiteShippingService()
        {
        }
        public WebsiteShippingService(string WebsiteShippingServiceID)
        {
            this.WebsiteShippingServiceID = WebsiteShippingServiceID;
            Load();
        }
        public WebsiteShippingService(DataRow objRow)
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
                         "FROM WebsiteShippingService (NOLOCK) " +
                         "WHERE WebsiteShippingServiceID=" + Database.HandleQuote(WebsiteShippingServiceID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("WebsiteShippingServiceID=" + WebsiteShippingServiceID + " is not found");
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

                if (objColumns.Contains("WebsiteShippingServiceID")) WebsiteShippingServiceID = Convert.ToString(objRow["WebsiteShippingServiceID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("ShippingServiceID")) ShippingServiceID = Convert.ToString(objRow["ShippingServiceID"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(WebsiteShippingServiceID)) throw new Exception("Missing WebsiteShippingServiceID in the datarow");
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
                if (ShippingServiceID == null) throw new Exception("ShippingServiceID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, WebsiteShippingServiceID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["ShippingServiceID"] = ShippingServiceID;
                dicParam["CreatedBy"] = CreatedBy;
                WebsiteShippingServiceID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "WebsiteShippingService"), objConn, objTran).ToString();

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
                if (ShippingServiceID == null) throw new Exception("ShippingServiceID is required");
                if (IsNew) throw new Exception("Update cannot be performed, WebsiteShippingServiceID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["ShippingServiceID"] = ShippingServiceID;
                dicWParam["WebsiteShippingServiceID"] = WebsiteShippingServiceID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "WebsiteShippingService"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, WebsiteShippingServiceID is missing");

                dicDParam["WebsiteShippingServiceID"] = WebsiteShippingServiceID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "WebsiteShippingService"), objConn, objTran);
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
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                    "FROM WebsiteShippingService (NOLOCK) p " +
                    "WHERE " +
                    "(" +
                    "  (p.ShippingServiceID=" + Database.HandleQuote(ShippingServiceID) + " AND p.WebsiteID=" + Database.HandleQuote(WebsiteID) + ")" +
                    ") ";


            if (!string.IsNullOrEmpty(WebsiteShippingServiceID)) strSQL += "AND p.WebsiteShippingServiceID<>" + Database.HandleQuote(WebsiteShippingServiceID);
            return Database.HasRows(strSQL);
        }

        public static WebsiteShippingService GetWebsiteShippingService(WebsiteShippingServiceFilter Filter)
        {
            List<WebsiteShippingService> objWebsiteShippingServices = null;
            WebsiteShippingService objReturn = null;

            try
            {
                objWebsiteShippingServices = GetWebsiteShippingServices(Filter);
                if (objWebsiteShippingServices != null && objWebsiteShippingServices.Count >= 1) objReturn = objWebsiteShippingServices[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebsiteShippingServices = null;
            }
            return objReturn;
        }

        public static List<WebsiteShippingService> GetWebsiteShippingServices()
        {
            int intTotalCount = 0;
            return GetWebsiteShippingServices(null, null, null, out intTotalCount);
        }

        public static List<WebsiteShippingService> GetWebsiteShippingServices(WebsiteShippingServiceFilter Filter)
        {
            int intTotalCount = 0;
            return GetWebsiteShippingServices(Filter, null, null, out intTotalCount);
        }

        public static List<WebsiteShippingService> GetWebsiteShippingServices(WebsiteShippingServiceFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetWebsiteShippingServices(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<WebsiteShippingService> GetWebsiteShippingServices(WebsiteShippingServiceFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<WebsiteShippingService> objReturn = null;
            WebsiteShippingService objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<WebsiteShippingService>();

                strSQL = "SELECT * " +
                         "FROM WebsiteShippingService (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.ShippingServiceID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ShippingServiceID, "ShippingServiceID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "WebsiteShippingServiceID" : Utility.CustomSorting.GetSortExpression(typeof(WebsiteShippingService), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new WebsiteShippingService(objData.Tables[0].Rows[i]);
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

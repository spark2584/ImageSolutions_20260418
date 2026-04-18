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
    public class WebsiteTab : ISBase.BaseClass
    {
        public string WebsiteTabID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(WebsiteTabID); } }
        public string WebsiteID { get; set; }
        public string ParentID { get; set; }
        public string TabName { get; set; }
        public string DisplayName { get; set; }
        public string TabPath
        {
            get
            {
                if (ParentWebsiteTab != null)
                    return ParentWebsiteTab.TabPath + " -> " + TabName;
                else
                    return TabName;
            }
        }
        public string TabPathBreadCrumb
        {
            get
            {
                if (ParentWebsiteTab != null)
                {
                    if (Website.ItemDisplayType == "List")
                    {
                        return ParentWebsiteTab.TabPathBreadCrumb + " > " + string.Format(@"<a href=""/itemlist.aspx?WebsiteTabID={0}"">{1}</a>", WebsiteTabID, TabName);
                    }
                    else
                    {
                        return ParentWebsiteTab.TabPathBreadCrumb + " > " + string.Format(@"<a href=""/items.aspx?WebsiteTabID={0}"">{1}</a>", WebsiteTabID, TabName);
                    }
                }
                else
                {
                    if (Website.ItemDisplayType == "List")
                    {
                        return string.Format(@"<a href=""/itemlist.aspx?WebsiteTabID={0}"">{1}</a>", WebsiteTabID, TabName);
                    }
                    else
                    {
                        return string.Format(@"<a href=""/items.aspx?WebsiteTabID={0}"">{1}</a>", WebsiteTabID, TabName);
                    }
                }
            }
        }
        public string ImageURL { get; set; }
        public bool AllowAllGroups { get; set; }
        public string DisplayUserPermission { get; set; }
        public string Message { get; set; }
        public int? Sort { get; set; }
        public bool DoNotAllowMixCart { get; set; }
        public bool ExcludeShipping { get; set; }
        public bool Inactive { get; set; }
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

        private WebsiteTab mParentWebsiteTab = null;
        public WebsiteTab ParentWebsiteTab
        {
            get
            {
                if (mParentWebsiteTab == null && !string.IsNullOrEmpty(ParentID))
                {
                    mParentWebsiteTab = new WebsiteTab(ParentID);
                }
                return mParentWebsiteTab;
            }
        }

        private List<WebsiteTab> mChildWebsiteTabs = null;
        public List<WebsiteTab> ChildWebsiteTabs
        {
            get
            {
                if (mChildWebsiteTabs == null && !string.IsNullOrEmpty(WebsiteTabID))
                {
                    WebsiteTabFilter objFilter = null;

                    try
                    {
                        objFilter = new WebsiteTabFilter();
                        objFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ParentID.SearchString = WebsiteTabID;
                        mChildWebsiteTabs = WebsiteTab.GetWebsiteTabs(objFilter);
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
                return mChildWebsiteTabs;
            }
        }

        private List<WebsiteTabItem> mWebsiteTabItems = null;
        public List<WebsiteTabItem> WebsiteTabItems
        {
            get
            {
                if (mWebsiteTabItems == null && !string.IsNullOrEmpty(WebsiteTabID))
                {
                    WebsiteTabItemFilter objFilter = null;

                    try
                    {
                        objFilter = new WebsiteTabItemFilter();
                        objFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteTabID.SearchString = WebsiteTabID;
                        mWebsiteTabItems = WebsiteTabItem.GetWebsiteTabItems(objFilter);
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
                return mWebsiteTabItems;
            }
        }
        private List<WebsiteGroupTab> mWebsiteGroupTabs = null;
        public List<WebsiteGroupTab> WebsiteGroupTabs
        {
            get
            {
                if (mWebsiteGroupTabs == null && !string.IsNullOrEmpty(WebsiteTabID))
                {
                    WebsiteGroupTabFilter objFilter = null;

                    try
                    {
                        objFilter = new WebsiteGroupTabFilter();
                        objFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteTabID.SearchString = WebsiteTabID;
                        mWebsiteGroupTabs = WebsiteGroupTab.GetWebsiteGroupTabs(objFilter);
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
                return mWebsiteGroupTabs;
            }
        }
        private List<UserWebsiteTab> mUserWebsiteTabs = null;
        public List<UserWebsiteTab> UserWebsiteTabs
        {
            get
            {
                if (mUserWebsiteTabs == null && !string.IsNullOrEmpty(WebsiteTabID))
                {
                    UserWebsiteTabFilter objFilter = null;

                    try
                    {
                        objFilter = new UserWebsiteTabFilter();
                        objFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteTabID.SearchString = WebsiteTabID;
                        mUserWebsiteTabs = UserWebsiteTab.GetUserWebsiteTabs(objFilter);
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
                return mUserWebsiteTabs;
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

        public WebsiteTab()
        {
        }
        public WebsiteTab(string WebsiteTabID)
        {
            this.WebsiteTabID = WebsiteTabID;
            Load();
        }
        public WebsiteTab(DataRow objRow)
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
                         "FROM WebsiteTab (NOLOCK) " +
                         "WHERE WebsiteTabID=" + Database.HandleQuote(WebsiteTabID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("WebsiteTabID=" + WebsiteTabID + " is not found");
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

                if (objColumns.Contains("WebsiteTabID")) WebsiteTabID = Convert.ToString(objRow["WebsiteTabID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("ParentID")) ParentID = Convert.ToString(objRow["ParentID"]);
                if (objColumns.Contains("TabName")) TabName = Convert.ToString(objRow["TabName"]);
                if (objColumns.Contains("DisplayName")) DisplayName = Convert.ToString(objRow["DisplayName"]);
                if (objColumns.Contains("ImageURL")) ImageURL = Convert.ToString(objRow["ImageURL"]);
                if (objColumns.Contains("AllowAllGroups")) AllowAllGroups = Convert.ToBoolean(objRow["AllowAllGroups"]);
                if (objColumns.Contains("DisplayUserPermission")) DisplayUserPermission = Convert.ToString(objRow["DisplayUserPermission"]);
                if (objColumns.Contains("Message")) Message = Convert.ToString(objRow["Message"]);
                if (objColumns.Contains("Sort") && objRow["Sort"] != DBNull.Value) Sort = Convert.ToInt32(objRow["Sort"]);
                if (objColumns.Contains("DoNotAllowMixCart")) DoNotAllowMixCart = Convert.ToBoolean(objRow["DoNotAllowMixCart"]);
                if (objColumns.Contains("ExcludeShipping")) ExcludeShipping = Convert.ToBoolean(objRow["ExcludeShipping"]);
                if (objColumns.Contains("Inactive")) Inactive = Convert.ToBoolean(objRow["Inactive"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(WebsiteTabID)) throw new Exception("Missing WebsiteTabID in the datarow");
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
        //protected bool ExistsParentIDTabName()
        //{
        //    bool _ret = false;

        //    try
        //    {
        //        WebsiteTab WebsiteTab = new WebsiteTab();
        //        WebsiteTabFilter WebsiteTabFilter = new WebsiteTabFilter();
        //        WebsiteTabFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
        //        WebsiteTabFilter.ParentID.SearchString = ParentID;
        //        WebsiteTabFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
        //        WebsiteTabFilter.WebsiteID.SearchString = WebsiteID;
        //        WebsiteTabFilter.TabName = new Database.Filter.StringSearch.SearchFilter();
        //        WebsiteTabFilter.TabName.SearchString = TabName;
        //        WebsiteTab = WebsiteTab.GetWebsiteTab(WebsiteTabFilter);
        //        if (WebsiteTab != null && !string.IsNullOrEmpty(WebsiteTab.WebsiteTabID) && WebsiteTab.WebsiteTabID != WebsiteTabID)
        //        {
        //            _ret = true;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return _ret;
        //}
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
                if (!IsNew) throw new Exception("Create cannot be performed, WebsiteTabID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["ParentID"] = ParentID;
                dicParam["TabName"] = TabName;
                dicParam["DisplayName"] = DisplayName;
                dicParam["ImageURL"] = ImageURL;
                dicParam["AllowAllGroups"] = AllowAllGroups;
                dicParam["DisplayUserPermission"] = DisplayUserPermission;
                dicParam["Message"] = Message;
                dicParam["Sort"] = Sort;
                dicParam["DoNotAllowMixCart"] = DoNotAllowMixCart;
                dicParam["ExcludeShipping"] = ExcludeShipping;
                dicParam["Inactive"] = Inactive;
                dicParam["CreatedBy"] = CreatedBy;

                WebsiteTabID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "WebsiteTab"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, WebsiteTabID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["ParentID"] = ParentID;
                dicParam["TabName"] = TabName;
                dicParam["DisplayName"] = DisplayName;
                dicParam["ImageURL"] = ImageURL;
                dicParam["AllowAllGroups"] = AllowAllGroups;
                dicParam["DisplayUserPermission"] = DisplayUserPermission;
                dicParam["Message"] = Message;
                dicParam["Sort"] = Sort;
                dicParam["DoNotAllowMixCart"] = DoNotAllowMixCart;
                dicParam["ExcludeShipping"] = ExcludeShipping;
                dicParam["Inactive"] = Inactive;
                dicWParam["WebsiteTabID"] = WebsiteTabID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "WebsiteTab"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, WebsiteTabID is missing");

                foreach(WebsiteGroupTab _WebsiteGroupTab in WebsiteGroupTabs)
                {
                    _WebsiteGroupTab.Delete(objConn, objTran);
                }
                foreach(WebsiteTabItem _WebsiteTabItem in WebsiteTabItems)
                {
                    _WebsiteTabItem.Delete(objConn, objTran);
                }                
                foreach (WebsiteTab _WebsiteTab in ChildWebsiteTabs)
                {
                    _WebsiteTab.Delete(objConn, objTran);
                }

                dicDParam["WebsiteTabID"] = WebsiteTabID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "WebsiteTab"), objConn, objTran);
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
                     "FROM WebsiteTab (NOLOCK) p " +
                     "WHERE " +
                     "(" +
                     "  (p.WebsiteID=" + Database.HandleQuote(WebsiteID) + " AND ISNULL(p.ParentID,'')=ISNULL(" + Database.HandleQuote(ParentID) + ",'') AND p.TabName=" + Database.HandleQuote(TabName) + ")" +
                     ") ";

            if (!string.IsNullOrEmpty(WebsiteTabID)) strSQL += "AND p.WebsiteTabID<>" + Database.HandleQuote(WebsiteTabID);
            return Database.HasRows(strSQL);
        }

        public static WebsiteTab GetWebsiteTab(WebsiteTabFilter Filter)
        {
            List<WebsiteTab> objWebsiteTabs = null;
            WebsiteTab objReturn = null;

            try
            {
                objWebsiteTabs = GetWebsiteTabs(Filter);
                if (objWebsiteTabs != null && objWebsiteTabs.Count >= 1) objReturn = objWebsiteTabs[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebsiteTabs = null;
            }
            return objReturn;
        }

        public static List<WebsiteTab> GetWebsiteTabs()
        {
            int intTotalCount = 0;
            return GetWebsiteTabs(null, null, null, out intTotalCount);
        }

        public static List<WebsiteTab> GetWebsiteTabs(WebsiteTabFilter Filter)
        {
            int intTotalCount = 0;
            return GetWebsiteTabs(Filter, null, null, out intTotalCount);
        }

        public static List<WebsiteTab> GetWebsiteTabs(WebsiteTabFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetWebsiteTabs(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<WebsiteTab> GetWebsiteTabs(WebsiteTabFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<WebsiteTab> objReturn = null;
            WebsiteTab objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<WebsiteTab>();

                strSQL = "SELECT * " +
                         "FROM WebsiteTab (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.ParentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ParentID, "ParentID");
                    if (Filter.TabName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.TabName, "TabName");
                    if (Filter.AllowAllGroups != null) strSQL += "AllowAllGroups=" + Database.HandleQuote(Convert.ToInt32(Filter.AllowAllGroups).ToString());
                    if (Filter.Sort != null) strSQL += "Sort=" + Database.HandleQuote(Convert.ToInt32(Filter.Sort).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "WebsiteTabID" : Utility.CustomSorting.GetSortExpression(typeof(WebsiteTab), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else strSQL += " ORDER BY ISNULL(Sort,9999) ASC";

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new WebsiteTab(objData.Tables[0].Rows[i]);
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

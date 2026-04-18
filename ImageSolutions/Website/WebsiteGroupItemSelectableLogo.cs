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
    public class WebsiteGroupItemSelectableLogo : ISBase.BaseClass
    {
        public string WebsiteGroupItemSelectableLogoID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(WebsiteGroupItemSelectableLogoID); } }
        public string WebsiteID { get; set; }
        public string WebsiteGroupID { get; set; }
        public string ItemSelectableLogoID { get; set; }
        public string CreatedBy { get; set; }
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
        private ImageSolutions.Item.ItemSelectableLogo mItemSelectableLogo = null;
        public ImageSolutions.Item.ItemSelectableLogo ItemSelectableLogo
        {
            get
            {
                if (mItemSelectableLogo == null && !string.IsNullOrEmpty(ItemSelectableLogoID))
                {
                    mItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo(ItemSelectableLogoID);
                }
                return mItemSelectableLogo;
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

        public WebsiteGroupItemSelectableLogo()
        {
        }
        public WebsiteGroupItemSelectableLogo(string WebsiteGroupItemSelectableLogoID)
        {
            this.WebsiteGroupItemSelectableLogoID = WebsiteGroupItemSelectableLogoID;
            Load();
        }
        public WebsiteGroupItemSelectableLogo(DataRow objRow)
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
                         "FROM WebsiteGroupItemSelectableLogo (NOLOCK) " +
                         "WHERE WebsiteGroupItemSelectableLogoID=" + Database.HandleQuote(WebsiteGroupItemSelectableLogoID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("WebsiteGroupItemSelectableLogoID=" + WebsiteGroupItemSelectableLogoID + " is not found");
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

                if (objColumns.Contains("WebsiteGroupItemSelectableLogoID")) WebsiteGroupItemSelectableLogoID = Convert.ToString(objRow["WebsiteGroupItemSelectableLogoID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("WebsiteGroupID")) WebsiteGroupID = Convert.ToString(objRow["WebsiteGroupID"]);
                if (objColumns.Contains("ItemSelectableLogoID")) ItemSelectableLogoID = Convert.ToString(objRow["ItemSelectableLogoID"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(WebsiteGroupItemSelectableLogoID)) throw new Exception("Missing WebsiteGroupItemSelectableLogoID in the datarow");
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
                if (ItemSelectableLogoID == null) throw new Exception("ItemSelectableLogoID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, WebsiteGroupItemSelectableLogoID already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["WebsiteGroupID"] = WebsiteGroupID;
                dicParam["ItemSelectableLogoID"] = ItemSelectableLogoID;
                dicParam["CreatedBy"] = CreatedBy;

                WebsiteGroupItemSelectableLogoID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "WebsiteGroupItemSelectableLogo"), objConn, objTran).ToString();

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
                if (ItemSelectableLogoID == null) throw new Exception("ItemSelectableLogoID is required");
                if (IsNew) throw new Exception("Update cannot be performed, WebsiteGroupItemSelectableLogoID is missing");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["WebsiteGroupID"] = WebsiteGroupID;
                dicParam["ItemSelectableLogoID"] = ItemSelectableLogoID;
                dicWParam["WebsiteGroupItemSelectableLogoID"] = WebsiteGroupItemSelectableLogoID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "WebsiteGroupItemSelectableLogo"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, WebsiteGroupItemSelectableLogoID is missing");

                dicDParam["WebsiteGroupItemSelectableLogoID"] = WebsiteGroupItemSelectableLogoID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "WebsiteGroupItemSelectableLogo"), objConn, objTran);
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

        public static WebsiteGroupItemSelectableLogo GetWebsiteGroupItemSelectableLogo(WebsiteGroupItemSelectableLogoFilter Filter)
        {
            List<WebsiteGroupItemSelectableLogo> objWebsiteGroupItemSelectableLogos = null;
            WebsiteGroupItemSelectableLogo objReturn = null;

            try
            {
                objWebsiteGroupItemSelectableLogos = GetWebsiteGroupItemSelectableLogos(Filter);
                if (objWebsiteGroupItemSelectableLogos != null && objWebsiteGroupItemSelectableLogos.Count >= 1) objReturn = objWebsiteGroupItemSelectableLogos[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebsiteGroupItemSelectableLogos = null;
            }
            return objReturn;
        }

        public static List<WebsiteGroupItemSelectableLogo> GetWebsiteGroupItemSelectableLogos()
        {
            int intTotalCount = 0;
            return GetWebsiteGroupItemSelectableLogos(null, null, null, out intTotalCount);
        }

        public static List<WebsiteGroupItemSelectableLogo> GetWebsiteGroupItemSelectableLogos(WebsiteGroupItemSelectableLogoFilter Filter)
        {
            int intTotalCount = 0;
            return GetWebsiteGroupItemSelectableLogos(Filter, null, null, out intTotalCount);
        }

        public static List<WebsiteGroupItemSelectableLogo> GetWebsiteGroupItemSelectableLogos(WebsiteGroupItemSelectableLogoFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetWebsiteGroupItemSelectableLogos(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<WebsiteGroupItemSelectableLogo> GetWebsiteGroupItemSelectableLogos(WebsiteGroupItemSelectableLogoFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<WebsiteGroupItemSelectableLogo> objReturn = null;
            WebsiteGroupItemSelectableLogo objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<WebsiteGroupItemSelectableLogo>();

                strSQL = "SELECT * " +
                         "FROM WebsiteGroupItemSelectableLogo (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.ItemSelectableLogoID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemSelectableLogoID, "ItemSelectableLogoID");
                    if (Filter.WebsiteGroupID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteGroupID, "WebsiteGroupID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "WebsiteGroupItemSelectableLogoID" : Utility.CustomSorting.GetSortExpression(typeof(WebsiteGroupItemSelectableLogo), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new WebsiteGroupItemSelectableLogo(objData.Tables[0].Rows[i]);
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

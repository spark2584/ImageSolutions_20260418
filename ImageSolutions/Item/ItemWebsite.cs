using ImageSolutions.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ImageSolutions.Item
{
    public class ItemWebsite : ISBase.BaseClass
    {
        public string ItemWebsiteID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemWebsiteID); } }
        public string ItemID { get; set; }
        public string WebsiteID { get; set; }
        public bool IsCompanyInvoiced { get; set; }
        public bool RequireApproval { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ItemNumber { get; set; }
        public string ItemName { get; set; }
        public string ParentID { get; set; }
        public string SalesDescription { get; set; }

        public string Description
        {
            get
            {
                return Item == null ? string.Empty : Item.Description;
            }
        }

        private Item mItem = null;
        public Item Item
        {
            get
            {
                if (mItem == null && !string.IsNullOrEmpty(ItemID))
                {
                    try
                    {
                        mItem = new Item(ItemID);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                    }
                }
                return mItem;
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

        public ItemWebsite()
        {
        }
        public ItemWebsite(string ItemWebsiteID)
        {
            this.ItemWebsiteID = ItemWebsiteID;
            Load();
        }
        public ItemWebsite(DataRow objRow)
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
                         "FROM ItemWebsite (NOLOCK) " +
                         "WHERE ItemWebsiteID=" + Database.HandleQuote(ItemWebsiteID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemWebsiteID=" + ItemWebsiteID + " is not found");
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

                if (objColumns.Contains("ItemWebsiteID")) ItemWebsiteID = Convert.ToString(objRow["ItemWebsiteID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("ItemNumber")) ItemNumber = Convert.ToString(objRow["ItemNumber"]);
                if (objColumns.Contains("ItemName")) ItemName = Convert.ToString(objRow["ItemName"]);
                if (objColumns.Contains("ParentID")) ParentID = Convert.ToString(objRow["ParentID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("IsCompanyInvoiced") && objRow["IsCompanyInvoiced"] != DBNull.Value) IsCompanyInvoiced = Convert.ToBoolean(objRow["IsCompanyInvoiced"]);
                if (objColumns.Contains("RequireApproval") && objRow["RequireApproval"] != DBNull.Value) RequireApproval = Convert.ToBoolean(objRow["RequireApproval"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemWebsiteID)) throw new Exception("Missing ItemWebsiteID in the datarow");
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
                if (ItemID == null) throw new Exception("ItemID is required");
                if (WebsiteID == null) throw new Exception("WebsiteID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemWebsiteID already exists");

                dicParam["ItemID"] = ItemID;
                dicParam["WebsiteID"] = WebsiteID;
                dicParam["IsCompanyInvoiced"] = IsCompanyInvoiced;
                dicParam["RequireApproval"] = RequireApproval;
                dicParam["CreatedBy"] = CreatedBy;
                ItemWebsiteID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemWebsite"), objConn, objTran).ToString();

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
                if (ItemID == null) throw new Exception("ItemID is required");
                if (WebsiteID == null) throw new Exception("WebsiteID is required");
                if (IsNew) throw new Exception("Update cannot be performed, ItemWebsiteID is missing");

                dicParam["ItemID"] = ItemID;
                dicParam["WebsiteID"] = WebsiteID;
                dicParam["IsCompanyInvoiced"] = IsCompanyInvoiced;
                dicParam["RequireApproval"] = RequireApproval;
                dicWParam["ItemWebsiteID"] = ItemWebsiteID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemWebsite"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemWebsiteID is missing");

                dicDParam["ItemWebsiteID"] = ItemWebsiteID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemWebsite"), objConn, objTran);
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

        public static ItemWebsite GetItemWebsite(ItemWebsiteFilter Filter)
        {
            List<ItemWebsite> objItemWebsites = null;
            ItemWebsite objReturn = null;

            try
            {
                objItemWebsites = GetItemWebsites(Filter);
                if (objItemWebsites != null && objItemWebsites.Count >= 1) objReturn = objItemWebsites[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemWebsites = null;
            }
            return objReturn;
        }

        public static List<ItemWebsite> GetItemWebsites()
        {
            int intTotalCount = 0;
            return GetItemWebsites(null, null, null, out intTotalCount);
        }

        public static List<ItemWebsite> GetItemWebsites(ItemWebsiteFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemWebsites(Filter, null, null, out intTotalCount);
        }

        public static List<ItemWebsite> GetItemWebsites(ItemWebsiteFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemWebsites(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemWebsite> GetItemWebsites(ItemWebsiteFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemWebsite> objReturn = null;
            ItemWebsite objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemWebsite>();

                strSQL = "SELECT i.ItemNumber, i.ItemName, i.ParentID, iw.* " +
                         "FROM ItemWebsite (NOLOCK) iw " +
                         "Inner Join Item (NOLOCK) i on i.ItemID = iw.ItemID " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ItemWebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemWebsiteID, "iw.ItemWebsiteID");
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "iw.ItemID");
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "iw.WebsiteID");
                    if (Filter.ItemNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemNumber, "i.ItemNumber");
                    if (Filter.ItemName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemName, "i.ItemName");
                    if (Filter.SalesDescription != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesDescription, "i.SalesDescription");
                    if (Filter.ParentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ParentID, "i.ParentID");
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "i.InternalID");
                    if (Filter.InActive != null) strSQL += "AND i.InActive=" + Database.HandleQuote(Convert.ToInt32(Filter.InActive.Value).ToString());
                    if (Filter.IsOnline != null) strSQL += "AND i.IsOnline=" + Database.HandleQuote(Convert.ToInt32(Filter.IsOnline.Value).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemWebsiteID" : Utility.CustomSorting.GetSortExpression(typeof(ItemWebsite), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemWebsite(objData.Tables[0].Rows[i]);
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

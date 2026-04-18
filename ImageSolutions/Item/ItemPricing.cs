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
    public class ItemPricing : ISBase.BaseClass
    {
        public string ItemPricingID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemPricingID); } }
        public string ItemID { get; set; }
        public string WebsiteGroupID { get; set; }
        public double Price { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }        

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

        private Website.WebsiteGroup mWebsiteGroup = null;
        public Website.WebsiteGroup WebsiteGroup
        {
            get
            {
                if (mWebsiteGroup == null && !string.IsNullOrEmpty(WebsiteGroupID))
                {
                    mWebsiteGroup = new Website.WebsiteGroup(WebsiteGroupID);
                }
                return mWebsiteGroup;
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
        public ItemPricing()
        {
        }
        public ItemPricing(string ItemPricingID)
        {
            this.ItemPricingID = ItemPricingID;
            Load();
        }
        public ItemPricing(DataRow objRow)
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
                         "FROM ItemPricing (NOLOCK) " +
                         "WHERE ItemPricingID=" + Database.HandleQuote(ItemPricingID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemPricingID=" + ItemPricingID + " is not found");
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

                if (objColumns.Contains("ItemPricingID")) ItemPricingID = Convert.ToString(objRow["ItemPricingID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("WebsiteGroupID")) WebsiteGroupID = Convert.ToString(objRow["WebsiteGroupID"]);
                if (objColumns.Contains("Price")) Price = Convert.ToDouble(objRow["Price"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemPricingID)) throw new Exception("Missing ItemPricingID in the datarow");
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
                if (WebsiteGroupID == null) throw new Exception("WebsiteGroupID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemPricingID already exists");

                dicParam["ItemID"] = ItemID;
                dicParam["WebsiteGroupID"] = WebsiteGroupID;
                dicParam["Price"] = Price;
                dicParam["CreatedBy"] = CreatedBy;
                ItemPricingID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemPricing"), objConn, objTran).ToString();

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
                if (WebsiteGroupID == null) throw new Exception("WebsiteID is required");
                if (IsNew) throw new Exception("Update cannot be performed, ItemPricingID is missing");

                dicParam["ItemID"] = ItemID;
                dicParam["WebsiteGroupID"] = WebsiteGroupID;
                dicParam["Price"] = Price;
                dicWParam["ItemPricingID"] = ItemPricingID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemPricing"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemPricingID is missing");

                dicDParam["ItemPricingID"] = ItemPricingID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemPricing"), objConn, objTran);
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

        public static ItemPricing GetItemPricing(ItemPricingFilter Filter)
        {
            List<ItemPricing> objItemPricings = null;
            ItemPricing objReturn = null;

            try
            {
                objItemPricings = GetItemPricings(Filter);
                if (objItemPricings != null && objItemPricings.Count >= 1) objReturn = objItemPricings[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemPricings = null;
            }
            return objReturn;
        }

        public static List<ItemPricing> GetItemPricings()
        {
            int intTotalCount = 0;
            return GetItemPricings(null, null, null, out intTotalCount);
        }

        public static List<ItemPricing> GetItemPricings(ItemPricingFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemPricings(Filter, null, null, out intTotalCount);
        }

        public static List<ItemPricing> GetItemPricings(ItemPricingFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemPricings(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemPricing> GetItemPricings(ItemPricingFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemPricing> objReturn = null;
            ItemPricing objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemPricing>();

                strSQL = "SELECT * " +
                         "FROM ItemPricing (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                    if (Filter.WebsiteGroupID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteGroupID, "WebsiteGroupID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemPricingID" : Utility.CustomSorting.GetSortExpression(typeof(ItemPricing), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemPricing(objData.Tables[0].Rows[i]);
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

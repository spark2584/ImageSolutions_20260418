using ImageSolutions.Attribute;
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

namespace ImageSolutions.Item
{
    public class ItemDetail : ISBase.BaseClass
    {
        public string ItemDetailID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemDetailID); } }
        public string ItemID { get; set; }
        public string Attribute { get; set; }
        public int? Sort { get; set; }
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

        public string ItemDetailValuesInHTML
        {
            get
            {
                string strReturn = string.Empty;
                if (ItemDetailValues != null)
                {
                    foreach (ItemDetailValue objItemDetailValue in ItemDetailValues)
                    {
                        strReturn += "<li>" + objItemDetailValue.Value + "</li>";
                    }
                }
                return strReturn;
            }
        }

        private List<ItemDetailValue> mItemDetailValues = null;
        public List<ItemDetailValue> ItemDetailValues
        {
            get
            {
                if (mItemDetailValues == null && !string.IsNullOrEmpty(ItemDetailID))
                {
                    ItemDetailValueFilter objFilter = null;

                    try
                    {
                        objFilter = new ItemDetailValueFilter();
                        objFilter.ItemDetailID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemDetailID.SearchString = ItemDetailID;
                        mItemDetailValues = ItemDetailValue.GetItemDetailValues(objFilter);
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
                return mItemDetailValues;
            }
        }

        public ItemDetail()
        {
        }
        public ItemDetail(string ItemDetailID)
        {
            this.ItemDetailID = ItemDetailID;
            Load();
        }
        public ItemDetail(DataRow objRow)
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
                         "FROM ItemDetail (NOLOCK) " +
                         "WHERE ItemDetailID=" + Database.HandleQuote(ItemDetailID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemDetailID=" + ItemDetailID + " is not found");
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


        protected void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("ItemDetailID")) ItemDetailID = Convert.ToString(objRow["ItemDetailID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("Attribute")) Attribute = Convert.ToString(objRow["Attribute"]);
                if (objColumns.Contains("Sort") && objRow["Sort"] != DBNull.Value) Sort = Convert.ToInt32(objRow["Sort"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemDetailID)) throw new Exception("Missing ItemDetailID in the datarow");
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
                if (Attribute == null) throw new Exception("Attribute is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemDetailID already exists");

                dicParam["ItemID"] = ItemID;
                dicParam["Attribute"] = Attribute;
                dicParam["Sort"] = Sort;
                dicParam["CreatedBy"] = CreatedBy;
                ItemDetailID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemDetail"), objConn, objTran).ToString();

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
                if (Attribute == null) throw new Exception("Attribute is required");
                if (IsNew) throw new Exception("Update cannot be performed, ItemDetailID is missing");

                dicParam["ItemID"] = ItemID;
                dicParam["Attribute"] = Attribute;
                dicParam["Sort"] = Sort;
                dicWParam["ItemDetailID"] = ItemDetailID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemDetail"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemDetailID is missing");

                dicDParam["ItemDetailID"] = ItemDetailID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemDetail"), objConn, objTran);
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

        public static ItemDetail GetItemDetail(ItemDetailFilter Filter)
        {
            List<ItemDetail> objItemDetails = null;
            ItemDetail objReturn = null;

            try
            {
                objItemDetails = GetItemDetails(Filter);
                if (objItemDetails != null && objItemDetails.Count >= 1) objReturn = objItemDetails[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemDetails = null;
            }
            return objReturn;
        }

        public static List<ItemDetail> GetItemDetails()
        {
            int intTotalCount = 0;
            return GetItemDetails(null, null, null, out intTotalCount);
        }

        public static List<ItemDetail> GetItemDetails(ItemDetailFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemDetails(Filter, null, null, out intTotalCount);
        }

        public static List<ItemDetail> GetItemDetails(ItemDetailFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemDetails(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemDetail> GetItemDetails(ItemDetailFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemDetail> objReturn = null;
            ItemDetail objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemDetail>();

                strSQL = "SELECT * " +
                         "FROM ItemDetail (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                    if (Filter.Attribute != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Attribute, "Attribute");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemDetailID" : Utility.CustomSorting.GetSortExpression(typeof(ItemDetail), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else strSQL += " ORDER BY ISNULL(Sort,9999) ASC"; 
                
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemDetail(objData.Tables[0].Rows[i]);
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

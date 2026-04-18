using ImageSolutions.User;
using ImageSolutions.Website;
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
    public class SuperceedingItem : ISBase.BaseClass
    {
        public string SuperceedingItemID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(SuperceedingItemID); } }
        public string ItemID { get; set; }
        public string ReplacementItemID { get; set; }
        public int? Sort { get; set; }
        public bool Inactive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        private Item mItem = null;
        public Item Item
        {
            get
            {
                if (mItem == null && !string.IsNullOrEmpty(ItemID))
                {
                    mItem = new Item(ItemID);
                }
                return mItem;
            }
        }

        private Item mReplacementItem = null;
        public Item ReplacementItem
        {
            get
            {
                if (mReplacementItem == null && !string.IsNullOrEmpty(ReplacementItemID))
                {
                    mReplacementItem = new Item(ReplacementItemID);
                }
                return mReplacementItem;
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

        public SuperceedingItem()
        {
        }
        public SuperceedingItem(string SuperceedingItemID)
        {
            this.SuperceedingItemID = SuperceedingItemID;
            Load();
        }
        public SuperceedingItem(DataRow objRow)
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
                         "FROM SuperceedingItem (NOLOCK) " +
                         "WHERE SuperceedingItemID=" + Database.HandleQuote(SuperceedingItemID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("SuperceedingItemID=" + SuperceedingItemID + " is not found");
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

                if (objColumns.Contains("SuperceedingItemID")) SuperceedingItemID = Convert.ToString(objRow["SuperceedingItemID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("ReplacementItemID")) ReplacementItemID = Convert.ToString(objRow["ReplacementItemID"]);
                if (objColumns.Contains("Sort") && objRow["Sort"] != DBNull.Value) Sort = Convert.ToInt32(objRow["Sort"]);
                if (objColumns.Contains("Inactive")) Inactive = Convert.ToBoolean(objRow["Inactive"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(SuperceedingItemID)) throw new Exception("Missing SuperceedingItemID in the datarow");
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
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (string.IsNullOrEmpty(ReplacementItemID)) throw new Exception("ReplacementItemID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, SuperceedingItemID already exists");

                dicParam["ItemID"] = ItemID;
                dicParam["ReplacementItemID"] = ReplacementItemID;
                dicParam["Sort"] = Sort;
                dicParam["Inactive"] = Inactive;
                dicParam["CreatedBy"] = CreatedBy;

                SuperceedingItemID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "SuperceedingItem"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (string.IsNullOrEmpty(ReplacementItemID)) throw new Exception("ReplacementItemID is required");
                if (IsNew) throw new Exception("Update cannot be performed, SuperceedingItemID is missing");

                dicParam["ItemID"] = ItemID;
                dicParam["ReplacementItemID"] = ReplacementItemID;
                dicParam["Sort"] = Sort;
                dicParam["Inactive"] = Inactive;
                dicWParam["SuperceedingItemID"] = SuperceedingItemID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "SuperceedingItem"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, SuperceedingItemID is missing");

                dicDParam["SuperceedingItemID"] = SuperceedingItemID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "SuperceedingItem"), objConn, objTran);
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

        public static SuperceedingItem GetSuperceedingItem(SuperceedingItemFilter Filter)
        {
            List<SuperceedingItem> objSuperceedingItems = null;
            SuperceedingItem objReturn = null;

            try
            {
                objSuperceedingItems = GetSuperceedingItems(Filter);
                if (objSuperceedingItems != null && objSuperceedingItems.Count >= 1) objReturn = objSuperceedingItems[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSuperceedingItems = null;
            }
            return objReturn;
        }

        public static List<SuperceedingItem> GetSuperceedingItems()
        {
            int intTotalCount = 0;
            return GetSuperceedingItems(null, null, null, out intTotalCount);
        }

        public static List<SuperceedingItem> GetSuperceedingItems(SuperceedingItemFilter Filter)
        {
            int intTotalCount = 0;
            return GetSuperceedingItems(Filter, null, null, out intTotalCount);
        }

        public static List<SuperceedingItem> GetSuperceedingItems(SuperceedingItemFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetSuperceedingItems(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<SuperceedingItem> GetSuperceedingItems(SuperceedingItemFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<SuperceedingItem> objReturn = null;
            SuperceedingItem objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<SuperceedingItem>();

                strSQL = "SELECT * " +
                         "FROM SuperceedingItem (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                    if (Filter.ReplacementItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ReplacementItemID, "ReplacementItemID");
                    if (Filter.Sort != null) strSQL += "AND Sort=" + Database.HandleQuote(Convert.ToInt32(Filter.Sort).ToString());

                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Sort" : Utility.CustomSorting.GetSortExpression(typeof(SuperceedingItem), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else strSQL += " ORDER BY Sort";

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new SuperceedingItem(objData.Tables[0].Rows[i]);
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

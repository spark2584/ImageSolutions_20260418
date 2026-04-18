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
    public class ItemSelectableLogoExcludeAttribute : ISBase.BaseClass
    {
        public string ItemSelectableLogoExcludeAttributeID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemSelectableLogoExcludeAttributeID); } }
        public string ItemSelectableLogoID { get; set; }
        public string AttributeValueID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        private ItemSelectableLogo mItemSelectableLogo = null;
        public ItemSelectableLogo ItemSelectableLogo
        {
            get
            {
                if (mItemSelectableLogo == null && !string.IsNullOrEmpty(ItemSelectableLogoID))
                {
                    mItemSelectableLogo = new ItemSelectableLogo(ItemSelectableLogoID);
                }
                return mItemSelectableLogo;
            }
        }

        private ImageSolutions.Attribute.AttributeValue mAttributeValue = null;
        public ImageSolutions.Attribute.AttributeValue AttributeValue
        {
            get
            {
                if (mAttributeValue == null && !string.IsNullOrEmpty(AttributeValueID))
                {
                    mAttributeValue = new ImageSolutions.Attribute.AttributeValue(AttributeValueID);
                }
                return mAttributeValue;
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

        public ItemSelectableLogoExcludeAttribute()
        {
        }
        public ItemSelectableLogoExcludeAttribute(string ItemSelectableLogoExcludeAttributeID)
        {
            this.ItemSelectableLogoExcludeAttributeID = ItemSelectableLogoExcludeAttributeID;
            Load();
        }
        public ItemSelectableLogoExcludeAttribute(DataRow objRow)
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
                         "FROM ItemSelectableLogoExcludeAttribute (NOLOCK) " +
                         "WHERE ItemSelectableLogoExcludeAttributeID=" + Database.HandleQuote(ItemSelectableLogoExcludeAttributeID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemSelectableLogoExcludeAttributeID=" + ItemSelectableLogoExcludeAttributeID + " is not found");
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

                if (objColumns.Contains("ItemSelectableLogoExcludeAttributeID")) ItemSelectableLogoExcludeAttributeID = Convert.ToString(objRow["ItemSelectableLogoExcludeAttributeID"]);
                if (objColumns.Contains("ItemSelectableLogoID")) ItemSelectableLogoID = Convert.ToString(objRow["ItemSelectableLogoID"]);
                if (objColumns.Contains("AttributeValueID")) AttributeValueID = Convert.ToString(objRow["AttributeValueID"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemSelectableLogoExcludeAttributeID)) throw new Exception("Missing ItemSelectableLogoExcludeAttributeID in the datarow");
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
                if (string.IsNullOrEmpty(ItemSelectableLogoID)) throw new Exception("ItemSelectableLogoID is required");
                if (ObjectAlreadyExists()) throw new Exception("This record already exsists");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemSelectableLogoExcludeAttributeID already exists");

                dicParam["ItemSelectableLogoID"] = ItemSelectableLogoID;
                dicParam["AttributeValueID"] = AttributeValueID;
                dicParam["CreatedBy"] = CreatedBy;

                ItemSelectableLogoExcludeAttributeID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemSelectableLogoExcludeAttribute"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(ItemSelectableLogoID)) throw new Exception("ItemSelectableLogoID is required");
                if (ObjectAlreadyExists()) throw new Exception("This record already exsists");
                if (IsNew) throw new Exception("Update cannot be performed, ItemSelectableLogoExcludeAttributeID is missing");


                dicParam["ItemSelectableLogoID"] = ItemSelectableLogoID;
                dicParam["AttributeValueID"] = AttributeValueID;
                dicWParam["ItemSelectableLogoExcludeAttributeID"] = ItemSelectableLogoExcludeAttributeID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemSelectableLogoExcludeAttribute"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemSelectableLogoExcludeAttributeID is missing");

                dicDParam["ItemSelectableLogoExcludeAttributeID"] = ItemSelectableLogoExcludeAttributeID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemSelectableLogoExcludeAttribute"), objConn, objTran);
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
                        "FROM ItemSelectableLogoExcludeAttribute (NOLOCK) p " +
                        "WHERE " +
                        "(" +
                        "  (p.ItemSelectableLogoID=" + Database.HandleQuote(ItemSelectableLogoID) + " AND p.AttributeValueID=" + Database.HandleQuote(AttributeValueID) + ")" +
                        ") ";

            if (!string.IsNullOrEmpty(ItemSelectableLogoExcludeAttributeID)) strSQL += "AND p.ItemSelectableLogoExcludeAttributeID<>" + Database.HandleQuote(ItemSelectableLogoExcludeAttributeID);
            return Database.HasRows(strSQL);
        }

        public static ItemSelectableLogoExcludeAttribute GetItemSelectableLogoExcludeAttribute(ItemSelectableLogoExcludeAttributeFilter Filter)
        {
            List<ItemSelectableLogoExcludeAttribute> objItemSelectableLogoExcludeAttributes = null;
            ItemSelectableLogoExcludeAttribute objReturn = null;

            try
            {
                objItemSelectableLogoExcludeAttributes = GetItemSelectableLogoExcludeAttributes(Filter);
                if (objItemSelectableLogoExcludeAttributes != null && objItemSelectableLogoExcludeAttributes.Count >= 1) objReturn = objItemSelectableLogoExcludeAttributes[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemSelectableLogoExcludeAttributes = null;
            }
            return objReturn;
        }

        public static List<ItemSelectableLogoExcludeAttribute> GetItemSelectableLogoExcludeAttributes()
        {
            int intTotalCount = 0;
            return GetItemSelectableLogoExcludeAttributes(null, null, null, out intTotalCount);
        }

        public static List<ItemSelectableLogoExcludeAttribute> GetItemSelectableLogoExcludeAttributes(ItemSelectableLogoExcludeAttributeFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemSelectableLogoExcludeAttributes(Filter, null, null, out intTotalCount);
        }

        public static List<ItemSelectableLogoExcludeAttribute> GetItemSelectableLogoExcludeAttributes(ItemSelectableLogoExcludeAttributeFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemSelectableLogoExcludeAttributes(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemSelectableLogoExcludeAttribute> GetItemSelectableLogoExcludeAttributes(ItemSelectableLogoExcludeAttributeFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemSelectableLogoExcludeAttribute> objReturn = null;
            ItemSelectableLogoExcludeAttribute objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemSelectableLogoExcludeAttribute>();

                strSQL = "SELECT * " +
                         "FROM ItemSelectableLogoExcludeAttribute (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ItemSelectableLogoID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemSelectableLogoID, "ItemSelectableLogoID");
                    if (Filter.AttributeValueID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.AttributeValueID, "AttributeValueID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Sort" : Utility.CustomSorting.GetSortExpression(typeof(ItemSelectableLogoExcludeAttribute), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemSelectableLogoExcludeAttribute(objData.Tables[0].Rows[i]);
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

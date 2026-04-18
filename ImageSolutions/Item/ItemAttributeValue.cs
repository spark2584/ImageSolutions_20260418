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
    public class ItemAttributeValue : ISBase.BaseClass
    {
        public string ItemAttributeValueID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemAttributeValueID); } }
        public string AttributeValueID { get; set; }
        public string ItemID { get; set; }
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

        private ImageSolutions.Attribute.AttributeValue mAttributeValue = null;
        public ImageSolutions.Attribute.AttributeValue AttributeValue
        {
            get
            {
                if (mAttributeValue == null && !string.IsNullOrEmpty(AttributeValueID))
                {
                    try
                    {
                        mAttributeValue = new ImageSolutions.Attribute.AttributeValue(AttributeValueID);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                    }
                }
                return mAttributeValue;
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
        public ItemAttributeValue()
        {
        }
        public ItemAttributeValue(string ItemAttributeValueID)
        {
            this.ItemAttributeValueID = ItemAttributeValueID;
            Load();
        }
        public ItemAttributeValue(DataRow objRow)
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
                         "FROM ItemAttributeValue (NOLOCK) " +
                         "WHERE ItemAttributeValueID=" + Database.HandleQuote(ItemAttributeValueID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemAttributeValueID=" + ItemAttributeValueID + " is not found");
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

                if (objColumns.Contains("ItemAttributeValueID")) ItemAttributeValueID = Convert.ToString(objRow["ItemAttributeValueID"]);
                if (objColumns.Contains("AttributeValueID")) AttributeValueID = Convert.ToString(objRow["AttributeValueID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemAttributeValueID)) throw new Exception("Missing ItemAttributeValueID in the datarow");
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
                if (AttributeValueID == null) throw new Exception("AttributeValueID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemAttributeValueID already exists");

                dicParam["AttributeValueID"] = AttributeValueID;
                dicParam["ItemID"] = ItemID;
                dicParam["CreatedBy"] = CreatedBy;
                ItemAttributeValueID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemAttributeValue"), objConn, objTran).ToString();

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
                if (AttributeValueID == null) throw new Exception("AttributeValueID is required");
                if (IsNew) throw new Exception("Update cannot be performed, ItemAttributeValueID is missing");


                dicParam["AttributeValueID"] = AttributeValueID;
                dicParam["ItemID"] = ItemID;

                dicWParam["ItemAttributeValueID"] = ItemAttributeValueID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemAttributeValue"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemAttributeValueID is missing");

                dicDParam["ItemAttributeValueID"] = ItemAttributeValueID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemAttributeValue"), objConn, objTran);
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

        public static ItemAttributeValue GetItemAttributeValue(ItemAttributeValueFilter Filter)
        {
            List<ItemAttributeValue> objItemAttributeValues = null;
            ItemAttributeValue objReturn = null;

            try
            {
                objItemAttributeValues = GetItemAttributeValues(Filter);
                if (objItemAttributeValues != null && objItemAttributeValues.Count >= 1) objReturn = objItemAttributeValues[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemAttributeValues = null;
            }
            return objReturn;
        }

        public static List<ItemAttributeValue> GetItemAttributeValues()
        {
            int intTotalCount = 0;
            return GetItemAttributeValues(null, null, null, out intTotalCount);
        }

        public static List<ItemAttributeValue> GetItemAttributeValues(ItemAttributeValueFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemAttributeValues(Filter, null, null, out intTotalCount);
        }

        public static List<ItemAttributeValue> GetItemAttributeValues(ItemAttributeValueFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemAttributeValues(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemAttributeValue> GetItemAttributeValues(ItemAttributeValueFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemAttributeValue> objReturn = null;
            ItemAttributeValue objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemAttributeValue>();

                strSQL = "SELECT * " +
                         "FROM ItemAttributeValue (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.AttributeValueID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.AttributeValueID, "AttributeValueID");
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemAttributeValueID" : Utility.CustomSorting.GetSortExpression(typeof(ItemAttributeValue), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemAttributeValue(objData.Tables[0].Rows[i]);
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

using ImageSolutions.Account;
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

namespace ImageSolutions.Attribute
{
    public class Attribute : ISBase.BaseClass
    {
        public string AttributeID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(AttributeID); } }
        public string AttributeName { get; set; }
        public string ItemID { get; set; }
        public int Sort { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        private ImageSolutions.Item.Item mItem = null;
        public ImageSolutions.Item.Item Item
        {
            get
            {
                if (mItem == null && !string.IsNullOrEmpty(ItemID))
                {
                    try
                    {
                        mItem = new ImageSolutions.Item.Item(ItemID);
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

        private List<AttributeValue> mAttributeValues = null;
        public List<AttributeValue> AttributeValues
        {
            get
            {
                if (mAttributeValues == null && !string.IsNullOrEmpty(AttributeID))
                {
                    AttributeValueFilter objFilter = null;

                    try
                    {
                        objFilter = new AttributeValueFilter();
                        objFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.AttributeID.SearchString = AttributeID;
                        mAttributeValues = AttributeValue.GetAttributeValues(objFilter);
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
                return mAttributeValues;
            }
        }

        public Attribute()
        {
        }
        public Attribute(string AttributeID)
        {
            this.AttributeID = AttributeID;
            Load();
        }
        public Attribute(DataRow objRow)
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
                         "FROM Attribute (NOLOCK) " +
                         "WHERE AttributeID=" + Database.HandleQuote(AttributeID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("AttributeID=" + AttributeID + " is not found");
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

                if (objColumns.Contains("AttributeID")) AttributeID = Convert.ToString(objRow["AttributeID"]);
                if (objColumns.Contains("AttributeName")) AttributeName = Convert.ToString(objRow["AttributeName"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("Sort") && objRow["Sort"] != DBNull.Value) Sort = Convert.ToInt32(objRow["Sort"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(AttributeID)) throw new Exception("Missing AttributeID in the datarow");
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
                if (string.IsNullOrEmpty(AttributeName)) throw new Exception("AttributeName is required");
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, AttributeID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["AttributeName"] = AttributeName;
                dicParam["ItemID"] = ItemID;
                dicParam["Sort"] = Sort;
                dicParam["CreatedBy"] = CreatedBy;

                AttributeID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Attribute"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(AttributeName)) throw new Exception("AttributeName is required");
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (IsNew) throw new Exception("Update cannot be performed, AttributeID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["AttributeName"] = AttributeName;
                dicParam["ItemID"] = ItemID;
                dicParam["Sort"] = Sort;


                dicWParam["AttributeID"] = AttributeID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Attribute"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, AttributeID is missing");

                foreach (AttributeValue _AttributeValue in AttributeValues)
                {
                    _AttributeValue.Delete(objConn, objTran);
                }

                dicDParam["AttributeID"] = AttributeID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Attribute"), objConn, objTran);
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
                    "FROM Attribute (NOLOCK) p " +
                    "WHERE " +
                    "(" +
                    "  (p.AttributeName=" + Database.HandleQuote(AttributeName) + " AND p.ItemID=" + Database.HandleQuote(ItemID) + ")" +
                    ") ";


            if (!string.IsNullOrEmpty(AttributeID)) strSQL += "AND p.AttributeID<>" + Database.HandleQuote(AttributeID);
            return Database.HasRows(strSQL);
        }

        public static Attribute GetAttribute(AttributeFilter Filter)
        {
            List<Attribute> objAttributes = null;
            Attribute objReturn = null;

            try
            {
                objAttributes = GetAttributes(Filter);
                if (objAttributes != null && objAttributes.Count >= 1) objReturn = objAttributes[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAttributes = null;
            }
            return objReturn;
        }

        public static List<Attribute> GetAttributes()
        {
            int intTotalCount = 0;
            return GetAttributes(null, null, null, out intTotalCount);
        }

        public static List<Attribute> GetAttributes(AttributeFilter Filter)
        {
            int intTotalCount = 0;
            return GetAttributes(Filter, null, null, out intTotalCount);
        }

        public static List<Attribute> GetAttributes(AttributeFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAttributes(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Attribute> GetAttributes(AttributeFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Attribute> objReturn = null;
            Attribute objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Attribute>();

                strSQL = "SELECT * " +
                         "FROM Attribute (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                    if (Filter.AttributeName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.AttributeName, "AttributeName");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "AttributeID" : Utility.CustomSorting.GetSortExpression(typeof(Attribute), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else if (string.IsNullOrEmpty(SortExpression))
                {
                    strSQL = string.Format("{0} {1}", strSQL, "Order By Sort");
                }

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Attribute(objData.Tables[0].Rows[i]);
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

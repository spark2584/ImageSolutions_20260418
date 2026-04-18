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
    public class AttributeValue: ISBase.BaseClass
    {
        public string AttributeValueID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(AttributeValueID); } }
        public string AttributeID { get; set; }
        public string Value { get; set; }
        public string Abbreviation { get; set; }
        public string BackgroundColor { get; set; }
        public int? Sort { get; set; }
        public bool IsDefault { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        private Attribute mAttribute = null;
        public Attribute Attribute
        {
            get
            {
                if (mAttribute == null && !string.IsNullOrEmpty(AttributeID))
                {
                    try
                    {
                        mAttribute = new Attribute(AttributeID);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                    }
                }
                return mAttribute;
            }
        }
        private List<Item.ItemAttributeValue> mItemAttributeValues = null;
        public List<Item.ItemAttributeValue> ItemAttributeValues
        {
            get
            {
                if (mItemAttributeValues == null && !string.IsNullOrEmpty(AttributeValueID))
                {
                    Item.ItemAttributeValueFilter objFilter = null;

                    try
                    {
                        objFilter = new Item.ItemAttributeValueFilter();
                        objFilter.AttributeValueID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.AttributeValueID.SearchString = AttributeValueID;
                        mItemAttributeValues = Item.ItemAttributeValue.GetItemAttributeValues(objFilter);
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
                return mItemAttributeValues;
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

        public AttributeValue()
        {
        }
        public AttributeValue(string AttributeValueID)
        {
            this.AttributeValueID = AttributeValueID;
            Load();
        }
        public AttributeValue(DataRow objRow)
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
                         "FROM AttributeValue (NOLOCK) " +
                         "WHERE AttributeValueID=" + Database.HandleQuote(AttributeValueID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("AttributeValueID=" + AttributeValueID + " is not found");
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

                if (objColumns.Contains("AttributeValueID")) AttributeValueID = Convert.ToString(objRow["AttributeValueID"]);
                if (objColumns.Contains("AttributeID")) AttributeID = Convert.ToString(objRow["AttributeID"]);
                if (objColumns.Contains("Value")) Value = Convert.ToString(objRow["Value"]);
                if (objColumns.Contains("Abbreviation")) Abbreviation = Convert.ToString(objRow["Abbreviation"]);
                if (objColumns.Contains("BackgroundColor")) BackgroundColor = Convert.ToString(objRow["BackgroundColor"]);
                if (objColumns.Contains("Sort") && objRow["Sort"] != DBNull.Value) Sort = Convert.ToInt32(objRow["Sort"]);
                if (objColumns.Contains("IsDefault") && objRow["IsDefault"] != DBNull.Value) IsDefault = Convert.ToBoolean(objRow["IsDefault"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(AttributeValueID)) throw new Exception("Missing AttributeValueID in the datarow");
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
                if (Value == null) throw new Exception("AttributeValueName is required");
                if (!IsNew) throw new Exception("Create cannot be performed, AttributeValueID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["AttributeID"] = AttributeID;
                dicParam["Value"] = Value;
                dicParam["Abbreviation"] = Abbreviation;
                dicParam["BackgroundColor"] = BackgroundColor;
                dicParam["Sort"] = Sort;
                dicParam["IsDefault"] = IsDefault;
                dicParam["CreatedBy"] = CreatedBy;
                AttributeValueID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AttributeValue"), objConn, objTran).ToString();

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
                if (Value == null) throw new Exception("AttributeValueName is required");
                if (IsNew) throw new Exception("Update cannot be performed, AttributeValueID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["AttributeID"] = AttributeID;
                dicParam["Value"] = Value;
                dicParam["Abbreviation"] = Abbreviation;
                dicParam["BackgroundColor"] = BackgroundColor;
                dicParam["Sort"] = Sort;
                dicParam["IsDefault"] = IsDefault;

                dicWParam["AttributeValueID"] = AttributeValueID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AttributeValue"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, AttributeValueID is missing");
                if (ItemAttributeValues != null && ItemAttributeValues.Count > 0) throw new Exception("Delete cannot be performed, Item has been assigned to the attribute");

                dicDParam["AttributeValueID"] = AttributeValueID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AttributeValue"), objConn, objTran);
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
                    "FROM AttributeValue (NOLOCK) p " +
                    "WHERE " +
                    "(" +
                    "  (p.Value=" + Database.HandleQuote(Value) + " AND p.AttributeID=" + Database.HandleQuote(AttributeID) + ")" +
                    "  OR " +
                    "  (p.Abbreviation=" + Database.HandleQuote(Abbreviation) + " AND p.AttributeID=" + Database.HandleQuote(AttributeID) + ")" +
                    ") ";


            if (!string.IsNullOrEmpty(AttributeValueID)) strSQL += "AND p.AttributeValueID<>" + Database.HandleQuote(AttributeValueID);
            return Database.HasRows(strSQL);
        }

        public static AttributeValue GetAttributeValue(AttributeValueFilter Filter)
        {
            List<AttributeValue> objAttributeValues = null;
            AttributeValue objReturn = null;

            try
            {
                objAttributeValues = GetAttributeValues(Filter);
                if (objAttributeValues != null && objAttributeValues.Count >= 1) objReturn = objAttributeValues[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAttributeValues = null;
            }
            return objReturn;
        }

        public static List<AttributeValue> GetAttributeValues()
        {
            int intTotalCount = 0;
            return GetAttributeValues(null, null, null, out intTotalCount);
        }

        public static List<AttributeValue> GetAttributeValues(AttributeValueFilter Filter)
        {
            int intTotalCount = 0;
            return GetAttributeValues(Filter, null, null, out intTotalCount);
        }

        public static List<AttributeValue> GetAttributeValues(AttributeValueFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAttributeValues(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AttributeValue> GetAttributeValues(AttributeValueFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AttributeValue> objReturn = null;
            AttributeValue objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AttributeValue>();

                strSQL = "SELECT * " +
                         "FROM AttributeValue (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.AttributeID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.AttributeID, "AttributeID");
                    if (Filter.Value != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Value, "Value");
                    if (Filter.BackgroundColor != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.BackgroundColor, "BackgroundColor");
                    if (Filter.Sort != null) strSQL += "AND Sort=" + Database.HandleQuote(Convert.ToInt32(Filter.Sort).ToString());
                    if (Filter.IsDefault != null) strSQL += "AND IsDefault=" + Database.HandleQuote(Convert.ToInt32(Filter.IsDefault).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "AttributeValueID" : Utility.CustomSorting.GetSortExpression(typeof(AttributeValue), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else strSQL += " ORDER BY ISNULL(Sort,9999) ASC";
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AttributeValue(objData.Tables[0].Rows[i]);
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

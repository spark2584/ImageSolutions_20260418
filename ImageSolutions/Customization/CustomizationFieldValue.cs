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

namespace ImageSolutions.Customization
{
    public class CustomizationFieldValue : ISBase.BaseClass
    {
        public string CustomizationFieldValueID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(CustomizationFieldValueID); } }
        public string CustomizationFieldID { get; set; }
        public string Value { get; set; }
        public string Query { get; set; }

        private CustomizationField mCustomizationField = null;
        public CustomizationField CustomizationField
        {
            get
            {
                if (mCustomizationField == null && !string.IsNullOrEmpty(CustomizationFieldID))
                {
                    try
                    {
                        mCustomizationField = new CustomizationField(CustomizationFieldID);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                    }
                }
                return mCustomizationField;
            }
        }
        public CustomizationFieldValue()
        {
        }
        public CustomizationFieldValue(string CustomizationFieldValueID)
        {
            this.CustomizationFieldValueID = CustomizationFieldValueID;
            Load();
        }
        public CustomizationFieldValue(DataRow objRow)
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
                         "FROM CustomizationFieldValue (NOLOCK) " +
                         "WHERE CustomizationFieldValueID=" + Database.HandleQuote(CustomizationFieldValueID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("CustomizationFieldValueID=" + CustomizationFieldValueID + " is not found");
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

                if (objColumns.Contains("CustomizationFieldValueID")) CustomizationFieldValueID = Convert.ToString(objRow["CustomizationFieldValueID"]);
                if (objColumns.Contains("CustomizationFieldID")) CustomizationFieldID = Convert.ToString(objRow["CustomizationFieldID"]);
                if (objColumns.Contains("Value")) Value = Convert.ToString(objRow["Value"]);
                if (objColumns.Contains("Query")) Query = Convert.ToString(objRow["Query"]);

                if (string.IsNullOrEmpty(CustomizationFieldValueID)) throw new Exception("Missing CustomizationFieldValueID in the datarow");
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
                if (string.IsNullOrEmpty(CustomizationFieldID)) throw new Exception("CustomizationFieldID is required");
                if (string.IsNullOrEmpty(Value)) throw new Exception("Value is required");

                if (!IsNew) throw new Exception("Create cannot be performed, CustomizationFieldValueID already exists");

                dicParam["CustomizationFieldID"] = CustomizationFieldID;
                dicParam["Value"] = Value;
                dicParam["Query"] = Query;

                CustomizationFieldValueID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "CustomizationFieldValue"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(CustomizationFieldID)) throw new Exception("CustomizationFieldID is required");
                if (string.IsNullOrEmpty(Value)) throw new Exception("Value is required");

                if (IsNew) throw new Exception("Update cannot be performed, CustomizationFieldValueID is missing");

                dicParam["CustomizationFieldID"] = CustomizationFieldID;
                dicParam["Value"] = Value;
                dicParam["Query"] = Query;

                dicWParam["CustomizationFieldValueID"] = CustomizationFieldValueID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "CustomizationFieldValue"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, CustomizationFieldValueID is missing");

                dicDParam["CustomizationFieldValueID"] = CustomizationFieldValueID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "CustomizationFieldValue"), objConn, objTran);
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

        public static CustomizationFieldValue GetCustomizationFieldValue(CustomizationFieldValueFilter Filter)
        {
            List<CustomizationFieldValue> objCustomizationFieldValues = null;
            CustomizationFieldValue objReturn = null;

            try
            {
                objCustomizationFieldValues = GetCustomizationFieldValues(Filter);
                if (objCustomizationFieldValues != null && objCustomizationFieldValues.Count >= 1) objReturn = objCustomizationFieldValues[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomizationFieldValues = null;
            }
            return objReturn;
        }

        public static List<CustomizationFieldValue> GetCustomizationFieldValues()
        {
            int intTotalCount = 0;
            return GetCustomizationFieldValues(null, null, null, out intTotalCount);
        }

        public static List<CustomizationFieldValue> GetCustomizationFieldValues(CustomizationFieldValueFilter Filter)
        {
            int intTotalCount = 0;
            return GetCustomizationFieldValues(Filter, null, null, out intTotalCount);
        }

        public static List<CustomizationFieldValue> GetCustomizationFieldValues(CustomizationFieldValueFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetCustomizationFieldValues(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<CustomizationFieldValue> GetCustomizationFieldValues(CustomizationFieldValueFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<CustomizationFieldValue> objReturn = null;
            CustomizationFieldValue objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<CustomizationFieldValue>();

                strSQL = "SELECT * " +
                         "FROM CustomizationFieldValue (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.CustomizationFieldID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomizationFieldID, "CustomizationFieldID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "CustomizationFieldValueID" : Utility.CustomSorting.GetSortExpression(typeof(CustomizationFieldValue), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new CustomizationFieldValue(objData.Tables[0].Rows[i]);
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

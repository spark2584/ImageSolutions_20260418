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

namespace ImageSolutions.Custom
{
    public class CustomValue : ISBase.BaseClass
    {
        public string CustomValueID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(CustomValueID); } }
        public string CustomFieldID { get; set; }
        public string Value { get; set; }
        public string SalesOrderID { get; set; }
        public string UserWebsiteID { get; set; }

        private CustomField mCustomField = null;
        public CustomField CustomField
        {
            get
            {
                if (mCustomField == null && !string.IsNullOrEmpty(CustomFieldID))
                {
                    mCustomField = new CustomField(CustomFieldID);
                }
                return mCustomField;
            }
        }

        public CustomValue()
        {
        }
        public CustomValue(string CustomValueID)
        {
            this.CustomValueID = CustomValueID;
            Load();
        }
        public CustomValue(DataRow objRow)
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
                         "FROM CustomValue (NOLOCK) " +
                         "WHERE CustomValueID=" + Database.HandleQuote(CustomValueID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("CustomValueID=" + CustomValueID + " is not found");
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

                if (objColumns.Contains("CustomValueID")) CustomValueID = Convert.ToString(objRow["CustomValueID"]);
                if (objColumns.Contains("CustomFieldID")) CustomFieldID = Convert.ToString(objRow["CustomFieldID"]);
                if (objColumns.Contains("Value")) Value = Convert.ToString(objRow["Value"]);
                if (objColumns.Contains("SalesOrderID")) SalesOrderID = Convert.ToString(objRow["SalesOrderID"]);
                if (objColumns.Contains("UserWebsiteID")) UserWebsiteID = Convert.ToString(objRow["UserWebsiteID"]);

                if (string.IsNullOrEmpty(CustomValueID)) throw new Exception("Missing CustomValueID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, CustomValueID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CustomFieldID"] = CustomFieldID;
                dicParam["Value"] = Value;
                dicParam["SalesOrderID"] = SalesOrderID;
                dicParam["UserWebsiteID"] = UserWebsiteID;

                CustomValueID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "CustomValue"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, CustomValueID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CustomFieldID"] = CustomFieldID;
                dicParam["Value"] = Value;
                dicParam["SalesOrderID"] = SalesOrderID;
                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicWParam["CustomValueID"] = CustomValueID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "CustomValue"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, CustomValueID is missing");
                dicDParam["CustomValueID"] = CustomValueID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "CustomValue"), objConn, objTran);
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

        private bool ObjectAlreadyExists()
        {
            return false;
        }

        public static CustomValue GetCustomValue(CustomValueFilter Filter)
        {
            List<CustomValue> objCustomValues = null;
            CustomValue objReturn = null;

            try
            {
                objCustomValues = GetCustomValues(Filter);
                if (objCustomValues != null && objCustomValues.Count >= 1) objReturn = objCustomValues[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomValues = null;
            }
            return objReturn;
        }

        public static List<CustomValue> GetCustomValues()
        {
            int intTotalCount = 0;
            return GetCustomValues(null, null, null, out intTotalCount);
        }

        public static List<CustomValue> GetCustomValues(CustomValueFilter Filter)
        {
            int intTotalCount = 0;
            return GetCustomValues(Filter, null, null, out intTotalCount);
        }

        public static List<CustomValue> GetCustomValues(CustomValueFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetCustomValues(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<CustomValue> GetCustomValues(CustomValueFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<CustomValue> objReturn = null;
            CustomValue objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<CustomValue>();

                strSQL = "SELECT * " +
                         "FROM CustomValue (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.CustomFieldID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomFieldID, "CustomFieldID");
                    if (Filter.SalesOrderID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderID, "SalesOrderID");
                    if (Filter.UserWebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserWebsiteID, "UserWebsiteID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Sort" : Utility.CustomSorting.GetSortExpression(typeof(CustomValue), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new CustomValue(objData.Tables[0].Rows[i]);
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

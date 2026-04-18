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
    public class CustomValueList : ISBase.BaseClass
    {
        public string CustomValueListID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(CustomValueListID); } }
        public string CustomFieldID { get; set; }
        public string Value { get; set; }
        public string FilterAccountID { get; set; }
        public bool Inactive { get; set; }        

        public CustomValueList()
        {
        }
        public CustomValueList(string CustomValueListID)
        {
            this.CustomValueListID = CustomValueListID;
            Load();
        }
        public CustomValueList(DataRow objRow)
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
                         "FROM CustomValueList (NOLOCK) " +
                         "WHERE CustomValueListID=" + Database.HandleQuote(CustomValueListID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("CustomValueListID=" + CustomValueListID + " is not found");
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

                if (objColumns.Contains("CustomValueListID")) CustomValueListID = Convert.ToString(objRow["CustomValueListID"]);
                if (objColumns.Contains("CustomFieldID")) CustomFieldID = Convert.ToString(objRow["CustomFieldID"]);
                if (objColumns.Contains("Value")) Value = Convert.ToString(objRow["Value"]);
                if (objColumns.Contains("FilterAccountID")) FilterAccountID = Convert.ToString(objRow["FilterAccountID"]);
                if (objColumns.Contains("Inactive")) Inactive = Convert.ToBoolean(objRow["Inactive"]);

                if (string.IsNullOrEmpty(CustomValueListID)) throw new Exception("Missing CustomValueListID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, CustomValueListID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CustomFieldID"] = CustomFieldID;
                dicParam["Value"] = Value;
                dicParam["FilterAccountID"] = FilterAccountID;
                dicParam["Inactive"] = Inactive;

                CustomValueListID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "CustomValueList"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, CustomValueListID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CustomFieldID"] = CustomFieldID;
                dicParam["Value"] = Value;
                dicParam["FilterAccountID"] = FilterAccountID;
                dicParam["Inactive"] = Inactive;
                dicWParam["CustomValueListID"] = CustomValueListID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "CustomValueList"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, CustomValueListID is missing");
                dicDParam["CustomValueListID"] = CustomValueListID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "CustomValueList"), objConn, objTran);
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

        public static CustomValueList GetCustomValueList(CustomValueListFilter Filter)
        {
            List<CustomValueList> objCustomValueLists = null;
            CustomValueList objReturn = null;

            try
            {
                objCustomValueLists = GetCustomValueLists(Filter);
                if (objCustomValueLists != null && objCustomValueLists.Count >= 1) objReturn = objCustomValueLists[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomValueLists = null;
            }
            return objReturn;
        }

        public static List<CustomValueList> GetCustomValueLists()
        {
            int intTotalCount = 0;
            return GetCustomValueLists(null, null, null, out intTotalCount);
        }

        public static List<CustomValueList> GetCustomValueLists(CustomValueListFilter Filter)
        {
            int intTotalCount = 0;
            return GetCustomValueLists(Filter, null, null, out intTotalCount);
        }

        public static List<CustomValueList> GetCustomValueLists(CustomValueListFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetCustomValueLists(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<CustomValueList> GetCustomValueLists(CustomValueListFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<CustomValueList> objReturn = null;
            CustomValueList objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<CustomValueList>();

                strSQL = "SELECT * " +
                         "FROM CustomValueList (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.CustomFieldID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomFieldID, "CustomFieldID");
                    if (Filter.FilterAccountID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.FilterAccountID, "FilterAccountID");
                    if (Filter.Inactive != null) strSQL += "AND Inactive=" + Database.HandleQuote(Convert.ToBoolean(Filter.Inactive).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Sort" : Utility.CustomSorting.GetSortExpression(typeof(CustomValueList), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else strSQL += " ORDER BY Value";

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new CustomValueList(objData.Tables[0].Rows[i]);
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

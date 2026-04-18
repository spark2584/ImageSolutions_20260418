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
    public class CustomListValue : ISBase.BaseClass
    {
        public string CustomListValueID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(CustomListValueID); } }
        public string CustomListID { get; set; }
        public string ListValue { get; set; }
        public int? Sort { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        private CustomList mCustomList = null;
        public CustomList CustomList
        {
            get
            {
                if (mCustomList == null && !string.IsNullOrEmpty(CustomListID))
                {
                    mCustomList = new CustomList(CustomListID);
                }
                return mCustomList;
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

        public CustomListValue()
        {
        }
        public CustomListValue(string CustomListValueID)
        {
            this.CustomListValueID = CustomListValueID;
            Load();
        }
        public CustomListValue(DataRow objRow)
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
                         "FROM CustomListValue (NOLOCK) " +
                         "WHERE CustomListValueID=" + Database.HandleQuote(CustomListValueID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("CustomListValueID=" + CustomListValueID + " is not found");
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

                if (objColumns.Contains("CustomListValueID")) CustomListValueID = Convert.ToString(objRow["CustomListValueID"]);
                if (objColumns.Contains("CustomListID")) CustomListID = Convert.ToString(objRow["CustomListID"]);
                if (objColumns.Contains("ListValue")) ListValue = Convert.ToString(objRow["ListValue"]);
                if (objColumns.Contains("Sort") && objRow["Sort"] != DBNull.Value) Sort = Convert.ToInt32(objRow["Sort"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(CustomListValueID)) throw new Exception("Missing CustomListValueID in the datarow");
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
                if (CustomListID == null) throw new Exception("CustomListID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, CustomListValueID already exists");

                dicParam["CustomListID"] = CustomListID;
                dicParam["ListValue"] = ListValue;
                dicParam["Sort"] = Sort;
                dicParam["CreatedBy"] = CreatedBy;

                CustomListValueID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "CustomListValue"), objConn, objTran).ToString();

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
                if (CustomListID == null) throw new Exception("CustomListID is required");
                if (IsNew) throw new Exception("Update cannot be performed, CustomListValueID is missing");

                dicParam["CustomListID"] = CustomListID;
                dicParam["ListValue"] = ListValue;
                dicParam["Sort"] = Sort;
                dicWParam["CustomListValueID"] = CustomListValueID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "CustomListValue"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, CustomListValueID is missing");

                dicDParam["CustomListValueID"] = CustomListValueID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "CustomListValue"), objConn, objTran);
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

        public static CustomListValue GetCustomListValue(CustomListValueFilter Filter)
        {
            List<CustomListValue> objCustomListValues = null;
            CustomListValue objReturn = null;

            try
            {
                objCustomListValues = GetCustomListValues(Filter);
                if (objCustomListValues != null && objCustomListValues.Count >= 1) objReturn = objCustomListValues[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomListValues = null;
            }
            return objReturn;
        }

        public static List<CustomListValue> GetCustomListValues()
        {
            int intTotalCount = 0;
            return GetCustomListValues(null, null, null, out intTotalCount);
        }

        public static List<CustomListValue> GetCustomListValues(CustomListValueFilter Filter)
        {
            int intTotalCount = 0;
            return GetCustomListValues(Filter, null, null, out intTotalCount);
        }

        public static List<CustomListValue> GetCustomListValues(CustomListValueFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetCustomListValues(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<CustomListValue> GetCustomListValues(CustomListValueFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<CustomListValue> objReturn = null;
            CustomListValue objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<CustomListValue>();

                strSQL = "SELECT * " +
                         "FROM CustomListValue (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.CustomListID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomListID, "CustomListID");
                    if (Filter.ListValue != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ListValue, "ListValue");
                    if (Filter.Sort != null) strSQL += "AND Sort=" + Database.HandleQuote(Convert.ToInt32(Filter.Sort).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Sort" : Utility.CustomSorting.GetSortExpression(typeof(CustomListValue), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else strSQL += " ORDER BY Sort";

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new CustomListValue(objData.Tables[0].Rows[i]);
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

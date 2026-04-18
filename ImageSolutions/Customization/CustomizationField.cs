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
    public class CustomizationField : ISBase.BaseClass
    {
        public string CustomizationFieldID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(CustomizationFieldID); } }
        public string CustomizationFieldName { get; set; }
        public string CustomizationID { get; set; }
        public string Type { get; set; }

        private Customization mCustomization = null;
        public Customization Customization
        {
            get
            {
                if (mCustomization == null && !string.IsNullOrEmpty(CustomizationID))
                {
                    try
                    {
                        mCustomization = new Customization(CustomizationID);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                    }
                }
                return mCustomization;
            }
        }
        public CustomizationField()
        {
        }
        public CustomizationField(string CustomizationFieldID)
        {
            this.CustomizationFieldID = CustomizationFieldID;
            Load();
        }
        public CustomizationField(DataRow objRow)
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
                         "FROM CustomizationField (NOLOCK) " +
                         "WHERE CustomizationFieldID=" + Database.HandleQuote(CustomizationFieldID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("CustomizationFieldID=" + CustomizationFieldID + " is not found");
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

                if (objColumns.Contains("CustomizationFieldID")) CustomizationFieldID = Convert.ToString(objRow["CustomizationFieldID"]);
                if (objColumns.Contains("CustomizationFieldName")) CustomizationFieldName = Convert.ToString(objRow["CustomizationFieldName"]);
                if (objColumns.Contains("CustomizationID")) CustomizationID = Convert.ToString(objRow["CustomizationID"]);

                if (string.IsNullOrEmpty(CustomizationFieldID)) throw new Exception("Missing CustomizationFieldID in the datarow");
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
                if (string.IsNullOrEmpty(CustomizationFieldName)) throw new Exception("CustomizationFieldName is required");
                if (string.IsNullOrEmpty(CustomizationID)) throw new Exception("CustomizationID is required");

                if (!IsNew) throw new Exception("Create cannot be performed, CustomizationFieldID already exists");

                dicParam["CustomizationFieldName"] = CustomizationFieldName;
                dicParam["CustomizationID"] = CustomizationID;
                dicParam["Type"] = Type;

                CustomizationFieldID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "CustomizationField"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(CustomizationFieldName)) throw new Exception("CustomizationFieldName is required");
                if (string.IsNullOrEmpty(CustomizationID)) throw new Exception("CustomizationID is required");

                if (IsNew) throw new Exception("Update cannot be performed, CustomizationFieldID is missing");

                dicParam["CustomizationFieldName"] = CustomizationFieldName;
                dicParam["CustomizationID"] = CustomizationID;
                dicParam["Type"] = Type;

                dicWParam["CustomizationFieldID"] = CustomizationFieldID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "CustomizationField"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, CustomizationFieldID is missing");

                dicDParam["CustomizationFieldID"] = CustomizationFieldID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "CustomizationField"), objConn, objTran);
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

        public static CustomizationField GetCustomizationField(CustomizationFieldFilter Filter)
        {
            List<CustomizationField> objCustomizationFields = null;
            CustomizationField objReturn = null;

            try
            {
                objCustomizationFields = GetCustomizationFields(Filter);
                if (objCustomizationFields != null && objCustomizationFields.Count >= 1) objReturn = objCustomizationFields[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomizationFields = null;
            }
            return objReturn;
        }

        public static List<CustomizationField> GetCustomizationFields()
        {
            int intTotalCount = 0;
            return GetCustomizationFields(null, null, null, out intTotalCount);
        }

        public static List<CustomizationField> GetCustomizationFields(CustomizationFieldFilter Filter)
        {
            int intTotalCount = 0;
            return GetCustomizationFields(Filter, null, null, out intTotalCount);
        }

        public static List<CustomizationField> GetCustomizationFields(CustomizationFieldFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetCustomizationFields(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<CustomizationField> GetCustomizationFields(CustomizationFieldFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<CustomizationField> objReturn = null;
            CustomizationField objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<CustomizationField>();

                strSQL = "SELECT * " +
                         "FROM CustomizationField (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.CustomizationID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomizationID, "CustomizationID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "CustomizationFieldID" : Utility.CustomSorting.GetSortExpression(typeof(CustomizationField), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new CustomizationField(objData.Tables[0].Rows[i]);
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

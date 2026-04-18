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
    public class CustomFieldSignature : ISBase.BaseClass
    {

        public string CustomFieldSignatureID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(CustomFieldSignatureID); } }
        public string CustomFieldID { get; set; }
        public string UserWebsiteID { get; set; }
        public DateTime CreatedOn { get; set; }

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
        private CustomField mUserWebsite = null;
        public CustomField UserWebsite
        {
            get
            {
                if (mUserWebsite == null && !string.IsNullOrEmpty(UserWebsiteID))
                {
                    mUserWebsite = new CustomField(UserWebsiteID);
                }
                return mUserWebsite;
            }
        }
        public CustomFieldSignature()
        {
        }
        public CustomFieldSignature(string CustomFieldSignatureID)
        {
            this.CustomFieldSignatureID = CustomFieldSignatureID;
            Load();
        }
        public CustomFieldSignature(DataRow objRow)
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
                         "FROM CustomFieldSignature (NOLOCK) " +
                         "WHERE CustomFieldSignatureID=" + Database.HandleQuote(CustomFieldSignatureID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("CustomFieldSignatureID=" + CustomFieldSignatureID + " is not found");
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

                if (objColumns.Contains("CustomFieldSignatureID")) CustomFieldSignatureID = Convert.ToString(objRow["CustomFieldSignatureID"]);
                if (objColumns.Contains("CustomFieldID")) CustomFieldID = Convert.ToString(objRow["CustomFieldID"]);
                if (objColumns.Contains("UserWebsiteID")) UserWebsiteID = Convert.ToString(objRow["UserWebsiteID"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(CustomFieldSignatureID)) throw new Exception("Missing CustomFieldSignatureID in the datarow");
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
                if (CustomFieldID == null) throw new Exception("CustomFieldID is required");
                if (UserWebsiteID == null) throw new Exception("UserWebsiteID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, CustomFieldSignatureID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CustomFieldID"] = CustomFieldID;
                dicParam["UserWebsiteID"] = UserWebsiteID;

                CustomFieldSignatureID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "CustomFieldSignature"), objConn, objTran).ToString();

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
                if (CustomFieldID == null) throw new Exception("CustomFieldID is required");
                if (UserWebsiteID == null) throw new Exception("UserWebsiteID is required");
                if (IsNew) throw new Exception("Update cannot be performed, CustomFieldSignatureID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["CustomFieldID"] = CustomFieldID;
                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicWParam["CustomFieldSignatureID"] = CustomFieldSignatureID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "CustomFieldSignature"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, CustomFieldSignatureID is missing");
                dicDParam["CustomFieldSignatureID"] = CustomFieldSignatureID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "CustomFieldSignature"), objConn, objTran);
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

        public static CustomFieldSignature GetCustomFieldSignature(CustomFieldSignatureFilter Filter)
        {
            List<CustomFieldSignature> objCustomFieldSignatures = null;
            CustomFieldSignature objReturn = null;

            try
            {
                objCustomFieldSignatures = GetCustomFieldSignatures(Filter);
                if (objCustomFieldSignatures != null && objCustomFieldSignatures.Count >= 1) objReturn = objCustomFieldSignatures[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomFieldSignatures = null;
            }
            return objReturn;
        }

        public static List<CustomFieldSignature> GetCustomFieldSignatures()
        {
            int intTotalCount = 0;
            return GetCustomFieldSignatures(null, null, null, out intTotalCount);
        }

        public static List<CustomFieldSignature> GetCustomFieldSignatures(CustomFieldSignatureFilter Filter)
        {
            int intTotalCount = 0;
            return GetCustomFieldSignatures(Filter, null, null, out intTotalCount);
        }

        public static List<CustomFieldSignature> GetCustomFieldSignatures(CustomFieldSignatureFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetCustomFieldSignatures(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<CustomFieldSignature> GetCustomFieldSignatures(CustomFieldSignatureFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<CustomFieldSignature> objReturn = null;
            CustomFieldSignature objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<CustomFieldSignature>();

                strSQL = "SELECT * " +
                         "FROM CustomFieldSignature (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.CustomFieldID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomFieldID, "CustomFieldID");
                    if (Filter.UserWebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserWebsiteID, "UserWebsiteID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Sort" : Utility.CustomSorting.GetSortExpression(typeof(CustomFieldSignature), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new CustomFieldSignature(objData.Tables[0].Rows[i]);
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

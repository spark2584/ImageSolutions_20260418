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
    public class CustomField : ISBase.BaseClass
    {
        public string CustomFieldID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(CustomFieldID); } }
        public string WebsiteID { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string LabelHTML { get; set; }
        public int? Sort { get; set; }
        public string DefaultUserWebsiteCustomFieldID { get; set; }
        public string AccountCustomFieldID { get; set; }
        public string Hint { get; set; }
        public bool IsNumeric { get; set; }
        public bool IsRequired { get; set; }
        public string UserWebsitePermission { get; set; }
        public string NetSuiteFieldID { get; set; }
        public bool IsSignature { get; set; }
        public bool Inactive { get; set; }

        private Website.Website mWebsite = null;
        public Website.Website Website
        {
            get
            {
                if (mWebsite == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    mWebsite = new Website.Website(WebsiteID);
                }
                return mWebsite;
            }
        }

        public CustomField()
        {
        }
        public CustomField(string CustomFieldID)
        {
            this.CustomFieldID = CustomFieldID;
            Load();
        }
        public CustomField(DataRow objRow)
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
                         "FROM CustomField (NOLOCK) " +
                         "WHERE CustomFieldID=" + Database.HandleQuote(CustomFieldID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("CustomFieldID=" + CustomFieldID + " is not found");
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

                if (objColumns.Contains("CustomFieldID")) CustomFieldID = Convert.ToString(objRow["CustomFieldID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("Type")) Type = Convert.ToString(objRow["Type"]);
                if (objColumns.Contains("Location")) Location = Convert.ToString(objRow["Location"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("LabelHTML")) LabelHTML = Convert.ToString(objRow["LabelHTML"]);
                if (objColumns.Contains("Hint")) Hint = Convert.ToString(objRow["Hint"]);
                if (objColumns.Contains("Sort") && objRow["Sort"] != DBNull.Value) Sort = Convert.ToInt32(objRow["Sort"]);
                if (objColumns.Contains("DefaultUserWebsiteCustomFieldID")) DefaultUserWebsiteCustomFieldID = Convert.ToString(objRow["DefaultUserWebsiteCustomFieldID"]);
                if (objColumns.Contains("AccountCustomFieldID")) AccountCustomFieldID = Convert.ToString(objRow["AccountCustomFieldID"]);
                if (objColumns.Contains("IsNumeric")) IsNumeric = Convert.ToBoolean(objRow["IsNumeric"]);
                if (objColumns.Contains("IsRequired")) IsRequired = Convert.ToBoolean(objRow["IsRequired"]);
                if (objColumns.Contains("UserWebsitePermission")) UserWebsitePermission = Convert.ToString(objRow["UserWebsitePermission"]);
                if (objColumns.Contains("NetSuiteFieldID")) NetSuiteFieldID = Convert.ToString(objRow["NetSuiteFieldID"]);
                if (objColumns.Contains("IsSignature")) IsSignature = Convert.ToBoolean(objRow["IsSignature"]);
                if (objColumns.Contains("Inactive")) Inactive = Convert.ToBoolean(objRow["Inactive"]);

                if (string.IsNullOrEmpty(CustomFieldID)) throw new Exception("Missing CustomFieldID in the datarow");
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
                if (WebsiteID == null) throw new Exception("WebsiteID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, CustomFieldID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["Type"] = Type;
                dicParam["Location"] = Location;
                dicParam["Name"] = Name;
                dicParam["LabelHTML"] = LabelHTML;
                dicParam["Hint"] = Hint;
                dicParam["Sort"] = Sort;
                dicParam["DefaultUserWebsiteCustomFieldID"] = DefaultUserWebsiteCustomFieldID;
                dicParam["AccountCustomFieldID"] = AccountCustomFieldID;
                dicParam["UserWebsitePermission"] = UserWebsitePermission;
                dicParam["IsNumeric"] = IsNumeric;
                dicParam["IsRequired"] = IsRequired;
                dicParam["NetSuiteFieldID"] = NetSuiteFieldID;
                dicParam["IsSignature"] = IsSignature;
                dicParam["Inactive"] = Inactive;

                CustomFieldID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "CustomField"), objConn, objTran).ToString();

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
                if (WebsiteID == null) throw new Exception("WebsiteID is required");
                if (IsNew) throw new Exception("Update cannot be performed, CustomFieldID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["Type"] = Type;
                dicParam["Location"] = Location;
                dicParam["Name"] = Name;
                dicParam["LabelHTML"] = LabelHTML;
                dicParam["Hint"] = Hint;
                dicParam["Sort"] = Sort;
                dicParam["DefaultUserWebsiteCustomFieldID"] = DefaultUserWebsiteCustomFieldID;
                dicParam["AccountCustomFieldID"] = AccountCustomFieldID;
                dicParam["UserWebsitePermission"] = UserWebsitePermission;
                dicParam["IsNumeric"] = IsNumeric;
                dicParam["IsRequired"] = IsRequired;
                dicParam["NetSuiteFieldID"] = NetSuiteFieldID;
                dicParam["Inactive"] = Inactive;
                dicParam["IsSignature"] = IsSignature;
                dicWParam["CustomFieldID"] = CustomFieldID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "CustomField"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, CustomFieldID is missing");
                dicDParam["CustomFieldID"] = CustomFieldID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "CustomField"), objConn, objTran);
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

        public static CustomField GetCustomField(CustomFieldFilter Filter)
        {
            List<CustomField> objCustomFields = null;
            CustomField objReturn = null;

            try
            {
                objCustomFields = GetCustomFields(Filter);
                if (objCustomFields != null && objCustomFields.Count >= 1) objReturn = objCustomFields[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomFields = null;
            }
            return objReturn;
        }

        public static List<CustomField> GetCustomFields()
        {
            int intTotalCount = 0;
            return GetCustomFields(null, null, null, out intTotalCount);
        }

        public static List<CustomField> GetCustomFields(CustomFieldFilter Filter)
        {
            int intTotalCount = 0;
            return GetCustomFields(Filter, null, null, out intTotalCount);
        }

        public static List<CustomField> GetCustomFields(CustomFieldFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetCustomFields(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<CustomField> GetCustomFields(CustomFieldFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<CustomField> objReturn = null;
            CustomField objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<CustomField>();

                strSQL = "SELECT * " +
                         "FROM CustomField (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.Location != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Location, "Location");
                    if (Filter.Name != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Name, "Name");
                    if (Filter.Inactive != null) strSQL += "AND Inactive=" + Database.HandleQuote(Convert.ToBoolean(Filter.Inactive).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Sort" : Utility.CustomSorting.GetSortExpression(typeof(CustomField), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else strSQL += " ORDER BY Sort";

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new CustomField(objData.Tables[0].Rows[i]);
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

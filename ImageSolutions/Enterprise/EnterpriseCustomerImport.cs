using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Enterprise
{
    public class EnterpriseCustomerImport : ISBase.BaseClass
    {
        public string EnterpriseCustomerImportID { get; private set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public bool IsStore { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsEncrypted { get; set; }
        public bool IsPreEmployee { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(EnterpriseCustomerImportID); } }

        public EnterpriseCustomerImport()
        {
        }
        public EnterpriseCustomerImport(string EnterpriseCustomerImportID)
        {
            this.EnterpriseCustomerImportID = EnterpriseCustomerImportID;
            Load();
        }
        public EnterpriseCustomerImport(DataRow objRow)
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
                         "FROM EnterpriseCustomerImport (NOLOCK) " +
                         "WHERE EnterpriseCustomerImportID=" + Database.HandleQuote(EnterpriseCustomerImportID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("EnterpriseCustomerImportID=" + EnterpriseCustomerImportID + " is not found");
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

                if (objColumns.Contains("EnterpriseCustomerImportID")) EnterpriseCustomerImportID = Convert.ToString(objRow["EnterpriseCustomerImportID"]);
                if (objColumns.Contains("FilePath")) FilePath = Convert.ToString(objRow["FilePath"]);
                if (objColumns.Contains("FileName")) FileName = Convert.ToString(objRow["FileName"]);
                if (objColumns.Contains("IsStore")) IsStore = Convert.ToBoolean(objRow["IsStore"]);
                if (objColumns.Contains("IsProcessed")) IsProcessed = Convert.ToBoolean(objRow["IsProcessed"]);
                if (objColumns.Contains("IsEncrypted")) IsEncrypted = Convert.ToBoolean(objRow["IsEncrypted"]);
                if (objColumns.Contains("IsPreEmployee")) IsPreEmployee = Convert.ToBoolean(objRow["IsPreEmployee"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(EnterpriseCustomerImportID)) throw new Exception("Missing EnterpriseCustomerImportID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, EnterpriseCustomerImportID already exists");

                dicParam["FilePath"] = FilePath;
                dicParam["FileName"] = FileName;
                dicParam["IsStore"] = IsStore;
                dicParam["IsProcessed"] = IsProcessed;
                dicParam["IsEncrypted"] = IsEncrypted;
                dicParam["IsPreEmployee"] = IsPreEmployee;
                dicParam["ErrorMessage"] = ErrorMessage;

                EnterpriseCustomerImportID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "EnterpriseCustomerImport"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, EnterpriseCustomerImportID is missing");

                dicParam["FilePath"] = FilePath;
                dicParam["FileName"] = FileName;
                dicParam["IsStore"] = IsStore;
                dicParam["IsProcessed"] = IsProcessed;
                dicParam["IsEncrypted"] = IsEncrypted;
                dicParam["IsPreEmployee"] = IsPreEmployee;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = DateTime.UtcNow;

                dicWParam["EnterpriseCustomerImportID"] = EnterpriseCustomerImportID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "EnterpriseCustomerImport"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, EnterpriseCustomerImportID is missing");

                dicDParam["EnterpriseCustomerImportID"] = EnterpriseCustomerImportID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "EnterpriseCustomerImport"), objConn, objTran);
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

        public static EnterpriseCustomerImport GetEnterpriseCustomerImport(EnterpriseCustomerImportFilter Filter)
        {
            List<EnterpriseCustomerImport> objEnterpriseCustomerImports = null;
            EnterpriseCustomerImport objReturn = null;

            try
            {
                objEnterpriseCustomerImports = GetEnterpriseCustomerImports(Filter);
                if (objEnterpriseCustomerImports != null && objEnterpriseCustomerImports.Count >= 1) objReturn = objEnterpriseCustomerImports[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEnterpriseCustomerImports = null;
            }
            return objReturn;
        }

        public static List<EnterpriseCustomerImport> GetEnterpriseCustomerImports()
        {
            int intTotalCount = 0;
            return GetEnterpriseCustomerImports(null, null, null, out intTotalCount);
        }

        public static List<EnterpriseCustomerImport> GetEnterpriseCustomerImports(EnterpriseCustomerImportFilter Filter)
        {
            int intTotalCount = 0;
            return GetEnterpriseCustomerImports(Filter, null, null, out intTotalCount);
        }

        public static List<EnterpriseCustomerImport> GetEnterpriseCustomerImports(EnterpriseCustomerImportFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetEnterpriseCustomerImports(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<EnterpriseCustomerImport> GetEnterpriseCustomerImports(EnterpriseCustomerImportFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<EnterpriseCustomerImport> objReturn = null;
            EnterpriseCustomerImport objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<EnterpriseCustomerImport>();

                strSQL = "SELECT * " +
                         "FROM EnterpriseCustomerImport (NOLOCK) " +
                         "where 1=1 ";

                if (Filter != null)
                {
                    if (Filter.EnterpriseCustomerImportID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EnterpriseCustomerImportID, "EnterpriseCustomerImportID");
                    if (Filter.FileName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.FileName, "FileName");
                    if (Filter.IsStore != null) strSQL += "AND IsStore=" + Database.HandleQuote(Convert.ToInt32(Filter.IsStore.Value).ToString());
                    if (Filter.IsProcessed != null) strSQL += "AND IsProcessed=" + Database.HandleQuote(Convert.ToInt32(Filter.IsProcessed.Value).ToString());
                    if (Filter.IsEncrypted != null) strSQL += "AND IsEncrypted=" + Database.HandleQuote(Convert.ToInt32(Filter.IsEncrypted.Value).ToString());
                    if (Filter.IsPreEmployee != null) strSQL += "AND IsPreEmployee=" + Database.HandleQuote(Convert.ToInt32(Filter.IsPreEmployee.Value).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "EnterpriseCustomerImportID" : Utility.CustomSorting.GetSortExpression(typeof(EnterpriseCustomerImport), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new EnterpriseCustomerImport(objData.Tables[0].Rows[i]);
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

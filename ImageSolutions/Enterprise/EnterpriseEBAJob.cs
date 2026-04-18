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
    public class EnterpriseEBAJob : ISBase.BaseClass
    {
        public string EnterpriseEBAJobID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(EnterpriseEBAJobID); } }
        public string JobProfile { get; set; }
        public string JobCode { get; set; }

        public bool IsViewAccess { get; set; }
        public bool IsAdminAccess { get; set; }

        public bool IsCorporate { get; set; }

        public bool AllowAddressChange { get; set; }

        public DateTime CreatedOn { get; set; }

        public EnterpriseEBAJob()
        {
        }
        public EnterpriseEBAJob(string EnterpriseEBAJobID)
        {
            this.EnterpriseEBAJobID = EnterpriseEBAJobID;
            Load();
        }
        public EnterpriseEBAJob(DataRow objRow)
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
                         "FROM EnterpriseEBAJob (NOLOCK) " +
                         "WHERE EnterpriseEBAJobID=" + Database.HandleQuote(EnterpriseEBAJobID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("EnterpriseEBAJobID=" + EnterpriseEBAJobID + " is not found");
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

                if (objColumns.Contains("EnterpriseEBAJobID")) EnterpriseEBAJobID = Convert.ToString(objRow["EnterpriseEBAJobID"]);
                if (objColumns.Contains("JobProfile")) JobProfile = Convert.ToString(objRow["JobProfile"]);
                if (objColumns.Contains("JobCode")) JobCode = Convert.ToString(objRow["JobCode"]);

                if (objColumns.Contains("IsViewAccess")) IsViewAccess = Convert.ToBoolean(objRow["IsViewAccess"]);
                if (objColumns.Contains("IsAdminAccess")) IsAdminAccess = Convert.ToBoolean(objRow["IsAdminAccess"]);
                if (objColumns.Contains("IsCorporate")) IsCorporate = Convert.ToBoolean(objRow["IsCorporate"]);
                if (objColumns.Contains("AllowAddressChange")) AllowAddressChange = Convert.ToBoolean(objRow["AllowAddressChange"]);

                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(EnterpriseEBAJobID)) throw new Exception("Missing EnterpriseEBAJobID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, EnterpriseEBAJobID already exists");

                dicParam["JobProfile"] = JobProfile;
                dicParam["JobCode"] = JobCode;

                dicParam["IsViewAccess"] = IsViewAccess;
                dicParam["IsAdminAccess"] = IsAdminAccess;
                dicParam["IsCorporate"] = IsCorporate;
                dicParam["AllowAddressChange"] = AllowAddressChange;

                EnterpriseEBAJobID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "EnterpriseEBAJob"), objConn, objTran).ToString();

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
                if (EnterpriseEBAJobID == null) throw new Exception("EnterpriseEBAJobID is required");
                if (IsNew) throw new Exception("Update cannot be performed, EnterpriseEBAJobID is missing");

                dicParam["JobProfile"] = JobProfile;
                dicParam["JobCode"] = JobCode;

                dicParam["IsViewAccess"] = IsViewAccess;
                dicParam["IsAdminAccess"] = IsAdminAccess;
                dicParam["IsCorporate"] = IsCorporate;
                dicParam["AllowAddressChange"] = AllowAddressChange;

                dicWParam["EnterpriseEBAJobID"] = EnterpriseEBAJobID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "EnterpriseEBAJob"), objConn, objTran);

                //foreach (EnterpriseEBAJobLine objEnterpriseEBAJobLine in EnterpriseEBAJobLines)
                //{
                //    objEnterpriseEBAJobLine.Update(objConn, objTran);
                //}

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
                if (IsNew) throw new Exception("Delete cannot be performed, EnterpriseEBAJobID is missing");

                //Delete Lines
                //List<EnterpriseEBAJobLine> lstEnterpriseEBAJobLine;
                //lstEnterpriseEBAJobLine = EnterpriseEBAJobLines;
                //foreach (EnterpriseEBAJobLine _EnterpriseEBAJobLine in lstEnterpriseEBAJobLine)
                //{
                //    _EnterpriseEBAJobLine.Delete(objConn, objTran);
                //}

                dicDParam["EnterpriseEBAJobID"] = EnterpriseEBAJobID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "EnterpriseEBAJob"), objConn, objTran);
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

        public static EnterpriseEBAJob GetEnterpriseEBAJob(EnterpriseEBAJobFilter Filter)
        {
            List<EnterpriseEBAJob> objEnterpriseEBAJobs = null;
            EnterpriseEBAJob objReturn = null;

            try
            {
                objEnterpriseEBAJobs = GetEnterpriseEBAJobs(Filter);
                if (objEnterpriseEBAJobs != null && objEnterpriseEBAJobs.Count >= 1) objReturn = objEnterpriseEBAJobs[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEnterpriseEBAJobs = null;
            }
            return objReturn;
        }

        public static List<EnterpriseEBAJob> GetEnterpriseEBAJobs()
        {
            int intTotalCount = 0;
            return GetEnterpriseEBAJobs(null, null, null, out intTotalCount);
        }

        public static List<EnterpriseEBAJob> GetEnterpriseEBAJobs(EnterpriseEBAJobFilter Filter)
        {
            int intTotalCount = 0;
            return GetEnterpriseEBAJobs(Filter, null, null, out intTotalCount);
        }

        public static List<EnterpriseEBAJob> GetEnterpriseEBAJobs(EnterpriseEBAJobFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetEnterpriseEBAJobs(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<EnterpriseEBAJob> GetEnterpriseEBAJobs(EnterpriseEBAJobFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<EnterpriseEBAJob> objReturn = null;
            EnterpriseEBAJob objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<EnterpriseEBAJob>();

                strSQL = "SELECT * " +
                         "FROM EnterpriseEBAJob (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.JobCode != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.JobCode, "JobCode");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "EnterpriseEBAJobID" : Utility.CustomSorting.GetSortExpression(typeof(EnterpriseEBAJob), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new EnterpriseEBAJob(objData.Tables[0].Rows[i]);
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

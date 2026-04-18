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
    public class EnterpriseFileSubmit : ISBase.BaseClass
    {
        public string EnterpriseFileSubmitID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(EnterpriseFileSubmitID); } }
        public string FilePath { get; set; }
        public string Type { get; set; }
        public DateTime CreatedOn { get; set; }

        public EnterpriseFileSubmit()
        {
        }

        public EnterpriseFileSubmit(string EnterpriseFileSubmitID)
        {
            this.EnterpriseFileSubmitID = EnterpriseFileSubmitID;
            Load();
        }
        public EnterpriseFileSubmit(DataRow objRow)
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
                         "FROM EnterpriseFileSubmit (NOLOCK) " +
                         "WHERE EnterpriseFileSubmitID=" + Database.HandleQuote(EnterpriseFileSubmitID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("EnterpriseFileSubmitID=" + EnterpriseFileSubmitID + " is not found");
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

                if (objColumns.Contains("EnterpriseFileSubmitID")) EnterpriseFileSubmitID = Convert.ToString(objRow["EnterpriseFileSubmitID"]);
                if (objColumns.Contains("FilePath")) FilePath = Convert.ToString(objRow["FilePath"]);
                if (objColumns.Contains("Type")) Type = Convert.ToString(objRow["Type"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(EnterpriseFileSubmitID)) throw new Exception("Missing EnterpriseFileSubmitID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, EnterpriseFileSubmitID already exists");

                dicParam["FilePath"] = FilePath;
                dicParam["Type"] = Type;

                EnterpriseFileSubmitID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "EnterpriseFileSubmit"), objConn, objTran).ToString();

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
                if (EnterpriseFileSubmitID == null) throw new Exception("EnterpriseFileSubmitID is required");
                if (IsNew) throw new Exception("Update cannot be performed, EnterpriseFileSubmitID is missing");

                dicParam["FilePath"] = FilePath;
                dicParam["Type"] = Type;
                dicWParam["EnterpriseFileSubmitID"] = EnterpriseFileSubmitID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "EnterpriseFileSubmit"), objConn, objTran);

                //foreach (EnterpriseFileSubmitLine objEnterpriseFileSubmitLine in EnterpriseFileSubmitLines)
                //{
                //    objEnterpriseFileSubmitLine.Update(objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, EnterpriseFileSubmitID is missing");

                //Delete Lines
                //List<EnterpriseFileSubmitLine> lstEnterpriseFileSubmitLine;
                //lstEnterpriseFileSubmitLine = EnterpriseFileSubmitLines;
                //foreach (EnterpriseFileSubmitLine _EnterpriseFileSubmitLine in lstEnterpriseFileSubmitLine)
                //{
                //    _EnterpriseFileSubmitLine.Delete(objConn, objTran);
                //}

                dicDParam["EnterpriseFileSubmitID"] = EnterpriseFileSubmitID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "EnterpriseFileSubmit"), objConn, objTran);
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

        public static EnterpriseFileSubmit GetEnterpriseFileSubmit(EnterpriseFileSubmitFilter Filter)
        {
            List<EnterpriseFileSubmit> objEnterpriseFileSubmits = null;
            EnterpriseFileSubmit objReturn = null;

            try
            {
                objEnterpriseFileSubmits = GetEnterpriseFileSubmits(Filter);
                if (objEnterpriseFileSubmits != null && objEnterpriseFileSubmits.Count >= 1) objReturn = objEnterpriseFileSubmits[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEnterpriseFileSubmits = null;
            }
            return objReturn;
        }

        public static List<EnterpriseFileSubmit> GetEnterpriseFileSubmits()
        {
            int intTotalCount = 0;
            return GetEnterpriseFileSubmits(null, null, null, out intTotalCount);
        }

        public static List<EnterpriseFileSubmit> GetEnterpriseFileSubmits(EnterpriseFileSubmitFilter Filter)
        {
            int intTotalCount = 0;
            return GetEnterpriseFileSubmits(Filter, null, null, out intTotalCount);
        }

        public static List<EnterpriseFileSubmit> GetEnterpriseFileSubmits(EnterpriseFileSubmitFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetEnterpriseFileSubmits(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<EnterpriseFileSubmit> GetEnterpriseFileSubmits(EnterpriseFileSubmitFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<EnterpriseFileSubmit> objReturn = null;
            EnterpriseFileSubmit objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<EnterpriseFileSubmit>();

                strSQL = "SELECT * " +
                         "FROM EnterpriseFileSubmit (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.EnterpriseFileSubmitID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EnterpriseFileSubmitID, "EnterpriseFileSubmitID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "EnterpriseFileSubmitID" : Utility.CustomSorting.GetSortExpression(typeof(EnterpriseFileSubmit), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new EnterpriseFileSubmit(objData.Tables[0].Rows[i]);
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

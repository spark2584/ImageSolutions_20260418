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
    public class EnterpriseEntityGroup : ISBase.BaseClass
    {
        public string EnterpriseEntityGroupID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(EnterpriseEntityGroupID); } }
        public string InternalID { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }

        public EnterpriseEntityGroup()
        {
        }
        public EnterpriseEntityGroup(string EnterpriseEntityGroupID)
        {
            this.EnterpriseEntityGroupID = EnterpriseEntityGroupID;
            Load();
        }
        public EnterpriseEntityGroup(DataRow objRow)
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
                         "FROM EnterpriseEntityGroup (NOLOCK) " +
                         "WHERE EnterpriseEntityGroupID=" + Database.HandleQuote(EnterpriseEntityGroupID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("EnterpriseEntityGroupID=" + EnterpriseEntityGroupID + " is not found");
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

                if (objColumns.Contains("EnterpriseEntityGroupID")) EnterpriseEntityGroupID = Convert.ToString(objRow["EnterpriseEntityGroupID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(EnterpriseEntityGroupID)) throw new Exception("Missing EnterpriseEntityGroupID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, EnterpriseEntityGroupID already exists");

                dicParam["InternalID"] = InternalID;
                dicParam["Name"] = Name;

                EnterpriseEntityGroupID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "EnterpriseEntityGroup"), objConn, objTran).ToString();

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
                if (EnterpriseEntityGroupID == null) throw new Exception("EnterpriseEntityGroupID is required");
                if (IsNew) throw new Exception("Update cannot be performed, EnterpriseEntityGroupID is missing");

                dicParam["InternalID"] = InternalID;
                dicParam["Name"] = Name;
                dicWParam["EnterpriseEntityGroupID"] = EnterpriseEntityGroupID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "EnterpriseEntityGroup"), objConn, objTran);

                //foreach (EnterpriseEntityGroupLine objEnterpriseEntityGroupLine in EnterpriseEntityGroupLines)
                //{
                //    objEnterpriseEntityGroupLine.Update(objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, EnterpriseEntityGroupID is missing");

                //Delete Lines
                //List<EnterpriseEntityGroupLine> lstEnterpriseEntityGroupLine;
                //lstEnterpriseEntityGroupLine = EnterpriseEntityGroupLines;
                //foreach (EnterpriseEntityGroupLine _EnterpriseEntityGroupLine in lstEnterpriseEntityGroupLine)
                //{
                //    _EnterpriseEntityGroupLine.Delete(objConn, objTran);
                //}

                dicDParam["EnterpriseEntityGroupID"] = EnterpriseEntityGroupID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "EnterpriseEntityGroup"), objConn, objTran);
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

        public static EnterpriseEntityGroup GetEnterpriseEntityGroup(EnterpriseEntityGroupFilter Filter)
        {
            List<EnterpriseEntityGroup> objEnterpriseEntityGroups = null;
            EnterpriseEntityGroup objReturn = null;

            try
            {
                objEnterpriseEntityGroups = GetEnterpriseEntityGroups(Filter);
                if (objEnterpriseEntityGroups != null && objEnterpriseEntityGroups.Count >= 1) objReturn = objEnterpriseEntityGroups[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEnterpriseEntityGroups = null;
            }
            return objReturn;
        }

        public static List<EnterpriseEntityGroup> GetEnterpriseEntityGroups()
        {
            int intTotalCount = 0;
            return GetEnterpriseEntityGroups(null, null, null, out intTotalCount);
        }

        public static List<EnterpriseEntityGroup> GetEnterpriseEntityGroups(EnterpriseEntityGroupFilter Filter)
        {
            int intTotalCount = 0;
            return GetEnterpriseEntityGroups(Filter, null, null, out intTotalCount);
        }

        public static List<EnterpriseEntityGroup> GetEnterpriseEntityGroups(EnterpriseEntityGroupFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetEnterpriseEntityGroups(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<EnterpriseEntityGroup> GetEnterpriseEntityGroups(EnterpriseEntityGroupFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<EnterpriseEntityGroup> objReturn = null;
            EnterpriseEntityGroup objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<EnterpriseEntityGroup>();

                strSQL = "SELECT * " +
                         "FROM EnterpriseEntityGroup (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.Name != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Name, "Name");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "EnterpriseEntityGroupID" : Utility.CustomSorting.GetSortExpression(typeof(EnterpriseEntityGroup), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new EnterpriseEntityGroup(objData.Tables[0].Rows[i]);
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

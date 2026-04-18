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
    public class EnterpriseEntityGroupMap : ISBase.BaseClass
    {
        public string EnterpriseEntityGroupMapID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(EnterpriseEntityGroupMapID); } }
        public string EnterpriseEntityGroupID { get; set; }
        public string GroupName { get; set; }
        public string Code { get; set; }
        public DateTime CreatedOn { get; set; }

        public EnterpriseEntityGroupMap()
        {
        }
        public EnterpriseEntityGroupMap(string EnterpriseEntityGroupMapID)
        {
            this.EnterpriseEntityGroupMapID = EnterpriseEntityGroupMapID;
            Load();
        }
        public EnterpriseEntityGroupMap(DataRow objRow)
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
                         "FROM EnterpriseEntityGroupMap (NOLOCK) " +
                         "WHERE EnterpriseEntityGroupMapID=" + Database.HandleQuote(EnterpriseEntityGroupMapID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("EnterpriseEntityGroupMapID=" + EnterpriseEntityGroupMapID + " is not found");
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

                if (objColumns.Contains("EnterpriseEntityGroupMapID")) EnterpriseEntityGroupMapID = Convert.ToString(objRow["EnterpriseEntityGroupMapID"]);
                if (objColumns.Contains("EnterpriseEntityGroupID")) EnterpriseEntityGroupID = Convert.ToString(objRow["EnterpriseEntityGroupID"]);
                if (objColumns.Contains("GroupName")) GroupName = Convert.ToString(objRow["GroupName"]);
                if (objColumns.Contains("Code")) Code = Convert.ToString(objRow["Code"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(EnterpriseEntityGroupMapID)) throw new Exception("Missing EnterpriseEntityGroupMapID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, EnterpriseEntityGroupMapID already exists");

                dicParam["EnterpriseEntityGroupID"] = EnterpriseEntityGroupID;
                dicParam["GroupName"] = GroupName;
                dicParam["Code"] = Code;

                EnterpriseEntityGroupMapID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "EnterpriseEntityGroupMap"), objConn, objTran).ToString();

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
                if (EnterpriseEntityGroupMapID == null) throw new Exception("EnterpriseEntityGroupMapID is required");
                if (IsNew) throw new Exception("Update cannot be performed, EnterpriseEntityGroupMapID is missing");

                dicParam["EnterpriseEntityGroupID"] = EnterpriseEntityGroupID;
                dicParam["GroupName"] = GroupName;
                dicParam["Code"] = Code;
                dicWParam["EnterpriseEntityGroupMapID"] = EnterpriseEntityGroupMapID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "EnterpriseEntityGroupMap"), objConn, objTran);

                //foreach (EnterpriseEntityGroupMapLine objEnterpriseEntityGroupMapLine in EnterpriseEntityGroupMapLines)
                //{
                //    objEnterpriseEntityGroupMapLine.Update(objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, EnterpriseEntityGroupMapID is missing");

                //Delete Lines
                //List<EnterpriseEntityGroupMapLine> lstEnterpriseEntityGroupMapLine;
                //lstEnterpriseEntityGroupMapLine = EnterpriseEntityGroupMapLines;
                //foreach (EnterpriseEntityGroupMapLine _EnterpriseEntityGroupMapLine in lstEnterpriseEntityGroupMapLine)
                //{
                //    _EnterpriseEntityGroupMapLine.Delete(objConn, objTran);
                //}

                dicDParam["EnterpriseEntityGroupMapID"] = EnterpriseEntityGroupMapID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "EnterpriseEntityGroupMap"), objConn, objTran);
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

        public static EnterpriseEntityGroupMap GetEnterpriseEntityGroupMap(EnterpriseEntityGroupMapFilter Filter)
        {
            List<EnterpriseEntityGroupMap> objEnterpriseEntityGroupMaps = null;
            EnterpriseEntityGroupMap objReturn = null;

            try
            {
                objEnterpriseEntityGroupMaps = GetEnterpriseEntityGroupMaps(Filter);
                if (objEnterpriseEntityGroupMaps != null && objEnterpriseEntityGroupMaps.Count >= 1) objReturn = objEnterpriseEntityGroupMaps[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEnterpriseEntityGroupMaps = null;
            }
            return objReturn;
        }

        public static List<EnterpriseEntityGroupMap> GetEnterpriseEntityGroupMaps()
        {
            int intTotalCount = 0;
            return GetEnterpriseEntityGroupMaps(null, null, null, out intTotalCount);
        }

        public static List<EnterpriseEntityGroupMap> GetEnterpriseEntityGroupMaps(EnterpriseEntityGroupMapFilter Filter)
        {
            int intTotalCount = 0;
            return GetEnterpriseEntityGroupMaps(Filter, null, null, out intTotalCount);
        }

        public static List<EnterpriseEntityGroupMap> GetEnterpriseEntityGroupMaps(EnterpriseEntityGroupMapFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetEnterpriseEntityGroupMaps(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<EnterpriseEntityGroupMap> GetEnterpriseEntityGroupMaps(EnterpriseEntityGroupMapFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<EnterpriseEntityGroupMap> objReturn = null;
            EnterpriseEntityGroupMap objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<EnterpriseEntityGroupMap>();

                strSQL = "SELECT * " +
                         "FROM EnterpriseEntityGroupMap (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.EnterpriseEntityGroupID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EnterpriseEntityGroupID, "EnterpriseEntityGroupID");
                    if (Filter.GroupName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.GroupName, "GroupName");
                    if (Filter.Code != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Code, "Code");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "EnterpriseEntityGroupMapID" : Utility.CustomSorting.GetSortExpression(typeof(EnterpriseEntityGroupMap), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new EnterpriseEntityGroupMap(objData.Tables[0].Rows[i]);
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

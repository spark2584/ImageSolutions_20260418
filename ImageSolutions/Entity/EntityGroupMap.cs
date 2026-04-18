using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Entity
{
    public class EntityGroupMap : ISBase.BaseClass
    {
        public string EntityGroupMapID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(EntityGroupMapID); } }
        public string EntityGroupID { get; set; }
        public string GroupName { get; set; }
        public string Code { get; set; }
        public DateTime CreatedOn { get; set; }

        public EntityGroupMap()
        {
        }
        public EntityGroupMap(string EntityGroupMapID)
        {
            this.EntityGroupMapID = EntityGroupMapID;
            Load();
        }
        public EntityGroupMap(DataRow objRow)
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
                         "FROM EntityGroupMap (NOLOCK) " +
                         "WHERE EntityGroupMapID=" + Database.HandleQuote(EntityGroupMapID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("EntityGroupMapID=" + EntityGroupMapID + " is not found");
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

                if (objColumns.Contains("EntityGroupMapID")) EntityGroupMapID = Convert.ToString(objRow["EntityGroupMapID"]);
                if (objColumns.Contains("EntityGroupID")) EntityGroupID = Convert.ToString(objRow["EntityGroupID"]);
                if (objColumns.Contains("GroupName")) GroupName = Convert.ToString(objRow["GroupName"]);
                if (objColumns.Contains("Code")) Code = Convert.ToString(objRow["Code"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(EntityGroupMapID)) throw new Exception("Missing EntityGroupMapID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, EntityGroupMapID already exists");

                dicParam["EntityGroupID"] = EntityGroupID;
                dicParam["GroupName"] = GroupName;
                dicParam["Code"] = Code;

                EntityGroupMapID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "EntityGroupMap"), objConn, objTran).ToString();

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
                if (EntityGroupMapID == null) throw new Exception("EntityGroupMapID is required");
                if (IsNew) throw new Exception("Update cannot be performed, EntityGroupMapID is missing");

                dicParam["EntityGroupID"] = EntityGroupID;
                dicParam["GroupName"] = GroupName;
                dicParam["Code"] = Code;
                dicWParam["EntityGroupMapID"] = EntityGroupMapID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "EntityGroupMap"), objConn, objTran);

                //foreach (EntityGroupMapLine objEntityGroupMapLine in EntityGroupMapLines)
                //{
                //    objEntityGroupMapLine.Update(objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, EntityGroupMapID is missing");

                //Delete Lines
                //List<EntityGroupMapLine> lstEntityGroupMapLine;
                //lstEntityGroupMapLine = EntityGroupMapLines;
                //foreach (EntityGroupMapLine _EntityGroupMapLine in lstEntityGroupMapLine)
                //{
                //    _EntityGroupMapLine.Delete(objConn, objTran);
                //}

                dicDParam["EntityGroupMapID"] = EntityGroupMapID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "EntityGroupMap"), objConn, objTran);
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

        public static EntityGroupMap GetEntityGroupMap(EntityGroupMapFilter Filter)
        {
            List<EntityGroupMap> objEntityGroupMaps = null;
            EntityGroupMap objReturn = null;

            try
            {
                objEntityGroupMaps = GetEntityGroupMaps(Filter);
                if (objEntityGroupMaps != null && objEntityGroupMaps.Count >= 1) objReturn = objEntityGroupMaps[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEntityGroupMaps = null;
            }
            return objReturn;
        }

        public static List<EntityGroupMap> GetEntityGroupMaps()
        {
            int intTotalCount = 0;
            return GetEntityGroupMaps(null, null, null, out intTotalCount);
        }

        public static List<EntityGroupMap> GetEntityGroupMaps(EntityGroupMapFilter Filter)
        {
            int intTotalCount = 0;
            return GetEntityGroupMaps(Filter, null, null, out intTotalCount);
        }

        public static List<EntityGroupMap> GetEntityGroupMaps(EntityGroupMapFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetEntityGroupMaps(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<EntityGroupMap> GetEntityGroupMaps(EntityGroupMapFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<EntityGroupMap> objReturn = null;
            EntityGroupMap objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<EntityGroupMap>();

                strSQL = "SELECT * " +
                         "FROM EntityGroupMap (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.EntityGroupID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EntityGroupID, "EntityGroupID");
                    if (Filter.GroupName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.GroupName, "GroupName");
                    if (Filter.Code != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Code, "Code");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "EntityGroupMapID" : Utility.CustomSorting.GetSortExpression(typeof(EntityGroupMap), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new EntityGroupMap(objData.Tables[0].Rows[i]);
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

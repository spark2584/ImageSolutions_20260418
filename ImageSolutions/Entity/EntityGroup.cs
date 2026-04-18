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
    public class EntityGroup : ISBase.BaseClass
    {
        public string EntityGroupID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(EntityGroupID); } }
        public string InternalID { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }

        public EntityGroup()
        {
        }
        public EntityGroup(string EntityGroupID)
        {
            this.EntityGroupID = EntityGroupID;
            Load();
        }
        public EntityGroup(DataRow objRow)
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
                         "FROM EntityGroup (NOLOCK) " +
                         "WHERE EntityGroupID=" + Database.HandleQuote(EntityGroupID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("EntityGroupID=" + EntityGroupID + " is not found");
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

                if (objColumns.Contains("EntityGroupID")) EntityGroupID = Convert.ToString(objRow["EntityGroupID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(EntityGroupID)) throw new Exception("Missing EntityGroupID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, EntityGroupID already exists");

                dicParam["InternalID"] = InternalID;
                dicParam["Name"] = Name;

                EntityGroupID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "EntityGroup"), objConn, objTran).ToString();

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
                if (EntityGroupID == null) throw new Exception("EntityGroupID is required");
                if (IsNew) throw new Exception("Update cannot be performed, EntityGroupID is missing");

                dicParam["InternalID"] = InternalID;
                dicParam["Name"] = Name;
                dicWParam["EntityGroupID"] = EntityGroupID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "EntityGroup"), objConn, objTran);

                //foreach (EntityGroupLine objEntityGroupLine in EntityGroupLines)
                //{
                //    objEntityGroupLine.Update(objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, EntityGroupID is missing");

                //Delete Lines
                //List<EntityGroupLine> lstEntityGroupLine;
                //lstEntityGroupLine = EntityGroupLines;
                //foreach (EntityGroupLine _EntityGroupLine in lstEntityGroupLine)
                //{
                //    _EntityGroupLine.Delete(objConn, objTran);
                //}

                dicDParam["EntityGroupID"] = EntityGroupID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "EntityGroup"), objConn, objTran);
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

        public static EntityGroup GetEntityGroup(EntityGroupFilter Filter)
        {
            List<EntityGroup> objEntityGroups = null;
            EntityGroup objReturn = null;

            try
            {
                objEntityGroups = GetEntityGroups(Filter);
                if (objEntityGroups != null && objEntityGroups.Count >= 1) objReturn = objEntityGroups[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEntityGroups = null;
            }
            return objReturn;
        }

        public static List<EntityGroup> GetEntityGroups()
        {
            int intTotalCount = 0;
            return GetEntityGroups(null, null, null, out intTotalCount);
        }

        public static List<EntityGroup> GetEntityGroups(EntityGroupFilter Filter)
        {
            int intTotalCount = 0;
            return GetEntityGroups(Filter, null, null, out intTotalCount);
        }

        public static List<EntityGroup> GetEntityGroups(EntityGroupFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetEntityGroups(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<EntityGroup> GetEntityGroups(EntityGroupFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<EntityGroup> objReturn = null;
            EntityGroup objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<EntityGroup>();

                strSQL = "SELECT * " +
                         "FROM EntityGroup (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.Name != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Name, "Name");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "EntityGroupID" : Utility.CustomSorting.GetSortExpression(typeof(EntityGroup), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new EntityGroup(objData.Tables[0].Rows[i]);
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

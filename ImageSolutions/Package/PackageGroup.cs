using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Package
{
    public class PackageGroup : ISBase.BaseClass
    {
        public string PackageGroupID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(PackageGroupID); } }
        public string InternalID { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }

        public PackageGroup()
        {
        }
        public PackageGroup(string PackageGroupID)
        {
            this.PackageGroupID = PackageGroupID;
            Load();
        }
        public PackageGroup(DataRow objRow)
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
                         "FROM PackageGroup (NOLOCK) " +
                         "WHERE PackageGroupID=" + Database.HandleQuote(PackageGroupID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("PackageGroupID=" + PackageGroupID + " is not found");
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

                if (objColumns.Contains("PackageGroupID")) PackageGroupID = Convert.ToString(objRow["PackageGroupID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(PackageGroupID)) throw new Exception("Missing PackageGroupID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, PackageGroupID already exists");

                dicParam["InternalID"] = InternalID;
                dicParam["Name"] = Name;

                PackageGroupID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "PackageGroup"), objConn, objTran).ToString();

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
                if (PackageGroupID == null) throw new Exception("PackageGroupID is required");
                if (IsNew) throw new Exception("Update cannot be performed, PackageGroupID is missing");

                dicParam["InternalID"] = InternalID;
                dicParam["Name"] = Name;
                dicWParam["PackageGroupID"] = PackageGroupID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "PackageGroup"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, PackageGroupID is missing");

                dicDParam["PackageGroupID"] = PackageGroupID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "PackageGroup"), objConn, objTran);
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

        public static PackageGroup GetPackageGroup(PackageGroupFilter Filter)
        {
            List<PackageGroup> objPackageGroups = null;
            PackageGroup objReturn = null;

            try
            {
                objPackageGroups = GetPackageGroups(Filter);
                if (objPackageGroups != null && objPackageGroups.Count >= 1) objReturn = objPackageGroups[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objPackageGroups = null;
            }
            return objReturn;
        }

        public static List<PackageGroup> GetPackageGroups()
        {
            int intTotalCount = 0;
            return GetPackageGroups(null, null, null, out intTotalCount);
        }

        public static List<PackageGroup> GetPackageGroups(PackageGroupFilter Filter)
        {
            int intTotalCount = 0;
            return GetPackageGroups(Filter, null, null, out intTotalCount);
        }

        public static List<PackageGroup> GetPackageGroups(PackageGroupFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetPackageGroups(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<PackageGroup> GetPackageGroups(PackageGroupFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<PackageGroup> objReturn = null;
            PackageGroup objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<PackageGroup>();

                strSQL = "SELECT * " +
                         "FROM PackageGroup (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.PackageGroupID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PackageGroupID, "PackageGroupID");
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.Name != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Name, "CategoryID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "PackageGroupID" : Utility.CustomSorting.GetSortExpression(typeof(PackageGroup), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new PackageGroup(objData.Tables[0].Rows[i]);
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

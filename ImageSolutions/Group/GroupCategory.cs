using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Group
{
    public class GroupCategory : ISBase.BaseClass
    {
        public string GroupCategoryID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(GroupCategoryID); } }
        public string GroupID { get; set; }
        public string CategoryID { get; set; }
        public DateTime CreatedOn { get; set; }

        public GroupCategory()
        {
        }
        public GroupCategory(string GroupCategoryID)
        {
            this.GroupCategoryID = GroupCategoryID;
            Load();
        }
        public GroupCategory(DataRow objRow)
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
                         "FROM GroupCategory (NOLOCK) " +
                         "WHERE GroupCategoryID=" + Database.HandleQuote(GroupCategoryID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("GroupCategoryID=" + GroupCategoryID + " is not found");
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

                if (objColumns.Contains("GroupCategoryID")) GroupCategoryID = Convert.ToString(objRow["GroupCategoryID"]);
                if (objColumns.Contains("GroupID")) GroupID = Convert.ToString(objRow["GroupID"]);
                if (objColumns.Contains("CategoryID")) CategoryID = Convert.ToString(objRow["CategoryID"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(GroupCategoryID)) throw new Exception("Missing GroupCategoryID in the datarow");
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
                if (GroupID == null) throw new Exception("GroupID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, GroupCategoryID already exists");

                dicParam["GroupID"] = GroupID;
                dicParam["CategoryID"] = CategoryID;

                GroupCategoryID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "GroupCategory"), objConn, objTran).ToString();

                //foreach (GroupCategoryLine objGroupCategoryLine in GroupCategoryLines)
                //{
                //    objGroupCategoryLine.GroupCategoryID = GroupCategoryID;
                //    objGroupCategoryLine.Create(objConn, objTran);
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
                if (GroupCategoryID == null) throw new Exception("GroupCategoryID is required");
                if (IsNew) throw new Exception("Update cannot be performed, GroupCategoryID is missing");

                dicParam["GroupID"] = GroupID;
                dicParam["CategoryID"] = CategoryID;

                dicWParam["GroupCategoryID"] = GroupCategoryID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "GroupCategory"), objConn, objTran);

                //foreach (GroupCategoryLine objGroupCategoryLine in GroupCategoryLines)
                //{
                //    objGroupCategoryLine.Update(objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, GroupCategoryID is missing");

                //Delete Lines
                //List<GroupCategoryLine> lstGroupCategoryLine;
                //lstGroupCategoryLine = GroupCategoryLines;
                //foreach (GroupCategoryLine _GroupCategoryLine in lstGroupCategoryLine)
                //{
                //    _GroupCategoryLine.Delete(objConn, objTran);
                //}

                dicDParam["GroupCategoryID"] = GroupCategoryID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "GroupCategory"), objConn, objTran);
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

        public static GroupCategory GetGroupCategory(GroupCategoryFilter Filter)
        {
            List<GroupCategory> objGroupCategorys = null;
            GroupCategory objReturn = null;

            try
            {
                objGroupCategorys = GetGroupCategorys(Filter);
                if (objGroupCategorys != null && objGroupCategorys.Count >= 1) objReturn = objGroupCategorys[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objGroupCategorys = null;
            }
            return objReturn;
        }

        public static List<GroupCategory> GetGroupCategorys()
        {
            int intTotalCount = 0;
            return GetGroupCategorys(null, null, null, out intTotalCount);
        }

        public static List<GroupCategory> GetGroupCategorys(GroupCategoryFilter Filter)
        {
            int intTotalCount = 0;
            return GetGroupCategorys(Filter, null, null, out intTotalCount);
        }

        public static List<GroupCategory> GetGroupCategorys(GroupCategoryFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetGroupCategorys(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<GroupCategory> GetGroupCategorys(GroupCategoryFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<GroupCategory> objReturn = null;
            GroupCategory objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<GroupCategory>();

                strSQL = "SELECT * " +
                         "FROM GroupCategory (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.GroupCategoryID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.GroupCategoryID, "GroupCategoryID");
                    if (Filter.GroupID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.GroupID, "GroupID");
                    if (Filter.CategoryID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CategoryID, "CategoryID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "GroupCategoryID" : Utility.CustomSorting.GetSortExpression(typeof(GroupCategory), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new GroupCategory(objData.Tables[0].Rows[i]);
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

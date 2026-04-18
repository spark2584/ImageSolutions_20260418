using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Category
{
    public class Category : ISBase.BaseClass
    {
        public string CategoryID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(CategoryID); } }
        public string CategoryName { get; set; }
        public DateTime CreatedOn { get; set; }

        public Category()
        {
        }
        public Category(string CategoryID)
        {
            this.CategoryID = CategoryID;
            Load();
        }
        public Category(DataRow objRow)
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
                         "FROM Category (NOLOCK) " +
                         "WHERE CategoryID=" + Database.HandleQuote(CategoryID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("CategoryID=" + CategoryID + " is not found");
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

                if (objColumns.Contains("CategoryID")) CategoryID = Convert.ToString(objRow["CategoryID"]);
                if (objColumns.Contains("CategoryName")) CategoryName = Convert.ToString(objRow["CategoryName"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(CategoryID)) throw new Exception("Missing CategoryID in the datarow");
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
                if (CategoryName == null) throw new Exception("CategoryName is required");
                if (!IsNew) throw new Exception("Create cannot be performed, CategoryID already exists");

                dicParam["CategoryName"] = CategoryName;

                CategoryID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Category"), objConn, objTran).ToString();

                //foreach (CategoryLine objCategoryLine in CategoryLines)
                //{
                //    objCategoryLine.CategoryID = CategoryID;
                //    objCategoryLine.Create(objConn, objTran);
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
                if (CategoryID == null) throw new Exception("CategoryID is required");
                if (IsNew) throw new Exception("Update cannot be performed, CategoryID is missing");

                dicParam["CategoryName"] = CategoryName;

                dicWParam["CategoryID"] = CategoryID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Category"), objConn, objTran);

                //foreach (CategoryLine objCategoryLine in CategoryLines)
                //{
                //    objCategoryLine.Update(objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, CategoryID is missing");

                //Delete Lines
                //List<CategoryLine> lstCategoryLine;
                //lstCategoryLine = CategoryLines;
                //foreach (CategoryLine _CategoryLine in lstCategoryLine)
                //{
                //    _CategoryLine.Delete(objConn, objTran);
                //}

                dicDParam["CategoryID"] = CategoryID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Category"), objConn, objTran);
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

        public static Category GetCategory(CategoryFilter Filter)
        {
            List<Category> objCategorys = null;
            Category objReturn = null;

            try
            {
                objCategorys = GetCategorys(Filter);
                if (objCategorys != null && objCategorys.Count >= 1) objReturn = objCategorys[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCategorys = null;
            }
            return objReturn;
        }

        public static List<Category> GetCategorys()
        {
            int intTotalCount = 0;
            return GetCategorys(null, null, null, out intTotalCount);
        }

        public static List<Category> GetCategorys(CategoryFilter Filter)
        {
            int intTotalCount = 0;
            return GetCategorys(Filter, null, null, out intTotalCount);
        }

        public static List<Category> GetCategorys(CategoryFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetCategorys(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Category> GetCategorys(CategoryFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Category> objReturn = null;
            Category objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Category>();

                strSQL = "SELECT * " +
                         "FROM Category (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.CategoryID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CategoryID, "CategoryID");
                    if (Filter.CategoryName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CategoryName, "CategoryName");

                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "CategoryID" : Utility.CustomSorting.GetSortExpression(typeof(Category), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Category(objData.Tables[0].Rows[i]);
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

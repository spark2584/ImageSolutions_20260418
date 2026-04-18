using ImageSolutions.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Website
{
    public class WebsiteTag : ISBase.BaseClass
    {
        public string WebsiteTagID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(WebsiteTagID); } }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public DateTime CreatedOn { get; set; }
        public WebsiteTag()
        {
        }
        public WebsiteTag(string WebsiteTagID)
        {
            this.WebsiteTagID = WebsiteTagID;
            Load();
        }
        public WebsiteTag(DataRow objRow)
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
                         "FROM WebsiteTag (NOLOCK) " +
                         "WHERE WebsiteTagID=" + Database.HandleQuote(WebsiteTagID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("WebsiteTagID=" + WebsiteTagID + " is not found");
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

                if (objColumns.Contains("WebsiteTagID")) WebsiteTagID = Convert.ToString(objRow["WebsiteTagID"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("Description")) Description = Convert.ToString(objRow["Description"]);
                if (objColumns.Contains("Value")) Value = Convert.ToString(objRow["Value"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(WebsiteTagID)) throw new Exception("Missing WebsiteTagID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, WebsiteTagID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["Name"] = Name;
                dicParam["Description"] = Description;
                dicParam["Value"] = Value;

                WebsiteTagID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "WebsiteTag"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, WebsiteTagID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");


                dicParam["Name"] = Name;
                dicParam["Description"] = Description;
                dicParam["Value"] = Value;
                dicWParam["WebsiteTagID"] = WebsiteTagID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "WebsiteTag"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, WebsiteTagID is missing");

                dicDParam["WebsiteTagID"] = WebsiteTagID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "WebsiteTag"), objConn, objTran);
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

        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                     "FROM WebsiteTag (NOLOCK) p " +
                     "WHERE " +
                     "(" +
                     "  p.Name=" + Database.HandleQuote(Name) + ")" +
                     ") ";

            if (!string.IsNullOrEmpty(WebsiteTagID)) strSQL += "AND p.WebsiteTagID<>" + Database.HandleQuote(WebsiteTagID);
            return Database.HasRows(strSQL);
        }

        public static WebsiteTag GetWebsiteTag(WebsiteTagFilter Filter)
        {
            List<WebsiteTag> objWebsiteTags = null;
            WebsiteTag objReturn = null;

            try
            {
                objWebsiteTags = GetWebsiteTags(Filter);
                if (objWebsiteTags != null && objWebsiteTags.Count >= 1) objReturn = objWebsiteTags[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebsiteTags = null;
            }
            return objReturn;
        }

        public static List<WebsiteTag> GetWebsiteTags()
        {
            int intTotalCount = 0;
            return GetWebsiteTags(null, null, null, out intTotalCount);
        }

        public static List<WebsiteTag> GetWebsiteTags(WebsiteTagFilter Filter)
        {
            int intTotalCount = 0;
            return GetWebsiteTags(Filter, null, null, out intTotalCount);
        }

        public static List<WebsiteTag> GetWebsiteTags(WebsiteTagFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetWebsiteTags(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<WebsiteTag> GetWebsiteTags(WebsiteTagFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<WebsiteTag> objReturn = null;
            WebsiteTag objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<WebsiteTag>();

                strSQL = "SELECT * " +
                         "FROM WebsiteTag (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.Name != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Name, "TabName");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "WebsiteTagID" : Utility.CustomSorting.GetSortExpression(typeof(WebsiteTag), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new WebsiteTag(objData.Tables[0].Rows[i]);
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

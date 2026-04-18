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

namespace ImageSolutions.Customization
{
    public class Customization : ISBase.BaseClass
    {
        public string CustomizationID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(CustomizationID); } }
        public string CustomizationName { get; set; }
        public string ItemID { get; set; }
        public string WebsiteTabID { get; set; }
        public string WebsiteID { get; set; }
        public int Sort { get; set; }

        private ImageSolutions.Item.Item mItem = null;
        public ImageSolutions.Item.Item Item
        {
            get
            {
                if (mItem == null && !string.IsNullOrEmpty(ItemID))
                {
                    try
                    {
                        mItem = new ImageSolutions.Item.Item(ItemID);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                    }
                }
                return mItem;
            }
        }
        public Customization()
        {
        }
        public Customization(string CustomizationID)
        {
            this.CustomizationID = CustomizationID;
            Load();
        }
        public Customization(DataRow objRow)
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
                         "FROM Customization (NOLOCK) " +
                         "WHERE CustomizationID=" + Database.HandleQuote(CustomizationID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("CustomizationID=" + CustomizationID + " is not found");
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


        protected void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("CustomizationID")) CustomizationID = Convert.ToString(objRow["CustomizationID"]);
                if (objColumns.Contains("CustomizationName")) CustomizationName = Convert.ToString(objRow["CustomizationName"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("WebsiteTabID")) WebsiteTabID = Convert.ToString(objRow["WebsiteTabID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);

                if (string.IsNullOrEmpty(CustomizationID)) throw new Exception("Missing CustomizationID in the datarow");
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
                if (string.IsNullOrEmpty(CustomizationName)) throw new Exception("CustomizationName is required");

                if (!IsNew) throw new Exception("Create cannot be performed, CustomizationID already exists");

                dicParam["CustomizationName"] = CustomizationName;
                dicParam["ItemID"] = ItemID;
                dicParam["WebsiteTabID"] = WebsiteTabID;
                dicParam["WebsiteID"] = WebsiteID;

                CustomizationID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Customization"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(CustomizationName)) throw new Exception("CustomizationName is required");

                if (IsNew) throw new Exception("Update cannot be performed, CustomizationID is missing");

                dicParam["CustomizationName"] = CustomizationName;
                dicParam["ItemID"] = ItemID;
                dicParam["WebsiteTabID"] = WebsiteTabID;
                dicParam["WebsiteID"] = WebsiteID;

                dicWParam["CustomizationID"] = CustomizationID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Customization"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, CustomizationID is missing");

                dicDParam["CustomizationID"] = CustomizationID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Customization"), objConn, objTran);
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

        public static Customization GetCustomization(CustomizationFilter Filter)
        {
            List<Customization> objCustomizations = null;
            Customization objReturn = null;

            try
            {
                objCustomizations = GetCustomizations(Filter);
                if (objCustomizations != null && objCustomizations.Count >= 1) objReturn = objCustomizations[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomizations = null;
            }
            return objReturn;
        }

        public static List<Customization> GetCustomizations()
        {
            int intTotalCount = 0;
            return GetCustomizations(null, null, null, out intTotalCount);
        }

        public static List<Customization> GetCustomizations(CustomizationFilter Filter)
        {
            int intTotalCount = 0;
            return GetCustomizations(Filter, null, null, out intTotalCount);
        }

        public static List<Customization> GetCustomizations(CustomizationFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetCustomizations(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Customization> GetCustomizations(CustomizationFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Customization> objReturn = null;
            Customization objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Customization>();

                strSQL = "SELECT * " +
                         "FROM Customization (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                    if (Filter.WebsiteTabID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteTabID, "WebsiteTabID");
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "CustomizationID" : Utility.CustomSorting.GetSortExpression(typeof(Customization), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Customization(objData.Tables[0].Rows[i]);
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

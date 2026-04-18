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

namespace ImageSolutions.Item
{
    public class ItemSelectableLogo : ISBase.BaseClass
    {
        public string ItemSelectableLogoID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemSelectableLogoID); } }
        public string ItemID { get; set; }
        public string SelectableLogoID { get; set; }
        public int? PositionXPercent { get; set; }
        public int? PositionYPercent { get; set; }
        public int? Width { get; set; }        
        public string ImageURL { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        private List<ImageSolutions.Website.WebsiteGroupItemSelectableLogo> mWebsiteGroupItemSelectableLogos = null;
        public List<ImageSolutions.Website.WebsiteGroupItemSelectableLogo> WebsiteGroupItemSelectableLogos
        {
            get
            {
                if (mWebsiteGroupItemSelectableLogos == null && !string.IsNullOrEmpty(ItemSelectableLogoID))
                {
                    ImageSolutions.Website.WebsiteGroupItemSelectableLogoFilter objFilter = null;

                    try
                    {
                        objFilter = new ImageSolutions.Website.WebsiteGroupItemSelectableLogoFilter();
                        objFilter.ItemSelectableLogoID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemSelectableLogoID.SearchString = ItemSelectableLogoID;
                        mWebsiteGroupItemSelectableLogos = ImageSolutions.Website.WebsiteGroupItemSelectableLogo.GetWebsiteGroupItemSelectableLogos(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mWebsiteGroupItemSelectableLogos;
            }
        }

        private ImageSolutions.Item.Item mItem = null;
        public ImageSolutions.Item.Item Item
        {
            get
            {
                if (mItem == null && !string.IsNullOrEmpty(ItemID))
                {
                    mItem = new ImageSolutions.Item.Item(ItemID);
                }
                return mItem;
            }
        }

        private SelectableLogo.SelectableLogo mSelectableLogo = null;
        public SelectableLogo.SelectableLogo SelectableLogo
        {
            get
            {
                if (mSelectableLogo == null && !string.IsNullOrEmpty(SelectableLogoID))
                {
                    mSelectableLogo = new SelectableLogo.SelectableLogo(SelectableLogoID);
                }
                return mSelectableLogo;
            }
        }

        public string Description
        {
            get
            {
                return string.Format("{0}", SelectableLogo.Name);
                //return string.Format("{0} ({1})", SelectableLogo.Name, SelectableLogo.LogoPosition);
            }
        }

        public ItemSelectableLogo()
        {
        }
        public ItemSelectableLogo(string ItemSelectableLogoID)
        {
            this.ItemSelectableLogoID = ItemSelectableLogoID;
            Load();
        }
        public ItemSelectableLogo(DataRow objRow)
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
                         "FROM ItemSelectableLogo (NOLOCK) " +
                         "WHERE ItemSelectableLogoID=" + Database.HandleQuote(ItemSelectableLogoID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemSelectableLogoID=" + ItemSelectableLogoID + " is not found");
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

                if (objColumns.Contains("ItemSelectableLogoID")) ItemSelectableLogoID = Convert.ToString(objRow["ItemSelectableLogoID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("SelectableLogoID")) SelectableLogoID = Convert.ToString(objRow["SelectableLogoID"]);
                if (objColumns.Contains("PositionXPercent") && objRow["PositionXPercent"] != DBNull.Value) PositionXPercent = Convert.ToInt32(objRow["PositionXPercent"]);
                if (objColumns.Contains("PositionYPercent") && objRow["PositionYPercent"] != DBNull.Value) PositionYPercent = Convert.ToInt32(objRow["PositionYPercent"]);
                if (objColumns.Contains("Width") && objRow["Width"] != DBNull.Value) Width = Convert.ToInt32(objRow["Width"]);
                if (objColumns.Contains("ImageURL")) ImageURL = Convert.ToString(objRow["ImageURL"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemSelectableLogoID)) throw new Exception("Missing ItemSelectableLogoID in the datarow");
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
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["ItemID"] = ItemID;
                dicParam["SelectableLogoID"] = SelectableLogoID;
                dicParam["PositionXPercent"] = PositionXPercent;
                dicParam["PositionYPercent"] = PositionYPercent;
                dicParam["Width"] = Width;
                dicParam["ImageURL"] = ImageURL;
                dicParam["CreatedBy"] = CreatedBy;

                ItemSelectableLogoID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemSelectableLogo"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(ItemSelectableLogoID)) throw new Exception("ItemSelectableLogoID is required");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["ItemID"] = ItemID;
                dicParam["SelectableLogoID"] = SelectableLogoID;
                dicParam["PositionXPercent"] = PositionXPercent;
                dicParam["PositionYPercent"] = PositionYPercent;
                dicParam["Width"] = Width;
                dicParam["ImageURL"] = ImageURL;

                dicWParam["ItemSelectableLogoID"] = ItemSelectableLogoID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemSelectableLogo"), objConn, objTran);

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
                dicDParam["ItemSelectableLogoID"] = ItemSelectableLogoID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemSelectableLogo"), objConn, objTran);
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

            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 i.* " +
                        "FROM ItemSelectableLogo (NOLOCK) i " +
                        "WHERE " +                        
                        "i.SelectableLogoID=" + Database.HandleQuote(SelectableLogoID) + " AND i.ItemID=" + Database.HandleQuote(ItemID) ;

            if (!string.IsNullOrEmpty(ItemSelectableLogoID)) strSQL += "AND i.ItemSelectableLogoID<>" + Database.HandleQuote(ItemSelectableLogoID);
            return Database.HasRows(strSQL);
        }

        public static ItemSelectableLogo GetItemSelectableLogo(ItemSelectableLogoFilter Filter)
        {
            List<ItemSelectableLogo> objTaskEntries = null;
            ItemSelectableLogo objReturn = null;

            try
            {
                objTaskEntries = GetItemSelectableLogos(Filter);
                if (objTaskEntries != null && objTaskEntries.Count >= 1) objReturn = objTaskEntries[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objTaskEntries = null;
            }
            return objReturn;
        }

        public static List<ItemSelectableLogo> GetItemSelectableLogos()
        {
            int intTotalCount = 0;
            return GetItemSelectableLogos(null, null, null, out intTotalCount);
        }

        public static List<ItemSelectableLogo> GetItemSelectableLogos(ItemSelectableLogoFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemSelectableLogos(Filter, null, null, out intTotalCount);
        }

        public static List<ItemSelectableLogo> GetItemSelectableLogos(ItemSelectableLogoFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemSelectableLogos(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemSelectableLogo> GetItemSelectableLogos(ItemSelectableLogoFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemSelectableLogo> objReturn = null;
            ItemSelectableLogo objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemSelectableLogo>();

                strSQL = "SELECT * " +
                         "FROM ItemSelectableLogo (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                    if (Filter.SelectableLogoID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SelectableLogoID, "SelectableLogoID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemSelectableLogoID" : Utility.CustomSorting.GetSortExpression(typeof(ItemSelectableLogo), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemSelectableLogo(objData.Tables[0].Rows[i]);
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

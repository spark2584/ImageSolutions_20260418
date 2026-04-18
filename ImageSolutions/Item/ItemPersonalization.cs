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


namespace ImageSolutions.Item
{
    public class ItemPersonalization : ISBase.BaseClass
    {
        public string ItemPersonalizationID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemPersonalizationID); } }
        public string ItemID { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public int? Sort { get; set; }
        public string DefaultValue { get; set; }
        public bool RequireVerification { get; set; }
        public bool RequireApproval { get; set; }
        public bool AllowBlank { get; set; }
        public bool BypassApprovalForBlank { get; set; }
        public string DefaultValueLabel { get; set; }
        public string FreeTextValueLabel { get; set; }
        public bool InActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        private List<ItemPersonalizationValueList> mItemPersonalizationValueLists = null;
        public List<ItemPersonalizationValueList> ItemPersonalizationValueLists
        {
            get
            {
                if (mItemPersonalizationValueLists == null && !string.IsNullOrEmpty(ItemPersonalizationID))
                {
                    ItemPersonalizationValueListFilter objFilter = null;

                    try
                    {
                        objFilter = new ItemPersonalizationValueListFilter();
                        objFilter.ItemPersonalizationID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemPersonalizationID.SearchString = ItemPersonalizationID;
                        mItemPersonalizationValueLists = ItemPersonalizationValueList.GetItemPersonalizationValueLists(objFilter);
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
                return mItemPersonalizationValueLists;
            }
        }

        public ItemPersonalization()
        {
        }
        public ItemPersonalization(string ItemPersonalizationID)
        {
            this.ItemPersonalizationID = ItemPersonalizationID;
            Load();
        }
        public ItemPersonalization(DataRow objRow)
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
                         "FROM ItemPersonalization (NOLOCK) " +
                         "WHERE ItemPersonalizationID=" + Database.HandleQuote(ItemPersonalizationID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemPersonalizationID=" + ItemPersonalizationID + " is not found");
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

                if (objColumns.Contains("ItemPersonalizationID")) ItemPersonalizationID = Convert.ToString(objRow["ItemPersonalizationID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("Label")) Label = Convert.ToString(objRow["Label"]);
                if (objColumns.Contains("Type")) Type = Convert.ToString(objRow["Type"]);
                if (objColumns.Contains("DefaultValue")) DefaultValue = Convert.ToString(objRow["DefaultValue"]);
                if (objColumns.Contains("Sort") && objRow["Sort"] != DBNull.Value) Sort = Convert.ToInt32(objRow["Sort"]);
                if (objColumns.Contains("RequireVerification")) RequireVerification = Convert.ToBoolean(objRow["RequireVerification"]);
                if (objColumns.Contains("RequireApproval")) RequireApproval = Convert.ToBoolean(objRow["RequireApproval"]);
                if (objColumns.Contains("AllowBlank")) AllowBlank = Convert.ToBoolean(objRow["AllowBlank"]);
                if (objColumns.Contains("BypassApprovalForBlank")) BypassApprovalForBlank = Convert.ToBoolean(objRow["BypassApprovalForBlank"]);
                if (objColumns.Contains("DefaultValueLabel")) DefaultValueLabel = Convert.ToString(objRow["DefaultValueLabel"]);
                if (objColumns.Contains("FreeTextValueLabel")) FreeTextValueLabel = Convert.ToString(objRow["FreeTextValueLabel"]);
                if (objColumns.Contains("InActive")) InActive = Convert.ToBoolean(objRow["InActive"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemPersonalizationID)) throw new Exception("Missing ItemPersonalizationID in the datarow");
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
                if (ItemID == null) throw new Exception("ItemID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemPersonalizationID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["ItemID"] = ItemID;
                dicParam["Name"] = Name;
                dicParam["Label"] = Label;
                dicParam["Type"] = Type;
                dicParam["DefaultValue"] = DefaultValue;
                dicParam["Sort"] = Sort;
                dicParam["RequireVerification"] = RequireVerification;
                dicParam["RequireApproval"] = RequireApproval;
                dicParam["AllowBlank"] = AllowBlank;
                dicParam["BypassApprovalForBlank"] = BypassApprovalForBlank;
                dicParam["InActive"] = InActive;
                dicParam["CreatedBy"] = CreatedBy;

                ItemPersonalizationID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ItemPersonalization"), objConn, objTran).ToString();

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
                if (ItemID == null) throw new Exception("ItemID is required");
                if (IsNew) throw new Exception("Update cannot be performed, ItemPersonalizationID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["ItemID"] = ItemID;
                dicParam["Name"] = Name;
                dicParam["Label"] = Label;
                dicParam["Type"] = Type;
                dicParam["DefaultValue"] = DefaultValue;
                dicParam["Sort"] = Sort;
                dicParam["RequireVerification"] = RequireVerification;
                dicParam["RequireApproval"] = RequireApproval;
                dicParam["DefaultValueLabel"] = DefaultValueLabel;
                dicParam["FreeTextValueLabel"] = FreeTextValueLabel;
                dicParam["AllowBlank"] = AllowBlank;
                dicParam["BypassApprovalForBlank"] = BypassApprovalForBlank;
                dicParam["InActive"] = InActive;
                dicWParam["ItemPersonalizationID"] = ItemPersonalizationID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ItemPersonalization"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemPersonalizationID is missing");
                dicDParam["ItemPersonalizationID"] = ItemPersonalizationID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ItemPersonalization"), objConn, objTran);
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
            return false;
        }

        public static ItemPersonalization GetItemPersonalization(ItemPersonalizationFilter Filter)
        {
            List<ItemPersonalization> objItemPersonalizations = null;
            ItemPersonalization objReturn = null;

            try
            {
                objItemPersonalizations = GetItemPersonalizations(Filter);
                if (objItemPersonalizations != null && objItemPersonalizations.Count >= 1) objReturn = objItemPersonalizations[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemPersonalizations = null;
            }
            return objReturn;
        }

        public static List<ItemPersonalization> GetItemPersonalizations()
        {
            int intTotalCount = 0;
            return GetItemPersonalizations(null, null, null, out intTotalCount);
        }

        public static List<ItemPersonalization> GetItemPersonalizations(ItemPersonalizationFilter Filter)
        {
            int intTotalCount = 0;
            return GetItemPersonalizations(Filter, null, null, out intTotalCount);
        }

        public static List<ItemPersonalization> GetItemPersonalizations(ItemPersonalizationFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItemPersonalizations(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ItemPersonalization> GetItemPersonalizations(ItemPersonalizationFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ItemPersonalization> objReturn = null;
            ItemPersonalization objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ItemPersonalization>();

                strSQL = "SELECT * " +
                         "FROM ItemPersonalization (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "ItemID");
                    if (Filter.Sort != null) strSQL += "AND Sort=" + Database.HandleQuote(Convert.ToInt32(Filter.Sort).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "Sort" : Utility.CustomSorting.GetSortExpression(typeof(ItemPersonalization), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else strSQL += " ORDER BY Sort";

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ItemPersonalization(objData.Tables[0].Rows[i]);
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

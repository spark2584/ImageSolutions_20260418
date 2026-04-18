using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections;
using System.ComponentModel;
using System.Data;

namespace ImageSolutions.Promotion
{
    [Serializable]
    public class PromotionGetTrans : ISBase.BaseClass
    {
        public string PromotionGetTransID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(PromotionGetTransID); } }
        public string PromotionTransID { get; set; }
        public string PromotionGetID { get; set; }
        public string WebsiteID { get; set; }
        public string PromotionID { get; set; }
        //public string ItemDetailID { get; set; }
        //private ItemDetail mItemDetail = null;
        //public ItemDetail ItemDetail
        //{
        //    get
        //    {
        //        if (mItemDetail == null && !string.IsNullOrEmpty(ItemDetailID) && !string.IsNullOrEmpty(BusinessID))
        //        {
        //            mItemDetail = new ItemDetail(BusinessID, ItemDetailID);
        //        }
        //        return mItemDetail;
        //    }
        //}
        public string WebsiteTabID { get; set; }
        private Website.WebsiteTab mWebsiteTab = null;
        public Website.WebsiteTab WebsiteTab
        {
            get
            {
                if (mWebsiteTab == null && !string.IsNullOrEmpty(WebsiteTabID) && !string.IsNullOrEmpty(WebsiteID))
                {
                    mWebsiteTab = new Website.WebsiteTab(WebsiteTabID);
                }
                return mWebsiteTab;
            }
        }
        public string ItemID { get; set; }
        private Item.Item mItem = null;
        public Item.Item Item
        {
            get
            {
                if (mItem == null && !string.IsNullOrEmpty(ItemID) && !string.IsNullOrEmpty(WebsiteID))
                {
                    mItem = new Item.Item(ItemID);
                }
                return mItem;
            }
        }
        public int? Quantity { get; set; }
        public bool IsConditionMet { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        public PromotionGetTrans()
        {
        }

        public PromotionGetTrans(string WebsiteID)
        {
            this.WebsiteID = WebsiteID;
        }

        public PromotionGetTrans(string WebsiteID, string PromotionGetTransID)
        {
            this.WebsiteID = WebsiteID;
            this.PromotionGetTransID = PromotionGetTransID;
            Load();
        }

        public PromotionGetTrans(DataRow objRow)
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
                         "FROM PromotionGetTrans (NOLOCK) " +
                         "WHERE WebsiteID=" + Database.HandleQuote(WebsiteID) +
                         "AND PromotionGetTransID=" + Database.HandleQuote(PromotionGetTransID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("PromotionGetTransID=" + PromotionGetTransID + " is not found");
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

                if (objColumns.Contains("PromotionGetTransID")) PromotionGetTransID = Convert.ToString(objRow["PromotionGetTransID"]);
                if (objColumns.Contains("PromotionTransID")) PromotionTransID = Convert.ToString(objRow["PromotionTransID"]);
                if (objColumns.Contains("PromotionGetID")) PromotionGetID = Convert.ToString(objRow["PromotionGetID"]);
                if (objColumns.Contains("PromotionID")) PromotionID = Convert.ToString(objRow["PromotionID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("WebsiteTabID")) WebsiteTabID = Convert.ToString(objRow["WebsiteTabID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("Quantity") && objRow["Quantity"] != DBNull.Value) Quantity = Convert.ToInt32(objRow["Quantity"]);
                if (objColumns.Contains("IsActive")) IsActive = Convert.ToBoolean(objRow["IsActive"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(PromotionGetTransID)) throw new Exception("Missing PromotionGetTransID in the datarow");
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

            Hashtable dicParam = new Hashtable();

            try
            {
                if (string.IsNullOrEmpty(PromotionTransID)) throw new Exception("PromotionTransID is required");
                if (string.IsNullOrEmpty(PromotionGetID)) throw new Exception("PromotionGetID is required");
                if (string.IsNullOrEmpty(WebsiteID)) throw new Exception("WebsiteID is required");
                if (string.IsNullOrEmpty(PromotionID)) throw new Exception("PromotionID is required");
                //if (string.IsNullOrEmpty(ItemCategoryID) && string.IsNullOrEmpty(ItemID) && string.IsNullOrEmpty(ItemDetailID)) throw new Exception("One of the promotion rule must be specified");
                if (Quantity != null && Quantity < 0) throw new Exception("Min quantity has to be greater than or equal to 0");
                if (!IsNew) throw new Exception("Create cannot be performed, PromotionGetID already exists");
                
                dicParam["PromotionTransID"] = PromotionTransID;
                dicParam["PromotionGetID"] = PromotionGetID;
                dicParam["WebsiteID"] = WebsiteID;
                dicParam["PromotionID"] = PromotionID;
                dicParam["WebsiteTabID"] = WebsiteTabID;
                dicParam["ItemID"] = ItemID;
                //dicParam["ItemDetailID"] = ItemDetailID;
                dicParam["Quantity"] = Quantity;
                dicParam["IsActive"] = IsActive;
                dicParam["CreatedBy"] = CreatedBy;
                PromotionGetID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "PromotionGetTrans"), objConn, objTran).ToString();
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

            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (string.IsNullOrEmpty(PromotionTransID)) throw new Exception("PromotionTransID is required");
                if (string.IsNullOrEmpty(PromotionGetID)) throw new Exception("PromotionGetID is required");
                if (string.IsNullOrEmpty(WebsiteID)) throw new Exception("WebsiteID is required");
                if (string.IsNullOrEmpty(PromotionID)) throw new Exception("PromotionID is required");
                //if (string.IsNullOrEmpty(ItemCategoryID) && string.IsNullOrEmpty(ItemID) && string.IsNullOrEmpty(ItemDetailID)) throw new Exception("One of the promotion rule must be specified");
                if (Quantity != null && Quantity < 0) throw new Exception("Min quantity has to be greater than or equal to 0");
                if (IsNew) throw new Exception("Update cannot be performed, PromotionGetID is missing");
                
                dicParam["PromotionTransID"] = PromotionTransID;
                dicParam["PromotionGetID"] = PromotionGetID;
                dicParam["WebsiteID"] = WebsiteID;
                dicParam["PromotionID"] = PromotionID;
                dicParam["WebsiteTabID"] = WebsiteTabID;
                dicParam["ItemID"] = ItemID;
                //dicParam["ItemDetailID"] = ItemDetailID;
                dicParam["Quantity"] = Quantity;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = "_#GETUTCDATE()";
                dicWParam["PromotionGetTransID"] = PromotionGetTransID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "PromotionGetTrans"), objConn, objTran);
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

            Hashtable dicWParam = new Hashtable();

            try
            {
                dicWParam["PromotionGetTransID"] = PromotionGetTransID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicWParam, "PromotionGetTrans"), objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicWParam = null;
            }
            return true;
        }

        public static List<PromotionGetTrans> GetPromotionGetTrans(string WebsiteID, string PromotionGetTransID)
        {
            List<PromotionGetTrans> objReturn = null;
            PromotionGetTrans objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(PromotionGetTransID))
                {
                    objReturn = new List<PromotionGetTrans>();

                    strSQL = "SELECT * " +
                             "FROM PromotionGetTrans (NOLOCK) " +
                             "WHERE IsActive=1 " +
                             "AND WebsiteID=" + Database.HandleQuote(WebsiteID) +
                             "AND PromotionGetTransID=" + Database.HandleQuote(PromotionGetTransID);

                    objData = Database.GetDataSet(strSQL);

                    if (objData != null && objData.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                        {
                            objNew = new PromotionGetTrans(WebsiteID, objData.Tables[0].Rows[i]["PromotionGetTransID"].ToString());
                            objReturn.Add(objNew);
                        }
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

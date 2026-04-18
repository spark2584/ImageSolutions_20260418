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
    public class PromotionBuy : ISBase.BaseClass
    {
        public string PromotionBuyID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(PromotionBuyID); } }
        public string WebsiteID { get; private set; }
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
                if (mItem == null && !string.IsNullOrEmpty(ItemID))
                {
                    mItem = new Item.Item(ItemID);
                }
                return mItem;
            }
        }
        public int? Quantity { get; set; }
        //public bool IsConditionMet { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        public PromotionBuy()
        {
        }

        public PromotionBuy(string WebsiteID)
        {
            this.WebsiteID = WebsiteID;
        }

        public PromotionBuy(string WebsiteID, string PromotionBuyID)
        {
            this.WebsiteID = WebsiteID;
            this.PromotionBuyID = PromotionBuyID;
            Load();
        }

        public PromotionBuy(DataRow objRow)
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
                         "FROM PromotionBuy (NOLOCK) " +
                         "WHERE WebsiteID=" + Database.HandleQuote(WebsiteID) +
                         "AND PromotionBuyID=" + Database.HandleQuote(PromotionBuyID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("PromotionBuyID=" + PromotionBuyID + " is not found");
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

                if (objColumns.Contains("PromotionBuyID")) PromotionBuyID = Convert.ToString(objRow["PromotionBuyID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("PromotionID")) PromotionID = Convert.ToString(objRow["PromotionID"]);
                if (objColumns.Contains("WebsiteTabID")) WebsiteTabID = Convert.ToString(objRow["WebsiteTabID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                //if (objColumns.Contains("ItemDetailID")) ItemDetailID = Convert.ToString(objRow["ItemDetailID"]);
                if (objColumns.Contains("Quantity") && objRow["Quantity"] != DBNull.Value) Quantity = Convert.ToInt32(objRow["Quantity"]);
                if (objColumns.Contains("IsActive")) IsActive = Convert.ToBoolean(objRow["IsActive"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(PromotionBuyID)) throw new Exception("Missing PromotionBuyID in the datarow");
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
            Promotion objPromotion = null;

            try
            {
                if (string.IsNullOrEmpty(WebsiteID)) throw new Exception("WebsiteID is required");
                if (string.IsNullOrEmpty(PromotionID)) throw new Exception("PromotionID is required");
                //if (string.IsNullOrEmpty(ItemCategoryID) && string.IsNullOrEmpty(ItemID) && string.IsNullOrEmpty(ItemDetailID) && Quantity == null) throw new Exception("One of the promotion rule must be specified");
                if (Quantity != null && Quantity < 0) throw new Exception("Min quantity has to be greater than or equal to 0");
                if (!IsNew) throw new Exception("Create cannot be performed, PromotionBuyID already exists");
                objPromotion = new Promotion(WebsiteID, PromotionID);
                //if (objPromotion.DiscountAmount != null) throw new Exception("Promotion Buy cannot be specified for Discount Amount type");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["PromotionID"] = PromotionID;
                dicParam["WebsiteTabID"] = WebsiteTabID; //string.IsNullOrEmpty(WebsiteTabID) && string.IsNullOrEmpty(WebsiteTabID) ? WebsiteTabID : string.Empty;
                dicParam["ItemID"] = ItemID; //string.IsNullOrEmpty(ItemDetailID) ? ItemID : string.Empty;
                dicParam["Quantity"] = Quantity;
                dicParam["IsActive"] = IsActive;
                dicParam["CreatedBy"] = CreatedBy;
                PromotionBuyID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "PromotionBuy"), objConn, objTran).ToString();
                Load(objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicParam = null;
                objPromotion = null;
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
            Promotion objPromotion = null;

            try
            {
                if (string.IsNullOrEmpty(WebsiteID)) throw new Exception("WebsiteID is required");
                if (string.IsNullOrEmpty(PromotionID)) throw new Exception("PromotionID is required");
                //if (string.IsNullOrEmpty(ItemCategoryID) && string.IsNullOrEmpty(ItemID) && string.IsNullOrEmpty(ItemDetailID) && Quantity == null) throw new Exception("One of the promotion rule must be specified");
                if (Quantity != null && Quantity < 0) throw new Exception("Min quantity has to be greater than or equal to 0");
                if (IsNew) throw new Exception("Update cannot be performed, PromotionBuyID is missing");
                objPromotion = new Promotion(WebsiteID, PromotionID);
                //if (objPromotion.DiscountAmount != null) throw new Exception("Promotion Buy cannot be specified for Discount Amount type");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["PromotionID"] = PromotionID;
                //dicParam["ItemDetailID"] = ItemDetailID;
                dicParam["WebsiteTabID"] = WebsiteTabID; //string.IsNullOrEmpty(ItemDetailID) && string.IsNullOrEmpty(ItemID) ? ItemCategoryID : string.Empty;
                dicParam["ItemID"] = ItemID; //string.IsNullOrEmpty(ItemDetailID) ? ItemID : string.Empty;
                dicParam["Quantity"] = Quantity;
                dicParam["IsActive"] = IsActive;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = "_#GETUTCDATE()";
                dicWParam["PromotionBuyID"] = PromotionBuyID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "PromotionBuy"), objConn, objTran);
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
                objPromotion = null;
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

            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();

            try
            {
                dicParam["IsActive"] = IsActive;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = "_#GETUTCDATE()";
                dicWParam["PromotionBuyID"] = PromotionBuyID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "PromotionBuy"), objConn, objTran);
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

        private bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                     "FROM PromotionBuy (NOLOCK) p " +
                     "WHERE p.IsActive=1 " +
                     "AND p.WebsiteID=" + Database.HandleQuote(WebsiteID) +
                     "AND PromotionID=" + Database.HandleQuote(PromotionID);

            //if (!string.IsNullOrEmpty(ItemDetailID))
            //{
            //    strSQL += "AND p.ItemDetailID=" + Database.HandleQuote(ItemDetailID);
            //}
            //else 
            if (!string.IsNullOrEmpty(ItemID))
            {
                strSQL += "AND p.ItemID=" + Database.HandleQuote(ItemID); 
            }
            else if (!string.IsNullOrEmpty(WebsiteTabID))
            {
                strSQL += "AND p.WebsiteTabID=" + Database.HandleQuote(WebsiteTabID);
            }
            if (!string.IsNullOrEmpty(PromotionBuyID)) strSQL += "AND p.PromotionBuyID<>" + Database.HandleQuote(PromotionBuyID);
            return Database.HasRows(strSQL);
        }

        public static List<PromotionBuy> GetPromotionBuys(string WebsiteID, string PromotionID)
        {
            List<PromotionBuy> objReturn = null;
            PromotionBuy objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(PromotionID))
                {
                    objReturn = new List<PromotionBuy>();

                    strSQL = "SELECT * " +
                             "FROM PromotionBuy (NOLOCK) " +
                             "WHERE IsActive=1 " +
                             "AND WebsiteID=" + Database.HandleQuote(WebsiteID) +
                             "AND PromotionID=" + Database.HandleQuote(PromotionID);

                    objData = Database.GetDataSet(strSQL);

                    if (objData != null && objData.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                        {
                            objNew = new PromotionBuy(objData.Tables[0].Rows[i]);
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

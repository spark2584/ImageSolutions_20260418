using ImageSolutions.Custom;
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

namespace ImageSolutions.ShoppingCart
{
    public class ShoppingCartPromotion : ISBase.BaseClass
    {
        public string ShoppingCartPromotionID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ShoppingCartPromotionID); } }
        public string ShoppingCartID { get; set; }
        public string PromotionID { get; set; }
        public decimal Amount{ get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }


        private ShoppingCart mShoppingCart = null;
        public ShoppingCart ShoppingCart
        {
            get
            {
                if (mShoppingCart == null && !string.IsNullOrEmpty(ShoppingCartID))
                {
                    mShoppingCart = new ShoppingCart(ShoppingCartID);
                }
                return mShoppingCart;
            }
        }
        private Promotion.Promotion mPromotion = null;
        public Promotion.Promotion Promotion
        {
            get
            {
                if (mPromotion == null && !string.IsNullOrEmpty(PromotionID))
                {
                    mPromotion = new Promotion.Promotion(ShoppingCart.UserWebsite.WebsiteID, PromotionID);
                }
                return mPromotion;
            }
        }
        public string PromotionName
        {
            get
            {
                return Promotion.PromotionName;
            }
        }
        public ShoppingCartPromotion()
        {
        }

        public ShoppingCartPromotion(string ShoppingCartPromotionID)
        {
            this.ShoppingCartPromotionID = ShoppingCartPromotionID;
            Load();
        }

        public ShoppingCartPromotion(DataRow objRow)
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
                strSQL = string.Format(@"
                            SELECT sl.* 
                            FROM ShoppingCartPromotion (NOLOCK) sl 
                            WHERE sl.ShoppingCartPromotionID = {0} "
                                                , Database.HandleQuote(ShoppingCartPromotionID)
                            );

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ShoppingCartPromotionID=" + ShoppingCartPromotionID + " is not found");
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

                if (objColumns.Contains("ShoppingCartPromotionID")) ShoppingCartPromotionID = Convert.ToString(objRow["ShoppingCartPromotionID"]);
                if (objColumns.Contains("ShoppingCartID")) ShoppingCartID = Convert.ToString(objRow["ShoppingCartID"]);
                if (objColumns.Contains("PromotionID")) PromotionID = Convert.ToString(objRow["PromotionID"]);
                if (objColumns.Contains("Amount") && objRow["Amount"] != DBNull.Value) Amount = Convert.ToDecimal(objRow["Amount"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ShoppingCartPromotionID)) throw new Exception("Missing ShoppingCartPromotionID in the datarow");
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
                if (string.IsNullOrEmpty(ShoppingCartID)) throw new Exception("ShoppingCartID is required");
                if (string.IsNullOrEmpty(PromotionID)) throw new Exception("PromotionID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ShoppingCartPromotionID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["ShoppingCartID"] = ShoppingCartID;
                dicParam["PromotionID"] = PromotionID;
                dicParam["Amount"] = Amount;
                dicParam["CreatedBy"] = CreatedBy;

                ShoppingCartPromotionID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ShoppingCartPromotion"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(ShoppingCartID)) throw new Exception("ShoppingCartID is required");
                if (string.IsNullOrEmpty(PromotionID)) throw new Exception("PromotionID is required");
                if (IsNew) throw new Exception("Update cannot be performed, ShoppingCartPromotionID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["ShoppingCartID"] = ShoppingCartID;
                dicParam["PromotionID"] = PromotionID;
                dicParam["Amount"] = Amount;
                dicWParam["ShoppingCartPromotionID"] = ShoppingCartPromotionID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ShoppingCartPromotion"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, ShoppingCartPromotionsID is missing");

                dicDParam["ShoppingCartPromotionID"] = ShoppingCartPromotionID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ShoppingCartPromotion"), objConn, objTran);
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

        public static ShoppingCartPromotion GetShoppingCartPromotion(ShoppingCartPromotionFilter Filter)
        {
            List<ShoppingCartPromotion> objShoppingCartPromotions = null;
            ShoppingCartPromotion objReturn = null;

            try
            {
                objShoppingCartPromotions = GetShoppingCartPromotions(Filter);
                if (objShoppingCartPromotions != null && objShoppingCartPromotions.Count >= 1) objReturn = objShoppingCartPromotions[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objShoppingCartPromotions = null;
            }
            return objReturn;

        }

        public static List<ShoppingCartPromotion> GetShoppingCartPromotions()
        {
            int intTotalCount = 0;
            return GetShoppingCartPromotions(null, null, null, out intTotalCount);
        }

        public static List<ShoppingCartPromotion> GetShoppingCartPromotions(ShoppingCartPromotionFilter Filter)
        {
            int intTotalCount = 0;
            return GetShoppingCartPromotions(Filter, null, null, out intTotalCount);
        }

        public static List<ShoppingCartPromotion> GetShoppingCartPromotions(ShoppingCartPromotionFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetShoppingCartPromotions(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ShoppingCartPromotion> GetShoppingCartPromotions(ShoppingCartPromotionFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ShoppingCartPromotion> objReturn = null;
            ShoppingCartPromotion objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ShoppingCartPromotion>();

                strSQL = string.Format(@"
                            SELECT * 
                            FROM ShoppingCartPromotion (NOLOCK) sl 
                            WHERE 1=1 ");

                if (Filter != null)
                {
                    if (Filter.ShoppingCartID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ShoppingCartID, "ShoppingCartID");
                    if (Filter.PromotionID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PromotionID, "PromotionID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ShoppingCartPromotionID" : Utility.CustomSorting.GetSortExpression(typeof(ShoppingCartPromotion), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ShoppingCartPromotion(objData.Tables[0].Rows[i]);
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

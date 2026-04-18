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
    public class PromotionTrans : ISBase.BaseClass
    {
        public string PromotionTransID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(PromotionTransID); } }
        public string WebsiteID { get; private set; }
        public string PromotionID { get; set; }
        public string SalesOrderID { get; set; }
        public string InvoiceID { get; set; }
        public string PromotionCode { get; set; }
        public string PromotionName { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxOrderAmount { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? MaxUsageCount { get; set; }
        public bool CanBeCombined { get; set; }
        public bool IsSalesTaxExempt { get; set; }
        public bool IsOrPromotionBuy { get; set; }
        public bool IsOrPromotionGet { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }
        private List<PromotionBuyTrans> mPromotionBuys = null;
        public List<PromotionBuyTrans> PromotionBuys
        {
            get
            {
                if (mPromotionBuys == null && !string.IsNullOrEmpty(WebsiteID) && !string.IsNullOrEmpty(PromotionTransID))
                {
                    mPromotionBuys = PromotionBuyTrans.GetPromotionBuyTrans(WebsiteID, PromotionTransID);
                }
                return mPromotionBuys;
            }
            set
            {
                mPromotionBuys = null;
            }
        }
        public string PromotionBuyDescription
        {
            get
            {
                string strReturn = string.Empty;

                if (PromotionBuys != null)
                {
                    foreach (PromotionBuyTrans objPromotionBuy in PromotionBuys)
                    {
                        strReturn += !string.IsNullOrEmpty(strReturn) ? (IsOrPromotionBuy ? " OR " : " AND ") : string.Empty;

                        strReturn += objPromotionBuy.Quantity == null ? " Any " : objPromotionBuy.Quantity.Value + " of ";

                        if (objPromotionBuy.Item != null)
                            strReturn += objPromotionBuy.Item.ItemNumber;
                        else if (objPromotionBuy.WebsiteTab != null)
                            strReturn += objPromotionBuy.WebsiteTab.TabName;
                        else if (objPromotionBuy.Item != null)
                            strReturn += objPromotionBuy.Item.ItemName;
                    }
                }
                return !string.IsNullOrEmpty(strReturn) ? "BUY " + strReturn : string.Empty;
            }
        }
        private List<PromotionGetTrans> mPromotionGets = null;
        public List<PromotionGetTrans> PromotionGets
        {
            get
            {
                if (mPromotionGets == null && !string.IsNullOrEmpty(WebsiteID) && !string.IsNullOrEmpty(PromotionTransID))
                {
                    mPromotionGets = PromotionGetTrans.GetPromotionGetTrans(WebsiteID, PromotionTransID);
                }
                return mPromotionGets;
            }
            set
            {
                mPromotionGets = null;
            }
        }
        public string PromotionGetDescription
        {
            get
            {
                string strReturn = string.Empty;

                if (PromotionGets != null)
                {
                    foreach (PromotionGetTrans objPromotionGet in PromotionGets)
                    {
                        strReturn += !string.IsNullOrEmpty(strReturn) ? (IsOrPromotionGet ? " OR " : " AND ") : string.Empty;

                        strReturn += objPromotionGet.Quantity == null ? " Any " : objPromotionGet.Quantity.Value + " of ";

                        if (objPromotionGet.Item != null)
                            strReturn += objPromotionGet.Item.ItemNumber;
                        else if (objPromotionGet.WebsiteTab != null)
                            strReturn += objPromotionGet.WebsiteTab.TabName;
                        else if (objPromotionGet.Item != null)
                            strReturn += objPromotionGet.Item.ItemName;
                    }
                }
                return !string.IsNullOrEmpty(strReturn) ? "GET " + strReturn : string.Empty;
            }
        }
        public string PromotionDiscountDescription
        {
            get
            {
                string strReturn = string.Empty;

                if (DiscountPercent != null) strReturn += DiscountPercent.Value * 100 + "% off, ";
                if (DiscountAmount != null) strReturn += string.Format("{0:d}", DiscountAmount.Value) + " off, ";
                if (MinOrderAmount != null) strReturn += "when you spend " + string.Format("{0:d}", MinOrderAmount.Value) + " or more, ";
                if (MaxOrderAmount != null) strReturn += "order not to exceed " + string.Format("{0:d}", MaxOrderAmount.Value) + ", ";

                if (FromDate != null || ToDate != null) strReturn += "offer is valid ";
                if (FromDate != null) strReturn += "from " + FromDate.Value.ToShortDateString() + ", ";
                if (ToDate != null) strReturn += " to " + ToDate.Value.ToShortDateString() + ", ";

                if (MaxUsageCount != null) strReturn += "coupon is valid for first " + MaxUsageCount.Value + " customers, ";
                if (CanBeCombined)
                    strReturn += "promotion can be combined with other offers, ";
                else
                    strReturn += "promotion cannot be combined with other offers, ";

                if (strReturn.Trim().EndsWith(",")) strReturn = strReturn.Trim().Remove(strReturn.Trim().Length - 1, 1);
                return !string.IsNullOrEmpty(strReturn) ? "At " + strReturn : string.Empty;
            }
        }

        public PromotionTrans()
        {
        }

        public PromotionTrans(string WebsiteID)
        {
            this.WebsiteID = WebsiteID;
        }

        public PromotionTrans(string WebsiteID, string PromotionTransID)
        {
            this.WebsiteID = WebsiteID;
            this.PromotionTransID = PromotionTransID;
            Load();
        }

        public PromotionTrans(DataRow objRow)
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
                strSQL = "SELECT pt.* " +
                         "FROM PromotionTrans pt (NOLOCK) " +
                         "WHERE pt.WebsiteID=" + Database.HandleQuote(WebsiteID) +
                         "AND pt.PromotionTransID=" + Database.HandleQuote(PromotionTransID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("PromotionTransID=" + PromotionTransID + " is not found");
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

                if (objColumns.Contains("PromotionTransID")) PromotionTransID = Convert.ToString(objRow["PromotionTransID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("PromotionID")) PromotionID = Convert.ToString(objRow["PromotionID"]);
                if (objColumns.Contains("SalesOrderID")) SalesOrderID = Convert.ToString(objRow["SalesOrderID"]);
                //if (objColumns.Contains("CustInvoiceID")) CustInvoiceID = Convert.ToString(objRow["CustInvoiceID"]);
                if (objColumns.Contains("PromotionCode")) PromotionCode = Convert.ToString(objRow["PromotionCode"]);
                if (objColumns.Contains("PromotionName")) PromotionName = Convert.ToString(objRow["PromotionName"]);
                if (objColumns.Contains("DiscountPercent") && objRow["DiscountPercent"] != DBNull.Value) DiscountPercent = Convert.ToDecimal(objRow["DiscountPercent"]);
                if (objColumns.Contains("DiscountAmount") && objRow["DiscountAmount"] != DBNull.Value) DiscountAmount = Convert.ToDecimal(objRow["DiscountAmount"]);
                if (objColumns.Contains("MinOrderAmount") && objRow["MinOrderAmount"] != DBNull.Value) MinOrderAmount = Convert.ToDecimal(objRow["MinOrderAmount"]);
                if (objColumns.Contains("MaxOrderAmount") && objRow["MaxOrderAmount"] != DBNull.Value) MaxOrderAmount = Convert.ToDecimal(objRow["MaxOrderAmount"]);
                if (objColumns.Contains("FromDate") && objRow["FromDate"] != DBNull.Value) FromDate = Convert.ToDateTime(objRow["FromDate"]);
                if (objColumns.Contains("ToDate") && objRow["ToDate"] != DBNull.Value) ToDate = Convert.ToDateTime(objRow["ToDate"]);
                if (objColumns.Contains("MaxUsageCount") && objRow["MaxUsageCount"] != DBNull.Value) MaxUsageCount = Convert.ToInt32(objRow["MaxUsageCount"]);
                if (objColumns.Contains("CanBeCombined")) CanBeCombined = Convert.ToBoolean(objRow["CanBeCombined"]);
                if (objColumns.Contains("IsSalesTaxExempt")) IsSalesTaxExempt = Convert.ToBoolean(objRow["IsSalesTaxExempt"]);
                if (objColumns.Contains("IsOrPromotionBuy")) IsOrPromotionBuy = Convert.ToBoolean(objRow["IsOrPromotionBuy"]);
                if (objColumns.Contains("IsOrPromotionGet")) IsOrPromotionGet = Convert.ToBoolean(objRow["IsOrPromotionGet"]);
                if (objColumns.Contains("IsActive")) IsActive = Convert.ToBoolean(objRow["IsActive"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(PromotionID)) throw new Exception("Missing PromotionTransID in the datarow");
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
                if (string.IsNullOrEmpty(WebsiteID)) throw new Exception("WebsiteID is required");
                if (string.IsNullOrEmpty(PromotionID)) throw new Exception("PromotionID is required");
                if (string.IsNullOrEmpty(SalesOrderID)) throw new Exception("One of SalesOrderID or CustInvoiceID is required");
                //if (string.IsNullOrEmpty(SalesOrderID) && string.IsNullOrEmpty(CustInvoiceID)) throw new Exception("One of SalesOrderID or CustInvoiceID is required");
                if (string.IsNullOrEmpty(PromotionCode)) throw new Exception("PromotionCode is required");
                if (string.IsNullOrEmpty(PromotionName)) throw new Exception("PromotionName is required");
                //if (DiscountPercent == null && DiscountAmount == null) throw new Exception("One of discount percent/amount has to be specified");
                if (DiscountPercent != null && (DiscountPercent < -1 || DiscountPercent > 0)) throw new Exception("Discount percent has to be between -0% to -100%");
                if (DiscountAmount != null && DiscountAmount > 0) throw new Exception("Discount amount has to be a negative number");
                //if (DiscountAmount != null && DiscountAmount != 0) throw new Exception("Discount amount is currently not available");
                if (MinOrderAmount != null && MinOrderAmount < 0) throw new Exception("Min order amount has to be greater than or equal to 0");
                if (MaxOrderAmount != null && MaxOrderAmount < 0) throw new Exception("Max order amount has to be greater than or equal to 0");
                if (MinOrderAmount != null && MaxOrderAmount != null && MinOrderAmount > MaxOrderAmount) throw new Exception("Min order amount cannot be greater than max order amount");
                if (FromDate != null && ToDate != null && FromDate > ToDate) throw new Exception("From date must be on or before to date");
                if (MaxUsageCount != null && MaxUsageCount <= 0) throw new Exception("Maximum usage count has to be empty or a positive number > 0");
                if (DiscountAmount != null && PromotionBuys != null && PromotionBuys.Count > 0) throw new Exception("Promotion Buys cannot specified for Discount Amount type");
                if (DiscountAmount != null && PromotionGets != null && PromotionGets.Count > 0) throw new Exception("Promotion Gets cannot specified for Discount Amount type");
                if (!IsNew) throw new Exception("Create cannot be performed, PromotionTransID already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["PromotionID"] = PromotionID;
                dicParam["SalesOrderID"] = SalesOrderID;
                //dicParam["CustInvoiceID"] = CustInvoiceID;
                dicParam["PromotionCode"] = PromotionCode;
                dicParam["PromotionName"] = PromotionName;
                dicParam["DiscountPercent"] = DiscountPercent;
                dicParam["DiscountAmount"] = DiscountAmount;
                dicParam["MinOrderAmount"] = MinOrderAmount;
                dicParam["MaxOrderAmount"] = MaxOrderAmount;
                dicParam["FromDate"] = FromDate;
                dicParam["ToDate"] = ToDate;
                dicParam["MaxUsageCount"] = MaxUsageCount;
                dicParam["CanBeCombined"] = CanBeCombined;
                dicParam["IsSalesTaxExempt"] = IsSalesTaxExempt;
                dicParam["IsOrPromotionBuy"] = IsOrPromotionBuy;
                dicParam["IsOrPromotionGet"] = IsOrPromotionGet;
                dicParam["IsActive"] = IsActive;
                dicParam["CreatedBy"] = CreatedBy;
                PromotionTransID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "PromotionTrans"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(WebsiteID)) throw new Exception("WebsiteID is required");
                if (string.IsNullOrEmpty(PromotionID)) throw new Exception("PromotionID is required");
                if (string.IsNullOrEmpty(SalesOrderID)) throw new Exception("One of SalesOrderID or CustInvoiceID is required");
                //if (string.IsNullOrEmpty(SalesOrderID) && string.IsNullOrEmpty(CustInvoiceID)) throw new Exception("One of SalesOrderID or CustInvoiceID is required");
                if (string.IsNullOrEmpty(PromotionCode)) throw new Exception("PromotionCode is required");
                if (string.IsNullOrEmpty(PromotionName)) throw new Exception("PromotionName is required");
                //if (DiscountPercent == null && DiscountAmount == null) throw new Exception("One of discount percent/amount has to be specified");
                if (DiscountPercent != null && (DiscountPercent < -1 || DiscountPercent > 0)) throw new Exception("Discount percent has to be between -0% to -100%");
                if (DiscountAmount != null && DiscountAmount > 0) throw new Exception("Discount amount has to be a negative number");
                //if (DiscountAmount != null && DiscountAmount != 0) throw new Exception("Discount amount is currently not available");
                if (MinOrderAmount != null && MinOrderAmount < 0) throw new Exception("Min order amount has to be greater than or equal to 0");
                if (MaxOrderAmount != null && MaxOrderAmount < 0) throw new Exception("Max order amount has to be greater than or equal to 0");
                if (MinOrderAmount != null && MaxOrderAmount != null && MinOrderAmount > MaxOrderAmount) throw new Exception("Min order amount cannot be greater than max order amount");
                if (FromDate != null && ToDate != null && FromDate > ToDate) throw new Exception("From date must be on or before to date");
                if (MaxUsageCount != null && MaxUsageCount <= 0) throw new Exception("Maximum usage count has to be empty or a positive number > 0");
                if (DiscountAmount != null && PromotionBuys != null && PromotionBuys.Count > 0) throw new Exception("Promotion Buys cannot specified for Discount Amount type");
                if (DiscountAmount != null && PromotionGets != null && PromotionGets.Count > 0) throw new Exception("Promotion Gets cannot specified for Discount Amount type");
                if (IsNew) throw new Exception("Update cannot be performed, PromotionTransID is missing");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["PromotionID"] = PromotionID;
                dicParam["SalesOrderID"] = SalesOrderID;
                //dicParam["CustInvoiceID"] = CustInvoiceID;
                dicParam["PromotionCode"] = PromotionCode;
                dicParam["PromotionName"] = PromotionName;
                dicParam["DiscountPercent"] = DiscountPercent;
                dicParam["DiscountAmount"] = DiscountAmount;
                dicParam["MinOrderAmount"] = MinOrderAmount;
                dicParam["MaxOrderAmount"] = MaxOrderAmount;
                dicParam["FromDate"] = FromDate;
                dicParam["ToDate"] = ToDate;
                dicParam["MaxUsageCount"] = MaxUsageCount;
                dicParam["CanBeCombined"] = CanBeCombined;
                dicParam["IsSalesTaxExempt"] = IsSalesTaxExempt;
                dicParam["IsOrPromotionBuy"] = IsOrPromotionBuy;
                dicParam["IsOrPromotionGet"] = IsOrPromotionGet;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = "_#GETUTCDATE()";
                dicWParam["PromotionID"] = PromotionTransID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "PromotionTrans"), objConn, objTran);
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
                foreach (PromotionBuyTrans objPromotionBuyTrans in PromotionBuys)
                {
                    objPromotionBuyTrans.UpdatedBy = UpdatedBy;
                    objPromotionBuyTrans.Delete(objConn, objTran);
                }

                foreach (PromotionGetTrans objPromotionGetTrans in PromotionGets)
                {
                    objPromotionGetTrans.UpdatedBy = UpdatedBy;
                    objPromotionGetTrans.Delete(objConn, objTran);
                }

                dicWParam["PromotionTransID"] = PromotionTransID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicWParam, "PromotionTrans"), objConn, objTran);
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

        public static List<PromotionTrans> GetPromotionTransBySalesOrderID(string WebsiteID, string SalesOrderID)
        {
            List<PromotionTrans> objReturn = null;
            PromotionTrans objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(SalesOrderID))
                {
                    objReturn = new List<PromotionTrans>();

                    strSQL = "SELECT pt.* " +
                             "FROM PromotionTrans pt (NOLOCK) " +
                             "WHERE pt.WebsiteID=" + Database.HandleQuote(WebsiteID) +
                             "AND pt.SalesOrderID=" + Database.HandleQuote(SalesOrderID);

                    objData = Database.GetDataSet(strSQL);

                    if (objData != null && objData.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                        {
                            objNew = new PromotionTrans(WebsiteID, objData.Tables[0].Rows[i]["PromotionTransID"].ToString());
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

        //public static List<PromotionTrans> GetPromotionTransByCustInvoiceID(string WebsiteID, string CustInvoiceID)
        //{
        //    List<PromotionTrans> objReturn = null;
        //    PromotionTrans objNew = null;
        //    DataSet objData = null;
        //    string strSQL = string.Empty;

        //    try
        //    {
        //        if (!string.IsNullOrEmpty(CustInvoiceID))
        //        {
        //            objReturn = new List<PromotionTrans>();

        //            strSQL = "SELECT pt.* " +
        //                     "FROM PromotionTrans pt (NOLOCK) " +
        //                     "WHERE pt.WebsiteID=" + Database.HandleQuote(WebsiteID) +
        //                     "AND pt.CustInvoiceID=" + Database.HandleQuote(CustInvoiceID);

        //            objData = Database.GetDataSet(strSQL);

        //            if (objData != null && objData.Tables[0].Rows.Count > 0)
        //            {
        //                for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
        //                {
        //                    objNew = new PromotionTrans(WebsiteID, objData.Tables[0].Rows[i]["PromotionTransID"].ToString());
        //                    objReturn.Add(objNew);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objData = null;
        //    }
        //    return objReturn;
        //}
    }
}

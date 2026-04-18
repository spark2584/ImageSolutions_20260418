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
    public class Promotion : ISBase.BaseClass
    {
        public string PromotionID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(PromotionID); } }
        public string WebsiteID { get; set; }
        public string PromotionCode { get; set; }
        public string PromotionName { get; set; }
        public string CustomerID { get; set; }
        //private Customer.Customer mCustomer = null;
        //public Customer.Customer Customer
        //{
        //    get
        //    {
        //        if (mCustomer == null && !string.IsNullOrEmpty(CustomerID))
        //        {
        //            mCustomer = new Customer.Customer(CustomerID);
        //        }
        //        return mCustomer;
        //    }
        //}
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxOrderAmount { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? MaxUsageCount { get; set; }
        private int? mUsageCount = null;
        public int UsageCount
        {
            get
            {
                if (mUsageCount == null && !string.IsNullOrEmpty(WebsiteID) && !string.IsNullOrEmpty(PromotionID))
                {
                    mUsageCount = GetUsageCount(WebsiteID, PromotionID);
                }
                return mUsageCount.HasValue ? mUsageCount.Value : 0;
            }
        }
        public bool CanBeCombined { get; set; }
        public bool ExcludeSaleItem { get; set; }
        public bool IsSalesTaxExempt { get; set; }
        public bool IsOrPromotionBuy { get; set; }
        public bool IsOrPromotionGet { get; set; }
        public string FreeShippingServiceID { get; set; }
        public bool ExcludeShippingAndTax { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }
        private List<PromotionBuy> mPromotionBuys = null;
        public List<PromotionBuy> PromotionBuys
        {
            get
            {
                if (mPromotionBuys == null && !string.IsNullOrEmpty(WebsiteID) && !string.IsNullOrEmpty(PromotionID))
                {
                    mPromotionBuys = PromotionBuy.GetPromotionBuys(WebsiteID, PromotionID);
                }
                return mPromotionBuys;
            }
            set
            {
                mPromotionBuys = value;
            }
        }
        public string PromotionBuyDescription
        {
            get
            {
                string strReturn = string.Empty;

                if (PromotionBuys != null)
                {
                    foreach (PromotionBuy objPromotionBuy in PromotionBuys)
                    {
                        strReturn += !string.IsNullOrEmpty(strReturn) ? (IsOrPromotionBuy ? " OR " : " AND ") : string.Empty;

                        strReturn += objPromotionBuy.Quantity == null ? " Any " : objPromotionBuy.Quantity.Value + " of ";

                        if (objPromotionBuy.Item != null)
                            strReturn += objPromotionBuy.Item.ItemNumber + " (ItemNumber)";
                        else if (objPromotionBuy.Item != null)
                            strReturn += objPromotionBuy.Item.ItemName + " (Item)";
                        else if (objPromotionBuy.WebsiteTab != null)
                            strReturn += objPromotionBuy.WebsiteTab.TabName + " (Category)";
                        else
                            strReturn += "any item";
                    }
                }
                return !string.IsNullOrEmpty(strReturn) ? "BUY " + strReturn : string.Empty;
            }
        }
        private List<PromotionGet> mPromotionGets = null;
        public List<PromotionGet> PromotionGets
        {
            get
            {
                if (mPromotionGets == null && !string.IsNullOrEmpty(WebsiteID) && !string.IsNullOrEmpty(PromotionID))
                {
                    mPromotionGets = PromotionGet.GetPromotionGets(WebsiteID, PromotionID);
                }
                return mPromotionGets;
            }
            set
            {
                mPromotionGets = value;
            }
        }
        public string PromotionGetDescription
        { 
            get
            {
                string strReturn = string.Empty;

                if (PromotionGets != null)
                {
                    foreach (PromotionGet objPromotionGet in PromotionGets)
                    {
                        strReturn += !string.IsNullOrEmpty(strReturn) ? (IsOrPromotionGet ? " OR " : " AND ") : string.Empty;

                        strReturn += objPromotionGet.Quantity == null ? " Any " : objPromotionGet.Quantity.Value + " of ";

                        if (objPromotionGet.Item != null)
                            strReturn += objPromotionGet.Item.ItemNumber + " (Item Number)";
                        else if (objPromotionGet.Item != null)
                            strReturn += objPromotionGet.Item.ItemName + " (Item)";
                        else if (objPromotionGet.WebsiteTab != null)
                            strReturn += objPromotionGet.WebsiteTab.TabName + " (Category)";
                        else
                            strReturn += "any item";
                    }
                }
                return !string.IsNullOrEmpty(strReturn) ? "GET " + strReturn : string.Empty;
            }
        }

        private Shipping.ShippingService mFreeShippingService = null;
        public Shipping.ShippingService FreeShippingService
        {
            get
            {
                if (mFreeShippingService == null && !string.IsNullOrEmpty(FreeShippingServiceID))
                {
                    mFreeShippingService = new Shipping.ShippingService(FreeShippingServiceID);
                }
                return mFreeShippingService;
            }
            set
            {
                mFreeShippingService = value;
            }
        }
        public string PromotionDiscountDescription
        {
            get
            {
                string strReturn = string.Empty;

                if (DiscountPercent != null) strReturn += DiscountPercent.Value * 100 + "% off, ";
                if (DiscountAmount != null) strReturn += string.Format("{0:c}", DiscountAmount.Value) + " off, ";
                if (MinOrderAmount != null) strReturn += "when you spend " + string.Format("{0:c}", MinOrderAmount.Value) + " or more, ";
                if (MaxOrderAmount != null) strReturn += "order not to exceed " + string.Format("{0:c}", MaxOrderAmount.Value) + ", ";

                if (FromDate != null || ToDate != null) strReturn += "offer is valid ";
                if (FromDate != null) strReturn += "from " + FromDate.Value.ToShortDateString() + ", ";
                if (ToDate != null) strReturn += " to " + ToDate.Value.ToShortDateString() + ", ";

                if (MaxUsageCount != null) strReturn += "coupon is valid for first " + MaxUsageCount.Value + " customers, ";
                //if (CanBeCombined)
                //    strReturn += "promotion can be combined with other offers, ";
                //else
                //    strReturn += "promotion cannot be combined with other offers, ";

                if(!string.IsNullOrEmpty(FreeShippingServiceID) && FreeShippingService != null) strReturn += "free shipping for " + FreeShippingService.Description + " , ";

                if (strReturn.Trim().EndsWith(",")) strReturn = strReturn.Trim().Remove(strReturn.Trim().Length - 1, 1);
                return !string.IsNullOrEmpty(strReturn) ? strReturn : string.Empty;
            }
        }


        private List<Payment.Payment> mPayments = null;
        public List<Payment.Payment> Payments
        {
            get
            {
                if (mPayments == null && !string.IsNullOrEmpty(PromotionID))
                {
                    Payment.PaymentFilter objFilter = null;

                    try
                    {
                        objFilter = new Payment.PaymentFilter();
                        objFilter.PromotionID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.PromotionID.SearchString = PromotionID;
                        mPayments = Payment.Payment.GetPayments(objFilter);
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
                return mPayments;
            }
        }
        public Promotion()
        {
        }

        public Promotion(string WebsiteID)
        {
            this.WebsiteID = WebsiteID;
        }

        public Promotion(string WebsiteID, string PromotionID)
        {
            this.WebsiteID = WebsiteID;
            this.PromotionID = PromotionID;
            Load();
        }

        public Promotion(DataRow objRow)
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
                         "FROM Promotion (NOLOCK) " +
                         "WHERE WebsiteID=" + Database.HandleQuote(WebsiteID) +
                         "AND PromotionID=" + Database.HandleQuote(PromotionID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("PromotionID=" + PromotionID + " is not found");
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

                if (objColumns.Contains("PromotionID")) PromotionID = Convert.ToString(objRow["PromotionID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("PromotionCode")) PromotionCode = Convert.ToString(objRow["PromotionCode"]);
                if (objColumns.Contains("PromotionName")) PromotionName = Convert.ToString(objRow["PromotionName"]);
                if (objColumns.Contains("CustomerID")) CustomerID = Convert.ToString(objRow["CustomerID"]);
                if (objColumns.Contains("DiscountAmount") && objRow["DiscountAmount"] != DBNull.Value) DiscountAmount = Convert.ToDecimal(objRow["DiscountAmount"]);
                if (objColumns.Contains("DiscountPercent") && objRow["DiscountPercent"] != DBNull.Value) DiscountPercent = Convert.ToDecimal(objRow["DiscountPercent"]);
                if (objColumns.Contains("MinOrderAmount") && objRow["MinOrderAmount"] != DBNull.Value) MinOrderAmount = Convert.ToDecimal(objRow["MinOrderAmount"]);
                if (objColumns.Contains("MaxOrderAmount") && objRow["MaxOrderAmount"] != DBNull.Value) MaxOrderAmount = Convert.ToDecimal(objRow["MaxOrderAmount"]);
                if (objColumns.Contains("FromDate") && objRow["FromDate"] != DBNull.Value) FromDate = Convert.ToDateTime(objRow["FromDate"]);
                if (objColumns.Contains("ToDate") && objRow["ToDate"] != DBNull.Value) ToDate = Convert.ToDateTime(objRow["ToDate"]);
                if (objColumns.Contains("MaxUsageCount") && objRow["MaxUsageCount"] != DBNull.Value) MaxUsageCount = Convert.ToInt32(objRow["MaxUsageCount"]);
                if (objColumns.Contains("CanBeCombined")) CanBeCombined = Convert.ToBoolean(objRow["CanBeCombined"]);
                if (objColumns.Contains("ExcludeSaleItem")) ExcludeSaleItem = Convert.ToBoolean(objRow["ExcludeSaleItem"]);
                if (objColumns.Contains("IsSalesTaxExempt")) IsSalesTaxExempt = Convert.ToBoolean(objRow["IsSalesTaxExempt"]);
                if (objColumns.Contains("IsOrPromotionBuy")) IsOrPromotionBuy = Convert.ToBoolean(objRow["IsOrPromotionBuy"]);
                if (objColumns.Contains("IsOrPromotionGet")) IsOrPromotionGet = Convert.ToBoolean(objRow["IsOrPromotionGet"]);
                if (objColumns.Contains("FreeShippingServiceID")) FreeShippingServiceID = Convert.ToString(objRow["FreeShippingServiceID"]);
                if (objColumns.Contains("ExcludeShippingAndTax")) ExcludeShippingAndTax = Convert.ToBoolean(objRow["ExcludeShippingAndTax"]);
                if (objColumns.Contains("IsActive")) IsActive = Convert.ToBoolean(objRow["IsActive"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(PromotionID)) throw new Exception("Missing PromotionID in the datarow");
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
                if (string.IsNullOrEmpty(PromotionCode)) throw new Exception("PromotionCode is required");
                if (string.IsNullOrEmpty(PromotionName)) throw new Exception("PromotionName is required");
                //if (DiscountPercent == null && DiscountAmount == null) throw new Exception("One of discount percent/amount has to be specified");
                if (DiscountPercent != null && DiscountAmount != null) throw new Exception("Discount percent and discount amount cannot be combined");
                if (DiscountPercent != null && (DiscountPercent < -1 || DiscountPercent > 0)) throw new Exception("Discount percent has to be between -0% to -100%");
                if (DiscountAmount != null && DiscountAmount > 0) throw new Exception("Discount amount has to be a negative number");
                //if (DiscountAmount != null && DiscountAmount != 0) throw new Exception("Discount amount is currently not available");
                if (MinOrderAmount != null && MinOrderAmount < 0) throw new Exception("Min order amount has to be greater than or equal to 0");
                if (MaxOrderAmount != null && MaxOrderAmount < 0) throw new Exception("Max order amount has to be greater than or equal to 0");
                if (MinOrderAmount != null && MaxOrderAmount != null && MinOrderAmount > MaxOrderAmount) throw new Exception("Min order amount cannot be greater than max order amount");
                if (FromDate != null && ToDate != null && FromDate > ToDate) throw new Exception("From date must be on or before to date");
                if (MaxUsageCount != null && MaxUsageCount <= 0) throw new Exception("Maximum usage count has to be empty or a positive number > 0");
                //if (DiscountAmount != null && PromotionBuys != null && PromotionBuys.Count > 0) throw new Exception("Promotion Buys cannot be specified for Discount Amount type");
                if (DiscountAmount != null && PromotionGets != null && PromotionGets.Count > 0) throw new Exception("Promotion Gets cannot be specified for Discount Amount type");
                if (!IsNew) throw new Exception("Create cannot be performed, PromotionID already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["PromotionCode"] = PromotionCode;
                dicParam["PromotionName"] = PromotionName;
                dicParam["CustomerID"] = CustomerID;
                dicParam["DiscountPercent"] = DiscountPercent;
                dicParam["DiscountAmount"] = DiscountAmount;
                dicParam["MinOrderAmount"] = MinOrderAmount;
                dicParam["MaxOrderAmount"] = MaxOrderAmount;
                dicParam["FromDate"] = FromDate;
                dicParam["ToDate"] = ToDate;
                dicParam["MaxUsageCount"] = MaxUsageCount;
                dicParam["CanBeCombined"] = CanBeCombined;
                dicParam["ExcludeSaleItem"] = ExcludeSaleItem;
                dicParam["IsSalesTaxExempt"] = IsSalesTaxExempt;
                dicParam["IsOrPromotionBuy"] = IsOrPromotionBuy;
                dicParam["IsOrPromotionGet"] = IsOrPromotionGet;
                dicParam["FreeShippingServiceID"] = FreeShippingServiceID;
                dicParam["ExcludeShippingAndTax"] = ExcludeShippingAndTax;
                dicParam["IsActive"] = IsActive;
                dicParam["CreatedBy"] = CreatedBy;
                PromotionID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Promotion"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(PromotionCode)) throw new Exception("PromotionCode is required");
                if (string.IsNullOrEmpty(PromotionName)) throw new Exception("PromotionName is required");
                //if (DiscountPercent == null && DiscountAmount == null) throw new Exception("One of discount percent/amount has to be specified");
                if (DiscountPercent != null && DiscountAmount != null) throw new Exception("Discount percent and discount amount cannot be combined");
                if (DiscountPercent != null && (DiscountPercent < -1 || DiscountPercent > 0)) throw new Exception("Discount percent has to be between -0% to -100%");
                if (DiscountAmount != null && DiscountAmount > 0) throw new Exception("Discount amount has to be a negative number");
                //if (DiscountAmount != null && DiscountAmount != 0) throw new Exception("Discount amount is currently not available");
                if (MinOrderAmount != null && MinOrderAmount < 0) throw new Exception("Min order amount has to be greater than or equal to 0");
                if (MaxOrderAmount != null && MaxOrderAmount < 0) throw new Exception("Max order amount has to be greater than or equal to 0");
                if (MinOrderAmount != null && MaxOrderAmount != null && MinOrderAmount > MaxOrderAmount) throw new Exception("Min order amount cannot be greater than max order amount");
                if (FromDate != null && ToDate != null && FromDate > ToDate) throw new Exception("From date must be on or before to date");
                if (MaxUsageCount != null && MaxUsageCount <= 0) throw new Exception("Maximum usage count has to be empty or a positive number > 0");
                //if (DiscountAmount != null && PromotionBuys != null && PromotionBuys.Count > 0) throw new Exception("Promotion Buys cannot be specified for Discount Amount type");
                //if (DiscountAmount != null && PromotionGets != null && PromotionGets.Count > 0) throw new Exception("Promotion Gets cannot be specified for Discount Amount type");
                if (IsNew) throw new Exception("Update cannot be performed, PromotionID is missing");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["PromotionCode"] = PromotionCode;
                dicParam["PromotionName"] = PromotionName;
                dicParam["CustomerID"] = CustomerID;
                dicParam["DiscountPercent"] = DiscountPercent;
                dicParam["DiscountAmount"] = DiscountAmount;
                dicParam["MinOrderAmount"] = MinOrderAmount;
                dicParam["MaxOrderAmount"] = MaxOrderAmount;
                dicParam["FromDate"] = FromDate;
                dicParam["ToDate"] = ToDate;
                dicParam["MaxUsageCount"] = MaxUsageCount;
                dicParam["CanBeCombined"] = CanBeCombined;
                dicParam["ExcludeSaleItem"] = ExcludeSaleItem;
                dicParam["IsSalesTaxExempt"] = IsSalesTaxExempt;
                dicParam["IsOrPromotionBuy"] = IsOrPromotionBuy;
                dicParam["IsOrPromotionGet"] = IsOrPromotionGet;
                dicParam["FreeShippingServiceID"] = FreeShippingServiceID;
                dicParam["ExcludeShippingAndTax"] = ExcludeShippingAndTax;
                dicParam["IsActive"] = IsActive;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = "_#GETUTCDATE()";
                dicWParam["PromotionID"] = PromotionID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Promotion"), objConn, objTran);
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

        //public override bool Delete()
        //{
        //    SqlConnection objConn = null;
        //    SqlTransaction objTran = null;

        //    try
        //    {
        //        objConn = new SqlConnection(Database.DefaultConnectionString);
        //        objConn.Open();
        //        objTran = objConn.BeginTransaction();
        //        Delete(objConn, objTran);
        //        objTran.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (objTran != null && objTran.Connection != null) objTran.Rollback();
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (objTran != null) objTran.Dispose();
        //        objTran = null;
        //        if (objConn != null) objConn.Dispose();
        //        objConn = null;
        //    }
        //    return true;
        //}

        //public override bool Delete(SqlConnection objConn, SqlTransaction objTran)
        //{
        //    base.Delete();

        //    Hashtable dicDParam = new Hashtable();

        //    try
        //    {
        //        if (IsNew) throw new Exception("Delete cannot be performed, DynamicAttributeLookupID is missing");

        //        foreach (DynamicAttributeLookupValue objLookupValue in DynamicAttributeLookupValues)
        //        {
        //            objLookupValue.Delete(objConn, objTran);
        //        }

        //        dicDParam["DynamicAttributeLookupID"] = DynamicAttributeLookupID;
        //        Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "DynamicAttributeLookup"), objConn, objTran);

        //        Load(objConn, objTran);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        dicDParam = null;
        //    }
        //    return true;
        //}

        private static int GetUsageCount(string WebsiteID, string PromotionID)
        {
            string strSQL = "SELECT COUNT(*) " +
                            "FROM PromotionTrans t (NOLOCK) " +
                            "WHERE t.WebsiteID=" + Database.HandleQuote(WebsiteID) +
                            "AND t.PromotionID=" + Database.HandleQuote(PromotionID);
            return Convert.ToInt32(Database.ExecuteScalar(strSQL));
        }

        public bool IsValid()
        {
            if (!IsActive) throw new Exception("This promotion code is not valid.");
            if (FromDate != null && FromDate.Value > DateTime.UtcNow) throw new Exception("This promotion code is not valid.");
            if (ToDate != null && ToDate.Value < DateTime.UtcNow) throw new Exception("This promotion has expired.");
            if (MaxUsageCount != null && UsageCount >= MaxUsageCount) throw new Exception("This promotion has expired. It's limited to the first " + MaxUsageCount + " customer(s).");

            //if (DiscountAmount != null && DiscountAmount != 0) throw new Exception("Discount amount is currently not valid with any purchase.");

            return true;
        }

        //private bool IsOrPromotionBuyMet(ShoppingCart.ShoppingCart ShoppingCart, PromotionBuy objPromotionBuy)
        //{
        //    objPromotionBuy.IsConditionMet = false;

        //    if (!string.IsNullOrEmpty(objPromotionBuy.ItemDetailID))
        //    {
        //        if (ShoppingCart.ShoppingCartLines.Exists(m => m.ItemID == objPromotionBuy.ItemID))
        //        {
        //            if (objPromotionBuy.Quantity != null)
        //            {
        //                int intQuantity = objPromotionBuy.Quantity.Value;

        //                foreach (ShoppingCart.ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines))//.OrderByDescending(m => m.UnitPriceBeforeDiscount))
        //                {
        //                    if (objShoppingCartLine.PromotionEligibleQuantity > 0)
        //                    {
        //                        if (objShoppingCartLine.ItemDetailID == objPromotionBuy.ItemDetailID)
        //                        {
        //                            if (objShoppingCartLine.PromotionEligibleQuantity >= intQuantity)
        //                            {
        //                                objShoppingCartLine.PromotionEligibleQuantity -= intQuantity;
        //                                intQuantity -= intQuantity;
        //                            }
        //                            else
        //                            {
        //                                intQuantity -= objShoppingCartLine.PromotionEligibleQuantity;
        //                                objShoppingCartLine.PromotionEligibleQuantity = 0;
        //                            }
        //                        }

        //                        if (intQuantity == 0)
        //                        {
        //                            objPromotionBuy.IsConditionMet = true;
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                objPromotionBuy.IsConditionMet = true;
        //            }
        //        }
        //    }
        //    else if (!string.IsNullOrEmpty(objPromotionBuy.ItemCategoryID))
        //    {
        //        if (ShoppingCart.ShoppingCartLines.Exists(m => m.ItemDetail.Item.ItemCategories.Exists(n => n.ItemCategoryID == objPromotionBuy.ItemCategoryID)))
        //        {
        //            if (objPromotionBuy.Quantity != null)
        //            {
        //                int intQuantity = objPromotionBuy.Quantity.Value;

        //                foreach (ShoppingCart.ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines.OrderByDescending(m => m.UnitPriceBeforeDiscount))
        //                {
        //                    if (objShoppingCartLine.PromotionEligibleQuantity > 0)
        //                    {
        //                        if (objShoppingCartLine.ItemDetail.Item.ItemCategories.Exists(mbox => mbox.ItemCategoryID == objPromotionBuy.ItemCategoryID))
        //                        {
        //                            if (objShoppingCartLine.PromotionEligibleQuantity >= intQuantity)
        //                            {
        //                                objShoppingCartLine.PromotionEligibleQuantity -= intQuantity;
        //                                intQuantity -= intQuantity;
        //                            }
        //                            else
        //                            {
        //                                intQuantity -= objShoppingCartLine.Quantity;
        //                                objShoppingCartLine.PromotionEligibleQuantity = 0;
        //                            }
        //                        }

        //                        if (intQuantity == 0)
        //                        {
        //                            objPromotionBuy.IsConditionMet = true;
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                objPromotionBuy.IsConditionMet = true;
        //            }
        //        }
        //    }
        //    else if (!string.IsNullOrEmpty(objPromotionBuy.ItemID))
        //    {
        //        if (ShoppingCart.ShoppingCartLines.Exists(m => m.ItemID == objPromotionBuy.ItemID))
        //        {
        //            if (objPromotionBuy.Quantity != null)
        //            {
        //                int intQuantity = objPromotionBuy.Quantity.Value;

        //                foreach (ShoppingCart.ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines.OrderByDescending(m => m.UnitPriceBeforeDiscount))
        //                {
        //                    if (objShoppingCartLine.PromotionEligibleQuantity > 0)
        //                    {
        //                        if (objShoppingCartLine.ItemID == objPromotionBuy.ItemID)
        //                        {
        //                            if (objShoppingCartLine.PromotionEligibleQuantity >= intQuantity)
        //                            {
        //                                objShoppingCartLine.PromotionEligibleQuantity -= intQuantity;
        //                                intQuantity -= intQuantity;
        //                            }
        //                            else
        //                            {
        //                                intQuantity -= objShoppingCartLine.Quantity;
        //                                objShoppingCartLine.PromotionEligibleQuantity = 0;
        //                            }
        //                        }

        //                        if (intQuantity == 0)
        //                        {
        //                            objPromotionBuy.IsConditionMet = true;
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                objPromotionBuy.IsConditionMet = true;
        //            }
        //        }
        //    }
        //    else if (objPromotionBuy.Quantity != null) //buy 1 get 1 free example
        //    {
        //        if (objPromotionBuy.Quantity > 0)
        //        {
        //            if (objPromotionBuy.Quantity != null)
        //            {
        //                int intQuantity = objPromotionBuy.Quantity.Value;

        //                foreach (ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines.OrderByDescending(m => m.UnitPriceBeforeDiscount))
        //                {
        //                    if (objShoppingCartLine.PromotionEligibleQuantity > 0)
        //                    {
        //                        if (objShoppingCartLine.PromotionEligibleQuantity >= intQuantity)
        //                        {
        //                            objShoppingCartLine.PromotionEligibleQuantity -= intQuantity;
        //                            intQuantity -= intQuantity;
        //                        }
        //                        else
        //                        {
        //                            intQuantity -= objShoppingCartLine.Quantity;
        //                            objShoppingCartLine.PromotionEligibleQuantity = 0;
        //                        }

        //                        if (intQuantity == 0)
        //                        {
        //                            objPromotionBuy.IsConditionMet = true;
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                objPromotionBuy.IsConditionMet = true;
        //            }
        //        }
        //    }
        //    return objPromotionBuy.IsConditionMet;
        //}

        //private bool IsAndPromotionBuyMet(ShoppingCart.ShoppingCart ShoppingCart)
        //{
        //    foreach (PromotionBuy objPromotionBuy in PromotionBuys)
        //    {
        //        objPromotionBuy.IsConditionMet = false;

        //        if (!string.IsNullOrEmpty(objPromotionBuy.ItemDetailID))
        //        {
        //            if (ShoppingCart.ShoppingCartLines.Exists(m => m.ItemDetailID == objPromotionBuy.ItemDetailID))
        //            {
        //                if (objPromotionBuy.Quantity != null)
        //                {
        //                    int intQuantity = objPromotionBuy.Quantity.Value;

        //                    foreach (ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines.OrderByDescending(m => m.UnitPriceBeforeDiscount))
        //                    {
        //                        if (objShoppingCartLine.PromotionEligibleQuantity > 0)
        //                        {
        //                            if (objShoppingCartLine.ItemDetailID == objPromotionBuy.ItemDetailID)
        //                            {
        //                                if (objShoppingCartLine.PromotionEligibleQuantity >= intQuantity)
        //                                {
        //                                    objShoppingCartLine.PromotionEligibleQuantity -= intQuantity;
        //                                    intQuantity -= intQuantity;
        //                                }
        //                                else
        //                                {
        //                                    intQuantity -= objShoppingCartLine.PromotionEligibleQuantity;
        //                                    objShoppingCartLine.PromotionEligibleQuantity = 0;
        //                                }
        //                            }

        //                            if (intQuantity == 0)
        //                            {
        //                                objPromotionBuy.IsConditionMet = true;
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    objPromotionBuy.IsConditionMet = true;
        //                }
        //            }
        //        }
        //        else if (!string.IsNullOrEmpty(objPromotionBuy.ItemCategoryID))
        //        {
        //            if (ShoppingCart.ShoppingCartLines.Exists(m => m.ItemDetail.Item.ItemCategories.Exists(n => n.ItemCategoryID == objPromotionBuy.ItemCategoryID)))
        //            {
        //                if (objPromotionBuy.Quantity != null)
        //                {
        //                    int intQuantity = objPromotionBuy.Quantity.Value;

        //                    foreach (ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines.OrderByDescending(m => m.UnitPriceBeforeDiscount))
        //                    {
        //                        if (objShoppingCartLine.PromotionEligibleQuantity > 0)
        //                        {
        //                            if (objShoppingCartLine.ItemDetail.Item.ItemCategories.Exists(mbox => mbox.ItemCategoryID == objPromotionBuy.ItemCategoryID))
        //                            {
        //                                if (objShoppingCartLine.PromotionEligibleQuantity >= intQuantity)
        //                                {
        //                                    objShoppingCartLine.PromotionEligibleQuantity -= intQuantity;
        //                                    intQuantity -= intQuantity;
        //                                }
        //                                else
        //                                {
        //                                    intQuantity -= objShoppingCartLine.Quantity;
        //                                    objShoppingCartLine.PromotionEligibleQuantity = 0;
        //                                }
        //                            }

        //                            if (intQuantity == 0)
        //                            {
        //                                objPromotionBuy.IsConditionMet = true;
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    objPromotionBuy.IsConditionMet = true;
        //                }
        //            }
        //        }
        //        else if (!string.IsNullOrEmpty(objPromotionBuy.ItemID))
        //        {
        //            if (ShoppingCart.ShoppingCartLines.Exists(m => m.ItemID == objPromotionBuy.ItemID))
        //            {
        //                if (objPromotionBuy.Quantity != null)
        //                {
        //                    int intQuantity = objPromotionBuy.Quantity.Value;

        //                    foreach (ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines.OrderByDescending(m => m.UnitPriceBeforeDiscount))
        //                    {
        //                        if (objShoppingCartLine.PromotionEligibleQuantity > 0)
        //                        {
        //                            if (objShoppingCartLine.ItemID == objPromotionBuy.ItemID)
        //                            {
        //                                if (objShoppingCartLine.PromotionEligibleQuantity >= intQuantity)
        //                                {
        //                                    objShoppingCartLine.PromotionEligibleQuantity -= intQuantity;
        //                                    intQuantity -= intQuantity;
        //                                }
        //                                else
        //                                {
        //                                    intQuantity -= objShoppingCartLine.Quantity;
        //                                    objShoppingCartLine.PromotionEligibleQuantity = 0;
        //                                }
        //                            }

        //                            if (intQuantity == 0)
        //                            {
        //                                objPromotionBuy.IsConditionMet = true;
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    objPromotionBuy.IsConditionMet = true;
        //                }
        //            }
        //        }
        //        else if (objPromotionBuy.Quantity != null) //buy 1 get 1 free example
        //        {
        //            if (objPromotionBuy.Quantity > 0)
        //            {
        //                if (objPromotionBuy.Quantity != null)
        //                {
        //                    int intQuantity = objPromotionBuy.Quantity.Value;

        //                    foreach (ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines.OrderByDescending(m => m.UnitPriceBeforeDiscount))
        //                    {
        //                        if (objShoppingCartLine.PromotionEligibleQuantity > 0)
        //                        {
        //                            if (objShoppingCartLine.PromotionEligibleQuantity >= intQuantity)
        //                            {
        //                                objShoppingCartLine.PromotionEligibleQuantity -= intQuantity;
        //                                intQuantity -= intQuantity;
        //                            }
        //                            else
        //                            {
        //                                intQuantity -= objShoppingCartLine.Quantity;
        //                                objShoppingCartLine.PromotionEligibleQuantity = 0;
        //                            }

        //                            if (intQuantity == 0)
        //                            {
        //                                objPromotionBuy.IsConditionMet = true;
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    objPromotionBuy.IsConditionMet = true;
        //                }
        //            }
        //        }
        //    }
        //    return PromotionBuys.Count(m => m.IsConditionMet == true) == PromotionBuys.Count;
        //}

        //private bool IsOrPromotionGetMet(ShoppingCart ShoppingCart, PromotionGet objPromotionGet)
        //{
        //    objPromotionGet.IsConditionMet = false;

        //    if (!string.IsNullOrEmpty(objPromotionGet.ItemDetailID))
        //    {
        //        if (ShoppingCart.ShoppingCartLines.Exists(m => m.ItemDetailID == objPromotionGet.ItemDetailID))
        //        {
        //            int intQuantity = objPromotionGet.Quantity == null ? int.MaxValue : objPromotionGet.Quantity.Value;

        //            foreach (ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines.OrderBy(m => m.UnitPriceBeforeDiscount))
        //            {
        //                //Exclude Sale Item
        //                if (!ExcludeSaleItem || objShoppingCartLine.ItemDetail.SalePrice == null)
        //                {
        //                    if (objShoppingCartLine.PromotionEligibleQuantity > 0)
        //                    {
        //                        if (objShoppingCartLine.ItemDetailID == objPromotionGet.ItemDetailID)
        //                        {
        //                            if (objShoppingCartLine.PromotionEligibleQuantity < intQuantity)
        //                            {
        //                                ApplyDiscount(ShoppingCart, objShoppingCartLine, objShoppingCartLine.PromotionEligibleQuantity);
        //                                intQuantity -= objShoppingCartLine.PromotionEligibleQuantity;
        //                                objPromotionGet.IsConditionMet = true;
        //                            }
        //                            else
        //                            {
        //                                ApplyDiscount(ShoppingCart, objShoppingCartLine, intQuantity);
        //                                intQuantity = 0;
        //                                objPromotionGet.IsConditionMet = true;
        //                            }
        //                        }

        //                        if (intQuantity == 0)
        //                        {
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else if (!string.IsNullOrEmpty(objPromotionGet.ItemCategoryID))
        //    {
        //        if (ShoppingCart.ShoppingCartLines.Exists(m => m.ItemDetail.Item.ItemCategories.Exists(n => n.ItemCategoryID == objPromotionGet.ItemCategoryID)))
        //        {
        //            int intQuantity = objPromotionGet.Quantity == null ? int.MaxValue : objPromotionGet.Quantity.Value;

        //            foreach (ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines.OrderBy(m => m.UnitPriceBeforeDiscount))
        //            {
        //                if (objShoppingCartLine.PromotionEligibleQuantity > 0)
        //                {
        //                    if (objShoppingCartLine.ItemDetail.Item.ItemCategories.Exists(m => m.ItemCategoryID == objPromotionGet.ItemCategoryID))
        //                    {
        //                        if (objShoppingCartLine.PromotionEligibleQuantity < intQuantity)
        //                        {
        //                            ApplyDiscount(ShoppingCart, objShoppingCartLine, objShoppingCartLine.PromotionEligibleQuantity);
        //                            intQuantity -= objShoppingCartLine.PromotionEligibleQuantity;
        //                            objPromotionGet.IsConditionMet = true;
        //                        }
        //                        else
        //                        {
        //                            ApplyDiscount(ShoppingCart, objShoppingCartLine, intQuantity);
        //                            intQuantity = 0;
        //                            objPromotionGet.IsConditionMet = true;
        //                        }
        //                    }

        //                    if (intQuantity == 0)
        //                    {
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else if (!string.IsNullOrEmpty(objPromotionGet.ItemID))
        //    {
        //        if (ShoppingCart.ShoppingCartLines.Exists(m => m.ItemID == objPromotionGet.ItemID))
        //        {
        //            int intQuantity = objPromotionGet.Quantity == null ? int.MaxValue : objPromotionGet.Quantity.Value;

        //            foreach (ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines.OrderBy(m => m.UnitPriceBeforeDiscount))
        //            {
        //                if (objShoppingCartLine.PromotionEligibleQuantity > 0)
        //                {
        //                    if (objShoppingCartLine.ItemID == objPromotionGet.ItemID)
        //                    {
        //                        if (objShoppingCartLine.PromotionEligibleQuantity < intQuantity)
        //                        {
        //                            ApplyDiscount(ShoppingCart, objShoppingCartLine, objShoppingCartLine.PromotionEligibleQuantity);
        //                            intQuantity -= objShoppingCartLine.PromotionEligibleQuantity;
        //                            objPromotionGet.IsConditionMet = true;
        //                        }
        //                        else
        //                        {
        //                            ApplyDiscount(ShoppingCart, objShoppingCartLine, intQuantity);
        //                            intQuantity = 0;
        //                            objPromotionGet.IsConditionMet = true;
        //                        }
        //                    }

        //                    if (intQuantity == 0)
        //                    {
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else if (objPromotionGet.Quantity != null) //Buy one get one free example
        //    {
        //        if (objPromotionGet.Quantity > 0)
        //        {
        //            int intQuantity = objPromotionGet.Quantity == null ? int.MaxValue : objPromotionGet.Quantity.Value;

        //            foreach (ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines.OrderBy(m => m.UnitPriceBeforeDiscount))
        //            {
        //                if (objShoppingCartLine.PromotionEligibleQuantity > 0)
        //                {
        //                    if (objShoppingCartLine.PromotionEligibleQuantity < intQuantity)
        //                    {
        //                        ApplyDiscount(ShoppingCart, objShoppingCartLine, objShoppingCartLine.PromotionEligibleQuantity);
        //                        intQuantity -= objShoppingCartLine.PromotionEligibleQuantity;
        //                        objPromotionGet.IsConditionMet = true;
        //                    }
        //                    else
        //                    {
        //                        ApplyDiscount(ShoppingCart, objShoppingCartLine, intQuantity);
        //                        intQuantity = 0;
        //                        objPromotionGet.IsConditionMet = true;
        //                    }

        //                    if (intQuantity == 0)
        //                    {
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return objPromotionGet.IsConditionMet;
        //}

        //private bool IsAndPromotionGetMet(ShoppingCart ShoppingCart)
        //{
        //    if (PromotionGets == null || PromotionGets.Count == 0)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        foreach (PromotionGet objPromotionGet in PromotionGets)
        //        {
        //            objPromotionGet.IsConditionMet = false;

        //            if (!string.IsNullOrEmpty(objPromotionGet.ItemDetailID))
        //            {
        //                if (ShoppingCart.ShoppingCartLines.Exists(m => m.ItemDetailID == objPromotionGet.ItemDetailID))
        //                {
        //                    int intQuantity = objPromotionGet.Quantity == null ? int.MaxValue : objPromotionGet.Quantity.Value;

        //                    foreach (ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines.OrderBy(m => m.UnitPriceBeforeDiscount))
        //                    {
        //                        //Exclude Sale Item
        //                        if (!ExcludeSaleItem || objShoppingCartLine.ItemDetail.SalePrice == null)
        //                        {
        //                            if (objShoppingCartLine.PromotionEligibleQuantity > 0)
        //                            {
        //                                if (objShoppingCartLine.ItemDetailID == objPromotionGet.ItemDetailID)
        //                                {
        //                                    if (objShoppingCartLine.PromotionEligibleQuantity < intQuantity)
        //                                    {
        //                                        ApplyDiscount(ShoppingCart, objShoppingCartLine, objShoppingCartLine.PromotionEligibleQuantity);
        //                                        intQuantity -= objShoppingCartLine.PromotionEligibleQuantity;
        //                                        objPromotionGet.IsConditionMet = true;
        //                                    }
        //                                    else
        //                                    {
        //                                        ApplyDiscount(ShoppingCart, objShoppingCartLine, intQuantity);
        //                                        intQuantity = 0;
        //                                        objPromotionGet.IsConditionMet = true;
        //                                    }
        //                                }

        //                                if (intQuantity == 0)
        //                                {
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else if (!string.IsNullOrEmpty(objPromotionGet.ItemCategoryID))
        //            {
        //                if (ShoppingCart.ShoppingCartLines.Exists(m => m.ItemDetail.Item.ItemCategories.Exists(n => n.ItemCategoryID == objPromotionGet.ItemCategoryID)))
        //                {
        //                    int intQuantity = objPromotionGet.Quantity == null ? int.MaxValue : objPromotionGet.Quantity.Value;

        //                    foreach (ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines.OrderBy(m => m.UnitPriceBeforeDiscount))
        //                    {
        //                        //Exclude Sale Item
        //                        if (!ExcludeSaleItem || objShoppingCartLine.ItemDetail.SalePrice == null)
        //                        {
        //                            if (objShoppingCartLine.PromotionEligibleQuantity > 0)
        //                            {
        //                                if (objShoppingCartLine.ItemDetail.Item.ItemCategories.Exists(m => m.ItemCategoryID == objPromotionGet.ItemCategoryID))
        //                                {
        //                                    if (objShoppingCartLine.PromotionEligibleQuantity < intQuantity)
        //                                    {
        //                                        ApplyDiscount(ShoppingCart, objShoppingCartLine, objShoppingCartLine.PromotionEligibleQuantity);
        //                                        intQuantity -= objShoppingCartLine.PromotionEligibleQuantity;
        //                                        objPromotionGet.IsConditionMet = true;
        //                                    }
        //                                    else
        //                                    {
        //                                        ApplyDiscount(ShoppingCart, objShoppingCartLine, intQuantity);
        //                                        intQuantity = 0;
        //                                        objPromotionGet.IsConditionMet = true;
        //                                    }
        //                                }

        //                                if (intQuantity == 0)
        //                                {
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else if (!string.IsNullOrEmpty(objPromotionGet.ItemID))
        //            {
        //                if (ShoppingCart.ShoppingCartLines.Exists(m => m.ItemID == objPromotionGet.ItemID))
        //                {
        //                    int intQuantity = objPromotionGet.Quantity == null ? int.MaxValue : objPromotionGet.Quantity.Value;

        //                    foreach (ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines.OrderBy(m => m.UnitPriceBeforeDiscount))
        //                    {
        //                        //Exclude Sale Item
        //                        if (!ExcludeSaleItem || objShoppingCartLine.ItemDetail.SalePrice == null)
        //                        {
        //                            if (objShoppingCartLine.PromotionEligibleQuantity > 0)
        //                            {
        //                                if (objShoppingCartLine.ItemID == objPromotionGet.ItemID)
        //                                {
        //                                    if (objShoppingCartLine.PromotionEligibleQuantity < intQuantity)
        //                                    {
        //                                        ApplyDiscount(ShoppingCart, objShoppingCartLine, objShoppingCartLine.PromotionEligibleQuantity);
        //                                        intQuantity -= objShoppingCartLine.PromotionEligibleQuantity;
        //                                        objPromotionGet.IsConditionMet = true;
        //                                    }
        //                                    else
        //                                    {
        //                                        ApplyDiscount(ShoppingCart, objShoppingCartLine, intQuantity);
        //                                        intQuantity = 0;
        //                                        objPromotionGet.IsConditionMet = true;
        //                                    }
        //                                }

        //                                if (intQuantity == 0)
        //                                {
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else if (objPromotionGet.Quantity != null) //buy one get one free example
        //            {
        //                if (objPromotionGet.Quantity > 0)
        //                {
        //                    int intQuantity = objPromotionGet.Quantity == null ? int.MaxValue : objPromotionGet.Quantity.Value;

        //                    foreach (ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines.OrderBy(m => m.UnitPriceBeforeDiscount))
        //                    {
        //                        //Exclude Sale Item
        //                        if (!ExcludeSaleItem || objShoppingCartLine.ItemDetail.SalePrice == null)
        //                        {
        //                            if (objShoppingCartLine.PromotionEligibleQuantity > 0)
        //                            {
        //                                if (objShoppingCartLine.PromotionEligibleQuantity < intQuantity)
        //                                {
        //                                    ApplyDiscount(ShoppingCart, objShoppingCartLine, objShoppingCartLine.PromotionEligibleQuantity);
        //                                    intQuantity -= objShoppingCartLine.PromotionEligibleQuantity;
        //                                    objPromotionGet.IsConditionMet = true;
        //                                }
        //                                else
        //                                {
        //                                    ApplyDiscount(ShoppingCart, objShoppingCartLine, intQuantity);
        //                                    intQuantity = 0;
        //                                    objPromotionGet.IsConditionMet = true;
        //                                }

        //                                if (intQuantity == 0)
        //                                {
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        return PromotionGets.Count(m => m.IsConditionMet == true) == PromotionGets.Count;
        //    }
        //}

        //private bool ApplyDiscount(ShoppingCart.ShoppingCart ShoppingCart)
        //{
        //    foreach (ShoppingCart.ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines)
        //    {
        //        ApplyDiscount(ShoppingCart, objShoppingCartLine, objShoppingCartLine.Quantity);
        //    }
        //    return true;
        //}

        //private bool ApplyDiscount(ShoppingCart.ShoppingCart ShoppingCart, ShoppingCart.ShoppingCartLine objShoppingCartLine, int ApplyQuantity)
        //{
        //    //Exclude Sale Item
        //    if (!ExcludeSaleItem || objShoppingCartLine.Item.BasePrice == null)
        //    {
        //        if (ApplyQuantity > objShoppingCartLine.PromotionEligibleQuantity) ApplyQuantity = objShoppingCartLine.PromotionEligibleQuantity;

        //        //objShoppingCartLine.DiscountAmount = DiscountAmount == null ? objShoppingCartLine.DiscountAmount : objShoppingCartLine.DiscountAmount + DiscountAmount.Value;
        //        objShoppingCartLine.DiscountAmount = DiscountAmount == null ? objShoppingCartLine.DiscountAmount : objShoppingCartLine.DiscountAmount + objShoppingCartLine.LineTotal * (DiscountAmount.Value / ShoppingCart.LineTotalBeforeDiscount) / objShoppingCartLine.Quantity;

        //        if (DiscountPercent != null)
        //        {
        //            //wrong logic, do not recalculate original percentage
        //            //decimal dcmApplyQuantityPercent = 0;
        //            //if (objShoppingCartLine.DiscountPercent >= 0)
        //            //{
        //            //    dcmApplyQuantityPercent = objShoppingCartLine.DiscountPercent + (1 - objShoppingCartLine.DiscountPercent) * DiscountPercent.Value;
        //            //}
        //            //else
        //            //{
        //            //    dcmApplyQuantityPercent = objShoppingCartLine.DiscountPercent + (1 + objShoppingCartLine.DiscountPercent) * DiscountPercent.Value;
        //            //}
        //            //objShoppingCartLine.DiscountPercent = objShoppingCartLine.DiscountPercent + (dcmApplyQuantityPercent * ApplyQuantity) / objShoppingCartLine.Quantity;

        //            objShoppingCartLine.DiscountPercent = objShoppingCartLine.DiscountPercent + (DiscountPercent.Value * ApplyQuantity) / objShoppingCartLine.Quantity;
        //        }
        //        objShoppingCartLine.PromotionEligibleQuantity -= ApplyQuantity;
        //    }
        //    return true;
        //}

        //private bool AppliedAtLeastOnePromotionGet(ShoppingCart.ShoppingCart ShoppingCart)
        //{
        //    bool blnReturn = false;

        //    if (ShoppingCart.ShoppingCartLines == null || ShoppingCart.ShoppingCartLines.Count == 0)
        //    {
        //        blnReturn = false;
        //    }
        //    else
        //    {
        //        if ((PromotionGets == null || PromotionGets.Count == 0) && ShoppingCart.ShoppingCartLines.Exists(m => m.PromotionEligibleQuantity > 0))
        //        {
        //            ApplyDiscount(ShoppingCart);
        //            blnReturn = true;
        //        }
        //        else
        //        {
        //            if (IsOrPromotionGet)
        //            {
        //                foreach (PromotionGet objPromotionGet in PromotionGets)
        //                {
        //                    bool blnPromotionGetMet = true;

        //                    blnPromotionGetMet = IsOrPromotionGetMet(ShoppingCart, objPromotionGet);
        //                    if (blnPromotionGetMet) blnReturn = true;
        //                    if (blnReturn) break;
        //                }
        //            }
        //            else
        //            {
        //                blnReturn = IsAndPromotionGetMet(ShoppingCart);
        //            }
        //        }
        //    }

        //    return blnReturn;
        //}

        //private void ApplyPromotion(ShoppingCart.ShoppingCart ShoppingCart)
        //{
        //    bool blnAtLeastOnePromotionBuysMet = false;
        //    bool blnAtLeastOnePromotionGetMet = false;

        //    if (PromotionBuys == null || PromotionBuys.Count == 0)
        //    {
        //        blnAtLeastOnePromotionBuysMet = true;

        //        //If cart is empty and promotiongets is empty, then allow the code to be added (e.g. cart abandonment)
        //        if ((PromotionGets == null || PromotionGets.Count == 0) && (ShoppingCart.ShoppingCartLines == null || ShoppingCart.ShoppingCartLines.Count == 0))
        //            blnAtLeastOnePromotionGetMet = true;
        //        else
        //            blnAtLeastOnePromotionGetMet = AppliedAtLeastOnePromotionGet(ShoppingCart);
        //    }
        //    else
        //    {
        //        if (IsOrPromotionBuy)
        //        {
        //            foreach (PromotionBuy objPromotionBuy in PromotionBuys)
        //            {
        //                bool blnPromotionBuysMet = true;

        //                while (blnPromotionBuysMet)
        //                {
        //                    blnPromotionBuysMet = IsOrPromotionBuyMet(ShoppingCart, objPromotionBuy);

        //                    if (blnPromotionBuysMet)
        //                    {
        //                        blnAtLeastOnePromotionBuysMet = true;

        //                        blnPromotionBuysMet = AppliedAtLeastOnePromotionGet(ShoppingCart);
        //                        if (blnPromotionBuysMet) blnAtLeastOnePromotionGetMet = true;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            bool blnPromotionBuysMet = true;

        //            while (blnPromotionBuysMet)
        //            {
        //                blnPromotionBuysMet = IsAndPromotionBuyMet(ShoppingCart);

        //                if (blnPromotionBuysMet)
        //                {
        //                    blnAtLeastOnePromotionBuysMet = true;

        //                    blnPromotionBuysMet = AppliedAtLeastOnePromotionGet(ShoppingCart);
        //                    if (blnPromotionBuysMet) blnAtLeastOnePromotionGetMet = true;
        //                }
        //            }
        //        }
        //    }

        //    if (!blnAtLeastOnePromotionBuysMet) throw new Exception("Promotion code '" + PromotionCode + "' is not valid on your purchase (code:1100b)");
        //    if (!blnAtLeastOnePromotionGetMet) throw new Exception("Promotion code '" + PromotionCode + "' is not valid on your purchase (code:1101g)");
        //}

        public bool IsValid(ShoppingCart.ShoppingCart ShoppingCart)
        {
            try
            {
                IsValid();
                //if (!string.IsNullOrEmpty(CustomerID) && ShoppingCart.CustomerID != CustomerID) throw new Exception("This Promotion code is not valid.");

                if (MinOrderAmount != null && Convert.ToDecimal(ShoppingCart.LineTotalBeforeDiscount) < MinOrderAmount.Value) throw new Exception("Promotion code '" + PromotionCode + "' is valid on orders of " + string.Format("{0:c}", MinOrderAmount.Value) + " or more.");
                if (MaxOrderAmount != null && Convert.ToDecimal(ShoppingCart.LineTotalBeforeDiscount) > MaxOrderAmount.Value) throw new Exception("Promotion code '" + PromotionCode + "' is valid on orders of " + string.Format("{0:c}", MaxOrderAmount.Value) + " or less.");

                //if (!CanBeCombined && ShoppingCart.Promotions != null && ShoppingCart.Promotions.FindAll(m => m.PromotionID != PromotionID).Count > 0) throw new Exception("Promotion code '" + PromotionCode + "' cannot be combined with other offers");
                //if (ShoppingCart.Promotions != null && ShoppingCart.Promotions.Count >= 1)
                //{
                //    int intCanNotBeCombined = ShoppingCart.Promotions.Count(m => !m.CanBeCombined);

                //    if (intCanNotBeCombined > 1) throw new Exception("Promotion code '" + ShoppingCart.Promotions.Find(m => !m.CanBeCombined).PromotionCode + "' cannot be combined with other offers");

                //    if (intCanNotBeCombined > 0 && !ShoppingCart.Promotions.Exists(m => m.PromotionID == this.PromotionID)) throw new Exception("Promotion code '" + ShoppingCart.Promotions.Find(m => !m.CanBeCombined).PromotionCode + "' cannot be combined with other offers");

                //    if (ShoppingCart.Promotions.Count > 0 && !ShoppingCart.Promotions.Exists(m => m.PromotionID == this.PromotionID) && !this.CanBeCombined) throw new Exception("Promotion code '" + this.PromotionCode + "' cannot be combined with other offers");
                //}

                //if (ShoppingCart.GiftCertificates != null && ShoppingCart.GiftCertificates.Count > 0 && ShoppingCart.GiftCertificates.Exists(m => !m.CanBeCombinedWithPromotion)) throw new Exception("Gift certificate code '" + ShoppingCart.GiftCertificates.Find(m => !m.CanBeCombinedWithPromotion).GiftCertificateCode + "' cannot be combined with other promotions");

                //if (ShoppingCart.ShoppingCartLines != null || ShoppingCart.ShoppingCartLines.Count > 0) ApplyPromotion(ShoppingCart);

                //if (CanBeCombined)
                //{
                //    foreach (ShoppingCartLine objShoppingCartLine in ShoppingCart.ShoppingCartLines)
                //    {
                //        objShoppingCartLine.ResetPromotionEligibleQuantity();
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return true;
        }

        //public bool IsValid(SalesOrder.SalesOrder SalesOrder)
        //{
        //    try
        //    {
        //        IsValid();
        //        if (MinOrderAmount != null && SalesOrder.LineTotalBeforeDiscount < MinOrderAmount.Value) throw new Exception("Promotion code '" + PromotionCode + "' is valid on orders of " + string.Format("{0:c}", MinOrderAmount.Value) + " or more.");
        //        if (MaxOrderAmount != null && SalesOrder.LineTotalBeforeDiscount > MaxOrderAmount.Value) throw new Exception("Promotion code '" + PromotionCode + "' is valid on orders of " + string.Format("{0:c}", MaxOrderAmount.Value) + " or less.");

        //        if (!CanBeCombined && SalesOrder.Promotions != null && SalesOrder.Promotions.FindAll(m => m.PromotionID != PromotionID).Count > 0) throw new Exception("Promotion code '" + PromotionCode + "' cannot be combined with other offers");
        //        if (SalesOrder.Promotions != null && SalesOrder.Promotions.Count > 1 && SalesOrder.Promotions.Exists(m => !m.CanBeCombined)) throw new Exception("Promotion code '" + SalesOrder.Promotions.Find(m => !m.CanBeCombined).PromotionCode + "' cannot be combined with other offers");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return true;
        //}

        //public bool IsValid(CustInvoice.CustInvoice CustInvoice)
        //{
        //    try
        //    {
        //        IsValid();
        //        if (MinOrderAmount != null && CustInvoice.LineTotalBeforeDiscount < MinOrderAmount.Value) throw new Exception("Promotion code '" + PromotionCode + "' is valid on orders of " + string.Format("{0:c}", MinOrderAmount.Value) + " or more.");
        //        if (MaxOrderAmount != null && CustInvoice.LineTotalBeforeDiscount > MaxOrderAmount.Value) throw new Exception("Promotion code '" + PromotionCode + "' is valid on orders of " + string.Format("{0:c}", MinOrderAmount.Value) + " or less.");

        //        if (!CanBeCombined && CustInvoice.Promotions != null && CustInvoice.Promotions.FindAll(m => m.PromotionID != PromotionID).Count > 0) throw new Exception("Promotion code '" + PromotionCode + "' cannot be combined with other offers");
        //        if (CustInvoice.Promotions != null && CustInvoice.Promotions.Count > 1 && CustInvoice.Promotions.Exists(m => !m.CanBeCombined)) throw new Exception("Promotion code '" + CustInvoice.Promotions.Find(m => !m.CanBeCombined).PromotionCode + "' cannot be combined with other offers");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return true;
        //}

        public static Promotion GetPromotionByPromotionCode(string WebsiteID, string PromotionCode)
        {
            Promotion objReturn = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(PromotionCode))
                {
                    strSQL = "SELECT p.* " +
                             "FROM Promotion p (NOLOCK) " +
                             "WHERE p.WebsiteID=" + Database.HandleQuote(WebsiteID) +
                             "AND p.PromotionCode=" + Database.HandleQuote(PromotionCode);

                    objData = Database.GetDataSet(strSQL);

                    if (objData != null && objData.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                        {
                            objReturn = new Promotion(objData.Tables[0].Rows[i]);
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

        public static List<Promotion> GetPromotions(string WebsiteID)
        {
            List<Promotion> objReturn = null;
            Promotion objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(WebsiteID))
                {
                    objReturn = new List<Promotion>();

                    strSQL = "SELECT * " +
                             "FROM Promotion (NOLOCK) " +
                             "WHERE WebsiteID=" + Database.HandleQuote(WebsiteID);

                    objData = Database.GetDataSet(strSQL);

                    if (objData != null && objData.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                        {
                            objNew = new Promotion(objData.Tables[0].Rows[i]);
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

        //public static List<Promotion> GetShoppingCartPromotion(string WebsiteID, string ShoppingCartGuid)
        //{
        //    List<Promotion> objReturn = null;
        //    Promotion objNew = null;
        //    DataSet objData = null;
        //    string strSQL = string.Empty;

        //    try
        //    {
        //        if (!string.IsNullOrEmpty(WebsiteID))
        //        {
        //            objReturn = new List<Promotion>();

        //            strSQL = "SELECT p.* " +
        //                     "FROM ShoppingCartPromotion (NOLOCK) sp " +
        //                     "INNER JOIN ShoppingCart sc (NOLOCK) ON sp.ShoppingCartID=sc.ShoppingCartID " +
        //                     "INNER JOIN Promotion p (NOLOCK) ON sp.PromotionID=p.PromotionID AND sp.BusinessID=p.BusinessID " +
        //                     "WHERE sp.IsActive=1 " +
        //                     "AND sp.WebsiteID=" + Database.HandleQuote(WebsiteID) +
        //                     "AND sc.ShoppingCartGuid=" + Database.HandleQuote(ShoppingCartGuid) +
        //                     "ORDER BY ShoppingCartPromotionID DESC";

        //            objData = Database.GetDataSet(strSQL);

        //            if (objData != null && objData.Tables[0].Rows.Count > 0)
        //            {
        //                for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
        //                {
        //                    objNew = new Promotion(objData.Tables[0].Rows[i]);
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

        public static List<Promotion> GetPromotion(string WebsiteID)
        {
            int intTotalCount = 0;
            return GetPromotion(WebsiteID, null, null, null, out intTotalCount);
        }
        public static List<Promotion> GetPromotion(string WebsiteID, PromotionFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetPromotion(WebsiteID, Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Promotion> GetPromotion(string WebsiteID, PromotionFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Promotion> objReturn = null;
            Promotion objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                if (!string.IsNullOrEmpty(WebsiteID))
                {
                    objReturn = new List<Promotion>();

                    strSQL = "SELECT p.* " +
                             "FROM Promotion p (NOLOCK) " +
                             //"LEFT OUTER JOIN Customer c (NOLOCK) ON p.CustomerID=c.CustomerID " +
                             "WHERE p.WebsiteID=" + Database.HandleQuote(WebsiteID);

                    if (Filter != null)
                    {
                        if (!string.IsNullOrEmpty(Filter.WebsiteID)) strSQL += "AND p.WebsiteID = " + Database.HandleQuote(Filter.WebsiteID);
                        if (!string.IsNullOrEmpty(Filter.PromotionCode)) strSQL += "AND p.PromotionCode LIKE " + Database.HandleQuote(Filter.PromotionCode.Replace("*", "%"));
                        if (!string.IsNullOrEmpty(Filter.PromotionName)) strSQL += "AND p.PromotionName LIKE " + Database.HandleQuote(Filter.PromotionName.Replace("*", "%"));
                        //if (!string.IsNullOrEmpty(Filter.CustomerEmail)) strSQL += "AND c.Email LIKE " + Database.HandleQuote(Filter.CustomerEmail.Replace("*", "%"));
                        //if (!string.IsNullOrEmpty(Filter.CustomerFirstName)) strSQL += "AND c.FirstName LIKE " + Database.HandleQuote(Filter.CustomerFirstName.Replace("*", "%"));
                        //if (!string.IsNullOrEmpty(Filter.CustomerLastName)) strSQL += "AND c.LastName LIKE " + Database.HandleQuote(Filter.CustomerLastName.Replace("*", "%"));

                        if (Filter.FromDate != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.FromDate, "p.FromDate");
                        if (Filter.ToDate != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.ToDate, "p.ToDate");

                        if (Filter.IsActive != null) strSQL += "AND IsActive=" + Database.HandleQuote(Convert.ToInt32(Filter.IsActive.Value).ToString());
                    }

                    if (PageSize != null && PageNumber != null)
                        strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "CreatedOn" : Utility.CustomSorting.GetSortExpression(typeof(Promotion), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                    else
                        strSQL += "ORDER BY CreatedOn ";
                    objData = Database.GetDataSet(strSQL);

                    if (objData != null && objData.Tables[0].Rows.Count > 0)
                    {
                        if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                        for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                        {
                            objNew = new Promotion(objData.Tables[0].Rows[i]);
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
                objNew = null;
            }
            return objReturn;
        }
    }
}

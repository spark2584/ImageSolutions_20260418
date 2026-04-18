using ImageSolutions.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.SalesOrder
{
    public class SalesOrderLine : ISBase.BaseClass
    {
        public string SalesOrderLineID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(SalesOrderLineID); } }
        public string SalesOrderID { get; set; }
        public string ExternalID { get; set; }
        public string ParentLineExternalID { get; set; }
        public string ParentSalesOrderLineID { get; set; }
        public long? NetSuiteLineID { get; set; }
        public string Location { get; set; }
        public string ItemID { get; set; }
        public string ItemInternalID { get; set; }
        public string Embellishment { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double? OnlinePrice { get; set; }
        public double? TariffCharge { get; set; }
        public string UserInfoID { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public string CustomListID_1 { get; set; }
        public string CustomListValueID_1 { get; set; }
        public string CustomListID_2 { get; set; }
        public string CustomListValueID_2 { get; set; }
        public string CustomDesignImagePath { get; set; }
        public string CustomDesignName { get; set; }
        public double? FinalPrice { get; set; }
        public double? TaxAmount { get; set; }
        public double? TaxRate { get; set; }
        public double? DiscountAmount { get; set; }
        public double? DiscountRate { get; set; }
        public DateTime CreatedOn { get; private set; }
        public DateTime? UpdatedOn { get; private set; }
        public double LineTotal { get { return Quantity * UnitPrice; } }

        public string LogosDescription
        {
            get
            {
                string _ret = string.Empty;

                if (Item.ParentItem != null && Item.ParentItem.EnableSelectableLogo && Item.ParentItem.ItemSelectableLogos.Count > 0)
                {
                    if (SalesOrderLineSelectableLogos != null && SalesOrderLineSelectableLogos.Count > 0)
                    {
                        foreach (SalesOrderLineSelectableLogo _SalesOrderLineSelectableLogo in SalesOrderLineSelectableLogos)
                        {
                            if (string.IsNullOrEmpty(_ret))
                            {
                                _ret = string.Format(@"Logo: {0}{1}{2}"
                                    , !string.IsNullOrEmpty(_SalesOrderLineSelectableLogo.SelectableLogoID)
                                        ? _SalesOrderLineSelectableLogo.SelectableLogo.Name
                                        : "No Logo"
                                    , string.IsNullOrEmpty(_SalesOrderLineSelectableLogo.SelectYear) ? string.Empty : string.Format(" ({0}: {1})", string.IsNullOrEmpty(_SalesOrderLineSelectableLogo.SelectableLogo.SelectYearsLabel) ? "Year" : _SalesOrderLineSelectableLogo.SelectableLogo.SelectYearsLabel, _SalesOrderLineSelectableLogo.SelectYear)
                                    , _SalesOrderLineSelectableLogo.BasePrice != null && _SalesOrderLineSelectableLogo.BasePrice > 0
                                        ? string.Format(" - {0:C}", Convert.ToDecimal(_SalesOrderLineSelectableLogo.BasePrice))
                                        : string.Empty
                                    );
                            }
                            else
                            {
                                _ret = string.Format(@"{0}, {1}", _ret
                                    , !string.IsNullOrEmpty(_SalesOrderLineSelectableLogo.SelectableLogoID)
                                        ? _SalesOrderLineSelectableLogo.SelectableLogo.Name
                                        : "No Logo");
                            }
                        }
                    }
                    else
                    {
                        _ret = string.Format(@"Logo: {1}", _ret, "No Logo");
                    }
                }
                return _ret;
            }
        }

        public string PersonalizationDescription
        {
            get
            {
                string _ret = string.Empty;
                if (ItemPersonalizationValues != null && ItemPersonalizationValues.Count > 0)
                {
                    foreach (Item.ItemPersonalizationValue _ItemPersonalizationValue in ItemPersonalizationValues)
                    {
                        if (string.IsNullOrEmpty(_ret))
                        {
                            _ret = string.Format(@"{0}({1})", _ItemPersonalizationValue.Value
                                , !string.IsNullOrEmpty(_ItemPersonalizationValue.TextOption)
                                    ? _ItemPersonalizationValue.TextOption
                                    : !string.IsNullOrEmpty(_ItemPersonalizationValue.ItemPersonalization.Label)
                                        ? _ItemPersonalizationValue.ItemPersonalization.Label
                                        : _ItemPersonalizationValue.ItemPersonalization.Name);
                        }
                        else
                        {
                            _ret = string.Format(@"{0}, {1}({2})", _ret, _ItemPersonalizationValue.Value
                                , !string.IsNullOrEmpty(_ItemPersonalizationValue.TextOption)
                                    ? _ItemPersonalizationValue.TextOption
                                    : !string.IsNullOrEmpty(_ItemPersonalizationValue.ItemPersonalization.Label)
                                        ? _ItemPersonalizationValue.ItemPersonalization.Label
                                        : _ItemPersonalizationValue.ItemPersonalization.Name);
                        }
                    }

                    if (PersonalizationPrice > 0)
                    {
                        _ret = String.Format("{0}-{1:C}", _ret, PersonalizationPrice);
                    }
                }
                return _ret;
            }
        }
        public bool HasCusomtization
        {
            get
            {
                bool _ret = false;
                if (SalesOrderLineSelectableLogos != null && SalesOrderLineSelectableLogos.Count > 0) _ret = true;
                if (ItemPersonalizationValues != null && ItemPersonalizationValues.Count > 0) _ret = true;
                return _ret;
            }
        }

        public double PersonalizationPrice
        {
            get
            {
                double _ret = 0;

                if (ItemPersonalizationValues != null && ItemPersonalizationValues.Count > 0)
                {
                    if (Item.ParentItem != null && Item.ParentItem.ItemSelectableLogos != null && Item.ParentItem.ItemSelectableLogos.Count > 0
                        && ItemPersonalizationValues.Exists(x => !string.IsNullOrEmpty(x.Value)))
                    {
                        List<ImageSolutions.Item.ItemSelectableLogo> ItemSelectableLogos = Item.ParentItem.ItemSelectableLogos.FindAll(x => !x.SelectableLogo.Inactive && x.SelectableLogo.IsPersonalization);

                        if (ItemSelectableLogos != null && ItemSelectableLogos.Count > 0)
                        {
                            _ret = Math.Round(Convert.ToDouble(ItemSelectableLogos.Sum(y => y.SelectableLogo.BasePrice == null ? 0 : y.SelectableLogo.BasePrice)), 2);
                        }
                    }
                }

                return _ret;
            }
        }

        public string PersonalizationListHTML
        {
            get
            {
                string strPersonalizationList = string.Empty;

                foreach (ImageSolutions.Item.ItemPersonalizationValue _ItemPersonalizationValue in ItemPersonalizationValues)
                {
                    if(string.IsNullOrEmpty(strPersonalizationList))
                    {
                        strPersonalizationList = string.Format(@"{0}: {1}"
                            , _ItemPersonalizationValue.ItemPersonalization.Name
                            , _ItemPersonalizationValue.TextOption == "No Embroidery" ? "No Embroidery" : _ItemPersonalizationValue.Value);

                    }
                    else
                    {
                        strPersonalizationList = string.Format(@"{0}<br>{1}: {2}"
                            , strPersonalizationList
                            , _ItemPersonalizationValue.ItemPersonalization.Name
                            , _ItemPersonalizationValue.Value);
                    }
                }

                return strPersonalizationList;
            }
        }

        private SalesOrder mSalesOrder = null;
        public SalesOrder SalesOrder
        {
            get
            {
                if (mSalesOrder == null && !string.IsNullOrEmpty(SalesOrderID))
                {
                    mSalesOrder = new SalesOrder(SalesOrderID);
                }
                return mSalesOrder;
            }
        }

        private UserInfo mUserInfo = null;
        public UserInfo UserInfo
        {
            get
            {
                if (mUserInfo == null && !string.IsNullOrEmpty(UserInfoID))
                {
                    mUserInfo = new UserInfo(UserInfoID);
                }
                return mUserInfo;
            }
        }

        private Custom.CustomList mCustomList_1 = null;
        public Custom.CustomList CustomList_1
        {
            get
            {
                if (mCustomList_1 == null && !string.IsNullOrEmpty(CustomListID_1))
                {
                    mCustomList_1 = new Custom.CustomList(CustomListID_1);
                }
                return mCustomList_1;
            }
        }

        private Custom.CustomListValue mCustomListValue_1 = null;
        public Custom.CustomListValue CustomListValue_1
        {
            get
            {
                if (mCustomListValue_1 == null && !string.IsNullOrEmpty(CustomListValueID_1))
                {
                    mCustomListValue_1 = new Custom.CustomListValue(CustomListValueID_1);
                }
                return mCustomListValue_1;
            }
        }

        private Custom.CustomList mCustomList_2 = null;
        public Custom.CustomList CustomList_2
        {
            get
            {
                if (mCustomList_2 == null && !string.IsNullOrEmpty(CustomListID_2))
                {
                    mCustomList_2 = new Custom.CustomList(CustomListID_2);
                }
                return mCustomList_2;
            }
        }

        private Custom.CustomListValue mCustomListValue_2 = null;
        public Custom.CustomListValue CustomListValue_2
        {
            get
            {
                if (mCustomListValue_2 == null && !string.IsNullOrEmpty(CustomListValueID_2))
                {
                    mCustomListValue_2 = new Custom.CustomListValue(CustomListValueID_2);
                }
                return mCustomListValue_2;
            }
        }

        private List<Fulfillment.FulfillmentLine> mFulfillmentLines = null;
        public List<Fulfillment.FulfillmentLine> FulfillmentLines
        {
            get
            {
                if (mFulfillmentLines == null && !string.IsNullOrEmpty(SalesOrderLineID))
                {
                    Fulfillment.FulfillmentLineFilter objFilter = null;

                    try
                    {
                        objFilter = new Fulfillment.FulfillmentLineFilter();
                        objFilter.SalesOrderLineID = SalesOrderLineID;
                        mFulfillmentLines = Fulfillment.FulfillmentLine.GetFulfillmentLines(objFilter);
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
                return mFulfillmentLines;
            }
        }

        private List<SalesOrderLineSelectableLogo> mSalesOrderLineSelectableLogos = null;
        public List<SalesOrderLineSelectableLogo> SalesOrderLineSelectableLogos
        {
            get
            {
                if (mSalesOrderLineSelectableLogos == null && !string.IsNullOrEmpty(SalesOrderLineID))
                {
                    SalesOrderLineSelectableLogoFilter objFilter = null;

                    try
                    {
                        objFilter = new SalesOrderLineSelectableLogoFilter();
                        objFilter.SalesOrderLineID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.SalesOrderLineID.SearchString = SalesOrderLineID;

                        mSalesOrderLineSelectableLogos = SalesOrderLineSelectableLogo.GetSalesOrderLineSelectableLogos(objFilter);

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
                return mSalesOrderLineSelectableLogos;
            }
            set
            {
                mSalesOrderLineSelectableLogos = value;
            }
        }

        private List<Item.ItemPersonalizationValue> mItemPersonalizationValues = null;
        public List<Item.ItemPersonalizationValue> ItemPersonalizationValues
        {
            get
            {
                if (mItemPersonalizationValues == null && !string.IsNullOrEmpty(SalesOrderLineID))
                {
                    Item.ItemPersonalizationValueFilter objFilter = null;

                    try
                    {
                        objFilter = new Item.ItemPersonalizationValueFilter();
                        objFilter.SalesOrderLineID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.SalesOrderLineID.SearchString = SalesOrderLineID;

                        mItemPersonalizationValues = ImageSolutions.Item.ItemPersonalizationValue.GetItemPersonalizationValues(objFilter);

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
                return mItemPersonalizationValues;
            }
            set
            {
                mItemPersonalizationValues = value;
            }
        }

        private SalesOrderLine mParentSalesOrderLine = null;
        public SalesOrderLine ParentSalesOrderLine
        {
            get
            {
                if (mParentSalesOrderLine == null && !string.IsNullOrEmpty(ParentLineExternalID))
                {
                    SalesOrderLineFilter objFilter = null;

                    try
                    {
                        //objFilter = new SalesOrderLineFilter();
                        //objFilter.ExternalID = ParentLineExternalID;
                        //mParentSalesOrderLine = SalesOrderLine.GetSalesOrderLine(objFilter);

                        objFilter = new SalesOrderLineFilter();
                        objFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ExternalID.SearchString = ParentLineExternalID;
                        mParentSalesOrderLine = SalesOrderLine.GetSalesOrderLine(objFilter);
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
                return mParentSalesOrderLine;
            }
        }

        private List<RMA.RMALine> mRMALines = null;
        public List<RMA.RMALine> RMALines
        {
            get
            {
                if (mRMALines == null && !string.IsNullOrEmpty(SalesOrderLineID))
                {
                    RMA.RMALineFilter objFilter = null;

                    try
                    {
                        objFilter = new RMA.RMALineFilter();
                        objFilter.SalesOrderLineID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.SalesOrderLineID.SearchString = SalesOrderLineID;
                        mRMALines = RMA.RMALine.GetRMALines(objFilter);
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
                return mRMALines;
            }
        }
        public int RMAAvailableQuantity
        {
            get
            {
                int intRMAQuantity = 0;

                if (RMALines != null && RMALines.Count > 0)
                {
                    intRMAQuantity = RMALines.Sum(x => x.Quantity);
                }

                return Quantity - intRMAQuantity;
            }
        }

        //private List<SalesOrderLine> mItemGroupComponentLines = null;
        //public List<SalesOrderLine> ItemGroupComponentLines
        //{
        //    get
        //    {
        //        if (mItemGroupComponentLines == null && (!string.IsNullOrEmpty(ParentLineExternalID) || Item.IsKit))
        //        // if (mItemGroupComponentLines == null)

        //        {
        //            SalesOrderLineFilter objFilter = null;
        //            try
        //            {
        //                objFilter = new SalesOrderLineFilter();
        //                //objFilter.ParentLineExternalID = Item.IsKit ? ExternalID : ParentLineExternalID;

        //                objFilter.ParentLineExternalID = new Database.Filter.StringSearch.SearchFilter();
        //                objFilter.ParentLineExternalID.SearchString = Item.IsKit ? ExternalID : ParentLineExternalID;

        //                mItemGroupComponentLines = SalesOrderLine.GetSalesOrderLines(objFilter);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //            finally
        //            {
        //                objFilter = null;
        //            }
        //        }
        //        return mItemGroupComponentLines;
        //    }
        //}
        //public decimal? ItemGroupComponentTotalPrice
        //{
        //    get
        //    {
        //        if (ItemGroupComponentLines != null)
        //        {
        //            if (Item.IsKit)
        //                return ItemGroupComponentLines.Sum(c => (c.Quantity / Quantity) * c.Item.Price);
        //            else
        //                return ItemGroupComponentLines.Sum(c => (c.Quantity / ParentSalesOrderLine.Quantity) * c.Item.Price);
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        private Item.Item mItem = null;
        public Item.Item Item
        {
            get
            {
                if (mItem == null && !string.IsNullOrEmpty(ItemID))
                {
                    mItem = new ImageSolutions.Item.Item(ItemID);
                }
                return mItem;
            }
            //set
            //{
            //    mItem = value;
            //}
        }

        private PurchaseOrder.PurchaseOrderLine mPurchaseOrderLine = null;
        public PurchaseOrder.PurchaseOrderLine PurchaseOrderLine
        {
            get
            {
                if (mPurchaseOrderLine == null && !string.IsNullOrEmpty(SalesOrderLineID))
                {
                    PurchaseOrder.PurchaseOrderLineFilter objFilter = null;

                    try
                    {
                        objFilter = new PurchaseOrder.PurchaseOrderLineFilter();
                        objFilter.SalesOrderLineID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.SalesOrderLineID.SearchString = SalesOrderLineID;
                        mPurchaseOrderLine = PurchaseOrder.PurchaseOrderLine.GetPurchaseOrderLine(objFilter);
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
                return mPurchaseOrderLine;
            }
            //set
            //{
            //    mItem = value;
            //}
        }

        public int QuantityPicked
        {
            get
            {
                int intReturn = 0;

                if (FulfillmentLines != null)
                {
                    foreach (Fulfillment.FulfillmentLine objFulfillmentLine in FulfillmentLines)
                    {
                        if (!string.IsNullOrEmpty(objFulfillmentLine.Fulfillment.InternalID))
                        {
                            intReturn += Convert.ToInt32(objFulfillmentLine.Quantity);
                        }
                    }
                }
                return intReturn;
            }
        }

        //private ShippingMethod mShippingMethod = null;
        //public ShippingMethod ShippingMethod
        //{
        //    get
        //    {
        //        if (mShippingMethod == null && !string.IsNullOrEmpty(ShippingCode) && SalesOrder != null && !string.IsNullOrEmpty(SalesOrder.RetailerID))
        //        {
        //            ShippingMethodFilter objFilter = null;

        //            try
        //            {
        //                objFilter = new ShippingMethodFilter();
        //                objFilter.RetailerID = SalesOrder.RetailerID;
        //                objFilter.ShippingCode = ShippingCode;
        //                mShippingMethod = ShippingMethod.GetShippingMethod(objFilter);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //            finally
        //            {
        //                objFilter = null;
        //            }
        //        }
        //        return mShippingMethod;
        //    }
        //}

        //public int QuantityPicked
        //{
        //    get
        //    {
        //        int intReturn = 0;

        //        if (FulfillmentLines != null)
        //        {
        //            foreach (FulfillmentLine objFulfillmentLine in FulfillmentLines)
        //            {
        //                if (!string.IsNullOrEmpty(objFulfillmentLine.Fulfillment.NetSuiteInternalID))
        //                {
        //                    intReturn += objFulfillmentLine.Quantity;
        //                }
        //            }
        //        }
        //        return intReturn;
        //    }
        //}

        //public int QuantityShipped
        //{
        //    get
        //    {
        //        int intReturn = 0;

        //        if (FulfillmentLines != null)
        //        {
        //            foreach (FulfillmentLine objFulfillmentLine in FulfillmentLines)
        //            {
        //                if (objFulfillmentLine.Fulfillment.IsShipped)
        //                {
        //                    intReturn += objFulfillmentLine.Quantity;
        //                }
        //            }
        //        }
        //        return intReturn;
        //    }
        //}

        //public int QuantityCancelled
        //{
        //    get
        //    {
        //        int intReturn = 0;

        //        if (FulfillmentLines != null)
        //        {
        //            foreach (FulfillmentLine objFulfillmentLine in FulfillmentLines)
        //            {
        //                if (objFulfillmentLine.Fulfillment.IsCancelled)
        //                {
        //                    intReturn += objFulfillmentLine.Quantity;
        //                }
        //            }
        //        }
        //        return intReturn;
        //    }
        //}

        //public bool IsCompleted
        //{
        //    get
        //    {
        //        return Quantity == QuantityShipped + QuantityCancelled;
        //    }
        //}

        public double? LineSubTotal
        {
            get
            {
                return Quantity * UnitPrice - (DiscountAmount != null ? DiscountAmount.Value : 0);
            }
        }

        public decimal? VendorLineSubTotal { get; set; }

        public SalesOrderLine()
        {
        }

        public SalesOrderLine(string SalesOrderLineID)
        {
            this.SalesOrderLineID = SalesOrderLineID;
            Load();
        }

        public SalesOrderLine(DataRow objRow)
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
SELECT s.Status, sl.* 
FROM SalesOrderLine (NOLOCK) sl 
INNER JOIN SalesOrder (NOLOCK) s ON sl.SalesOrderID=s.SalesOrderID
WHERE sl.SalesOrderLineID = {0} "
                    , Database.HandleQuote(SalesOrderLineID)
);

                //strSQL = "SELECT * " +
                //         "FROM SalesOrderLine (NOLOCK) " +
                //         "WHERE SalesOrderLineID=" + Database.HandleQuote(SalesOrderLineID);

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("SalesOrderLineID=" + SalesOrderLineID + " is not found");
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

                if (objColumns.Contains("SalesOrderLineID")) SalesOrderLineID = Convert.ToString(objRow["SalesOrderLineID"]);
                if (objColumns.Contains("SalesOrderID")) SalesOrderID = Convert.ToString(objRow["SalesOrderID"]);
                //if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
                //if (objColumns.Contains("NetSuiteLineID") && objRow["NetSuiteLineID"] != DBNull.Value) NetSuiteLineID = Convert.ToInt64(objRow["NetSuiteLineID"]);
                //if (objColumns.Contains("Location")) Location = Convert.ToString(objRow["Location"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("ItemInternalID")) ItemInternalID = Convert.ToString(objRow["ItemInternalID"]);
                if (objColumns.Contains("Quantity") && objRow["Quantity"] != DBNull.Value) Quantity = Convert.ToInt32(objRow["Quantity"]);

                if (objColumns.Contains("UnitPrice") && objRow["UnitPrice"] != DBNull.Value) UnitPrice = Convert.ToDouble(objRow["UnitPrice"]);
                if (objColumns.Contains("OnlinePrice") && objRow["OnlinePrice"] != DBNull.Value) OnlinePrice = Convert.ToDouble(objRow["OnlinePrice"]);
                if (objColumns.Contains("TariffCharge") && objRow["TariffCharge"] != DBNull.Value) TariffCharge = Convert.ToDouble(objRow["TariffCharge"]);
                if (objColumns.Contains("UserInfoID")) UserInfoID = Convert.ToString(objRow["UserInfoID"]);

                if (objColumns.Contains("SKU")) SKU = Convert.ToString(objRow["SKU"]);
                if (objColumns.Contains("Description")) Description = Convert.ToString(objRow["Description"]);

                if (objColumns.Contains("CustomListID_1")) CustomListID_1 = Convert.ToString(objRow["CustomListID_1"]);
                if (objColumns.Contains("CustomListValueID_1")) CustomListValueID_1 = Convert.ToString(objRow["CustomListValueID_1"]);
                if (objColumns.Contains("CustomListID_2")) CustomListID_2 = Convert.ToString(objRow["CustomListID_2"]);
                if (objColumns.Contains("CustomListValueID_2")) CustomListValueID_2 = Convert.ToString(objRow["CustomListValueID_2"]);
                if (objColumns.Contains("CustomDesignImagePath")) CustomDesignImagePath = Convert.ToString(objRow["CustomDesignImagePath"]);
                if (objColumns.Contains("CustomDesignName")) CustomDesignName = Convert.ToString(objRow["CustomDesignName"]);
                if (objColumns.Contains("Embellishment")) Embellishment = Convert.ToString(objRow["Embellishment"]);

                //if (objColumns.Contains("FinalPrice") && objRow["FinalPrice"] != DBNull.Value) FinalPrice = Convert.ToDecimal(objRow["FinalPrice"]);
                //if (objColumns.Contains("TaxAmount") && objRow["TaxAmount"] != DBNull.Value) TaxAmount = Convert.ToDecimal(objRow["TaxAmount"]);
                //if (objColumns.Contains("TaxRate") && objRow["TaxRate"] != DBNull.Value) TaxRate = Convert.ToDecimal(objRow["TaxRate"]);
                if (objColumns.Contains("DiscountAmount") && objRow["DiscountAmount"] != DBNull.Value) DiscountAmount = Convert.ToDouble(objRow["DiscountAmount"]);
                //if (objColumns.Contains("DiscountRate") && objRow["DiscountRate"] != DBNull.Value) DiscountRate = Convert.ToDecimal(objRow["DiscountRate"]);
                //if (objColumns.Contains("ParentLineExternalID") && objRow["ParentLineExternalID"] != DBNull.Value) ParentLineExternalID = Convert.ToString(objRow["ParentLineExternalID"]);
                //if (objColumns.Contains("ParentSalesOrderLineID") && objRow["ParentSalesOrderLineID"] != DBNull.Value) ParentSalesOrderLineID = Convert.ToString(objRow["ParentSalesOrderLineID"]);
                //if (objColumns.Contains("VendorLineSubTotal") && objRow["VendorLineSubTotal"] != DBNull.Value) VendorLineSubTotal = Convert.ToDecimal(objRow["VendorLineSubTotal"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);

                if (string.IsNullOrEmpty(SalesOrderLineID)) throw new Exception("Missing SalesOrderLineID in the datarow");
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
                if (string.IsNullOrEmpty(SalesOrderID)) throw new Exception("SalesOrderID is required");
                if ((string.IsNullOrEmpty(ItemID) && string.IsNullOrEmpty(ItemInternalID))) throw new Exception("ItemID or ItemInternalID is required");
                if (Quantity <= 0) throw new Exception("Quantity cannot be less than or equal to 0");
                if (!IsNew) throw new Exception("Create cannot be performed, SalesOrderLineID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["SalesOrderID"] = SalesOrderID;
                dicParam["ItemID"] = ItemID;
                dicParam["ItemInternalID"] = ItemInternalID;
                dicParam["Quantity"] = Quantity;
                dicParam["UnitPrice"] = UnitPrice;
                dicParam["DiscountAmount"] = DiscountAmount;
                dicParam["OnlinePrice"] = OnlinePrice;
                dicParam["TariffCharge"] = TariffCharge;
                dicParam["UserInfoID"] = UserInfoID;
                dicParam["CustomListID_1"] = CustomListID_1;
                dicParam["CustomListValueID_1"] = CustomListValueID_1;
                dicParam["CustomListID_2"] = CustomListID_2;
                dicParam["CustomListValueID_2"] = CustomListValueID_2;
                dicParam["CustomDesignImagePath"] = CustomDesignImagePath;
                dicParam["CustomDesignName"] = CustomDesignName;
                dicParam["Embellishment"] = Embellishment;

                dicParam["SKU"] = SKU;
                dicParam["Description"] = Description;

                SalesOrderLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "SalesOrderLine"), objConn, objTran).ToString();

                foreach (SalesOrderLineSelectableLogo _SalesOrderLineSelectableLogo in SalesOrderLineSelectableLogos)
                {
                    _SalesOrderLineSelectableLogo.SalesOrderLineID = SalesOrderLineID;
                    if (_SalesOrderLineSelectableLogo.IsNew)
                    {
                        _SalesOrderLineSelectableLogo.Create(objConn, objTran);
                    }
                    else
                    {
                        _SalesOrderLineSelectableLogo.Update(objConn, objTran);
                    }
                }

                foreach (Item.ItemPersonalizationValue _ItemPersonalizationValue in ItemPersonalizationValues)
                {
                    _ItemPersonalizationValue.SalesOrderLineID = SalesOrderLineID;
                    if (_ItemPersonalizationValue.IsNew)
                    {
                        _ItemPersonalizationValue.Create(objConn, objTran);
                    }
                    else
                    {
                        _ItemPersonalizationValue.Update(objConn, objTran);
                    }
                }

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
                if (string.IsNullOrEmpty(SalesOrderID)) throw new Exception("SalesOrderID is required");
                if ((string.IsNullOrEmpty(ItemID) && string.IsNullOrEmpty(ItemInternalID))) throw new Exception("ItemID or ItemInternalID is required");
                if (Quantity <= 0) throw new Exception("Quantity cannot be less than or equal to 0");
                if (IsNew) throw new Exception("Update cannot be performed, SalesOrderLineID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["ItemID"] = ItemID;
                dicParam["ItemInternalID"] = ItemInternalID;
                dicParam["Quantity"] = Quantity;
                dicParam["UnitPrice"] = UnitPrice;
                dicParam["DiscountAmount"] = DiscountAmount;
                dicParam["OnlinePrice"] = OnlinePrice;
                dicParam["TariffCharge"] = TariffCharge;
                dicParam["UserInfoID"] = UserInfoID;
                dicParam["CustomListID_1"] = CustomListID_1;
                dicParam["CustomListValueID_1"] = CustomListValueID_1;
                dicParam["CustomListID_2"] = CustomListID_2;
                dicParam["CustomListValueID_2"] = CustomListValueID_2;
                dicParam["CustomDesignImagePath"] = CustomDesignImagePath;
                dicParam["CustomDesignName"] = CustomDesignName;
                dicParam["Embellishment"] = Embellishment;

                dicParam["SKU"] = SKU;
                dicParam["Description"] = Description;

                dicWParam["SalesOrderLineID"] = SalesOrderLineID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "SalesOrderLine"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, SalesOrderLinesID is missing");

                dicDParam["SalesOrderLineID"] = SalesOrderLineID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "SalesOrderLine"), objConn, objTran);
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

        public static SalesOrderLine GetSalesOrderLine(SalesOrderLineFilter Filter)
        {
            List<SalesOrderLine> objSalesOrderLines = null;
            SalesOrderLine objReturn = null;

            try
            {
                objSalesOrderLines = GetSalesOrderLines(Filter);
                if (objSalesOrderLines != null && objSalesOrderLines.Count >= 1) objReturn = objSalesOrderLines[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSalesOrderLines = null;
            }
            return objReturn;

        }

        public static List<SalesOrderLine> GetSalesOrderLines()
        {
            int intTotalCount = 0;
            return GetSalesOrderLines(null, null, null, out intTotalCount);
        }

        public static List<SalesOrderLine> GetSalesOrderLines(SalesOrderLineFilter Filter)
        {
            int intTotalCount = 0;
            return GetSalesOrderLines(Filter, null, null, out intTotalCount);
        }

        public static List<SalesOrderLine> GetSalesOrderLines(SalesOrderLineFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetSalesOrderLines(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<SalesOrderLine> GetSalesOrderLines(SalesOrderLineFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<SalesOrderLine> objReturn = null;
            SalesOrderLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<SalesOrderLine>();

                strSQL = string.Format(@"
SELECT sl.* 
FROM SalesOrderLine (NOLOCK) sl 
INNER JOIN SalesOrder (NOLOCK) s ON sl.SalesOrderID=s.SalesOrderID 
WHERE 1=1 ");

                if (Filter != null)
                {
                    //if (!string.IsNullOrEmpty(Filter.SalesOrderID)) strSQL += "AND sl.SalesOrderID=" + Database.HandleQuote(Filter.SalesOrderID);
                    //if (!string.IsNullOrEmpty(Filter.ExternalID)) strSQL += "AND sl.ExternalID=" + Database.HandleQuote(Filter.ExternalID);


                    if (Filter.SalesOrderID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderID, "sl.SalesOrderID");
                    if (Filter.ExternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ExternalID, "sl.ExternalID");
                    if (Filter.ParentLineExternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ParentLineExternalID, "sl.ParentLineExternalID");
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "sl.ItemID");

                    if (Filter.ParentSalesOrderLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ParentSalesOrderLineID, "sl.ParentSalesOrderLineID");
                    if (Filter.SalesOrderStatus != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderStatus, "s.Status");
                    if (Filter.VendorID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.VendorID, "i.VendorID");
                    if (Filter.UserInfoID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserInfoID, "sl.UserInfoID");
                    if (Filter.CustomListID_1 != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomListID_1, "sl.CustomListID_1");
                    if (Filter.CustomListValueID_1 != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomListValueID_1, "sl.CustomListValueID_1");
                    if (Filter.CustomListID_2 != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomListID_2, "sl.CustomListID_2");
                    if (Filter.CustomListValueID_2 != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomListValueID_2, "sl.CustomListValueID_2");
                    if (Filter.CustomDesignImagePath != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomDesignImagePath, "sl.CustomDesignImagePath");
                    if (Filter.CustomDesignName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomDesignName, "sl.CustomDesignName");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "SalesOrderLineID" : Utility.CustomSorting.GetSortExpression(typeof(SalesOrderLine), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new SalesOrderLine(objData.Tables[0].Rows[i]);
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

        //Sales Orders with line item from a Vendor
        public static List<SalesOrderLine> GetSalesOrderLinesByVendorSalesOrder(string VendorID, string SalesOrderID)
        {
            int intTotalCount = 0;
            return GetSalesOrderLinesByVendorSalesOrder(VendorID, SalesOrderID, null, null, out intTotalCount);
        }

        public static List<SalesOrderLine> GetSalesOrderLinesByVendorSalesOrder(string VendorID, string SalesOrderID, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetSalesOrderLinesByVendorSalesOrder(VendorID, SalesOrderID, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<SalesOrderLine> GetSalesOrderLinesByVendorSalesOrder(string VendorID, string SalesOrderID, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<SalesOrderLine> objReturn = null;
            SalesOrderLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<SalesOrderLine>();

                strSQL = string.Format(@"
SELECT sl.SalesOrderLineID, sl.SalesOrderID, sl.ParentSalesOrderLineID, sl.Location, sl.ItemID, sl.UnitPrice
	, sl.Quantity	
	, sl.Quantity * sl.UnitPrice - isnull(sl.DiscountAmount,0) as VendorLineSubTotal
    , sl.DiscountAmount, sl.DiscountRate
FROM SalesOrder (NOLOCK) s
Inner Join SalesOrderLine (NOLOCK) sl on sl.SalesOrderID = s.SalesOrderID
Inner Join Item (NOLOCK) i on i.ItemID = sl.ItemID
WHERE i.VendorID = {0}
and sl.SalesOrderID = {1}
and isnull(sl.ParentSalesOrderLineID,'') = ''
"
                , Database.HandleQuote(VendorID)
                , Database.HandleQuote(SalesOrderID));

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "SalesOrderLineID" : Utility.CustomSorting.GetSortExpression(typeof(SalesOrderLine), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new SalesOrderLine(objData.Tables[0].Rows[i]);
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

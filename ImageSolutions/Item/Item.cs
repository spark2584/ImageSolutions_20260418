using ImageSolutions.User;
using ImageSolutions.Website;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Item
{
    public class Item : ISBase.BaseClass
    {
        public string ItemID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ItemID); } }
        public string ParentID { get; set; }
        public string InternalID { get; set; }
        public string ExternalID { get; set; }
        public string ItemNumber { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public string VendorID { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public string ProductTitle { get; set; }
        public double? PurchasePrice { get; set; }
        public double? PurchasePricePendingUpdate { get; set; }
        public string StoreDisplayName { get; set; }
        public string SalesDescription { get; set; }
        public string DetailedDescription { get; set; }
        public double? BasePrice { get; set; }
        public double? OnlinePrice { get; set; }
        public double? DiscountAmount { get; set; }
        public string StyleID { get; set; }
        public string StyleNumber { get; set; }
        public string SizeCode { get; set; }
        public string SizeIndex { get; set; }
        public string Color { get; set; }
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
        public decimal? ColorNameDistance { get; set; }
        public string MainFrameColor { get; set; }
        public decimal? VendorInventory { get; set; }
        public DateTime? VendorInventoryLastUpdatedOn { get; set; }
        public decimal? VendorInventoryOOSThreshold { get; set; }
        public decimal? UnitWeight { get; set; }
        public string ImageURL { get; set; }
        public string ImageURL2 { get; set; }
        public string ImageURL3 { get; set; }
        public string LogoImageURL { get; set; }
        public string AttributeDisplayType { get; set; }
        public string GroupByAttributeID { get; set; }
        public string ListAttributeID { get; set; }
        public string UniqueKey { get; set; }
        public string InventoryKey { get; set; }
        public int QuantityAvailable { get; set; }
        public int AvailableInventory
        {
            get
            {
                if (ItemType == "_nonInventoryItem")
                {                    
                    return VendorInventory == null ? 0 
                        : Convert.ToInt32(VendorInventory.Value) < OOSThreshold ? 0 : Convert.ToInt32(VendorInventory.Value) - OOSThreshold;
                }
                else
                {
                    return QuantityAvailable;
                }
            }
        }
        public bool IsNonInventory { get; set; }
        public string UnitItemID { get; set; }
        public bool EnablePersonalization { get; set; }
        public bool EnableSelectableLogo { get; set; }
        //public bool IsCompanyInvoiced { get; set; }
        public bool RequireLogoSelection { get; set; }

        public bool HideSizeChart { get; set; }
        public string SizeChartURL { get; set; }
        public bool AllowBackOrder { get; set; }
        public bool UseLengthAndWidth { get; set; }
        public string ItemLength { get; set; }

        public decimal? QuantityPerPerson { get; set; }
        public bool HideDetailedDescription { get; set; }
        public bool ExcludeDisplayUser { get; set; }
        public bool DoNotDisplayNIMessage { get; set; }

        public int OOSThreshold { get; set; }
        public string PackageGroupID { get; set; }

        public bool DefaultSelectableLogo { get; set; }
        public bool IsOnline { get; set; }
        public bool InActive { get; set; }
        public string AvataxTaxCode { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        public string StoreDisplayNameHTML
        {
            get
            {
                return StoreDisplayName.Replace("CLEARANCE", "<span style=\"color: red; font: bold;\">CLEARANCE</span>").Replace("- ALL SALES FINAL", "<span style=\"color: red; font: bold;\">- ALL SALES FINAL</span>".Replace("SALE", "<span style=\"color: red; font: bold;\">SALE</span>"));
            }
        }

        public string PriceRange
        {
            get
            {
                string strReturn = string.Empty;
                List<ImageSolutions.Item.Item> Items = new List<ImageSolutions.Item.Item>();

                ImageSolutions.Item.ItemFilter ItemFilter = new ImageSolutions.Item.ItemFilter();
                ItemFilter = new ImageSolutions.Item.ItemFilter();
                ItemFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                ItemFilter.ParentID.SearchString = ItemID;
                Items = ImageSolutions.Item.Item.GetItems(ItemFilter);

                if (Items != null && Items.Count > 0)
                {
                    double dbMinPrice = Items.Min(x => x.BasePrice != null ? x.BasePrice : 0).Value;
                    double dbMaxPrice = Items.Max(x => x.BasePrice != null ? x.BasePrice : 0).Value;

                    if (dbMinPrice == dbMaxPrice)

                        strReturn = string.Format("{0:c}", dbMinPrice);
                    else
                        strReturn = string.Format("{0}-{1}", string.Format("{0:c}", dbMinPrice), string.Format("{0:c}", dbMaxPrice));
                }
                return strReturn;
            }
        }
        public string MinPrice
        {
            get
            {
                List<ImageSolutions.Item.Item> Items = new List<ImageSolutions.Item.Item>();

                ImageSolutions.Item.ItemFilter ItemFilter = new ImageSolutions.Item.ItemFilter();
                ItemFilter = new ImageSolutions.Item.ItemFilter();
                ItemFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                ItemFilter.ParentID.SearchString = ItemID;
                Items = ImageSolutions.Item.Item.GetItems(ItemFilter);

                return string.Format("{0}", Convert.ToString(Items.Min(x => x.BasePrice != null ? x.BasePrice : 0)));
            }
        }

        public string MaxPrice
        {
            get
            {
                List<ImageSolutions.Item.Item> Items = new List<ImageSolutions.Item.Item>();

                ImageSolutions.Item.ItemFilter ItemFilter = new ImageSolutions.Item.ItemFilter();
                ItemFilter = new ImageSolutions.Item.ItemFilter();
                ItemFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                ItemFilter.ParentID.SearchString = ItemID;
                Items = ImageSolutions.Item.Item.GetItems(ItemFilter);

                return string.Format("{0}", Convert.ToString(Items.Max(x => x.BasePrice != null ? x.BasePrice : 0)));
            }
        }

        public string Description
        {
            get
            {
                return string.Format("{0} ({1})", ItemNumber, StoreDisplayName);
                //return StoreDisplayName + "　(" + ItemNumber + ")";
            }
        }

        private Item mParentItem = null;
        public Item ParentItem
        {
            get
            {
                if (mParentItem == null && !string.IsNullOrEmpty(ParentID))
                {
                    mParentItem = new Item(ParentID);
                }
                return mParentItem;
            }
        }
        private Item mUnitItem = null;
        public Item UnitItem
        {
            get
            {
                if (mUnitItem == null && !string.IsNullOrEmpty(UnitItemID))
                {
                    mUnitItem = new Item(UnitItemID);
                }
                return mUnitItem;
            }
        }
        private Attribute.Attribute mGroupByAttribute = null;
        public Attribute.Attribute GroupByAttribute
        {
            get
            {
                if (mGroupByAttribute == null && !string.IsNullOrEmpty(GroupByAttributeID))
                {
                    mGroupByAttribute = new Attribute.Attribute(GroupByAttributeID);
                }
                return mGroupByAttribute;
            }
        }

        private UserInfo mCreatedByUser = null;
        public UserInfo CreatedByUser
        {
            get
            {
                if (mCreatedByUser == null && !string.IsNullOrEmpty(CreatedBy))
                {
                    mCreatedByUser = new UserInfo(CreatedBy);
                }
                return mCreatedByUser;
            }
        }

        private List<ItemPricing> mItemPricings = null;
        public List<ItemPricing> ItemPricings
        {
            get
            {
                if (mItemPricings == null && !string.IsNullOrEmpty(ItemID))
                {
                    ItemPricingFilter objFilter = null;

                    try
                    {
                        objFilter = new ItemPricingFilter();
                        objFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemID.SearchString = ItemID;
                        mItemPricings = ItemPricing.GetItemPricings(objFilter);
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
                return mItemPricings;
            }
        }

        private List<Attribute.Attribute> mAttributes = null;
        public List<Attribute.Attribute> Attributes
        {
            get
            {
                if (mAttributes == null && !string.IsNullOrEmpty(ItemID))
                {
                    Attribute.AttributeFilter objFilter = null;

                    try
                    {
                        objFilter = new Attribute.AttributeFilter();
                        objFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemID.SearchString = ItemID;
                        mAttributes = Attribute.Attribute.GetAttributes(objFilter);
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
                return mAttributes;
            }
        }

        private List<ItemAttributeValue> mItemAttributeValues = null;
        public List<ItemAttributeValue> ItemAttributeValues
        {
            get
            {
                ItemAttributeValueFilter objFilter = null;
                if (mItemAttributeValues == null && !string.IsNullOrEmpty(ItemID))
                {
                    objFilter = new ItemAttributeValueFilter();
                    objFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                    objFilter.ItemID.SearchString = ItemID;
                    mItemAttributeValues = ItemAttributeValue.GetItemAttributeValues(objFilter);
                }
                return mItemAttributeValues;
            }
        }

        private List<ItemDetail> mItemDetails = null;
        public List<ItemDetail> ItemDetails
        {
            get
            {
                if (mItemDetails == null && !string.IsNullOrEmpty(ItemID))
                {
                    ItemDetailFilter objFilter = null;

                    try
                    {
                        objFilter = new ItemDetailFilter();
                        objFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemID.SearchString = ItemID;
                        mItemDetails = ItemDetail.GetItemDetails(objFilter);
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
                return mItemDetails;
            }
        }
        private List<WebsiteGroupItem> mWebsiteGroupItems = null;
        public List<WebsiteGroupItem> WebsiteGroupItems
        {
            get
            {
                if (mWebsiteGroupItems == null && !string.IsNullOrEmpty(ItemID))
                {
                    WebsiteGroupItemFilter objFilter = null;

                    try
                    {
                        objFilter = new WebsiteGroupItemFilter();
                        objFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemID.SearchString = ItemID;
                        mWebsiteGroupItems = WebsiteGroupItem.GetWebsiteGroupItems(objFilter);
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
                return mWebsiteGroupItems;
            }
        }
        private List<WebsiteGroupItemExclude> mWebsiteGroupItemExcludes = null;
        public List<WebsiteGroupItemExclude> WebsiteGroupItemExcludes
        {
            get
            {
                if (mWebsiteGroupItemExcludes == null && !string.IsNullOrEmpty(ItemID))
                {
                    WebsiteGroupItemExcludeFilter objFilter = null;

                    try
                    {
                        objFilter = new WebsiteGroupItemExcludeFilter();
                        objFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemID.SearchString = ItemID;
                        mWebsiteGroupItemExcludes = WebsiteGroupItemExclude.GetWebsiteGroupItemExcludes(objFilter);
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
                return mWebsiteGroupItemExcludes;
            }
        }
        private List<ItemWebsite> mItemWebsites = null;
        public List<ItemWebsite> ItemWebsites
        {
            get
            {
                ItemWebsiteFilter objFilter = null;
                if (mItemWebsites == null && !string.IsNullOrEmpty(ItemID))
                {
                    objFilter = new ItemWebsiteFilter();
                    objFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                    objFilter.ItemID.SearchString = ItemID;
                    mItemWebsites = ItemWebsite.GetItemWebsites(objFilter);
                }
                return mItemWebsites;
            }
        }
        private List<Item> mVariations = null;
        public List<Item> Variations
        {
            get
            {
                if (mVariations == null && !string.IsNullOrEmpty(ItemID))
                {
                    ItemFilter objFilter = null;

                    try
                    {
                        objFilter = new ItemFilter();
                        objFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ParentID.SearchString = ItemID;
                        mVariations = Item.GetItems(objFilter);
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
                return mVariations;
            }
        }

        private List<SuperceedingItem> mSuperceedingItems = null;
        public List<SuperceedingItem> SuperceedingItems
        {
            get
            {
                if (mSuperceedingItems == null && !string.IsNullOrEmpty(ItemID))
                {
                    SuperceedingItemFilter objFilter = null;

                    try
                    {
                        objFilter = new SuperceedingItemFilter();
                        objFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemID.SearchString = ItemID;
                        //if (ParentItem != null)
                        //    objFilter.ItemID.SearchString = ParentItem.ItemID;
                        //else
                        //    objFilter.ItemID.SearchString = ItemID;
                        mSuperceedingItems = SuperceedingItem.GetSuperceedingItems(objFilter);
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
                return mSuperceedingItems;
            }
        }
        private List<ItemPersonalization> mItemPersonalizations = null;
        public List<ItemPersonalization> ItemPersonalizations
        {
            get
            {
                if (mItemPersonalizations == null && !string.IsNullOrEmpty(ItemID))
                {
                    ItemPersonalizationFilter objFilter = null;

                    try
                    {
                        objFilter = new ItemPersonalizationFilter();
                        objFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemID.SearchString = ItemID;
                        mItemPersonalizations = ItemPersonalization.GetItemPersonalizations(objFilter);
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
                return mItemPersonalizations;
            }
        }

        private List<Website.WebsiteTabItem> mWebsiteTabItems = null;
        public List<Website.WebsiteTabItem> WebsiteTabItems
        {
            get
            {
                if (mWebsiteTabItems == null && !string.IsNullOrEmpty(ItemID))
                {
                    WebsiteTabItemFilter objFilter = null;

                    try
                    {
                        objFilter = new WebsiteTabItemFilter();
                        objFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemID.SearchString = ItemID;
                        mWebsiteTabItems = Website.WebsiteTabItem.GetWebsiteTabItems(objFilter);
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
                return mWebsiteTabItems;
            }
        }
        private List<ItemSelectableLogo> mItemSelectableLogos = null;
        public List<ItemSelectableLogo> ItemSelectableLogos
        {
            get
            {
                if (mItemSelectableLogos == null && !string.IsNullOrEmpty(ItemID))
                {
                    ItemSelectableLogoFilter objFilter = null;

                    try
                    {
                        objFilter = new ItemSelectableLogoFilter();
                        objFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ItemID.SearchString = ItemID;
                        mItemSelectableLogos = ImageSolutions.Item.ItemSelectableLogo.GetItemSelectableLogos(objFilter);
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
                return mItemSelectableLogos;
            }
        }
        public bool IsVariation
        {
            get
            {
                return Attributes != null && Attributes.Count > 0;
            }
        }

        public string DisplayImageURL
        {
            get
            {
                if (!string.IsNullOrEmpty(ImageURL))
                {
                    return ImageURL;
                }
                else if (ParentItem != null)
                {
                    return ParentItem.ImageURL;
                }
                else if (Variations != null)
                {
                    if (Variations.Exists(m => !string.IsNullOrEmpty(m.ImageURL)))
                    {
                        return Variations.Find(m => !string.IsNullOrEmpty(m.ImageURL)).ImageURL;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                return string.Empty;
            }
        }
        public string DisplayLogoImageURL
        {
            get
            {
                if (!string.IsNullOrEmpty(LogoImageURL))
                {
                    return LogoImageURL;
                }
                else if (ParentItem != null)
                {
                    return ParentItem.LogoImageURL;
                }
                else if (Variations != null)
                {
                    if (Variations.Exists(m => !string.IsNullOrEmpty(m.LogoImageURL)))
                    {
                        return Variations.Find(m => !string.IsNullOrEmpty(m.LogoImageURL)).LogoImageURL;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                return string.Empty;
            }
        }
        public Item()
        {
        }

        public Item(string ItemID)
        {
            this.ItemID = ItemID;
            Load();
        }

        public Item(DataRow objRow)
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
                         "FROM Item (NOLOCK) " +
                         "WHERE ItemID=" + Database.HandleQuote(ItemID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ItemID=" + ItemID + " is not found");
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

                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("ParentID")) ParentID = Convert.ToString(objRow["ParentID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
                if (objColumns.Contains("ItemNumber")) ItemNumber = Convert.ToString(objRow["ItemNumber"]);
                if (objColumns.Contains("ItemName")) ItemName = Convert.ToString(objRow["ItemName"]);
                if (objColumns.Contains("ItemType")) ItemType = Convert.ToString(objRow["ItemType"]);
                if (objColumns.Contains("VendorID")) VendorID = Convert.ToString(objRow["VendorID"]);
                if (objColumns.Contains("VendorName")) VendorName = Convert.ToString(objRow["VendorName"]);
                if (objColumns.Contains("VendorCode")) VendorCode = Convert.ToString(objRow["VendorCode"]);
                if (objColumns.Contains("ProductTitle")) ProductTitle = Convert.ToString(objRow["ProductTitle"]);
                if (objColumns.Contains("PurchasePrice") && objRow["PurchasePrice"] != DBNull.Value) PurchasePrice = Convert.ToDouble(objRow["PurchasePrice"]);
                if (objColumns.Contains("PurchasePricePendingUpdate") && objRow["PurchasePricePendingUpdate"] != DBNull.Value) PurchasePricePendingUpdate = Convert.ToDouble(objRow["PurchasePricePendingUpdate"]);
                if (objColumns.Contains("StoreDisplayName")) StoreDisplayName = Convert.ToString(objRow["StoreDisplayName"]);
                if (objColumns.Contains("SalesDescription")) SalesDescription = Convert.ToString(objRow["SalesDescription"]);
                if (objColumns.Contains("DetailedDescription")) DetailedDescription = Convert.ToString(objRow["DetailedDescription"]);
                if (objColumns.Contains("BasePrice") && objRow["BasePrice"] != DBNull.Value) BasePrice = Convert.ToDouble(objRow["BasePrice"]);
                if (objColumns.Contains("OnlinePrice") && objRow["OnlinePrice"] != DBNull.Value) OnlinePrice = Convert.ToDouble(objRow["OnlinePrice"]);
                if (objColumns.Contains("DiscountAmount") && objRow["DiscountAmount"] != DBNull.Value) DiscountAmount = Convert.ToDouble(objRow["DiscountAmount"]);
                if (objColumns.Contains("StyleID")) StyleID = Convert.ToString(objRow["StyleID"]);
                if (objColumns.Contains("StyleNumber")) StyleNumber = Convert.ToString(objRow["StyleNumber"]);
                if (objColumns.Contains("SizeCode")) SizeCode = Convert.ToString(objRow["SizeCode"]);
                if (objColumns.Contains("SizeIndex")) SizeIndex = Convert.ToString(objRow["SizeIndex"]);
                if (objColumns.Contains("Color")) Color = Convert.ToString(objRow["Color"]);
                if (objColumns.Contains("ColorCode")) ColorCode = Convert.ToString(objRow["ColorCode"]);
                if (objColumns.Contains("ColorName")) ColorName = Convert.ToString(objRow["ColorName"]);
                if (objColumns.Contains("ColorNameDistance") && objRow["ColorNameDistance"] != DBNull.Value) ColorNameDistance = Convert.ToDecimal(objRow["ColorNameDistance"]);
                if (objColumns.Contains("MainFrameColor")) MainFrameColor = Convert.ToString(objRow["MainFrameColor"]);
                if (objColumns.Contains("VendorInventory") && objRow["VendorInventory"] != DBNull.Value) VendorInventory = Convert.ToDecimal(objRow["VendorInventory"]);
                if (objColumns.Contains("VendorInventoryLastUpdatedOn") && objRow["VendorInventoryLastUpdatedOn"] != DBNull.Value) VendorInventoryLastUpdatedOn = Convert.ToDateTime(objRow["VendorInventoryLastUpdatedOn"]);
                if (objColumns.Contains("VendorInventoryOOSThreshold") && objRow["VendorInventoryOOSThreshold"] != DBNull.Value) VendorInventoryOOSThreshold = Convert.ToDecimal(objRow["VendorInventoryOOSThreshold"]);
                if (objColumns.Contains("UnitWeight") && objRow["UnitWeight"] != DBNull.Value) UnitWeight = Convert.ToDecimal(objRow["UnitWeight"]);
                if (objColumns.Contains("ImageURL")) ImageURL = Convert.ToString(objRow["ImageURL"]);
                if (objColumns.Contains("ImageURL2")) ImageURL2 = Convert.ToString(objRow["ImageURL2"]);
                if (objColumns.Contains("ImageURL3")) ImageURL3 = Convert.ToString(objRow["ImageURL3"]);
                if (objColumns.Contains("ImageURL")) LogoImageURL = Convert.ToString(objRow["LogoImageURL"]);
                if (objColumns.Contains("AttributeDisplayType")) AttributeDisplayType = Convert.ToString(objRow["AttributeDisplayType"]);
                if (objColumns.Contains("GroupByAttributeID")) GroupByAttributeID = Convert.ToString(objRow["GroupByAttributeID"]);
                if (objColumns.Contains("ListAttributeID")) ListAttributeID = Convert.ToString(objRow["ListAttributeID"]);
                if (objColumns.Contains("UniqueKey")) UniqueKey = Convert.ToString(objRow["UniqueKey"]);
                if (objColumns.Contains("InventoryKey")) InventoryKey = Convert.ToString(objRow["InventoryKey"]);
                if (objColumns.Contains("QuantityAvailable")) QuantityAvailable = Convert.ToInt32(objRow["QuantityAvailable"]);
                if (objColumns.Contains("IsNonInventory")) IsNonInventory = Convert.ToBoolean(objRow["IsNonInventory"]);
                if (objColumns.Contains("UnitItemID")) UnitItemID = Convert.ToString(objRow["UnitItemID"]);
                if (objColumns.Contains("IsOnline") && objRow["IsOnline"] != DBNull.Value) IsOnline = Convert.ToBoolean(objRow["IsOnline"]);
                if (objColumns.Contains("EnablePersonalization") && objRow["EnablePersonalization"] != DBNull.Value) EnablePersonalization = Convert.ToBoolean(objRow["EnablePersonalization"]);
                if (objColumns.Contains("EnableSelectableLogo") && objRow["EnableSelectableLogo"] != DBNull.Value) EnableSelectableLogo = Convert.ToBoolean(objRow["EnableSelectableLogo"]);
                if (objColumns.Contains("HideSizeChart") && objRow["HideSizeChart"] != DBNull.Value) HideSizeChart = Convert.ToBoolean(objRow["HideSizeChart"]);
                if (objColumns.Contains("SizeChartURL")) SizeChartURL = Convert.ToString(objRow["SizeChartURL"]);
                //if (objColumns.Contains("IsCompanyInvoiced") && objRow["IsCompanyInvoiced"] != DBNull.Value) IsCompanyInvoiced = Convert.ToBoolean(objRow["IsCompanyInvoiced"]);
                if (objColumns.Contains("AllowBackOrder") && objRow["AllowBackOrder"] != DBNull.Value) AllowBackOrder = Convert.ToBoolean(objRow["AllowBackOrder"]);
                if (objColumns.Contains("UseLengthAndWidth") && objRow["UseLengthAndWidth"] != DBNull.Value) UseLengthAndWidth = Convert.ToBoolean(objRow["UseLengthAndWidth"]);
                if (objColumns.Contains("ItemLength")) ItemLength = Convert.ToString(objRow["ItemLength"]);

                if (objColumns.Contains("QuantityPerPerson") && objRow["QuantityPerPerson"] != DBNull.Value) QuantityPerPerson = Convert.ToDecimal(objRow["QuantityPerPerson"]);
                if (objColumns.Contains("HideDetailedDescription") && objRow["HideDetailedDescription"] != DBNull.Value) HideDetailedDescription = Convert.ToBoolean(objRow["HideDetailedDescription"]);
                if (objColumns.Contains("ExcludeDisplayUser") && objRow["ExcludeDisplayUser"] != DBNull.Value) ExcludeDisplayUser = Convert.ToBoolean(objRow["ExcludeDisplayUser"]);
                if (objColumns.Contains("RequireLogoSelection") && objRow["RequireLogoSelection"] != DBNull.Value) RequireLogoSelection = Convert.ToBoolean(objRow["RequireLogoSelection"]);
                if (objColumns.Contains("DoNotDisplayNIMessage") && objRow["DoNotDisplayNIMessage"] != DBNull.Value) DoNotDisplayNIMessage = Convert.ToBoolean(objRow["DoNotDisplayNIMessage"]);

                if (objColumns.Contains("OOSThreshold") && objRow["OOSThreshold"] != DBNull.Value) OOSThreshold = Convert.ToInt32(objRow["OOSThreshold"]);
                if (objColumns.Contains("PackageGroupID")) PackageGroupID = Convert.ToString(objRow["PackageGroupID"]);

                if (objColumns.Contains("DefaultSelectableLogo") && objRow["DefaultSelectableLogo"] != DBNull.Value) DefaultSelectableLogo = Convert.ToBoolean(objRow["DefaultSelectableLogo"]);

                if (objColumns.Contains("InActive") && objRow["InActive"] != DBNull.Value) InActive = Convert.ToBoolean(objRow["InActive"]);
                if (objColumns.Contains("AvataxTaxCode")) AvataxTaxCode = Convert.ToString(objRow["AvataxTaxCode"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ItemID)) throw new Exception("Missing ItemID in the datarow");
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
                if (string.IsNullOrEmpty(ItemNumber)) throw new Exception("ItemNumber is required");
                if (!IsNew) throw new Exception("Create cannot be performed, ItemID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["ParentID"] = ParentID;
                dicParam["InternalID"] = InternalID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["ItemNumber"] = ItemNumber;
                dicParam["ItemName"] = ItemName;
                dicParam["ItemType"] = ItemType;
                dicParam["VendorID"] = VendorID;
                dicParam["VendorCode"] = VendorCode;
                dicParam["VendorName"] = VendorName;
                dicParam["ProductTitle"] = ProductTitle;
                dicParam["PurchasePrice"] = PurchasePrice;
                dicParam["PurchasePricePendingUpdate"] = PurchasePricePendingUpdate;
                dicParam["StoreDisplayName"] = StoreDisplayName;
                dicParam["SalesDescription"] = SalesDescription;
                dicParam["DetailedDescription"] = DetailedDescription;
                dicParam["BasePrice"] = BasePrice;
                dicParam["OnlinePrice"] = OnlinePrice;
                dicParam["DiscountAmount"] = DiscountAmount;
                dicParam["StyleID"] = StyleID;
                dicParam["StyleNumber"] = StyleNumber;
                dicParam["SizeCode"] = SizeCode;
                dicParam["SizeIndex"] = SizeIndex;
                dicParam["Color"] = Color;
                dicParam["ColorCode"] = ColorCode;
                dicParam["ColorName"] = ColorName;
                dicParam["ColorNameDistance"] = ColorNameDistance;
                dicParam["MainFrameColor"] = MainFrameColor;
                dicParam["VendorInventory"] = VendorInventory;
                dicParam["VendorInventoryLastUpdatedOn"] = VendorInventoryLastUpdatedOn;
                dicParam["VendorInventoryOOSThreshold"] = VendorInventoryOOSThreshold;
                dicParam["UnitWeight"] = UnitWeight;
                dicParam["ImageURL"] = ImageURL;
                dicParam["ImageURL2"] = ImageURL2;
                dicParam["ImageURL3"] = ImageURL3;
                dicParam["LogoImageURL"] = LogoImageURL;
                dicParam["AttributeDisplayType"] = AttributeDisplayType;
                dicParam["GroupByAttributeID"] = GroupByAttributeID;
                dicParam["ListAttributeID"] = ListAttributeID;
                dicParam["UniqueKey"] = UniqueKey;
                dicParam["InventoryKey"] = InventoryKey;
                dicParam["QuantityAvailable"] = QuantityAvailable;
                dicParam["IsNonInventory"] = IsNonInventory;
                dicParam["UnitItemID"] = UnitItemID;
                dicParam["EnablePersonalization"] = EnablePersonalization;
                dicParam["EnableSelectableLogo"] = EnableSelectableLogo;
                dicParam["HideSizeChart"] = HideSizeChart;
                dicParam["SizeChartURL"] = SizeChartURL;
                //dicParam["IsCompanyInvoiced"] = IsCompanyInvoiced;
                dicParam["AllowBackOrder"] = AllowBackOrder;
                dicParam["UseLengthAndWidth"] = UseLengthAndWidth;
                dicParam["ItemLength"] = ItemLength;

                dicParam["QuantityPerPerson"] = QuantityPerPerson;
                dicParam["HideDetailedDescription"] = HideDetailedDescription;
                dicParam["ExcludeDisplayUser"] = ExcludeDisplayUser;
                dicParam["RequireLogoSelection"] = RequireLogoSelection;
                dicParam["DoNotDisplayNIMessage"] = DoNotDisplayNIMessage;

                dicParam["OOSThreshold"] = OOSThreshold;
                dicParam["PackageGroupID"] = PackageGroupID;

                dicParam["DefaultSelectableLogo"] = DefaultSelectableLogo;

                dicParam["IsOnline"] = IsOnline;
                dicParam["InActive"] = InActive;
                dicParam["AvataxTaxCode"] = AvataxTaxCode;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["CreatedBy"] = CreatedBy;
                ItemID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Item"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(ItemNumber)) throw new Exception("ItemNumber is required");
                if (IsNew) throw new Exception("Update cannot be performed, ItemID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["ParentID"] = ParentID;
                dicParam["InternalID"] = InternalID;
                dicParam["ExternalID"] = ExternalID;
                dicParam["ItemNumber"] = ItemNumber;
                dicParam["ItemName"] = ItemName;
                dicParam["ItemType"] = ItemType;
                dicParam["VendorID"] = VendorID;
                dicParam["VendorCode"] = VendorCode;
                dicParam["VendorName"] = VendorName;
                dicParam["ProductTitle"] = ProductTitle;
                dicParam["PurchasePrice"] = PurchasePrice;
                dicParam["PurchasePricePendingUpdate"] = PurchasePricePendingUpdate;
                dicParam["StoreDisplayName"] = StoreDisplayName;
                dicParam["SalesDescription"] = SalesDescription;
                dicParam["DetailedDescription"] = DetailedDescription;
                dicParam["BasePrice"] = BasePrice;
                dicParam["OnlinePrice"] = OnlinePrice;
                dicParam["DiscountAmount"] = DiscountAmount;
                dicParam["StyleID"] = StyleID;
                dicParam["StyleNumber"] = StyleNumber;
                dicParam["SizeCode"] = SizeCode;
                dicParam["SizeIndex"] = SizeIndex;
                dicParam["Color"] = Color;
                dicParam["ColorCode"] = ColorCode;
                dicParam["ColorName"] = ColorName;
                dicParam["ColorNameDistance"] = ColorNameDistance;
                dicParam["MainFrameColor"] = MainFrameColor;
                dicParam["VendorInventory"] = VendorInventory;
                dicParam["VendorInventoryLastUpdatedOn"] = VendorInventoryLastUpdatedOn;
                dicParam["VendorInventoryOOSThreshold"] = VendorInventoryOOSThreshold;
                dicParam["UnitWeight"] = UnitWeight;
                dicParam["ImageURL"] = ImageURL;
                dicParam["ImageURL2"] = ImageURL2;
                dicParam["ImageURL3"] = ImageURL3;
                dicParam["LogoImageURL"] = LogoImageURL;
                dicParam["AttributeDisplayType"] = AttributeDisplayType;
                dicParam["GroupByAttributeID"] = GroupByAttributeID;
                dicParam["ListAttributeID"] = ListAttributeID;
                dicParam["UniqueKey"] = UniqueKey;
                dicParam["InventoryKey"] = InventoryKey;
                dicParam["QuantityAvailable"] = QuantityAvailable;
                dicParam["IsNonInventory"] = IsNonInventory;
                dicParam["UnitItemID"] = UnitItemID;
                dicParam["EnablePersonalization"] = EnablePersonalization;
                dicParam["EnableSelectableLogo"] = EnableSelectableLogo;
                dicParam["HideSizeChart"] = HideSizeChart;
                dicParam["SizeChartURL"] = SizeChartURL;
                //dicParam["IsCompanyInvoiced"] = IsCompanyInvoiced;
                dicParam["AllowBackOrder"] = AllowBackOrder;
                dicParam["UseLengthAndWidth"] = UseLengthAndWidth;
                dicParam["ItemLength"] = ItemLength;

                dicParam["QuantityPerPerson"] = QuantityPerPerson;
                dicParam["HideDetailedDescription"] = HideDetailedDescription;
                dicParam["ExcludeDisplayUser"] = ExcludeDisplayUser;
                dicParam["RequireLogoSelection"] = RequireLogoSelection;
                dicParam["DoNotDisplayNIMessage"] = DoNotDisplayNIMessage;

                dicParam["OOSThreshold"] = OOSThreshold;
                dicParam["PackageGroupID"] = PackageGroupID;

                dicParam["DefaultSelectableLogo"] = DefaultSelectableLogo;

                dicParam["IsOnline"] = IsOnline;
                dicParam["InActive"] = InActive;
                dicParam["AvataxTaxCode"] = AvataxTaxCode;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["ItemID"] = ItemID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Item"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, ItemID is missing");

                SalesOrder.SalesOrderLine SalesOrderLine = new SalesOrder.SalesOrderLine();
                SalesOrder.SalesOrderLineFilter SalesOrderLineFilter = new SalesOrder.SalesOrderLineFilter();
                SalesOrderLineFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                SalesOrderLineFilter.ItemID.SearchString = ItemID;
                SalesOrderLine = SalesOrder.SalesOrderLine.GetSalesOrderLine(SalesOrderLineFilter);
                if (SalesOrderLine != null && !string.IsNullOrEmpty(SalesOrderLine.SalesOrderLineID)) throw new Exception("Delete cannot be performed, transaction exists");

                //PurchaseOrder.PurchaseOrderLine PurchaseOrderLine= new PurchaseOrder.PurchaseOrderLine();
                //PurchaseOrder.PurchaseOrderLineFilter PurchaseOrderLineFilter = new PurchaseOrder.PurchaseOrderLineFilter();
                //PurchaseOrderLineFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                //PurchaseOrderLineFilter.ItemID.SearchString = ItemID;
                //PurchaseOrderLine = PurchaseOrder.PurchaseOrderLine.GetPurchaseOrderLine(PurchaseOrderLineFilter);
                //if (PurchaseOrderLine != null && !string.IsNullOrEmpty(PurchaseOrderLine.PurchaseOrderLineID)) throw new Exception("Delete cannot be performed, transaction exists");

                List<ImageSolutions.ShoppingCart.ShoppingCartLine> ShoppingCartLine = new List<ShoppingCart.ShoppingCartLine>();
                ImageSolutions.ShoppingCart.ShoppingCartLineFilter ShoppingCartLineFilter = new ShoppingCart.ShoppingCartLineFilter();
                ShoppingCartLineFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                ShoppingCartLineFilter.ItemID.SearchString = ItemID;
                ShoppingCartLine = ImageSolutions.ShoppingCart.ShoppingCartLine.GetShoppingCartLines(ShoppingCartLineFilter);
                foreach (ShoppingCart.ShoppingCartLine _ShoppingCartLine in ShoppingCartLine)
                {
                    _ShoppingCartLine.Delete(objConn, objTran);
                }

                foreach (ItemWebsite _ItemWebsite in ItemWebsites)
                {
                    _ItemWebsite.Delete(objConn, objTran);
                }
                foreach (ItemAttributeValue _ItemAttributeValue in ItemAttributeValues)
                {
                    _ItemAttributeValue.Delete(objConn, objTran);
                }
                foreach (ImageSolutions.Attribute.Attribute _Attribute in Attributes)
                {
                    _Attribute.Delete(objConn, objTran);
                }
                foreach (Item _Item in Variations)
                {
                    _Item.Delete(objConn, objTran);
                }

                dicDParam["ItemID"] = ItemID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Item"), objConn, objTran);
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
                     "FROM Item (NOLOCK) p " +
                     "WHERE p.ItemNumber=" + Database.HandleQuote(ItemNumber) +
                     string.Format("AND ISNULL(p.ParentiD,'') = ISNULL({0},'') ", Database.HandleQuote(ParentID));

            if (!string.IsNullOrEmpty(ItemID)) strSQL += "AND p.ItemID<>" + Database.HandleQuote(ItemID);
            return Database.HasRows(strSQL);
        }

        public static Item GetItem(ItemFilter Filter)
        {
            List<Item> objItems = null;
            Item objReturn = null;

            try
            {
                objItems = GetItems(Filter);
                if (objItems != null && objItems.Count >= 1) objReturn = objItems[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItems = null;
            }
            return objReturn;
        }

        public static List<Item> GetItems()
        {
            int intTotalCount = 0;
            return GetItems(null, null, null, out intTotalCount);
        }

        public static List<Item> GetItems(ItemFilter Filter)
        {
            int intTotalCount = 0;
            return GetItems(Filter, null, null, out intTotalCount);
        }

        public static List<Item> GetItems(ItemFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetItems(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Item> GetItems(ItemFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Item> objReturn = null;
            Item objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Item>();

                strSQL = "SELECT * " +
                         "FROM Item (NOLOCK) " +
                         "WHERE 1=1  ";

                if (Filter != null)
                {
                    if (Filter.ParentID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ParentID, "ParentID");
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.ExternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ExternalID, "ExternalID");
                    if (Filter.StyleID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.StyleID, "StyleID");
                    if (Filter.SizeCode != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SizeCode, "SizeCode");
                    if (Filter.Color != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Color, "Color");
                    if (Filter.ItemNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemNumber, "ItemNumber");
                    if (Filter.IsNonInventory != null) strSQL += "AND IsNonInventory=" + Database.HandleQuote(Convert.ToInt32(Filter.IsNonInventory.Value).ToString());
                    if (Filter.VendorName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.VendorName, "VendorName");
                    if (Filter.UpdatedOn != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.UpdatedOn, "UpdatedOn");
                    if (Filter.InActive != null) strSQL += "AND InActive=" + Database.HandleQuote(Convert.ToInt32(Filter.InActive.Value).ToString());
                    if (Filter.IsOnline != null) strSQL += "AND IsOnline=" + Database.HandleQuote(Convert.ToInt32(Filter.IsOnline.Value).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ItemID" : Utility.CustomSorting.GetSortExpression(typeof(Item), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Item(objData.Tables[0].Rows[i]);
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

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary;
using NetSuiteLibrary.com.netsuite.webservices;

namespace ScheduledTask.Task
{
    public class SyncItem
    {
        public bool ImportNSItem(NetSuiteLibrary.Item.Item item)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;
            ImageSolutions.Item.Item objItem = null;
            ImageSolutions.Item.ItemFilter objFilter = null;
            bool blnIsSystemBarcode = false;
            bool blnIsSystemBarcodeFound = false;
            ImageSolutions.Item.Item objItemKitItem = null;

            try
            {
                System.Console.WriteLine(item.NetSuiteInventoryItem.itemId);

                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                objFilter = new ImageSolutions.Item.ItemFilter();
                objFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.InternalID.SearchString = item.NetSuiteInventoryItem.internalId;
                objItem = ImageSolutions.Item.Item.GetItem(objFilter);
                
                if (objItem == null)
                {
                    if (item.NetSuiteInventoryItem.isInactive) return true; //do not create if item is never created and it's inactive in NS

                    objItem = new ImageSolutions.Item.Item();
                }
                else
                {
                    if (item.NetSuiteInventoryItem.isInactive) objItem.InActive = true;
                }

                objItem.InternalID = item.NetSuiteInventoryItem.internalId;
                objItem.ItemType = "_inventoryItem";
                objItem.ItemNumber = item.NetSuiteInventoryItem.itemId;
                objItem.ItemName = item.NetSuiteInventoryItem.displayName;
                //objItem.VendorID = item.NetSuiteNonInventoryResaleItem.vendor.internalId;
                if (item.NetSuiteInventoryItem.vendor != null)
                    objItem.VendorName = item.NetSuiteInventoryItem.vendor.name;

                objItem.VendorCode = item.NetSuiteInventoryItem.vendorName;

                objItem.PurchasePrice = Convert.ToDouble(item.NetSuiteInventoryItem.cost);
                //objItem.BasePrice = Convert.ToDouble(item.NetSuiteNonInventoryResaleItem.pri);
                objItem.StyleNumber = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteInventoryItem, "custitem_sm_style_number");
                objItem.SizeCode = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteInventoryItem, "custitemsizecode");
                objItem.SizeIndex = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteInventoryItem, "custitem_sm_size_index");
                objItem.Color = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteInventoryItem, "custitemqbcolor");
                objItem.ColorCode = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteInventoryItem, "custitem_qb_color_code");
                objItem.ColorName = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteInventoryItem, "custitem_sm_color_name");
                objItem.ColorNameDistance = NetSuiteHelper.GetLongCustomFieldValue(item.NetSuiteInventoryItem, "custitem_kotn_vendor_match_distance");
                objItem.MainFrameColor = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteInventoryItem, "custitem_sm_mainframe_color");
                objItem.VendorInventory = NetSuiteHelper.GetLongCustomFieldValue(item.NetSuiteInventoryItem, "custitem_is_vendor_inventory");
                objItem.VendorInventoryLastUpdatedOn = NetSuiteHelper.GetDateCustomFieldValue(item.NetSuiteInventoryItem, "custitem_is_ven_inv_last_update");
                objItem.VendorInventoryOOSThreshold = NetSuiteHelper.GetLongCustomFieldValue(item.NetSuiteInventoryItem, "custitem_is_ven_inv_oos_threshold");
                objItem.UniqueKey = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteInventoryItem, "custitem_sm_unique_key");
                objItem.InventoryKey = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteInventoryItem, "custitem_sm_inventory_key");

                objItem.StoreDisplayName = item.NetSuiteInventoryItem.storeDisplayName;
                objItem.SalesDescription = item.NetSuiteInventoryItem.salesDescription;

                if(item.NetSuiteInventoryItem.pricingMatrix != null)
                {
                    foreach(Pricing _pricing in item.NetSuiteInventoryItem.pricingMatrix.pricing)
                    {
                        if(_pricing.priceLevel.name == "Base Price")
                        {
                            foreach(Price _price in _pricing.priceList)
                            {
                                if(_price.quantity == 0)
                                {
                                    objItem.BasePrice = Convert.ToDouble(item.NetSuiteInventoryItem.pricingMatrix.pricing[0].priceList[0].value);
                                }
                            }
                        }
                    }
                }

                objItem.IsOnline = item.NetSuiteInventoryItem.isOnline;

                objItem.InActive = item.NetSuiteInventoryItem.isInactive;
                //objItem.LastModifiedOn = item.NetSuiteInventoryItem.lastModifiedDate;
                //objItem.IsUpdated = true;

                objItem.ErrorMessage = string.Empty;
                if (objItem.IsNew)
                    objItem.Create(objConn, objTran);
                else
                    objItem.Update(objConn, objTran);

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

        public bool ImportNSNonInventoryResaleItem(NetSuiteLibrary.Item.Item item)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;
            ImageSolutions.Item.Item objItem = null;
            ImageSolutions.Item.ItemFilter objFilter = null;
            bool blnIsSystemBarcode = false;
            bool blnIsSystemBarcodeFound = false;
            ImageSolutions.Item.Item objItemKitItem = null;
            try
            {
                System.Console.WriteLine(item.NetSuiteNonInventoryResaleItem.itemId);

                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                objFilter = new ImageSolutions.Item.ItemFilter();
                objFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.InternalID.SearchString = item.NetSuiteNonInventoryResaleItem.internalId;
                objItem = ImageSolutions.Item.Item.GetItem(objFilter);

                if (objItem == null)
                {
                    if (item.NetSuiteNonInventoryResaleItem.isInactive) return true; //do not create if item is never created and it's inactive in NS

                    objItem = new ImageSolutions.Item.Item();
                }
                else
                {
                    if (item.NetSuiteNonInventoryResaleItem.isInactive) objItem.InActive = true;
                }

                objItem.InternalID = item.NetSuiteNonInventoryResaleItem.internalId;
                objItem.ExternalID = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryResaleItem, "custitem_is_vendor_part_id");
                objItem.ItemType = "_nonInventoryResaleItem";
                objItem.ItemNumber = item.NetSuiteNonInventoryResaleItem.itemId;
                objItem.ItemName = item.NetSuiteNonInventoryResaleItem.displayName;
                //objItem.VendorID = item.NetSuiteNonInventoryResaleItem.vendor.internalId;

                if (item.NetSuiteNonInventoryResaleItem.vendor != null)
                    objItem.VendorName = item.NetSuiteNonInventoryResaleItem.vendor.name;

                objItem.VendorCode = item.NetSuiteNonInventoryResaleItem.vendorName;

                objItem.PurchasePrice = Convert.ToDouble(item.NetSuiteNonInventoryResaleItem.cost);
                //objItem.BasePrice = Convert.ToDouble(item.NetSuiteNonInventoryResaleItem.pri);
                objItem.StyleNumber = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryResaleItem, "custitem_sm_style_number");
                objItem.SizeCode = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryResaleItem, "custitemsizecode");
                objItem.SizeIndex = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryResaleItem, "custitem_sm_size_index");
                objItem.Color = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryResaleItem, "custitemqbcolor");
                objItem.ColorCode = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryResaleItem, "custitem_qb_color_code");
                objItem.ColorName = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryResaleItem, "custitem_sm_color_name");
                objItem.ColorNameDistance = NetSuiteHelper.GetLongCustomFieldValue(item.NetSuiteNonInventoryResaleItem, "custitem_kotn_vendor_match_distance");
                objItem.MainFrameColor = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryResaleItem, "custitem_sm_mainframe_color");
                objItem.VendorInventory = NetSuiteHelper.GetLongCustomFieldValue(item.NetSuiteNonInventoryResaleItem, "custitem_is_vendor_inventory");
                objItem.VendorInventoryLastUpdatedOn = NetSuiteHelper.GetDateCustomFieldValue(item.NetSuiteNonInventoryResaleItem, "custitem_is_ven_inv_last_update");
                objItem.VendorInventoryOOSThreshold = NetSuiteHelper.GetLongCustomFieldValue(item.NetSuiteNonInventoryResaleItem, "custitem_is_ven_inv_oos_threshold");
                objItem.UniqueKey = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryResaleItem, "custitem_sm_unique_key");
                objItem.InventoryKey = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryResaleItem, "custitem_sm_inventory_key");

                objItem.StoreDisplayName = item.NetSuiteNonInventoryResaleItem.storeDisplayName;
                objItem.SalesDescription = item.NetSuiteNonInventoryResaleItem.salesDescription;

                if (item.NetSuiteNonInventoryResaleItem.pricingMatrix != null)
                {
                    foreach (Pricing _pricing in item.NetSuiteNonInventoryResaleItem.pricingMatrix.pricing)
                    {
                        if (_pricing.priceLevel.name == "Base Price")
                        {
                            foreach (Price _price in _pricing.priceList)
                            {
                                if (_price.quantity == 0)
                                {
                                    objItem.BasePrice = Convert.ToDouble(item.NetSuiteNonInventoryResaleItem.pricingMatrix.pricing[0].priceList[0].value);
                                }
                            }
                        }
                    }
                }

                objItem.IsOnline = item.NetSuiteNonInventoryResaleItem.isOnline;

                objItem.InActive = item.NetSuiteNonInventoryResaleItem.isInactive;
                //objItem.LastModifiedOn = item.NetSuiteInventoryItem.lastModifiedDate;
                //objItem.IsUpdated = true;
                objItem.ErrorMessage = string.Empty;
                if (objItem.IsNew)
                    objItem.Create(objConn, objTran);
                else
                    objItem.Update(objConn, objTran);

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

        public bool ImportNSNonInventoryItem(NetSuiteLibrary.Item.Item item)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;
            ImageSolutions.Item.Item objItem = null;
            ImageSolutions.Item.ItemFilter objFilter = null;
            bool blnIsSystemBarcode = false;
            bool blnIsSystemBarcodeFound = false;
            ImageSolutions.Item.Item objItemKitItem = null;
            try
            {
                System.Console.WriteLine(item.NetSuiteNonInventoryItem.itemId);

                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                objFilter = new ImageSolutions.Item.ItemFilter();
                objFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.InternalID.SearchString = item.NetSuiteNonInventoryItem.internalId;
                objItem = ImageSolutions.Item.Item.GetItem(objFilter);

                if (objItem == null)
                {
                    if (item.NetSuiteNonInventoryItem.isInactive) return true; //do not create if item is never created and it's inactive in NS

                    objItem = new ImageSolutions.Item.Item();
                }
                else
                {
                    if (item.NetSuiteNonInventoryItem.isInactive) objItem.InActive = true;
                }

                objItem.InternalID = item.NetSuiteNonInventoryItem.internalId;
                objItem.ExternalID = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryItem, "custitem_is_vendor_part_id");
                objItem.ItemType = "_nonInventoryItem";
                objItem.ItemNumber = item.NetSuiteNonInventoryItem.itemId;
                objItem.ItemName = item.NetSuiteNonInventoryItem.displayName;
                //objItem.VendorID = item.NetSuiteNonInventoryItem.vendor.internalId;

                //objItem.BasePrice = Convert.ToDouble(item.NetSuiteNonInventoryItem.pri);
                objItem.StyleNumber = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryItem, "custitem_sm_style_number");
                objItem.SizeCode = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryItem, "custitemsizecode");
                objItem.SizeIndex = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryItem, "custitem_sm_size_index");
                objItem.Color = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryItem, "custitemqbcolor");
                objItem.ColorCode = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryItem, "custitem_qb_color_code");
                objItem.ColorName = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryItem, "custitem_sm_color_name");
                objItem.ColorNameDistance = NetSuiteHelper.GetLongCustomFieldValue(item.NetSuiteNonInventoryItem, "custitem_kotn_vendor_match_distance");
                objItem.MainFrameColor = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryItem, "custitem_sm_mainframe_color");
                objItem.VendorInventory = NetSuiteHelper.GetLongCustomFieldValue(item.NetSuiteNonInventoryItem, "custitem_is_vendor_inventory");
                objItem.VendorInventoryLastUpdatedOn = NetSuiteHelper.GetDateCustomFieldValue(item.NetSuiteNonInventoryItem, "custitem_is_ven_inv_last_update");
                objItem.VendorInventoryOOSThreshold = NetSuiteHelper.GetLongCustomFieldValue(item.NetSuiteNonInventoryItem, "custitem_is_ven_inv_oos_threshold");
                objItem.UniqueKey = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryItem, "custitem_sm_unique_key");
                objItem.InventoryKey = NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteNonInventoryItem, "custitem_sm_inventory_key");

                objItem.StoreDisplayName = item.NetSuiteNonInventoryItem.storeDisplayName;
                objItem.SalesDescription = item.NetSuiteNonInventoryItem.salesDescription;

                if (item.NetSuiteNonInventoryItem.pricingMatrix != null)
                {
                    foreach (Pricing _pricing in item.NetSuiteNonInventoryItem.pricingMatrix.pricing)
                    {
                        if (_pricing.priceLevel.name == "Base Price")
                        {
                            foreach (Price _price in _pricing.priceList)
                            {
                                if (_price.quantity == 0)
                                {
                                    objItem.BasePrice = Convert.ToDouble(item.NetSuiteNonInventoryItem.pricingMatrix.pricing[0].priceList[0].value);
                                }
                            }
                        }
                    }
                }

                objItem.IsOnline = item.NetSuiteNonInventoryItem.isOnline;

                objItem.InActive = item.NetSuiteNonInventoryItem.isInactive;
                //objItem.LastModifiedOn = item.NetSuiteInventoryItem.lastModifiedDate;
                //objItem.IsUpdated = true;
                objItem.ErrorMessage = string.Empty;
                if (objItem.IsNew)
                    objItem.Create(objConn, objTran);
                else
                    objItem.Update(objConn, objTran);

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

        public bool ImportStoreCategory(NetSuiteLibrary.Item.Item item)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;
            ImageSolutions.Item.Item objItem = null;
            ImageSolutions.Item.ItemFilter objFilter = null;
            ImageSolutions.Item.ItemWebsite objItemWebsite = null;
            ImageSolutions.Item.ItemWebsiteFilter objItemWebsiteFilter = null;
            ImageSolutions.Website.Website objWebsite = null;
            ImageSolutions.Website.WebsiteFilter objWebsiteFilter = null;

            bool blnIsSystemBarcode = false;
            bool blnIsSystemBarcodeFound = false;
            ImageSolutions.Item.Item objItemKitItem = null;
            string strInternalID = string.Empty;
            SiteCategoryList objSiteCategoryList = null;

            try
            {

                if(item.NetSuiteInventoryItem != null)
                {
                    strInternalID = item.NetSuiteInventoryItem.internalId;
                    objSiteCategoryList = item.NetSuiteInventoryItem.siteCategoryList;
                }
                else if(item.NetSuiteNonInventoryItem != null)
                {
                    strInternalID = item.NetSuiteNonInventoryItem.internalId;
                    objSiteCategoryList = item.NetSuiteNonInventoryItem.siteCategoryList;

                }
                else if(item.NetSuiteNonInventoryResaleItem != null)
                {
                    strInternalID = item.NetSuiteNonInventoryResaleItem.internalId;
                    objSiteCategoryList = item.NetSuiteNonInventoryResaleItem.siteCategoryList;
                }

                System.Console.WriteLine(string.Format("{0} : InternalID Importing ItemWebsite", strInternalID));


                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                objFilter = new ImageSolutions.Item.ItemFilter();
                objFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.InternalID.SearchString = strInternalID;
                objItem = ImageSolutions.Item.Item.GetItem(objFilter);

                if (objItem != null && objSiteCategoryList != null)
                {
                    foreach (SiteCategory1 _sitecategory in objSiteCategoryList.siteCategory)
                    {
                        objWebsiteFilter = new ImageSolutions.Website.WebsiteFilter();
                        objWebsiteFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                        objWebsiteFilter.InternalID.SearchString = _sitecategory.website.internalId;
                        objWebsite = ImageSolutions.Website.Website.GetWebsite(objWebsiteFilter);

                        if (objWebsite != null)
                        {
                            objItemWebsiteFilter = new ImageSolutions.Item.ItemWebsiteFilter();
                            objItemWebsiteFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                            objItemWebsiteFilter.ItemID.SearchString = objItem.ItemID;
                            objItemWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            objItemWebsiteFilter.WebsiteID.SearchString = objWebsite.WebsiteID;
                            objItemWebsite = ImageSolutions.Item.ItemWebsite.GetItemWebsite(objItemWebsiteFilter);

                            if (objItemWebsite == null)
                            {
                                objItemWebsite = new ImageSolutions.Item.ItemWebsite();
                                objItemWebsite.ItemID = objItem.ItemID;
                                objItemWebsite.WebsiteID = objWebsite.WebsiteID;
                                objItemWebsite.Create(objConn, objTran);
                            }
                            else
                            {
                                objItemWebsite.ItemID = objItem.ItemID;
                                objItemWebsite.WebsiteID = objWebsite.WebsiteID;
                                objItemWebsite.Update(objConn, objTran);
                            }

                        }
                    }
                }

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
        //public bool ImportNSKitItem(NetSuiteLibrary.Item.Item item)
        //{
        //    SqlConnection objConn = null;
        //    SqlTransaction objTran = null;
        //    ImageSolutions.Item.Item objItem = null;
        //    ImageSolutions.Item.ItemFilter objFilter = null;
        //    bool blnIsSystemBarcode = false;
        //    bool blnIsSystemBarcodeFound = false;
        //    ImageSolutions.Item.ItemKit objItemKit = null;
        //    ImageSolutions.Item.Item objItemKitItem = null;

        //    try
        //    {
        //        System.Console.WriteLine(item.NetSuiteKitItem.itemId);

        //        objConn = new SqlConnection(Database.DefaultConnectionString);
        //        objConn.Open();
        //        objTran = objConn.BeginTransaction();

        //        objFilter = new Item.ItemFilter();
        //        objFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
        //        objFilter.InternalID.SearchString = item.NetSuiteKitItem.internalId;
        //        objItem = ImageSolutions.Item.Item.GetItem(objFilter);
        //        if (objItem == null)
        //        {
        //            if (item.NetSuiteKitItem.isInactive) return true; //do not create if item is never created and it's inactive in NS

        //            objItem = new Item.Item();
        //            objItem.BusinessID = serviceaccount.BusinessID;
        //        }
        //        else
        //        {
        //            if (item.NetSuiteKitItem.isInactive) objItem.InActive = true;
        //        }

        //        objItem.BusinessID = serviceaccount.BusinessID;
        //        objItem.InternalID = item.NetSuiteKitItem.internalId;
        //        objItem.ItemType = "_kitItem";

        //        string strItemName = NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_fulfillment_display_name");
        //        if (string.IsNullOrEmpty(strItemName))
        //        {
        //            objItem.ItemName = item.NetSuiteKitItem.itemId;
        //        }
        //        else
        //        {
        //            objItem.ItemName = strItemName;
        //        }

        //        objItem.FulfillByKit = NetSuite.NetSuiteHelper.GetBoolCustomFieldValue(item.NetSuiteKitItem, "custitem_fulfill_by_kit");
        //        objItem.ReceiveByKit = NetSuite.NetSuiteHelper.GetBoolCustomFieldValue(item.NetSuiteKitItem, "custitem_receive_by_kit");


        //        if (objItem.ItemBarcodes == null) objItem.ItemBarcodes = new List<Item.ItemBarcode>();

        //        foreach (ImageSolutions.Item.ItemBarcode objItemBarcode in objItem.ItemBarcodes)
        //        {
        //            objItemBarcode.IsSystemBarcode = false;

        //            if (objItemBarcode.BarcodeType == "Vendor Code")
        //            {
        //                objItemBarcode.Delete(objConn, objTran);
        //            }
        //        }

        //        blnIsSystemBarcode = false;
        //        if (!blnIsSystemBarcodeFound)
        //        {
        //            blnIsSystemBarcode = !string.IsNullOrEmpty(NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_hawthorne_code"));
        //            blnIsSystemBarcodeFound = blnIsSystemBarcode;
        //        }
        //        SetItemBarcode(objItem, "Hawthorne Code", NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_hawthorne_code"), blnIsSystemBarcode);

        //        blnIsSystemBarcode = false;
        //        if (!blnIsSystemBarcodeFound)
        //        {
        //            blnIsSystemBarcode = !string.IsNullOrEmpty(NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_hydrofarm_code"));
        //            blnIsSystemBarcodeFound = blnIsSystemBarcode;
        //        }
        //        SetItemBarcode(objItem, "HydroFarm Code", NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_hydrofarm_code"), blnIsSystemBarcode);

        //        blnIsSystemBarcode = false;
        //        if (!blnIsSystemBarcodeFound)
        //        {
        //            blnIsSystemBarcode = !string.IsNullOrEmpty(NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_vend_1_code"));
        //            blnIsSystemBarcodeFound = blnIsSystemBarcode;
        //        }
        //        SetItemBarcode(objItem, "Vendor 1 Code", NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_vend_1_code"), blnIsSystemBarcode);

        //        blnIsSystemBarcode = false;
        //        if (!blnIsSystemBarcodeFound)
        //        {
        //            blnIsSystemBarcode = !string.IsNullOrEmpty(NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_vend_2_code"));
        //            blnIsSystemBarcodeFound = blnIsSystemBarcode;
        //        }
        //        SetItemBarcode(objItem, "Vendor 2 Code", NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_vend_2_code"), blnIsSystemBarcode);

        //        blnIsSystemBarcode = false;
        //        if (!blnIsSystemBarcodeFound)
        //        {
        //            blnIsSystemBarcode = !string.IsNullOrEmpty(NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_vend_3_code"));
        //            blnIsSystemBarcodeFound = blnIsSystemBarcode;
        //        }
        //        SetItemBarcode(objItem, "Vendor 3 Code", NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_vend_3_code"), blnIsSystemBarcode);

        //        blnIsSystemBarcode = false;
        //        if (!blnIsSystemBarcodeFound)
        //        {
        //            blnIsSystemBarcode = !string.IsNullOrEmpty(NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_vend_4_code"));
        //            blnIsSystemBarcodeFound = blnIsSystemBarcode;
        //        }
        //        SetItemBarcode(objItem, "Vendor 4 Code", NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_vend_4_code"), blnIsSystemBarcode);

        //        blnIsSystemBarcode = false;
        //        if (!blnIsSystemBarcodeFound)
        //        {
        //            blnIsSystemBarcode = !string.IsNullOrEmpty(NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_vend_5_code"));
        //            blnIsSystemBarcodeFound = blnIsSystemBarcode;
        //        }
        //        SetItemBarcode(objItem, "Vendor 5 Code", NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_vend_5_code"), blnIsSystemBarcode);

        //        blnIsSystemBarcode = false;
        //        if (!blnIsSystemBarcodeFound)
        //        {
        //            blnIsSystemBarcode = !string.IsNullOrEmpty(NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_old_hg_code"));
        //            blnIsSystemBarcodeFound = blnIsSystemBarcode;
        //        }
        //        SetItemBarcode(objItem, "Old Hawthorne Code", NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_old_hg_code"), blnIsSystemBarcode);

        //        blnIsSystemBarcode = false;
        //        if (!blnIsSystemBarcodeFound)
        //        {
        //            blnIsSystemBarcode = !string.IsNullOrEmpty(NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_amazonsku"));
        //            blnIsSystemBarcodeFound = blnIsSystemBarcode;
        //        }
        //        SetItemBarcode(objItem, "Amazon SKU", NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_amazonsku"), blnIsSystemBarcode);

        //        blnIsSystemBarcode = false;
        //        if (!blnIsSystemBarcodeFound)
        //        {
        //            blnIsSystemBarcode = !string.IsNullOrEmpty(NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_shopify_sku"));
        //            blnIsSystemBarcodeFound = blnIsSystemBarcode;
        //        }
        //        SetItemBarcode(objItem, "Shopify SKU", NetSuite.NetSuiteHelper.GetStringCustomFieldValue(item.NetSuiteKitItem, "custitem_thsi_shopify_sku"), blnIsSystemBarcode);

        //        SetItemBarcode(objItem, "UPC", item.NetSuiteKitItem.upcCode, false);

        //        if (item.NetSuiteKitItem != null)
        //        {
        //            if (objItem.ItemKits != null)
        //            {
        //                foreach (ImageSolutions.Item.ItemKit _ItemKit in objItem.ItemKits)
        //                {
        //                    _ItemKit.Delete(objConn, objTran);
        //                }
        //            }

        //            if (objItem.ItemKits == null) objItem.ItemKits = new List<Item.ItemKit>();

        //            //foreach (ImageSolutions.Item.ItemKit objItemKit in objItem.ItemKits)
        //            //{
        //            //Delete the ones that are no longer exist
        //            //}

        //            foreach (ItemMember objItemMember in item.NetSuiteKitItem.memberList.itemMember)
        //            {
        //                objFilter = new Item.ItemFilter();
        //                objFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
        //                objFilter.InternalID.SearchString = objItemMember.item.internalId;
        //                objItemKitItem = ImageSolutions.Item.Item.GetItem(objFilter);

        //                if (objItemKitItem == null)
        //                {
        //                    throw new Exception("Child item not available");
        //                }

        //                objItemKit = new Item.ItemKit();
        //                objItemKit.BusinessID = objItem.BusinessID;
        //                objItemKit.ChildItemID = objItemKitItem.ItemID;
        //                objItemKit.Quantity = Convert.ToDouble(objItemMember.quantity);

        //                if (!string.IsNullOrEmpty(objItemMember.memberUnit))
        //                {
        //                    Item.ItemUnit ItemUnit = new Item.ItemUnit();
        //                    Item.ItemUnitFilter ItemUnitFilter = new Item.ItemUnitFilter();
        //                    ItemUnitFilter.ItemUnitTypeID = new Database.Filter.StringSearch.SearchFilter();
        //                    ItemUnitFilter.ItemUnitTypeID.SearchString = objItemKitItem.ItemUnitTypeID;
        //                    ItemUnitFilter.Name = new Database.Filter.StringSearch.SearchFilter();
        //                    ItemUnitFilter.Name.SearchString = objItemMember.memberUnit;
        //                    ItemUnit = Item.ItemUnit.GetItemUnit(ItemUnitFilter);

        //                    if (ItemUnit == null)
        //                    {
        //                        Item.ItemUnit ItemUnitPlural = new Item.ItemUnit();
        //                        Item.ItemUnitFilter ItemUnitFilterPlural = new Item.ItemUnitFilter();
        //                        ItemUnitFilterPlural.ItemUnitTypeID = new Database.Filter.StringSearch.SearchFilter();
        //                        ItemUnitFilterPlural.ItemUnitTypeID.SearchString = objItemKitItem.ItemUnitTypeID;
        //                        ItemUnitFilterPlural.Plural = new Database.Filter.StringSearch.SearchFilter();
        //                        ItemUnitFilterPlural.Plural.SearchString = objItemMember.memberUnit;
        //                        ItemUnitPlural = Item.ItemUnit.GetItemUnit(ItemUnitFilterPlural);

        //                        objItemKit.ItemUnitID = ItemUnitPlural.ItemUnitID;
        //                    }
        //                    else
        //                    {
        //                        objItemKit.ItemUnitID = ItemUnit.ItemUnitID;
        //                    }
        //                }

        //                objItem.ItemKits.Add(objItemKit);
        //            }
        //        }

        //        objItem.InActive = item.NetSuiteKitItem.isInactive;
        //        objItem.LastModifiedOn = item.NetSuiteKitItem.lastModifiedDate;
        //        objItem.IsUpdated = true;
        //        objItem.ErrorMessage = string.Empty;
        //        if (objItem.IsNew)
        //            objItem.Create(objConn, objTran);
        //        else
        //            objItem.Update(objConn, objTran);

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
    }
}

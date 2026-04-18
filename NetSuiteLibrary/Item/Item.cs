using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using NetSuiteLibrary.com.netsuite.webservices;
using System.Net;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;

namespace NetSuiteLibrary.Item
{
    public class Item : NetSuiteBase
    {
        public static string NetSuiteInventoryFormID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteInventoryFormID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteInventoryFormID"].ToString();
                else
                    return string.Empty;
            }
        }
        public static string NetSuiteNonInventoryFormID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteNonInventoryFormID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteNonInventoryFormID"].ToString();
                else
                    return string.Empty;
            }
        }
        private static string NetSuiteGroupKitAssemblyFormID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteGroupKitAssemblyFormID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteGroupKitAssemblyFormID"].ToString();
                else
                    return string.Empty;
            }
        }
        private static string NetSuiteBundleDiscountID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteBundleDiscountID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteBundleDiscountID"].ToString();
                else
                    return string.Empty;
            }
        }

        private ImageSolutions.Item.Item mImageSolutionsItem = null;
        public ImageSolutions.Item.Item ImageSolutionsItem
        {
            get
            {
                if (mImageSolutionsItem == null && mNetSuiteInventoryItem != null && !string.IsNullOrEmpty(mNetSuiteInventoryItem.internalId))
                {
                    ImageSolutions.Item.ItemFilter objFilter = null;

                    try
                    {
                        objFilter = new ImageSolutions.Item.ItemFilter();
                        objFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.InternalID.SearchString = mNetSuiteInventoryItem.internalId;
                        mImageSolutionsItem = ImageSolutions.Item.Item.GetItem(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return mImageSolutionsItem;
            }
            private set
            {
                mImageSolutionsItem = value;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.InventoryItem mNetSuiteInventoryItem = null;
        public NetSuiteLibrary.com.netsuite.webservices.InventoryItem NetSuiteInventoryItem
        {
            get
            {
                if (mNetSuiteInventoryItem == null && mImageSolutionsItem != null && !string.IsNullOrEmpty(mImageSolutionsItem.InternalID))
                {
                    mNetSuiteInventoryItem = LoadNetSuiteInventoryItem(mImageSolutionsItem.InternalID);
                }
                return mNetSuiteInventoryItem;
            }
            private set
            {
                mNetSuiteInventoryItem = value;
            }
        }


        private NetSuiteLibrary.com.netsuite.webservices.LotNumberedInventoryItem mNetSuiteLotNumberInventoryItem = null;
        public NetSuiteLibrary.com.netsuite.webservices.LotNumberedInventoryItem NetSuiteLotNumberInventoryItem
        {
            get
            {
                if (mNetSuiteLotNumberInventoryItem == null && mImageSolutionsItem != null && !string.IsNullOrEmpty(mImageSolutionsItem.InternalID))
                {
                    mNetSuiteLotNumberInventoryItem = LoadNetSuiteLotNumberedInventoryItem(mImageSolutionsItem.InternalID);
                }
                return mNetSuiteLotNumberInventoryItem;
            }
            private set
            {
                mNetSuiteLotNumberInventoryItem = value;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.NonInventorySaleItem mNetSuiteNonInventoryItem = null;
        public NetSuiteLibrary.com.netsuite.webservices.NonInventorySaleItem NetSuiteNonInventoryItem
        {
            get
            {
                if (mNetSuiteNonInventoryItem == null && mImageSolutionsItem != null && !string.IsNullOrEmpty(mImageSolutionsItem.InternalID))
                {
                    mNetSuiteNonInventoryItem = LoadNetSuiteNonInventoryItem(mImageSolutionsItem.InternalID);
                }
                return mNetSuiteNonInventoryItem;
            }
            private set
            {
                mNetSuiteNonInventoryItem = value;
            }
        }
        private NetSuiteLibrary.com.netsuite.webservices.NonInventoryResaleItem mNetSuiteNonInventoryResaleItem = null;
        public NetSuiteLibrary.com.netsuite.webservices.NonInventoryResaleItem NetSuiteNonInventoryResaleItem
        {
            get
            {
                if (mNetSuiteNonInventoryResaleItem == null && mImageSolutionsItem != null && !string.IsNullOrEmpty(mImageSolutionsItem.InternalID))
                {
                    mNetSuiteNonInventoryResaleItem = LoadNetSuiteNonInventoryResaleItem(mImageSolutionsItem.InternalID);
                }
                return mNetSuiteNonInventoryResaleItem;
            }
            private set
            {
                mNetSuiteNonInventoryResaleItem = value;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.KitItem mNetSuiteKitItem = null;
        public NetSuiteLibrary.com.netsuite.webservices.KitItem NetSuiteKitItem
        {
            get
            {
                if (mNetSuiteKitItem == null && mImageSolutionsItem != null && !string.IsNullOrEmpty(mImageSolutionsItem.InternalID))
                {
                    mNetSuiteKitItem = LoadNetSuiteKitItem(mImageSolutionsItem.InternalID);
                }
                return mNetSuiteKitItem;
            }
            private set
            {
                mNetSuiteKitItem = value;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.ServiceSaleItem mNetSuiteServiceSaleItem = null;
        public NetSuiteLibrary.com.netsuite.webservices.ServiceSaleItem NetSuiteServiceSaleItem
        {
            get
            {
                if (mNetSuiteServiceSaleItem == null && mImageSolutionsItem != null && !string.IsNullOrEmpty(mImageSolutionsItem.InternalID))
                {
                    mNetSuiteServiceSaleItem = LoadNetSuiteServiceItem(mImageSolutionsItem.InternalID);
                }
                return mNetSuiteServiceSaleItem;
            }
            private set
            {
                mNetSuiteServiceSaleItem = value;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.ItemGroup mNetSuiteItemGroup = null;
        public NetSuiteLibrary.com.netsuite.webservices.ItemGroup NetSuiteItemGroup
        {
            get
            {
                if (mNetSuiteItemGroup == null && mImageSolutionsItem != null && !string.IsNullOrEmpty(mImageSolutionsItem.InternalID))
                {
                    mNetSuiteItemGroup = LoadNetSuiteItemGroup(mImageSolutionsItem.InternalID);
                }
                return mNetSuiteItemGroup;
            }
            private set
            {
                mNetSuiteItemGroup = value;
            }
        }
        private NetSuiteLibrary.com.netsuite.webservices.AssemblyItem mNetSuiteAssemblyItem = null;
        public NetSuiteLibrary.com.netsuite.webservices.AssemblyItem NetSuiteAssemblyItem
        {
            get
            {
                if (mNetSuiteAssemblyItem == null && mImageSolutionsItem != null && !string.IsNullOrEmpty(mImageSolutionsItem.InternalID))
                {
                    mNetSuiteAssemblyItem = LoadNetSuiteAssemblyItem(mImageSolutionsItem.InternalID);
                }
                return mNetSuiteAssemblyItem;
            }
            private set
            {
                mNetSuiteAssemblyItem = value;
            }
        }
        public Item(ImageSolutions.Item.Item ImageSolutionsItem)
        {
            mImageSolutionsItem = ImageSolutionsItem;
        }

        public Item(NetSuiteLibrary.com.netsuite.webservices.InventoryItem NetSuiteInventoryItem)
        {
            mNetSuiteInventoryItem = NetSuiteInventoryItem;
        }
        public Item(NetSuiteLibrary.com.netsuite.webservices.NonInventorySaleItem NetSuiteNonInventoryItem)
        {
            mNetSuiteNonInventoryItem = NetSuiteNonInventoryItem;
        }
        public Item(NetSuiteLibrary.com.netsuite.webservices.NonInventoryResaleItem NetSuiteNonInventoryResaleItem)
        {
            mNetSuiteNonInventoryResaleItem = NetSuiteNonInventoryResaleItem;
        }
        public Item(NetSuiteLibrary.com.netsuite.webservices.KitItem NetSuiteKitItem)
        {
            mNetSuiteKitItem = NetSuiteKitItem;
        }
        public Item(NetSuiteLibrary.com.netsuite.webservices.ServiceSaleItem NetSuiteServiceItem)
        {
            mNetSuiteServiceSaleItem = NetSuiteServiceItem;
        }
        //public Item(NetSuiteLibrary.com.netsuite.webservices.ServiceResaleItem NetSuiteInventoryItem)
        //{
        //    mNetSuiteServiceItem = NetSuiteInventoryItem;
        //}
        public Item(NetSuiteLibrary.com.netsuite.webservices.ItemGroup NetSuiteInventoryItem)
        {
            mNetSuiteItemGroup = NetSuiteInventoryItem;
        }
        public Item(NetSuiteLibrary.com.netsuite.webservices.AssemblyItem NetSuiteAssemblyItem)
        {
            mNetSuiteAssemblyItem = NetSuiteAssemblyItem;
        }
        private NetSuiteLibrary.com.netsuite.webservices.InventoryItem LoadNetSuiteInventoryItem(string NetSuiteInternalID)
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.InventoryItem objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.inventoryItem;
                objSORef.typeSpecified = true;
                objSORef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.InventoryItem))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.InventoryItem)objReadResult.record;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSORef = null;
                objReadResult = null;
            }
            return objReturn;
        }

        private NetSuiteLibrary.com.netsuite.webservices.LotNumberedInventoryItem LoadNetSuiteLotNumberedInventoryItem(string NetSuiteInternalID)
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.LotNumberedInventoryItem objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.lotNumberedInventoryItem;
                objSORef.typeSpecified = true;
                objSORef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.LotNumberedInventoryItem))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.LotNumberedInventoryItem)objReadResult.record;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSORef = null;
                objReadResult = null;
            }
            return objReturn;
        }

        private NetSuiteLibrary.com.netsuite.webservices.NonInventorySaleItem LoadNetSuiteNonInventoryItem(string NetSuiteInternalID)
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.NonInventorySaleItem objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.nonInventorySaleItem;
                objSORef.typeSpecified = true;
                objSORef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.NonInventorySaleItem))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.NonInventorySaleItem)objReadResult.record;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSORef = null;
                objReadResult = null;
            }
            return objReturn;
        }
        private NetSuiteLibrary.com.netsuite.webservices.NonInventoryResaleItem LoadNetSuiteNonInventoryResaleItem(string NetSuiteInternalID)
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.NonInventoryResaleItem objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.nonInventoryResaleItem;
                objSORef.typeSpecified = true;
                objSORef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.NonInventoryResaleItem))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.NonInventoryResaleItem)objReadResult.record;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSORef = null;
                objReadResult = null;
            }
            return objReturn;
        }
        private NetSuiteLibrary.com.netsuite.webservices.KitItem LoadNetSuiteKitItem(string NetSuiteInternalID)
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.KitItem objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.kitItem;
                objSORef.typeSpecified = true;
                objSORef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.KitItem))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.KitItem)objReadResult.record;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSORef = null;
                objReadResult = null;
            }
            return objReturn;
        }
        private NetSuiteLibrary.com.netsuite.webservices.ServiceSaleItem LoadNetSuiteServiceItem(string NetSuiteInternalID)
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.ServiceSaleItem objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.serviceSaleItem;
                objSORef.typeSpecified = true;
                objSORef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.ServiceSaleItem))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.ServiceSaleItem)objReadResult.record;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSORef = null;
                objReadResult = null;
            }
            return objReturn;
        }
        private NetSuiteLibrary.com.netsuite.webservices.ItemGroup LoadNetSuiteItemGroup(string NetSuiteInternalID)
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.ItemGroup objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.itemGroup;
                objSORef.typeSpecified = true;
                objSORef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.ItemGroup))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.ItemGroup)objReadResult.record;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSORef = null;
                objReadResult = null;
            }
            return objReturn;
        }
        private NetSuiteLibrary.com.netsuite.webservices.AssemblyItem LoadNetSuiteAssemblyItem(string NetSuiteInternalID)
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.AssemblyItem objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.assemblyItem;
                objSORef.typeSpecified = true;
                objSORef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.AssemblyItem))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.AssemblyItem)objReadResult.record;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSORef = null;
                objReadResult = null;
            }
            return objReturn;
        }
        public bool Create()
        {
            WriteResponse objWriteResponse = null;
            Item objItem = null;

            try
            {
                if (ImageSolutionsItem == null) throw new Exception("ImageSolutionsItem cannot be null");
                //if (NetSuiteInventoryItem != null) throw new Exception("Item record already exists in NetSuite");
                if (string.IsNullOrEmpty(ImageSolutionsItem.ItemNumber)) throw new Exception("ItemNumber is missing");

                objItem = ObjectAlreadyExists();
                if (objItem != null)
                {
                    //NetSuite InternalID did not get updated, auto fix
                    switch (ImageSolutionsItem.ItemType)
                    {
                        case "_nonInventoryItem":
                            ImageSolutionsItem.InternalID = objItem.NetSuiteNonInventoryResaleItem.internalId;
                            break;
                        case "_inventoryItem":
                            ImageSolutionsItem.InternalID = objItem.NetSuiteInventoryItem.internalId;
                            break;
                        default:
                            throw new Exception("Item Type not handeled");
                    }
                }
                else
                {
                    switch (ImageSolutionsItem.ItemType)
                    {
                        case "_nonInventoryItem":
                            objWriteResponse = Service.add(CreateNetSuiteNonInventoryResaleItem());
                            break;
                        case "_inventoryItem":
                            objWriteResponse = Service.add(CreateNetSuiteInventoryItem());
                            break;
                        default:
                            throw new Exception("Item Type not handeled");
                    }

                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create item: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        ImageSolutionsItem.InternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                    }
                }

                ImageSolutionsItem.ErrorMessage = string.Empty;
                ImageSolutionsItem.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsItem.ErrorMessage = ex.Message;
                ImageSolutionsItem.Update();
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        //public bool Update()
        //{
        //    WriteResponse objWriteResponse = null;

        //    try
        //    {
        //        if (ImageSolutionsItem == null) throw new Exception("ImageSolutionsItem cannot be null");
        //        if (NetSuiteInventoryItem == null && NetSuiteServiceItem == null && NetSuiteItemGroup == null) throw new Exception("Item record does not exists in NetSuite");
        //        if (string.IsNullOrEmpty(ImageSolutionsItem.TPIN)) throw new Exception("TPIN is missing");
        //        //if (ImageSolutionsItem.Vendor.ExternalID == "12") throw new Exception("Bolton Item can not updated");

        //        switch (ImageSolutionsItem.ItemType)
        //            {
        //                case Toolots.Item.Item.enumItemType.simple:
        //                    objWriteResponse = Service.update(UpdateNetSuiteInventoryItem());
        //                    break;
        //            //case Toolots.Item.Item.enumItemType._virtual:
        //            //    objWriteResponse = Service.update(NetSuiteServiceItem);
        //            //    break;
        //            case Toolots.Item.Item.enumItemType.bundle:
        //                objWriteResponse = Service.update(UpdateNetSuiteItemGroup());
        //                break;
        //            default:
        //                    throw new Exception("Item Type not handeled");
        //            }

        //            if (objWriteResponse.status.isSuccess != true)
        //            {
        //                throw new Exception("Unable to update item: " + objWriteResponse.status.statusDetail[0].message);
        //            }
        //        ImageSolutionsItem.ErrorMessage = string.Empty;
        //        ImageSolutionsItem.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        ImageSolutionsItem.ErrorMessage = ex.Message;
        //        ImageSolutionsItem.Update();
        //    }
        //    finally
        //    {
        //        objWriteResponse = null;
        //    }
        //    return true;
        //}

        //public bool Delete()
        //{
        //    RecordRef objItemRef = null;
        //    WriteResponse objDeleteResponse = null;

        //    try
        //    {
        //        if (ImageSolutionsItem == null) throw new Exception("ToolotsFulfillment cannot be null");

        //        if (NetSuiteInventoryItem != null)
        //        {
        //            objItemRef = new RecordRef();
        //            objItemRef.internalId = NetSuiteInventoryItem.internalId;
        //            objItemRef.type = RecordType.inventoryItem;
        //            objItemRef.typeSpecified = true;                    
        //        }
        //        else if (NetSuiteItemGroup != null)
        //        {
        //            objItemRef = new RecordRef();
        //            objItemRef.internalId = NetSuiteItemGroup.internalId;
        //            objItemRef.type = RecordType.itemGroup;
        //            objItemRef.typeSpecified = true;
        //        }
        //        else if (NetSuiteServiceItem != null)
        //        {
        //            objItemRef = new RecordRef();
        //            objItemRef.internalId = NetSuiteServiceItem.internalId;
        //            objItemRef.type = RecordType.serviceResaleItem;
        //            objItemRef.typeSpecified = true;
        //        }

        //        if (objItemRef != null)
        //        {
        //            objDeleteResponse = Service.delete(objItemRef, null);
        //            if (objDeleteResponse.status.isSuccess != true)
        //            {
        //                throw new Exception("Unable to delete item: " + objDeleteResponse.status.statusDetail[0].message);
        //            }
        //            else
        //            {
        //                mNetSuiteInventoryItem = null;
        //                mNetSuiteItemGroup = null;
        //                mNetSuiteServiceItem = null;
        //            }
        //        }
        //        ImageSolutionsItem.ErrorMessage = string.Empty;
        //        ImageSolutionsItem.NetSuiteInternalID = string.Empty;
        //        ImageSolutionsItem.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message.Contains("Can not find item with Internal ID"))
        //        {
        //            ImageSolutionsItem.ErrorMessage = string.Empty;
        //            ImageSolutionsItem.NetSuiteInternalID = string.Empty;
        //            ImageSolutionsItem.Update();
        //        }
        //        else
        //        {
        //            ImageSolutionsItem.ErrorMessage = "item.cs - Delete() - " + ex.Message;
        //            ImageSolutionsItem.Update();
        //        }
        //    }
        //    finally
        //    {
        //        objItemRef = null;
        //        objDeleteResponse = null;
        //    }
        //    return true;
        //}

        //public bool SyncInventory()
        //{
        //    ImageSolutions.Inventory.Inventory objInventory = null;
        //    ImageSolutions.Inventory.InventoryFilter objInventoryFilter = null;

        //    ImageSolutions.Location.Location objLocation = null;
        //    ImageSolutions.Location.LocationFilter objLocationFilter = null;
        //    List<ImageSolutions.Item.RetailerItem> objRetailerItems = null;
        //    ImageSolutions.Item.RetailerItemFilter objRetailerItemFilter = null;
        //    List<ImageSolutions.Item.UOM> objUOMs = null;
        //    ImageSolutions.Item.UOM objUOM = null;

        //    try
        //    {
        //        if (ImageSolutionsItem == null) throw new Exception("ImageSolutionsItem cannot be null");
        //        objUOMs = ImageSolutions.Item.UOM.GetUOMs("9");

        //        if (ImageSolutionsItem.BusinessID == "9")
        //        {
        //            if (ImageSolutionsItem == null) throw new Exception("ImageSolutionsItem cannot be null");
        //            if (NetSuiteLotNumberInventoryItem == null) throw new Exception("Item record does not exists in NetSuite");

        //            if (NetSuiteLotNumberInventoryItem != null && NetSuiteLotNumberInventoryItem.locationsList != null && NetSuiteLotNumberInventoryItem.locationsList.locations != null && NetSuiteLotNumberInventoryItem.locationsList.locations.Count() > 0)
        //            {
        //                foreach (com.netsuite.webservices.LotNumberedInventoryItemLocations objInventoryItemLocation in NetSuiteLotNumberInventoryItem.locationsList.locations)
        //                {
        //                    if (objInventoryItemLocation.locationId.internalId == "1")
        //                    {
        //                        if (objInventoryItemLocation.quantityAvailableSpecified)
        //                        {
        //                            objRetailerItemFilter = new ImageSolutions.Item.RetailerItemFilter();
        //                            objRetailerItemFilter.BusinessID = new Database.Filter.StringSearch.SearchFilter();
        //                            objRetailerItemFilter.BusinessID.SearchString = "9";
        //                            objRetailerItemFilter.ItemID = ImageSolutionsItem.ItemID;
        //                            objRetailerItems = ImageSolutions.Item.RetailerItem.GetRetailerItems(objRetailerItemFilter);
        //                            if (objRetailerItems != null)
        //                            {
        //                                foreach (ImageSolutions.Item.RetailerItem objRetailerItem in objRetailerItems)
        //                                {
        //                                    objUOM = objUOMs.Find(m => m.Unit == ImageSolutionsItem.UOMStock);

        //                                    if (objUOM != null)
        //                                    {
        //                                        int intAvailableInBaseUnit = Convert.ToInt32(objInventoryItemLocation.quantityAvailable * objUOM.Conversion);

        //                                        if (string.IsNullOrEmpty(objRetailerItem.UOM))
        //                                        {
        //                                            objRetailerItem.Available = intAvailableInBaseUnit;
        //                                        }
        //                                        else
        //                                        {
        //                                            objUOM = objUOMs.Find(m => m.Unit == objRetailerItem.UOM);
        //                                            if (objUOM != null)
        //                                            {
        //                                                objRetailerItem.Available = Convert.ToInt32(intAvailableInBaseUnit / objUOM.Conversion);
        //                                            }
        //                                        }
        //                                        objRetailerItem.InventoryUpdatedOn = DateTime.UtcNow;
        //                                        objRetailerItem.Update();
        //                                    }

        //                                }
        //                            }
        //                        }
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (NetSuiteInventoryItem == null) throw new Exception("Item record does not exists in NetSuite");

        //            if (NetSuiteInventoryItem != null && NetSuiteInventoryItem.locationsList != null && NetSuiteInventoryItem.locationsList.locations != null && NetSuiteInventoryItem.locationsList.locations.Count() > 0)
        //            {
        //                foreach (com.netsuite.webservices.InventoryItemLocations objInventoryItemLocation in NetSuiteInventoryItem.locationsList.locations)
        //                {
        //                    objLocationFilter = new ImageSolutions.Location.LocationFilter();
        //                    objLocationFilter.NetSuiteInternalID = new Database.Filter.StringSearch.SearchFilter();
        //                    objLocationFilter.NetSuiteInternalID.SearchString = objInventoryItemLocation.locationId.internalId;
        //                    objLocation = ImageSolutions.Location.Location.GetLocation(objLocationFilter);

        //                    if (objLocation != null)
        //                    {
        //                        objInventoryFilter = new ImageSolutions.Inventory.InventoryFilter();
        //                        objInventoryFilter.LocationID = new Database.Filter.StringSearch.SearchFilter();
        //                        objInventoryFilter.LocationID.SearchString = objLocation.LocationID;
        //                        objInventoryFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
        //                        objInventoryFilter.ItemID.SearchString = ImageSolutionsItem.ItemID;
        //                        objInventory = ImageSolutions.Inventory.Inventory.GetInventory(objInventoryFilter);
        //                        if (objInventory == null)
        //                        {
        //                            objInventory = new ImageSolutions.Inventory.Inventory(ImageSolutionsItem.BusinessID);
        //                            objInventory.ItemID = ImageSolutionsItem.ItemID;
        //                            objInventory.LocationID = objLocation.LocationID;
        //                        }
        //                        else
        //                        {
        //                            objInventory.OnHand = 0;
        //                            objInventory.Available = 0;
        //                        }
        //                        if (objInventoryItemLocation.quantityAvailableSpecified) objInventory.Available += Convert.ToInt32(objInventoryItemLocation.quantityAvailable);
        //                        if (objInventoryItemLocation.quantityOnHandSpecified) objInventory.OnHand += Convert.ToInt32(objInventoryItemLocation.quantityOnHand);

        //                        if (objInventory.IsNew)
        //                            objInventory.Create();
        //                        else
        //                            objInventory.Update();
        //                    }
        //                }
        //            }
        //        }

        //        ImageSolutionsItem.ErrorMessage = string.Empty;
        //        ImageSolutionsItem.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        ImageSolutionsItem.ErrorMessage = "SyncInventory()- " + ex.Message;
        //        ImageSolutionsItem.Update();
        //    }
        //    finally { }
        //    return true;
        //}

        public bool AddPreferredVendor(string VendorID)
        {
            InventoryItem objInventoryItem = null;
            WriteResponse objResponse = null;

            try
            {
                if (NetSuiteInventoryItem == null) throw new Exception("Netsuite inventory item is required");
                if (string.IsNullOrEmpty(NetSuiteInventoryItem.internalId)) throw new Exception("Netsuite internalID is required");
                objInventoryItem = new InventoryItem();
                objInventoryItem.internalId = NetSuiteInventoryItem.internalId;
                objInventoryItem.itemVendorList = new ItemVendorList();
                objInventoryItem.itemVendorList.replaceAll = false;

                objInventoryItem.itemVendorList.itemVendor = new ItemVendor[1];
                objInventoryItem.itemVendorList.itemVendor[0] = new ItemVendor();
                objInventoryItem.itemVendorList.itemVendor[0].vendor = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(VendorID, RecordType.vendor);
                objInventoryItem.itemVendorList.itemVendor[0].preferredVendor = true;
                objInventoryItem.itemVendorList.itemVendor[0].preferredVendorSpecified = true;

                objResponse = Service.update(objInventoryItem);
                if (!objResponse.status.isSuccess)
                    throw new Exception("Fail to add vendor - " + objResponse.status.statusDetail[0].message);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objInventoryItem = null;
                objResponse = null;
            }
            return true;
        }

        private NetSuiteLibrary.com.netsuite.webservices.InventoryItem CreateNetSuiteInventoryItem()
        {
            NetSuiteLibrary.com.netsuite.webservices.InventoryItem objReturn = null;
    
            try
            {
                if (string.IsNullOrEmpty(ImageSolutionsItem.ItemName)) throw new Exception("ItemName is missing");
                if (string.IsNullOrEmpty(ImageSolutionsItem.ItemNumber)) throw new Exception("ItemNumber is missing");

                objReturn = new NetSuiteLibrary.com.netsuite.webservices.InventoryItem();
                objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef("-200", RecordType.inventoryItem);
                objReturn.internalId = ImageSolutionsItem.InternalID;
                objReturn.itemId = ImageSolutionsItem.ItemNumber;
                objReturn.displayName = ImageSolutionsItem.ItemName;
                objReturn.salesDescription = ImageSolutionsItem.SalesDescription;
                objReturn.taxSchedule = NetSuiteLibrary.NetSuiteHelper.GetRecordRef("1", RecordType.taxAcct);


                //objReturn.pricingMatrix = new PricingMatrix();
                //objReturn.pricingMatrix.replaceAll = false;
                //objReturn.pricingMatrix.pricing = new Pricing[5];

                //objReturn.pricingMatrix.pricing[0] = new Pricing();
                //objReturn.pricingMatrix.pricing[0].priceLevel = NetSuiteHelper.GetRecordRef("1", RecordType.priceLevel); //base price
                ////objReturn.pricingMatrix.pricing[0].currency = NetSuiteHelper.GetRecordRef(NetSuiteUSCurrency, RecordType.currency);
                //objReturn.pricingMatrix.pricing[0].priceList = new Price[2];
                //objReturn.pricingMatrix.pricing[0].priceList[0] = new Price();
                //objReturn.pricingMatrix.pricing[0].priceList[0].value = Convert.ToDouble(ImageSolutionsItem.BasePrice);
                //objReturn.pricingMatrix.pricing[0].priceList[0].quantity = 0.0;

                //objReturn.pricingMatrix.pricing[1] = new Pricing();
                //objReturn.pricingMatrix.pricing[1].priceLevel = NetSuiteHelper.GetRecordRef("5", RecordType.priceLevel); //online price
                ////objReturn.pricingMatrix.pricing[1].currency = NetSuiteHelper.GetRecordRef(NetSuiteUSCurrency, RecordType.currency);
                //objReturn.pricingMatrix.pricing[1].priceList = new Price[1];
                //objReturn.pricingMatrix.pricing[1].priceList[0] = new Price();
                //objReturn.pricingMatrix.pricing[1].priceList[0].value = Convert.ToDouble(ImageSolutionsItem.BasePrice);
                //objReturn.pricingMatrix.pricing[1].priceList[0].quantity = 0.0;

                //objReturn.pricingMatrix.pricing[2] = new Pricing();
                //objReturn.pricingMatrix.pricing[2].priceLevel = NetSuiteHelper.GetRecordRef("2", RecordType.priceLevel); // Alternate Price 1 
                ////objReturn.pricingMatrix.pricing[2].currency = NetSuiteHelper.GetRecordRef(NetSuiteUSCurrency, RecordType.currency);
                //objReturn.pricingMatrix.pricing[2].priceList = new Price[1];
                //objReturn.pricingMatrix.pricing[2].priceList[0] = new Price();
                //objReturn.pricingMatrix.pricing[2].priceList[0].value = 0;
                //objReturn.pricingMatrix.pricing[2].priceList[0].quantity = 0.0;

                //objReturn.pricingMatrix.pricing[3] = new Pricing();
                //objReturn.pricingMatrix.pricing[3].priceLevel = NetSuiteHelper.GetRecordRef("3", RecordType.priceLevel); //Alternate Price 3
                ////objReturn.pricingMatrix.pricing[0].currency = NetSuiteHelper.GetRecordRef(NetSuiteUSCurrency, RecordType.currency);
                //objReturn.pricingMatrix.pricing[3].priceList = new Price[1];
                //objReturn.pricingMatrix.pricing[3].priceList[0] = new Price();
                //objReturn.pricingMatrix.pricing[3].priceList[0].value = 0;
                //objReturn.pricingMatrix.pricing[3].priceList[0].quantity = 0.0;

                //objReturn.pricingMatrix.pricing[4] = new Pricing();
                //objReturn.pricingMatrix.pricing[4].priceLevel = NetSuiteHelper.GetRecordRef("4", RecordType.priceLevel); //Alternate Price 3
                ////objReturn.pricingMatrix.pricing[0].currency = NetSuiteHelper.GetRecordRef(NetSuiteUSCurrency, RecordType.currency);
                //objReturn.pricingMatrix.pricing[4].priceList = new Price[1];
                //objReturn.pricingMatrix.pricing[4].priceList[0] = new Price();
                //objReturn.pricingMatrix.pricing[4].priceList[0].value = 0;
                //objReturn.pricingMatrix.pricing[4].priceList[0].quantity = 0.0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        private NetSuiteLibrary.com.netsuite.webservices.NonInventoryResaleItem CreateNetSuiteNonInventoryResaleItem()
        {
            NetSuiteLibrary.com.netsuite.webservices.NonInventoryResaleItem objReturn = null;

            try
            {
                if (string.IsNullOrEmpty(ImageSolutionsItem.ItemName)) throw new Exception("ItemName is missing");
                if (string.IsNullOrEmpty(ImageSolutionsItem.ItemNumber)) throw new Exception("ItemNumber is missing");

                objReturn = new NetSuiteLibrary.com.netsuite.webservices.NonInventoryResaleItem();
                objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef("-210", RecordType.nonInventoryResaleItem);

                objReturn.internalId = ImageSolutionsItem.InternalID;
                objReturn.itemId = ImageSolutionsItem.ItemNumber;
                objReturn.displayName = ImageSolutionsItem.ItemName;
                objReturn.salesDescription = ImageSolutionsItem.SalesDescription;
                objReturn.taxSchedule = NetSuiteLibrary.NetSuiteHelper.GetRecordRef("1", RecordType.taxAcct);

                //objReturn.pricingMatrix = new PricingMatrix();
                //objReturn.pricingMatrix.replaceAll = false;
                //objReturn.pricingMatrix.pricing = new Pricing[5];

                //objReturn.pricingMatrix.pricing[0] = new Pricing();
                //objReturn.pricingMatrix.pricing[0].priceLevel = NetSuiteHelper.GetRecordRef("1", RecordType.priceLevel); //base price
                ////objReturn.pricingMatrix.pricing[0].currency = NetSuiteHelper.GetRecordRef(NetSuiteUSCurrency, RecordType.currency);
                //objReturn.pricingMatrix.pricing[0].priceList = new Price[1];
                //objReturn.pricingMatrix.pricing[0].priceList[0] = new Price();
                //objReturn.pricingMatrix.pricing[0].priceList[0].value = Convert.ToDouble(ImageSolutionsItem.BasePrice);
                //objReturn.pricingMatrix.pricing[0].priceList[0].quantity = 0.0;

                //objReturn.pricingMatrix.pricing[1] = new Pricing();
                //objReturn.pricingMatrix.pricing[1].priceLevel = NetSuiteHelper.GetRecordRef("5", RecordType.priceLevel); //online price
                ////objReturn.pricingMatrix.pricing[1].currency = NetSuiteHelper.GetRecordRef(NetSuiteUSCurrency, RecordType.currency);
                //objReturn.pricingMatrix.pricing[1].priceList = new Price[1];
                //objReturn.pricingMatrix.pricing[1].priceList[0] = new Price();
                //objReturn.pricingMatrix.pricing[1].priceList[0].value = Convert.ToDouble(ImageSolutionsItem.BasePrice);
                //objReturn.pricingMatrix.pricing[1].priceList[0].quantity = 0.0;


                //objReturn.pricingMatrix.pricing[2] = new Pricing();
                //objReturn.pricingMatrix.pricing[2].priceLevel = NetSuiteHelper.GetRecordRef("2", RecordType.priceLevel); // Alternate Price 1 
                ////objReturn.pricingMatrix.pricing[2].currency = NetSuiteHelper.GetRecordRef(NetSuiteUSCurrency, RecordType.currency);
                //objReturn.pricingMatrix.pricing[2].priceList = new Price[1];
                //objReturn.pricingMatrix.pricing[2].priceList[0] = new Price();
                //objReturn.pricingMatrix.pricing[2].priceList[0].value = 0;
                //objReturn.pricingMatrix.pricing[2].priceList[0].quantity = 0.0;

                //objReturn.pricingMatrix.pricing[3] = new Pricing();
                //objReturn.pricingMatrix.pricing[3].priceLevel = NetSuiteHelper.GetRecordRef("3", RecordType.priceLevel); //Alternate Price 3
                ////objReturn.pricingMatrix.pricing[0].currency = NetSuiteHelper.GetRecordRef(NetSuiteUSCurrency, RecordType.currency);
                //objReturn.pricingMatrix.pricing[3].priceList = new Price[1];
                //objReturn.pricingMatrix.pricing[3].priceList[0] = new Price();
                //objReturn.pricingMatrix.pricing[3].priceList[0].value = 0;
                //objReturn.pricingMatrix.pricing[3].priceList[0].quantity = 0.0;

                //objReturn.pricingMatrix.pricing[4] = new Pricing();
                //objReturn.pricingMatrix.pricing[4].priceLevel = NetSuiteHelper.GetRecordRef("4", RecordType.priceLevel); //Alternate Price 3
                ////objReturn.pricingMatrix.pricing[0].currency = NetSuiteHelper.GetRecordRef(NetSuiteUSCurrency, RecordType.currency);
                //objReturn.pricingMatrix.pricing[4].priceList = new Price[1];
                //objReturn.pricingMatrix.pricing[4].priceList[0] = new Price();
                //objReturn.pricingMatrix.pricing[4].priceList[0].value = 0;
                //objReturn.pricingMatrix.pricing[4].priceList[0].quantity = 0.0;



            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        //private NetSuiteLibrary.com.netsuite.webservices.InventoryItem UpdateNetSuiteInventoryItem()
        //{
        //    NetSuiteLibrary.com.netsuite.webservices.InventoryItem objReturn = null;
        //    NetSuiteLibrary.Vendor.Vendor objNSVendor = null;
        //    int intCustomFieldIndex = 0;

        //    //do not update bolton item type 
        //    try
        //    {
        //        if (string.IsNullOrEmpty(ImageSolutionsItem.TPIN)) throw new Exception("TPIN is missing");
        //        if (string.IsNullOrEmpty(ImageSolutionsItem.Name)) throw new Exception("Name is missing");
        //        if (string.IsNullOrEmpty(ImageSolutionsItem.SKU)) throw new Exception("SKU is missing");
        //        if (string.IsNullOrEmpty(ImageSolutionsItem.VendorSKU)) throw new Exception("SKU is missing");

        //        if (NetSuiteInventoryItem == null) throw new Exception("Item not found as inventory item in NetSuite, can not update");

        //        objReturn = new NetSuiteLibrary.com.netsuite.webservices.InventoryItem();
        //        objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteInventoryFormID, RecordType.inventoryItem);
        //        objReturn.customFieldList = new CustomFieldRef[99];
        //        objReturn.internalId = ImageSolutionsItem.NetSuiteInternalID;

        //        if (ImageSolutionsItem.Width != null) objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateDoubleCustomField(ImageSolutionsItem.Width.Value, "custitem_width");
        //        if (ImageSolutionsItem.Height != null) objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateDoubleCustomField(ImageSolutionsItem.Height.Value, "custitem_height");
        //        if (ImageSolutionsItem.Length != null) objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateDoubleCustomField(ImageSolutionsItem.Length.Value, "custitem_length");

        //        if (ImageSolutionsItem.IsPartItem != null) objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateSelectCustomField(ImageSolutionsItem.IsPartItem.Value ? "1" : "2", "custitem_is_part_item");

        //        if (ImageSolutionsItem.VendorExternalID != "12")
        //        {
        //            if (ImageSolutionsItem.IsConsignment)
        //            {
        //                if (ImageSolutionsItem.Vendor == null) throw new Exception("Missing Vendor Setup");
        //                if (string.IsNullOrEmpty(ImageSolutionsItem.Vendor.NetSuiteInternalID)) throw new Exception("NetSuite vendor has not been created");

        //                if (ImageSolutionsItem.FulfilledBy == null || ImageSolutionsItem.FulfilledBy != Toolots.Item.Item.enumFulfilledBy.Toolots)
        //                    throw new Exception("Consignment items should be set to fulfilled by Toolots, fix DB and Magento");

        //                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateSelectCustomField("1", "custitem_item_type");
        //                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateSelectCustomField(ImageSolutionsItem.Vendor.NetSuiteInternalID, "custitem_vendor");

        //                objReturn.isDropShipItem = false;
        //                objReturn.isDropShipItemSpecified = true;

        //                if (NetSuiteInventoryItem.itemVendorList != null && NetSuiteInventoryItem.itemVendorList.itemVendor != null
        //                    && NetSuiteInventoryItem.itemVendorList.itemVendor.Count() > 0)
        //                {
        //                    objReturn.itemVendorList = new ItemVendorList();
        //                    objReturn.itemVendorList = new ItemVendorList();
        //                    objReturn.itemVendorList.replaceAll = true;
        //                }
        //            }
        //            else
        //            {
        //                if (ImageSolutionsItem.Vendor == null) throw new Exception("Missing Vendor Setup");
        //                if (string.IsNullOrEmpty(ImageSolutionsItem.Vendor.NetSuiteInternalID)) throw new Exception("NetSuite vendor has not been created");

        //                if (ImageSolutionsItem.FulfilledBy == null || ImageSolutionsItem.FulfilledBy != Toolots.Item.Item.enumFulfilledBy.Merchant) throw new Exception("Dropship items should be set to fulfilled by Merchant, fix DB and Magento");
        //                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateSelectCustomField("2", "custitem_item_type");
        //                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateSelectCustomField(ImageSolutionsItem.Vendor.NetSuiteInternalID, "custitem_vendor");

        //                objReturn.isDropShipItem = true;
        //                objReturn.isDropShipItemSpecified = true;
        //                objReturn.itemVendorList = new ItemVendorList();
        //                objReturn.itemVendorList.replaceAll = true;
        //                objReturn.itemVendorList.itemVendor = new ItemVendor[1];
        //                objReturn.itemVendorList.itemVendor[0] = new ItemVendor();
        //                objReturn.itemVendorList.itemVendor[0].vendor = NetSuiteHelper.GetRecordRef(ImageSolutionsItem.Vendor.NetSuiteInternalID, RecordType.vendor);
        //                objReturn.itemVendorList.itemVendor[0].preferredVendor = true;
        //                objReturn.itemVendorList.itemVendor[0].preferredVendorSpecified = true;
        //                objReturn.itemVendorList.itemVendor[0].purchasePrice = Convert.ToDouble(ImageSolutionsItem.DropShipPrice);

        //                objNSVendor = new Vendor.Vendor(ImageSolutionsItem.Vendor);
        //                if (objNSVendor == null || objNSVendor.NetSuiteVendor == null) throw new Exception("Unable to load NetSuiteVendor from VendorID:" + ImageSolutionsItem.VendorID);
        //                if (objNSVendor.Lead == null) throw new Exception("Unabel to load NetSuiteLead from VendorID:" + ImageSolutionsItem.VendorID);

        //                switch (objNSVendor.Lead.MerchantTerm)
        //                {
        //                    case Customer.Customer.enumMerchantTerm.Consigned:
        //                        objReturn.itemVendorList.itemVendor[0].purchasePrice = 0;
        //                        break;
        //                    case Customer.Customer.enumMerchantTerm.Dropship:
        //                        if (ImageSolutionsItem.DropShipPrice == null) throw new Exception("Missing drop ship vendor purchase price");
        //                        objReturn.itemVendorList.itemVendor[0].purchasePrice = Convert.ToDouble(ImageSolutionsItem.DropShipPrice.Value);
        //                        break;
        //                    default:
        //                        throw new Exception("MerchantTerm not handeled");
        //                }
        //                objReturn.itemVendorList.itemVendor[0].purchasePriceSpecified = true;
        //            }
        //        }
        //        objReturn.vendorName = ImageSolutionsItem.VendorSKU;
        //        objReturn.displayName = ImageSolutionsItem.SKU;
        //        objReturn.salesDescription = ImageSolutionsItem.Name;
        //        objReturn.purchaseDescription = ImageSolutionsItem.Name;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally { }
        //    return objReturn;
        //}

        //private NetSuiteLibrary.com.netsuite.webservices.ServiceResaleItem CreateNetSuiteServiceItem()
        //{
        //    NetSuiteLibrary.com.netsuite.webservices.ServiceResaleItem objReturn = null;
        //    int intCustomFieldIndex = 0;

        //    try
        //    {
        //        if (string.IsNullOrEmpty(ImageSolutionsItem.TPIN)) throw new Exception("TPIN is missing");
        //        if (string.IsNullOrEmpty(ImageSolutionsItem.Name)) throw new Exception("Name is missing");
        //        if (string.IsNullOrEmpty(ImageSolutionsItem.SKU)) throw new Exception("SKU is missing");
        //        if (string.IsNullOrEmpty(ImageSolutionsItem.VendorSKU)) throw new Exception("SKU is missing");

        //        objReturn = new NetSuiteLibrary.com.netsuite.webservices.ServiceResaleItem();
        //        objReturn.subsidiaryList = new RecordRef[1];
        //        objReturn.subsidiaryList[0] = NetSuiteHelper.GetRecordRef(ToolotsSubsidiaryID, RecordType.subsidiary);
        //        objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteNonInventoryFormID, RecordType.serviceResaleItem);
        //        objReturn.customFieldList = new CustomFieldRef[99];
        //        objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsItem.ItemID, "custitem_api_external_id");

        //        //objReturn.internalId = ImageSolutionsItem.NetSuiteInternalID;
        //        objReturn.itemId = ImageSolutionsItem.TPIN;
        //        objReturn.vendorName = ImageSolutionsItem.VendorSKU;
        //        objReturn.displayName = ImageSolutionsItem.SKU;
        //        objReturn.salesDescription = ImageSolutionsItem.Name;
        //        objReturn.purchaseDescription = ImageSolutionsItem.Name;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally { }
        //    return objReturn;
        //}

        //private NetSuiteLibrary.com.netsuite.webservices.ItemGroup CreateNetSuiteItemGroup()
        //{
        //    NetSuiteLibrary.com.netsuite.webservices.ItemGroup objReturn = null;
        //    int intCustomFieldIndex = 0;

        //    try
        //    {
        //        if (string.IsNullOrEmpty(ImageSolutionsItem.TPIN)) throw new Exception("TPIN is missing");
        //        if (string.IsNullOrEmpty(ImageSolutionsItem.Name)) throw new Exception("Name is missing");
        //        if (string.IsNullOrEmpty(ImageSolutionsItem.SKU)) throw new Exception("SKU is missing");
        //        if (string.IsNullOrEmpty(ImageSolutionsItem.VendorSKU)) throw new Exception("SKU is missing");

        //        objReturn = new NetSuiteLibrary.com.netsuite.webservices.ItemGroup();
        //        objReturn.subsidiaryList = new RecordRef[1];
        //        objReturn.subsidiaryList[0] = NetSuiteHelper.GetRecordRef(ToolotsSubsidiaryID, RecordType.subsidiary);
        //        objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteGroupKitAssemblyFormID, RecordType.itemGroup);
        //        objReturn.customFieldList = new CustomFieldRef[99];
        //        objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsItem.ItemID, "custitem_api_external_id");

        //        //objReturn.internalId = ImageSolutionsItem.NetSuiteInternalID;
        //        objReturn.itemId = ImageSolutionsItem.TPIN;
        //        objReturn.vendorName = ImageSolutionsItem.VendorSKU;
        //        objReturn.displayName = ImageSolutionsItem.SKU;
        //        objReturn.description = ImageSolutionsItem.Name;

        //        objReturn.printItems = true;
        //        objReturn.printItemsSpecified = true;

        //        objReturn.memberList = new ItemMemberList();
        //        objReturn.memberList.replaceAll = true;
        //        objReturn.memberList.itemMember = new ItemMember[1];
        //        objReturn.memberList.itemMember[0] = new ItemMember();
        //        objReturn.memberList.itemMember[0].item = NetSuiteHelper.GetRecordRef(NetSuiteBundleDiscountID, RecordType.discountItem);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally { }
        //    return objReturn;
        //}

        //private NetSuiteLibrary.com.netsuite.webservices.ItemGroup UpdateNetSuiteItemGroup()
        //{
        //    NetSuiteLibrary.com.netsuite.webservices.ItemGroup objReturn = null;
        //    int intCustomFieldIndex = 0;

        //    try
        //    {
        //        if (string.IsNullOrEmpty(ImageSolutionsItem.TPIN)) throw new Exception("TPIN is missing");
        //        if (string.IsNullOrEmpty(ImageSolutionsItem.Name)) throw new Exception("Name is missing");
        //        if (string.IsNullOrEmpty(ImageSolutionsItem.SKU)) throw new Exception("SKU is missing");
        //        if (string.IsNullOrEmpty(ImageSolutionsItem.VendorSKU)) throw new Exception("SKU is missing");

        //        objReturn = new NetSuiteLibrary.com.netsuite.webservices.ItemGroup();

        //        objReturn.internalId = ImageSolutionsItem.NetSuiteInternalID;   
        //        objReturn.vendorName = ImageSolutionsItem.VendorSKU;
        //        objReturn.displayName = ImageSolutionsItem.SKU;
        //        objReturn.description = ImageSolutionsItem.Name;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally { }
        //    return objReturn;
        //}

        public Item ObjectAlreadyExists()
        {
            List<Item> objItems = null;
            ItemFilter objFilter = null;
            Item objReturn = null;

            try
            {
                objFilter = new NetSuiteLibrary.Item.ItemFilter();
                //objFilter.APIExternalID = ImageSolutionsItem.ItemID;
                objFilter.ItemNumber = new SearchStringField();
                objFilter.ItemNumber.searchValue = ImageSolutionsItem.ItemNumber;
                objFilter.ItemNumber.operatorSpecified = true;
                objFilter.ItemNumber.@operator = SearchStringFieldOperator.@is;
                //objFilter.APIExternalID = ImageSolutionsItem.ItemID;

                objItems = GetItems(Service, objFilter);
                if (objItems != null && objItems.Count() > 0)
                {
                    if (objItems.Count > 1) throw new Exception("More than one Items with API External ID:" + ImageSolutionsItem.ItemID + " found in Netsuite with InternalIDs " + string.Join(", ", objItems.Select(m => m.NetSuiteInventoryItem.internalId)));
                    objReturn = objItems[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItems = null;
                objFilter = null;
            }
            return objReturn;
        }
        public Item GetItem(ItemFilter Filter)
        {
            return GetItem(Service, Filter);
        }
        public List<Item> GetItems(ItemFilter Filter)
        {
            return GetItems(Service, Filter);
        }

        public static Item GetItem(NetSuiteService Service, ItemFilter Filter)
        {
            List<Item> objItems = null;
            Item objReturn = null;

            try
            {
                objItems = GetItems(Service, Filter);
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

        public static List<Item> GetItems(NetSuiteService Service, ItemFilter Filter)
        {
            List<Item> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<Item>();
                objSearchResult = GetNetSuiteItems(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objNetItem in objSearchResult.recordList)
                        {
                            if (objNetItem is NetSuiteLibrary.com.netsuite.webservices.InventoryItem)
                            {
                                //objReturn.Add(new Item((NetSuiteLibrary.com.netsuite.webservices.InventoryItem)objNetItem));
                                objReturn.Add(new Item((NetSuiteLibrary.com.netsuite.webservices.InventoryItem)objNetItem));
                            }
                            if (objNetItem is NetSuiteLibrary.com.netsuite.webservices.KitItem)
                            {
                                //objReturn.Add(new Item((NetSuiteLibrary.com.netsuite.webservices.KitItem)objNetItem));
                                objReturn.Add(new Item(((NetSuiteLibrary.com.netsuite.webservices.KitItem)objNetItem)));
                            }
                            else if (objNetItem is NetSuiteLibrary.com.netsuite.webservices.NonInventorySaleItem)
                            {
                                objReturn.Add(new Item((NetSuiteLibrary.com.netsuite.webservices.NonInventorySaleItem)objNetItem));
                            }
                            else if (objNetItem is NetSuiteLibrary.com.netsuite.webservices.NonInventoryResaleItem)
                            {
                                objReturn.Add(new Item((NetSuiteLibrary.com.netsuite.webservices.NonInventoryResaleItem)objNetItem));
                            }
                            //else if (objNetItem is NetSuiteLibrary.com.netsuite.webservices.ServiceResaleItem)
                            //{
                            //    objReturn.Add(new Item((NetSuiteLibrary.com.netsuite.webservices.ServiceResaleItem)objNetItem));
                            //}
                            else if (objNetItem is NetSuiteLibrary.com.netsuite.webservices.AssemblyItem)
                            {
                                objReturn.Add(new Item((NetSuiteLibrary.com.netsuite.webservices.AssemblyItem)objNetItem));
                            }
                            else if (objNetItem is NetSuiteLibrary.com.netsuite.webservices.ItemGroup)
                            {
                                objReturn.Add(new Item((NetSuiteLibrary.com.netsuite.webservices.ItemGroup)objNetItem));
                            }
                            //else if (objNetItem is NetSuiteLibrary.com.netsuite.webservices.ServiceSaleItem)
                            //{
                            //    objReturn.Add(new Item((NetSuiteLibrary.com.netsuite.webservices.ServiceSaleItem)objNetItem));
                            //}
                        }
                        Service.tokenPassport = new NetSuiteLibrary.User().TokenPassport();
                        objSearchResult = Service.searchMoreWithId(objSearchResult.searchId, objSearchResult.pageIndex + 1);
                    }
                    while (objSearchResult.pageSizeSpecified == true && objSearchResult.totalPages >= objSearchResult.pageIndex);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSearchResult = null;
            }
            return objReturn;
        }

        private static SearchResult GetNetSuiteItems(NetSuiteService Service, ItemFilter Filter)
        {
            SearchResult objSearchResult = null;
            ItemSearch objItemSearch = null;
            SearchPreferences objSearchPreferences = null;
           
            try
            {
                objItemSearch = new ItemSearch();
                objItemSearch.basic = new ItemSearchBasic();
                objItemSearch.basic.customFieldList = new SearchCustomField[99];
                int intCutstomField = 0;
                if (Filter != null)
                {
                    objItemSearch.basic.isInactive = new SearchBooleanField();
                    objItemSearch.basic.isInactive.searchValue = false;
                    objItemSearch.basic.isInactive.searchValueSpecified = true;

                    if (Filter.ItemInternalIDs != null)
                    {
                        objItemSearch.basic.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.ItemInternalIDs);
                    }

                    if (Filter.ItemNumber != null)
                    {
                        objItemSearch.basic.itemId = Filter.ItemNumber;
                    }

                    if (!string.IsNullOrEmpty(Filter.APIExternalID))
                    {
                        SearchStringCustomField objAPIExternalID = new SearchStringCustomField();
                        objAPIExternalID.scriptId = "custitem_api_external_id";
                        objAPIExternalID.searchValue = Filter.APIExternalID;
                        objAPIExternalID.@operator = SearchStringFieldOperator.@is;
                        objAPIExternalID.operatorSpecified = true;
                        objItemSearch.basic.customFieldList[intCutstomField] = objAPIExternalID;
                        intCutstomField++;
                    }
                    if (Filter.InventoryValidatedOn != null)
                    {
                        if (Filter.InventoryValidatedOnOperator == null) throw new Exception("Inventory validated on operator must be specified");
                        SearchDateCustomField objInventoryValidatedOn = new SearchDateCustomField();
                        objInventoryValidatedOn.scriptId = "custitem_inventory_validated_on";
                        objInventoryValidatedOn.searchValue = Filter.InventoryValidatedOn.Value;
                        objInventoryValidatedOn.searchValueSpecified = true;
                        objInventoryValidatedOn.@operator = Filter.InventoryValidatedOnOperator.Value;
                        objInventoryValidatedOn.operatorSpecified = true;
                        objItemSearch.basic.customFieldList[intCutstomField] = objInventoryValidatedOn;
                        intCutstomField++;
                    }
                    if (Filter.LastModified != null)
                    {
                        objItemSearch.basic.lastModifiedDate = Filter.LastModified;
                    }
                    if (Filter.LastQtyAvailableChanged != null)
                    {
                        objItemSearch.basic.lastQuantityAvailableChange = new SearchDateField();
                        objItemSearch.basic.lastQuantityAvailableChange.searchValue = Filter.LastQtyAvailableChanged.Value;
                        objItemSearch.basic.lastQuantityAvailableChange.searchValueSpecified = true;
                        objItemSearch.basic.lastQuantityAvailableChange.@operator = SearchDateFieldOperator.after;
                        objItemSearch.basic.lastQuantityAvailableChange.operatorSpecified = true;
                    }

                    if (Filter.NetSuiteItemTypes != null && Filter.NetSuiteItemTypes.Count > 0)
                    {
                        objItemSearch.basic.type = new SearchEnumMultiSelectField();
                        objItemSearch.basic.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                        objItemSearch.basic.type.operatorSpecified = true;
                        objItemSearch.basic.type.searchValue = Filter.NetSuiteItemTypes.Select(s => s.ToString()).ToArray();
                    }

                    if (!string.IsNullOrEmpty(Filter.PreferredVendor))
                    {
                        objItemSearch.basic.vendor = new SearchMultiSelectField();
                        objItemSearch.basic.vendor.@operator = SearchMultiSelectFieldOperator.anyOf;
                        objItemSearch.basic.vendor.operatorSpecified = true;
                        objItemSearch.basic.vendor.searchValue = new RecordRef[1];
                        objItemSearch.basic.vendor.searchValue[0] = NetSuiteHelper.GetRecordRef(Filter.PreferredVendor, RecordType.vendor);
                    }

                }

                //RecordRef objRecordRef = new RecordRef();
                //objRecordRef.internalId = GoDirectSubsidiaryID;
                //RecordRef[] objRecordRefs = new RecordRef[1];
                //objRecordRefs[0] = objRecordRef;

                //objItemSearch.basic.subsidiary = new SearchMultiSelectField();
                //objItemSearch.basic.subsidiary.@operator = SearchMultiSelectFieldOperator.anyOf;
                //objItemSearch.basic.subsidiary.operatorSpecified = true;
                //objItemSearch.basic.subsidiary.searchValue = objRecordRefs;

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 250;

                objSearchPreferences.pageSizeSpecified = true;

                //Service.tokenPassport = new NetSuiteLibrary.User().TokenPassport();

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objItemSearch);

                if (objSearchResult.status.isSuccess != true)  throw new Exception("Cannot find Item - " + objSearchResult.status.statusDetail[0].message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemSearch = null;
                objSearchPreferences = null;
            }
            return objSearchResult;
        }
    }
    public enum NetSuiteItemType
    {
        _assembly,
        _description,
        _discount,
        _downloadItem,
        _giftCertificateItem,
        _inventoryItem,
        _itemGroup,
        _kit,
        _markup,
        _nonInventoryItem,
        _otherCharge,
        _payment,
        _service,
        _subtotal,
    }
}


